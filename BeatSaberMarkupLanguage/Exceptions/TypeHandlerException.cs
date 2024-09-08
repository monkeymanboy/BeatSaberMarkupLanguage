using System;
using System.Reflection;
using BeatSaberMarkupLanguage.TypeHandlers;

namespace BeatSaberMarkupLanguage
{
    public class TypeHandlerException : BSMLException
    {
        internal TypeHandlerException(TypeHandler typeHandler, Exception innerException)
            : base($"Failed to handle component {typeHandler.GetType().GetCustomAttribute<ComponentHandler>().Type.Name}", innerException)
        {
        }

        internal TypeHandlerException(TypeHandler typeHandler, string propertyName, Exception innerException)
            : base($"Failed to parse property '{propertyName}' for component {typeHandler.GetType().GetCustomAttribute<ComponentHandler>().Type.Name}", innerException)
        {
        }
    }
}
