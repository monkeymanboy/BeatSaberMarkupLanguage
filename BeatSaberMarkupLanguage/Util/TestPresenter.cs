#if DEBUG
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

#pragma warning disable IDE0051, IDE0052

namespace BeatSaberMarkupLanguage.Util
{
    internal class TestPresenter : IInitializable
    {
        public void Initialize()
        {
            // GameplaySetup.GameplaySetup.instance.AddTab("Test", "BeatSaberMarkupLanguage.Views.gameplay-setup-test.bsml", new GameplaySetup.GameplaySetupTest());
            // Settings.BSMLSettings.instance.AddSettingsMenu("Test", "BeatSaberMarkupLanguage.Views.settings-test.bsml", new Settings.SettingsTest());
            // _ = PresentTest<ViewControllers.TestViewController>();
            // _ = PresentTest<ViewControllers.LocalizationTestViewController>();
            // MenuButtons.MenuButtons.instance.RegisterButton(new MenuButtons.MenuButton("test", () => MenuButtons.MenuButtons.instance.RegisterButton(new MenuButtons.MenuButton("test2", null))));
        }

        private async Task PresentTest<T>()
            where T : HMUI.ViewController
        {
            await Task.Delay(1000);
            HMUI.ViewController testViewController = BeatSaberUI.CreateViewController<T>();
            FloatingScreen.FloatingScreen floatingScreen = FloatingScreen.FloatingScreen.CreateFloatingScreen(new Vector2(400, 200), true, Vector3.zero, Quaternion.identity);
            floatingScreen.SetRootViewController(testViewController, HMUI.ViewController.AnimationType.None);
            Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First().PresentViewController(testViewController, null, HMUI.ViewController.AnimationDirection.Horizontal, false);
        }
    }
}
#endif
