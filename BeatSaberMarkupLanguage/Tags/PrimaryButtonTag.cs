using BeatSaberMarkupLanguage.Util;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class PrimaryButtonTag : ButtonTag
    {
        public override string[] Aliases => new[] { "primary-button", "action-button" };

        public override GameObject CreateObject(Transform parent)
        {
            return base.CreateObject(parent).AddComponent<LayoutElement>().gameObject;
        }

        protected override Button GetButtonPrefab()
        {
            return DiContainer.Resolve<PracticeViewController>().GetComponentOnChild<Button>("PlayButton");
        }
    }
}
