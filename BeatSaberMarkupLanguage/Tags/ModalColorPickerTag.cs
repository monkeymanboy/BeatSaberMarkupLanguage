using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModalColorPickerTag : ModalTag
    {
        private RGBPanelController rgbTemplate;
        private HSVPanelController hsvTemplate;
        private Image currentColorTemplate;

        public override string[] Aliases => new[] { "modal-color-picker" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = base.CreateObject(parent);
            RectTransform windowTransform = gameObject.transform as RectTransform;
            windowTransform.name = "BSMLModalColorPicker";
            windowTransform.sizeDelta = new Vector2(135, 70);

            ModalColorPicker colorPicker = gameObject.AddComponent<ModalColorPicker>();
            colorPicker.ModalView = gameObject.GetComponent<ModalView>();

            EditColorSchemeController editColorSchemeController = DiContainer.Resolve<GameplaySetupViewController>().GetComponentInChildren<EditColorSchemeController>(true);

            if (rgbTemplate == null)
            {
                rgbTemplate = editColorSchemeController.GetComponentInChildren<RGBPanelController>();
            }

            if (hsvTemplate == null)
            {
                hsvTemplate = editColorSchemeController.GetComponentInChildren<HSVPanelController>();
            }

            if (currentColorTemplate == null)
            {
                currentColorTemplate = DiContainer.Resolve<GameplaySetupViewController>()._colorsOverrideSettingsPanelController._colorSchemeDropDown._cellPrefab._colorSchemeView._saberAColorImage;
            }

            RGBPanelController rgbController = Object.Instantiate(rgbTemplate, gameObject.transform, false);
            rgbController.name = "BSMLRGBPanel";
            RectTransform rgbTransform = (RectTransform)rgbController.transform;
            rgbTransform.anchoredPosition = new Vector2(0, 3);
            rgbTransform.anchorMin = new Vector2(0, 0.25f);
            rgbTransform.anchorMax = new Vector2(0, 0.25f);
            colorPicker.RgbPanel = rgbController;
            rgbController.colorDidChangeEvent += colorPicker.OnValueChanged;

            HSVPanelController hsvController = Object.Instantiate(hsvTemplate, gameObject.transform, false);
            hsvController.name = "BSMLHSVPanel";
            RectTransform hsvTransform = (RectTransform)hsvController.transform;
            hsvTransform.anchoredPosition = new Vector2(0, 3);
            hsvTransform.anchorMin = new Vector2(0.6f, 0.15f);
            hsvTransform.anchorMax = new Vector2(0.6f, 0.15f);
            colorPicker.HsvPanel = hsvController;
            hsvController.colorDidChangeEvent += colorPicker.OnValueChanged;

            Image colorImage = Object.Instantiate(currentColorTemplate, gameObject.transform, false);
            colorImage.name = "BSMLCurrentColor";
            RectTransform imageRectTransform = (RectTransform)colorImage.transform;
            imageRectTransform.anchoredPosition = new Vector2(0, 0);
            imageRectTransform.anchorMin = new Vector2(0.53f, 0.53f);
            imageRectTransform.anchorMax = new Vector2(0.53f, 0.53f);
            imageRectTransform.sizeDelta = new Vector2(6, 6);
            colorPicker.ColorImage = colorImage;

            BSMLParser.Instance.Parse(@"<horizontal anchor-pos-y='-28' spacing='2' horizontal-fit='PreferredSize'><button text-key='BUTTON_CANCEL' on-click='cancel' pref-width='34' pref-height='10' /><action-button text-key='BUTTON_OK' on-click='done' pref-width='34' pref-height='10' /></horizontal>", gameObject, colorPicker);

            return gameObject;
        }
    }
}
