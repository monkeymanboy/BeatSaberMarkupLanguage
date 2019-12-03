using BS_Utils.Utilities;
using System;
using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(LeaderboardTableView))]
    public class LeaderboardHandler : TypeHandler<LeaderboardTableView>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "cellSize", new[]{"cell-size"} }
        };

        public override Dictionary<string, Action<LeaderboardTableView, string>> Setters => new Dictionary<string, Action<LeaderboardTableView, string>>()
        {
            {"cellSize", new Action<LeaderboardTableView, string>((component, value) => component.SetPrivateField("_rowHeight", Parse.Float(value))) }
        };
    }
}
