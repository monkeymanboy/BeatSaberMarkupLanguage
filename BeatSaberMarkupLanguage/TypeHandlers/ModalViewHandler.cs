using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(ModalView))]
    public class ModalViewHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "showEvent", new[]{ "show-event" } },
            { "hideEvent", new[]{ "hide-event" } },
            { "clickOffCloses", new[]{ "click-off-closes", "clickerino-offerino-closerino" } },
            { "moveToCenter", new[]{ "move-to-center" } }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            try
            {
                ModalView modalView = componentType.component as ModalView;
                Transform originalParent = modalView.transform.parent;
                bool moveToCenter = true;
                if (componentType.data.TryGetValue("moveToCenter", out string moveToCenterString))
                    moveToCenter = bool.Parse(moveToCenterString);

                if (componentType.data.TryGetValue("showEvent", out string showEvent))
                {
                    parserParams.AddEvent(showEvent, delegate
                    {
                        modalView.Show(true, moveToCenter);
                    });
                }

                if (componentType.data.TryGetValue("hideEvent", out string hideEvent))
                {
                    parserParams.AddEvent(hideEvent, delegate
                    {
                        modalView.Hide(true, () => modalView.transform.SetParent(originalParent, true));
                    });
                }

                if (componentType.data.TryGetValue("clickOffCloses", out string clickOffCloses) && Parse.Bool(clickOffCloses))
                {
                    modalView.blockerClickedEvent += delegate
                    {
                        modalView.Hide(true, () => modalView.transform.SetParent(originalParent, true));
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.log?.Error(ex);
            }
        }
    }
}
