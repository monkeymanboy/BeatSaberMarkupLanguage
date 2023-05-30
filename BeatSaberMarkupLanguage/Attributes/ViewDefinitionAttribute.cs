using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ViewDefinitionAttribute : Attribute
    {
        public string Definition { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewDefinitionAttribute"/> class.
        /// When applied to a BSMLAutomaticViewController, indicates that it uses the embedded resource <paramref name="definition"/> instead of the default name.
        /// </summary>
        /// <param name="definition">the name of the embedded resource to use.</param>
        public ViewDefinitionAttribute(string definition)
            => Definition = definition;
    }
}
