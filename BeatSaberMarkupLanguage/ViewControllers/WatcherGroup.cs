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

        private readonly WaitForSeconds HotReloadDelay = new WaitForSeconds(.5f);

        private readonly Dictionary<int, WeakReference<IHotReloadableController>> BoundControllers = new Dictionary<int, WeakReference<IHotReloadableController>>();

        internal WatcherGroup(string directory)
        {
            ContentDirectory = directory;
            CreateWatcher();
        }

        private void CreateWatcher()
        {
            if (Watcher != null) return;
            if (!Directory.Exists(ContentDirectory)) return;
#if HRVC_DEBUG
            Logger.log.Critical($"Creating FileSystemWatcher for {ContentDirectory}");
#endif
            Watcher = new FileSystemWatcher(ContentDirectory, "*.bsml")
            {
                NotifyFilter = NotifyFilters.LastWrite
            };
            Watcher.Changed += OnFileWasChanged;
        }

        private void DestroyWatcher()
        {
#if HRVC_DEBUG
            Logger.log.Critical($"Destroying FileSystemWatcher for {ContentDirectory}");
#endif
            Watcher.Dispose();
            Watcher = null;
        }

        private void OnFileWasChanged(object sender, FileSystemEventArgs e)
        {
            foreach (KeyValuePair<int, WeakReference<IHotReloadableController>> pair in BoundControllers.ToArray())
            {
                if (!pair.Value.TryGetTarget(out IHotReloadableController controller))
                {
#if HRVC_DEBUG
                    Logger.log.Critical($"Watcher_Changed: {pair.Key} has been Garbage Collected, unbinding.");
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
            if (BoundControllers.Count == 0)
            {
#if HRVC_DEBUG
                Logger.log.Critical($"BoundControllers is empty in Watcher_Changed.");
#endif
                DestroyWatcher();
            }
        }

        private IEnumerator<WaitForSeconds> HotReloadCoroutine()
        {
            if (IsReloading) yield break;
            IsReloading = true;
            yield return HotReloadDelay;
            KeyValuePair<int, WeakReference<IHotReloadableController>>[] array = BoundControllers.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                KeyValuePair<int, WeakReference<IHotReloadableController>> pair = array[i];
                if (!pair.Value.TryGetTarget(out IHotReloadableController controller))
                {
#if HRVC_DEBUG
                    Logger.log.Critical($"{pair.Key} has been Garbage Collected, unbinding.");
#endif
                    UnbindController(pair.Key);
                    continue;
                }
                if (controller.ContentChanged)
                {
#if HRVC_DEBUG
                    Logger.log.Critical($"{pair.Key} seems to exist and has changed content.");
#endif
                    controller?.Refresh();
                    //RefreshViewController(controller);
                }
            }
            IsReloading = false;
        }

        internal bool BindController(IHotReloadableController controller)
        {
            if (BoundControllers.ContainsKey(controller.GetInstanceID()))
            {
#if HRVC_DEBUG
                Logger.log.Critical($"Failed to register controller, already exists. {controller.GetInstanceID()}:{controller.Name}");
#endif
                return false;
            }
            BoundControllers.Add(controller.GetInstanceID(), new WeakReference<IHotReloadableController>(controller));
            CreateWatcher();
            Watcher.EnableRaisingEvents = true;
#if HRVC_DEBUG
            Logger.log.Info($"Registering controller {controller.GetInstanceID()}:{controller.Name}");
#endif
            return true;
        }

        internal bool UnbindController(int instanceId)
        {
#if HRVC_DEBUG
            if (BoundControllers.TryGetValue(instanceId, out WeakReference<IHotReloadableController> controllerRef))
                if (!controllerRef.TryGetTarget(out IHotReloadableController controller))
                    Logger.log.Warn($"Unbinding garbage collected controller {instanceId}");
                else
                    Logger.log.Warn($"Unbinding existing controller {instanceId}:{controller.Name}");
            else
                Logger.log.Warn($"Trying to unbind controller that isn't in the dictionary");
#endif
            bool remove = BoundControllers.Remove(instanceId);
            if (BoundControllers.Count == 0)
                DestroyWatcher();
            return remove;
        }

        internal bool UnbindController(IHotReloadableController controller)
        {
            if (controller == null)
            {
#if HRVC_DEBUG
                Logger.log.Critical($"Unable to unbind controller, it is null.");
#endif
                return false;
            }
            return UnbindController(controller.GetInstanceID());
        }


        private static readonly Dictionary<string, WatcherGroup> WatcherDictionary = new Dictionary<string, WatcherGroup>();
        public static bool RegisterViewController(IHotReloadableController controller)
        {
            string contentFile = controller.ContentFilePath;
            if (string.IsNullOrEmpty(contentFile)) return false;
            string contentDirectory = Path.GetDirectoryName(contentFile);
            if (!Directory.Exists(contentDirectory)) return false;
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
                Logger.log.Critical($"Skipping registration for {controller.GetInstanceID()}:{controller.Name}, it has not content file defined.");
#endif
                return false;
            }
            bool successful = false;
            string contentDirectory = Path.GetDirectoryName(contentFile);
            if (WatcherDictionary.TryGetValue(contentDirectory, out WatcherGroup watcherGroup))
                successful = watcherGroup.UnbindController(controller);
#if HRVC_DEBUG
            else
                Logger.log.Warn($"Unable to get WatcherGroup for {contentDirectory}");
            if (successful)
                Logger.log.Info($"Successfully unregistered {controller.GetInstanceID()}:{controller.Name}");
            else
                Logger.log.Warn($"Failed to Unregister {controller.GetInstanceID()}:{controller.Name}");
#endif
            return successful;
        }

    }
}

