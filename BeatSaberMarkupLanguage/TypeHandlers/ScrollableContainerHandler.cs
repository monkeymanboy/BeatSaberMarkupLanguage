﻿using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(BSMLScrollableContainer))]
    public class ScrollableContainerHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props { get; } = new Dictionary<string, string[]>
        {
            { "id", new[] { "id" } },
            { "maskOverflow", new[] { "mask-overflow" } },
            { "alignBottom", new[] { "align-bottom" } },
            { "scrollToBottomOnUpdate", new[] { "scroll-to-bottom-on-update" } },
        };

        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            BSMLScrollableContainer scrollView = componentType.Component as BSMLScrollableContainer;

            if (componentType.Data.TryGetValue("id", out string id))
            {
                parserParams.AddEvent(id + "#PageUp", scrollView.PageUpButtonPressed);
                parserParams.AddEvent(id + "#PageDown", scrollView.PageDownButtonPressed);
            }

            if (componentType.Data.TryGetValue("maskOverflow", out string value))
            {
                scrollView.MaskOverflow = bool.TryParse(value, out bool bval) ? bval : true;
            }

            if (componentType.Data.TryGetValue("alignBottom", out value))
            {
                scrollView.AlignBottom = bool.TryParse(value, out bool bval) ? bval : false;
            }

            if (componentType.Data.TryGetValue("scrollToBottomOnUpdate", out value))
            {
                scrollView.ScrollToBottomOnUpdate = bool.TryParse(value, out bool bval) ? bval : false;
            }
        }

        public override void HandleTypeAfterChildren(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            foreach (GameObject go in parserParams.GetObjectsWithTag("ScrollFocus"))
            {
                go.AddComponent<ItemForFocussedScrolling>();
            }
        }

        public override void HandleTypeAfterParse(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            BSMLScrollableContainer scrollView = componentType.Component as BSMLScrollableContainer;

            if (componentType.Data.TryGetValue("id", out string id))
            {
                scrollView.PageUpButton = parserParams.GetObjectsWithTag("PageUpFor:" + id)
                    .Select(o => o.GetComponent<Button>())
                    .Where(b => b != null)
                    .FirstOrDefault();

                scrollView.PageDownButton = parserParams.GetObjectsWithTag("PageDownFor:" + id)
                    .Select(o => o.GetComponent<Button>())
                    .Where(b => b != null)
                    .FirstOrDefault();

                scrollView.ScrollIndicator = parserParams.GetObjectsWithTag("IndicatorFor:" + id)
                    .Select(o => o.GetComponent<VerticalScrollIndicator>())
                    .Where(i => i != null)
                    .FirstOrDefault();
            }

            scrollView.RefreshContent();
            scrollView.RefreshButtons();
        }
    }
}
