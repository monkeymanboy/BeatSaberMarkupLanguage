using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Parser;
using BGLib.Polyglot;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BeatSaberMarkupLanguage.Components
{
    public class ModalKeyboard : MonoBehaviour
    {
        public ModalView modalView;
        public KEYBOARD keyboard;

        public BSMLValue associatedValue;
        public BSMLAction onEnter;
        public bool clearOnOpen;

        public void OnEnter(string value)
        {
            associatedValue?.SetValue(value);
            onEnter?.Invoke(value);
            modalView.Hide(true);
        }

        public void SetText(string text)
        {
            keyboard.KeyboardText.text = text;
        }

        internal void ReceiveValue()
        {
            if (associatedValue != null)
            {
                SetText(associatedValue.GetValue() as string);
            }

            if (clearOnOpen)
            {
                SetText(string.Empty);
            }
        }

        private void OnEnable()
        {
            ReceiveValue();
        }
    }

    // Experimental chat console
    public class KEYBOARD
    {
        // Keyboard spaces and CR/LF are significant.
        // A slash following a space or CR/LF alters the width of the space
        // CR on an empty line results in a half life advance
        public const string QWERTY =
@"[CLEAR]/20
(`~) (1!) (2@) (3#) (4$) (5%) (6^) (7&) (8*) (9() (0)) (-_) (=+) [<--]/15
[TAB]/15'	' (qQ) (wW) (eE) (rR) (tT) (yY) (uU) (iI) (oO) (pP) ([{) (]}) (\|)
[CAPS]/20 (aA) (sS) (dD) (fF) (gG) (hH) (jJ) (kK) (lL) (;:) ('"") [ENTER]/20,22#20A0D0
[SHIFT]/25 (zZ) (xX) (cC) (vV) (bB) (nN) (mM) (,<) (.>) (/?)
/23 (!!) (@@) [SPACE]/40' ' (##) (__)";

        public const string FKEYROW =
@"
[Esc] /2 [F1] [F2] [F3] [F4] /2 [F5] [F6] [F7] [F8] /2 [F9] [F10] [F11] [F12]
";

        public const string NUMPAD =
@"
[NUM] (//) (**) (--)
(77) (88) (99) [+]/10,22
(44) (55) (66)
(11) (22) (33) [ENTER]/10,22
[0]/22 [.]
";

        public const string DVORAK =
@"
(`~) (1!) (2@) (3#) (4$) (5%) (6^) (7&) (8*) (9() (0)) ([{) (]}) [<--]/15
[TAB]/15 ('"") (,<) (.>) (pP) (yY) (fF) (gG) (cC) (rR) (lL) (/?) (=+) (\|)
[CAPS]/20 (aA) (oO) (eE) (uU) (iI) (dD) (hH) (tT) (nN) (sS) (-_) [ENTER]/20
[SHIFT] (;:) (qQ) (jJ) (kK) (xX) (bB) (mM) (wW) (vV) (zZ) [CLEAR]/28
/23 (!!) (@@) [SPACE]/40 (##) (__)";

        public List<KEY> keys = new();

        public TextMeshProUGUI KeyboardText;
        public TextMeshProUGUI KeyboardCursor;
        public Button BaseButton;

        private readonly KEY dummy = new(); // This allows for some lazy programming, since unfound key searches will point to this instead of null. It still logs an error though
        private readonly RectTransform keyboardTextContainer;

        private readonly RectTransform container;
        private readonly bool enableInputField = true;
        private readonly float padding = 0.5f;
        private readonly float buttonWidth = 12f;
        private bool shift = false;
        private bool caps = false;
        private Vector2 currentPosition;
        private Vector2 basePosition;
        private float scale = 0.5f; // BUG: Effect of changing this has NOT been tested. assume changing it doesn't work.

        public KEYBOARD(RectTransform container, string defaultKeyboard = QWERTY, bool enableInputField = true, float x = 0, float y = 0)
        {
            this.enableInputField = enableInputField;

            GameObject containerObject = new("Keys", typeof(RectTransform))
            {
                layer = 5,
            };

            containerObject.SetActive(false);

            this.container = (RectTransform)containerObject.transform;
            this.container.SetParent(container, false);

            basePosition = new Vector2(-50 + x, 23 + y);
            currentPosition = basePosition;

            SetButtonType();

            GameObject textContainer = new("KeyboardText", typeof(RectTransform), typeof(RectMask2D))
            {
                layer = 5,
            };

            keyboardTextContainer = (RectTransform)textContainer.transform;
            keyboardTextContainer.SetParent(container, false);
            keyboardTextContainer.anchoredPosition = new Vector2(0, 15f);
            keyboardTextContainer.sizeDelta = new Vector2(88, 10);

            HorizontalLayoutGroup layoutGroup = textContainer.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;

            // BUG: Make this an input field maybe
            KeyboardText = BeatSaberUI.CreateText<WhitespaceIncludingCurvedTextMeshPro>(keyboardTextContainer, string.Empty, new Vector2(0, 0));
            KeyboardText.name = "Text";
            KeyboardText.fontSize = 6f;
            KeyboardText.overflowMode = TextOverflowModes.Masking;
            KeyboardText.color = Color.white;
            KeyboardText.alignment = TextAlignmentOptions.BaselineRight;
            KeyboardText.enableWordWrapping = false;
            KeyboardText.text = string.Empty;
            KeyboardText.enabled = this.enableInputField;

            KeyboardCursor = BeatSaberUI.CreateText(keyboardTextContainer, "|", new Vector2(0, 0));
            KeyboardCursor.name = "Cursor";
            KeyboardCursor.fontSize = 6f;
            KeyboardCursor.color = new Color(0.60f, 0.80f, 1);
            KeyboardCursor.alignment = TextAlignmentOptions.Baseline;
            KeyboardCursor.enableWordWrapping = false;
            KeyboardCursor.enabled = this.enableInputField;

            // We protect this since setting nonexistent keys will throw.

            // BUG: These are here on a temporary basis, they will be moving out as soon as API is finished
            if (!string.IsNullOrEmpty(defaultKeyboard))
            {
                AddKeys(defaultKeyboard);
                DefaultActions();
            }

            return;
        }

        public event Action<string> EnterPressed;

        public KEY this[string index]
        {
            get
            {
                foreach (KEY key in keys)
                {
                    if (key.name == index)
                    {
                        return key;
                    }
                }

                return dummy;
            }
        }

        public void SetButtonType(string buttonName = "Q")
        {
            if (BaseButton == null)
            {
                BaseButton = BeatSaberUI.DiContainer.Resolve<UIKeyboardManager>().transform.Find("KeyboardWrapper/Keyboard/Letters/Row/Q").GetComponent<Button>();
            }
        }

        public void SetValue(string keylabel, string value)
        {
            foreach (KEY key in keys)
            {
                if (key.name == keylabel)
                {
                    key.value = value;
                }
            }
        }

        public void SetAction(string keyname, Action<KEY> action)
        {
            foreach (KEY key in keys)
            {
                if (key.name == keyname)
                {
                    key.keyaction = action;
                }
            }
        }

        // Very basic parser for the keyboard grammar - no doubt can be improved. Tricky to implement because of special characters.
        // It might possible to make grep do this, but it would be even harder to read than this!
        public KEYBOARD AddKeys(string keyboard, float scale = 0.5f)
        {
            this.scale = scale;
            bool space = true;
            float spacing = padding;
            float width = buttonWidth;
            float height = 10f;
            string label = string.Empty;
            string key = string.Empty;
            string newvalue = string.Empty;
            int color = 0xffffff;
            int p = 0; // P is for parser

            try
            {
                while (p < keyboard.Length)
                {
                    switch (keyboard[p])
                    {
                        case '@': // Position key
                            EmitKey(ref spacing, ref width, ref label, ref key, ref space, ref newvalue, ref height, ref color);
                            p++;
                            if (ReadFloat(ref keyboard, ref p, ref currentPosition.x))
                            {
                                basePosition.x = currentPosition.x;
                                if (p < keyboard.Length && keyboard[p] == ',')
                                {
                                    p++;
                                    ReadFloat(ref keyboard, ref p, ref currentPosition.y);
                                    basePosition.y = currentPosition.y;
                                }
                            }

                            continue;
                        case 'S': // Scale
                            EmitKey(ref spacing, ref width, ref label, ref key, ref space, ref newvalue, ref height, ref color);
                            p++;
                            ReadFloat(ref keyboard, ref p, ref this.scale);

                            continue;
                        case '\r':
                            space = true;

                            break;
                        case '\n':
                            EmitKey(ref spacing, ref width, ref label, ref key, ref space, ref newvalue, ref height, ref color);
                            space = true;
                            NextRow();

                            break;
                        case ' ':
                            space = true;

                            break;
                        case '[':
                            EmitKey(ref spacing, ref width, ref label, ref key, ref space, ref newvalue, ref height, ref color);
                            space = false;
                            p++;
                            int labelIndex = p;

                            while (p < keyboard.Length && keyboard[p] != ']')
                            {
                                p++;
                            }

                            label = keyboard.Substring(labelIndex, p - labelIndex);
                            label = label switch
                            {
                                "<--" => "\u2B05",
                                _ => label,
                            };

                            break;
                        case '(':
                            EmitKey(ref spacing, ref width, ref label, ref key, ref space, ref newvalue, ref height, ref color);
                            p++;
                            key = keyboard.Substring(p, 2);
                            p += 2;
                            space = false;

                            break;
                        case '#':
                            // BUG: Make this support alpha and 6/8 digit forms
                            p++;
                            color = int.Parse(keyboard.Substring(p, 6), System.Globalization.NumberStyles.HexNumber);
                            p += 6;

                            continue;
                        case '/':
                            p++;
                            float number = 0;
                            if (ReadFloat(ref keyboard, ref p, ref number))
                            {
                                if (p < keyboard.Length && keyboard[p] == ',')
                                {
                                    p++;
                                    ReadFloat(ref keyboard, ref p, ref height);
                                }

                                if (space)
                                {
                                    if (!string.IsNullOrEmpty(label) || !string.IsNullOrEmpty(key))
                                    {
                                        EmitKey(ref spacing, ref width, ref label, ref key, ref space, ref newvalue, ref height, ref color);
                                    }

                                    spacing = number;
                                }
                                else
                                {
                                    width = number;
                                }

                                continue;
                            }

                            break;
                        case '\'':
                            p++;
                            int newvaluep = p;
                            while (p < keyboard.Length && keyboard[p] != '\'')
                            {
                                p++;
                            }

                            newvalue = keyboard.Substring(newvaluep, p - newvaluep);

                            break;
                        default:
                            return this;
                    }

                    p++;
                }

                EmitKey(ref spacing, ref width, ref label, ref key, ref space, ref newvalue, ref height, ref color);

                this.container.gameObject.SetActive(true);
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Unable to parse keyboard at position {p} : [{keyboard}]");
                Logger.Log.Error(ex);
            }

            return this;
        }

        // Default actions may be called more than once. Make sure to only set any overrides that replace these AFTER all keys have been added
        public KEYBOARD DefaultActions()
        {
            SetAction("CLEAR", Clear);
            SetAction("ENTER", Enter);
            SetAction("\u2B05", Backspace);
            SetAction("SHIFT", SHIFT);
            SetAction("CAPS", CAPS);

            return this;
        }

        public KEYBOARD NextRow(float adjustx = 0)
        {
            currentPosition.y -= currentPosition.x == basePosition.x ? 3 : 6; // Next row on an empty row only results in a half row advance
            currentPosition.x = basePosition.x;
            return this;
        }

        public KEYBOARD SetScale(float scale)
        {
            this.scale = scale;
            return this;
        }

        public void Clear(KEY key)
        {
            key.kb.KeyboardText.text = string.Empty;
        }

        public void Enter(KEY key)
        {
            string typedtext = key.kb.KeyboardText.text;
            EnterPressed?.Invoke(typedtext);
            key.kb.KeyboardText.text = string.Empty;
        }

        [Obsolete("Calling this method is no longer necessary. The cursor is redrawn by Unity UI directly.")]
        public void DrawCursor()
        {
        }

        private KEY AddKey(string keylabel, float width = 12, float height = 10, int color = 0xffffff)
        {
            Vector2 position = currentPosition;

            Color co = Color.white;
            co.r = (float)(color & 0xff) / 255;
            co.g = (float)((color >> 8) & 0xff) / 255;
            co.b = (float)((color >> 16) & 0xff) / 255;

            KEY key = new(this, position, keylabel, width, height, co);
            keys.Add(key);

            return key;
        }

        private KEY AddKey(string keylabel, string shifted, float width = 12, float height = 10)
        {
            KEY key = AddKey(keylabel, width, height);
            key.shifted = shifted;
            return key;
        }

        // BUG: Refactor this within a keyboard parser subclass once everything works.
        private void EmitKey(ref float spacing, ref float width, ref string label, ref string key, ref bool space, ref string newvalue, ref float height, ref int color)
        {
            currentPosition.x += spacing;

            if (!string.IsNullOrEmpty(label))
            {
                AddKey(label, width, height, color).Set(newvalue);
            }
            else if (!string.IsNullOrEmpty(key))
            {
                AddKey(key[0].ToString(), key[1].ToString()).Set(newvalue);
            }

            spacing = 0;
            width = buttonWidth;
            height = 10f;
            label = string.Empty;
            key = string.Empty;
            newvalue = string.Empty;
            color = 0xffffff;
            space = false;

            return;
        }

        private bool ReadFloat(ref string data, ref int position, ref float result)
        {
            if (position >= data.Length)
            {
                return false;
            }

            int start = position;
            while (position < data.Length)
            {
                char c = data[position];
                if (c is not (>= '0' and <= '9' or '+' or '-' or '.'))
                {
                    break;
                }

                position++;
            }

            if (float.TryParse(data.Substring(start, position - start), out result))
            {
                return true;
            }

            position = start;
            return false;
        }

        private void Backspace(KEY key)
        {
            int length = key.kb.KeyboardText.text.Length;
            if (length > 0)
            {
                key.kb.KeyboardText.text = key.kb.KeyboardText.text.Remove(length - 1);
            }
        }

        private void SHIFT(KEY key) => SHIFT(key, !key.kb.shift);

        private void SHIFT(KEY key, bool state)
        {
            key.kb.shift = state;
            UpdateKeyText(key.kb);
        }

        private void CAPS(KEY key)
        {
            key.kb.caps = !key.kb.caps;
            UpdateKeyText(key.kb);
        }

        private void UpdateKeyText(KEYBOARD keyboard)
        {
            foreach (KEY key in keyboard.keys)
            {
                key.UpdateText();

                if (key.name == "SHIFT")
                {
                    key.SetHighlighted(keyboard.shift);
                }

                if (key.name == "CAPS")
                {
                    key.SetHighlighted(keyboard.caps);
                }
            }
        }

        public class KEY
        {
            public string name = string.Empty;
            public string value = string.Empty;
            public string shifted = string.Empty;
            public Button mybutton;
            public KEYBOARD kb;
            public Action<KEY> keyaction = null;

            private readonly Graphic[] graphicsToColor;
            private readonly Color[] defaultColors;
            private readonly Color[] highlightedColors;
            private readonly TMP_Text buttonText;

            public KEY()
            {
            }

            public KEY(KEYBOARD kb, Vector2 position, string text, float width, float height, Color color)
            {
                name = text;
                value = text;
                this.kb = kb;

                mybutton = Object.Instantiate(kb.BaseButton, kb.container, false);
                mybutton.name = name;
                Object.DestroyImmediate(mybutton.GetComponent<UIKeyboardKey>());

                graphicsToColor = mybutton.GetComponentsInChildren<Graphic>();
                defaultColors = new Color[graphicsToColor.Length];
                highlightedColors = new Color[graphicsToColor.Length];

                // TODO: passed color isn't taken into account because of this, figure out if there's a nicer way to deal with this
                for (int i = 0; i < graphicsToColor.Length; i++)
                {
                    Color graphicColor = graphicsToColor[i].color;
                    defaultColors[i] = graphicColor;
                    highlightedColors[i] = new Color(0.1f, 1, 0.1f, graphicColor.a);
                }

                LocalizedTextMeshProUGUI localizer = mybutton.GetComponentInChildren<LocalizedTextMeshProUGUI>(true);
                if (localizer != null)
                {
                    Object.Destroy(localizer);
                }

                buttonText = mybutton.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.richText = true;
                buttonText.enableWordWrapping = false;
                buttonText.fontSize = 5f;
                buttonText.text = text;

                ExternalComponents externalComponents = mybutton.gameObject.AddComponent<ExternalComponents>();
                externalComponents.components.Add(buttonText);

                RectTransform buttonTransform = (RectTransform)mybutton.transform;
                buttonTransform.anchorMin = new Vector2(0.5f, 0.5f);
                buttonTransform.anchorMax = new Vector2(0.5f, 0.5f);
                buttonTransform.localScale = new Vector3(kb.scale, kb.scale, 1.0f);

                if (width == 0)
                {
                    Vector2 v = buttonText.GetPreferredValues(text);
                    v.x += 10f;
                    v.y += 2f;
                    width = v.x;
                }

                // Adjust starting position so button aligns to upper left of current drawing position
                position.x += kb.scale * width / 2;
                position.y -= kb.scale * height / 2;
                (mybutton.transform as RectTransform).anchoredPosition = position;

                (mybutton.transform as RectTransform).sizeDelta = new Vector2(width, height);

                kb.currentPosition.x += (width * kb.scale) + kb.padding;

                mybutton.onClick.RemoveAllListeners();

                mybutton.onClick.AddListener(() =>
                {
                    if (keyaction != null)
                    {
                        keyaction(this);
                        return;
                    }

                    if (value.EndsWith("%CR%", StringComparison.Ordinal))
                    {
                        kb.KeyboardText.text += value.Substring(0, value.Length - 4);
                        kb.Enter(this);
                        return;
                    }

                    kb.KeyboardText.text += TextForCurrentState;
                    kb.SHIFT(this, false);
                });

                HoverHint myHintText = BeatSaberUI.DiContainer.InstantiateComponent<HoverHint>(mybutton.gameObject);
                myHintText.text = value switch
                {
                    "\u2B05" => "Backspace",
                    _ => value,
                };
            }

            internal string TextForCurrentState
            {
                get
                {
                    if (value.ToUpper() != value)
                    {
                        return kb.shift ^ kb.caps ? value.ToUpper() : value;
                    }
                    else
                    {
                        return kb.shift ? shifted : value;
                    }
                }
            }

            public KEY Set(string value)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.value = value;
                }

                return this;
            }

            internal void SetHighlighted(bool highlighted)
            {
                for (int i = 0; i < graphicsToColor.Length; i++)
                {
                    graphicsToColor[i].color = highlighted ? highlightedColors[i] : defaultColors[i];
                }
            }

            internal void UpdateText()
            {
                if (string.IsNullOrEmpty(shifted))
                {
                    buttonText.text = name;
                }
                else
                {
                    buttonText.text = TextForCurrentState;
                }
            }
        }
    }
}
