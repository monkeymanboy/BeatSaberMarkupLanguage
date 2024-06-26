﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
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

        private readonly XmlDocument document = new();
        private readonly XmlReaderSettings readerSettings = new()
        {
            IgnoreComments = true,
        };

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
                Type type = typeHandler.GetType().GetCustomAttribute<ComponentHandler>(true)?.type;
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
                if (!tag.isInitialized)
                {
                    tag.Setup();
                    tag.isInitialized = true;
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
            Type targetType = typeHandlerType.GetCustomAttribute<ComponentHandler>()?.type;

            if (targetType == null)
            {
                Logger.Log.Warn($"TypeHandler {typeHandlerType.FullName} does not have the [{nameof(ComponentHandler)}] attribute and will be ignored.");
                return;
            }

            foreach (TypeHandler otherTypeHandler in typeHandlers.Where(th => th.GetType().GetCustomAttribute<ComponentHandler>().type == targetType))
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
            document.Load(XmlReader.Create(new StringReader(content), readerSettings));
            return Parse(document, parent, host);
        }

        public BSMLParserParams Parse(XmlNode parentNode, GameObject parent, object host = null)
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
                        uiActionName = uiaction.id;
                        if (parserParams.actions.TryGetValue(uiActionName, out BSMLAction existing))
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
                            parserParams.actions.Add(uiActionName, new BSMLAction(host, methodInfo, true));
                        }

                        if (methodAccessOptions == MethodAccessOption.AllowBoth && methodName != uiActionName && !parserParams.actions.ContainsKey(methodName))
                        {
                            parserParams.actions.Add(methodName, new BSMLAction(host, methodInfo, false));
                        }
                    }
                    else if (methodAccessOptions != MethodAccessOption.OptIn)
                    {
                        if (!parserParams.actions.ContainsKey(methodName))
                        {
                            parserParams.actions.Add(methodName, new BSMLAction(host, methodInfo));
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
                        uiValueName = uivalue.id;
                        if (parserParams.values.TryGetValue(uiValueName, out BSMLValue existing))
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
                            parserParams.values.Add(uiValueName, new BSMLFieldValue(host, fieldInfo));
                        }

                        if (fieldAccessOptions == FieldAccessOption.AllowBoth && fieldName != uiValueName && !parserParams.values.ContainsKey(fieldName))
                        {
                            parserParams.values.Add(fieldName, new BSMLFieldValue(host, fieldInfo, false));
                        }
                    }
                    else if (fieldAccessOptions != FieldAccessOption.OptIn && !parserParams.values.ContainsKey(fieldName))
                    {
                        parserParams.values.Add(fieldName, new BSMLFieldValue(host, fieldInfo, false));
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
                        uiValueName = uivalue.id;
                        if (parserParams.values.TryGetValue(uiValueName, out BSMLValue existing))
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
                            parserParams.values.Add(uiValueName, new BSMLPropertyValue(host, propertyInfo));
                        }

                        if (propertyAccessOptions == PropertyAccessOption.AllowBoth && propName != uiValueName && !parserParams.values.ContainsKey(propName))
                        {
                            parserParams.values.Add(propName, new BSMLPropertyValue(host, propertyInfo, false));
                        }
                    }
                    else if (propertyAccessOptions != PropertyAccessOption.OptIn && !parserParams.values.ContainsKey(propName))
                    {
                        parserParams.values.Add(propName, new BSMLPropertyValue(host, propertyInfo, false));
                    }
                }
            }

            IEnumerable<ComponentTypeWithData> componentInfo = Enumerable.Empty<ComponentTypeWithData>();
            foreach (XmlNode node in parentNode.ChildNodes)
            {
                HandleNode(node, parent, parserParams, out IEnumerable<ComponentTypeWithData> components);
                componentInfo = componentInfo.Concat(components);
            }

            foreach (KeyValuePair<string, BSMLAction> action in parserParams.actions.Where(x => x.Key.StartsWith(SubscribeEventActionPrefix, StringComparison.Ordinal)))
            {
                parserParams.AddEvent(action.Key.Substring(1), () => action.Value.Invoke());
            }

            foreach (ComponentTypeWithData component in componentInfo)
            {
                component.typeHandler.HandleTypeAfterParse(component, parserParams);
            }

            parserParams.EmitEvent("post-parse");

            return parserParams;
        }

        public void HandleNode(XmlNode node, GameObject parent, BSMLParserParams parserParams, out IEnumerable<ComponentTypeWithData> componentInfo)
        {
            if (node.Name.StartsWith(MacroPrefix, StringComparison.Ordinal))
            {
                HandleMacroNode(node, parent, parserParams, out componentInfo);
            }
            else
            {
                HandleTagNode(node, parent, parserParams, out componentInfo);
            }
        }

        internal static Component GetExternalComponent(GameObject gameObject, Type type)
        {
            Component component = null;

            if (gameObject.TryGetComponent(out ExternalComponents externalComponents))
            {
                foreach (Component externalComponent in externalComponents.components)
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

        private void HandleTagNode(XmlNode node, GameObject parent, BSMLParserParams parserParams, out IEnumerable<ComponentTypeWithData> componentInfo)
        {
            if (!this.tags.TryGetValue(node.Name, out BSMLTag currentTag))
            {
                throw new TagNotFoundException(node.Name);
            }

            GameObject currentNode = currentTag.CreateObject(parent.transform);

            List<ComponentTypeWithData> componentTypes = new();
            foreach (TypeHandler typeHandler in typeHandlers)
            {
                Type type = typeHandler.GetType().GetCustomAttribute<ComponentHandler>(true)?.type;
                if (type == null)
                {
                    continue;
                }

                Component component = GetExternalComponent(currentNode, type);
                if (component != null)
                {
                    ComponentTypeWithData componentType;
                    componentType.data = GetParameters(node, typeHandler.CachedProps, parserParams, out Dictionary<string, BSMLValue> valueMap);
                    componentType.valueMap = valueMap;
                    componentType.typeHandler = typeHandler;
                    componentType.component = component;
                    componentTypes.Add(componentType);
                }
            }

            foreach (ComponentTypeWithData componentType in componentTypes)
            {
                componentType.typeHandler.HandleType(componentType, parserParams);
            }

            object host = parserParams.host;
            XmlAttribute id = node.Attributes["id"];
            if (host != null && id != null)
            {
                foreach (FieldInfo fieldInfo in host.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (fieldInfo.GetCustomAttribute<UIComponent>(true)?.id == id.Value)
                    {
                        fieldInfo.SetValue(host, GetExternalComponent(currentNode, fieldInfo.FieldType));
                    }

                    if (fieldInfo.GetCustomAttribute<UIObject>(true)?.id == id.Value)
                    {
                        fieldInfo.SetValue(host, currentNode);
                    }
                }
            }

            XmlAttribute tags = node.Attributes["tags"];
            if (tags != null)
            {
                parserParams.AddObjectTags(currentNode, tags.Value.Split(','));
            }

            IEnumerable<ComponentTypeWithData> childrenComponents = Enumerable.Empty<ComponentTypeWithData>();

            if (currentTag.AddChildren)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    HandleNode(childNode, currentNode, parserParams, out IEnumerable<ComponentTypeWithData> children);
                    childrenComponents = childrenComponents.Concat(children);
                }
            }

            foreach (ComponentTypeWithData componentType in componentTypes)
            {
                componentType.typeHandler.HandleTypeAfterChildren(componentType, parserParams);
            }

            componentInfo = componentTypes.Concat(childrenComponents);
        }

        private void HandleMacroNode(XmlNode node, GameObject parent, BSMLParserParams parserParams, out IEnumerable<ComponentTypeWithData> components)
        {
            if (!macros.TryGetValue(node.Name, out BSMLMacro currentMacro))
            {
                throw new MacroNotFoundException(node.Name);
            }

            Dictionary<string, string> properties = GetParameters(node, currentMacro.CachedProps, parserParams, out _);
            currentMacro.Execute(node, parent, properties, parserParams, out components);
        }

        private Dictionary<string, string> GetParameters(XmlNode node, Dictionary<string, string[]> properties, BSMLParserParams parserParams, out Dictionary<string, BSMLValue> valueMap)
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
                    XmlAttribute attribute = node.Attributes[alias];
                    if (attribute != null)
                    {
                        string value = attribute.Value;
                        if (value.StartsWith(RetrieveValuePrefix, StringComparison.Ordinal))
                        {
                            try
                            {
                                string valueID = value.Substring(1);
                                if (!parserParams.values.TryGetValue(valueID, out BSMLValue uiValue) || uiValue == null)
                                {
                                    throw new ValueNotFoundException(valueID, parserParams.host);
                                }

                                parameters.Add(propertyAliases.Key, uiValue.GetValue()?.InvariantToString());
                                valueMap.Add(propertyAliases.Key, uiValue);
                            }
                            catch (Exception)
                            {
                                Logger.Log?.Error($"Error parsing '{propertyAliases.Key}'='{value}' in {parserParams.host.GetTypeFullNameSafe()}");
                                throw;
                            }
                        }
                        else
                        {
                            parameters.Add(propertyAliases.Key, value);
                        }

                        break;
                    }

                    if (alias == "_children")
                    {
                        parameters.Add(propertyAliases.Key, node.InnerXml);
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
            public TypeHandler typeHandler;
            public Component component;
            public Dictionary<string, string> data;
            public Dictionary<string, BSMLValue> valueMap;
        }
    }
}
