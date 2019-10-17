using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using System.Collections.Generic;
using System.Linq;

namespace BeatSaberMarkupLanguage
{
    public class SettingsTest : PersistentSingleton<SettingsTest>
    {
        [UIParams]
        private BSMLParserParams parserParams;

        [UIValue("list-options")]
        private List<object> options = new object[] { "1", "Something", "Kapow", "Yeet" }.ToList();

        [UIValue("list-choice")]
        private string listChoice = "Something";

        [UIValue("bool-test")]
        private bool boolTest = true;

        [UIValue("slider-value")]
        private int sliderValue = 5;

        [UIValue("string-value")]
        private string testString = "Shazam";

        [UIAction("#apply")]
        public void OnApply()
        {
            Logger.log.Info($"{sliderValue}");
            Logger.log.Info($"{testString}");
            Logger.log.Info($"Bool Test: {boolTest}");
            Logger.log.Info($"List Test: {listChoice}");
        }

        [UIAction("format")]
        public string Format(int number)
        {
            return number + "x";
        }

        public void Update()
        {
            //Logger.log.Info($"{sliderValue}");
            //Logger.log.Info($"{testString}");
            //Logger.log.Info($"Bool Test: {boolTest}");
            //Logger.log.Info($"List Test: {listChoice}");
        }
    }
}
