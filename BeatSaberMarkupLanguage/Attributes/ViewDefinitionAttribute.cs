using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ViewDefinitionAttribute : Attribute
    {
        public string Definition { get; }

        /// <summary>
        /// When applied to a BSMLAutomaticViewController, indicates that it uses the embedded resource
        /// <paramref name="definition"/> instead of the default name.
        /// </summary>
        /// <param name="definition">the name of the embedded resource to use</param>
        public ViewDefinitionAttribute(string definition)
            => Definition = definition;
    }
}
