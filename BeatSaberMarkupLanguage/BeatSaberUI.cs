using BeatSaberMarkupLanguage.Animations;
using HMUI;
using IPA.Utilities;
using System;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage
{
    public delegate void PresentFlowCoordinatorDelegate(FlowCoordinator current, FlowCoordinator flowCoordinator, Action finishedCallback = null, bool immediately = false, bool replaceTopViewController = false);
    public delegate void DismissFlowCoordinatorDelegate(FlowCoordinator current, FlowCoordinator flowCoordinator, Action finishedCallback = null, bool immediately = false);

    public static class BeatSaberUI
    {
        private static MainFlowCoordinator _mainFlowCoordinator;
        public static MainFlowCoordinator MainFlowCoordinator
        {
            get
            {
                if (_mainFlowCoordinator == null)
                    _mainFlowCoordinator = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
                return _mainFlowCoordinator;
            }
        }

        /// <summary>
        /// Creates a ViewController of type T, and marks it to not be destroyed.
        /// </summary>
        /// <typeparam name="T">The variation of ViewController you want to create.</typeparam>
        /// <returns>The newly created ViewController of type T.</returns>
        public static T CreateViewController<T>() where T : ViewController
        {
            T vc = new GameObject("BSMLViewController").AddComponent<T>();
            MonoBehaviour.DontDestroyOnLoad(vc.gameObject);

            vc.rectTransform.anchorMin = new Vector2(0f, 0f);
            vc.rectTransform.anchorMax = new Vector2(1f, 1f);
            vc.rectTransform.sizeDelta = new Vector2(0f, 0f);
            vc.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            return vc;
        }

        /// <summary>
        /// Creates a FlowCoordinator of type T, and marks it to not be destroyed.
        /// </summary>
        /// <typeparam name="T">The variation of FlowCoordinator you want to create.</typeparam>
        /// <returns>The newly created FlowCoordinator of type T.</returns>
        public static T CreateFlowCoordinator<T>() where T : FlowCoordinator
        {
            T flow = new GameObject("BSMLFlowCoordinator").AddComponent<T>();
            flow.SetField<FlowCoordinator, BaseInputModule>("_baseInputModule", MainFlowCoordinator.GetField<BaseInputModule, FlowCoordinator>("_baseInputModule"));
            return flow;
        }


        private static TMP_FontAsset mainTextFont = null;
        /// <summary>
        /// Gets the main font used by the game for UI text.
        /// </summary>
        public static TMP_FontAsset MainTextFont
            => mainTextFont ??= Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(t => t.name == "Teko-Medium SDF No Glow");

        /// <summary>
        /// Creates a clone of the given font, with its material fixed to be a no-glow material suitable for use on UI elements.
        /// </summary>
        /// <param name="font">the font to clone and fix</param>
        /// <returns>the fixed clone</returns>
        public static TMP_FontAsset CreateFixedUIFontClone(TMP_FontAsset font)
        {
            var matCopy = GameObject.Instantiate(MainTextFont.material);
            matCopy.mainTexture = font.material.mainTexture;
            matCopy.mainTextureOffset = font.material.mainTextureOffset;
            matCopy.mainTextureScale = font.material.mainTextureScale;
            var copy = GameObject.Instantiate(font);
            copy.material = matCopy;
            copy.SetName(font.name);
            return copy;
        }

        /// <summary>
        /// Sets the <c>name</c> of the font, recalculating its hash code as necessary.
        /// </summary>
        /// <param name="font">the font to modify</param>
        /// <param name="name">the name to assign</param>
        /// <returns>the <paramref name="name"/> provided</returns>
        public static string SetName(this TMP_FontAsset font, string name)
        {
            font.name = name;
            font.hashCode = TMP_TextUtilities.GetSimpleHashCode(font.name);
            return name;
        }

        /// <summary>
        /// Creates a <see cref="TMP_FontAsset"/> from a Unity <see cref="Font"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="TMP_FontAsset"/> returned is not usable for UI text. Use <see cref="CreateFixedUIFontClone(TMP_FontAsset)"/>
        /// to get a usable font.
        /// </remarks>
        /// <param name="font">the Unity font to use</param>
        /// <returns>the new <see cref="TMP_FontAsset"/></returns>
        public static TMP_FontAsset CreateTMPFont(Font font)
        {
            var tmpFont = TMP_FontAsset.CreateFontAsset(font);
            tmpFont.SetName(font.name);
            return tmpFont;
        }

        /// <summary>
        /// Creates a TextMeshProUGUI component.
        /// </summary>
        /// <param name="parent">The transform to parent the new TextMeshProUGUI component to.</param>
        /// <param name="text">The text to be displayed.</param>
        /// <param name="anchoredPosition">The position the button should be anchored to.</param>
        /// <returns>The newly created TextMeshProUGUI component.</returns>
        public static TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition)
        {
            return CreateText(parent, text, anchoredPosition, new Vector2(60f, 10f));
        }

        /// <summary>
        /// Creates a TextMeshProUGUI component.
        /// </summary>
        /// <param name="parent">The transform to parent the new TextMeshProUGUI component to.</param>
        /// <param name="text">The text to be displayed.</param>
        /// <param name="anchoredPosition">The position the text component should be anchored to.</param>
        /// <param name="sizeDelta">The size of the text components RectTransform.</param>
        /// <returns>The newly created TextMeshProUGUI component.</returns>
        public static TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            GameObject gameObj = new GameObject("CustomUIText");
            gameObj.SetActive(false);

            TextMeshProUGUI textMesh = gameObj.AddComponent<TextMeshProUGUI>();
            textMesh.font = MainTextFont;
            textMesh.rectTransform.SetParent(parent, false);
            textMesh.text = text;
            textMesh.fontSize = 4;
            textMesh.color = Color.white;

            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.sizeDelta = sizeDelta;
            textMesh.rectTransform.anchoredPosition = anchoredPosition;

            gameObj.SetActive(true);
            return textMesh;
        }

        #region Button Extensions
        public static void SetButtonText(this Button _button, string _text)
        {
            Polyglot.LocalizedTextMeshProUGUI localizer = _button.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>();
            if (localizer != null)
                GameObject.Destroy(localizer);

            TextMeshProUGUI tmpUgui = _button.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpUgui != null)
                tmpUgui.text = _text;
        }

        public static void SetButtonTextSize(this Button _button, float _fontSize)
        {
            if (_button.GetComponentInChildren<TextMeshProUGUI>() != null)
                _button.GetComponentInChildren<TextMeshProUGUI>().fontSize = _fontSize;
        }

        public static void ToggleWordWrapping(this Button _button, bool enableWordWrapping)
        {
            if (_button.GetComponentInChildren<TextMeshProUGUI>() != null)
                _button.GetComponentInChildren<TextMeshProUGUI>().enableWordWrapping = enableWordWrapping;
        }

        public static void SetButtonIcon(this Button _button, Sprite _icon)
        {
            if (_button.GetComponentsInChildren<Image>().Count() > 1)
                _button.GetComponentsInChildren<Image>().First(x => x.name == "Icon").sprite = _icon;
        }

        public static void SetButtonBackground(this Button _button, Sprite _background)
        {
            if (_button.GetComponentsInChildren<Image>().Count() > 0)
                _button.GetComponentsInChildren<Image>()[0].sprite = _background;
        }
        #endregion
        
        /// <summary>
        /// Sets an image or gif/apng from a resource path
        /// </summary>
        /// <param name="image">Image component to set the image to</param>
        /// <param name="location">Resource path, file path, or url of image. Can prefix with # to find and use a base game sprite. May need to prefix resource paths with 'AssemblyName:'</param>
        public static void SetImage(this Image image, string location)
        {
            AnimationStateUpdater oldStateUpdater = image.GetComponent<AnimationStateUpdater>();
            if (oldStateUpdater != null)
                MonoBehaviour.DestroyImmediate(oldStateUpdater);

            if (location.StartsWith("#"))
            {
                string imgName = location.Substring(1);
                try
                {
                    image.sprite = Resources.FindObjectsOfTypeAll<Sprite>().First(x => x.name == imgName);
                }
                catch
                {
                    Logger.log.Error($"Could not find Texture with image name {imgName}");
                }
            }
            else if (location.EndsWith(".gif") || location.EndsWith(".apng"))
            {
                AnimationStateUpdater stateUpdater = image.gameObject.AddComponent<AnimationStateUpdater>();
                stateUpdater.image = image;
                stateUpdater.controllerData = AnimationController.instance.loadingAnimation;

                if (AnimationController.instance.RegisteredAnimations.TryGetValue(location, out AnimationControllerData animControllerData))
                {
                    stateUpdater.controllerData = animControllerData;
                }
                else
                {
                    Utilities.GetData(location, (byte[] data) => {
                        AnimationLoader.Process(location.EndsWith(".gif") ? AnimationType.GIF : AnimationType.APNG, data, (Texture2D tex, Rect[] uvs, float[] delays, int width, int height) =>
                        {
                            AnimationControllerData controllerData = AnimationController.instance.Register(location, tex, uvs, delays);
                            stateUpdater.controllerData = controllerData;
                        });
                    });
                }
            }
            else
            {
                AnimationStateUpdater stateUpdater = image.gameObject.AddComponent<AnimationStateUpdater>();
                stateUpdater.image = image;
                stateUpdater.controllerData = AnimationController.instance.loadingAnimation;

                Utilities.GetData(location, (byte[] data) =>
                {
                    if (stateUpdater != null)
                        GameObject.DestroyImmediate(stateUpdater);
                    image.sprite = Utilities.LoadSpriteRaw(data);
                    image.sprite.texture.wrapMode = TextureWrapMode.Clamp;
                });
            }
        }

        #region FlowCoordinator Extensions
        public static void PresentFlowCoordinator(this FlowCoordinator current, FlowCoordinator flowCoordinator, Action finishedCallback = null, bool immediately = false, bool replaceTopViewController = false)
        {
            PresentFlowCoordinatorDelegate(current, flowCoordinator, finishedCallback, immediately, replaceTopViewController);
        }
        public static void DismissFlowCoordinator(this FlowCoordinator current, FlowCoordinator flowCoordinator, Action finishedCallback = null, bool immediately = false)
        {
            DismissFlowCoordinatorDelegate(current, flowCoordinator, finishedCallback, immediately);
        }

        #region Delegate Creation
        private static PresentFlowCoordinatorDelegate _presentFlowCoordinatorDelegate;
        private static PresentFlowCoordinatorDelegate PresentFlowCoordinatorDelegate
        {
            get
            {
                if (_presentFlowCoordinatorDelegate == null)
                {
                    MethodInfo presentMethod = typeof(FlowCoordinator).GetMethod("PresentFlowCoordinator", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    _presentFlowCoordinatorDelegate = (PresentFlowCoordinatorDelegate)Delegate.CreateDelegate(typeof(PresentFlowCoordinatorDelegate), presentMethod);
                }
                return _presentFlowCoordinatorDelegate;
            }
        }
        private static DismissFlowCoordinatorDelegate _dismissFlowCoordinatorDelegate;
        private static DismissFlowCoordinatorDelegate DismissFlowCoordinatorDelegate
        {
            get
            {
                if (_dismissFlowCoordinatorDelegate == null)
                {
                    MethodInfo dismissMethod = typeof(FlowCoordinator).GetMethod("DismissFlowCoordinator", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    _dismissFlowCoordinatorDelegate = (DismissFlowCoordinatorDelegate)Delegate.CreateDelegate(typeof(DismissFlowCoordinatorDelegate), dismissMethod);
                }
                return _dismissFlowCoordinatorDelegate;
            }
        }
        #endregion
        #endregion
    }
}
