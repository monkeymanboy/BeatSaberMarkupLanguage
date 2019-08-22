using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRUI;

namespace BeatSaberMarkupLanguage
{
    //This class is temporary while customui is being worked on
    public class BeatSaberUI
    {
        public static T CreateViewController<T>() where T : VRUIViewController
        {
            T vc = new GameObject("CustomViewController").AddComponent<T>();
            MonoBehaviour.DontDestroyOnLoad(vc.gameObject);

            vc.rectTransform.anchorMin = new Vector2(0f, 0f);
            vc.rectTransform.anchorMax = new Vector2(1f, 1f);
            vc.rectTransform.sizeDelta = new Vector2(0f, 0f);
            vc.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            return vc;
        }
    }
}
