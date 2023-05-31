using System.Collections.Generic;
using System.Xml;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Macros
{
    public abstract class BSMLMacro
    {
        private Dictionary<string, string[]> cachedProps;

        public abstract string[] Aliases { get; }

        public Dictionary<string, string[]> CachedProps => cachedProps ??= Props;

        public abstract Dictionary<string, string[]> Props { get; }

        public abstract void Execute(XmlNode node, GameObject parent, Dictionary<string, string> data, BSMLParserParams parserParams, out IEnumerable<BSMLParser.ComponentTypeWithData> childComponentTypes);
    }
}
