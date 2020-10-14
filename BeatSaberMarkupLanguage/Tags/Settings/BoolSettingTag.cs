using BeatSaberMarkupLanguage.Components.Settings;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class BoolSettingTag : IncDecSettingTag<BoolSetting>
    {
        public override string[] Aliases => new[] { "bool-setting", "checkbox-setting", "checkbox" };
    }
}
