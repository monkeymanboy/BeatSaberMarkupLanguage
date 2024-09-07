using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BeatSaberMarkupLanguage.Util;
using Zenject;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    public class MenuButtons : PersistentSingleton<MenuButtons>, ILateDisposable
    {
        private static readonly IComparer<MenuButton> MenuButtonComparer = Comparer<MenuButton>.Create((a, b) => a.StrippedText.CompareTo(b.StrippedText));

        private MenuButtonsViewController menuButtonsViewController;

        internal SortedList<MenuButton> Buttons { get; } = new(MenuButtonComparer);

        public void RegisterButton(MenuButton menuButton)
        {
            if (Buttons.Any(mb => mb.Text == menuButton.Text))
            {
                return;
            }

            Buttons.Add(menuButton);
            Refresh();
        }

        public void UnregisterButton(MenuButton menuButton)
        {
            Buttons.Remove(menuButton);
            Refresh();
        }

        public void LateDispose()
        {
            menuButtonsViewController = null;
        }

        internal void Refresh()
        {
            if (menuButtonsViewController == null)
            {
                return;
            }

            menuButtonsViewController.RefreshView();
        }

        [Inject]
        [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by Zenject")]
        private void Construct(MenuButtonsViewController menuButtonsViewController)
        {
            this.menuButtonsViewController = menuButtonsViewController;
        }
    }
}
