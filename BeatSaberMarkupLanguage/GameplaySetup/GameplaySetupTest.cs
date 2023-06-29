using BeatSaberMarkupLanguage.Attributes;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    internal class GameplaySetupTest
    {
        [UIValue("test")]
        private bool checkbox1;

        [UIValue("test2")]
        private bool checkbox2;

        [UIValue("test3")]
        private bool checkbox3;

        [UIValue("test4")]
        private bool checkbox4 = true;
    }
}
