using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using BS_Utils.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    class StringSetting : MonoBehaviour
    {
        public BSMLAction onChange;
        public BSMLValue associatedValue;
        public bool updateOnChange = false;

        public TextMeshProUGUI label;
        public TextMeshProUGUI text;
        public Button editButton;

        private static KeyboardViewController keyboardViewController;

        public string Text
        {
            set
            {
                text.text = value;
            }
            get
            {
                return text.text;
            }
        }

        public string LabelText
        {
            set
            {
                label.text = value;
            }
        }

        public void Setup()
        {
            ReceiveValue();
        }

        protected virtual void OnEnable()
        {
            editButton.onClick.AddListener(EditButtonPressed);
        }

        protected void OnDisable()
        {
            editButton.onClick.RemoveListener(EditButtonPressed);
        }

        public void EditButtonPressed()
        {
            ModSettingsFlowCoordinator settingsFlowCoordinator = Resources.FindObjectsOfTypeAll<ModSettingsFlowCoordinator>().FirstOrDefault();
            if (settingsFlowCoordinator)
            {
                if (keyboardViewController == null)
                    keyboardViewController = BeatSaberUI.CreateViewController<KeyboardViewController>();
                keyboardViewController.startingText = Text;
                keyboardViewController.enterPressed = null;
                keyboardViewController.enterPressed += delegate (string text)
                {
                    EnterPressed(text);
                    settingsFlowCoordinator.InvokeMethod("DismissViewController", new object[] { keyboardViewController, null, false });
                };
                settingsFlowCoordinator.InvokeMethod("PresentViewController", new object[] { keyboardViewController, null, false});
            }
        }

        public void EnterPressed(string text)
        {
            Text = text;
            onChange?.Invoke(Text);
            if (updateOnChange)
            {
                ApplyValue();
            }
        }
        public void ApplyValue()
        {
            if (associatedValue != null)
                associatedValue.SetValue(Text);
        }
        public void ReceiveValue()
        {
            if (associatedValue != null)
                Text = (string)associatedValue.GetValue();
        }
    }
}
