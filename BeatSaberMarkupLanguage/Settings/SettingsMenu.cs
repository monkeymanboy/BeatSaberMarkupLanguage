using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System.Reflection;
using UnityEngine;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage.Settings
{
    internal class SettingsMenu : CustomCellInfo
    {
        public ViewController viewController;
        public BSMLParserParams parserParams;
        
        public string resource;
        public object host;
        public Assembly assembly;

        public SettingsMenu(string name, string resource, object host, Assembly assembly) : base(name)
        {
            this.resource = resource;
            this.host = host;
            this.assembly = assembly;
        }

        public void Setup()
        {
            viewController = BeatSaberUI.CreateViewController<ViewController>();
            SetupViewControllerTransform(viewController);
            parserParams = BSMLParser.instance.Parse(Utilities.GetResourceContent(assembly, resource), viewController.gameObject, host);
        }

        public static void SetupViewControllerTransform(ViewController viewController)
        {
            viewController.rectTransform.sizeDelta = new Vector2(110, 0);
            viewController.rectTransform.anchorMin = new Vector2(0.5f, 0);
            viewController.rectTransform.anchorMax = new Vector2(0.5f, 1);
        }
    }
}
