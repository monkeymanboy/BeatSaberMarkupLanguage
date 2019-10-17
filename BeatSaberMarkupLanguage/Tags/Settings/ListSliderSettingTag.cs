using BeatSaberMarkupLanguage.Components.Settings;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class ListSliderSettingTag : GenericSliderSettingTag<ListSliderSetting>
    {
        public override string[] Aliases => new[] { "list-slider-setting" };
    }
}
