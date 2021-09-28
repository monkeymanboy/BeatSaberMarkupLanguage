using UnityEngine;
using UnityEngine.EventSystems;

namespace BeatSaberMarkupLanguage.FloatingScreen
{
    internal class FloatingScreenHandle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private static Material hoverMaterial = new Material(Shader.Find("Hidden/Internal-DepthNormalsTexture"));
        private Material originalMaterial;

        private MeshRenderer renderer;

        public void Awake()
        {
            renderer = GetComponent<MeshRenderer>();
            originalMaterial = renderer.material;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            renderer.material = hoverMaterial;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            renderer.material = originalMaterial;
        }
    }
}
