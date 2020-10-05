using Polyglot;
using System.Collections.Generic;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace BeatSaberMarkupLanguage
{
    public class Config
    {
        [UseConverter(typeof(EnumConverter<Language>))]
        public virtual Language SelectedLanguage { get; set; }

        [NonNullable, UseConverter(typeof(ListConverter<string>))]
        public virtual List<string> PinnedMods { get; set; } = new List<string>();
    }
}