using BeatSaberMarkupLanguage.Attributes;
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
        private Dictionary<string, BSMLTag> tags = new Dictionary<string, BSMLTag>();
        private List<TypeHandler> typeHandlers;

        private XmlDocument doc = new XmlDocument();
        private XmlReaderSettings readerSettings = new XmlReaderSettings();

        public void Awake()
        {
            readerSettings.IgnoreComments = true;

            foreach (BSMLTag tag in Utilities.GetListOfType<BSMLTag>())
                RegisterTag(tag);

            typeHandlers = Utilities.GetListOfType<TypeHandler>();
        }

        public void RegisterTag(BSMLTag tag)
        {
            foreach (string alias in tag.Aliases)
                tags.Add(alias, tag);
        }

        public void RegisterTypeHandler(TypeHandler typeHandler)
        {
            typeHandlers.Add(typeHandler);
        }

        public BSMLParserParams Parse(string content, GameObject parent, object host = null)
        {
            doc.Load(XmlReader.Create(new StringReader(content), readerSettings));
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

            foreach (XmlNode node in doc.ChildNodes)
                HandleNode(node, parent, parserParams);

            foreach (KeyValuePair<string, BSMLAction> action in parserParams.actions.Where(x => x.Key.StartsWith("#")))
                parserParams.AddEvent(action.Key.Substring(1), delegate { action.Value.Invoke(); });

            parserParams.EmitEvent("post-parse");
            
            return parserParams;
        }

        private GameObject HandleNode(XmlNode node, GameObject parent, BSMLParserParams parserParams)
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
                    componentType.data = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string[]> parameters in typeHandler.Props)
                    {
                        foreach (string alias in parameters.Value)
                        {
                            if (node.Attributes[alias] != null)
                            {
                                string value = node.Attributes[alias].Value;
                                if (value.StartsWith("~"))
                                {
                                    string valueID = value.Substring(1);
                                    if (!parserParams.values.TryGetValue(valueID, out BSMLValue uiValue))
                                        throw new Exception("No UIValue exists with the id '" + valueID + "'");

                                    componentType.data.Add(parameters.Key, uiValue.GetValue().ToString());
                                }
                                else
                                {
                                    componentType.data.Add(parameters.Key, value);
                                }

                                break;
                            }
                            if(alias == "_children")
                            {
                                componentType.data.Add(parameters.Key, node.InnerXml);
                            }
                        }
                    }
                    componentType.typeHandler = typeHandler;
                    componentType.component = component;
                    componentTypes.Add(componentType);
                }
            }
            foreach(ComponentTypeWithData componentType in componentTypes){
                componentType.typeHandler.HandleType(componentType.component, componentType.data, parserParams);
            }

            object host = parserParams.host;
            if (host != null && node.Attributes["id"] != null)
            {
                parserParams.objectsWithID.Add(node.Attributes["id"].Value, currentNode);
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
            if (currentTag.AddChildren)
                foreach (XmlNode childNode in node.ChildNodes)
                    HandleNode(childNode, currentNode, parserParams);

            foreach (ComponentTypeWithData componentType in componentTypes)
            {
                componentType.typeHandler.HandleTypeAfterChildren(componentType.component, componentType.data, parserParams);
            }

            return currentNode;
        }

        internal struct ComponentTypeWithData
        {
            public TypeHandler typeHandler;
            public Component component;
            public Dictionary<string, string> data;
        }
    }
}
