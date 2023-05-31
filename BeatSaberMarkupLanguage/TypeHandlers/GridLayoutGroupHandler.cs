using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(GridLayoutGroup))]
    public class GridLayoutGroupHandler : TypeHandler<GridLayoutGroup>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "cellSizeX", new[] { "cell-size-x" } },
            { "cellSizeY", new[] { "cell-size-y" } },
            { "spacingX", new[] { "spacing-x" } },
            { "spacingY", new[] { "spacing-y" } },
        };

        public override Dictionary<string, Action<GridLayoutGroup, string>> Setters => new Dictionary<string, Action<GridLayoutGroup, string>>()
        {
            { "cellSizeX", new Action<GridLayoutGroup, string>((component, value) => component.cellSize = new Vector2(Parse.Float(value), component.cellSize.y)) },
            { "cellSizeY", new Action<GridLayoutGroup, string>((component, value) => component.cellSize = new Vector2(component.cellSize.x, Parse.Float(value))) },
            { "spacingX", new Action<GridLayoutGroup, string>((component, value) => component.spacing = new Vector2(Parse.Float(value), component.spacing.y)) },
            { "spacingY", new Action<GridLayoutGroup, string>((component, value) => component.spacing = new Vector2(component.spacing.x, Parse.Float(value))) },
        };
    }
}
