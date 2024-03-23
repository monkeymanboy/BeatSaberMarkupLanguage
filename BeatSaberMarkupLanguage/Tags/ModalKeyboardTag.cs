using BeatSaberMarkupLanguage.Components;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class ModalKeyboardTag : ModalTag
    {
        public override string[] Aliases => new[] { "modal-keyboard" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = base.CreateObject(parent);

            KEYBOARD keyboard = new((RectTransform)gameObject.transform, KEYBOARD.QWERTY, true, 4, -12);
            ((RectTransform)gameObject.transform).localScale *= 1.4f;

            ModalKeyboard modalKeyboard = gameObject.AddComponent<ModalKeyboard>();
            modalKeyboard.keyboard = keyboard;
            modalKeyboard.modalView = gameObject.GetComponent<ModalView>();
            keyboard.EnterPressed += modalKeyboard.OnEnter;

            return gameObject;
        }

        protected override PrefabParams CreatePrefab()
        {
            PrefabParams prefab = base.CreatePrefab();
            GameObject gameObject = prefab.ContainerObject;

            RectTransform windowTransform = gameObject.transform as RectTransform;
            windowTransform.name = "BSMLModalKeyboard";
            windowTransform.sizeDelta = new Vector2(135, 75);

            RectTransform parentTransform = new GameObject("KeyboardParent").AddComponent<RectTransform>();
            parentTransform.SetParent(gameObject.transform, false);

            return new PrefabParams(prefab.RootObject, gameObject, false);
        }
    }
}
