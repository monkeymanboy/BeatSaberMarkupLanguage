using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings.UI.ViewControllers;
using BS_Utils.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRUI;

namespace BeatSaberMarkupLanguage.Settings
{
    class ModSettingsFlowCoordinator : FlowCoordinator
    {
        protected SettingsMenuListViewController settingsMenuListViewController;
        protected VRUINavigationController navigationController;

        protected VRUIViewController activeController;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            if (activationType == ActivationType.AddedToHierarchy)
            {
                navigationController = BeatSaberUI.CreateViewController<VRUINavigationController>();
                BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.settings-buttons.bsml"), navigationController.gameObject, this);

                settingsMenuListViewController = BeatSaberUI.CreateViewController<SettingsMenuListViewController>();
                settingsMenuListViewController.clickedMenu += OpenMenu;
                SetViewControllersToNavigationConctroller(navigationController, settingsMenuListViewController/*, settingsMenuListViewController.selectedMenu.viewController*/);
                ProvideInitialViewControllers(navigationController);
            }
        }

        public void OpenMenu(VRUIViewController viewController)
        {
            bool wasActive = activeController != null;
            if (wasActive)
            {
                PopViewControllerFromNavigationController(navigationController, null, immediately: true);
            }
            PushViewControllerToNavigationController(navigationController, viewController, null, wasActive);
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
            //emit apply event
        }

        [UIAction("cancel-click")]
        private void Cancel()
        {
            Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First().InvokeMethod("DismissFlowCoordinator", new object[] { this, null, false });
            //emit reset to variable state event
        }
    }
}
