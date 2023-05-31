#if DEBUG
#define HRVC_DEBUG
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    internal class WatcherGroup
    {
        private static readonly Dictionary<string, WatcherGroup> WatcherDictionary = new();

        private readonly WaitForSeconds hotReloadDelay = new(0.5f);
        private readonly Dictionary<int, WeakReference<IHotReloadableController>> boundControllers = new();

        internal WatcherGroup(string directory)
        {
            ContentDirectory = directory;
            CreateWatcher();
        }

        internal interface IHotReloadableController
        {
            bool ContentChanged { get; }

            string ContentFilePath { get; }

            string Name { get; }

            void MarkDirty();

            void Refresh(bool forceReload = false);

            int GetInstanceID();
        }

        internal FileSystemWatcher Watcher { get; private set; }

        internal string ContentDirectory { get; private set; }

        internal bool IsReloading { get; private set; }

        public static bool RegisterViewController(IHotReloadableController controller)
        {
            string contentFile = controller.ContentFilePath;
            if (string.IsNullOrEmpty(contentFile))
            {
                return false;
            }

            string contentDirectory = Path.GetDirectoryName(contentFile);
            if (!Directory.Exists(contentDirectory))
            {
                return false;
            }

            if (!WatcherDictionary.TryGetValue(contentDirectory, out WatcherGroup watcherGroup))
            {
                watcherGroup = new WatcherGroup(contentDirectory);
                WatcherDictionary.Add(contentDirectory, watcherGroup);
            }

            watcherGroup.BindController(controller);

            return true;
        }

        public static bool UnregisterViewController(IHotReloadableController controller)
        {
            string contentFile = controller.ContentFilePath;
            if (string.IsNullOrEmpty(contentFile))
            {
#if HRVC_DEBUG
                Logger.Log.Critical($"Skipping registration for {controller.GetInstanceID()}:{controller.Name}, it has not content file defined.");
#endif
                return false;
            }

            bool successful = false;
            string contentDirectory = Path.GetDirectoryName(contentFile);
            if (WatcherDictionary.TryGetValue(contentDirectory, out WatcherGroup watcherGroup))
            {
                successful = watcherGroup.UnbindController(controller);
            }
#if HRVC_DEBUG
            else
            {
                Logger.Log.Warn($"Unable to get WatcherGroup for {contentDirectory}");
            }

            if (successful)
            {
                Logger.Log.Info($"Successfully unregistered {controller.GetInstanceID()}:{controller.Name}");
            }
            else
            {
                Logger.Log.Warn($"Failed to Unregister {controller.GetInstanceID()}:{controller.Name}");
            }
#endif
            return successful;
        }

        internal bool BindController(IHotReloadableController controller)
        {
            if (boundControllers.ContainsKey(controller.GetInstanceID()))
            {
#if HRVC_DEBUG
                Logger.Log.Critical($"Failed to register controller, already exists. {controller.GetInstanceID()}:{controller.Name}");
#endif
                return false;
            }

            boundControllers.Add(controller.GetInstanceID(), new WeakReference<IHotReloadableController>(controller));
            CreateWatcher();
            Watcher.EnableRaisingEvents = true;
#if HRVC_DEBUG
            Logger.Log.Info($"Registering controller {controller.GetInstanceID()}:{controller.Name}");
#endif
            return true;
        }

        internal bool UnbindController(int instanceId)
        {
#if HRVC_DEBUG
            if (boundControllers.TryGetValue(instanceId, out WeakReference<IHotReloadableController> controllerRef))
            {
                if (!controllerRef.TryGetTarget(out IHotReloadableController controller))
                {
                    Logger.Log.Warn($"Unbinding garbage collected controller {instanceId}");
                }
                else
                {
                    Logger.Log.Warn($"Unbinding existing controller {instanceId}:{controller.Name}");
                }
            }
            else
            {
                Logger.Log.Warn($"Trying to unbind controller that isn't in the dictionary");
            }
#endif
            bool remove = boundControllers.Remove(instanceId);

            if (boundControllers.Count == 0)
            {
                DestroyWatcher();
            }

            return remove;
        }

        internal bool UnbindController(IHotReloadableController controller)
        {
            if (controller == null)
            {
#if HRVC_DEBUG
                Logger.Log.Critical($"Unable to unbind controller, it is null.");
#endif
                return false;
            }

            return UnbindController(controller.GetInstanceID());
        }

        private void CreateWatcher()
        {
            if (Watcher != null)
            {
                return;
            }

            if (!Directory.Exists(ContentDirectory))
            {
                return;
            }
#if HRVC_DEBUG
            Logger.Log.Critical($"Creating FileSystemWatcher for {ContentDirectory}");
#endif
            Watcher = new FileSystemWatcher(ContentDirectory, "*.bsml")
            {
                NotifyFilter = NotifyFilters.LastWrite,
            };
            Watcher.Changed += OnFileWasChanged;
        }

        private void DestroyWatcher()
        {
#if HRVC_DEBUG
            Logger.Log.Critical($"Destroying FileSystemWatcher for {ContentDirectory}");
#endif
            Watcher.Dispose();
            Watcher = null;
        }

        private void OnFileWasChanged(object sender, FileSystemEventArgs e)
        {
            foreach (KeyValuePair<int, WeakReference<IHotReloadableController>> pair in boundControllers.ToArray())
            {
                if (!pair.Value.TryGetTarget(out IHotReloadableController controller))
                {
#if HRVC_DEBUG
                    Logger.Log.Critical($"Watcher_Changed: {pair.Key} has been Garbage Collected, unbinding.");
#endif
                    UnbindController(pair.Key);
                    continue;
                }

                if (e.FullPath == Path.GetFullPath(controller.ContentFilePath))
                {
                    controller.MarkDirty();
                    HMMainThreadDispatcher.instance.Enqueue(HotReloadCoroutine());
                }
            }

            if (boundControllers.Count == 0)
            {
#if HRVC_DEBUG
                Logger.Log.Critical($"BoundControllers is empty in Watcher_Changed.");
#endif
                DestroyWatcher();
            }
        }

        private IEnumerator<WaitForSeconds> HotReloadCoroutine()
        {
            if (IsReloading)
            {
                yield break;
            }

            IsReloading = true;
            yield return hotReloadDelay;
            KeyValuePair<int, WeakReference<IHotReloadableController>>[] array = boundControllers.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                KeyValuePair<int, WeakReference<IHotReloadableController>> pair = array[i];
                if (!pair.Value.TryGetTarget(out IHotReloadableController controller))
                {
#if HRVC_DEBUG
                    Logger.Log.Critical($"{pair.Key} has been Garbage Collected, unbinding.");
#endif
                    UnbindController(pair.Key);
                    continue;
                }

                if (controller.ContentChanged)
                {
#if HRVC_DEBUG
                    Logger.Log.Critical($"{pair.Key} seems to exist and has changed content.");
#endif
                    controller?.Refresh();
                }
            }

            IsReloading = false;
        }
    }
}
