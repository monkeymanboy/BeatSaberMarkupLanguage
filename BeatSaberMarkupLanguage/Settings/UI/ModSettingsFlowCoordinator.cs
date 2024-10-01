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
        private readonly Stack<ViewController> submenuStack = new();
        private bool isPresenting;

        [UIComponent("bottom-buttons")]
        private Transform bottomButtons;

        public bool IsAnimating { get; internal set; }

        protected SettingsMenuListViewController SettingsMenuListViewController { get; private set; }

        protected NavigationController NavigationController { get; private set; }

        protected ViewController ActiveController { get; private set; }

        public void OpenMenu(SettingsMenu menu)
        {
            if (!menu.DidSetup)
            {
                menu.ParserParams.AddEvent("back", Back);
                menu.DidSetup = true;
            }

            OpenMenu(menu.ViewController, false, false);
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
                    submenuStack.Push(ActiveController);
                }
                else
                {
                    submenuStack.Clear();
                }
            }

            bool wasActive = ActiveController != null;
            if (wasActive)
            {
                PopViewControllerFromNavigationController(NavigationController, null, immediately: true);
            }

            PushViewControllerToNavigationController(
                NavigationController,
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

            ActiveController = viewController;
        }

        public void ShowInitial()
        {
            if (ActiveController != null)
            {
                return;
            }

            SettingsMenuListViewController.List.TableView.SelectCellWithIdx(0);
            OpenMenu(BSMLSettings.Instance.SettingsMenus.First() as SettingsMenu);
            isPresenting = true;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                SetTitle(Localization.Get("BSML_MOD_SETTINGS_TITLE"));
                NavigationController = BeatSaberUI.CreateViewController<NavigationController>();
                BSMLParser.Instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.settings-buttons.bsml"), NavigationController.gameObject, this);

                RectTransform container = new GameObject("Container").AddComponent<RectTransform>();
                container.SetParent(NavigationController.transform, false);
                container.anchorMin = Vector2.zero;
                container.anchorMax = Vector2.one;
                container.sizeDelta = new Vector2(0, -12);
                container.anchoredPosition = new Vector2(0, 6);
                NavigationController._controllersContainer = container;

                SettingsMenuListViewController = BeatSaberUI.CreateViewController<SettingsMenuListViewController>();
                SettingsMenuListViewController.ClickedMenu += OpenMenu;

                RectTransform viewControllerTransform = (RectTransform)SettingsMenuListViewController.transform;
                viewControllerTransform.sizeDelta = new Vector2(0, -12);
                viewControllerTransform.anchoredPosition = new Vector2(0, 6);

                SetViewControllerToNavigationController(NavigationController, SettingsMenuListViewController);
                ProvideInitialViewControllers(NavigationController);
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
            if (isPresenting || IsAnimating)
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
            foreach (SettingsMenu menu in BSMLSettings.Instance.SettingsMenus)
            {
                menu.ParserParams.EmitEvent(ev);
            }
        }
    }
}
