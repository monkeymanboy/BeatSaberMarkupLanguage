﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Animations;
using BGLib.Polyglot;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using VRUIControls;
using Zenject;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage
{
    public delegate void PresentFlowCoordinatorDelegate(FlowCoordinator current, FlowCoordinator flowCoordinator, Action finishedCallback = null, ViewController.AnimationDirection animationDirection = ViewController.AnimationDirection.Horizontal, bool immediately = false, bool replaceTopViewController = false);

    public delegate void DismissFlowCoordinatorDelegate(FlowCoordinator current, FlowCoordinator flowCoordinator, ViewController.AnimationDirection animationDirection = ViewController.AnimationDirection.Horizontal, Action finishedCallback = null, bool immediately = false);

    public static class BeatSaberUI
    {
        private static DiContainer diContainer;
        private static BasicUIAudioManager basicUIAudioManager;

        public static DiContainer DiContainer
        {
            get
            {
                if (diContainer == null)
                {
                    Logger.Log.Error("Tried getting DiContainer too early!");
                }

                return diContainer;
            }
        }

        public static MainFlowCoordinator MainFlowCoordinator => DiContainer.Resolve<MainFlowCoordinator>();

        public static PhysicsRaycasterWithCache PhysicsRaycasterWithCache => DiContainer.Resolve<PhysicsRaycasterWithCache>();

        public static IVRPlatformHelper PlatformHelper => DiContainer.Resolve<IVRPlatformHelper>();

        public static HoverHintController HoverHintController => DiContainer.Resolve<HoverHintController>();

        public static BasicUIAudioManager BasicUIAudioManager
        {
            get
            {
                if (basicUIAudioManager == null)
                {
                    basicUIAudioManager = Object.FindAnyObjectByType<BasicUIAudioManager>();
                }

                return basicUIAudioManager;
            }
        }

        /// <summary>
        /// Gets the main font used by the game for UI text.
        /// </summary>
        public static TMP_FontAsset MainTextFont { get; private set; }

        /// <summary>
        /// Gets the main font used by the game for UI text.
        /// </summary>
        public static TMP_FontAsset MonochromeTextFont { get; private set; }

        /// <summary>
        /// Gets the main font material used by the game for UI text.
        /// </summary>
        /// <remarks>
        /// This material is meant to be used on curved canvases. Usage on non-curved canvases may result in unexpected behavior.
        /// </remarks>
        internal static Material MainUIFontMaterial { get; private set; }

        /// <summary>
        /// Gets a material derived from the main font material used by the game for UI text that can be used on regular (non curved) <see cref="TextMeshProUGUI"/> and <see cref="TextMeshPro"/>.
        /// </summary>
        /// <remarks>
        /// Usage on curved canvases may result in unexpected behavior.
        /// </remarks>
        internal static Material MainFlatUIFontMaterial { get; private set; }

        /// <summary>
        /// Creates a ViewController of type T, and marks it to not be destroyed.
        /// </summary>
        /// <typeparam name="T">The variation of ViewController you want to create.</typeparam>
        /// <returns>The newly created ViewController of type T.</returns>
        public static T CreateViewController<T>()
            where T : ViewController
        {
            GameObject gameObject = new(typeof(T).Name)
            {
                layer = 5,
            };

            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.sizeDelta = new Vector2(0f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 0f);

            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;

            T viewController;
            if (DiContainer.IsInstalling)
            {
                DiContainer.QueueForInject(gameObject.AddComponent<VRGraphicRaycaster>());

                viewController = gameObject.AddComponent<T>();
                DiContainer.QueueForInject(viewController);
            }
            else
            {
                DiContainer.InstantiateComponent<VRGraphicRaycaster>(gameObject);
                viewController = DiContainer.InstantiateComponent<T>(gameObject);
            }

            gameObject.SetActive(false);

            return viewController;
        }

        /// <summary>
        /// Creates a <see cref="FlowCoordinator"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The variation of FlowCoordinator you want to create.</typeparam>
        /// <returns>The newly created <see cref="FlowCoordinator"/> of type <typeparamref name="T"/>.</returns>
        public static T CreateFlowCoordinator<T>()
            where T : FlowCoordinator
        {
            if (DiContainer.IsInstalling)
            {
                GameObject gameObject = new(typeof(T).Name);
                T flowCoordinator = gameObject.AddComponent<T>();
                DiContainer.QueueForInject(flowCoordinator);
                return flowCoordinator;
            }
            else
            {
                return DiContainer.InstantiateComponentOnNewGameObject<T>(typeof(T).Name);
            }
        }

        /// <summary>
        /// Creates a clone of the given font, with its material fixed to be a no-glow material suitable for use on UI elements.
        /// </summary>
        /// <param name="font">The font to clone and fix.</param>
        /// <returns>The fixed clone.</returns>
        [Obsolete("This method is dangerous as it can cause separate TMP_FontAssets to overwrite each others' textures. Consider using CreateTMPFont and adding the font as a fallback to an existing TMP_FontAsset.")]
        public static TMP_FontAsset CreateFixedUIFontClone(TMP_FontAsset font)
        {
            Shader noglowShader = MainTextFont.material.shader;
            TMP_FontAsset newFont = Object.Instantiate(font);
            newFont.material.shader = noglowShader;
            newFont.material.EnableKeyword("CURVED");
            newFont.material.EnableKeyword("UNITY_UI_CLIP_RECT");
            return newFont;
        }

        /// <summary>
        /// Sets the <c>name</c> of the font, recalculating its hash code as necessary.
        /// </summary>
        /// <param name="font">The font to modify.</param>
        /// <param name="name">The name to assign.</param>
        /// <returns>The <paramref name="name"/> provided.</returns>
        public static string SetName(this TMP_FontAsset font, string name)
        {
            font.name = name;
            font.hashCode = TMP_TextUtilities.GetSimpleHashCode(font.name);
            return name;
        }

        /// <summary>
        /// Creates a <see cref="TMP_FontAsset"/> from a Unity <see cref="Font"/>.
        /// </summary>
        /// <param name="font">The Unity font to use.</param>
        /// <param name="nameOverride">The name to use for the <see cref="TMP_FontAsset"/>. Defaults to the name of <paramref name="font"/>.</param>
        /// <returns>The new <see cref="TMP_FontAsset"/>.</returns>
        public static TMP_FontAsset CreateTMPFont(Font font, string nameOverride = null)
        {
            TMP_FontAsset tmpFont = TMP_FontAsset.CreateFontAsset(font);
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
        /// <remarks>Proxied to the generic method, but kept for binary compatibility.</remarks>
        [Obsolete("This method never worked properly for flat text. Use CreateCurvedUIText instead.")]
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
        /// <returns>The newly created <see cref="TextMeshProUGUI"/> component.</returns>
        /// <remarks>Proxied to the generic method, but kept for binary compatibility.</remarks>
        [Obsolete("This method never worked properly for flat text. Use CreateCurvedUIText instead.")]
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
        /// <typeparam name="T">The type of <see cref="TMP_Text"/> to create.</typeparam>
        /// <returns>The newly created text component.</returns>
        [Obsolete("This method never worked properly for flat text. Use CreateCurvedUIText instead.")]
        public static T CreateText<T>(RectTransform parent, string text, Vector2 anchoredPosition)
            where T : TMP_Text
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
        /// <typeparam name="T">The type of <see cref="TMP_Text"/> to create.</typeparam>
        /// <returns>The newly created text component.</returns>
        [Obsolete("This method never worked properly for flat text. Use CreateCurvedUIText instead.")]
        public static T CreateText<T>(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
            where T : TMP_Text
        {
            GameObject gameObj = new("CustomUIText")
            {
                layer = 5,
            };

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

        /// <summary>
        /// Create a CurvedTextMeshPro component.
        /// </summary>
        /// <param name="parent">The parent <see cref="RectTransform"/>. Must have a <see cref="Canvas"/> in the hierarchy with <see cref="AdditionalCanvasShaderChannels.TexCoord2"/> added to <see cref="Canvas.additionalShaderChannels"/>.</param>
        /// <param name="text">The text to display.</param>
        /// <returns>The created <see cref="CurvedTextMeshPro"/> object.</returns>
        public static CurvedTextMeshPro CreateCurvedUIText(RectTransform parent, string text) => CreateCurvedUIText<CurvedTextMeshPro>(parent, text);

        /// <summary>
        /// Create a CurvedTextMeshPro component.
        /// </summary>
        /// <param name="parent">The parent <see cref="RectTransform"/>. Must have a <see cref="Canvas"/> in the hierarchy with <see cref="AdditionalCanvasShaderChannels.TexCoord2"/> added to <see cref="Canvas.additionalShaderChannels"/>.</param>
        /// <param name="text">The text to display.</param>
        /// <typeparam name="T">The type of <see cref="CurvedTextMeshPro"/> to create.</typeparam>
        /// <returns>The created <see cref="CurvedTextMeshPro"/> object.</returns>
        public static T CreateCurvedUIText<T>(RectTransform parent, string text)
            where T : CurvedTextMeshPro => CreateCurvedUIText<T>(parent, text, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

        /// <summary>
        /// Create a CurvedTextMeshPro component.
        /// </summary>
        /// <param name="parent">The parent <see cref="RectTransform"/>. Must have a <see cref="Canvas"/> in the hierarchy with <see cref="AdditionalCanvasShaderChannels.TexCoord2"/> added to <see cref="Canvas.additionalShaderChannels"/>.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="anchorMin">The <see cref="RectTransform.anchorMin"/> value.</param>
        /// <param name="anchorMax">The <see cref="RectTransform.anchorMax"/> value.</param>
        /// <param name="anchoredPosition">The <see cref="RectTransform.anchoredPosition"/> value.</param>
        /// <param name="sizeDelta">The <see cref="RectTransform.sizeDelta"/> value.</param>
        /// <returns>The created <see cref="CurvedTextMeshPro"/> object.</returns>
        public static CurvedTextMeshPro CreateCurvedUIText(RectTransform parent, string text, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta) => CreateCurvedUIText<CurvedTextMeshPro>(parent, text, anchorMin, anchorMax, anchoredPosition, sizeDelta);

        /// <summary>
        /// Create a CurvedTextMeshPro component.
        /// </summary>
        /// <param name="parent">The parent <see cref="RectTransform"/>. Must have a <see cref="Canvas"/> in the hierarchy with <see cref="AdditionalCanvasShaderChannels.TexCoord2"/> added to <see cref="Canvas.additionalShaderChannels"/>.</param>
        /// <param name="text">The text to display.</param>
        /// <param name="anchorMin">The <see cref="RectTransform.anchorMin"/> value.</param>
        /// <param name="anchorMax">The <see cref="RectTransform.anchorMax"/> value.</param>
        /// <param name="anchoredPosition">The <see cref="RectTransform.anchoredPosition"/> value.</param>
        /// <param name="sizeDelta">The <see cref="RectTransform.sizeDelta"/> value.</param>
        /// <typeparam name="T">The type of <see cref="CurvedTextMeshPro"/> to create.</typeparam>
        /// <returns>The created <see cref="CurvedTextMeshPro"/> object.</returns>
        public static T CreateCurvedUIText<T>(RectTransform parent, string text, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
             where T : CurvedTextMeshPro
        {
            GameObject gameObj = new("CustomCurvedUIText")
            {
                layer = 5,
            };

            gameObj.SetActive(false);

            T textComponent = gameObj.AddComponent<T>();
            textComponent.font = MainTextFont;
            textComponent.fontSharedMaterial = MainUIFontMaterial;
            textComponent.rectTransform.SetParent(parent, false);
            textComponent.text = text;
            textComponent.fontSize = 4;
            textComponent.color = Color.white;

            textComponent.rectTransform.anchorMin = anchorMin;
            textComponent.rectTransform.anchorMax = anchorMax;
            textComponent.rectTransform.sizeDelta = sizeDelta;
            textComponent.rectTransform.anchoredPosition = anchoredPosition;

            gameObj.SetActive(true);

            return textComponent;
        }

        [Obsolete]
        public static void SetButtonText(this Button button, string text)
        {
            LocalizedTextMeshProUGUI localizer = button.GetComponentInChildren<LocalizedTextMeshProUGUI>(true);
            if (localizer != null)
            {
                Object.Destroy(localizer);
            }

            TextMeshProUGUI tmpUgui = button.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpUgui != null)
            {
                tmpUgui.SetText(text);
            }
        }

        [Obsolete]
        public static void SetButtonTextSize(this Button button, float fontSize)
        {
            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.fontSize = fontSize;
            }
        }

        [Obsolete]
        public static void ToggleWordWrapping(this Button button, bool enableWordWrapping)
        {
            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.textWrappingMode = enableWordWrapping ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
            }
        }

        [Obsolete]
        public static void SetButtonIcon(this Button button, Sprite icon)
        {
            Image image = button.GetComponentsInChildren<Image>().Where(x => x.name == "Icon").First();
            if (image != null)
            {
                image.sprite = icon;
            }
        }

        [Obsolete]
        public static void SetButtonBackground(this Button button, Sprite background)
        {
            Image image = button.GetComponentInChildren<Image>();
            if (image != null)
            {
                image.sprite = background;
            }
        }

        [Obsolete]
        public static void SetButtonStates(this Button button, Sprite normal, Sprite hover)
        {
            ButtonSpriteSwap swap = button.GetComponent<ButtonSpriteSwap>();

            swap._disabledStateSprite = normal;
            swap._normalStateSprite = normal;
            swap._highlightStateSprite = hover;

            // Unneeded?
            swap._pressedStateSprite = hover;
        }

        /// <summary>
        /// Sets an image or gif/apng from a resource path.
        /// </summary>
        /// <param name="image">Image component to set the image to.</param>
        /// <param name="location">Resource path, file path, or url of image. Can prefix with # to find and use a base game sprite. May need to prefix resource paths with 'AssemblyName:'.</param>
        [Obsolete]
        public static void SetImage(this Image image, string location)
        {
            SetImage(image, location, true, default, null);
        }

        /// <summary>
        /// Sets an image or gif/apng from a resource path.
        /// </summary>
        /// <param name="image">Image component to set the image to.</param>
        /// <param name="location">Resource path, file path, or url of image. Can prefix with # to find and use a base game sprite. May need to prefix resource paths with 'AssemblyName:'.</param>
        /// <param name="loadingAnimation">Whether a loading animation is shown as a placeholder until the image is loaded.</param>
        /// <param name="scaleOptions">If the image should be downscaled and what it should be downscaled to.</param>
        [Obsolete]
        public static void SetImage(this Image image, string location, bool loadingAnimation, ScaleOptions scaleOptions)
        {
            SetImage(image, location, loadingAnimation, scaleOptions, null);
        }

        /// <summary>
        /// Sets an image or gif/apng from a resource path.
        /// </summary>
        /// <param name="image">Image component to set the image to.</param>
        /// <param name="location">Resource path, file path, or url of image. Can prefix with # to find and use a base game sprite. May need to prefix resource paths with 'AssemblyName:'.</param>
        /// <param name="loadingAnimation">Whether a loading animation is shown as a placeholder until the image is loaded.</param>
        /// <param name="scaleOptions">If the image should be downscaled and what it should be downscaled to.</param>
        /// <param name="callback">Method to call once SetImage has finished.</param>
        [Obsolete]
        public static void SetImage(this Image image, string location, bool loadingAnimation, ScaleOptions scaleOptions, Action callback)
        {
            SetImageAsync(image, location, loadingAnimation, scaleOptions).ContinueWith(
                t =>
                {
                    if (t.IsFaulted)
                    {
                        Logger.Log.Error(t.Exception);
                    }

                    callback?.Invoke();
                },
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Sets an image (PNG/JPEG/GIF/APNG/etc.) from a URL, local file, or resource path.
        /// </summary>
        /// <param name="image">Image component to set the image to.</param>
        /// <param name="location">Resource path, file path, or URL of the image to load. Can prefix with # to find and use a base game sprite. May need to prefix resource paths with 'AssemblyName:'.</param>
        /// <param name="loadingAnimation">Whether a loading animation is shown as a placeholder until the image is loaded.</param>
        /// <param name="scaleOptions">If the image should be downscaled and what it should be downscaled to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task SetImageAsync(this Image image, string location, bool loadingAnimation = true, ScaleOptions scaleOptions = default)
        {
            Utilities.EnsureRunningOnMainThread();

            if (image.TryGetComponent(out AnimationStateUpdater oldStateUpdater))
            {
                Object.DestroyImmediate(oldStateUpdater);
            }

            if (location.Length > 1 && location[0] == '#')
            {
                string imgName = location.Substring(1);
                image.sprite = Utilities.FindSpriteCached(imgName);
                if (image.sprite == null)
                {
                    Logger.Log.Error($"Could not find Sprite with image name {imgName}");
                }

                return;
            }

            bool isURL = Uri.TryCreate(location, UriKind.Absolute, out Uri uri);
            if (IsAnimated(location) || (isURL && IsAnimated(uri.LocalPath)))
            {
                AnimationStateUpdater stateUpdater = image.gameObject.AddComponent<AnimationStateUpdater>();
                stateUpdater.Image = image;
                if (loadingAnimation)
                {
                    stateUpdater.ControllerData = AnimationController.Instance.LoadingAnimation;
                }

                if (AnimationController.Instance.RegisteredAnimations.TryGetValue(location, out AnimationControllerData animControllerData))
                {
                    stateUpdater.ControllerData = animControllerData;
                }
                else
                {
                    byte[] data = await Utilities.GetDataAsync(location);
                    AnimationData animationData;

                    if (location.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) || (isURL && uri.LocalPath.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)))
                    {
                        animationData = await AnimationLoader.ProcessGifAsync(data);
                    }
                    else
                    {
                        animationData = await AnimationLoader.ProcessApngAsync(data);
                    }

                    AnimationControllerData controllerData = AnimationController.Instance.Register(location, animationData);
                    stateUpdater.ControllerData = controllerData;

                    return;
                }
            }
            else
            {
                AnimationStateUpdater stateUpdater = image.gameObject.AddComponent<AnimationStateUpdater>();
                stateUpdater.Image = image;

                if (loadingAnimation)
                {
                    stateUpdater.ControllerData = AnimationController.Instance.LoadingAnimation;
                }

                byte[] data = await Utilities.GetDataAsync(location);

                if (stateUpdater != null)
                {
                    Object.DestroyImmediate(stateUpdater);
                }

                if (scaleOptions.ShouldScale)
                {
                    data = await Task.Run(() => DownscaleImage(data, scaleOptions.Width, scaleOptions.Height, scaleOptions.MaintainRatio));
                }

                image.sprite = await Utilities.LoadSpriteAsync(data);
                image.sprite.texture.wrapMode = TextureWrapMode.Clamp;

                return;
            }
        }

        /// <summary>
        /// Downscale the image in <paramref name="data"/> to the specified size. This method uses <see cref="System.Drawing"/> (CPU) so it can be run on a non-main thread.
        /// </summary>
        /// <param name="data">Byte array containing the image data.</param>
        /// <param name="width">The maximum width of the scaled image.</param>
        /// <param name="height">The maximum height of the scaled image.</param>
        /// <param name="maintainAspectRatio">If true, the image will be scaled while maintaining its original aspect ratio. The image will be scaled to fit within the bounds defined by <paramref name="width"/> × <paramref name="height"/>.</param>
        /// <returns>A byte array containing the image data.</returns>
        public static byte[] DownscaleImage(byte[] data, int width, int height, bool maintainAspectRatio = true)
        {
            try
            {
                // this memory stream needs to stay open or else GDI+ dies
                using MemoryStream workMemoryStream = new(data);
                System.Drawing.Image originalImage = System.Drawing.Image.FromStream(workMemoryStream);

                if (originalImage.Width <= width && originalImage.Height <= height)
                {
                    return data;
                }

                System.Drawing.Size newSize;
                if (maintainAspectRatio)
                {
                    double scale = Math.Min((double)width / originalImage.Width, (double)height / originalImage.Height);
                    newSize = new System.Drawing.Size((int)Math.Round(originalImage.Width * scale), (int)Math.Round(originalImage.Height * scale));
                }
                else
                {
                    newSize = new System.Drawing.Size(Math.Min(width, originalImage.Width), Math.Min(height, originalImage.Height));
                }

                System.Drawing.Bitmap resizedImage = new(newSize.Width, newSize.Height);

                using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(resizedImage))
                {
                    // TODO: eventually add options for low/high quality resizing
                    graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    graphics.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, newSize.Width, newSize.Height), 0, 0, originalImage.Width, originalImage.Height, System.Drawing.GraphicsUnit.Pixel);
                }

                using MemoryStream saveMemoryStream = new();
                resizedImage.Save(saveMemoryStream, originalImage.RawFormat);

                return saveMemoryStream.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Failed to downscale image; returning original\n{ex}");
                return data;
            }
        }

        public static void PresentFlowCoordinator(this FlowCoordinator current, FlowCoordinator flowCoordinator, Action finishedCallback = null, ViewController.AnimationDirection animationDirection = ViewController.AnimationDirection.Horizontal, bool immediately = false, bool replaceTopViewController = false)
            => current.PresentFlowCoordinator(flowCoordinator, finishedCallback, animationDirection, immediately, replaceTopViewController);

        public static void DismissFlowCoordinator(this FlowCoordinator current, FlowCoordinator flowCoordinator, Action finishedCallback = null, ViewController.AnimationDirection animationDirection = ViewController.AnimationDirection.Horizontal, bool immediately = false)
            => current.DismissFlowCoordinator(flowCoordinator, animationDirection, finishedCallback, immediately);

        internal static void Init(DiContainer container)
        {
            diContainer = container;

            if (!TryGetSoloButton(out Button soloButton))
            {
                Logger.Log.Error("Failed to get Solo button. Fonts will not be set up.");
                return;
            }

            TextMeshProUGUI textMesh = soloButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();

            MainTextFont = textMesh.font;
            MainUIFontMaterial = textMesh.fontSharedMaterial;

            MonochromeTextFont = CopyFontAsset(MainTextFont, MainUIFontMaterial, $"{MainTextFont.name} Monochrome");

            MainFlatUIFontMaterial = new Material(MainUIFontMaterial);
            MainFlatUIFontMaterial.DisableKeyword("CURVED");
        }

        internal static bool TryGetSoloButton(out Button soloButton)
        {
            if (DiContainer == null)
            {
                soloButton = null;
                return false;
            }

            soloButton = DiContainer.Resolve<MainMenuViewController>()._soloButton;
            return soloButton != null;
        }

        private static TMP_FontAsset CopyFontAsset(TMP_FontAsset original, Material referenceMaterial, string newName)
        {
            TMP_FontAsset copy = Object.Instantiate(original);

            // Unity doesn't copy textures when using Object.Instantiate so we have to do it manually
            Texture2D[] newTextures = CopyTextures(original.atlasTextures);

            Texture2D texture = original.m_AtlasTexture;
            Texture2D newTexture;
            int index = Array.IndexOf(original.atlasTextures, texture);

            if (index >= 0)
            {
                newTexture = newTextures[index];
            }
            else
            {
                newTexture = CopyTexture(texture, $"{texture.name} Atlas");
            }

            Material material = new(referenceMaterial)
            {
                name = $"{newName} Atlas Material",
            };

            material.SetTexture("_MainTex", newTexture);

            copy.m_AtlasTexture = newTexture;
            copy.name = newName;
            copy.atlasTextures = newTextures;
            copy.material = material;

            return copy;
        }

        private static Texture2D[] CopyTextures(Texture2D[] textures)
        {
            Texture2D[] newTextures = new Texture2D[textures.Length];

            for (int i = 0; i < textures.Length; i++)
            {
                newTextures[i] = CopyTexture(textures[i], $"{textures[i].name} Atlas {i}");
            }

            return newTextures;
        }

        private static Texture2D CopyTexture(Texture2D texture, string newName)
        {
            bool shouldCopy = texture != null && texture.width > 0 && texture.height > 0;

            // 1 × 1 texture causes TMP to reinitialize the texture
            Texture2D newTexture = new(shouldCopy ? texture.width : 1, shouldCopy ? texture.height : 1, texture.graphicsFormat, texture.mipmapCount, TextureCreationFlags.DontInitializePixels | TextureCreationFlags.DontUploadUponCreate)
            {
                name = newName,
            };

            if (shouldCopy)
            {
                Graphics.CopyTexture(texture, newTexture);
            }

            return newTexture;
        }

        private static bool IsAnimated(string str)
        {
            return str.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) || str.EndsWith(".apng", StringComparison.OrdinalIgnoreCase);
        }

        public struct ScaleOptions
        {
            public bool ShouldScale;
            public bool MaintainRatio;
            public int Width;
            public int Height;
        }
    }
}
