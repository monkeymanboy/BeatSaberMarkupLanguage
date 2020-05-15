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
            ExternalComponents externalComponents = gameObject.GetComponent<ExternalComponents>();

            RectTransform windowTransform = externalComponents.Get<RectTransform>();
            windowTransform.name = "BSMLModalKeyboard";
            windowTransform.sizeDelta = new Vector2(135, 75);

            RectTransform parentTransform = new GameObject("KeyboardParent").AddComponent<RectTransform>();
            parentTransform.SetParent(gameObject.transform, false);

            KEYBOARD keyboard = new KEYBOARD(parentTransform, KEYBOARD.QWERTY, true, 4, -12);
            parentTransform.localScale *= 1.4f;

            ModalKeyboard modalKeyboard = gameObject.AddComponent<ModalKeyboard>();
            modalKeyboard.keyboard = keyboard;
            modalKeyboard.modalView = externalComponents.Get<ModalView>();
            keyboard.EnterPressed += delegate (string value) { modalKeyboard.OnEnter(value); };

            return gameObject;
        }
    }
}
