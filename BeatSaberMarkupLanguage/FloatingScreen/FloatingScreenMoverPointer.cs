using System;
using System.Linq;
using UnityEngine;
using VRUIControls;

namespace BeatSaberMarkupLanguage.FloatingScreen
{
    // yoinked from https://github.com/Kylemc1413/CameraPlus/blob/master/CameraPlus/CameraMoverPointer.cs
    public class FloatingScreenMoverPointer : MonoBehaviour
    {
        [Obsolete("Use FloatingScreen.HandleGrabbed event")]
        public Action<Vector3, Quaternion> OnGrab;

        [Obsolete("Use FloatingScreen.HandleReleased event")]
        public Action<Vector3, Quaternion> OnRelease;

        protected const float MinScrollDistance = 0.25f;
        protected const float MaxLaserDistance = 50;

        protected VRPointer _vrPointer;
        protected FloatingScreen _floatingScreen;
        protected Transform _screenHandle;
        protected VRController _grabbingController;
        protected Vector3 _grabPos;
        protected Quaternion _grabRot;
        protected Vector3 _realPos;
        protected Quaternion _realRot;
        protected FirstPersonFlyingController _fpfc;

        private bool IsFpfc => _fpfc != null && _fpfc.enabled;

        public virtual void Init(FloatingScreen floatingScreen, VRPointer pointer)
        {
            _floatingScreen = floatingScreen;
            _screenHandle = floatingScreen.handle.transform;
            _realPos = floatingScreen.transform.position;
            _realRot = floatingScreen.transform.rotation;
            _vrPointer = pointer;
            _fpfc = Resources.FindObjectsOfTypeAll<FirstPersonFlyingController>().FirstOrDefault();
        }

        public virtual void Init(FloatingScreen floatingScreen)
        {
            VRPointer vrPointer = GetComponent<VRPointer>();
            Init(floatingScreen, vrPointer);
        }

#pragma warning disable CS0618
        protected virtual void Update()
        {
            VRPointer pointer = _vrPointer;
            VRController vrController = pointer != null ? pointer.lastSelectedVrController : null;

            if ((vrController != null && vrController.triggerValue > 0.9f) || Input.GetMouseButton(0))
            {
                if (_grabbingController != null)
                {
                    return;
                }

                if (Physics.Raycast(vrController.position, vrController.forward, out RaycastHit hit, MaxLaserDistance))
                {
                    if (hit.transform != _screenHandle)
                    {
                        return;
                    }

                    _grabbingController = vrController;
                    _grabPos = vrController.transform.InverseTransformPoint(_floatingScreen.transform.position);
                    _grabRot = Quaternion.Inverse(vrController.transform.rotation) * _floatingScreen.transform.rotation;
                    _floatingScreen.OnHandleGrab(pointer);
                    OnGrab?.Invoke(_floatingScreen.transform.position, _floatingScreen.transform.rotation);
                }
            }

            if (_grabbingController == null || (!IsFpfc && _grabbingController.triggerValue > 0.9f) || (IsFpfc && Input.GetMouseButton(0)))
            {
                return;
            }

            _grabbingController = null;
            _floatingScreen.OnHandleReleased(pointer);
            OnRelease?.Invoke(_floatingScreen.transform.position, _floatingScreen.transform.rotation);
        }

        protected void OnDestroy()
        {
            OnGrab = null;
            OnRelease = null;
            _vrPointer = null;
            _floatingScreen = null;
            _screenHandle = null;
            _grabbingController = null;
        }
#pragma warning restore CS0618

        protected virtual void LateUpdate()
        {
            if (_grabbingController != null)
            {
                float diff = _grabbingController.thumbstick.y * Time.unscaledDeltaTime;

                if (_grabPos.magnitude > MinScrollDistance)
                {
                    _grabPos -= Vector3.forward * diff;
                }
                else
                {
                    _grabPos -= Vector3.forward * Mathf.Clamp(diff, float.MinValue, 0);
                }

                _realPos = _grabbingController.transform.TransformPoint(_grabPos);
                _realRot = _grabbingController.transform.rotation * _grabRot;
            }
            else
            {
                return;
            }

            _floatingScreen.transform.SetPositionAndRotation(
                Vector3.Lerp(_floatingScreen.transform.position, _realPos, 10 * Time.unscaledDeltaTime),
                Quaternion.Slerp(_floatingScreen.transform.rotation, _realRot, 5 * Time.unscaledDeltaTime));
        }
    }
}
