using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings.UI.ViewControllers;
using HMUI;
using IPA.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage.Settings
{
    internal class ModSettingsFlowCoordinator : FlowCoordinator
    {
        protected SettingsMenuListViewController settingsMenuListViewController;
        protected NavigationController navigationController;

        protected ViewController activeController;

        private Stack<ViewController> submenuStack = new Stack<ViewController>();
        private bool isPresenting;
        public bool isAnimating;

        [UIComponent("bottom-buttons")]
        private Transform bottomButtons;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                this.SetField<FlowCoordinator, string>("_title", "Mod Settings");
                navigationController = BeatSaberUI.CreateViewController<NavigationController>();
                BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.settings-buttons.bsml"), navigationController.gameObject, this);

                settingsMenuListViewController = BeatSaberUI.CreateViewController<SettingsMenuListViewController>();
                settingsMenuListViewController.clickedMenu += OpenMenu;
                SetViewControllerToNavigationController(navigationController, settingsMenuListViewController);
                ProvideInitialViewControllers(navigationController);
            }
        }

        public void OpenMenu(SettingsMenu menu)
        {
            if (!menu.didSetup)
            {
                menu.Setup();
                menu.parserParams.AddEvent("back", Back);
            }
            OpenMenu(menu.viewController, false, false);
        }

        public void OpenMenu(ViewController viewController, bool isSubmenu, bool isBack)
        {
            if (isPresenting) return;
            if (!isBack)
            {
                if (isSubmenu)
                    submenuStack.Push(activeController);
                else
                    submenuStack.Clear();
            }

            bool wasActive = activeController != null;
            if (wasActive)
                PopViewControllerFromNavigationController(navigationController, null, immediately: true);
            PushViewControllerToNavigationController(navigationController, viewController, delegate
            {
                isPresenting = false;
                bottomButtons?.SetAsLastSibling();
            }, wasActive);
            activeController = viewController;
        }

        public void ShowInitial()
        {
            if (activeController != null)
                return;

            settingsMenuListViewController.list.tableView.SelectCellWithIdx(0);
            OpenMenu(BSMLSettings.instance.settingsMenus.First() as SettingsMenu);
            isPresenting = true;
        }

        [UIAction("ok-click")]
        private void Ok()
        {
            EmitEventToAll("apply");
            FindObjectOfType<MenuTransitionsHelper>().RestartGame();
        }

        [UIAction("cancel-click")]
        private void Cancel()
        {
            if (isPresenting || isAnimating) return;
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical);
            EmitEventToAll("cancel");
        }

        private void Back()
        {
            if (submenuStack.Count > 0)
                OpenMenu(submenuStack.Pop(), false, true);
        }

        private void EmitEventToAll(string ev)
        {
            foreach (SettingsMenu menu in BSMLSettings.instance.settingsMenus)
                if (menu.didSetup)
                    menu.parserParams.EmitEvent(ev);
        }
    }
}
