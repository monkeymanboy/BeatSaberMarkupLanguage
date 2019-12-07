using BeatSaberMarkupLanguage.Components.Settings;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class IncrementSettingTag : IncDecSettingTag<IncrementSetting>
    {
        public override string[] Aliases => new[] { "increment-setting" };
    }
}
