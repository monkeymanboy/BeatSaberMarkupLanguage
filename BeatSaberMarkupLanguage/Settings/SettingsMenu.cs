using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUI;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage.Settings
{
    public class SettingsMenu : CustomCellInfo
    {
        public VRUIViewController viewController;
        public BSMLParserParams parserParams;

        public SettingsMenu(string name, VRUIViewController viewController, BSMLParserParams parserParams) : base(name)
        {
            this.viewController = viewController;
            this.parserParams = parserParams;
        }
    }
}
