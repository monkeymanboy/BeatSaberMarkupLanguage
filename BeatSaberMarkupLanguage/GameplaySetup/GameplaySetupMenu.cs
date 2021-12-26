using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using System;
using System.Reflection;
using UnityEngine;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    internal class GameplaySetupMenu
    {
        private const string ERROR_PATH = "BeatSaberMarkupLanguage.Views.gameplay-tab-error.bsml";
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

        public bool visible
        {
            get => !Plugin.config.HiddenTabs.Contains(name);
            set
            {
                if (value)
                {
                    Plugin.config.HiddenTabs.Remove(name);
                }
                else if (visible)
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
                    Logger.log.Error($"Cannot find bsml resource '{resEx.ResourcePath}' in '{resEx.Assembly?.GetName().Name ?? "<NULL>"}' for Gameplay Settings tab.");
                }
                else
                {
                    Logger.log.Error($"Error adding gameplay settings tab for {assembly?.GetName().Name ?? "<NULL>"} ({name}): {ex.Message}");
                }
                Logger.log.Debug(ex);
                BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), ERROR_PATH), tabObject);
            }
        }

        public void SetVisible(bool isVisible)
        {
            tab.IsVisible = visible && isVisible;
        }

        public bool IsMenuType(MenuType toCheck)
        {
            return (menuType & toCheck) == toCheck;
        }
    }
}
