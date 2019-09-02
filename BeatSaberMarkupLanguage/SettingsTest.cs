using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage
{
    public class SettingsTest : PersistentSingleton<SettingsTest>
    {
        [UIParams]
        BSMLParserParams parserParams;

        [UIValue("list-options")]
        List<object> options = new object[]{ "1", "Something", "Kapow", "Yeet"}.ToList();

        [UIValue("list-choice")]
        string listChoice = "Something";

        [UIValue("bool-test")]
        bool boolTest = true;

        [UIValue("slider-value")]
        int sliderValue = 5;

        [UIValue("string-value")]
        string testString = "Shazam";

        [UIAction("#apply")]
        public void OnApply()
        {
            Console.WriteLine(sliderValue);
            Console.WriteLine(testString);
            Console.WriteLine("Bool Test:" + boolTest);
            Console.WriteLine("List Test:" + listChoice);
        }

        [UIAction("format")]
        public string Format(int number)
        {
            return number + "x";
        }

        public void Update()
        {
            /*
            Console.WriteLine(sliderValue);
            Console.WriteLine(testString);
            Console.WriteLine("Bool Test:" + boolTest);
            Console.WriteLine("List Test:" + listChoice);*/
        }
    }
}
