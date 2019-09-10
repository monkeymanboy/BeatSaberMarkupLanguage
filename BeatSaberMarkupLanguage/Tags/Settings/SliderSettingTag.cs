using BeatSaberMarkupLanguage.Components.Settings;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class SliderSettingTag : GenericSliderSettingTag<SliderSetting>
    {
        public override string[] Aliases => new[] { "slider-setting" };
    }
}
