using BeatSaberMarkupLanguage.Parser;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Macros
{
    public abstract class BSMLMacro
    {
        public abstract string[] Aliases { get; }
        public abstract Dictionary<string, string[]> Props { get; }
        public abstract void Execute(XmlNode node, GameObject parent, Dictionary<string, string> data, BSMLParserParams parserParams);
    }
}
