using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings.UI.ViewControllers;
using BS_Utils.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VRUI;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage.Settings
{
    internal class ModSettingsFlowCoordinator : FlowCoordinator
    {
        protected SettingsMenuListViewController settingsMenuListViewController;
        protected VRUINavigationController navigationController;

        protected VRUIViewController activeController;

        private Stack<VRUIViewController> submenuStack = new Stack<VRUIViewController>();

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (firstActivation)
            {
                title = "Mod Settings";
                navigationController = BeatSaberUI.CreateViewController<VRUINavigationController>();
                BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.settings-buttons.bsml"), navigationController.gameObject, this);

                settingsMenuListViewController = BeatSaberUI.CreateViewController<SettingsMenuListViewController>();
                settingsMenuListViewController.clickedMenu += OpenMenu;
                SetViewControllerToNavigationConctroller(navigationController, settingsMenuListViewController);
                ProvideInitialViewControllers(navigationController);

                foreach (CustomCellInfo cellInfo in BSMLSettings.instance.settingsMenus)
                {
                    (cellInfo as SettingsMenu).parserParams.AddEvent("back", Back);
                }
            }
        }

        public void OpenMenu(VRUIViewController viewController)
        {
            OpenMenu(viewController, false, false);
        }

        public void OpenMenu(VRUIViewController viewController, bool isSubmenu, bool isBack)
        {
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

            PushViewControllerToNavigationController(navigationController, viewController, null, wasActive);
            activeController = viewController;
        }

        public void ShowInitial()
        {
            if (activeController != null)
            {
                return;
            }

            settingsMenuListViewController.list.tableView.SelectCellWithIdx(0);
            OpenMenu((BSMLSettings.instance.settingsMenus.First() as SettingsMenu).viewController);
        }

        [UIAction("ok-click")]
        private void Ok()
        {
            Apply();
            Resources.FindObjectsOfTypeAll<MenuTransitionsHelperSO>().First().RestartGame(skipHealthWarning: true);
        }

        [UIAction("apply-click")]
        private void Apply()
        {
            EmitEventToAll("apply");
        }

        [UIAction("cancel-click")]
        private void Cancel()
        {
            Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First().InvokeMethod("DismissFlowCoordinator", new object[] { this, null, false });
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
            foreach (CustomCellInfo cellInfo in BSMLSettings.instance.settingsMenus)
            {
                (cellInfo as SettingsMenu).parserParams.EmitEvent(ev);
            }
        }
    }
}
