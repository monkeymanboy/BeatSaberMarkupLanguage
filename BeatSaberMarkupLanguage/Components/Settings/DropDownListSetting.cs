using System;
using System.Collections;
using System.Linq;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class DropDownListSetting : GenericInteractableSetting
    {
        public SimpleTextDropdown Dropdown;

        private int index;

        public IList Values { get; set; } = Array.Empty<object>();

        public object Value
        {
            get
            {
                return Values.Count > 0 ? Values[Mathf.Clamp(index, 0, Values.Count - 1)] : null;
            }

            set
            {
                index = Values.IndexOf(value);
                if (index < 0)
                {
                    index = 0;
                }

                Dropdown.SelectCellWithIdx(index);

                UpdateState();
            }
        }

        public override bool Interactable
        {
            get => Dropdown._button.interactable;
            set => Dropdown._button.interactable = value;
        }

        public override void Setup()
        {
            Dropdown.didSelectCellWithIdxEvent += OnSelectIndex;
            ReceiveValue();
            UpdateChoices();
            gameObject.SetActive(true);
        }

        public void UpdateChoices()
        {
            if (Formatter != null)
            {
                Dropdown.SetTexts(Values.Cast<object>().Select(o => Formatter.Invoke(o) as string).ToList());
            }
            else
            {
                Dropdown.SetTexts(Values.Cast<object>().Select(o => o.ToString()).ToList());
            }
        }

        public override void ApplyValue()
        {
            AssociatedValue?.SetValue(Value);
        }

        public override void ReceiveValue()
        {
            if (AssociatedValue != null)
            {
                Value = AssociatedValue.GetValue();
            }
        }

        private void OnSelectIndex(DropdownWithTableView tableView, int index)
        {
            this.index = index;
            UpdateState();
            OnChange?.Invoke(Value);

            if (UpdateOnChange)
            {
                ApplyValue();
            }
        }

        private void UpdateState()
        {
            Dropdown._text.text = Value != null ? (Formatter == null ? Value?.ToString() : (Formatter.Invoke(Value) as string)) : string.Empty;
        }
    }
}
