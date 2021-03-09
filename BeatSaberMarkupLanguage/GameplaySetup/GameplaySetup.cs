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

        [UIComponent("selector")]
        private RectTransform selector;

        internal void Setup()
        {
            if (menus.Count == 0) return;
            gameplaySetupViewController = Resources.FindObjectsOfTypeAll<GameplaySetupViewController>().First();
            gameplaySetupViewController.transform.Find("HeaderPanel").GetComponentInChildren<TextMeshProUGUI>().fontSize = 4;
            vanillaItems.Clear();
            foreach(Transform transform in gameplaySetupViewController.transform)
            {
                if (transform.name != "HeaderPanel")
                    vanillaItems.Add(transform);
            }
            // (gameplaySetupViewController.transform.Find("HeaderPanel") as RectTransform).sizeDelta = new Vector2(90, 6);
            (gameplaySetupViewController.transform.Find("TextSegmentedControl") as RectTransform).localPosition = new Vector2(0, 38f);
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.gameplay-setup.bsml"), gameplaySetupViewController.gameObject, this);
            selector.parent.localPosition = new Vector3(0f, -5f);
            selector.localPosition = new Vector3(0f, 45f);
            gameplaySetupViewController.didActivateEvent += GameplaySetupDidActivate;
        }

        private void GameplaySetupDidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            MenuType menuType;
            switch (BeatSaberUI.MainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf())
            {
                case CampaignFlowCoordinator _:
                    menuType = MenuType.Campaign;
                    break;
                case SinglePlayerLevelSelectionFlowCoordinator _:
                    menuType = MenuType.Solo;
                    break;
                case HostGameServerLobbyFlowCoordinator _:
                case QuickPlayLobbyFlowCoordinator _:
                case GameServerLobbyFlowCoordinator _:
                    menuType = MenuType.Online;
                    break;
                default:
                    menuType = MenuType.Custom;
                    break;
            }
            foreach (GameplaySetupMenu menu in menus)
                menu.SetVisible(menu.IsMenuType(menuType));
            /*switch (BeatSaberUI.MainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf().GetType())
            {
                case typeof(CampaignFlowCoordinator):
                    break;
            }*/
        }

        public void AddTab(string name, string resource, object host)
        {
            AddTab(Assembly.GetCallingAssembly(), name, resource, host, MenuType.All);
        }

        public void AddTab(string name, string resource, object host, MenuType menuType)
        {
            AddTab(Assembly.GetCallingAssembly(), name, resource, host, menuType);
        }

        private void AddTab(Assembly assembly, string name, string resource, object host, MenuType menuType)
        {
            if (menus.Any(x => (x as GameplaySetupMenu).name == name))
                return;
            menus.Add(new GameplaySetupMenu(name, resource, host, assembly, menuType));
        }

        /// <summary>Warning, for now it will not be removed until fresh menu scene reload</summary>
        public void RemoveTab(string name)
        {
            IEnumerable<object> menu = menus.Where(x => (x as GameplaySetupMenu).name == name);
            if (menu.Count() > 0)
                menus.Remove(menu.FirstOrDefault());
        }
    }
}
