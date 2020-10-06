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

namespace BeatSaberMarkupLanguage
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static Config config;
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
                string[] lines = File.ReadAllLines(folder);
                string pinnnedModsLine = lines.FirstOrDefault(x => x.StartsWith("Pinned Mods"));
                var splitLine = pinnnedModsLine.Split('=');
                if (splitLine.Length > 1)
                {
                    var mods = splitLine[1].Split(',');
                    config.PinnedMods.AddRange(mods.Where(x => !string.IsNullOrWhiteSpace(x) && x != " " && !config.PinnedMods.Contains(x)));
                }
                File.Delete(folder);
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

        public void MenuLoadFresh(ScenesTransitionSetupDataSO _)
        {
            //GameplaySetup.GameplaySetup.instance.AddTab("Test", "BeatSaberMarkupLanguage.Views.gameplay-setup-test.bsml", GameplaySetupTest.instance);
            //BSMLSettings.instance.AddSettingsMenu("Test", "BeatSaberMarkupLanguage.Views.settings-test.bsml", SettingsTest.instance);
            //Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault().StartCoroutine(PresentTest());
            BSMLSettings.instance.Setup();
            MenuButtons.MenuButtons.instance.Setup();
            GameplaySetup.GameplaySetup.instance.Setup();
            Polyglot.Localization.Instance.SelectedLanguage = config.SelectedLanguage;
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name.Contains("Menu") && prevScene.name == "EmptyTransition")
            {
                BSMLParser.instance.MenuSceneLoaded();
                SharedCoroutineStarter.instance.StartCoroutine(WaitForSetup());
            }
        }

        private IEnumerator WaitForSetup()
        {
            yield return new WaitForSecondsRealtime(0.025f);
            MenuLoadFresh(null);
        }

        //It's just for testing so don't yell at me
        private IEnumerator PresentTest()
        {
            yield return new WaitForSeconds(1);
            TestViewController testViewController = BeatSaberUI.CreateViewController<TestViewController>();
            Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First().InvokeMethod<object, FlowCoordinator>("PresentViewController", new object[] { testViewController, null, false });
        }
    }
}
