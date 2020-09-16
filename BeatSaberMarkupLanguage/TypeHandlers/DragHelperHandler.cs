using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using static BeatSaberMarkupLanguage.BSMLParser;

namespace BeatSaberMarkupLanguage.TypeHandlers
{
    [ComponentHandler(typeof(DragHelper))]
    public class DragHelperHandler : TypeHandler<DragHelper>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "onDragStarted", Array.Empty<string>() },
            { "dragStartedEvent", Array.Empty<string>() },
            { "onDragReleased", Array.Empty<string>() },
            { "dragReleasedEvent", Array.Empty<string>() }
        };

        public override Dictionary<string, Action<DragHelper, string>> Setters => new Dictionary<string, Action<DragHelper, string>>();

        public override void HandleType(ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            try
            {
                DragHelper dragHelper = componentType.component as DragHelper;
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

        public static void SetInteractable(Button button, string flag)
        {
            button.interactable = Parse.Bool(flag);
        }

    }
}