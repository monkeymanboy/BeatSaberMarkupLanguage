using System;

namespace BeatSaberMarkupLanguage.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class HostOptionsAttribute : Attribute
    {
        public FieldAccessOption FieldAccessOption { get; }
        public PropertyAccessOption PropertyAccessOption { get; }
        public MethodAccessOption MethodAccessOption { get; }
        public HostOptionsAttribute(FieldAccessOption fieldAccessOption = FieldAccessOption.Auto, PropertyAccessOption propertyAccessOption = PropertyAccessOption.Auto,
            MethodAccessOption methodAccessOption = MethodAccessOption.Auto)
        {
            FieldAccessOption = fieldAccessOption;
            PropertyAccessOption = propertyAccessOption;
            MethodAccessOption = methodAccessOption;
        }
    }

    public enum FieldAccessOption
    {
        /// <summary>
        /// Fields not marked with <see cref="UIValue"/> can be used in BSML files using their property name.
        /// If a <see cref="UIValue"/> has the same name as an unmarked field, the <see cref="UIValue"/> will be used.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Only fields marked with <see cref="UIValue"/> can by used in BSML files.
        /// </summary>
        OptIn = 1,
        /// <summary>
        /// Fields marked with <see cref="UIValue"/> can be accessed by both the <see cref="UIValue.id"/> and their field name.
        /// </summary>
        AllowBoth = 2
    }
    public enum PropertyAccessOption
    {
        /// <summary>
        /// Properties not marked with <see cref="UIValue"/> can be used in BSML files using their property name.
        /// If a <see cref="UIValue"/> has the same name as an unmarked property, the <see cref="UIValue"/> will be used.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Only properties marked with <see cref="UIValue"/> can by used in BSML files.
        /// </summary>
        OptIn = 1,
        /// <summary>
        /// Properties marked with <see cref="UIValue"/> can be accessed by both the <see cref="UIValue.id"/> and their property name.
        /// </summary>
        AllowBoth = 2
    }
    public enum MethodAccessOption
    {
        /// <summary>
        /// Methods not marked with <see cref="UIAction"/> can be used in BSML files using their method name.
        /// If a <see cref="UIAction"/> has the same name as an unmarked method, the <see cref="UIAction"/> will be used.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Only methods marked with <see cref="UIAction"/> can by used in BSML files.
        /// </summary>
        OptIn = 1,
        /// <summary>
        /// Methods marked with <see cref="UIAction"/> can be accessed by both the <see cref="UIAction.id"/> and their method name.
        /// </summary>
        AllowBoth = 2
    }
}
