using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Macros
{
    public class IfMacro : BSMLMacro
    {
        public override string[] Aliases => new[] { "if" };

        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "value", new[]{"bool","value"} },
        };

        public override void Execute(XmlNode node, GameObject parent, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            if (data.TryGetValue("value", out string valueId))
            {
                bool notOperator = false;
                if (valueId.StartsWith("!"))
                {
                    notOperator = true;
                    valueId = valueId.Substring(1);
                }
                if (!parserParams.values.TryGetValue(valueId, out BSMLValue value))
                    throw new Exception("value '" + valueId + "' not found");

                bool boolValue = (bool) value.GetValue();
                if (boolValue != notOperator)
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        BSMLParser.instance.HandleNode(childNode, parent, parserParams);
                    }
                }
            }
        }
    }
}
