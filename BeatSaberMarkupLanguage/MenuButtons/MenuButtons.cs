using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BeatSaberMarkupLanguage.Util;
using Zenject;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    public class MenuButtons : PersistentSingleton<MenuButtons>, ILateDisposable
    {
        private MenuButtonsViewController _menuButtonsViewController;

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

        public void LateDispose()
        {
            _menuButtonsViewController = null;
        }

        internal void Refresh()
        {
            if (_menuButtonsViewController == null)
            {
                return;
            }

            _menuButtonsViewController.RefreshView();
        }

        [Inject]
        [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by Zenject")]
        private void Construct(MenuButtonsViewController menuButtonsViewController)
        {
            _menuButtonsViewController = menuButtonsViewController;
        }
    }
}
