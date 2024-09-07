using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Util;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage.GameplaySetup
{
    public class GameplaySetup : ZenjectSingleton<GameplaySetup>, TableView.IDataSource, IInitializable, IDisposable, INotifyPropertyChanged
    {
        public const string ReuseIdentifier = "GameplaySetupCell";

        private readonly MainFlowCoordinator mainFlowCoordinator;
        private readonly GameplaySetupViewController gameplaySetupViewController;
        private readonly HierarchyManager hierarchyManager;

        private LayoutGroup layoutGroup;
        private bool listParsed;
        private bool loaded;

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
        private List<Transform> vanillaItems = new();

        [UIValue("mod-menus")]
        private List<GameplaySetupMenu> menus = new();

        private GameplaySetup(MainFlowCoordinator mainFlowCoordinator, GameplaySetupViewController gameplaySetupViewController, HierarchyManager hierarchyManager)
        {
            this.mainFlowCoordinator = mainFlowCoordinator;
            this.gameplaySetupViewController = gameplaySetupViewController;
            this.hierarchyManager = hierarchyManager;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
            if (!gameplaySetupViewController.isActiveAndEnabled)
            {
                return;
            }

            GameplaySetupMenu menu = menus.OfType<GameplaySetupMenu>().Where(x => x.Name == name).FirstOrDefault();
            menu?.SetVisible(isVisible);
        }

        /// <summary>
        /// Warning, for now it will not be removed until fresh menu scene reload.
        /// </summary>
        /// <param name="name">The name of the tab.</param>
        public void RemoveTab(string name)
        {
            menus.RemoveAll(m => m.Name == name);

            if (rootObject != null)
            {
                RefreshView();
            }
        }

        public float CellSize(int idx) => 8f;

        public int NumberOfCells() => menus.Count;

        public TableCell CellForIdx(TableView tableView, int idx) => GetCell().PopulateCell(menus[idx]);

        public void Initialize()
        {
            if (menus.Count == 0)
            {
                return;
            }

            vanillaItems.Clear();
            foreach (Transform transform in gameplaySetupViewController.transform)
            {
                if (transform.name != "HeaderPanel")
                {
                    vanillaItems.Add(transform);
                }
            }

            RectTransform textSegmentedControl = gameplaySetupViewController.transform.Find("TextSegmentedControl") as RectTransform;
            textSegmentedControl.sizeDelta = new Vector2(0, 6);
            layoutGroup = textSegmentedControl.GetComponent<LayoutGroup>();
            RefreshView();

            modsList.TableView.SetDataSource(this, false);
            gameplaySetupViewController.didActivateEvent += GameplaySetupDidActivate;
            gameplaySetupViewController.didDeactivateEvent += GameplaySetupDidDeactivate;
            listModal.blockerClickedEvent += ClickedOffModal;
        }

        public void Dispose()
        {
            gameplaySetupViewController.didActivateEvent -= GameplaySetupDidActivate;
            gameplaySetupViewController.didDeactivateEvent -= GameplaySetupDidDeactivate;

            if (listModal != null)
            {
                listModal.blockerClickedEvent -= ClickedOffModal;
            }

            Object.Destroy(rootObject);
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Logger.Log?.Error($"Error invoking PropertyChanged for property '{propertyName}' on {GetType().FullName}\n{ex}");
            }
        }

        private void RefreshView()
        {
            // the isBeingDestroyed check is kind of a hack but it works
            if (gameplaySetupViewController == null || hierarchyManager._screenSystem.mainScreen.isBeingDestroyed)
            {
                return;
            }

            Object.Destroy(rootObject);

            BSMLParser.Instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.gameplay-setup.bsml"), gameplaySetupViewController.gameObject, this);

            listParsed = false;
        }

        private void AddTab(Assembly assembly, string name, string resource, object host, MenuType menuType)
        {
            if (menus.Any(m => m.Name == name))
            {
                return;
            }

            GameplaySetupMenu menu = new(name, resource, host, assembly, menuType);
            menus.Add(menu);

            if (rootObject != null)
            {
                RefreshView();
            }
        }

        private GameplaySetupCell GetCell()
        {
            TableCell tableCell = modsList.TableView.DequeueReusableCellForIdentifier(ReuseIdentifier);

            if (tableCell == null)
            {
                tableCell = new GameObject(nameof(GameplaySetupCell)).AddComponent<GameplaySetupCell>();
                tableCell.interactable = true;

                tableCell.reuseIdentifier = ReuseIdentifier;
                BSMLParser.Instance.Parse(
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

            TabsCreatedEvent?.Invoke();
        }

        private void GameplaySetupDidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            tabSelector.TextSegmentedControl.SelectCellWithNumber(0);
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
                    modsList.TableView.ReloadData();
                    listParsed = true;
                }

                modsList.TableView.RefreshContentSize();
                Loaded = true;
            });
        }
    }
}
