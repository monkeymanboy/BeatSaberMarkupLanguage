using HarmonyLib;
using BeatSaberMarkupLanguage.Animations;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using IPA;
using IPA.Utilities.Async;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Conf = IPA.Config.Config;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;
using IPA.Utilities;
using HMUI;
using IPA.Config.Stores;
using System.IO;
using Zenject;

namespace BeatSaberMarkupLanguage
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static Config config;
        private static bool hasInited = false;

        private GameScenesManager gameScenesManager;
        [Init]
        public void Init(Conf conf, IPALogger logger)
        {
            Logger.log = logger;
            try
            {
                Harmony harmony = new Harmony("com.monkeymanboy.BeatSaberMarkupLanguage");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Logger.log.Error(e.Message);
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
                    string pinnnedModsLine = lines.FirstOrDefault(x => x.StartsWith("Pinned Mods")) ?? "";
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
            FontManager.AsyncLoadSystemFonts()
                .ContinueWith(_ =>
                {
                    if (!FontManager.TryGetTMPFontByFullName("Segoe UI", out TMP_FontAsset fallback))
                    {
                        if (!FontManager.TryGetTMPFontByFamily("Arial", out fallback))
                        {
                            Logger.log.Warn("Could not find fonts for either Segoe UI or Arial to set up fallbacks");
                            return;
                        }
                    }

                    IEnumerator SetupFont()
                    {
                        yield return new WaitUntil(() => BeatSaberUI.MainTextFont != null);
                        Logger.log.Debug("Setting up default font fallbacks");
                        // FontManager doesn't give fixed fonts
                        //fallback = BeatSaberUI.CreateFixedUIFontClone(fallback);
                        BeatSaberUI.MainTextFont.fallbackFontAssetTable.Add(fallback);
                    }

                    Logger.log.Debug("Waiting for default font presence");
                    if (fallback != null)
                        SharedCoroutineStarter.instance.StartCoroutine(SetupFont());
                }, UnityMainThreadTaskScheduler.Default)
                .ContinueWith(t =>
                {
                    Logger.log.Error("Errored while setting up fallback fonts:");
                    Logger.log.Error(t.Exception);
                }, TaskContinuationOptions.NotOnRanToCompletion);
        }

        public void MenuLoadFresh(ScenesTransitionSetupDataSO _, DiContainer diContainer)
        {
            GameplaySetup.GameplaySetup.instance.AddTab("Test", "BeatSaberMarkupLanguage.Views.gameplay-setup-test.bsml", GameplaySetupTest.instance);
            //BSMLSettings.instance.AddSettingsMenu("Test", "BeatSaberMarkupLanguage.Views.settings-test.bsml", SettingsTest.instance);
            //SharedCoroutineStarter.instance.StartCoroutine(PresentTest());
            //MenuButtons.MenuButtons.instance.RegisterButton(new MenuButtons.MenuButton("test", () => MenuButtons.MenuButtons.instance.RegisterButton(new MenuButtons.MenuButton("test2",null))));
            BSMLSettings.instance.Setup();
            MenuButtons.MenuButtons.instance.Setup();
            GameplaySetup.GameplaySetup.instance.Setup();
            Polyglot.Localization.Instance.SelectedLanguage = config.SelectedLanguage;
            gameScenesManager.transitionDidFinishEvent -= MenuLoadFresh;
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (prevScene.name == "PCInit")
                hasInited = true;
            if (hasInited && nextScene.name.Contains("Menu") && prevScene.name == "EmptyTransition")
            {
                hasInited = false;
                BSMLParser.instance.MenuSceneLoaded();
                if (gameScenesManager == null)
                {
                    gameScenesManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();
                }
                gameScenesManager.transitionDidFinishEvent += MenuLoadFresh;
            }
        }

        //It's just for testing so don't yell at me
        private IEnumerator PresentTest()
        {
            yield return new WaitForSeconds(1);
            TestViewController testViewController = BeatSaberUI.CreateViewController<TestViewController>();
            Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First().InvokeMethod<object, FlowCoordinator>("PresentViewController", new object[] { testViewController, null, ViewController.AnimationDirection.Horizontal, false });
        }
    }
}
