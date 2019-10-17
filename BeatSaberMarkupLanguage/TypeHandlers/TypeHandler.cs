using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    public abstract class TypeHandler
    {
        public abstract Dictionary<string, string[]> Props { get; }

        public virtual void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams) { }
        public virtual void HandleTypeAfterChildren(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams) { }
    }

    public class ComponentHandler : Attribute
    {
        public Type type;

        public ComponentHandler(Type type)
        {
            this.type = type;
        }
    }
}
