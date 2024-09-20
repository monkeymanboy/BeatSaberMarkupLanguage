using BeatSaberMarkupLanguage.Attributes;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    internal class LocalizationTestViewController : BSMLResourceViewController
    {
        [UIValue("contents")]
        public string[] Contents { get; } = ["one", "two", "three"];

        public override string ResourceName => "BeatSaberMarkupLanguage.Views.localization-test.bsml";
    }
}
