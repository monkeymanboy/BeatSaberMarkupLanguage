using System;
using System.Xml;
using System.Xml.Linq;

namespace BeatSaberMarkupLanguage
{
    public class BSMLParserException : BSMLException
    {
        internal BSMLParserException(XElement element)
            : base($"Failed to parse element {element.Name}")
        {
            Element = element;
        }

        internal BSMLParserException(XElement element, Exception innerException)
            : base($"Failed to parse element {element.Name}", innerException)
        {
            Element = element;
        }

        public XElement Element { get; }

        public override string Message
        {
            get
            {
                if (Element is IXmlLineInfo xmlLineInfo && xmlLineInfo.HasLineInfo())
                {
                    return $"{base.Message} at line {xmlLineInfo.LineNumber}, position {xmlLineInfo.LinePosition}";
                }
                else
                {
                    return base.Message;
                }
            }
        }
    }
}
