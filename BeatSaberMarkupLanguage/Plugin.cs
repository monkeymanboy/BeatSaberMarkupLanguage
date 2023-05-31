using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Animations;
using BeatSaberMarkupLanguage.Settings;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPA.Utilities;
using IPA.Utilities.Async;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Conf = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

[assembly: InternalsVisibleTo("BSML.BeatmapEditor", AllInternalsVisible = true)]

namespace BeatSaberMarkupLanguage
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static Config config;

        private static readonly string[] FontNamesToRemove = { "NotoSansJP-Medium SDF", "NotoSansKR-Medium SDF", "SourceHanSansCN-Bold-SDF-Common-1(2k)", "SourceHanSansCN-Bold-SDF-Common-2(2k)", "SourceHanSansCN-Bold-SDF-Uncommon(2k)" };

        private static bool hasInited = false;

        private GameScenesManager gameScenesManager;

        [Init]
        public void Init(Conf conf, IPALogger logger)
        {
            Logger.Log = logger;
            try
            {
                Harmony harmony = new Harmony("com.monkeymanboy.BeatSaberMarkupLanguage");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message);
            }

            AnimationController.instance.InitializeLoadingAnimation();
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            config = conf.Generated<Config>();

            // Old Config Migration
            Task.Run(() =>
            {
                var folder = Path.Combine(UnityGame.UserDataPath, "BSML.ini");
                if (File.Exists(folder))
                {
                    string[] lines = File.ReadAllLines(folder);
                    string pinnnedModsLine = lines.Where(x => x.StartsWith("Pinned Mods", StringComparison.Ordinal)).FirstOrDefault() ?? string.Empty;
                    var splitLine = pinnnedModsLine.Split('=');
                    if (splitLine.Length > 1)
                    {
                        var mods = splitLine[1].Split(',');
                        config.PinnedMods.AddRange(mods.Where(x => !string.IsNullOrWhiteSpace(x) && x != " " && !config.PinnedMods.Contains(x)));
                    }

                    File.Delete(folder);
                }
            });
        }

        [OnStart]
        public void OnStart()
        {
            FontManager.AsyncLoadSystemFonts().ContinueWith(
                task =>
                {
                    if (!FontManager.TryGetTMPFontByFullName("Segoe UI", out TMP_FontAsset fallback) &&
                        !FontManager.TryGetTMPFontByFamily("Arial", out fallback))
                    {
                        Logger.Log.Warn("Could not find fonts for either Segoe UI or Arial to set up fallbacks");
                        return;
                    }

                    IEnumerator SetupFont()
                    {
                        yield return new WaitUntil(() => BeatSaberUI.MainTextFont != null);
                        Logger.Log.Debug("Setting up default font fallbacks");

                        // remove built-in fallback fonts to avoid inconsistencies between CJK characters
                        BeatSaberUI.MainTextFont.fallbackFontAssets.RemoveAll((asset) => FontNamesToRemove.Contains(asset.name));
                        BeatSaberUI.MainTextFont.fallbackFontAssetTable.RemoveAll((asset) => FontNamesToRemove.Contains(asset.name));
                        BeatSaberUI.MainTextFont.fallbackFontAssetTable.Add(fallback);
                    }

                    Logger.Log.Debug("Waiting for default font presence");
                    if (fallback != null)
                    {
                        SharedCoroutineStarter.instance.StartCoroutine(SetupFont());
                    }
                },
                UnityMainThreadTaskScheduler.Default)
            .ContinueWith(
                t =>
                {
                    Logger.Log.Error("Errored while setting up fallback fonts:");
                    Logger.Log.Error(t.Exception);
                },
                TaskContinuationOptions.NotOnRanToCompletion);
        }

        [OnExit]
        public void OnExit()
        {
        }

        public void MenuLoadFresh(ScenesTransitionSetupDataSO scenesTransitionSetupData, DiContainer diContainer)
        {
            // GameplaySetup.GameplaySetup.instance.AddTab("Test", "BeatSaberMarkupLanguage.Views.gameplay-setup-test.bsml", GameplaySetupTest.instance);
            // BSMLSettings.instance.AddSettingsMenu("Test", "BeatSaberMarkupLanguage.Views.settings-test.bsml", SettingsTest.instance);
            // SharedCoroutineStarter.instance.StartCoroutine(PresentTest<TestViewController>());
            // SharedCoroutineStarter.instance.StartCoroutine(PresentTest<LocalizationTestViewController>());
            // MenuButtons.MenuButtons.instance.RegisterButton(new MenuButtons.MenuButton("test", () => MenuButtons.MenuButtons.instance.RegisterButton(new MenuButtons.MenuButton("test2",null))));
            BSMLSettings.instance.Setup();
            MenuButtons.MenuButtons.instance.Setup();
            GameplaySetup.GameplaySetup.instance.Setup();
            gameScenesManager.installEarlyBindingsEvent -= OnInstallEarlyBindings;
            gameScenesManager.transitionDidFinishEvent -= MenuLoadFresh;
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (prevScene.name == "PCInit")
            {
                hasInited = true;
            }

            if (hasInited && nextScene.name.Contains("Menu") && prevScene.name == "EmptyTransition")
            {
                hasInited = false;
                BSMLParser.instance.MenuSceneLoaded();
                BeatSaberUI.MainTextFont.boldSpacing = 2.2f; // default bold spacing is rather  w i d e
                if (gameScenesManager == null)
                {
                    gameScenesManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();
                }

                // installEarlyBindingsEvent is invoked by Zenject's LoadSceneAsync so we get the container as early as possible without using another FindObjectsOfTypeAll
                gameScenesManager.installEarlyBindingsEvent += OnInstallEarlyBindings;
                gameScenesManager.transitionDidFinishEvent += MenuLoadFresh;
            }
        }

        private void OnInstallEarlyBindings(ScenesTransitionSetupDataSO setupData, DiContainer diContainer)
        {
            BeatSaberUI.DiContainer = diContainer;
        }

        // It's just for testing so don't yell at me
        /*private IEnumerator PresentTest<T>()
            where T : ViewController
        {
            yield return new WaitForSeconds(1);
            ViewController testViewController = BeatSaberUI.CreateViewController<T>();
            FloatingScreen.FloatingScreen floatingScreen = FloatingScreen.FloatingScreen.CreateFloatingScreen(new Vector2(400, 200), true, Vector3.zero, Quaternion.identity);
            floatingScreen.SetRootViewController(testViewController, ViewController.AnimationType.None);
            Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First().PresentViewController(testViewController, null, ViewController.AnimationDirection.Horizontal, false);
        }*/
    }
}
