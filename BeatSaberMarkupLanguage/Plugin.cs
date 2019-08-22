using BeatSaberMarkupLanguage.ViewControllers;
using BS_Utils.Utilities;
using IPA;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            if (scene.name == "MenuCore")
            {
                Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault().StartCoroutine(PresentTest());
            }
        }

        //It's just for testing so don't yell at me
        private IEnumerator PresentTest()
        {
            yield return new WaitForSeconds(1);
            Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First().InvokeMethod("PresentViewController", new object[] { BeatSaberUI.CreateViewController<TestViewController>(), null, false });
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
