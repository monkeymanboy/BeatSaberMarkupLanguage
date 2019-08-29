using BS_Utils.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public class KeyboardViewController : VRUIViewController
    {

        KEYBOARD keyboard;
        public Action<string> enterPressed;
        public string startingText;

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            base.DidActivate(firstActivation, activationType);

            if (firstActivation && activationType == ActivationType.AddedToHierarchy)
            {
                RectTransform keyboardRect = new GameObject("Keyboard").AddComponent<RectTransform>();
                keyboardRect.gameObject.transform.SetParent(rectTransform);
                keyboardRect.position = rectTransform.position;
                keyboardRect.localScale = rectTransform.localScale;
                keyboard = new KEYBOARD(keyboardRect, KEYBOARD.QWERTY);
                keyboard.EnterPressed += delegate (string value) { enterPressed?.Invoke(value); };
                keyboardRect.localScale *= 1.6f;
                keyboardRect.anchoredPosition = new Vector2(6, -10);
            }
            keyboard.KeyboardText.text = startingText;
            keyboard.DrawCursor();
            
        }

    }
    // Experimental chat console
    public class KEYBOARD
    {
        public List<KEY> keys = new List<KEY>();

        bool EnableInputField = true;
        bool Shift = false;
        bool Caps = false;
        RectTransform container;
        Vector2 currentposition;
        Vector2 baseposition;
        public event Action<string> EnterPressed;
        float scale = 0.5f; // BUG: Effect of changing this has NOT beed tested. assume changing it doesn't work.
        float padding = 0.5f;
        float buttonwidth = 12f;
        public TextMeshProUGUI KeyboardText;
        public TextMeshProUGUI KeyboardCursor;
        public Button BaseButton;


        KEY dummy = new KEY(); // This allows for some lazy programming, since unfound key searches will point to this instead of null. It still logs an error though

        // Keyboard spaces and CR/LF are significicant.
        // A slash following a space or CR/LF alters the width of the space
        // CR on an empty line results in a half life advance

        public const string QWERTY =
@"[CLEAR]/20
(`~) (1!) (2@) (3#) (4$) (5%) (6^) (7&) (8*) (9() (0)) (-_) (=+) [<--]/15
[TAB]/15'\t' (qQ) (wW) (eE) (rR) (tT) (yY) (uU) (iI) (oO) (pP) ([{) (]}) (\|)
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





        public KEY this[string index]
        {
            get
            {
                foreach (KEY key in keys) if (key.name == index) return key;

                return dummy;
            }

        }


        public void SetButtonType(string ButtonName = "KeyboardButton")
        {
            BaseButton = Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == ButtonName));
            if (BaseButton == null) BaseButton = Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "KeyboardButton"));
        }
        public void SetValue(string keylabel, string value)
        {
            bool found = false;
            foreach (KEY key in keys) if (key.name == keylabel)
                {
                    found = true;
                    key.value = value;
                    //key.shifted = value;
                }

        }

        public void SetAction(string keyname, Action<KEY> action)
        {
            bool found = false;
            foreach (KEY key in keys) if (key.name == keyname)
                {
                    found = true;
                    key.keyaction = action;
                }

        }


        KEY AddKey(string keylabel, float width = 12, float height = 10, int color = 0xffffff)
        {
            var position = currentposition;
            //position.x += width / 4;

            Color co = Color.white;

            co.r = (float)(color & 0xff) / 255;
            co.g = (float)((color >> 8) & 0xff) / 255;
            co.b = (float)((color >> 16) & 0xff) / 255;
            KEY key = new KEY(this, position, keylabel, width, height, co);
            keys.Add(key);
            //currentposition.x += width / 2 + padding;
            return key;
        }

        KEY AddKey(string keylabel, string Shifted, float width = 12, float height = 10)
        {
            KEY key = AddKey(keylabel, width);
            key.shifted = Shifted;
            return key;
        }


        // BUG: Refactor this within a keybard parser subclass once everything works.
        void EmitKey(ref float spacing, ref float Width, ref string Label, ref string Key, ref bool space, ref string newvalue, ref float height, ref int color)
        {
            currentposition.x += spacing;

            if (Label != "") AddKey(Label, Width, height, color).Set(newvalue);
            else if (Key != "") AddKey(Key[0].ToString(), Key[1].ToString()).Set(newvalue);
            spacing = 0;
            Width = buttonwidth;
            height = 10f;
            Label = "";
            Key = "";
            newvalue = "";
            color = 0xffffff;
            space = false;
            return;
        }

        bool ReadFloat(ref String data, ref int Position, ref float result)
        {
            if (Position >= data.Length) return false;
            int start = Position;
            while (Position < data.Length)
            {
                char c = data[Position];
                if (!((c >= '0' && c <= '9') || c == '+' || c == '-' || c == '.')) break;
                Position++;
            }


            if (float.TryParse(data.Substring(start, Position - start), out result)) return true;

            Position = start;
            return false;
        }


        // Very basic parser for the keyboard grammar - no doubt can be improved. Tricky to implement because of special characters.
        // It might possible to make grep do this, but it would be even harder to read than this!
        public KEYBOARD AddKeys(string Keyboard, float scale = 0.5f)
        {
            this.scale = scale;
            bool space = true;
            float spacing = padding;
            float width = buttonwidth;
            float height = 10f;
            string Label = "";
            string Key = "";
            string newvalue = "";
            int color = 0xffffff;
            int p = 0; // P is for parser

            try
            {

                while (p < Keyboard.Length)
                {

                    switch (Keyboard[p])
                    {
                        case '@': // Position key
                            EmitKey(ref spacing, ref width, ref Label, ref Key, ref space, ref newvalue, ref height, ref color);
                            p++;
                            if (ReadFloat(ref Keyboard, ref p, ref currentposition.x))
                            {
                                baseposition.x = currentposition.x;
                                if (p < Keyboard.Length && Keyboard[p] == ',')
                                {
                                    p++;
                                    ReadFloat(ref Keyboard, ref p, ref currentposition.y);
                                    baseposition.y = currentposition.y;
                                }
                            }
                            continue;

                        case 'S': // Scale
                            {
                                EmitKey(ref spacing, ref width, ref Label, ref Key, ref space, ref newvalue, ref height, ref color);
                                p++;
                                ReadFloat(ref Keyboard, ref p, ref this.scale);
                                continue;
                            }

                        case '\r':
                            space = true;
                            break;

                        case '\n':
                            EmitKey(ref spacing, ref width, ref Label, ref Key, ref space, ref newvalue, ref height, ref color);
                            space = true;
                            NextRow();
                            break;

                        case ' ':
                            space = true;
                            //spacing += padding;
                            break;

                        case '[':
                            EmitKey(ref spacing, ref width, ref Label, ref Key, ref space, ref newvalue, ref height, ref color);

                            space = false;
                            p++;
                            int label = p;
                            while (p < Keyboard.Length && Keyboard[p] != ']') p++;
                            Label = Keyboard.Substring(label, p - label);
                            break;

                        case '(':
                            EmitKey(ref spacing, ref width, ref Label, ref Key, ref space, ref newvalue, ref height, ref color);

                            p++;
                            Key = Keyboard.Substring(p, 2);
                            p += 2;
                            space = false;
                            break;

                        case '#':
                            // BUG: Make this support alpha and 6/8 digit forms
                            p++;
                            color = int.Parse(Keyboard.Substring(p, 6), System.Globalization.NumberStyles.HexNumber);
                            p += 6;
                            continue;

                        case '/':

                            p++;
                            float number = 0;
                            if (ReadFloat(ref Keyboard, ref p, ref number))
                            {
                                if (p < Keyboard.Length && Keyboard[p] == ',')
                                {
                                    p++;
                                    ReadFloat(ref Keyboard, ref p, ref height);
                                }

                                if (space)
                                {
                                    if (Label != "" || Key != "") EmitKey(ref spacing, ref width, ref Label, ref Key, ref space, ref newvalue, ref height, ref color);
                                    spacing = number;
                                }
                                else width = number;
                                continue;
                            }

                            break;

                        case '\'':
                            p++;
                            int newvaluep = p;
                            while (p < Keyboard.Length && Keyboard[p] != '\'') p++;
                            newvalue = Keyboard.Substring(newvaluep, p - newvaluep);
                            break;


                        default:
                            return this;
                    }

                    p++;
                }

                EmitKey(ref spacing, ref width, ref Label, ref Key, ref space, ref newvalue, ref height, ref color);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to parse keyboard at position {p} : [{Keyboard}]");
                Console.WriteLine(ex.ToString());
            }

            return this;
        }


        // Default actions may be called more than once. Make sure to only set any overrides that replace these AFTER all keys have been added
        public KEYBOARD DefaultActions()
        {
            SetAction("CLEAR", Clear);
            SetAction("ENTER", Enter);
            SetAction("<--", Backspace);
            SetAction("SHIFT", SHIFT);
            SetAction("CAPS", CAPS);
            return this;
        }

        public KEYBOARD(RectTransform container, string DefaultKeyboard = QWERTY, bool EnableInputField = true, float x = 0, float y = 0)
        {
            this.EnableInputField = EnableInputField;
            this.container = container;
            baseposition = new Vector2(-50 + x, 23 + y);
            currentposition = baseposition;
            //bool addhint = true;

            SetButtonType();

            // BUG: Make this an input field maybe

            KeyboardText = BeatSaberUI.CreateText(container, "", new Vector2(0, 23f));
            KeyboardText.fontSize = 6f;
            KeyboardText.color = Color.white;
            KeyboardText.alignment = TextAlignmentOptions.Center;
            KeyboardText.enableWordWrapping = false;
            KeyboardText.text = "";
            KeyboardText.enabled = this.EnableInputField;
            //KeyboardText



            KeyboardCursor = BeatSaberUI.CreateText(container, "|", new Vector2(0, 0));
            KeyboardCursor.fontSize = 6f;
            KeyboardCursor.color = Color.cyan;
            KeyboardCursor.alignment = TextAlignmentOptions.Left;
            KeyboardCursor.enableWordWrapping = false;
            KeyboardCursor.enabled = this.EnableInputField;

            DrawCursor(); // BUG: Doesn't handle trailing spaces.. seriously, wtf.

            // We protect this since setting nonexistent keys will throw.

            // BUG: These are here on a temporary basis, they will be moving out as soon as API is finished


            if (DefaultKeyboard != "")
            {
                AddKeys(DefaultKeyboard);
                DefaultActions();
            }


            return;
        }

        public KEYBOARD NextRow(float adjustx = 0)
        {
            currentposition.y -= currentposition.x == baseposition.x ? 3 : 6; // Next row on an empty row only results in a half row advance
            currentposition.x = baseposition.x;
            return this;
        }

        public KEYBOARD SetScale(float scale)
        {
            this.scale = scale;
            return this;
        }


        public void Clear(KEY key)
        {
            key.kb.KeyboardText.text = "";
        }

        public void Enter(KEY key)
        {
            var typedtext = key.kb.KeyboardText.text;
            EnterPressed?.Invoke(typedtext);
            key.kb.KeyboardText.text = "";
        }

        void Backspace(KEY key)
        {
            // BUG: This is terribly long winded... 
            if (key.kb.KeyboardText.text.Length > 0) key.kb.KeyboardText.text = key.kb.KeyboardText.text.Substring(0, key.kb.KeyboardText.text.Length - 1); // Is there a cleaner way to say this?
        }
        void SHIFT(KEY key)
        {
            key.kb.Shift = !key.kb.Shift;

            foreach (KEY k in key.kb.keys)
            {
                string x = key.kb.Shift ? k.shifted : k.value;
                //if (key.kb.Caps) x = k.value.ToUpper();
                if (k.shifted != "") k.mybutton.SetButtonText(x);

                if (k.name == "SHIFT") k.mybutton.GetComponentInChildren<Image>().color = key.kb.Shift ? Color.green : Color.white;
            }
        }

        void CAPS(KEY key)
        {
            key.kb.Caps = !key.kb.Caps;
            key.mybutton.GetComponentInChildren<Image>().color = key.kb.Caps ? Color.green : Color.white;
        }

        public void DrawCursor()
        {
            if (!EnableInputField) return;

            Vector2 v = KeyboardText.GetPreferredValues(KeyboardText.text + "|");

            v.y = 23f; // BUG: This needs to be derived from the text position
                       // BUG: I do not know why that 30f is here, It makes things work, but I can't understand WHY! Me stupid.
            v.x = v.x / 2 + 30f - 0.5f; // BUG: The .5 gets rid of the trailing |, but technically, we need to calculate its width and store it
            (KeyboardCursor.transform as RectTransform).anchoredPosition = v;
        }



        public class KEY
        {
            public string name = "";
            public string value = "";
            public string shifted = "";
            public Button mybutton;
            public KEYBOARD kb;
            public Action<KEY> keyaction = null;

            public KEY Set(string Value)
            {
                if (Value != "")
                {
                    this.value = Value;
                    //this.shifted = Value;
                }
                return this;
            }

            public KEY()
            {
                // This key is not intialized at all
            }

            public KEY(KEYBOARD kb, Vector2 position, string text, float width, float height, Color color)
            {
                value = text;
                this.kb = kb;

                name = text;
                mybutton = Button.Instantiate(kb.BaseButton, kb.container, false);

                (mybutton.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                (mybutton.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);

                TMP_Text txt = mybutton.GetComponentInChildren<TMP_Text>();
                mybutton.ToggleWordWrapping(false);

                mybutton.transform.localScale = new Vector3(kb.scale, kb.scale, 1.0f);
                mybutton.SetButtonTextSize(5f);
                mybutton.SetButtonText(text);
                mybutton.GetComponentInChildren<Image>().color = color;

                if (width == 0)
                {
                    Vector2 v = txt.GetPreferredValues(text);
                    v.x += 10f;
                    v.y += 2f;
                    width = v.x;
                }

                // Adjust starting position so button aligns to upper left of current drawing position

                position.x += kb.scale * width / 2;
                position.y -= kb.scale * height / 2;
                (mybutton.transform as RectTransform).anchoredPosition = position;

                (mybutton.transform as RectTransform).sizeDelta = new Vector2(width, height);

                kb.currentposition.x += width * kb.scale + kb.padding;

                mybutton.onClick.RemoveAllListeners();

                mybutton.onClick.AddListener(delegate ()
                {

                    if (keyaction != null)
                    {
                        keyaction(this);
                        kb.DrawCursor();
                        return;
                    }

                    if (value.EndsWith("%CR%"))
                    {
                        kb.KeyboardText.text += value.Substring(0, value.Length - 4);
                        kb.Enter(this);
                        kb.DrawCursor();

                        return;
                    }

                    {
                        string x = kb.Shift ? shifted : value;
                        if (x == "") x = value;
                        if (kb.Caps) x = value.ToUpper();
                        kb.KeyboardText.text += x;
                        kb.DrawCursor();

                    }
                });
                HoverHint _MyHintText = mybutton.gameObject.AddComponent<HoverHint>();
                _MyHintText.text = value;
                _MyHintText.SetPrivateField("_hoverHintController", Resources.FindObjectsOfTypeAll<HoverHintController>().First());
            }
        }
    }
}

