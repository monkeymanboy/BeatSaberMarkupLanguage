/*
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using HMUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BeatSaberMarkupLanguage.ModifierButtons
{
    public class ModButtons : PersistentSingleton<ModButtons>
    {
        private GameplaySetupViewController gameplaySetupViewController;

        [UIComponent("tab-parent")]
        private Transform tabParent;

        [UIComponent("button-parent")]
        private Transform buttonParent;

        [UIComponent("grid-parent")]
        private Transform gridParent;

        [UIValue("buttons")]
        private List<object> buttons = new List<object>();

        [UIValue("any-buttons")]
        public bool AnyButtons => buttons.Count > 0;

        [UIParams]
        private BSMLParserParams parserParams;

        [UIAction("clicked-q")]
        private void Clicked()
        {
            int currentlySelected = gameplaySetupViewController.GetPrivateField<int>("_activePanelIdx");
            Logger.log.Info(currentlySelected.ToString());
            if (currentlySelected == 0)
            {
                
            }
            if (currentlySelected == 1)
            {

            }
            if (currentlySelected == 2)
            {

            }
            if (currentlySelected == 3)
            {

            }
            parserParams.EmitEvent("show-new");
        }

        internal IEnumerator Setup()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<GameplaySetupViewController>().Any());
            gameplaySetupViewController = Resources.FindObjectsOfTypeAll<GameplaySetupViewController>().First();
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.gameplay-mod-left-screen.bsml"), gameplaySetupViewController.gameObject, this);
            
        }

        // Please do something better monkey

        [UIAction("#post-parse")]
        private void PostParse()
        {
            var textseg = gameplaySetupViewController.GetPrivateField<TextSegmentedControl>("_selectionSegmentedControl");
            //var gm = gameplaySetupViewController.GetPrivateField<GameplayModifiersPanelController>("_gameplayModifiersPanelController");
            
            //gridParent.SetParent(coseg.transform, false);
            //buttonParent.SetParent(textseg.transform, false);
            tabParent.SetParent(textseg.transform, false);
        }

        public void RegisterButton(ModButton modButton)
        {
            buttons.Add(modButton);
            modButton.isActive = true;
        }

        public void UnregisterButton(ModButton modButton)
        {
            if (modButton.isActive)
                buttons.Remove(modButton);
            
        }
    }
}
*/