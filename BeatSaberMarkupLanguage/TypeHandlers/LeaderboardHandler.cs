using BeatSaberMarkupLanguage.Parser;
using BS_Utils.Utilities;
using System.Collections.Generic;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(LeaderboardTableView))]
    public class LeaderboardHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "cellSize", new[]{"cell-size"} }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            LeaderboardTableView table = (componentType.component as LeaderboardTableView);
            if (componentType.data.TryGetValue("cellSize", out string cellSize))
                table.SetPrivateField("_rowHeight", Parse.Float(cellSize));
        }
    }
}
