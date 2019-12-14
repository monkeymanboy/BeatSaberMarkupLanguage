#if DEBUG
#define HRVC_DEBUG
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HMUI;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public abstract class HotReloadableViewController : BSMLViewController
    {
        #region FileSystemWatcher
        internal class WatcherGroup
        {
            internal FileSystemWatcher Watcher { get; private set; }

            internal string ContentDirectory { get; private set; }
            
            internal bool IsReloading { get; private set; }

            private readonly WaitForSeconds HotReloadDelay = new WaitForSeconds(.5f);

            private readonly Dictionary<int, WeakReference<HotReloadableViewController>> BoundControllers = new Dictionary<int, WeakReference<HotReloadableViewController>>();

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
                Watcher.Changed += Watcher_Changed;
            }

            private void DestroyWatcher()
            {
#if HRVC_DEBUG
                Logger.log.Critical($"Destroying FileSystemWatcher for {ContentDirectory}");
#endif
                Watcher.Dispose();
                Watcher = null;
            }

            private void Watcher_Changed(object sender, FileSystemEventArgs e)
            {
                foreach (KeyValuePair<int, WeakReference<HotReloadableViewController>> pair in BoundControllers.ToArray())
                {
                    if (!pair.Value.TryGetTarget(out HotReloadableViewController controller))
                    {
#if HRVC_DEBUG
                        Logger.log.Critical($"Watcher_Changed: {pair.Key} has been Garbage Collected, unbinding.");
#endif
                        UnbindController(pair.Key);
                        continue;
                    }
                    if (e.FullPath == Path.GetFullPath(controller.ResourceFilePath))
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
                KeyValuePair<int, WeakReference<HotReloadableViewController>>[] array = BoundControllers.ToArray();
                for (int i = 0; i < array.Length; i++)
                {
                    KeyValuePair<int, WeakReference<HotReloadableViewController>> pair = array[i];
                    if (!pair.Value.TryGetTarget(out HotReloadableViewController controller))
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
                        RefreshViewController(controller);
                    }
                }
                IsReloading = false;
            }
            
            internal bool BindController(HotReloadableViewController controller)
            {
                if (BoundControllers.ContainsKey(controller.GetInstanceID()))
                {
#if HRVC_DEBUG
                    Logger.log.Critical($"Failed to register controller, already exists. {controller.GetInstanceID()}:{controller.name}");
#endif
                    return false;
                }
                BoundControllers.Add(controller.GetInstanceID(), new WeakReference<HotReloadableViewController>(controller));
                CreateWatcher();
                Watcher.EnableRaisingEvents = true;
#if HRVC_DEBUG
                Logger.log.Info($"Registering controller {controller.GetInstanceID()}:{controller.name}");
#endif
                return true;
            }

            internal bool UnbindController(int instanceId)
            {
#if HRVC_DEBUG
                if (BoundControllers.TryGetValue(instanceId, out WeakReference<HotReloadableViewController> controllerRef))
                    if (!controllerRef.TryGetTarget(out HotReloadableViewController controller))
                        Logger.log.Warn($"Unbinding garbage collected controller {instanceId}");
                    else
                        Logger.log.Warn($"Unbinding existing controller {instanceId}:{controller.name}");
                else
                    Logger.log.Warn($"Trying to unbind controller that isn't in the dictionary");
#endif
                bool remove = BoundControllers.Remove(instanceId);
                if (BoundControllers.Count == 0)
                    DestroyWatcher();
                return remove;
            }

            internal bool UnbindController(HotReloadableViewController controller)
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
        }

        private static Dictionary<string, WatcherGroup> WatcherDictionary = new Dictionary<string, WatcherGroup>();
        public static bool RegisterViewController(HotReloadableViewController controller)
        {
            string contentFile = controller.ResourceFilePath;
            if (string.IsNullOrEmpty(contentFile)) return false;
            string contentDirectory = Path.GetDirectoryName(contentFile);
            if (!Directory.Exists(contentDirectory)) return false;
            WatcherGroup watcherGroup;
            if (!WatcherDictionary.TryGetValue(contentDirectory, out watcherGroup))
            {
                watcherGroup = new WatcherGroup(contentDirectory);
                WatcherDictionary.Add(contentDirectory, watcherGroup);
            }
            watcherGroup.BindController(controller);

            return true;
        }

        public static bool UnRegisterViewController(HotReloadableViewController controller)
        {
            string contentFile = controller.ResourceFilePath;
            if (string.IsNullOrEmpty(contentFile))
            {
#if HRVC_DEBUG
                Logger.log.Critical($"Skipping registration for {controller.GetInstanceID()}:{controller.name}, it has not content file defined.");
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
                Logger.log.Info($"Successfully unregistered {controller.GetInstanceID()}:{controller.name}");
            else
                Logger.log.Warn($"Failed to Unregister {controller.GetInstanceID()}:{controller.name}");
#endif
            return successful;
        }

        #endregion

        public static void RefreshViewController(HotReloadableViewController viewController, bool forceReload = false)
        {
            if (viewController == null)
            {
#if HRVC_DEBUG
                Logger.log.Warn($"Trying to refresh a HotReloadableViewController when it doesn't exist.");
#endif
                return;
            }
            if (!viewController.isActiveAndEnabled)
            {
#if HRVC_DEBUG
                Logger.log.Warn($"Trying to refresh {viewController.GetInstanceID()}:{viewController.name} when it isn't ActiveAndEnabled.");
#endif
                return;
            }
            if (viewController.ContentChanged || forceReload)
            {
                try
                {
                    viewController.__Deactivate(ViewController.DeactivationType.NotRemovedFromHierarchy, false);
                    for (int i = 0; i < viewController.transform.childCount; i++)
                        GameObject.Destroy(viewController.transform.GetChild(i).gameObject);
                    viewController.__Activate(ViewController.ActivationType.NotAddedToHierarchy);
                }
                catch (Exception ex)
                {
                    Logger.log?.Error(ex);
                }
            }
        }

        public abstract string ResourceName { get; }
        public abstract string ResourceFilePath { get; }

        public virtual string FallbackContent => @"<vertical child-control-height='false' child-control-width='true' child-align='UpperCenter' pref-width='110' pad-left='3' pad-right='3'>
                                                      <horizontal bg='panel-top' pad-left='10' pad-right='10' horizontal-fit='PreferredSize' vertical-fit='PreferredSize'>
                                                        <text text='Invalid BSML' font-size='10'/>
                                                      </horizontal>
                                                      <text text ='{0}' font-size='5'/>
                                                    </vertical>";

        private string _content;
        public override string Content
        {
            get
            {
                if (string.IsNullOrEmpty(_content))
                {
                    if (!string.IsNullOrEmpty(ResourceFilePath) && File.Exists(ResourceFilePath))
                    {
                        try
                        {
                            _content = File.ReadAllText(ResourceFilePath);
                        }
                        catch(Exception ex)
                        {
                            Logger.log?.Warn($"Unable to read file {ResourceFilePath} for {name}: {ex.Message}");
                            Logger.log?.Debug(ex);
                        }
                    }
                    if (string.IsNullOrEmpty(_content) && !string.IsNullOrEmpty(ResourceName))
                    {
#if HRVC_DEBUG
                        Logger.log.Warn($"No content from file {ResourceFilePath}, using resource {ResourceName}");
#endif
                        _content = Utilities.GetResourceContent(Assembly.GetAssembly(this.GetType()), ResourceName);
                    }
                }
                return _content;
            }
        }

        public bool ContentChanged { get; protected set; }

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            if (ContentChanged && !firstActivation)
            {
                ContentChanged = false;
                ParseWithFallback();
            }
            else if (firstActivation)
                ParseWithFallback();
            bool registered = RegisterViewController(this);
#if HRVC_DEBUG
            if (registered)
                Logger.log.Info($"Registered {this.name}");
            else
                Logger.log.Error($"Failed to register {this.name}");
#endif
            
            didActivate?.Invoke(firstActivation, type);
        }


        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            _content = null;
#if HRVC_DEBUG
            Logger.log.Warn($"DidDeactive: {GetInstanceID()}:{name}");
#endif
            if (!UnRegisterViewController(this))
            {
#if HRVC_DEBUG
                Logger.log.Warn($"Failed to Unregister {GetInstanceID()}:{name}");
#endif
            }
            base.DidDeactivate(deactivationType);
        }

        private void ParseWithFallback()
        {
            try
            {
                BSMLParser.instance.Parse(Content, gameObject, this);
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error parsing BSML: {ex.Message}");
                Logger.log.Debug(ex);
                BSMLParser.instance.Parse(string.Format(FallbackContent, Utilities.EscapeXml(ex.Message)), gameObject, this);
            }
        }

        public void MarkDirty()
        {
            ContentChanged = true;
            _content = null;
        }
    }
}

