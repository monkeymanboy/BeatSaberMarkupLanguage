using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings.UI.ViewControllers;
using BGLib.Polyglot;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Settings
{
    internal class ModSettingsFlowCoordinator : FlowCoordinator
    {
        public bool isAnimating;

        protected SettingsMenuListViewController settingsMenuListViewController;
        protected NavigationController navigationController;
        protected ViewController activeController;

        private readonly Stack<ViewController> submenuStack = new();
        private bool isPresenting;

        [UIComponent("bottom-buttons")]
        private Transform bottomButtons;

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
            if (isPresenting)
            {
                return;
            }

            if (!isBack)
            {
                if (isSubmenu)
                {
                    submenuStack.Push(activeController);
                }
                else
                {
                    submenuStack.Clear();
                }
            }

            bool wasActive = activeController != null;
            if (wasActive)
            {
                PopViewControllerFromNavigationController(navigationController, null, immediately: true);
            }

            PushViewControllerToNavigationController(
                navigationController,
                viewController,
                () =>
                {
                    isPresenting = false;

                    if (bottomButtons != null)
                    {
                        bottomButtons.SetAsLastSibling();
                    }
                },
                wasActive);

            activeController = viewController;
        }

        public void ShowInitial()
        {
            if (activeController != null)
            {
                return;
            }

            settingsMenuListViewController.list.tableView.SelectCellWithIdx(0);
            OpenMenu(BSMLSettings.instance.settingsMenus.First() as SettingsMenu);
            isPresenting = true;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                SetTitle(Localization.Get("BSML_MOD_SETTINGS_TITLE"));
                navigationController = BeatSaberUI.CreateViewController<NavigationController>();
                BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.settings-buttons.bsml"), navigationController.gameObject, this);

                settingsMenuListViewController = BeatSaberUI.CreateViewController<SettingsMenuListViewController>();
                settingsMenuListViewController.clickedMenu += OpenMenu;
                SetViewControllerToNavigationController(navigationController, settingsMenuListViewController);
                ProvideInitialViewControllers(navigationController);
            }
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
            if (isPresenting || isAnimating)
            {
                return;
            }

            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical);
            EmitEventToAll("cancel");
        }

        private void Back()
        {
            if (submenuStack.Count > 0)
            {
                OpenMenu(submenuStack.Pop(), false, true);
            }
        }

        private void EmitEventToAll(string ev)
        {
            foreach (SettingsMenu menu in BSMLSettings.instance.settingsMenus)
            {
                if (menu.didSetup)
                {
                    menu.parserParams.EmitEvent(ev);
                }
            }
        }
    }
}
