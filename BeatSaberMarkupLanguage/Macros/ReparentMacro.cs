using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Macros
{
    public class ReparentMacro : BSMLMacro
    {
        public override string[] Aliases => new[] { "reparent" };

        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "transform", new[]{"transform"} },
        };

        public override void Execute(XmlNode node, GameObject parent, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            if (data.TryGetValue("transform", out string transformId))
            {
                if (!parserParams.values.TryGetValue(transformId, out BSMLValue value))
                    throw new Exception("transform '" + transformId + "' not found");
                (value.GetValue() as Transform).SetParent(parent.transform, false);
            }
        }
    }
}
