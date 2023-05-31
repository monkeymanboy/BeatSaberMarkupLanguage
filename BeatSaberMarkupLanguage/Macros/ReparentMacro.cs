using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Macros
{
    public class ReparentMacro : BSMLMacro
    {
        public override string[] Aliases => new[] { "reparent" };

        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "transform", new[] { "transform" } },
            { "transforms", new[] { "transforms" } },
        };

        public override void Execute(XmlNode node, GameObject parent, Dictionary<string, string> data, BSMLParserParams parserParams, out IEnumerable<BSMLParser.ComponentTypeWithData> components)
        {
            components = Enumerable.Empty<BSMLParser.ComponentTypeWithData>();
            if (data.TryGetValue("transform", out string transformId))
            {
                if (!parserParams.values.TryGetValue(transformId, out BSMLValue value))
                {
                    throw new Exception("transform '" + transformId + "' not found");
                }

                ((Transform)value.GetValue()).SetParent(parent.transform, false);
            }

            if (data.TryGetValue("transforms", out string transformsId))
            {
                if (!parserParams.values.TryGetValue(transformsId, out BSMLValue value))
                {
                    throw new Exception("transform list '" + transformsId + "' not found");
                }

                foreach (Transform transform in value.GetValue() as List<Transform>)
                {
                    transform.SetParent(parent.transform, false);
                }
            }
        }
    }
}
