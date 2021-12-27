using System.ComponentModel;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    internal class GameplaySetupCell : TableCell, INotifyPropertyChanged
    {
        private GameplaySetupMenu tab;

        [UIValue("tab-name")]
        private string Name => tab?.name;

        [UIValue("tab-visible")]
        private bool Visible
        {
            get => tab != null && tab.visible;
            set
            {
                if (tab != null)
                {
                    tab.visible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visible)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public GameplaySetupCell PopulateCell(GameplaySetupMenu tab)
        {
            this.tab = tab;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visible)));
            return this;
        }
    }
}
