using BeatSaberMarkupLanguage.Animations;
using HMUI;
using IPA.Utilities.Async;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;
using Zenject;
using Color = UnityEngine.Color;
using Font = UnityEngine.Font;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage
{
    public delegate void PresentFlowCoordinatorDelegate(FlowCoordinator current, FlowCoordinator flowCoordinator, Action finishedCallback = null, ViewController.AnimationDirection animationDirection = ViewController.AnimationDirection.Horizontal, bool immediately = false, bool replaceTopViewController = false);
    public delegate void DismissFlowCoordinatorDelegate(FlowCoordinator current, FlowCoordinator flowCoordinator, ViewController.AnimationDirection animationDirection = ViewController.AnimationDirection.Horizontal, Action finishedCallback = null, bool immediately = false);

    public static class BeatSaberUI
    {
        private static DiContainer _diContainer;

        public static DiContainer DiContainer
        {
            get
            {
                if (_diContainer == null)
                {
                    Logger.log.Error("Tried getting DiContainer too early!");
                }

                return _diContainer;
            }
            internal set => _diContainer = value;
        }

        public static MainFlowCoordinator MainFlowCoordinator => DiContainer.Resolve<MainFlowCoordinator>();

        public static PhysicsRaycasterWithCache PhysicsRaycasterWithCache => DiContainer.Resolve<PhysicsRaycasterWithCache>();

        public static IVRPlatformHelper PlatformHelper => DiContainer.Resolve<IVRPlatformHelper>();

        public static HoverHintController HoverHintController => DiContainer.Resolve<HoverHintController>();

        private static BasicUIAudioManager _basicUIAudioManager;
        public static BasicUIAudioManager BasicUIAudioManager
        {
            get
            {
                if (_basicUIAudioManager == null)
                {
                    _basicUIAudioManager = Resources.FindObjectsOfTypeAll<BasicUIAudioManager>().First();
                }
                return _basicUIAudioManager;
            }
        }

        private static Canvas canvasTemplate;

        /// <summary>
        /// Creates a ViewController of type T, and marks it to not be destroyed.
        /// </summary>
        /// <typeparam name="T">The variation of ViewController you want to create.</typeparam>
        /// <returns>The newly created ViewController of type T.</returns>
        public static T CreateViewController<T>() where T : ViewController
        {
            if (canvasTemplate == null)
                canvasTemplate = Resources.FindObjectsOfTypeAll<Canvas>().First(x => x.name == "DropdownTableView");

            GameObject go = new GameObject(typeof(T).Name);
            go.AddComponent(canvasTemplate);
            DiContainer.InstantiateComponent<VRGraphicRaycaster>(go.gameObject);
            go.gameObject.AddComponent<CanvasGroup>();
            T vc = go.AddComponent<T>();

            vc.rectTransform.anchorMin = new Vector2(0f, 0f);
            vc.rectTransform.anchorMax = new Vector2(1f, 1f);
            vc.rectTransform.sizeDelta = new Vector2(0f, 0f);
            vc.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            vc.gameObject.SetActive(false);
            return vc;
        }

        /// <summary>
        /// Creates a <see cref="FlowCoordinator"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The variation of FlowCoordinator you want to create.</typeparam>
        /// <returns>The newly created <see cref="FlowCoordinator"/> of type <typeparamref name="T"/>.</returns>
        public static T CreateFlowCoordinator<T>() where T : FlowCoordinator => DiContainer.InstantiateComponentOnNewGameObject<T>(typeof(T).Name);

        private static TMP_FontAsset mainTextFont = null;
        /// <summary>
        /// Gets the main font used by the game for UI text.
        /// </summary>
        public static TMP_FontAsset MainTextFont
        {
            get
            {
                if (mainTextFont == null)
                    mainTextFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().Where(t => t.name == "Teko-Medium SDF").FirstOrDefault();
                return mainTextFont;
            }
        }

        private static Material mainUIFontMaterial = null;

        internal static Material MainUIFontMaterial
        {
            get
            {
                if (mainUIFontMaterial == null)
                    mainUIFontMaterial = Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "Teko-Medium SDF Curved Softer").First();
                return mainUIFontMaterial;
            }
        }

        /// <summary>
        /// Creates a clone of the given font, with its material fixed to be a no-glow material suitable for use on UI elements.
        /// </summary>
        /// <param name="font">the font to clone and fix</param>
        /// <returns>the fixed clone</returns>
        public static TMP_FontAsset CreateFixedUIFontClone(TMP_FontAsset font)
        {
            var noglowShader = MainTextFont.material.shader;
            var newFont = Object.Instantiate(font);
            newFont.material.shader = noglowShader;
            newFont.material.EnableKeyword("CURVED");
            newFont.material.EnableKeyword("UNITY_UI_CLIP_RECT");
            return newFont;
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
        /// Creates a <see cref="TMP_FontAsset"/> from a Unity <see cref="UnityEngine.Font"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="TMP_FontAsset"/> returned is not usable for UI text. Use <see cref="CreateFixedUIFontClone(TMP_FontAsset)"/>
        /// to get a usable font.
        /// </remarks>
        /// <param name="font">the Unity font to use</param>
        /// <returns>the new <see cref="TMP_FontAsset"/></returns>
        public static TMP_FontAsset CreateTMPFont(Font font, string nameOverride = null)
        {
            var tmpFont = TMP_FontAsset.CreateFontAsset(font);
            tmpFont.SetName(nameOverride ?? font.name);
            return tmpFont;
        }

        /// <summary>
        /// Creates a TextMeshProUGUI component.
        /// </summary>
        /// <param name="parent">The transform to parent the new TextMeshProUGUI component to.</param>
        /// <param name="text">The text to be displayed.</param>
        /// <param name="anchoredPosition">The position the button should be anchored to.</param>
        /// <returns>The newly created TextMeshProUGUI component.</returns>
        /// <remarks>Proxied to the generic method, but kept for binary compatibility</remarks>
        public static TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition)
        {
            return CreateText<CurvedTextMeshPro>(parent, text, anchoredPosition, new Vector2(60f, 10f));
        }

        /// <summary>
        /// Creates a TextMeshProUGUI component.
        /// </summary>
        /// <param name="parent">The transform to parent the new TextMeshProUGUI component to.</param>
        /// <param name="text">The text to be displayed.</param>
        /// <param name="anchoredPosition">The position the text component should be anchored to.</param>
        /// <param name="sizeDelta">The size of the text components RectTransform.</param>
        /// <returns>The newly created TextMeshProUGUI component.</returns>
        /// <remarks>Proxied to the generic method, but kept for binary compatibility</remarks>
        public static TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            return CreateText<CurvedTextMeshPro>(parent, text, anchoredPosition, sizeDelta);
        }

        /// <summary>
        /// Creates a TMP_Text (or one of its inheritances) component.
        /// </summary>
        /// <param name="parent">The transform to parent the new TMP_Text (or one of its inheritances) component to.</param>
        /// <param name="text">The text to be displayed.</param>
        /// <param name="anchoredPosition">The position the button should be anchored to.</param>
        /// <returns>The newly created TMP_Text (or one of its inheritances) component.</returns>
        public static T CreateText<T>(RectTransform parent, string text, Vector2 anchoredPosition) where T : TMP_Text
        {
            return CreateText<T>(parent, text, anchoredPosition, new Vector2(60f, 10f));
        }

        /// <summary>
        /// Creates a TMP_Text (or one of its inheritances) component.
        /// </summary>
        /// <param name="parent">The transform to parent the new TMP_Text (or one of its inheritances) component to.</param>
        /// <param name="text">The text to be displayed.</param>
        /// <param name="anchoredPosition">The position the text component should be anchored to.</param>
        /// <param name="sizeDelta">The size of the text components RectTransform.</param>
        /// <returns>The newly created TMP_Text (or one of its inheritances) component.</returns>
        public static T CreateText<T>(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta) where T : TMP_Text
        {
            GameObject gameObj = new GameObject("CustomUIText");
            gameObj.SetActive(false);

            T textComponent = gameObj.AddComponent<T>();
            textComponent.font = MainTextFont;
            textComponent.fontSharedMaterial = MainUIFontMaterial;
            textComponent.rectTransform.SetParent(parent, false);
            textComponent.text = text;
            textComponent.fontSize = 4;
            textComponent.color = Color.white;

            textComponent.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textComponent.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textComponent.rectTransform.sizeDelta = sizeDelta;
            textComponent.rectTransform.anchoredPosition = anchoredPosition;

            gameObj.SetActive(true);
            return textComponent;
        }

        #region Button Extensions
        public static void SetButtonText(this Button _button, string _text)
        {
            Polyglot.LocalizedTextMeshProUGUI localizer = _button.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>(true);
            if (localizer != null)
                Object.Destroy(localizer);

            TextMeshProUGUI tmpUgui = _button.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpUgui != null)
                tmpUgui.SetText(_text);
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

        public static void SetButtonStates(this Button _button, Sprite _normal, Sprite _hover)
        {
            ButtonSpriteSwap swap = _button.GetComponent<ButtonSpriteSwap>();

            swap._disabledStateSprite = _normal;
            swap._normalStateSprite = _normal;
            swap._highlightStateSprite = _hover;
            //Unneeded?
            swap._pressedStateSprite = _hover;
        }
        #endregion

        /// <summary>
        /// Sets an image or gif/apng from a resource path
        /// </summary>
        /// <param name="image">Image component to set the image to</param>
        /// <param name="location">Resource path, file path, or url of image. Can prefix with # to find and use a base game sprite. May need to prefix resource paths with 'AssemblyName:'</param>
        public static void SetImage(this Image image, string location)
        {
            SetImage(image, location, true, new ScaleOptions(), null);
        }

        /// <summary>
        /// Sets an image or gif/apng from a resource path
        /// </summary>
        /// <param name="image">Image component to set the image to</param>
        /// <param name="location">Resource path, file path, or url of image. Can prefix with # to find and use a base game sprite. May need to prefix resource paths with 'AssemblyName:'</param>
        /// <param name="loadingAnimation">Whether a loading animation is shown as a placeholder until the image is loaded</param>
        /// <param name="scaleOptions">If the image should be downscaled and what it should be downscaled to</param>
        public static void SetImage(this Image image, string location, bool loadingAnimation, ScaleOptions scaleOptions)
        {
            SetImage(image, location, loadingAnimation, scaleOptions, null);
        }

        /// <summary>
        /// Sets an image or gif/apng from a resource path
        /// </summary>
        /// <param name="image">Image component to set the image to</param>
        /// <param name="location">Resource path, file path, or url of image. Can prefix with # to find and use a base game sprite. May need to prefix resource paths with 'AssemblyName:'</param>
        /// <param name="loadingAnimation">Whether a loading animation is shown as a placeholder until the image is loaded</param>
        /// <param name="scaleOptions">If the image should be downscaled and what it should be downscaled to</param>
        /// <param name="callback">Method to call once SetImage has finished</param>
        public static void SetImage(this Image image, string location, bool loadingAnimation, ScaleOptions scaleOptions, Action callback)
        {
            AnimationStateUpdater oldStateUpdater = image.GetComponent<AnimationStateUpdater>();
            if (oldStateUpdater != null)
                Object.DestroyImmediate(oldStateUpdater);

            if (location.Length > 1 && location[0] == '#')
            {
                string imgName = location.Substring(1);
                image.sprite = Utilities.FindSpriteCached(imgName);
                if (image.sprite == null)
                {
                    Logger.log.Error($"Could not find Sprite with image name {imgName}");
                }

                return;
            }


            bool isURL = Uri.TryCreate(location, UriKind.Absolute, out Uri uri);
            if (IsAnimated(location) || isURL && IsAnimated(uri.LocalPath))
            {
                AnimationStateUpdater stateUpdater = image.gameObject.AddComponent<AnimationStateUpdater>();
                stateUpdater.image = image;
                if (loadingAnimation)
                    stateUpdater.controllerData = AnimationController.instance.loadingAnimation;

                if (AnimationController.instance.RegisteredAnimations.TryGetValue(location, out AnimationControllerData animControllerData))
                {
                    stateUpdater.controllerData = animControllerData;
                }
                else
                {
                    Utilities.GetData(location, (byte[] data) =>
                    {
                        AnimationLoader.Process(
                            (location.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                            (isURL && uri.LocalPath.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))) ? AnimationType.GIF : AnimationType.APNG,

                            data, (Texture2D tex, Rect[] uvs, float[] delays, int width, int height) =>
                        {
                            AnimationControllerData controllerData = AnimationController.instance.Register(location, tex, uvs, delays);
                            stateUpdater.controllerData = controllerData;
                            callback?.Invoke();
                        });
                    });
                    return;
                }
            }
            else
            {
                AnimationStateUpdater stateUpdater = image.gameObject.AddComponent<AnimationStateUpdater>();
                stateUpdater.image = image;
                if (loadingAnimation)
                    stateUpdater.controllerData = AnimationController.instance.loadingAnimation;

                Utilities.GetData(location, async (byte[] data) =>
                {
                    if (stateUpdater != null)
                        GameObject.DestroyImmediate(stateUpdater);

                    if (scaleOptions.ShouldScale)
                    {
                        var imageBytes = await Task.Run(() => DownscaleImage(data, scaleOptions.Width, scaleOptions.Height, scaleOptions.MaintainRatio)).ConfigureAwait(false);
                        _ = UnityMainThreadTaskScheduler.Factory.StartNew(() =>
                        {
                            image.sprite = Utilities.LoadSpriteRaw(imageBytes);
                            image.sprite.texture.wrapMode = TextureWrapMode.Clamp;
                        });
                    }
                    else
                    {
                        image.sprite = Utilities.LoadSpriteRaw(data);
                        image.sprite.texture.wrapMode = TextureWrapMode.Clamp;
                    }

                    callback?.Invoke();
                });
                return;
            }
            callback?.Invoke();
        }

        public struct ScaleOptions
        {
            public bool ShouldScale;
            public bool MaintainRatio;
            public int Width;
            public int Height;
        }

        /// <summary>
        /// Downscale the image in <paramref name="data"/> to the specified size.
        ///
        /// If <paramref name="maintainAspectRatio"/> is true, the image will be scaled to fit into the bounds defined by <paramref name="width"/> × <paramref name="height"/>.
        /// </summary>
        /// <param name="data">Byte array containing the image data.</param>
        /// <param name="width">The maximum width of the scaled image.</param>
        /// <param name="height">The maximum height of the scaled image.</param>
        /// <param name="maintainAspectRatio">If true, the image will be scaled while maintaining its aspect ratio.</param>
        /// <returns>A byte array containing the image data.</returns>
        public static byte[] DownscaleImage(byte[] data, int width, int height, bool maintainAspectRatio = true)
        {
            try
            {
                System.Drawing.Image originalImage;
                using (var readMemoryStream = new MemoryStream(data))
                {
                    originalImage = System.Drawing.Image.FromStream(readMemoryStream);
                }

                if (originalImage.Width <= width && originalImage.Height <= height)
                {
                    return data;
                }

                Bitmap resizedImage;
                if (maintainAspectRatio)
                {
                    var scale = Math.Min((double)width / originalImage.Width, (double)height / originalImage.Height);
                    resizedImage = new Bitmap(originalImage, (int)Math.Round(width * scale), (int)Math.Round(height * scale));
                }
                else
                {
                    resizedImage = new Bitmap(originalImage, width, height);
                }

                using var writeMemoryStream = new MemoryStream();
                resizedImage.Save(writeMemoryStream, originalImage.RawFormat);

                return writeMemoryStream.ToArray();
            }
            catch (Exception)
            {
                return data;
            }
        }

        private static bool IsAnimated(string str)
        {
            return str.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) || str.EndsWith(".apng", StringComparison.OrdinalIgnoreCase);
        }

        #region FlowCoordinator Extensions
        public static void PresentFlowCoordinator(this FlowCoordinator current, FlowCoordinator flowCoordinator, Action finishedCallback = null, ViewController.AnimationDirection animationDirection = ViewController.AnimationDirection.Horizontal, bool immediately = false, bool replaceTopViewController = false)
        {
            PresentFlowCoordinatorDelegate(current, flowCoordinator, finishedCallback, animationDirection, immediately, replaceTopViewController);
        }
        public static void DismissFlowCoordinator(this FlowCoordinator current, FlowCoordinator flowCoordinator, Action finishedCallback = null, ViewController.AnimationDirection animationDirection = ViewController.AnimationDirection.Horizontal, bool immediately = false)
        {
            DismissFlowCoordinatorDelegate(current, flowCoordinator, animationDirection, finishedCallback, immediately);
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
