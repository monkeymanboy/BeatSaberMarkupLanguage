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

        private LayoutGroup _layoutGroup;
        private bool _listParsed;
        private bool _loaded;

        [UIComponent("new-tab-selector")]
        private TabSelector _tabSelector;

        [UIComponent("vanilla-tab")]
        private Transform _vanillaTab;

        [UIComponent("mods-tab")]
        private Transform _modsTab;

        [UIComponent("list-modal")]
        private ModalView _listModal;

        [UIComponent("mods-list")]
        private CustomListTableData _modsList;

        [UIValue("vanilla-items")]
        private List<Transform> _vanillaItems = new();

        [UIValue("mod-menus")]
        private List<GameplaySetupMenu> _menus = new();

        public event Action TabsCreatedEvent;

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

            GameplaySetupMenu menu = _menus.OfType<GameplaySetupMenu>().Where(x => x.name == name).FirstOrDefault();
            menu?.SetVisible(isVisible);
        }

        /// <summary>
        /// Warning, for now it will not be removed until fresh menu scene reload.
        /// </summary>
        /// <param name="name">The name of the tab.</param>
        public void RemoveTab(string name)
        {
            _menus.RemoveAll(m => m.name == name);
        }

        public float CellSize() => 8f;

        public int NumberOfCells() => _menus.Count;

        public TableCell CellForIdx(TableView tableView, int idx) => GetCell().PopulateCell(_menus[idx]);

        public void Initialize()
        {
            if (_menus.Count == 0)
            {
                return;
            }

            _vanillaItems.Clear();
            foreach (Transform transform in _gameplaySetupViewController.transform)
            {
                if (transform.name != "HeaderPanel")
                {
                    _vanillaItems.Add(transform);
                }
            }

            RectTransform textSegmentedControl = _gameplaySetupViewController.transform.Find("TextSegmentedControl") as RectTransform;
            textSegmentedControl.sizeDelta = new Vector2(0, 6);
            _layoutGroup = textSegmentedControl.GetComponent<LayoutGroup>();
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.gameplay-setup.bsml"), _gameplaySetupViewController.gameObject, this);

            _modsList.tableView.SetDataSource(this, false);
            _listParsed = false;
            _gameplaySetupViewController.didActivateEvent += GameplaySetupDidActivate;
            _gameplaySetupViewController.didDeactivateEvent += GameplaySetupDidDeactivate;
            _listModal.blockerClickedEvent += ClickedOffModal;
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
            if (_menus.Any(m => m.name == name))
            {
                return;
            }

            _menus.Add(new GameplaySetupMenu(name, resource, host, assembly, menuType));
        }

        private GameplaySetupCell GetCell()
        {
            TableCell tableCell = _modsList.tableView.DequeueReusableCellForIdentifier(ReuseIdentifier);

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
            _layoutGroup.m_RectChildren.Clear();

            MenuType menuType = _mainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf() switch
            {
                CampaignFlowCoordinator => MenuType.Campaign,
                SinglePlayerLevelSelectionFlowCoordinator => MenuType.Solo,
                GameServerLobbyFlowCoordinator => MenuType.Online,
                _ => MenuType.Custom,
            };

            foreach (GameplaySetupMenu menu in _menus)
            {
                menu.SetVisible(menu.IsMenuType(menuType));
            }

            TabsCreatedEvent?.Invoke();
        }

        private void GameplaySetupDidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _tabSelector.textSegmentedControl.SelectCellWithNumber(0);
            _vanillaTab.gameObject.SetActive(true);
            _modsTab.gameObject.SetActive(false);
        }

        private void ClickedOffModal()
        {
            GameplaySetupDidActivate(false, false, false);
        }

        [UIAction("show-modal")]
        private void ShowModal()
        {
            Loaded = false;
            _listModal.Show(true, true, () =>
            {
                if (!_listParsed)
                {
                    _modsList.tableView.ReloadData();
                    _listParsed = true;
                }

                _modsList.tableView.RefreshContentSize();
                Loaded = true;
            });
        }
    }
}
