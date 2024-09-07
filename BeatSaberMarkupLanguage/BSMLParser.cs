using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Exceptions;
using BeatSaberMarkupLanguage.Macros;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using BeatSaberMarkupLanguage.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Zenject;

namespace BeatSaberMarkupLanguage
{
    public class BSMLParser : PersistentSingleton<BSMLParser>, IInitializable
    {
        internal static readonly string MacroPrefix = "macro.";
        internal static readonly string RetrieveValuePrefix = "~";
        internal static readonly string SubscribeEventActionPrefix = "#";

        private readonly Dictionary<string, BSMLTag> tags = new();
        private readonly Dictionary<string, BSMLMacro> macros = new();
        private readonly List<TypeHandler> typeHandlers = new();

        public BSMLParser()
        {
            foreach (BSMLTag tag in Utilities.GetInstancesOfDescendants<BSMLTag>())
            {
                RegisterTag(tag);
            }

            foreach (BSMLMacro macro in Utilities.GetInstancesOfDescendants<BSMLMacro>())
            {
                RegisterMacro(macro);
            }

            typeHandlers.AddRange(Utilities.GetInstancesOfDescendants<TypeHandler>());
            foreach (TypeHandler typeHandler in typeHandlers.ToArray())
            {
                Type type = typeHandler.GetType().GetCustomAttribute<ComponentHandler>(true)?.Type;
                if (type == null)
                {
                    Logger.Log.Warn($"TypeHandler {typeHandler.GetType().FullName} does not have the [ComponentHandler] attribute and will be ignored.");
                    typeHandlers.Remove(typeHandler);
                }
            }
        }

        public void Initialize()
        {
            foreach (BSMLTag tag in tags.Values.Distinct())
            {
#pragma warning disable CS0612, CS0618
                if (!tag.IsInitialized)
                {
                    tag.Setup();
                    tag.IsInitialized = true;
                }
#pragma warning restore CS0612, CS0618

                tag.Initialize();
            }

#if DEBUG
            if (Environment.GetCommandLineArgs().Contains("--bsml-generate-documentation"))
            {
                IEnumerable<Type> typeHandlers = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TypeHandler)));
                new DocumentationDataGenerator(typeHandlers).Generate();
            }
