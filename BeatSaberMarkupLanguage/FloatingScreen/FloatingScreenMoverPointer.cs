using System;
using System.Linq;
using UnityEngine;
using VRUIControls;

namespace BeatSaberMarkupLanguage.FloatingScreen
{
    // yoinked from https://github.com/Kylemc1413/CameraPlus/blob/master/CameraPlus/CameraMoverPointer.cs
    [Obsolete]
    public class FloatingScreenMoverPointer : MonoBehaviour
    {
        [Obsolete("Use FloatingScreen.HandleGrabbed event")]
        public Action<Vector3, Quaternion> OnGrab;

        [Obsolete("Use FloatingScreen.HandleReleased event")]
        public Action<Vector3, Quaternion> OnRelease;

        protected const float MinScrollDistance = 0.25f;
        protected const float MaxLaserDistance = 50;

        protected VRPointer vrPointer;
        protected FloatingScreen floatingScreen;
        protected Transform screenHandle;
        protected VRController grabbingController;
        protected Vector3 grabPos;
        protected Quaternion grabRot;
        protected Vector3 realPos;
        protected Quaternion realRot;
        protected FirstPersonFlyingController fpfc;

        private bool IsFpfc => fpfc != null && fpfc.enabled;

        public virtual void Init(FloatingScreen floatingScreen, VRPointer pointer)
        {
            this.floatingScreen = floatingScreen;
            screenHandle = floatingScreen.Handle.transform;
            realPos = floatingScreen.transform.position;
            realRot = floatingScreen.transform.rotation;
            vrPointer = pointer;
            fpfc = Resources.FindObjectsOfTypeAll<FirstPersonFlyingController>().FirstOrDefault();
        }

        public virtual void Init(FloatingScreen floatingScreen)
        {
            VRPointer vrPointer = GetComponent<VRPointer>();
            Init(floatingScreen, vrPointer);
        }

        protected virtual void Update()
        {
            VRPointer pointer = vrPointer;
            VRController vrController = pointer != null ? pointer.lastSelectedVrController : null;

            if (vrController != null && vrController.active && (vrController.triggerValue > 0.9f || Input.GetMouseButton(0)))
            {
                if (grabbingController != null)
                {
                    return;
                }

                if (Physics.Raycast(vrController.viewAnchorTransform.position, vrController.viewAnchorTransform.forward, out RaycastHit hit, MaxLaserDistance))
                {
                    if (hit.transform != screenHandle)
                    {
                        return;
                    }

                    grabbingController = vrController;
                    grabPos = vrController.transform.InverseTransformPoint(floatingScreen.transform.position);
                    grabRot = Quaternion.Inverse(vrController.transform.rotation) * floatingScreen.transform.rotation;
                    floatingScreen.OnHandleGrab(pointer);
                    OnGrab?.Invoke(floatingScreen.transform.position, floatingScreen.transform.rotation);
                }
            }

            if (grabbingController == null || (!IsFpfc && grabbingController.triggerValue > 0.9f) || (IsFpfc && Input.GetMouseButton(0)))
            {
                return;
            }

            grabbingController = null;
            floatingScreen.OnHandleReleased(pointer);
            OnRelease?.Invoke(floatingScreen.transform.position, floatingScreen.transform.rotation);
        }

        protected void OnDestroy()
        {
            OnGrab = null;
            OnRelease = null;
            vrPointer = null;
            floatingScreen = null;
            screenHandle = null;
            grabbingController = null;
        }

        protected virtual void LateUpdate()
        {
            if (grabbingController != null)
            {
                float diff = grabbingController.thumbstick.y * Time.unscaledDeltaTime;

                if (grabPos.magnitude > MinScrollDistance)
                {
                    grabPos -= Vector3.forward * diff;
                }
                else
                {
                    grabPos -= Vector3.forward * Mathf.Clamp(diff, float.MinValue, 0);
                }

                realPos = grabbingController.transform.TransformPoint(grabPos);
                realRot = grabbingController.transform.rotation * grabRot;
            }
            else
            {
                return;
            }

            floatingScreen.transform.SetPositionAndRotation(
                Vector3.Lerp(floatingScreen.transform.position, realPos, 10 * Time.unscaledDeltaTime),
                Quaternion.Slerp(floatingScreen.transform.rotation, realRot, 5 * Time.unscaledDeltaTime));
        }
    }
}
