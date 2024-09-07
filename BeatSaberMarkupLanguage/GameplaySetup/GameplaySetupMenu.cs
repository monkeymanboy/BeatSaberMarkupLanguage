using System;
using System.Reflection;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using UnityEngine;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    internal class GameplaySetupMenu
    {
        public string Resource;
        public object Host;
        public Assembly Assembly;
        public MenuType MenuType;

        [UIValue("tab-name")]
        public string Name;

        private const string ErrorViewResourcePath = "BeatSaberMarkupLanguage.Views.gameplay-tab-error.bsml";

        [UIObject("root-tab")]
        private GameObject tabObject;

        [UIComponent("root-tab")]
        private Tab tab;

        public GameplaySetupMenu(string name, string resource, object host, Assembly assembly, MenuType menuType)
        {
            this.Name = name;
            this.Resource = resource;
            this.Host = host;
            this.Assembly = assembly;
            this.MenuType = menuType;
        }

        public bool Visible
        {
            get => !Plugin.Config.HiddenTabs.Contains(Name);
            set
            {
                if (value)
                {
                    Plugin.Config.HiddenTabs.Remove(Name);
                }
                else if (Visible)
                {
                    Plugin.Config.HiddenTabs.Add(Name);
                }
            }
        }

        [UIAction("#post-parse")]
        public void Setup()
        {
            try
            {
                BSMLParser.Instance.Parse(Utilities.GetResourceContent(Assembly, Resource), tabObject, Host);
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Error adding gameplay settings tab for {Assembly?.GetName().Name ?? "<NULL>"} ({Name})\n{ex}");
                BSMLParser.Instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), ErrorViewResourcePath), tabObject);
            }
        }

        public void SetVisible(bool isVisible)
        {
            tab.IsVisible = Visible && isVisible;
        }

        public bool IsMenuType(MenuType toCheck)
        {
            return (MenuType & toCheck) == toCheck;
        }
    }
}
