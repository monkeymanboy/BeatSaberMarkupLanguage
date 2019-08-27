using BeatSaberMarkupLanguage.Components.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class ListSettingTag : IncDecSettingTag<ListSetting>
    {
        public override string[] Aliases => new[] { "list-setting" };
    }
}
