using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUI;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public abstract class BSMLViewController : VRUIViewController
    {
        public abstract string Content
        {
            get;
        }
        public Action<bool, ActivationType> didActivate;
        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            if (firstActivation)
            {
                BSMLParser.instance.Parse(Content, gameObject);
            }
            didActivate?.Invoke(firstActivation, type);
        }
    }
}
