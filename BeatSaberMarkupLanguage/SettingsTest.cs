using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage
{
    public class SettingsTest : PersistentSingleton<SettingsTest>
    {
        [UIValue("list-options")]
        List<object> options = new object[]{ "1", "Something", "Kapow", "Yeet"}.ToList();

        [UIValue("list-choice")]
        string listChoice = "Something";

        [UIValue("bool-test")]
        bool boolTest = true;

        public void Update()
        {
            Console.WriteLine("Bool Test:" + boolTest);
            Console.WriteLine("List Test:" + listChoice);
        }
    }
}
