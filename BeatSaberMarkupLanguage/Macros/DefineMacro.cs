using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Macros
{
    public class DefineMacro : BSMLMacro
    {
        public override string[] Aliases => new[] { "define" };

        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "name", new[]{ "name", "id" } },
            { "value", new[]{ "value" } }
        };

        public override void Execute(XmlNode node, GameObject parent, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            if (!data.TryGetValue("name", out string name))
                throw new Exception("define macro must have an id");
            if (!data.TryGetValue("value", out string value))
                throw new Exception("define macro must have a value");
            if (parserParams.values.TryGetValue(name, out BSMLValue existingValue))
                existingValue.SetValue(value);
            else
                parserParams.values.Add(name, new BSMLStringValue(value));
        }
    }
}
