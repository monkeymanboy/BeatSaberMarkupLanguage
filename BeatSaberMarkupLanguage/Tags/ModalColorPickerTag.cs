﻿using BeatSaberMarkupLanguage.Components;
using HMUI;
using System.Linq;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModalColorPickerTag : ModalTag
    {
        public override string[] Aliases => new[] { "modal-color-picker" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = base.CreateObject(parent);
            ExternalComponents externalComponents = gameObject.GetComponent<ExternalComponents>();

            RectTransform windowTransform = externalComponents.Get<RectTransform>();
            windowTransform.name = "BSMLModalColorPicker";
            windowTransform.sizeDelta = new Vector2(135, 75);

            ModalColorPicker colorPicker = gameObject.AddComponent<ModalColorPicker>();
            colorPicker.modalView = externalComponents.Get<ModalView>();

            RGBPanelController rgbController = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<RGBPanelController>().First(x => x.name == "RGBColorPicker"), gameObject.transform, false);
            rgbController.name = "BSMLRGBPanel";
            (rgbController.gameObject.transform as RectTransform).anchoredPosition = new Vector2(0, 3);
            (rgbController.gameObject.transform as RectTransform).anchorMin = new Vector2(0.1f, 0.73f);
            (rgbController.gameObject.transform as RectTransform).anchorMax = new Vector2(0.1f, 0.73f);
            colorPicker.rgbPanel = rgbController;
            rgbController.colorDidChangeEvent += colorPicker.OnChange;

            HSVPanelController hsvController = Object.Instantiate(Resources.FindObjectsOfTypeAll<HSVPanelController>().First(x => x.name == "HSVColorPicker"), gameObject.transform, false);
            hsvController.name = "BSMLHSVPanel";
            (hsvController.gameObject.transform as RectTransform).anchoredPosition = new Vector2(0, 3);
            (hsvController.gameObject.transform as RectTransform).anchorMin = new Vector2(0.75f, 0.5f);
            (hsvController.gameObject.transform as RectTransform).anchorMax = new Vector2(0.75f, 0.5f);
            colorPicker.hsvPanel = hsvController;
            hsvController.colorDidChangeEvent += colorPicker.OnChange;

            Image colorImage = Object.Instantiate(Resources.FindObjectsOfTypeAll<Image>().First(x => x.gameObject.name == "ColorImage" && x.sprite?.name == "NoteCircle"), gameObject.transform, false);
            colorImage.name = "BSMLCurrentColor";
            (colorImage.gameObject.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            (colorImage.gameObject.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
            (colorImage.gameObject.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
            colorPicker.colorImage = colorImage;

            BSMLParser.instance.Parse(@"<horizontal anchor-pos-y='-30' spacing='2' horizontal-fit='PreferredSize'><button text='Cancel' on-click='cancel' pref-width='30'/><button text='Done' on-click='done' pref-width='30'/></horizontal>", gameObject, colorPicker);

            return gameObject;
        }
    }
}
