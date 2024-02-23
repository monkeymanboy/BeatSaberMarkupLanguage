using System;
using System.Collections.Generic;
using System.ComponentModel;
using BeatSaberMarkupLanguage.Components;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLParserParams
    {
        public object host;
        public Dictionary<string, BSMLAction> actions = new();
        public Dictionary<string, BSMLValue> values = new();

        private readonly Dictionary<string, Action> events = new();
        private readonly Dictionary<string, List<GameObject>> objectsWithTag = new();

        internal BSMLParserParams(object host)
        {
            this.host = host;
            this.NotifyUpdater = host is INotifyPropertyChanged notifyHost ? new NotifyUpdater(notifyHost) : null;
        }

        internal NotifyUpdater NotifyUpdater { get; }

        public void AddEvent(string ids, Action action)
        {
            foreach (string id in ids.Split(','))
            {
                if (events.ContainsKey(id))
                {
                    events[id] += action;
                }
                else
                {
                    events.Add(id, action);
                }
            }
        }

        public void EmitEvent(string ids)
        {
            foreach (string id in ids.Split(','))
            {
                if (events.ContainsKey(id))
                {
                    events[id].Invoke();
                }
            }
        }

        public void AddObjectTags(GameObject gameObject, params string[] tags)
        {
            foreach (string tag in tags)
            {
                if (objectsWithTag.TryGetValue(tag, out List<GameObject> list))
                {
                    list.Add(gameObject);
                }
                else
                {
                    objectsWithTag.Add(tag, new List<GameObject> { gameObject });
                }
            }
        }

        public List<GameObject> GetObjectsWithTag(string tag)
        {
            if (objectsWithTag.TryGetValue(tag, out List<GameObject> list))
            {
                return list;
            }
            else
            {
                return new List<GameObject>();
            }
        }

        public void PassTaggedObjects(BSMLParserParams parserParams)
        {
            foreach (KeyValuePair<string, List<GameObject>> pair in objectsWithTag)
            {
                parserParams.AddObjectsToTag(pair.Key, pair.Value);
            }
        }

        private void AddObjectsToTag(string tag, List<GameObject> gameObjects)
        {
            if (objectsWithTag.TryGetValue(tag, out List<GameObject> list))
            {
                list.AddRange(gameObjects);
            }
            else
            {
                objectsWithTag.Add(tag, gameObjects);
            }
        }
    }
}
