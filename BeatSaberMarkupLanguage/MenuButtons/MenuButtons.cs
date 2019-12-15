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

        [UIValue("release-notes")]
        private Transform releaseNotesScrollView;
        
        [UIValue("buttons")]
        private List<object> buttons = new List<object>();

        [UIValue("any-buttons")]
        public bool AnyButtons => buttons.Count > 0;

        [UIObject("root-object")]
        private GameObject rootObject;

        [UIParams]
        private BSMLParserParams parserParams;

        internal void Setup()
        {
            releaseInfoViewController = Resources.FindObjectsOfTypeAll<ReleaseInfoViewController>().First();
            releaseNotesScrollView = releaseInfoViewController.GetPrivateField<TextPageScrollView>("_textPageScrollView").transform;
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.main-left-screen.bsml"), releaseInfoViewController.gameObject, this);
        }

        internal void Refresh()
        {
            if(rootObject != null)
            {
                releaseNotesScrollView.transform.SetParent(null, false);
                GameObject.Destroy(rootObject);
                StopAllCoroutines();
                Setup();
            }
        }

        public void RegisterButton(MenuButton menuButton)
        {
            if (!buttons.Any(x => x == menuButton))
                buttons.Add(menuButton);
            Refresh();

            
        }

        public void UnregisterButton(MenuButton menuButton)
        {
            buttons.Remove(menuButton);
            Refresh();
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            if (AnyButtons && !Plugin.config.GetBool("New", "seenMenuButton", false)) 
            {
                StartCoroutine(ShowNew());
            }
        }
        IEnumerator ShowNew()
        {
            yield return new WaitForSeconds(1);
            parserParams.EmitEvent("show-new");
            Plugin.config.SetBool("New", "seenMenuButton", true);
        }
    }
}
