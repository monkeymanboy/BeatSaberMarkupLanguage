using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class BoolSettingTag : IncDecSettingTag<BoolSetting>
    {
        public override string[] Aliases => new[] { "bool-setting" };
    }
}
