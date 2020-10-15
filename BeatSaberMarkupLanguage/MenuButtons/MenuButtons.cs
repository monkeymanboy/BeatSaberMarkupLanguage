using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using IPA.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    public class MenuButtons : PersistentSingleton<MenuButtons>
    {
        private MenuButtonsViewController menuButtonsViewController;

        [UIValue("buttons")]
        private List<object> buttons = new List<object>();

        [UIValue("pin-buttons")]
        internal List<object> pinButtons = new List<object>();

        [UIParams]
        private BSMLParserParams parserParams;

        internal void Setup()
        {
            menuButtonsViewController = BeatSaberUI.CreateViewController<MenuButtonsViewController>();
            menuButtonsViewController.buttons = buttons;
            StopAllCoroutines();
            StartCoroutine(PresentView());
            /*
            if (MenuPins.instance.rootObject == null)
                MenuPins.instance.Setup();
            else
                MenuPins.instance.Refresh();*/
        }
        IEnumerator PresentView()
        {
            yield return new WaitForSeconds(0.2f);//Forgive me lord for what I must do
            yield return new WaitWhile(() => BeatSaberUI.MainFlowCoordinator == null);
            ShowView(false, false, false);
            Resources.FindObjectsOfTypeAll<MainMenuViewController>().First().didActivateEvent += ShowView;
        }

        internal void ShowView(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            BeatSaberUI.MainFlowCoordinator.InvokeMethod<object, FlowCoordinator>("SetLeftScreenViewController", menuButtonsViewController, ViewController.AnimationType.None);
        }

        internal void Refresh()
        {
            if (menuButtonsViewController == null)
                return;
            menuButtonsViewController.RefreshView();
        }

        public void RegisterButton(MenuButton menuButton)
        {
            if (buttons.Any(x => (x as MenuButton).Text == menuButton.Text)) return;
            buttons.Add(menuButton);
            //pinButtons.Add(new PinnedMod(menuButton));
            Refresh();
        }

        public void UnregisterButton(MenuButton menuButton)
        {
            buttons.Remove(menuButton);
            //pinButtons.RemoveAll(x => (x as PinnedMod).menuButton == menuButton);
            Refresh();
        }
    }
    /*
    internal class MenuPins : PersistentSingleton<MenuPins>
    {
        [UIValue("pin-buttons")]
        public List<object> pinButtons => MenuButtons.instance.pinButtons;

        [UIObject("root-object")]
        internal GameObject rootObject;

        private List<string> pins;
        private List<string> Pins
        {
            get
            {
                if (pins == null)
                    pins = Plugin.config.PinnedMods; //.GetString("Pins", "Pinned Mods").Split(',').ToList();
                return pins;
            }
        }

        internal void PinButton(string text)
        {
            Pins.Add(text);
            Refresh();
            SavePins();
        }
        internal void UnPinButton(string text)
        {
            Pins.Remove(text);
            Refresh();
            SavePins();
        }
        internal bool IsPinned(string text) => Pins.Contains(text);

        internal void Refresh()
        {
            if(rootObject != null)
            {
                GameObject.Destroy(rootObject);
                Setup();
            }
        }
        internal void Setup()
        {
            MainMenuViewController mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "BeatSaberMarkupLanguage.Views.main-menu-screen.bsml"), mainMenuViewController.gameObject, this);
        }
        internal void SavePins()
        {
            Plugin.config.PinnedMods = Pins; //.SetString("Pins", "Pinned Mods", string.Join(",", Pins));
        }
    }
    internal class PinnedMod : INotifyPropertyChanged
    {
        [UIValue("menu-button")]
        public MenuButton menuButton;

        private string pinButtonText;
        [UIValue("pin-button-text")]
        public string PinButtonText
        {
            get => pinButtonText;
            set
            {
                pinButtonText = value;
                NotifyPropertyChanged();
            }
        }

        private string pinButtonStrokeColor;
        [UIValue("pin-button-stroke-color")]
        public string PinButtonStrokeColor
        {
            get => pinButtonStrokeColor;
            set
            {
                pinButtonStrokeColor = value;
                NotifyPropertyChanged();
            }
        }
        [UIValue("is-pinned")]
        public bool IsPinned => MenuPins.instance.IsPinned(menuButton.Text);


        public PinnedMod(MenuButton menuButton)
        {
            this.menuButton = menuButton;
            UpdatePinButtonText();
        }

        [UIAction("pin-button-click")]
        private void PinButtonClick()
        {
            if (IsPinned)
                MenuPins.instance.UnPinButton(menuButton.Text);
            else
                MenuPins.instance.PinButton(menuButton.Text);
            UpdatePinButtonText();
        }

        private void UpdatePinButtonText()
        {
            PinButtonText = IsPinned ? "x" : "+";
            PinButtonStrokeColor = IsPinned ? "#34eb55" : "white";
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {
                Logger.log?.Error($"Error Invoking PropertyChanged: {ex.Message}");
                Logger.log?.Error(ex);
            }
        }
    }*/
}
