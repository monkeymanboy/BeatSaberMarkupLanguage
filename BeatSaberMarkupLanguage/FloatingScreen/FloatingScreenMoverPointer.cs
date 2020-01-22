using System;
using UnityEngine;
using VRUIControls;

namespace BeatSaberMarkupLanguage.FloatingScreen
{
    //yoinked from https://github.com/Kylemc1413/CameraPlus/blob/master/CameraPlus/CameraMoverPointer.cs
    public class FloatingScreenMoverPointer : MonoBehaviour
    {
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
        protected bool _isFpfc;

        public Action<Vector3, Quaternion> OnGrab;
        public Action<Vector3, Quaternion> OnRelease;

        public virtual void Init(FloatingScreen floatingScreen)
        {
            _floatingScreen = floatingScreen;
            _screenHandle = floatingScreen.handle.transform;
            _realPos = floatingScreen.transform.position;
            _realRot = floatingScreen.transform.rotation;
            _vrPointer = GetComponent<VRPointer>();
            _isFpfc = Environment.CommandLine.Contains("fpfc");
        }

        protected virtual void Update()
        {
            if (_vrPointer.vrController != null)
                if (_vrPointer.vrController.triggerValue > 0.9f || Input.GetMouseButton(0))
                {
                    if (_grabbingController != null) return;
                    if (Physics.Raycast(_vrPointer.vrController.position, _vrPointer.vrController.forward, out RaycastHit hit, MaxLaserDistance))
                    {
                        if (hit.transform != _screenHandle) return;
                        _grabbingController = _vrPointer.vrController;
                        _grabPos = _vrPointer.vrController.transform.InverseTransformPoint(_floatingScreen.transform.position);
                        _grabRot = Quaternion.Inverse(_vrPointer.vrController.transform.rotation) * _floatingScreen.transform.rotation;
                        OnGrab?.Invoke(_floatingScreen.transform.position, _floatingScreen.transform.rotation);
                    }
                }

            if (_grabbingController == null || !_isFpfc && _grabbingController.triggerValue > 0.9f ||
                _isFpfc && Input.GetMouseButton(0)) return;
            _grabbingController = null;
            OnRelease?.Invoke(_floatingScreen.transform.position, _floatingScreen.transform.rotation);
        }

        protected virtual void LateUpdate()
        {
            if (_grabbingController != null)
            {
                var diff = _grabbingController.verticalAxisValue * Time.unscaledDeltaTime;
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
            else return;

            
            _floatingScreen.transform.position = Vector3.Lerp(_floatingScreen.transform.position, _realPos, 10 * Time.unscaledDeltaTime);

            _floatingScreen.transform.rotation = Quaternion.Slerp(_floatingScreen.transform.rotation, _realRot, 5 * Time.unscaledDeltaTime);
              
        }
    }
}
