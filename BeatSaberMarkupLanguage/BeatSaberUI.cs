using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Animations;
using HMUI;
using IPA.Utilities.Async;
using TMPro;
using UnityEngine;
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
        private static Canvas canvasTemplate;
        private static TMP_FontAsset mainTextFont;
        private static Material mainUIFontMaterial;

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
            internal set => diContainer = value;
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
                    basicUIAudioManager = Resources.FindObjectsOfTypeAll<BasicUIAudioManager>().First();
                }

                return basicUIAudioManager;
            }
        }

        /// <summary>
        /// Gets the main font used by the game for UI text.
        /// </summary>
        public static TMP_FontAsset MainTextFont
        {
            get
            {
                if (mainTextFont == null)
                {
                    mainTextFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().Where(t => t.name == "Teko-Medium SDF").FirstOrDefault();
                }

                return mainTextFont;
            }
        }

        internal static ICoroutineStarter CoroutineStarter { get; set; }

        internal static Material MainUIFontMaterial
        {
            get
            {
                if (mainUIFontMaterial == null)
                {
                    mainUIFontMaterial = Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "Teko-Medium SDF Curved Softer").First();
                }

                return mainUIFontMaterial;
            }
        }

        /// <summary>
        /// Creates a ViewController of type T, and marks it to not be destroyed.
        /// </summary>
        /// <typeparam name="T">The variation of ViewController you want to create.</typeparam>
        /// <returns>The newly created ViewController of type T.</returns>
        public static T CreateViewController<T>()
            where T : ViewController
        {
            if (canvasTemplate == null)
            {
                canvasTemplate = Resources.FindObjectsOfTypeAll<Canvas>().First(x => x.name == "DropdownTableView");
            }

            GameObject go = new(typeof(T).Name);
            go.AddComponent(canvasTemplate);
            DiContainer.InstantiateComponent<VRGraphicRaycaster>(go);
            go.AddComponent<CanvasGroup>();
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
        public static T CreateFlowCoordinator<T>()
            where T : FlowCoordinator => DiContainer.InstantiateComponentOnNewGameObject<T>(typeof(T).Name);

        /// <summary>
        /// Creates a clone of the given font, with its material fixed to be a no-glow material suitable for use on UI elements.
        /// </summary>
        /// <param name="font">The font to clone and fix.</param>
        /// <returns>The fixed clone.</returns>
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
        /// <remarks>
        /// The <see cref="TMP_FontAsset"/> returned is not usable for UI text. Use <see cref="CreateFixedUIFontClone(TMP_FontAsset)"/>
        /// to get a usable font.
        /// </remarks>
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
        public static T CreateText<T>(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
            where T : TMP_Text
        {
            GameObject gameObj = new("CustomUIText");
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

        public static void SetButtonText(this Button button, string text)
        {
            Polyglot.LocalizedTextMeshProUGUI localizer = button.GetComponentInChildren<Polyglot.LocalizedTextMeshProUGUI>(true);
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

        public static void SetButtonTextSize(this Button button, float fontSize)
        {
            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.fontSize = fontSize;
            }
        }

        public static void ToggleWordWrapping(this Button button, bool enableWordWrapping)
        {
            TextMeshProUGUI textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.enableWordWrapping = enableWordWrapping;
            }
        }

        public static void SetButtonIcon(this Button button, Sprite icon)
        {
            Image image = button.GetComponentsInChildren<Image>().First(x => x.name == "Icon");
            if (image != null)
            {
                image.sprite = icon;
            }
        }

        public static void SetButtonBackground(this Button button, Sprite background)
        {
            Image image = button.GetComponentInChildren<Image>();
            if (image != null)
            {
                image.sprite = background;
            }
        }

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
        public static void SetImage(this Image image, string location, bool loadingAnimation, ScaleOptions scaleOptions, Action callback)
        {
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
                stateUpdater.image = image;
                if (loadingAnimation)
                {
                    stateUpdater.controllerData = AnimationController.instance.loadingAnimation;
                }

                if (AnimationController.instance.RegisteredAnimations.TryGetValue(location, out AnimationControllerData animControllerData))
                {
                    stateUpdater.controllerData = animControllerData;
                }
                else
                {
                    Utilities.GetData(location, (byte[] data) =>
                    {
                        AnimationLoader.Process(
                            (location.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) || (isURL && uri.LocalPath.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))) ? AnimationType.GIF : AnimationType.APNG,
                            data,
                            (Texture2D tex, Rect[] uvs, float[] delays, int width, int height) =>
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
                {
                    stateUpdater.controllerData = AnimationController.instance.loadingAnimation;
                }

                Utilities.GetData(location, async (byte[] data) =>
                {
                    if (stateUpdater != null)
                    {
                        Object.DestroyImmediate(stateUpdater);
                    }

                    if (scaleOptions.ShouldScale)
                    {
                        byte[] imageBytes = await Task.Run(() => DownscaleImage(data, scaleOptions.Width, scaleOptions.Height, scaleOptions.MaintainRatio)).ConfigureAwait(false);
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
