using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void Parse(string content, GameObject parent)
        {
            doc.LoadXml(content);
            foreach(XmlNode node in doc.ChildNodes)
            {
                HandleNode(node, parent);
            }
        }

        private GameObject HandleNode(XmlNode node, GameObject parent)
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
                    typeHandler.HandleType(component, data);
                }
            }
            foreach (XmlNode childNode in node.ChildNodes)
            {
                HandleNode(childNode, currentNode);
            }
            return currentNode;
        }
    }
}
