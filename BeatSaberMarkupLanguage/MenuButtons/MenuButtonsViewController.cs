using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    [ViewDefinition("BeatSaberMarkupLanguage.Views.main-left-screen.bsml")]
    internal class MenuButtonsViewController : BSMLAutomaticViewController, IInitializable, IDisposable
    {
        private MainFlowCoordinator _mainFlowCoordinator;
        private MainMenuViewController _mainMenuViewController;

        [UIObject("root-object")]
        private GameObject _rootObject;

        [UIValue("buttons")]
        public List<MenuButton> buttons => MenuButtons.instance.buttons;

        [UIValue("any-buttons")]
        private bool AnyButtons => buttons.Count > 0;

        public void Initialize()
        {
            _mainMenuViewController.didActivateEvent += OnMainMenuViewControllerActivated;
        }

        public void Dispose()
        {
            _mainMenuViewController.didActivateEvent -= OnMainMenuViewControllerActivated;
        }

        public void RefreshView()
        {
            if (_rootObject == null)
            {
                return;
            }

            Destroy(_rootObject);
            DidActivate(true, false, false);
        }

        [Inject]
        [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by Zenject")]
        private void Construct(MainFlowCoordinator mainFlowCoordinator, MainMenuViewController mainMenuViewController)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _mainMenuViewController = mainMenuViewController;
        }

        private void OnMainMenuViewControllerActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            _mainFlowCoordinator.SetLeftScreenViewController(this, addedToHierarchy ? AnimationType.None : AnimationType.In);
        }
    }
}
