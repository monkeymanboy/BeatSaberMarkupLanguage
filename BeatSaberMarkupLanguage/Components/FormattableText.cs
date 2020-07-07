using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace BeatSaberMarkupLanguage.Components
{
    public class FormattableText : TextMeshProUGUI
    {
        private string textFormat;
        private ICustomFormatter textFormatter;
        private object data;
        public object Data
        {
            get => data;
            set
            {
                data = value;
                RefreshText();
            }
        }

        public void RefreshText()
        {
            if (data == null)
                return;
            string val;
            
            object o = data;
            if (TextFormatter != null)
                val = TextFormatter.Format(TextFormat, o, null);
            else if (o is IFormattable formattable && !string.IsNullOrEmpty(TextFormat))
                val = formattable.ToString(TextFormat, null); // TODO: Will this cause problems for certain types if formatProvider is null?
            else
            {
                val = o?.ToString() ?? "";
            }
            text = val;
        }

        public ICustomFormatter TextFormatter
        {
            get => textFormatter;
            set
            {
                if (textFormatter == value)
                    return;
                textFormatter = value;
                RefreshText();
            }
        }

        public string TextFormat
        {
            get => textFormat;
            set
            {
                if (textFormat == value)
                    return;
                textFormat = value;
                RefreshText();
            }
        }

        public void SetFormatter(object formatter)
        {
            if (formatter == null)
            {
                TextFormatter = null;
                return;
            }
            if (formatter is ICustomFormatter valueConverter)
            {
                TextFormatter = valueConverter;
            }
            else
            {
                throw new ArgumentException("formatter must by of type 'ICustomFormatter'", nameof(formatter));
            }
        }

        protected override void OnDestroy()
        {
            EventHandler handler = Destroyed;
            if (handler != null)
                handler.Invoke(this, null);
            base.OnDestroy();
        }

        public event EventHandler Destroyed;
    }
}
