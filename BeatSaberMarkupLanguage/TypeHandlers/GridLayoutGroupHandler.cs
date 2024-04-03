using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(GridLayoutGroup))]
    public class GridLayoutGroupHandler : TypeHandler<GridLayoutGroup>
    {
        public override Dictionary<string, string[]> Props => new()
        {
            { "cellSize", new[] { "cell-size" } },
            { "cellSizeX", new[] { "cell-size-x" } },
            { "cellSizeY", new[] { "cell-size-y" } },
            { "spacing", new[] { "spacing" } },
            { "spacingX", new[] { "spacing-x" } },
            { "spacingY", new[] { "spacing-y" } },
        };

        public override Dictionary<string, Action<GridLayoutGroup, string>> Setters => new()
        {
            { "cellSize", new Action<GridLayoutGroup, string>((component, value) => component.cellSize = Parse.Vector2(value)) },
            { "cellSizeX", new Action<GridLayoutGroup, string>((component, value) => component.cellSize = new Vector2(Parse.Float(value), component.cellSize.y)) },
            { "cellSizeY", new Action<GridLayoutGroup, string>((component, value) => component.cellSize = new Vector2(component.cellSize.x, Parse.Float(value))) },
            { "spacing", new Action<GridLayoutGroup, string>((component, value) => component.spacing = Parse.Vector2(value)) },
            { "spacingX", new Action<GridLayoutGroup, string>((component, value) => component.spacing = new Vector2(Parse.Float(value), component.spacing.y)) },
            { "spacingY", new Action<GridLayoutGroup, string>((component, value) => component.spacing = new Vector2(component.spacing.x, Parse.Float(value))) },
        };
    }
}
