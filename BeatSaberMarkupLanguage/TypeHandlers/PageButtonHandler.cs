using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(PageButton))]
    public class PageButtonHandler : TypeHandler<PageButton>
    {
        public enum PageButtonDirection
        {
            Up, Down, Left, Right
        }

        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "direction", new[]{"dir", "direction"} },
            { "buttonWidth", new[]{ "button-width" } }
        };

        public override Dictionary<string, Action<PageButton, string>> Setters => _setters;
        private Dictionary<string, Action<PageButton, string>> _setters = new Dictionary<string, Action<PageButton, string>>()
        {
            {"direction", new Action<PageButton, string>(SetButtonDirection) },
            {"buttonWidth", new Action<PageButton, string>(SetButtonWidth) }
        };

        public static void SetButtonDirection(PageButton component, string value)
        {
            switch (Enum.Parse(typeof(PageButtonDirection), value))
            {
                case PageButtonDirection.Up:
                    component.transform.localRotation = Quaternion.Euler(0, 0, -180);
                    break;
                case PageButtonDirection.Down:
                    component.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case PageButtonDirection.Left:
                    component.transform.localRotation = Quaternion.Euler(0, 0, -90);
                    break;
                case PageButtonDirection.Right:
                    component.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    break;
            }
        }

        public static void SetButtonWidth(PageButton component, string value)
        {

            LayoutElement layoutElement = component.gameObject.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = Parse.Float(value);
            RectTransform buttonTransform = component.transform.GetChild(0).transform as RectTransform;
            buttonTransform.sizeDelta = new Vector2(layoutElement.preferredWidth, layoutElement.preferredHeight);

            RectTransform glowTransform = buttonTransform.Find("BSMLPageButtonGlowContainer") as RectTransform;
            if (glowTransform != null)
            {
                glowTransform.localPosition = buttonTransform.localPosition;
                glowTransform.anchoredPosition = buttonTransform.anchoredPosition;
                glowTransform.anchorMin = buttonTransform.anchorMin;
                glowTransform.anchorMax = buttonTransform.anchorMax;
                glowTransform.sizeDelta = buttonTransform.sizeDelta;
            }
        }
    }
}
