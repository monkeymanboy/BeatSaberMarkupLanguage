using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Settings
{
    public class SettingsTest : NotifiableBase
    {
        [UIParams]
        private BSMLParserParams parserParams;

        private bool boolTest = true;

        [UIValue("slider-value")]
        private int sliderValue = 5;

        [UIValue("string-value")]
        private string testString = "Shazam";

        [UIValue("color-value")]
        private Color testColor = Color.yellow;

        [UIValue("list-options")]
        private string[] options = new string[] { "1", "Something", "Kapow", "Yeet" };

        [UIValue("list-choice")]
        private string listChoice = "Kapow";

        [UIValue("bool-test")]
        private bool BoolTest
        {
            get => boolTest;
            set
            {
                boolTest = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("#apply")]
        public void OnApply()
        {
            Logger.Log.Info($"{sliderValue}");
            Logger.Log.Info($"{testString}");
            Logger.Log.Info($"Bool Test: {boolTest}");
            Logger.Log.Info($"List Test: {listChoice}");
        }

        [UIAction("format")]
        public string Format(int number)
        {
            return number + "x";
        }

        [UIAction("change-bool")]
        public void ChangeBool()
        {
            BoolTest = !BoolTest;
        }
    }
}
