using System;
using System.Reflection;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using UnityEngine;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    internal class GameplaySetupMenu
    {
        public string resource;
        public object host;
        public Assembly assembly;
        public MenuType menuType;

        [UIValue("tab-name")]
        public string name;

        private const string ErrorViewResourcePath = "BeatSaberMarkupLanguage.Views.gameplay-tab-error.bsml";

        [UIObject("root-tab")]
        private GameObject tabObject;

        [UIComponent("root-tab")]
        private Tab tab;

        public GameplaySetupMenu(string name, string resource, object host, Assembly assembly, MenuType menuType)
        {
            this.name = name;
            this.resource = resource;
            this.host = host;
            this.assembly = assembly;
            this.menuType = menuType;
        }

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

        [UIAction("#post-parse")]
        public void Setup()
        {
            try
            {
                BSMLParser.instance.Parse(Utilities.GetResourceContent(assembly, resource), tabObject, host);
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Error adding gameplay settings tab for {assembly?.GetName().Name ?? "<NULL>"} ({name})\n{ex}");
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
