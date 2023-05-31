using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public class LocalizationTestViewController : BSMLResourceViewController
    {
        [UIValue("contents")]
        public List<object> contents = new(new[] { "one", "two", "three" });

        public override string ResourceName => "BeatSaberMarkupLanguage.Views.localization-test.bsml";
    }
}
