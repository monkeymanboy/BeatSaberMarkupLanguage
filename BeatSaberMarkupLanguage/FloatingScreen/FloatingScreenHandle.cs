using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using VRUIControls;

namespace BeatSaberMarkupLanguage.FloatingScreen
{
    internal class FloatingScreenHandle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly Color DefaultColor = new(1, 1, 1, 0);
        private static readonly Color HoverColor = new(1, 1, 0, 1);

        private static Shader shader;

        private FloatingScreen _floatingScreen;
        private Material _material;
        private VRController _grabbingController;
        private Vector3 _grabPos;
        private Quaternion _grabRot;

        private bool _isHovering;

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
            UpdateMaterial();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
            UpdateMaterial();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RaycastResult result = eventData.pointerPressRaycast;

            if (result.gameObject != gameObject)
            {
                return;
            }

            if (EventSystem.current == null || EventSystem.current.currentInputModule is not VRInputModule vrInputModule)
            {
                return;
            }

            VRPointer vrPointer = vrInputModule._vrPointer;
            VRController vrController = vrPointer.lastSelectedVrController;
            _grabbingController = vrController;
            _grabPos = vrController.transform.InverseTransformPoint(_floatingScreen.transform.position);
            _grabRot = Quaternion.Inverse(vrController.transform.rotation) * _floatingScreen.transform.rotation;
            _floatingScreen.OnHandleGrab(vrPointer);

            UpdateMaterial();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (EventSystem.current == null || EventSystem.current.currentInputModule is not VRInputModule vrInputModule)
            {
                return;
            }

            _grabbingController = null;
            _floatingScreen.OnHandleReleased(vrInputModule._vrPointer);

            UpdateMaterial();
        }

        internal void Init(FloatingScreen floatingScreen)
        {
            _floatingScreen = floatingScreen;
        }

        private void Awake()
        {
            if (shader == null)
            {
                shader = Resources.FindObjectsOfTypeAll<Shader>().First(s => s.name == "Custom/Glowing");
            }

            _material = GetComponent<MeshRenderer>().material;
            _material.shader = shader;
            _material.SetColor(ColorId, DefaultColor);
        }

        private void Update()
        {
            if (_grabbingController == null)
            {
                return;
            }

            _grabPos -= Vector3.forward * (_grabbingController.thumbstick.y * Time.unscaledDeltaTime);

            Vector3 targetPosition = _grabbingController.transform.TransformPoint(_grabPos);
            Quaternion targetRotation = _grabbingController.transform.rotation * _grabRot;

            _floatingScreen.transform.SetPositionAndRotation(
                Vector3.Lerp(_floatingScreen.transform.position, targetPosition, 10 * Time.unscaledDeltaTime),
                Quaternion.Slerp(_floatingScreen.transform.rotation, targetRotation, 5 * Time.unscaledDeltaTime));
        }

        private void UpdateMaterial()
        {
            if (_floatingScreen.HighlightHandle && (_isHovering || _grabbingController != null))
            {
                _material.SetColor(ColorId, HoverColor);
            }
            else
            {
                _material.SetColor(ColorId, DefaultColor);
            }
        }
    }
}
