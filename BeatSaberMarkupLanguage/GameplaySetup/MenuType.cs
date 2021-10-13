using System;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    [Flags]
    public enum MenuType
    {
        None = 0,
        Solo = 1,
        Online = 2,
        Campaign = 4,
        Custom = 8,
        All = Solo | Online | Campaign | Custom
    }
}
