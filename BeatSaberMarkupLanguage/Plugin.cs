using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using BS_Utils.Utilities;
using IPA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage
{
    public class Plugin : IBeatSaberPlugin
    {
        public void OnApplicationStart()
        {
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {

        }

        public void OnSceneUnloaded(Scene scene)
        {

        }
        public void OnActiveSceneChanged(Scene _, Scene scene)
        {
            if (_.name == "PCInit" && scene.name == "EmptyTransition")
                GameObject.Destroy(BSMLSettings.instance.gameObject);//For if the game is restarted
            if (scene.name == "MenuCore")
            {
                //BSMLSettings.instance.AddSettingsMenu("Test", "BeatSaberMarkupLanguage.Views.settings-test.bsml", SettingsTest.instance);
                //Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault().StartCoroutine(PresentTest());
                if (BSMLSettings.instance.settingsMenus.Count > 0) BSMLSettings.instance.StartCoroutine(BSMLSettings.instance.AddButtonToMainScreen());
            }
        }

        //It's just for testing so don't yell at me
        private IEnumerator PresentTest()
        {
            yield return new WaitForSeconds(1);
            TestViewController testViewController = BeatSaberUI.CreateViewController<TestViewController>();
            testViewController.didActivate += delegate
            {
                List<CustomCellInfo> test = new List<CustomCellInfo>();
                for (int i = 0; i < 10; i++)
                {
                    test.Add(new CustomCellInfo("test" + i, "yee haw"));
                }
                testViewController.tableData.data = test;
                testViewController.tableData.tableView.ReloadData();
            };
            Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First().InvokeMethod("PresentViewController", new object[] { testViewController, null, false });
        }

        public void OnApplicationQuit()
        {
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }
    }
}
