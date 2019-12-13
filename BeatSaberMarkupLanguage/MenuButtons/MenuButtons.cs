using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    public class MenuButtons : PersistentSingleton<MenuButtons>
    {
        private ReleaseInfoViewController releaseInfoViewController;

        [UIComponent("release-notes-parent")]
        private Transform releaseNoteTab;

        [UIValue("buttons")]
        private List<object> buttons = new List<object>();

        [UIValue("any-buttons")]
        public bool AnyButtons => buttons.Count > 0;

        [UIParams]
        private BSMLParserParams parserParams;

        internal void Setup()
        {
            releaseInfoViewController = Resources.FindObjectsOfTypeAll<ReleaseInfoViewController>().First();
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.main-left-screen.bsml"), releaseInfoViewController.gameObject, this);
        }

        public void RegisterButton(MenuButton menuButton)
        {
            buttons.Add(menuButton);
        }
        
        [UIAction("#post-parse")]
        private void PostParse()
        {
            releaseInfoViewController.GetPrivateField<TextPageScrollView>("_textPageScrollView").transform.SetParent(releaseNoteTab, false);
            if (AnyButtons && !Plugin.config.GetBool("New", "seenMenuButton", false)) 
            {
                StartCoroutine(ShowNew());
                Plugin.config.SetBool("New", "seenMenuButton", true);
            }
        }
        IEnumerator ShowNew()
        {
            yield return new WaitForSeconds(1);
            parserParams.EmitEvent("show-new");
        }
    }
}
