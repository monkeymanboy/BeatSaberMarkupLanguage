using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage.Tags
{
    public abstract class PrefabBSMLTag : BSMLTag
    {
        private BSMLPrefab prefab;

        public sealed override void SetUp()
        {
            PrefabParams prefabParams = CreatePrefab();

            prefab = prefabParams.RootObject.AddComponent<BSMLPrefab>();
            prefab.ContainerObject = prefabParams.ContainerObject;
            prefab.EnableOnInstantiate = prefabParams.EnableOnInstantiate;
            prefabParams.RootObject.SetActive(false);
        }

        public override GameObject CreateObject(Transform parent)
        {
            if (prefab == null)
            {
                throw new InvalidOperationException("CreateObject called before or during CreatePrefab");
            }

            BSMLPrefab instance = BeatSaberUI.DiContainer.InstantiatePrefabForComponent<BSMLPrefab>(prefab, parent);
            Object.Destroy(instance);

            string name = instance.name;
            instance.name = name.Substring(0, name.Length - 7); // remove (Clone)

            if (instance.EnableOnInstantiate)
            {
                instance.gameObject.SetActive(true);
            }

            return instance.ContainerObject;
        }

        protected abstract PrefabParams CreatePrefab();

        protected readonly struct PrefabParams
        {
            internal PrefabParams(GameObject rootObject, bool enableOnInstantiate = true)
            {
                RootObject = rootObject;
                ContainerObject = rootObject;
                EnableOnInstantiate = enableOnInstantiate;
            }

            internal PrefabParams(GameObject rootObject, GameObject containerObject, bool enableOnInstantiate = true)
            {
                RootObject = rootObject;
                ContainerObject = containerObject;
                EnableOnInstantiate = enableOnInstantiate;
            }

            internal GameObject RootObject { get; }

            internal GameObject ContainerObject { get; }

            internal bool EnableOnInstantiate { get; }
        }

        private class BSMLPrefab : MonoBehaviour
        {
            [SerializeField]
            internal GameObject ContainerObject;

            [SerializeField]
            internal bool EnableOnInstantiate;
        }
    }
}
