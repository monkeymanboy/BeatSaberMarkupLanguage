﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Macros
{
    public class ForEachMacro : BSMLMacro
    {
        public override string[] Aliases => new[] { "for-each" };

        public override Dictionary<string, string[]> Props => new()
        {
            { "hosts", new[] { "hosts", "items" } },
            { "passTags", new[] { "pass-back-tags" } },
        };

        public override void Execute(XElement element, GameObject parent, Dictionary<string, string> data, BSMLParserParams parserParams, out IEnumerable<BSMLParser.ComponentTypeWithData> components)
        {
            components = Enumerable.Empty<BSMLParser.ComponentTypeWithData>();
            if (data.TryGetValue("hosts", out string hosts))
            {
                if (!parserParams.Values.TryGetValue(hosts, out BSMLValue values))
                {
                    throw new ValueNotFoundException(hosts, parserParams.Host);
                }

                bool passTags = false;
                if (data.TryGetValue("passTags", out string passTagsString))
                {
                    passTags = Parse.Bool(passTagsString);
                }

                foreach (object host in values.GetValue() as IEnumerable)
                {
                    BSMLParserParams nodeParams = BSMLParser.instance.Parse(element, parent, host);
                    if (passTags)
                    {
                        nodeParams.PassTaggedObjects(parserParams);
                    }
                }
            }
        }
    }
}
