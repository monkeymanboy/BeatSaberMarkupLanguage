﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Macros
{
    public class AsHostMacro : BSMLMacro
    {
        public override string[] Aliases => new[] { "as-host" };

        public override Dictionary<string, string[]> Props => new()
        {
            { "host", new[] { "host" } },
        };

        public override void Execute(XElement element, GameObject parent, Dictionary<string, string> data, BSMLParserParams parserParams, out IEnumerable<BSMLParser.ComponentTypeWithData> components)
        {
            components = Enumerable.Empty<BSMLParser.ComponentTypeWithData>();
            if (data.TryGetValue("host", out string host))
            {
                if (!parserParams.Values.TryGetValue(host, out BSMLValue value))
                {
                    throw new ValueNotFoundException(host, parserParams.Host);
                }

                BSMLParser.instance.Parse(element, parent, value.GetValue());
            }
        }
    }
}
