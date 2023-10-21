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

#if DEBUG
using System.Diagnostics;
#endif

namespace BeatSaberMarkupLanguage
{
    public class BSMLParser : PersistentSingleton<BSMLParser>
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
            typeHandlers.Add(typeHandler);
        }

        public BSMLParserParams Parse(string content, GameObject parent, object host = null)
        {
            document.Load(XmlReader.Create(new StringReader(content), readerSettings));
            return Parse(document, parent, host);
        }

        public BSMLParserParams Parse(XmlNode parentNode, GameObject parent, object host = null)
        {
            BSMLParserParams parserParams = new()
            {
                host = host,
            };

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
                    if (methodInfo.GetCustomAttributes(typeof(UIAction), true).FirstOrDefault() is UIAction uiaction)
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
                    if (fieldInfo.GetCustomAttributes(typeof(UIValue), true).FirstOrDefault() is UIValue uivalue)
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

                    if (fieldInfo.GetCustomAttributes(typeof(UIParams), true).FirstOrDefault() is UIParams uiParams)
                    {
                        fieldInfo.SetValue(host, parserParams);
                    }
                }

                foreach (PropertyInfo propertyInfo in host.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    string propName = propertyInfo.Name;
                    string uiValueName = null;
                    if (propertyInfo.GetCustomAttributes(typeof(UIValue), true).FirstOrDefault() is UIValue uivalue)
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

        internal void RegisterAssemblyTags()
        {
#if DEBUG
            Stopwatch stopwatch = Stopwatch.StartNew();
#endif

            foreach (BSMLTag tag in Utilities.GetInstancesOfDescendants<BSMLTag>())
            {
                RegisterTag(tag);
            }

            foreach (BSMLMacro macro in Utilities.GetInstancesOfDescendants<BSMLMacro>())
            {
                RegisterMacro(macro);
            }

            foreach (TypeHandler typeHandler in Utilities.GetInstancesOfDescendants<TypeHandler>())
            {
                Type type = typeHandler.GetType().GetCustomAttribute<ComponentHandler>(true)?.type;

                if (type == null)
                {
                    Logger.Log.Warn($"TypeHandler {typeHandler.GetType().FullName} does not have the [{nameof(ComponentHandler)}] attribute and will be ignored.");
                }
                else
                {
                    typeHandlers.Add(typeHandler);
                }
            }

#if DEBUG
            Logger.Log.Debug("Got all tags in " + stopwatch.Elapsed);
#endif
        }

        internal void SetUpTags()
        {
#if DEBUG
            Stopwatch stopwatch = Stopwatch.StartNew();
#endif

            foreach (BSMLTag tag in tags.Values)
            {
                tag.Setup();
                tag.isInitialized = true;
            }

#if DEBUG
            Logger.Log.Debug("Initialized all tags in " + stopwatch.Elapsed);
#endif

#if false//don't worry about this, it's for the docs
            string contents = "";
            foreach (BSMLTag tag in Utilities.GetListOfType<BSMLTag>())
            {
                tag.Setup();
                contents += $"- type: {tag.GetType().Name}\n";
                contents += $"  aliases:\n";
                foreach (string alias in tag.Aliases)
                    contents += $"  - {alias}\n";
                contents += $"  components:\n";
                GameObject currentNode = tag.CreateObject(transform);
                foreach (TypeHandler typeHandler in typeHandlers)
                {
                    Type type = (typeHandler.GetType().GetCustomAttributes(typeof(ComponentHandler), true).FirstOrDefault() as ComponentHandler).type;
                    if (GetExternalComponent(currentNode, type) != null)
                        contents += $"  - {type.Name}\n";
                }
            }
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "Tags.yml"), contents);
#endif
        }

        private void HandleTagNode(XmlNode node, GameObject parent, BSMLParserParams parserParams, out IEnumerable<ComponentTypeWithData> componentInfo)
        {
            if (!tags.TryGetValue(node.Name, out BSMLTag currentTag))
            {
                throw new TagNotFoundException(node.Name);
            }

            GameObject currentNode = currentTag.CreateObjectInternal(parent.transform);

            List<ComponentTypeWithData> componentTypes = new();
            foreach (TypeHandler typeHandler in typeHandlers)
            {
                Type type = (typeHandler.GetType().GetCustomAttributes(typeof(ComponentHandler), true).FirstOrDefault() as ComponentHandler)?.type;
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
            if (host != null && node.Attributes["id"] != null)
            {
                foreach (FieldInfo fieldInfo in host.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (fieldInfo.GetCustomAttributes(typeof(UIComponent), true).FirstOrDefault() is UIComponent uiComponent && uiComponent.id == node.Attributes["id"].Value)
                    {
                        fieldInfo.SetValue(host, GetExternalComponent(currentNode, fieldInfo.FieldType));
                    }

                    if (fieldInfo.GetCustomAttributes(typeof(UIObject), true).FirstOrDefault() is UIObject uiObject && uiObject.id == node.Attributes["id"].Value)
                    {
                        fieldInfo.SetValue(host, currentNode);
                    }
                }
            }

            if (node.Attributes["tags"] != null)
            {
                parserParams.AddObjectTags(currentNode, node.Attributes["tags"].Value.Split(','));
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

        private Component GetExternalComponent(GameObject gameObject, Type type)
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
                    if (node.Attributes[alias] != null)
                    {
                        string value = node.Attributes[alias].Value;
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
                                Logger.Log?.Error($"Error parsing '{propertyAliases.Key}'='{value}' in {parserParams.host?.GetType().FullName}");
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

        public struct ComponentTypeWithData
        {
            public TypeHandler typeHandler;
            public Component component;
            public Dictionary<string, string> data;
            public Dictionary<string, BSMLValue> valueMap;
        }
    }
}
