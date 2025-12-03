using System;
using BeatSaberMarkupLanguage.Animations;
using BeatSaberMarkupLanguage.Macros;
using BeatSaberMarkupLanguage.MenuButtons;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using BeatSaberMarkupLanguage.Util;
using HarmonyLib;
using Zenject;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(BeatSaberInit), nameof(BeatSaberInit.InstallBindings))]
    internal static class BeatSaberInit_InstallBindings
    {
        private static void Prefix(BeatSaberInit __instance)
        {
            DiContainer container = __instance.Container;
            container.Bind(typeof(AnimationController), typeof(ITickable)).To<AnimationController>().AsSingle();
        }
    }

    [HarmonyPatch(typeof(MainSettingsMenuViewControllersInstaller), nameof(MainSettingsMenuViewControllersInstaller.InstallBindings))]
    internal static class Context_InstallInstallers
    {
        private static void Prefix(MainSettingsMenuViewControllersInstaller __instance)
        {
            DiContainer container = __instance.Container;

            BeatSaberUI.Init(container);

            container.Bind(typeof(AnimationController), typeof(IInitializable), typeof(ITickable)).To<AnimationController>().AsSingle().NonLazy();
            container.Bind(typeof(BSMLSettings), typeof(IInitializable)).To<BSMLSettings>().AsSingle().NonLazy();
            container.Bind(typeof(BSMLParser), typeof(IInitializable)).To<BSMLParser>().AsSingle().NonLazy();
            container.Bind(typeof(MenuButtons.MenuButtons)).To<MenuButtons.MenuButtons>().AsSingle().NonLazy();
            container.Bind(typeof(GameplaySetup.GameplaySetup), typeof(IInitializable), typeof(IDisposable)).To<GameplaySetup.GameplaySetup>().AsSingle().NonLazy();

            // initialize early & dispose late
            container.BindExecutionOrder<AnimationController>(-900);
            container.BindExecutionOrder<BSMLSettings>(-900);
            container.BindExecutionOrder<BSMLParser>(-1000);
            container.BindExecutionOrder<GameplaySetup.GameplaySetup>(-900);

            container.Bind(typeof(ModSettingsFlowCoordinator)).FromInstance(BeatSaberUI.CreateFlowCoordinator<ModSettingsFlowCoordinator>());
            container.Bind(typeof(MenuButtonsViewController)).FromInstance(BeatSaberUI.CreateViewController<MenuButtonsViewController>());

            foreach (Type type in Utilities.GetDescendants<BSMLTag>())
            {
                container.Bind<BSMLTag>().To(type).AsSingle();
            }

            foreach (Type type in Utilities.GetDescendants<BSMLMacro>())
            {
                container.Bind<BSMLMacro>().To(type).AsSingle();
            }

            foreach (Type type in Utilities.GetDescendants<TypeHandler>())
            {
                container.Bind<TypeHandler>().To(type).AsSingle();
            }

            container.Bind(typeof(IInitializable), typeof(IDisposable)).To<MainMenuAwaiter>().AsSingle();

#if DEBUG
            container.Bind(typeof(IInitializable)).To<TestPresenter>().AsSingle();
#endif
        }
    }
}
