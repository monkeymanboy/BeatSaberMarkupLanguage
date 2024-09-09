using System.ComponentModel;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    internal class GameplaySetupCell : TableCell, INotifyPropertyChanged
    {
        private GameplaySetupMenu tab;

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        [UIValue("tab-name")]
        private string Name => tab?.Name;

        [UIValue("tab-visible")]
        private bool Visible
        {
            get => tab != null && tab.Visible;
            set
            {
                if (tab != null)
                {
                    tab.Visible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        internal GameplaySetupCell PopulateCell(GameplaySetupMenu tab)
        {
            this.tab = tab;

            NotifyPropertyChanged(nameof(Name));
            NotifyPropertyChanged(nameof(Visible));

            return this;
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
