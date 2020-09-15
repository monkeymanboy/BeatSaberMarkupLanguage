using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers.Settings
{
    [ComponentHandler(typeof(SliderSetting))]
    public class SliderSettingHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "increment", new[] { "increment" } },
            { "minValue", new[] { "min" } },
            { "maxValue", new[] { "max" } },
            { "isInt", new[] { "integer-only" } },
            { "updateDuringDrag", Array.Empty<string>() },
            { "onDragStarted", Array.Empty<string>() },
            { "dragStartedEvent", Array.Empty<string>() },
            { "onDragReleased", Array.Empty<string>() },
            { "dragReleasedEvent", Array.Empty<string>() }
        };

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            SliderSetting sliderSetting = componentType.component as SliderSetting;

            if (componentType.data.TryGetValue("isInt", out string isInt))
                sliderSetting.isInt = Parse.Bool(isInt);

            if (componentType.data.TryGetValue("increment", out string increment))
                sliderSetting.increments = Parse.Float(increment);

            if (componentType.data.TryGetValue("minValue", out string minValue))
                sliderSetting.slider.minValue = Parse.Float(minValue);

            if (componentType.data.TryGetValue("maxValue", out string maxValue))
                sliderSetting.slider.maxValue = Parse.Float(maxValue);

            if (componentType.data.TryGetValue("updateDuringDrag", out string updateDuringDrag))
                sliderSetting.updateDuringDrag = Parse.Bool(updateDuringDrag);
            else
                sliderSetting.updateDuringDrag = true;

            try
            {
                DragHelper dragHelper = sliderSetting.dragHelper;
                //-----Drag Started-----
                if (componentType.data.TryGetValue("onDragStarted", out string onDragStarted))
                {
                    dragHelper.onDragStarted.AddListener(delegate
                    {
                        if (!parserParams.actions.TryGetValue(onDragStarted, out BSMLAction onDragStartedAction))
                            throw new Exception("onDragStarted '" + onDragStarted + "' not found");

                        onDragStartedAction.Invoke();
                    });
                }
                if (componentType.data.TryGetValue("dragStartedEvent", out string dragStartedEvent))
                {
                    dragHelper.onDragStarted.AddListener(delegate
                    {
                        parserParams.EmitEvent(dragStartedEvent);
                    });
                }

                //-----Drag Released-----
                if (componentType.data.TryGetValue("onDragReleased", out string onDragReleased))
                {
                    dragHelper.onDragReleased.AddListener(delegate
                    {
                        if (!parserParams.actions.TryGetValue(onDragReleased, out BSMLAction onDragReleasedAction))
                            throw new Exception("onDragReleased '" + onDragReleased + "' not found");

                        onDragReleasedAction.Invoke();
                    });
                }
                if (componentType.data.TryGetValue("dragReleasedEvent", out string dragReleasedEvent))
                {
                    dragHelper.onDragReleased.AddListener(delegate
                    {
                        parserParams.EmitEvent(dragStartedEvent);
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.log?.Error(ex);
            }
        }
    }
}
