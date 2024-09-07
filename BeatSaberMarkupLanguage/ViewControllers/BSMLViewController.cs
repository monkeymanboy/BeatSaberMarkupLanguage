using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public abstract class BSMLViewController : ViewController, INotifyPropertyChanged
    {
        private GameObject contentObject;
        private bool destroyed;

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract string Content { get; }

        public virtual string FallbackContent => @"<bg>
                                                     <vertical child-control-height='false' child-control-width='true' child-align='UpperCenter' pref-width='110' pad-left='3' pad-right='3'>
                                                       <horizontal bg='panel-top' pad-left='10' pad-right='10' horizontal-fit='PreferredSize' vertical-fit='PreferredSize'>
                                                         <text text='Invalid BSML' font-size='10'/>
                                                       </horizontal>
                                                     </vertical>
                                                     <text-page text='{0}' anchor-min-x='0.1' anchor-max-x='0.9' anchor-max-y='0.8'/>
                                                   </bg>";

        protected internal void ClearContents()
        {
            Destroy(contentObject);

            contentObject = new GameObject("Contents");
            contentObject.transform.SetParent(transform, false);

            RectTransform rectTransform = contentObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                ParseWithFallback();
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Logger.Log?.Error($"Error invoking PropertyChanged for property '{propertyName}' on View Controller {name}\n{ex}");
            }
        }

        protected void ParseWithFallback()
        {
            if (destroyed)
            {
                return;
            }

            ClearContents();

            try
            {
                BSMLParser.instance.Parse(Content, contentObject, this);
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Error parsing BSML\n{ex}");
                ClearContents();
                BSMLParser.instance.Parse(string.Format(FallbackContent, Utilities.EscapeXml(ex.Message)), contentObject, this);
            }
        }

        protected override void OnDestroy()
        {
            destroyed = true;
            base.OnDestroy();
        }
    }
}
