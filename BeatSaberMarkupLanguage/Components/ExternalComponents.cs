using System.Collections.Generic;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components
{
    public class ExternalComponents : MonoBehaviour
    {
        public List<Component> components = new(); // Components added to this list will be handled as if they are part of this object while parsing even if they aren't

        public T Get<T>()
            where T : Component
        {
            foreach (Component component in components)
            {
                if (component is T componentT)
                {
                    return componentT;
                }
            }

            return null;
        }
    }
}
