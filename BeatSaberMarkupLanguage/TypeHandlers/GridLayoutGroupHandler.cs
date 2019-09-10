using BeatSaberMarkupLanguage.Parser;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(GridLayoutGroup))]
    public class GridLayoutGroupHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "cellSizeX", new[]{ "cell-size-x" } },
            { "cellSizeY", new[]{ "cell-size-y"} },
            { "spacingX", new[]{ "spacing-x"} },
            { "spacingY", new[]{ "spacing-y"} }
        };

        public override void HandleType(Component obj, Dictionary<string, string> data, BSMLParserParams parserParams)
        {
            GridLayoutGroup gridLayout = obj as GridLayoutGroup;
            gridLayout.cellSize = new Vector2(data.ContainsKey("cellSizeX") ? Parse.Float(data["cellSizeX"]) : gridLayout.cellSize.x, data.ContainsKey("cellSizeY") ? Parse.Float(data["cellSizeY"]) : gridLayout.cellSize.y);
            gridLayout.spacing = new Vector2(data.ContainsKey("spacingX") ? Parse.Float(data["spacingX"]) : gridLayout.spacing.x, data.ContainsKey("spacingY") ? Parse.Float(data["spacingY"]) : gridLayout.spacing.y);
        }
    }
}
