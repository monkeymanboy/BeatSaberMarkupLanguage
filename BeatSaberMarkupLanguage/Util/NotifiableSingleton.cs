using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#if GAME_VERSION_1_29_0
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
#else
namespace BeatSaberMarkupLanguage.Util
#endif
{
    public class NotifiableSingleton<T> : PersistentSingleton<T>, INotifyPropertyChanged
#if GAME_VERSION_1_29_0
        where T : MonoBehaviour
#else
        where T : class, new()
#endif
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Logger.Log?.Error($"Error invoking PropertyChanged\n{ex}");
            }
        }
    }
}
