using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using Conf = IPA.Config.Config;
using IPALogger = IPA.Logging.Logger;

[assembly: InternalsVisibleTo("BSML.BeatmapEditor", AllInternalsVisible = true)]

namespace BeatSaberMarkupLanguage
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Config Config { get; private set; }

        [Init]
        public void Init(Conf conf, IPALogger logger)
        {
            Logger.Log = logger;

            try
            {
                Harmony harmony = new("com.monkeymanboy.BeatSaberMarkupLanguage");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Failed to apply Harmony patches\n{ex}");
            }

            Config = conf.Generated<Config>();
        }

        [OnStart]
        public void OnStart()
        {
            FontManager.AsyncLoadSystemFonts().ContinueWith((task) => Logger.Log.Error($"Failed to load system fonts\n{task.Exception}"), TaskContinuationOptions.OnlyOnFaulted);
        }

        [OnExit]
        public void OnExit()
        {
        }
    }
}
