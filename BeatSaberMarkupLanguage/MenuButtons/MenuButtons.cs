using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BeatSaberMarkupLanguage.Util;
using Zenject;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    public class MenuButtons : PersistentSingleton<MenuButtons>
    {
        private readonly WeakReference<MenuButtonsViewController> _menuButtonsViewController = new(null);

        internal List<MenuButton> buttons { get; } = new();

        public void RegisterButton(MenuButton menuButton)
        {
            if (buttons.Any(mb => mb.Text == menuButton.Text))
            {
                return;
            }

            buttons.Add(menuButton);
            Refresh();
        }

        public void UnregisterButton(MenuButton menuButton)
        {
            buttons.Remove(menuButton);
            Refresh();
        }

        internal void Refresh()
        {
            if (!_menuButtonsViewController.TryGetTarget(out MenuButtonsViewController menuButtonsViewController))
            {
                return;
            }

            menuButtonsViewController.RefreshView();
        }

        [Inject]
        [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by Zenject")]
        private void Construct(MenuButtonsViewController menuButtonsViewController)
        {
            _menuButtonsViewController.SetTarget(menuButtonsViewController);
        }
    }
}
