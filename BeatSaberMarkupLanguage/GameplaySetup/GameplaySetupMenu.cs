using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using System;
using System.Reflection;
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

        [UIAction("#post-parse")]
        public void Setup()
        {
            BSMLParser.instance.Parse(Utilities.GetResourceContent(assembly, resource), tabObject, host);
        }

        public void SetVisible(bool isVisible)
        {
            tab.IsVisible = isVisible;
        }

        public bool IsMenuType(MenuType toCheck)
        {
            return (menuType & toCheck) == toCheck;
        }
    }
}
