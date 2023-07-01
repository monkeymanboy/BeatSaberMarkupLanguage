using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Util
{
    internal static class BehaviourExtensions
    {
        internal static T GetComponentOnChild<T>(this GameObject gameObject, string path)
            where T : Component => gameObject.transform.GetComponentOnChild<T>(path);

        internal static T GetComponentOnChild<T>(this Component component, string path)
            where T : Component => component.transform.GetComponentOnChild<T>(path);

        internal static T GetComponentOnChild<T>(this Transform transform, string path)
            where T : Component
        {
            Transform child = transform.Find(path);

            if (child == null)
            {
                throw new ArgumentException($"Transform '{transform.name}' does not have a child '{path}'");
            }

            if (child.TryGetComponent(out T component))
            {
                return component;
            }
            else
            {
                throw new ArgumentException($"Transform '{transform.name}' does not have a component of type {typeof(T).FullName}");
            }
        }

        internal static GameObject GetChildGameObject(this GameObject gameObject, string path) => gameObject.transform.GetChildGameObject(path);

        internal static GameObject GetChildGameObject(this Component component, string path) => component.transform.GetChildGameObject(path);

        internal static GameObject GetChildGameObject(this Transform transform, string path)
        {
            Transform child = transform.Find(path);

            return child == null
                ? throw new ArgumentException($"Transform '{transform.name}' does not have a child '{path}'")
                : child.gameObject;
        }

        internal static string GetTransformPath(this Component component) => component.transform.GetTransformPath();

        internal static string GetTransformPath(this Transform transform)
        {
            List<string> path = new();

            while (transform != null)
            {
                path.Add(transform.name);
                transform = transform.parent;
            }

            path.Reverse();

            return string.Join("/", path);
        }

        internal static List<MonoBehaviour> GetInjectedParentComponents(this Component component, DiContainer container) => component.transform.GetInjectedParentComponents(container);

        internal static List<MonoBehaviour> GetInjectedParentComponents(this Transform transform, DiContainer container)
        {
            List<MonoBehaviour> behaviours = new();

            while (transform != null)
            {
                foreach (MonoBehaviour monoBehaviour in transform.GetComponents<MonoBehaviour>())
                {
                    if (container.HasBinding(monoBehaviour.GetType()))
                    {
                        behaviours.Add(monoBehaviour);
                    }
                }

                transform = transform.parent;
            }

            return behaviours;
        }

        internal static void PrintDebugInfo(this Component component) => component.transform.PrintDebugInfo();

        internal static void PrintDebugInfo(this GameObject gameObject) => gameObject.transform.PrintDebugInfo();

        internal static void PrintDebugInfo(this Transform transform)
        {
            Type caller = new StackTrace().GetFrames().Select(f => f.GetMethod().DeclaringType).First(t => t != typeof(BehaviourExtensions));

            Logger.Log.Notice($"{caller.Name}: GameObject '{transform.GetTransformPath()}'");

            List<MonoBehaviour> list = transform.GetInjectedParentComponents(BeatSaberUI.DiContainer);

            if (list.Count > 0)
            {
                Logger.Log.Notice("Injected components in parent(s):");
                foreach (MonoBehaviour behaviour in list)
                {
                    Logger.Log.Notice($"> {behaviour.GetType().FullName} {behaviour.name}");
                }
            }
            else
            {
                Logger.Log.Error("!! No injected components in parent(s)");
            }
        }
    }
}
