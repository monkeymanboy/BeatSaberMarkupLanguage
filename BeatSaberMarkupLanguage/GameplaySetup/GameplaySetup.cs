using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    public class GameplaySetup : NotifiableSingleton<GameplaySetup>, TableView.IDataSource
    {
        public event Action TabsCreatedEvent;

        private GameplaySetupViewController gameplaySetupViewController;
        private LayoutGroup layoutGroup;
        private bool listParsed;

        [UIComponent("new-tab-selector")]
        private TabSelector tabSelector;

        [UIComponent("vanilla-tab")]
        private Transform vanillaTab;

        [UIComponent("mods-tab")]
        private Transform modsTab;

        [UIComponent("list-modal")]
        private ModalView listModal;

        [UIComponent("mods-list")]
        private CustomListTableData modsList;

        [UIValue("vanilla-items")]
        private List<Transform> vanillaItems = new List<Transform>();

        [UIValue("mod-menus")]
        private List<object> menus = new List<object>();

        private bool _loaded;

        [UIValue("is-loading")]
        public bool IsLoading => !Loaded;

        [UIValue("loaded")]
        public bool Loaded
        {
            get => _loaded;
            set
            {
                _loaded = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsLoading));
            }
        }

        internal void Setup()
        {
            if (menus.Count == 0) return;
            gameplaySetupViewController = Resources.FindObjectsOfTypeAll<GameplaySetupViewController>().First();
            vanillaItems.Clear();
            foreach (Transform transform in gameplaySetupViewController.transform)
            {
                if (transform.name != "HeaderPanel")
                    vanillaItems.Add(transform);
            }
            RectTransform textSegmentedControl = gameplaySetupViewController.transform.Find("TextSegmentedControl") as RectTransform;
            textSegmentedControl.sizeDelta = new Vector2(0, 6);
            layoutGroup = textSegmentedControl.GetComponent<LayoutGroup>();
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.gameplay-setup.bsml"), gameplaySetupViewController.gameObject, this);

            modsList.tableView.SetDataSource(this, false);
            listParsed = false;
            gameplaySetupViewController.didActivateEvent += GameplaySetupDidActivate;
            gameplaySetupViewController.didDeactivateEvent += GameplaySetupDidDeactivate;
            listModal.blockerClickedEvent += ClickedOffModal;
        }

        private void GameplaySetupDidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            layoutGroup.m_RectChildren.Clear();

            var menuType = BeatSaberUI.MainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf() switch
            {
                CampaignFlowCoordinator _ => MenuType.Campaign,
                SinglePlayerLevelSelectionFlowCoordinator _ => MenuType.Solo,
                GameServerLobbyFlowCoordinator _ => MenuType.Online,
                _ => MenuType.Custom,
            };

            foreach (GameplaySetupMenu menu in menus)
            {
                menu.SetVisible(menu.IsMenuType(menuType));
            }

            TabsCreatedEvent?.Invoke();
        }

        private void GameplaySetupDidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            tabSelector.textSegmentedControl.SelectCellWithNumber(0);
            vanillaTab.gameObject.SetActive(true);
            modsTab.gameObject.SetActive(false);
        }

        private void ClickedOffModal()
        {
            GameplaySetupDidActivate(false, false, false);
        }

        [UIAction("show-modal")]
        private void ShowModal()
        {
            Loaded = false;
            listModal.Show(true, true, () =>
            {
                if (!listParsed)
                {
                    modsList.tableView.ReloadData();
                    listParsed = true;
                }
                modsList.tableView.RefreshContentSize();
                Loaded = true;
            });
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

        /// <summary>Allows tab to dynamically disappear and reappear</summary>
        public void SetTabVisibility(string name, bool isVisible)
        {
            if (!gameplaySetupViewController.isActiveAndEnabled)
                return;

            IEnumerable<GameplaySetupMenu> menu = menus.OfType<GameplaySetupMenu>().Where(x => x.name == name);
            if (menu.Count() > 0)
                menu.FirstOrDefault().SetVisible(isVisible);
        }

        /// <summary>Warning, for now it will not be removed until fresh menu scene reload</summary>
        public void RemoveTab(string name)
        {
            IEnumerable<object> menu = menus.Where(x => (x as GameplaySetupMenu).name == name);
            if (menu.Count() > 0)
                menus.Remove(menu.FirstOrDefault());
        }

        #region Data Source

        public const string ReuseIdentifier = "GameplaySetupCell";
        private GameplaySetupCell GetCell()
        {
            TableCell tableCell = modsList.tableView.DequeueReusableCellForIdentifier(ReuseIdentifier);

            if (tableCell == null)
            {
                tableCell = new GameObject(nameof(GameplaySetupCell)).AddComponent<GameplaySetupCell>();
                tableCell.interactable = true;

                tableCell.reuseIdentifier = ReuseIdentifier;
                BSMLParser.instance.Parse(
                Utilities.GetResourceContent(
                    Assembly.GetExecutingAssembly(),
                    "BeatSaberMarkupLanguage.Views.gameplay-setup-cell.bsml"),
                tableCell.gameObject,
                tableCell);
            }

            return (GameplaySetupCell)tableCell;
        }

        public float CellSize() => 8f;

        public int NumberOfCells() => menus.Count;

        public TableCell CellForIdx(TableView tableView, int idx) => GetCell().PopulateCell((GameplaySetupMenu)menus[idx]);

        #endregion
    }
}
