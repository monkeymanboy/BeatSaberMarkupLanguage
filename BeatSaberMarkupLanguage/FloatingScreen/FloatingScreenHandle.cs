using UnityEngine;
using UnityEngine.EventSystems;

namespace BeatSaberMarkupLanguage.FloatingScreen
{
    internal class FloatingScreenHandle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private static readonly Color DefaultColor = new(1, 1, 1, 0);
        private static readonly Color HoverColor = new(1, 1, 0, 1);

        private static Shader shader;

        private Material material;

        public void Awake()
        {
            if (shader == null)
            {
                shader = Shader.Find("Custom/Glowing");
            }

            material = GetComponent<MeshRenderer>().material;
            material.shader = shader;
            material.SetColor(ColorId, DefaultColor);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            material.SetColor(ColorId, HoverColor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            material.SetColor(ColorId, DefaultColor);
        }
    }
}
