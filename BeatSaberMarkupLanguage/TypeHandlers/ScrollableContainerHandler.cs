using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    
    [ComponentHandler(typeof(BSMLScrollableContainer))]
    public class ScrollableContainerHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props { get; } = new Dictionary<string, string[]>
        {
            { "id", new[]{ "id" } },
            { "maskOverflow", new[] { "mask-overflow" } },
            { "alignBottom", new[] { "align-bottom" } },
            { "onUpdateToBottom", new[] { "on-update-to-bottom" } },
        };

        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            BSMLScrollableContainer scrollView = componentType.component as BSMLScrollableContainer;

            if (componentType.data.TryGetValue("id", out string id))
            {
                parserParams.AddEvent(id + "#PageUp", scrollView.PageUpButtonPressed);
                parserParams.AddEvent(id + "#PageDown", scrollView.PageDownButtonPressed);
            }

            if (componentType.data.TryGetValue("maskOverflow", out string value))
            {
                scrollView.MaskOverflow = bool.TryParse(value, out bool bval) ? bval : true;
            }

            if (componentType.data.TryGetValue("alignBottom", out value))
            {
                scrollView.AlignBottom = bool.TryParse(value, out bool bval) ? bval : false;
            }

            if (componentType.data.TryGetValue("onUpdateToBottom", out value))
            {
                scrollView.OnUpdateToBottom = bool.TryParse(value, out bool bval) ? bval : false;
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
            BSMLScrollableContainer scrollView = componentType.component as BSMLScrollableContainer;

            if (componentType.data.TryGetValue("id", out string id))
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
                    .Select(o => o.GetComponent<VerticalScrollIndicator>() ?? o.GetComponent<BSMLScrollIndicator>())
                    .Where(i => i != null)
                    .FirstOrDefault();
            }

            scrollView.RefreshContent();
            scrollView.RefreshButtons();
            //scrollView.ScrollAt(0, false);
        }
    }
}
