using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Util;
using HMUI;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    public class GameplaySetup : ZenjectSingleton<GameplaySetup>, TableView.IDataSource, IInitializable, IDisposable
    {
        private const string ReuseIdentifier = "GameplaySetupCell";

        private readonly MainFlowCoordinator mainFlowCoordinator;
        private readonly GameplaySetupViewController gameplaySetupViewController;
        private readonly HierarchyManager hierarchyManager;
        private readonly ICoroutineStarter coroutineStarter;

        private Coroutine debounceCoroutine;

        [UIObject("root-object")]
        private GameObject rootObject;

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
        private List<Transform> vanillaItems = [];

        [UIValue("mod-menus")]
        private List<GameplaySetupMenu> menus = [];

        private GameplaySetup(MainFlowCoordinator mainFlowCoordinator, GameplaySetupViewController gameplaySetupViewController, HierarchyManager hierarchyManager, ICoroutineStarter coroutineStarter)
        {
            this.mainFlowCoordinator = mainFlowCoordinator;
            this.gameplaySetupViewController = gameplaySetupViewController;
            this.hierarchyManager = hierarchyManager;
            this.coroutineStarter = coroutineStarter;
        }

        [UIValue("has-menus")]
        private bool HasMenus => menus.Count > 0;

        [UIValue("anchored-position")]
        private float AnchoredPosition => HasMenus ? -4 : 0;

        [UIValue("size-delta")]
        private float SizeDelta => HasMenus ? -8 : 0;

        public void AddTab(string name, string resource, object host)
        {
            AddTab(Assembly.GetCallingAssembly(), name, resource, host, MenuType.All);
        }

        public void AddTab(string name, string resource, object host, MenuType menuType)
        {
            AddTab(Assembly.GetCallingAssembly(), name, resource, host, menuType);
        }

        /// <summary>
        /// Remove a tab.
        /// </summary>
        /// <param name="name">The name of the tab.</param>
        public void RemoveTab(string name)
        {
            menus.RemoveAll(m => m.Name == name);

            if (rootObject != null)
            {
                QueueRefreshView();
            }
        }

        /// <inheritdoc />
        public float CellSize(int idx) => 8f;

        /// <inheritdoc />
        public int NumberOfCells() => menus.Count;

        /// <inheritdoc />
        public TableCell CellForIdx(TableView tableView, int idx)
        {
            GameplaySetupCell tableCell = (GameplaySetupCell)modsList.TableView.DequeueReusableCellForIdentifier(ReuseIdentifier);

            if (tableCell == null)
            {
                tableCell = new GameObject(nameof(GameplaySetupCell)).AddComponent<GameplaySetupCell>();
                tableCell.interactable = true;
                tableCell.reuseIdentifier = ReuseIdentifier;

                BSMLParser.Instance.Parse(
                    Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.gameplay-setup-cell.bsml"),
                    tableCell.gameObject,
                    tableCell);
            }

            tableCell.PopulateCell(menus[idx]);

            return tableCell;
        }

        /// <inheritdoc />
        public void Initialize()
        {
            foreach (Transform transform in gameplaySetupViewController.transform)
            {
                if (transform.name != "HeaderPanel")
                {
                    vanillaItems.Add(transform);
                }
            }

            QueueRefreshView();

            gameplaySetupViewController.didActivateEvent += GameplaySetupDidActivate;
            gameplaySetupViewController.didDeactivateEvent += GameplaySetupDidDeactivate;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            gameplaySetupViewController.didActivateEvent -= GameplaySetupDidActivate;
            gameplaySetupViewController.didDeactivateEvent -= GameplaySetupDidDeactivate;
        }

        private void QueueRefreshView()
        {
            debounceCoroutine ??= coroutineStarter.StartCoroutine(RefreshViewCoroutine());
        }

        private IEnumerator RefreshViewCoroutine()
        {
            yield return new WaitForEndOfFrame();
            RefreshView();
            debounceCoroutine = null;
        }

        private void RefreshView()
        {
            // the isBeingDestroyed check is kind of a hack but it works
            if (gameplaySetupViewController == null || hierarchyManager._screenSystem.mainScreen.isBeingDestroyed)
            {
                return;
            }

            if (listModal != null)
            {
                listModal.blockerClickedEvent -= ClickedOffModal;
            }

            Object.Destroy(rootObject);

            BSMLParser.Instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.gameplay-setup.bsml"), gameplaySetupViewController.gameObject, this);

            UpdateTabsVisibility();

            modsList.TableView.SetDataSource(this, true);
            listModal.blockerClickedEvent += ClickedOffModal;
        }

        private void AddTab(Assembly assembly, string name, string resource, object host, MenuType menuType)
        {
            if (menus.Any(m => m.Name == name))
            {
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentNullException(nameof(resource));
            }

            GameplaySetupMenu menu = new(name, resource, host, assembly, menuType);
            menus.Add(menu);

            if (rootObject != null)
            {
                QueueRefreshView();
            }
        }

        private void GameplaySetupDidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            UpdateTabsVisibility();
        }

        private void GameplaySetupDidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            tabSelector.TextSegmentedControl.SelectCellWithNumber(0);
            vanillaTab.gameObject.SetActive(true);
            modsTab.gameObject.SetActive(false);
        }

        private void ClickedOffModal()
        {
            UpdateTabsVisibility();
        }

        private void UpdateTabsVisibility()
        {
            MenuType menuType = mainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf() switch
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
        }

        [UIAction("show-modal")]
        private void ShowModal()
        {
            listModal.Show(true, true);
        }
    }
}
