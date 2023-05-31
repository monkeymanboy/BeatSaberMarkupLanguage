using System.ComponentModel;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    internal class GameplaySetupCell : TableCell, INotifyPropertyChanged
    {
        private GameplaySetupMenu tab;

        public event PropertyChangedEventHandler PropertyChanged;

        [UIValue("tab-name")]
        private string Name => tab?.name;

        [UIValue("tab-visible")]
        private bool Visible
        {
            get => tab != null && tab.Visible;
            set
            {
                if (tab != null)
                {
                    tab.Visible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visible)));
                }
            }
        }

        public GameplaySetupCell PopulateCell(GameplaySetupMenu tab)
        {
            this.tab = tab;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visible)));
            return this;
        }
    }
}
