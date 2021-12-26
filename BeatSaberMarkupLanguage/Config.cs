using System.Collections.Generic;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace BeatSaberMarkupLanguage
{
    public class Config
    {
        [NonNullable, UseConverter(typeof(ListConverter<string>))]
        public virtual List<string> PinnedMods { get; set; } = new List<string>();

        [NonNullable, UseConverter(typeof(ListConverter<string>))]
        public virtual List<string> HiddenTabs { get; set; } = new List<string>();
    }
}