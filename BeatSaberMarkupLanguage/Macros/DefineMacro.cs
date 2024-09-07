using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Macros
{
    public class DefineMacro : BSMLMacro
    {
        public override string[] Aliases => new[] { "define" };

        public override Dictionary<string, string[]> Props => new()
        {
            { "name", new[] { "name", "id" } },
            { "value", new[] { "value" } },
        };

        public override void Execute(XElement element, GameObject parent, Dictionary<string, string> data, BSMLParserParams parserParams, out IEnumerable<BSMLParser.ComponentTypeWithData> components)
        {
            components = Enumerable.Empty<BSMLParser.ComponentTypeWithData>();

            if (!data.TryGetValue("name", out string name))
            {
                throw new MissingAttributeException(this, "name");
            }

            if (!data.TryGetValue("value", out string value))
            {
                throw new MissingAttributeException(this, "value");
            }

            if (parserParams.Values.TryGetValue(name, out BSMLValue existingValue))
            {
                existingValue.SetValue(value);
            }
            else
            {
                parserParams.Values.Add(name, new BSMLStringValue(value, name));
            }
        }
    }
}
