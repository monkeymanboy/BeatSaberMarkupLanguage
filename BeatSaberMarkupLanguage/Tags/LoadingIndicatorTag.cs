using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class LoadingIndicatorTag : BSMLTag
    {
        public override string[] Aliases => new string[] {"loading", "loading-indicator"};

        public override GameObject CreateObject(Transform parent)
        {
            GameObject loadingIndicator = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.name == "LoadingIndicator").First(), parent, false);
            loadingIndicator.name = "BSMLLoadingIndicator";

            loadingIndicator.AddComponent<LayoutElement>();

            return loadingIndicator;
        }
    }
}
