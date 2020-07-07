using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public abstract class GenericInteractableSetting : GenericSetting
    {
        public abstract bool interactable { get; set; }
    }
}
