using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace BeatSaberMarkupLanguage
{
    public class BSMLParser : PersistentSingleton<BSMLParser>
    {
        public const float SCREEN_WIDTH = 160;
        public const float SCREEN_HEIGHT = 80;

        //Todo think of better way to deal with these
        private List<BSMLTag> tags = new BSMLTag[] { new ButtonTag(), new VerticalLayoutTag(), new HorizontalLayoutTag(), new TextTag() }.ToList();
        private List<TypeHandler> typeHandlers = new TypeHandler[] { new RectTransformHandler(), new ButtonHandler(), new HorizontalOrVerticalLayoutGroupHandler(), new LayoutGroupHandler(), new TextMeshProUGUIHandler() }.ToList();

        private XmlDocument doc = new XmlDocument();
        public void Parse(string content, GameObject parent, object host = null)
        {
            doc.LoadXml(content);
            Dictionary<string, Action> actions = new Dictionary<string, Action>();
            if (host != null)
            {
                foreach (MethodInfo methodInfo in host.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    UIAction uiaction = methodInfo.GetCustomAttributes(typeof(UIAction), true).FirstOrDefault() as UIAction;
                    if (uiaction != null)
                        actions.Add(uiaction.id, delegate
                        {
                            methodInfo.Invoke(host, new object[] { });
                        });
                }
            }
            foreach(XmlNode node in doc.ChildNodes)
            {
                HandleNode(node, parent, host, actions);
            }
        }

        private GameObject HandleNode(XmlNode node, GameObject parent, object host, Dictionary<string, Action> actions)
        {
            BSMLTag currentTag = tags.First(x => x.Aliases.Contains(node.Name));
            GameObject currentNode = currentTag.CreateObject(parent.transform);
            foreach (TypeHandler typeHandler in typeHandlers)
            {
                Component component = currentNode.GetComponent((typeHandler.GetType().GetCustomAttributes(typeof(ComponentHandler), true).FirstOrDefault() as ComponentHandler).type);
                if (component != null)
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string[]> parameters in typeHandler.Props)
                    {
                        foreach(string alias in parameters.Value)
                        {
                            if (node.Attributes[alias] != null)
                            {
                                data.Add(parameters.Key, node.Attributes[alias].Value);
                                break;
                            }
                        }
                    }
                    typeHandler.HandleType(component, data, actions);
                }
            }
            if(host != null && node.Attributes["id"] != null)
            {
                foreach(FieldInfo fieldInfo in host.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    UIComponent uicomponent = fieldInfo.GetCustomAttributes(typeof(UIComponent), true).FirstOrDefault() as UIComponent;
                    if (uicomponent != null && uicomponent.id == node.Attributes["id"].Value)
                        fieldInfo.SetValue(host, currentNode.GetComponent(fieldInfo.FieldType));
                    UIObject uiobject = fieldInfo.GetCustomAttributes(typeof(UIObject), true).FirstOrDefault() as UIObject;
                    if (uiobject != null && uiobject.id == node.Attributes["id"].Value)
                        fieldInfo.SetValue(host, currentNode);
                }
            }
            foreach (XmlNode childNode in node.ChildNodes)
            {
                HandleNode(childNode, currentNode, host, actions);
            }
            return currentNode;
        }
    }
}
