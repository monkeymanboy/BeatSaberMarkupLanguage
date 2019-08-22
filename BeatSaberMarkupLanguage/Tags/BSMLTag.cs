using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public abstract class BSMLTag
    {
        public abstract string[] Aliases
        {
            get;
        }

        public abstract GameObject CreateObject(Transform parent);
    }
}
