using BeatSaberMarkupLanguage.Components.Settings;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class ListSettingTag : IncDecSettingTag<ListSetting>
    {
        public override string[] Aliases => new[] { "list-setting" };
    }
}
