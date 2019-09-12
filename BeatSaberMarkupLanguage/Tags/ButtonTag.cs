using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ButtonTag : BSMLTag
    {
        public override string[] Aliases => new[] { "button" };

        public override GameObject CreateObject(Transform parent)
        {
            Button button = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => (x.name == (parent.GetComponent<StartMiddleEndButtonsGroup>() == null ? "PlayButton" : "CreditsButton"))), parent, false);
            button.name = "BSMLButton";
            button.interactable = true;
            return button.gameObject;
        }
    }
}
