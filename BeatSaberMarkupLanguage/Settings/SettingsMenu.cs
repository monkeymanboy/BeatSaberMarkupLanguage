using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System;
using System.Reflection;
using UnityEngine;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage.Settings
{
    internal class SettingsMenu : CustomCellInfo
    {
        private const string SETTINGS_ERROR_PATH = "BeatSaberMarkupLanguage.Views.settings-error.bsml";
        public ViewController viewController;
        public BSMLParserParams parserParams;
        public bool didSetup;

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
            if (BSMLParser.IsSingletonAvailable)
            {
                try
                {
                    viewController = BeatSaberUI.CreateViewController<ViewController>();
                    SetupViewControllerTransform(viewController);
                    parserParams = BSMLParser.instance.Parse(Utilities.GetResourceContent(assembly, resource), viewController.gameObject, host);
                    didSetup = true;
                }
                catch (Exception ex)
                {
                    if (ex is BSMLResourceException resEx)
                    {
                        Logger.log.Error($"Cannot find bsml resource '{resEx.ResourcePath}' in '{resEx.Assembly?.GetName().Name ?? "<NULL>"}' for settings menu.");
                    }
                    else
                    {
                        Logger.log.Error($"Error adding settings menu for {assembly?.GetName().Name ?? "<NULL>"} ({text}): {ex.Message}");
                    }
                    Logger.log.Debug(ex);
                    parserParams = BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), SETTINGS_ERROR_PATH), viewController.gameObject);
                }
            }
        }

        public static void SetupViewControllerTransform(ViewController viewController)
        {
            viewController.rectTransform.sizeDelta = new Vector2(110, 0);
            viewController.rectTransform.anchorMin = new Vector2(0.5f, 0);
            viewController.rectTransform.anchorMax = new Vector2(0.5f, 1);
        }
    }
}
