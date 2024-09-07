using System.Reflection;
using BeatSaberMarkupLanguage.Macros;
using BeatSaberMarkupLanguage.TypeHandlers;

namespace BeatSaberMarkupLanguage
{
    public class MissingAttributeException : BSMLException
    {
        internal MissingAttributeException(BSMLMacro sender, string attributeName)
            : base($"Attribute '{attributeName}' missing for macro {sender.GetType().Name}")
        {
            this.AttributeName = attributeName;
        }

        internal MissingAttributeException(TypeHandler sender, string attributeName)
            : base($"Attribute '{attributeName}' missing for type handler {sender.GetType().GetCustomAttribute<ComponentHandler>()?.Type.Name ?? "<null>"}")
        {
            this.AttributeName = attributeName;
        }

        public string AttributeName { get; }
    }
}
