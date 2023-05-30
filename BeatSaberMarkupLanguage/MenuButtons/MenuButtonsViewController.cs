using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using UnityEngine;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    [ViewDefinition("BeatSaberMarkupLanguage.Views.main-left-screen.bsml")]
    internal class MenuButtonsViewController : BSMLAutomaticViewController
    {
        [UIValue("buttons")]
        public List<object> buttons = new List<object>();

        [UIValue("any-buttons")]
        private bool AnyButtons => buttons.Count > 0;

        [UIObject("root-object")]
        private GameObject rootObject;

        public void RefreshView()
        {
            if (rootObject == null || !BSMLParser.IsSingletonAvailable)
                return;
            Destroy(rootObject);
            DidActivate(true, false, false);
        }
    }
}
