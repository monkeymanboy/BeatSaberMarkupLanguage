using BeatSaberMarkupLanguage.Parser;
using HMUI;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace BeatSaberMarkupLanguage.Settings
{
    public class SettingsMenu : CustomCellInfo
    {
        public ViewController viewController;
        public BSMLParserParams parserParams;

        public SettingsMenu(string name, ViewController viewController, BSMLParserParams parserParams) : base(name)
        {
            this.viewController = viewController;
            this.parserParams = parserParams;
        }
    }
}
