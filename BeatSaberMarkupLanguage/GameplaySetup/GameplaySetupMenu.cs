using System;
using System.Reflection;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using UnityEngine;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    internal class GameplaySetupMenu
    {
        private const string ErrorViewResourcePath = "BeatSaberMarkupLanguage.Views.gameplay-tab-error.bsml";
        public string resource;
        public object host;
        public Assembly assembly;
        public MenuType menuType;

        [UIValue("tab-name")]
        public string name;

        [UIObject("root-tab")]
        private GameObject tabObject;

        [UIComponent("root-tab")]
        private Tab tab;

        public bool Visible
        {
            get => !Plugin.config.HiddenTabs.Contains(name);
            set
            {
                if (value)
                {
                    Plugin.config.HiddenTabs.Remove(name);
                }
                else if (Visible)
                {
                    Plugin.config.HiddenTabs.Add(name);
                }
            }
        }

        public GameplaySetupMenu(string name, string resource, object host, Assembly assembly, MenuType menuType)
        {
            this.name = name;
            this.resource = resource;
            this.host = host;
            this.assembly = assembly;
            this.menuType = menuType;
        }

        [UIAction("#post-parse")]
        public void Setup()
        {
            try
            {
                BSMLParser.instance.Parse(Utilities.GetResourceContent(assembly, resource), tabObject, host);
            }
            catch (Exception ex)
            {
                if (ex is BSMLResourceException resEx)
                {
                    Logger.Log.Error($"Cannot find bsml resource '{resEx.ResourcePath}' in '{resEx.Assembly?.GetName().Name ?? "<NULL>"}' for Gameplay Settings tab.");
                }
                else
                {
                    Logger.Log.Error($"Error adding gameplay settings tab for {assembly?.GetName().Name ?? "<NULL>"} ({name}): {ex.Message}");
                }

                Logger.Log.Debug(ex);
                BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), ErrorViewResourcePath), tabObject);
            }
        }

        public void SetVisible(bool isVisible)
        {
            tab.IsVisible = Visible && isVisible;
        }

        public bool IsMenuType(MenuType toCheck)
        {
            return (menuType & toCheck) == toCheck;
        }
    }
}
