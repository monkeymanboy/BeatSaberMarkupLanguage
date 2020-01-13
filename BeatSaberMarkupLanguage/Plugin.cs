using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using BS_Utils.Utilities;
using Harmony;
using IPA;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace BeatSaberMarkupLanguage
{
    public class Plugin : IBeatSaberPlugin
    {
        public static Config config;
        public void Init(IPALogger logger)
        {
            try
            {
                HarmonyInstance harmony = HarmonyInstance.Create("com.monkeymanboy.BeatSaberMarkupLanguage");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Logger.log.Error(e.Message);
            }

            Logger.log = logger;
            BSEvents.menuSceneLoadedFresh += MenuLoadFresh;
            config = new Config("BSML");
        }
        public void MenuLoadFresh()
        {
            //GameplaySetup.GameplaySetup.instance.AddTab("Test", "BeatSaberMarkupLanguage.Views.gameplay-setup-test.bsml", GameplaySetupTest.instance);
            //BSMLSettings.instance.AddSettingsMenu("Test", "BeatSaberMarkupLanguage.Views.settings-test.bsml", SettingsTest.instance);
            //Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault().StartCoroutine(PresentTest());
            BSMLSettings.instance.Setup();
            MenuButtons.MenuButtons.instance.Setup();
            GameplaySetup.GameplaySetup.instance.Setup();
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) { }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "MenuViewControllers" && prevScene.name == "EmptyTransition")
                BSMLParser.instance.MenuSceneLoaded();
        }

        public void OnApplicationStart() { }
        public void OnApplicationQuit() { }
        public void OnSceneUnloaded(Scene scene) { }
        public void OnUpdate() { }
        public void OnFixedUpdate() { }

        //It's just for testing so don't yell at me
        private IEnumerator PresentTest()
        {
            yield return new WaitForSeconds(1);
            TestViewController testViewController = BeatSaberUI.CreateViewController<TestViewController>();
            Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First().InvokeMethod("PresentViewController", new object[] { testViewController, null, false });
        }
    }
}
