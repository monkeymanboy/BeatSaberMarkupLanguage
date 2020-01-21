using BeatSaberMarkupLanguage.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    public class GameplaySetup : PersistentSingleton<GameplaySetup>
    {
        private GameplaySetupViewController gameplaySetupViewController;
        
        [UIValue("vanilla-items")]
        private List<Transform> vanillaItems = new List<Transform>();

        [UIValue("mod-menus")]
        private List<object> menus = new List<object>();

        internal void Setup()
        {
            if (menus.Count == 0) return;
            gameplaySetupViewController = Resources.FindObjectsOfTypeAll<GameplaySetupViewController>().First();
            gameplaySetupViewController.transform.Find("HeaderPanel").GetComponentInChildren<TextMeshProUGUI>().fontSize = 4;
            foreach(Transform transform in gameplaySetupViewController.transform)
            {
                if (transform.name != "HeaderPanel")
                    vanillaItems.Add(transform);
            }
            (gameplaySetupViewController.transform.Find("HeaderPanel") as RectTransform).sizeDelta = new Vector2(90, 6);
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.gameplay-setup.bsml"), gameplaySetupViewController.gameObject, this);
        }
        
        public void AddTab(string name, string resource, object host)
        {
            if (menus.Any(x => (x as GameplaySetupMenu).name == name))
                return;
            menus.Add(new GameplaySetupMenu(name, resource, host, Assembly.GetCallingAssembly()));
        }
    }
}
