using HMUI;
using IPA.Utilities;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;
using Screen = HMUI.Screen;

namespace BeatSaberMarkupLanguage.FloatingScreen
{
    public class FloatingScreen : Screen
    {
        public FloatingScreenMoverPointer screenMover;
        public GameObject handle;
        public event EventHandler<FloatingScreenHandleEventArgs> HandleReleased;
        public event EventHandler<FloatingScreenHandleEventArgs> HandleGrabbed;
        
        public Vector2 ScreenSize
        {
            get => (transform as RectTransform).sizeDelta;
            set
            {
                (transform as RectTransform).sizeDelta = value;
                UpdateHandle();
            }
        }
        
        public Vector3 ScreenPosition
        {
            get => transform.position;
            set
            {
                (transform as RectTransform).position = value;
            }
        }
        
        public Quaternion ScreenRotation
        {
            get => (transform as RectTransform).rotation;
            set
            {
                (transform as RectTransform).rotation = value;
            }
        }

        private bool _showHandle = false;
        public bool ShowHandle
        {
            get => _showHandle;
            set
            {
                _showHandle = value;
                if(_showHandle)
                {
                    if (handle == null)
                        CreateHandle();
                    else
                        handle.SetActive(true);
                }
                else if(!_showHandle && handle != null)
                {
                    handle.SetActive(false);
                }
            }
        }

        private Side _handleSide = Side.Left;
        public Side HandleSide
        {
            get => _handleSide;
            set
            {
                _handleSide = value;
                UpdateHandle();
            }
        }

        public static FloatingScreen CreateFloatingScreen(Vector2 screenSize, bool createHandle, Vector3 position, Quaternion rotation)//for binary compatibility
        {
            return CreateFloatingScreen(screenSize, createHandle, position, rotation, 0, false);
        }

        public static FloatingScreen CreateFloatingScreen(Vector2 screenSize, bool createHandle, Vector3 position, Quaternion rotation, float curvatureRadius = 0f, bool hasBackground = false)
        {
            FloatingScreen screen = new GameObject("BSMLFloatingScreen", typeof(FloatingScreen), typeof(CanvasScaler), typeof(RectMask2D), typeof(VRGraphicRaycaster), typeof(CurvedCanvasSettings)).GetComponent<FloatingScreen>();
            screen.GetComponent<VRGraphicRaycaster>().SetField("_physicsRaycaster", BeatSaberUI.PhysicsRaycasterWithCache);

            CurvedCanvasSettings curvedCanvasSettings = screen.GetComponent<CurvedCanvasSettings>();
            curvedCanvasSettings.SetRadius(curvatureRadius);

            Canvas canvas = screen.GetComponent<Canvas>();
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.TexCoord2;
            canvas.sortingOrder = 4;

            CanvasScaler scaler = screen.GetComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 3.44f;
            scaler.referencePixelsPerUnit = 10f;
            
            if (hasBackground)
            {
                GameObject backGroundGo = new GameObject("bg", typeof(RectTransform), typeof(ImageView));
                backGroundGo.transform.SetParent(canvas.transform, false);
                RectTransform rectTransform = backGroundGo.GetComponent<RectTransform>();
                rectTransform.sizeDelta = screenSize;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                ImageView background = backGroundGo.GetComponent<ImageView>();
                background.sprite = Resources.FindObjectsOfTypeAll<Sprite>().First(x => x.name == "MainScreenMask");
                background.type = Image.Type.Sliced;
                background.color = new Color(0.7450981f, 0.7450981f, 0.7450981f, 1f);
                background.material = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name == "UIFogBG");
                background.preserveAspect = true;
            }

            var screenTransform = screen.transform;
            screenTransform.position = position;
            screenTransform.rotation = rotation;
            screenTransform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            screen.ScreenSize = screenSize;
            screen.ShowHandle = createHandle;

            return screen;
        }

        private void CreateHandle()
        {
            VRPointer pointer = Resources.FindObjectsOfTypeAll<VRPointer>().FirstOrDefault();
            if (pointer != null)
            {
                if (screenMover) Destroy(screenMover);
                screenMover = pointer.gameObject.AddComponent<FloatingScreenMoverPointer>();
                handle = GameObject.CreatePrimitive(PrimitiveType.Cube);

                handle.transform.SetParent(transform, false);
                UpdateHandle();

                screenMover.Init(this);
            }
            else
            {
                Logger.log.Warn("Failed to get VRPointer!");
            }
        }

        internal void OnHandleGrab(VRPointer vrPointer)
        {
            HandleGrabbed?.Invoke(this, new FloatingScreenHandleEventArgs(vrPointer, transform.position, transform.rotation));
        }
        internal void OnHandleReleased(VRPointer vrPointer)
        {
            HandleReleased?.Invoke(this, new FloatingScreenHandleEventArgs(vrPointer, transform.position, transform.rotation));
        }

        public void UpdateHandle()
        {
            if (handle == null) return;
            switch (HandleSide)
            {
                case Side.Left:
                    handle.transform.localPosition = new Vector3(-ScreenSize.x / 2f, 0f, 0f);
                    handle.transform.localScale = new Vector3(ScreenSize.x / 15f, ScreenSize.y * 0.8f, ScreenSize.x / 15f);
                    break;
                case Side.Right:
                    handle.transform.localPosition = new Vector3(ScreenSize.x / 2f, 0f, 0f);
                    handle.transform.localScale = new Vector3(ScreenSize.x / 15f, ScreenSize.y * 0.8f, ScreenSize.x / 15f);
                    break;
                case Side.Top:
                    handle.transform.localPosition = new Vector3(0f, ScreenSize.y / 2f, 0f);
                    handle.transform.localScale = new Vector3(ScreenSize.x * 0.8f, ScreenSize.y / 15f, ScreenSize.y / 15f);
                    break;
                case Side.Bottom:
                    handle.transform.localPosition = new Vector3(0f, -ScreenSize.y / 2f, 0f);
                    handle.transform.localScale = new Vector3(ScreenSize.x * 0.8f, ScreenSize.y / 15f, ScreenSize.y / 15f);
                    break;
                case Side.Full:
                    handle.transform.localPosition = Vector3.zero;
                    handle.transform.localScale = new Vector3(ScreenSize.x, ScreenSize.y, ScreenSize.x / 15f);
                    break;
            }

            handle.GetComponent<MeshRenderer>().enabled = HandleSide != Side.Full;
        }

        public enum Side
        {
            Left, Right, Bottom, Top, Full
        }
    }
    public struct FloatingScreenHandleEventArgs
    {
        public FloatingScreenHandleEventArgs(VRPointer vrPointer, Vector3 position, Quaternion rotation)
        {
            Pointer = vrPointer;
            Position = position;
            Rotation = rotation;
        }

        public readonly VRPointer Pointer;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;

        public override bool Equals(object obj)
        {
            if (obj is FloatingScreenHandleEventArgs posRot)
            {
                return Position == posRot.Position && Rotation == posRot.Rotation;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Rotation.GetHashCode();
        }

        public static bool operator ==(FloatingScreenHandleEventArgs left, FloatingScreenHandleEventArgs right)
        {
            return left.Position == right.Position && left.Rotation == right.Rotation;
        }

        public static bool operator !=(FloatingScreenHandleEventArgs left, FloatingScreenHandleEventArgs right)
        {
            return !(left == right);
        }
    }
}
