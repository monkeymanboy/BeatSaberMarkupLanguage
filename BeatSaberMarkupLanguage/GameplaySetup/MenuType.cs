using System;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    [Flags]
    public enum MenuType
    {
        Solo = 1,
        Online = 2,
        Campaign = 4,
        All = Solo | Online | Campaign
    }
}
