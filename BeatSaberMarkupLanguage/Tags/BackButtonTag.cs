using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class BackButtonTag : BSMLTag
    {
        public override string[] Aliases => new[] { "back-button" };

        public override GameObject CreateObject(Transform parent)
        {
            Button button = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => x.name == "BackArrowButton"), parent);
            button.name = "BSMLBackButton";
            button.interactable = true;
            return button.gameObject;
        }
    }
}
