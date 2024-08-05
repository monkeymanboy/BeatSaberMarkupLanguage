using System;
using System.Collections;
using System.Linq;
using HMUI;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Components.Settings
{
    public class DropDownListSetting : GenericInteractableSetting
    {
        public SimpleTextDropdown dropdown;

        private int index;

        public IList values { get; set; } = Array.Empty<object>();

        public object Value
        {
            get
            {
                return values.Count > 0 ? values[Mathf.Clamp(index, 0, values.Count - 1)] : null;
            }

            set
            {
                index = values.IndexOf(value);
                if (index < 0)
                {
                    index = 0;
                }

                dropdown.SelectCellWithIdx(index);

                UpdateState();
            }
        }

        public override bool interactable
        {
            get => dropdown._button.interactable;
            set => dropdown._button.interactable = value;
        }

        public override void Setup()
        {
            dropdown.didSelectCellWithIdxEvent += OnSelectIndex;
            ReceiveValue();
            UpdateChoices();
            gameObject.SetActive(true);
        }

        public void UpdateChoices()
        {
            if (formatter != null)
            {
                dropdown.SetTexts(values.Cast<object>().Select(o => formatter.Invoke(o) as string).ToList());
            }
            else
            {
                dropdown.SetTexts(values.Cast<object>().Select(o => o.ToString()).ToList());
            }
        }

        public override void ApplyValue()
        {
            associatedValue?.SetValue(Value);
        }

        public override void ReceiveValue()
        {
            if (associatedValue != null)
            {
                Value = associatedValue.GetValue();
            }
        }

        private void OnSelectIndex(DropdownWithTableView tableView, int index)
        {
            this.index = index;
            UpdateState();
            onChange?.Invoke(Value);

            if (updateOnChange)
            {
                ApplyValue();
            }
        }

        private void UpdateState()
        {
            dropdown._text.text = Value != null ? (formatter == null ? Value?.ToString() : (formatter.Invoke(Value) as string)) : string.Empty;
        }
    }
}
