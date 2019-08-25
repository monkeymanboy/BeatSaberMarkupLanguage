using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(LeaderboardTableView))]
    public class LeaderboardHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "cellSize", new[]{"cell-size"} }
        };
        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            LeaderboardTableView table = (obj as LeaderboardTableView);
            if (data.ContainsKey("cellSize"))
            {
                table.SetPrivateField("_rowHeight", float.Parse(data["cellSize"]));
            }
        }
    }
}
