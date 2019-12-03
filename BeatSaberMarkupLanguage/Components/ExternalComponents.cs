using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    class ExternalComponents : MonoBehaviour
    {
        public List<Component> components = new List<Component>(); //Components added to this list will be handled as if they are part of this object while parsing even if they aren't
    }
}
