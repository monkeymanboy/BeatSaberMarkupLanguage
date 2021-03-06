﻿using BeatSaberMarkupLanguage.Attributes;
using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public class LocalizationTestViewController : BSMLResourceViewController
    {
        public override string ResourceName => "BeatSaberMarkupLanguage.Views.localization-test.bsml";

        [UIValue("contents")]
        public List<object> contents = new List<object>(new[] { "one", "two", "three" });
    }
}
