using BeatSaberMarkupLanguage.Attributes;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public class LocalizationTestViewController : BSMLResourceViewController
    {
        [UIValue("contents")]
        public string[] contents = new[] { "one", "two", "three" };

        public override string ResourceName => "BeatSaberMarkupLanguage.Views.localization-test.bsml";
    }
}
