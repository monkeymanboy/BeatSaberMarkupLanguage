using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Util;
using Zenject;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    public class MenuButtons : ZenjectSingleton<MenuButtons>
    {
        private static readonly IComparer<MenuButton> MenuButtonComparer = Comparer<MenuButton>.Create((a, b) => a.StrippedText.CompareTo(b.StrippedText));

        private readonly MenuButtonsViewController menuButtonsViewController;

        [Inject]
        private MenuButtons(MenuButtonsViewController menuButtonsViewController)
        {
            this.menuButtonsViewController = menuButtonsViewController;
            this.menuButtonsViewController.Buttons = Buttons;
        }

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

        internal void Refresh()
        {
            if (menuButtonsViewController == null)
            {
                return;
            }

            menuButtonsViewController.RefreshView();
        }
    }
}
