using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Macros
{
    public class IfMacro : BSMLMacro
    {
        public override string[] Aliases => new[] { "if" };

        public override Dictionary<string, string[]> Props => new()
        {
            { "value", new[] { "bool", "value" } },
        };

        public override void Execute(XElement element, GameObject parent, Dictionary<string, string> data, BSMLParserParams parserParams, out IEnumerable<BSMLParser.ComponentTypeWithData> components)
        {
            components = Enumerable.Empty<BSMLParser.ComponentTypeWithData>();
            if (data.TryGetValue("value", out string valueId))
            {
                bool notOperator = valueId.Length > 1 && valueId[0] == '!';
                if (notOperator)
                {
                    valueId = valueId.Substring(1);
                }

                if (!parserParams.Values.TryGetValue(valueId, out BSMLValue value))
                {
                    throw new ValueNotFoundException(valueId, parserParams.Host);
                }

                bool boolValue = (bool)value.GetValue();
                if (boolValue != notOperator)
                {
                    foreach (XElement childElement in element.Elements())
                    {
                        BSMLParser.instance.HandleNode(childElement, parent, parserParams, out IEnumerable<BSMLParser.ComponentTypeWithData> children);
                        components = components.Concat(children);
                    }
                }
            }
        }
    }
}
