using System;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Settings.UI.ViewControllers
{
    [ViewDefinition("BeatSaberMarkupLanguage.Views.settings-list.bsml")]
    internal class SettingsMenuListViewController : BSMLAutomaticViewController
    {
        [UIComponent("list")]
        public CustomListTableData List;

        public event Action<SettingsMenu> ClickedMenu;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

            if (firstActivation)
            {
                rectTransform.sizeDelta = new Vector2(40, 0);
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
            }

            List.Data = BSMLSettings.instance.SettingsMenus;

            if (List.TableView != null)
            {
                List.TableView.ReloadData();
            }
        }

        [UIAction("settings-click")]
        private void SettingsClick(TableView tableView, int index)
        {
            SettingsMenu settingsMenu = (SettingsMenu)List.Data[index];
            ClickedMenu?.Invoke(settingsMenu);
        }
    }
}
