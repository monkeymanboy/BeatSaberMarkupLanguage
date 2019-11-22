using BeatSaberMarkupLanguage.Parser;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.BSMLParser;

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

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            GridLayoutGroup gridLayout = componentType.component as GridLayoutGroup;
            gridLayout.cellSize = new Vector2(componentType.data.TryGetValue("cellSizeX", out string cellSizeX) ? Parse.Float(cellSizeX) : gridLayout.cellSize.x, componentType.data.TryGetValue("cellSizeY", out string cellSizeY) ? Parse.Float(cellSizeY) : gridLayout.cellSize.y);
            gridLayout.spacing = new Vector2(componentType.data.TryGetValue("spacingX", out string spacingX) ? Parse.Float(spacingX) : gridLayout.spacing.x, componentType.data.TryGetValue("spacingY", out string spacingY) ? Parse.Float(spacingY) : gridLayout.spacing.y);
        }
    }
}
