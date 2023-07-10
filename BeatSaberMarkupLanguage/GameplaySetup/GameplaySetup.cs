using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Util;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    public class GameplaySetup : NotifiableSingleton<GameplaySetup>, TableView.IDataSource, IInitializable, IDisposable, ILateDisposable
    {
        public const string ReuseIdentifier = "GameplaySetupCell";

        private MainFlowCoordinator _mainFlowCoordinator;
        private GameplaySetupViewController _gameplaySetupViewController;

        private LayoutGroup layoutGroup;
        private bool listParsed;
        private bool loaded;

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
        private List<Transform> vanillaItems = new();

        [UIValue("mod-menus")]
        private List<GameplaySetupMenu> menus = new();

        public event Action TabsCreatedEvent;

        [UIValue("is-loading")]
        public bool IsLoading => !Loaded;

        [UIValue("loaded")]
        public bool Loaded
        {
            get => loaded;
            set
            {
                loaded = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsLoading));
            }
        }

        public void AddTab(string name, string resource, object host)
        {
            AddTab(Assembly.GetCallingAssembly(), name, resource, host, MenuType.All);
        }

        public void AddTab(string name, string resource, object host, MenuType menuType)
        {
            AddTab(Assembly.GetCallingAssembly(), name, resource, host, menuType);
        }

        /// <summary>
        /// Allows tab to dynamically disappear and reappear.
        /// </summary>
        /// <param name="name">The name of the tab.</param>
        /// <param name="isVisible">Whether or not the tab should be visible.</param>
        public void SetTabVisibility(string name, bool isVisible)
        {
            if (!_gameplaySetupViewController.isActiveAndEnabled)
            {
                return;
            }

            GameplaySetupMenu menu = menus.OfType<GameplaySetupMenu>().Where(x => x.name == name).FirstOrDefault();
            menu?.SetVisible(isVisible);
        }

        /// <summary>
        /// Warning, for now it will not be removed until fresh menu scene reload.
        /// </summary>
        /// <param name="name">The name of the tab.</param>
        public void RemoveTab(string name)
        {
            menus.RemoveAll(m => m.name == name);
        }

        public float CellSize() => 8f;

        public int NumberOfCells() => menus.Count;

        public TableCell CellForIdx(TableView tableView, int idx) => GetCell().PopulateCell(menus[idx]);

        public void Initialize()
        {
            if (menus.Count == 0)
            {
                return;
            }

            vanillaItems.Clear();
            foreach (Transform transform in _gameplaySetupViewController.transform)
            {
                if (transform.name != "HeaderPanel")
                {
                    vanillaItems.Add(transform);
                }
            }

            RectTransform textSegmentedControl = _gameplaySetupViewController.transform.Find("TextSegmentedControl") as RectTransform;
            textSegmentedControl.sizeDelta = new Vector2(0, 6);
            layoutGroup = textSegmentedControl.GetComponent<LayoutGroup>();
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.gameplay-setup.bsml"), _gameplaySetupViewController.gameObject, this);

            modsList.tableView.SetDataSource(this, false);
            listParsed = false;
            _gameplaySetupViewController.didActivateEvent += GameplaySetupDidActivate;
            _gameplaySetupViewController.didDeactivateEvent += GameplaySetupDidDeactivate;
            listModal.blockerClickedEvent += ClickedOffModal;
        }

        public void Dispose()
        {
            _gameplaySetupViewController.didActivateEvent -= GameplaySetupDidActivate;
            _gameplaySetupViewController.didDeactivateEvent -= GameplaySetupDidDeactivate;
        }

        public void LateDispose()
        {
            _mainFlowCoordinator = null;
            _gameplaySetupViewController = null;
        }

        [Inject]
        private void Construct(MainFlowCoordinator mainFlowCoordinator, GameplaySetupViewController gameplaySetupViewController)
        {
            _mainFlowCoordinator = mainFlowCoordinator;
            _gameplaySetupViewController = gameplaySetupViewController;
        }

        private void AddTab(Assembly assembly, string name, string resource, object host, MenuType menuType)
        {
            if (menus.Any(m => m.name == name))
            {
                return;
            }

            menus.Add(new GameplaySetupMenu(name, resource, host, assembly, menuType));
        }

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

        private void GameplaySetupDidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            layoutGroup.m_RectChildren.Clear();

            MenuType menuType = _mainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf() switch
            {
                CampaignFlowCoordinator => MenuType.Campaign,
                SinglePlayerLevelSelectionFlowCoordinator => MenuType.Solo,
                GameServerLobbyFlowCoordinator => MenuType.Online,
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
    }
}
