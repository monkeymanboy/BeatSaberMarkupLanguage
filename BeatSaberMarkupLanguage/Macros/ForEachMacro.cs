using System;
using System.Collections.Generic;
using System.Xml;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Macros
{
    public class ForEachMacro : BSMLMacro
    {
        public override string[] Aliases => new[] { "for-each" };

        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "hosts", new[]{"hosts","items"} },
            { "passTags", new[]{"pass-back-tags"}}
        };
        
        public override void Execute(XmlNode node, GameObject parent, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            if (data.TryGetValue("hosts", out string hosts))
            {
                if (!parserParams.values.TryGetValue(hosts, out BSMLValue values))
                    throw new Exception("host list '" + hosts + "' not found");
                bool passTags = false;
                if (data.TryGetValue("passTags", out string passTagsString))
                    passTags = Parse.Bool(passTagsString);
                foreach(object host in values.GetValue() as List<object>)
                {
                    BSMLParserParams nodeParams = BSMLParser.instance.Parse(node, parent, host);
                    if (passTags)
                        nodeParams.PassTaggedObjects(parserParams);
                }
            }
        }
    }
}
