using System;
using System.Collections.Generic;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLParserParams
    {
        public object host;
        public Dictionary<string, BSMLAction> actions = new Dictionary<string, BSMLAction>();
        public Dictionary<string, BSMLValue> values = new Dictionary<string, BSMLValue>();
        public Dictionary<string, Action> events = new Dictionary<string, Action>();

        public void AddEvent(string id, Action action)
        {
            if (events.ContainsKey(id))
                events[id] += action;
            else
                events.Add(id, action);
        }

        public void EmitEvent(string id)
        {
            if (events.ContainsKey(id))
                events[id].Invoke();
        }
    }
}
