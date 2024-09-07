using System;
using System.Reflection;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using UnityEngine;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage.Settings
{
    internal class SettingsMenu : CustomCellInfo
    {
        private const string SettingsErrorViewResourcePath = "BeatSaberMarkupLanguage.Views.settings-error.bsml";

        public SettingsMenu(string name, string resource, object host, Assembly assembly)
            : base(name)
        {
            this.Resource = resource;
            this.Host = host;
            this.Assembly = assembly;
        }

        public ViewController ViewController { get; private set; }

        public BSMLParserParams ParserParams { get; private set; }

        public bool DidSetup { get; private set; }

        public string Resource { get; }

        public object Host { get; }

        public Assembly Assembly { get; }

        public static void SetupViewControllerTransform(ViewController viewController)
        {
            viewController.rectTransform.sizeDelta = new Vector2(110, 0);
            viewController.rectTransform.anchorMin = new Vector2(0.5f, 0);
            viewController.rectTransform.anchorMax = new Vector2(0.5f, 1);
        }

        public void Setup()
        {
            try
            {
                ViewController = BeatSaberUI.CreateViewController<ViewController>();
                SetupViewControllerTransform(ViewController);
                ParserParams = BSMLParser.Instance.Parse(Utilities.GetResourceContent(Assembly, Resource), ViewController.gameObject, Host);
                DidSetup = true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Error adding settings menu for {Assembly?.GetName().Name ?? "<NULL>"} ({Text})\n{ex}");
                ParserParams = BSMLParser.Instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), SettingsErrorViewResourcePath), ViewController.gameObject);
            }
        }
    }
}