#endif
        }

        public void RegisterTag(BSMLTag tag)
        {
            foreach (string alias in tag.Aliases)
            {
                tags.Add(alias, tag);
            }
        }

        public void RegisterMacro(BSMLMacro macro)
        {
            foreach (string alias in macro.Aliases)
            {
                macros.Add(MacroPrefix + alias, macro);
            }
        }

        public void RegisterTypeHandler(TypeHandler typeHandler)
        {
            if (typeHandler == null)
            {
                throw new ArgumentNullException(nameof(typeHandler));
            }

            Type typeHandlerType = typeHandler.GetType();
            Type targetType = typeHandlerType.GetCustomAttribute<ComponentHandler>()?.Type;

            if (targetType == null)
            {
                Logger.Log.Warn($"TypeHandler {typeHandlerType.FullName} does not have the [{nameof(ComponentHandler)}] attribute and will be ignored.");
                return;
            }

            foreach (TypeHandler otherTypeHandler in typeHandlers.Where(th => th.GetType().GetCustomAttribute<ComponentHandler>().Type == targetType))
            {
                List<string> conflictingProps = typeHandler.Props.Keys.Intersect(otherTypeHandler.Props.Keys).ToList();

                if (conflictingProps.Count > 0)
                {
                    Logger.Log.Warn($"Registering type handler {typeHandlerType.FullName} for type {targetType.FullName} that has conflicting properties [{string.Join(", ", conflictingProps)}] with {otherTypeHandler.GetType().FullName}! This may lead to unexpected behaviour.");
                }
                else
                {
                    Logger.Log.Warn($"Registering type handler {typeHandlerType.FullName} for type {targetType.FullName} that is already handled by {otherTypeHandler.GetType().FullName}! This may lead to unexpected behaviour in the future if type handlers share attributes with the same name.");
                }
            }

            typeHandlers.Add(typeHandler);
        }

        public BSMLParserParams Parse(string content, GameObject parent, object host = null)
        {
            XDocument document = XDocument.Parse(content, LoadOptions.SetLineInfo);
            return Parse(document, parent, host);
        }

        public BSMLParserParams Parse(XContainer container, GameObject parent, object host = null)
        {
            if (!IsMainMenuSceneLoaded())
            {
                return null;
            }

            BSMLParserParams parserParams = new(host);

            FieldAccessOption fieldAccessOptions = FieldAccessOption.Auto;
            PropertyAccessOption propertyAccessOptions = PropertyAccessOption.Auto;
            MethodAccessOption methodAccessOptions = MethodAccessOption.Auto;
            HostOptionsAttribute hostOptions = host?.GetType().GetCustomAttribute<HostOptionsAttribute>();
            if (hostOptions != null)
            {
                fieldAccessOptions = hostOptions.FieldAccessOption;
                propertyAccessOptions = hostOptions.PropertyAccessOption;
                methodAccessOptions = hostOptions.MethodAccessOption;
            }

            if (host != null)
            {
                foreach (MethodInfo methodInfo in host.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    string methodName = methodInfo.Name;
                    string uiActionName = null;
                    if (methodInfo.GetCustomAttribute<UIAction>(true) is UIAction uiaction)
                    {
                        uiActionName = uiaction.Id;
                        if (parserParams.Actions.TryGetValue(uiActionName, out BSMLAction existing))
                        {
                            if (existing.FromUIAction)
                            {
                                throw new InvalidOperationException($"UIAction '{uiActionName}' is already used by member '{existing.MemberName}'.");
                            }

                            existing.MethodInfo = methodInfo;
                            existing.FromUIAction = true;
                        }
                        else
                        {
                            parserParams.Actions.Add(uiActionName, new BSMLAction(host, methodInfo, true));
                        }

                        if (methodAccessOptions == MethodAccessOption.AllowBoth && methodName != uiActionName && !parserParams.Actions.ContainsKey(methodName))
                        {
                            parserParams.Actions.Add(methodName, new BSMLAction(host, methodInfo, false));
                        }
                    }
                    else if (methodAccessOptions != MethodAccessOption.OptIn)
                    {
                        if (!parserParams.Actions.ContainsKey(methodName))
                        {
                            parserParams.Actions.Add(methodName, new BSMLAction(host, methodInfo));
                        }
                    }
                }

                // TODO: Figure out a way to prioritize [UIValue] attributes across both fields and properties.
                // If a field has the same name as a UIValue on a property, the field will take precedence. This is usually not the expected behavior.
                foreach (FieldInfo fieldInfo in host.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    string fieldName = fieldInfo.Name;
                    string uiValueName = null;
                    if (fieldInfo.GetCustomAttribute<UIValue>(true) is UIValue uivalue)
                    {
                        uiValueName = uivalue.Id;
                        if (parserParams.Values.TryGetValue(uiValueName, out BSMLValue existing))
                        {
                            if (existing.FromUIValue)
                            {
                                throw new InvalidOperationException($"UIValue '{uiValueName}' is already used by member '{existing.MemberName}'.");
                            }

                            if (existing is BSMLFieldValue existingField)
                            {
                                existingField.FieldInfo = fieldInfo;
                                existingField.FromUIValue = true;
                            }
                        }
                        else
                        {
                            parserParams.Values.Add(uiValueName, new BSMLFieldValue(host, fieldInfo));
                        }

                        if (fieldAccessOptions == FieldAccessOption.AllowBoth && fieldName != uiValueName && !parserParams.Values.ContainsKey(fieldName))
                        {
                            parserParams.Values.Add(fieldName, new BSMLFieldValue(host, fieldInfo, false));
                        }
                    }
                    else if (fieldAccessOptions != FieldAccessOption.OptIn && !parserParams.Values.ContainsKey(fieldName))
                    {
                        parserParams.Values.Add(fieldName, new BSMLFieldValue(host, fieldInfo, false));
                    }

                    if (fieldInfo.GetCustomAttribute<UIParams>(true) != null)
                    {
                        fieldInfo.SetValue(host, parserParams);
                    }
                }

                foreach (PropertyInfo propertyInfo in host.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    string propName = propertyInfo.Name;
                    string uiValueName = null;
                    if (propertyInfo.GetCustomAttribute<UIValue>(true) is UIValue uivalue)
                    {
                        uiValueName = uivalue.Id;
                        if (parserParams.Values.TryGetValue(uiValueName, out BSMLValue existing))
                        {
                            if (existing.FromUIValue)
                            {
                                throw new InvalidOperationException($"UIValue '{uiValueName}' is already used by member '{existing.MemberName}'.");
                            }

                            if (existing is BSMLPropertyValue existingProp)
                            {
                                existingProp.PropertyInfo = propertyInfo;
                                existingProp.FromUIValue = true;
                            }
                        }
                        else
                        {
                            parserParams.Values.Add(uiValueName, new BSMLPropertyValue(host, propertyInfo));
                        }

                        if (propertyAccessOptions == PropertyAccessOption.AllowBoth && propName != uiValueName && !parserParams.Values.ContainsKey(propName))
                        {
                            parserParams.Values.Add(propName, new BSMLPropertyValue(host, propertyInfo, false));
                        }
                    }
                    else if (propertyAccessOptions != PropertyAccessOption.OptIn && !parserParams.Values.ContainsKey(propName))
                    {
                        parserParams.Values.Add(propName, new BSMLPropertyValue(host, propertyInfo, false));
                    }
                }
            }

            IEnumerable<ComponentTypeWithData> componentInfo = Enumerable.Empty<ComponentTypeWithData>();
            foreach (XElement element in container.Elements())
            {
                HandleNode(element, parent, parserParams, out IEnumerable<ComponentTypeWithData> components);
                componentInfo = componentInfo.Concat(components);
            }

            foreach (KeyValuePair<string, BSMLAction> action in parserParams.Actions.Where(x => x.Key.StartsWith(SubscribeEventActionPrefix, StringComparison.Ordinal)))
            {
                parserParams.AddEvent(action.Key.Substring(1), () => action.Value.Invoke());
            }

            foreach (ComponentTypeWithData component in componentInfo)
            {
                try
                {
                    component.TypeHandler.HandleTypeAfterParse(component, parserParams);
                }
                catch (Exception ex)
                {
                    throw new TypeHandlerException(component.TypeHandler, ex);
                }
            }

            parserParams.EmitEvent("post-parse");

            return parserParams;
        }

        public void HandleNode(XElement element, GameObject parent, BSMLParserParams parserParams, out IEnumerable<ComponentTypeWithData> componentInfo)
        {
            try
            {
                if (element.Name.LocalName.StartsWith(MacroPrefix, StringComparison.Ordinal))
                {
                    HandleMacroNode(element, parent, parserParams, out componentInfo);
                }
                else
                {
                    HandleTagNode(element, parent, parserParams, out componentInfo);
                }
            }
            catch (Exception ex) when (ex is not BSMLParserException)
            {
                throw new BSMLParserException(element, ex);
            }
        }

        internal static Component GetExternalComponent(GameObject gameObject, Type type)
        {
            Component component = null;

            if (gameObject.TryGetComponent(out ExternalComponents externalComponents))
            {
                foreach (Component externalComponent in externalComponents.Components)
                {
                    if (type.IsAssignableFrom(externalComponent.GetType()))
                    {
                        component = externalComponent;
                    }
                }
            }

            if (component == null)
            {
                component = gameObject.GetComponent(type);
            }

            return component;
        }

        private void HandleTagNode(XElement element, GameObject parent, BSMLParserParams parserParams, out IEnumerable<ComponentTypeWithData> componentInfo)
        {
            if (!this.tags.TryGetValue(element.Name.LocalName, out BSMLTag currentTag))
            {
                throw new TagNotFoundException(element.Name.LocalName);
            }

            GameObject currentNode = currentTag.CreateObject(parent.transform);

            List<ComponentTypeWithData> componentTypes = new();
            foreach (TypeHandler typeHandler in typeHandlers)
            {
                Type type = typeHandler.GetType().GetCustomAttribute<ComponentHandler>(true)?.Type;
                if (type == null)
                {
                    continue;
                }

                Component component = GetExternalComponent(currentNode, type);
                if (component != null)
                {
                    ComponentTypeWithData componentType;
                    componentType.Data = GetParameters(element, typeHandler.CachedProps, parserParams, out Dictionary<string, BSMLValue> valueMap);
                    componentType.ValueMap = valueMap;
                    componentType.TypeHandler = typeHandler;
                    componentType.Component = component;
                    componentTypes.Add(componentType);
                }
            }

            foreach (ComponentTypeWithData componentType in componentTypes)
            {
                try
                {
                    componentType.TypeHandler.HandleType(componentType, parserParams);
                }
                catch (Exception ex) when (ex is not TypeHandlerException)
                {
                    throw new TypeHandlerException(componentType.TypeHandler, ex);
                }
            }

            object host = parserParams.Host;
            XAttribute id = element.Attribute("id");
            if (host != null && id != null)
            {
                foreach (FieldInfo fieldInfo in host.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (fieldInfo.GetCustomAttribute<UIComponent>(true)?.Id == id.Value)
                    {
                        fieldInfo.SetValue(host, GetExternalComponent(currentNode, fieldInfo.FieldType));
                    }

                    if (fieldInfo.GetCustomAttribute<UIObject>(true)?.Id == id.Value)
                    {
                        fieldInfo.SetValue(host, currentNode);
                    }
                }
            }

            XAttribute tags = element.Attribute("tags");
            if (tags != null)
            {
                parserParams.AddObjectTags(currentNode, tags.Value.Split(','));
            }

            IEnumerable<ComponentTypeWithData> childrenComponents = Enumerable.Empty<ComponentTypeWithData>();

            if (currentTag.AddChildren)
            {
                foreach (XElement childElement in element.Elements())
                {
                    HandleNode(childElement, currentNode, parserParams, out IEnumerable<ComponentTypeWithData> children);
                    childrenComponents = childrenComponents.Concat(children);
                }
            }

            foreach (ComponentTypeWithData componentType in componentTypes)
            {
                try
                {
                    componentType.TypeHandler.HandleTypeAfterChildren(componentType, parserParams);
                }
                catch (Exception ex)
                {
                    throw new TypeHandlerException(componentType.TypeHandler, ex);
                }
            }

            componentInfo = componentTypes.Concat(childrenComponents);
        }

        private void HandleMacroNode(XElement element, GameObject parent, BSMLParserParams parserParams, out IEnumerable<ComponentTypeWithData> components)
        {
            if (!macros.TryGetValue(element.Name.LocalName, out BSMLMacro currentMacro))
            {
                throw new MacroNotFoundException(element.Name.LocalName);
            }

            Dictionary<string, string> properties = GetParameters(element, currentMacro.CachedProps, parserParams, out _);
            currentMacro.Execute(element, parent, properties, parserParams, out components);
        }

        private Dictionary<string, string> GetParameters(XElement element, Dictionary<string, string[]> properties, BSMLParserParams parserParams, out Dictionary<string, BSMLValue> valueMap)
        {
            Dictionary<string, string> parameters = new();
            valueMap = new Dictionary<string, BSMLValue>();
            foreach (KeyValuePair<string, string[]> propertyAliases in properties)
            {
                List<string> aliasList = new(propertyAliases.Value);
                if (!aliasList.Contains(propertyAliases.Key))
                {
                    aliasList.Add(propertyAliases.Key);
                }

                foreach (string alias in aliasList)
                {
                    XAttribute attribute = element.Attribute(alias);
                    if (attribute != null)
                    {
                        string value = attribute.Value;
                        if (value.StartsWith(RetrieveValuePrefix, StringComparison.Ordinal))
                        {
                            string valueID = value.Substring(1);
                            if (!parserParams.Values.TryGetValue(valueID, out BSMLValue uiValue) || uiValue == null)
                            {
                                throw new ValueNotFoundException(valueID, parserParams.Host);
                            }

                            parameters.Add(propertyAliases.Key, uiValue.GetValue()?.InvariantToString());
                            valueMap.Add(propertyAliases.Key, uiValue);
                        }
                        else
                        {
                            parameters.Add(propertyAliases.Key, value);
                        }

                        break;
                    }

                    if (alias == "_children")
                    {
                        using XmlReader reader = element.CreateReader();
                        reader.MoveToContent();
                        parameters.Add(propertyAliases.Key, reader.ReadInnerXml());
                    }
                }
            }

            return parameters;
        }

        private bool IsMainMenuSceneLoaded()
        {
            // Unity tends to unload all asset bundles before calling OnDestroy on everything when shutting down.
            // Since scenes are now loaded through Addressables, we need to make sure asset bundles haven't been
            // unloaded before parsing. Without this check, weird instantiation errors can pop up.
            AsyncOperationHandle instance = Addressables.Instance.m_SceneInstances.FirstOrDefault(si => ((SceneInstance)si.Result).Scene.name == "MainMenu");
            return instance.IsValid() && ((SceneProvider.SceneOp)instance.m_InternalOp).m_DepOp.Result.Select(op => op.Result).OfType<AssetBundleResource>().All(ab => ab.m_AssetBundle != null);
        }

        public struct ComponentTypeWithData
        {
            public TypeHandler TypeHandler;
            public Component Component;
            public Dictionary<string, string> Data;
            public Dictionary<string, BSMLValue> ValueMap;
        }
    }
}
