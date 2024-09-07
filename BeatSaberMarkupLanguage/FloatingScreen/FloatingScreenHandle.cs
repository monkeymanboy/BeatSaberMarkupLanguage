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

        private FloatingScreen floatingScreen;
        private Material material;
        private VRController grabbingController;
        private Vector3 grabPos;
        private Quaternion grabRot;

        private bool isHovering;

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovering = true;
            UpdateMaterial();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
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
            grabbingController = vrController;
            grabPos = vrController.transform.InverseTransformPoint(floatingScreen.transform.position);
            grabRot = Quaternion.Inverse(vrController.transform.rotation) * floatingScreen.transform.rotation;
            floatingScreen.OnHandleGrab(vrPointer);

            UpdateMaterial();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (EventSystem.current == null || EventSystem.current.currentInputModule is not VRInputModule vrInputModule)
            {
                return;
            }

            grabbingController = null;
            floatingScreen.OnHandleReleased(vrInputModule._vrPointer);

            UpdateMaterial();
        }

        internal void Init(FloatingScreen floatingScreen)
        {
            this.floatingScreen = floatingScreen;
        }

        private void Awake()
        {
            if (shader == null)
            {
                shader = Resources.FindObjectsOfTypeAll<Shader>().First(s => s.name == "Custom/Glowing");
            }

            material = GetComponent<MeshRenderer>().material;
            material.shader = shader;
            material.SetColor(ColorId, DefaultColor);
        }

        private void Update()
        {
            if (grabbingController == null)
            {
                return;
            }

            grabPos -= Vector3.forward * (grabbingController.thumbstick.y * Time.unscaledDeltaTime);

            Vector3 targetPosition = grabbingController.transform.TransformPoint(grabPos);
            Quaternion targetRotation = grabbingController.transform.rotation * grabRot;

            floatingScreen.transform.SetPositionAndRotation(
                Vector3.Lerp(floatingScreen.transform.position, targetPosition, 10 * Time.unscaledDeltaTime),
                Quaternion.Slerp(floatingScreen.transform.rotation, targetRotation, 5 * Time.unscaledDeltaTime));
        }

        private void UpdateMaterial()
        {
            if (floatingScreen.HighlightHandle && (isHovering || grabbingController != null))
            {
                material.SetColor(ColorId, HoverColor);
            }
            else
            {
                material.SetColor(ColorId, DefaultColor);
            }
        }
    }
}
