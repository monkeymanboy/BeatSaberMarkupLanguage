using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.MenuButtons
{
    [ViewDefinition("BeatSaberMarkupLanguage.Views.main-left-screen.bsml")]
    internal class MenuButtonsViewController : BSMLAutomaticViewController
    {
        [UIValue("buttons")]
        public List<object> buttons = new List<object>();

        [UIValue("any-buttons")]
        private bool AnyButtons => buttons.Count > 0;
    }
}
