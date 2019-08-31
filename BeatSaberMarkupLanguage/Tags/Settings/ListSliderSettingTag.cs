using BeatSaberMarkupLanguage.Components.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class ListSliderSettingTag : GenericSliderSettingTag<ListSliderSetting>
    {
        public override string[] Aliases => new[] { "list-slider-setting" };
    }
}
