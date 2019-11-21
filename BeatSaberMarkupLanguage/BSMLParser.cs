using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Macros;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace BeatSaberMarkupLanguage
{
    public class BSMLParser : PersistentSingleton<BSMLParser>
    {
        internal static string MACRO_PREFIX => "macro.";
        internal static string RETRIEVE_VALUE_PREFIX => "~";
        internal static string SUBSCRIVE_EVENT_ACTION_PREFIX => "#";

        private Dictionary<string, BSMLTag> tags = new Dictionary<string, BSMLTag>();
        private Dictionary<string, BSMLMacro> macros = new Dictionary<string, BSMLMacro>();
        private List<TypeHandler> typeHandlers;

        private XmlDocument doc = new XmlDocument();
        private XmlReaderSettings readerSettings = new XmlReaderSettings();

        public void Awake()
        {
            readerSettings.IgnoreComments = true;

            foreach (BSMLTag tag in Utilities.GetListOfType<BSMLTag>())
                RegisterTag(tag);

            foreach (BSMLMacro macro in Utilities.GetListOfType<BSMLMacro>())
                RegisterMacro(macro);

            typeHandlers = Utilities.GetListOfType<TypeHandler>();
        }

        public void RegisterTag(BSMLTag tag)
        {
            foreach (string alias in tag.Aliases)
                tags.Add(alias, tag);
        }
        public void RegisterMacro(BSMLMacro macro)
        {
            foreach (string alias in macro.Aliases)
                macros.Add(MACRO_PREFIX + alias, macro);
        }

        public void RegisterTypeHandler(TypeHandler typeHandler)
        {
            typeHandlers.Add(typeHandler);
        }

        public BSMLParserParams Parse(string content, GameObject parent, object host = null)
        {
            doc.Load(XmlReader.Create(new StringReader(content), readerSettings));
            return Parse(doc, parent, host);
        }

        public BSMLParserParams Parse(XmlNode parentNode, GameObject parent, object host = null)
        {
            BSMLParserParams parserParams = new BSMLParserParams();
            parserParams.host = host;
            if (host != null)
            {
                foreach (MethodInfo methodInfo in host.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    UIAction uiaction = methodInfo.GetCustomAttributes(typeof(UIAction), true).FirstOrDefault() as UIAction;
                    if (uiaction != null)
                        parserParams.actions.Add(uiaction.id, new BSMLAction(host, methodInfo));
                }

                foreach (FieldInfo fieldInfo in host.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    UIValue uivalue = fieldInfo.GetCustomAttributes(typeof(UIValue), true).FirstOrDefault() as UIValue;
                    if (uivalue != null)
                        parserParams.values.Add(uivalue.id, new BSMLFieldValue(host, fieldInfo));

                    UIParams uiParams = fieldInfo.GetCustomAttributes(typeof(UIParams), true).FirstOrDefault() as UIParams;
                    if (uiParams != null)
                        fieldInfo.SetValue(host, parserParams);
                }

                foreach (PropertyInfo propertyInfo in host.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    UIValue uivalue = propertyInfo.GetCustomAttributes(typeof(UIValue), true).FirstOrDefault() as UIValue;
                    if (uivalue != null)
                        parserParams.values.Add(uivalue.id, new BSMLPropertyValue(host, propertyInfo));
                }
            }

            foreach (XmlNode node in parentNode.ChildNodes)
                HandleNode(node, parent, parserParams);

            foreach (KeyValuePair<string, BSMLAction> action in parserParams.actions.Where(x => x.Key.StartsWith(SUBSCRIVE_EVENT_ACTION_PREFIX)))
                parserParams.AddEvent(action.Key.Substring(1), delegate { action.Value.Invoke(); });

            parserParams.EmitEvent("post-parse");

            return parserParams;
        }

        public void HandleNode(XmlNode node, GameObject parent, BSMLParserParams parserParams)
        {
            if (node.Name.StartsWith(MACRO_PREFIX))
                HandleMacroNode(node, parent, parserParams);
            else
                HandleTagNode(node, parent, parserParams);
        }
        private void HandleTagNode(XmlNode node, GameObject parent, BSMLParserParams parserParams)
        {
            if (!tags.TryGetValue(node.Name, out BSMLTag currentTag))
                throw new Exception("Tag type '" + node.Name + "' not found");

            GameObject currentNode = currentTag.CreateObject(parent.transform);
            List<ComponentTypeWithData> componentTypes = new List<ComponentTypeWithData>();
            foreach (TypeHandler typeHandler in typeHandlers)
            {
                Component component = currentNode.GetComponent((typeHandler.GetType().GetCustomAttributes(typeof(ComponentHandler), true).FirstOrDefault() as ComponentHandler).type);
                if (component != null)
                {
                    ComponentTypeWithData componentType = new ComponentTypeWithData();
                    componentType.data = GetParameters(node, typeHandler.Props, parserParams);
                    componentType.typeHandler = typeHandler;
                    componentType.component = component;
                    componentTypes.Add(componentType);
                }
            }
            foreach (ComponentTypeWithData componentType in componentTypes)
            {
                componentType.typeHandler.HandleType(componentType.component, componentType.data, parserParams);
            }

            object host = parserParams.host;
            if (host != null && node.Attributes["id"] != null)
            {
                foreach (FieldInfo fieldInfo in host.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    UIComponent uicomponent = fieldInfo.GetCustomAttributes(typeof(UIComponent), true).FirstOrDefault() as UIComponent;
                    if (uicomponent != null && uicomponent.id == node.Attributes["id"].Value)
                        fieldInfo.SetValue(host, currentNode.GetComponent(fieldInfo.FieldType));

                    UIObject uiobject = fieldInfo.GetCustomAttributes(typeof(UIObject), true).FirstOrDefault() as UIObject;
                    if (uiobject != null && uiobject.id == node.Attributes["id"].Value)
                        fieldInfo.SetValue(host, currentNode);
                }
            }
            if (host != null && node.Attributes["tags"] != null)
                parserParams.AddObjectTags(currentNode, node.Attributes["tags"].Value.Split(','));

            if (currentTag.AddChildren)
                foreach (XmlNode childNode in node.ChildNodes)
                    HandleNode(childNode, currentNode, parserParams);

            foreach (ComponentTypeWithData componentType in componentTypes)
            {
                componentType.typeHandler.HandleTypeAfterChildren(componentType.component, componentType.data, parserParams);
            }
        }
        private void HandleMacroNode(XmlNode node, GameObject parent, BSMLParserParams parserParams)
        {
            if (!macros.TryGetValue(node.Name, out BSMLMacro currentMacro))
                throw new Exception("Macro type '" + node.Name + "' not found");

            Dictionary<string, string> properties = GetParameters(node, currentMacro.Props, parserParams);
            currentMacro.Execute(node, parent, properties, parserParams);
        }

        private Dictionary<string, string> GetParameters(XmlNode node, Dictionary<string, string[]> properties, BSMLParserParams parserParams)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            bool isNotifyHost = typeof(Notify.INotifiableHost).IsAssignableFrom(parserParams.host.GetType());
            foreach (KeyValuePair<string, string[]> propertyAliases in properties)
            {
                foreach (string alias in propertyAliases.Value)
                {
                    if (node.Attributes[alias] != null)
                    {
                        string value = node.Attributes[alias].Value;
                        if (value.StartsWith(RETRIEVE_VALUE_PREFIX))
                        {
                            string valueID = value.Substring(1);
                            if (!parserParams.values.TryGetValue(valueID, out BSMLValue uiValue))
                                throw new Exception("No UIValue exists with the id '" + valueID + "'");
                            parameters.Add(propertyAliases.Key, uiValue.GetValue()?.ToString());
                            if (isNotifyHost && uiValue is BSMLPropertyValue propVal)
                                if (propVal != null)
                                    parserParams.propertyMap.Add(propertyAliases.Key, propVal.propertyInfo);
                                else
                                    Logger.log?.Warn($"PropertyValue is null for {propertyAliases.Key}");
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

        internal struct ComponentTypeWithData
        {
            public TypeHandler typeHandler;
            public Component component;
            public Dictionary<string, string> data;
        }
    }
}
