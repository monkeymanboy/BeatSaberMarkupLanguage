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
            gridLayout.cellSize = new Vector2(data.TryGetValue("cellSizeX", out string cellSizeX) ? Parse.Float(cellSizeX) : gridLayout.cellSize.x, data.TryGetValue("cellSizeY", out string cellSizeY) ? Parse.Float(cellSizeY) : gridLayout.cellSize.y);
            gridLayout.spacing = new Vector2(data.TryGetValue("spacingX", out string spacingX) ? Parse.Float(spacingX) : gridLayout.spacing.x, data.TryGetValue("spacingY", out string spacingY) ? Parse.Float(spacingY) : gridLayout.spacing.y);
        }
    }
}
