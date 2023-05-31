using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(PageButton))]
    public class PageButtonHandler : TypeHandler<PageButton>
    {
        public enum PageButtonDirection
        {
            Up,
            Down,
            Left,
            Right,
        }

        public override Dictionary<string, string[]> Props => new()
        {
            { "direction", new[] { "dir", "direction" } },
        };

        public override Dictionary<string, Action<PageButton, string>> Setters => new()
        {
            { "direction", new Action<PageButton, string>(SetButtonDirection) },
        };

        public static void SetButtonDirection(PageButton button, string value)
        {
            LayoutElement layoutElement = button.gameObject.GetComponent<LayoutElement>();
            RectTransform buttonTransform = button.transform.Find("Icon") as RectTransform;
            buttonTransform.anchoredPosition = Vector2.zero;
            bool isHorizontal = false;
            switch (Enum.Parse(typeof(PageButtonDirection), value))
            {
                case PageButtonDirection.Up:
                    isHorizontal = true;
                    buttonTransform.localRotation = Quaternion.Euler(0, 0, -180);
                    break;
                case PageButtonDirection.Down:
                    isHorizontal = true;
                    buttonTransform.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case PageButtonDirection.Left:
                    isHorizontal = false;
                    buttonTransform.localRotation = Quaternion.Euler(0, 0, -90);
                    break;
                case PageButtonDirection.Right:
                    isHorizontal = false;
                    buttonTransform.localRotation = Quaternion.Euler(0, 0, 90);
                    break;
            }

            // Establish default dimensions if they weren't changed
            if (layoutElement.preferredHeight == -1)
            {
                layoutElement.preferredHeight = isHorizontal ? 6 : 40;
            }

            if (layoutElement.preferredWidth == -1)
            {
                layoutElement.preferredWidth = isHorizontal ? 40 : 6;
            }
        }
    }
}
