using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(TextMeshProProxy))]
    public class TextMeshProProxyHandler : TextMeshProUGUIHandler
    {
        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            if (componentType.component is TextMeshProProxy proxy) 
            {
                componentType.component = proxy.Text;
                base.HandleType(componentType, parserParams);
                NotifyUpdater updater = componentType.component.gameObject.GetComponent<NotifyUpdater>();
                if(updater != null)
                {
                    updater.ComponentChanged += (s, _) =>
                    {
                        proxy.RefreshScrollView();
                    };
                }
            }
        }
    }
}
