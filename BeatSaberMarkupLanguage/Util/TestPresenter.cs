#if DEBUG
using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;

#pragma warning disable IDE0051, IDE0052

namespace BeatSaberMarkupLanguage.Util
{
    internal class TestPresenter : IInitializable
    {
        private readonly ICoroutineStarter _coroutineStarter;

        private TestPresenter(ICoroutineStarter coroutineStarter)
        {
            _coroutineStarter = coroutineStarter;
        }

        public void Initialize()
        {
            // GameplaySetup.GameplaySetup.instance.AddTab("Test", "BeatSaberMarkupLanguage.Views.gameplay-setup-test.bsml", new GameplaySetup.GameplaySetupTest());
            // Settings.BSMLSettings.instance.AddSettingsMenu("Test", "BeatSaberMarkupLanguage.Views.settings-test.bsml", new Settings.SettingsTest());
            // _coroutineStarter.StartCoroutine(PresentTest<ViewControllers.TestViewController>());
            // _coroutineStarter.StartCoroutine(PresentTest<ViewControllers.LocalizationTestViewController>());
            // MenuButtons.MenuButtons.instance.RegisterButton(new MenuButtons.MenuButton("test", () => MenuButtons.MenuButtons.instance.RegisterButton(new MenuButtons.MenuButton("test2", null))));
        }

        private IEnumerator PresentTest<T>()
            where T : HMUI.ViewController
        {
            yield return new WaitForSeconds(1);
            HMUI.ViewController testViewController = BeatSaberUI.CreateViewController<T>();
            FloatingScreen.FloatingScreen floatingScreen = FloatingScreen.FloatingScreen.CreateFloatingScreen(new Vector2(400, 200), true, Vector3.zero, Quaternion.identity);
            floatingScreen.SetRootViewController(testViewController, HMUI.ViewController.AnimationType.None);
            Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First().PresentViewController(testViewController, null, HMUI.ViewController.AnimationDirection.Horizontal, false);
        }
    }
}
#endif
