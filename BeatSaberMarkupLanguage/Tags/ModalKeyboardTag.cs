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

            RectTransform windowTransform = gameObject.transform as RectTransform;
            windowTransform.name = "BSMLModalKeyboard";
            windowTransform.sizeDelta = new Vector2(135, 75);

            RectTransform parentTransform = new GameObject("KeyboardParent").AddComponent<RectTransform>();
            parentTransform.SetParent(gameObject.transform, false);

            KEYBOARD keyboard = new(parentTransform, KEYBOARD.QWERTY, true, 4, -12);
            parentTransform.localScale *= 1.4f;

            ModalKeyboard modalKeyboard = gameObject.AddComponent<ModalKeyboard>();
            modalKeyboard.keyboard = keyboard;
            modalKeyboard.modalView = gameObject.GetComponent<ModalView>();
            keyboard.EnterPressed += (value) => modalKeyboard.OnEnter(value);

            return gameObject;
        }
    }
}
