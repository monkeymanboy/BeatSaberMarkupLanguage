using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags.Settings
{
    public class ListSettingTag : BSMLTag
    {
        public override string[] Aliases => new[] { "list-setting" };

        public override GameObject CreateObject(Transform parent)
        {
            return null;//
        }
    }
}
