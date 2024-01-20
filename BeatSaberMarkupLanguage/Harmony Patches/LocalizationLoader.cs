using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BGLib.Polyglot;
using HarmonyLib;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Harmony_Patches
{
    [HarmonyPatch(typeof(LocalizationAsyncInstaller), "LoadResourcesBeforeInstall")]
    internal class LocalizationLoader
    {
        public static void Prefix(IList<TextAsset> assets)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BeatSaberMarkupLanguage.Resources.beat-saber-markup-language.csv"))
            using (StreamReader reader = new(stream))
            {
                string content = reader.ReadToEnd();
                assets.Add(new TextAsset(content));
            }
        }
    }
}
