#if DEBUG
#define HRVC_DEBUG
#endif
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage.Attributes;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public abstract class BSMLAutomaticViewController : BSMLViewController, WatcherGroup.IHotReloadableController
    {
        private string resourceName;
        private string content;

        protected BSMLAutomaticViewController()
            : base()
        {
            HotReloadAttribute hotReloadAttr = GetType().GetCustomAttribute<HotReloadAttribute>();
            if (hotReloadAttr == null)
            {
                ContentFilePath = null;
            }
            else
            {
                ContentFilePath = Path.ChangeExtension(hotReloadAttr.Path, ".bsml");
            }
        }

        public override string Content
        {
            get
            {
                if (resourceName == null)
                {
                    ViewDefinitionAttribute viewDef = GetType().GetCustomAttribute<ViewDefinitionAttribute>();
                    if (viewDef != null)
                    {
                        resourceName = viewDef.Definition;
                    }
                    else
                    {
                        resourceName = GetDefaultResourceName(GetType());
                    }
                }

                if (string.IsNullOrEmpty(content))
                {
                    if (!string.IsNullOrEmpty(ContentFilePath) && File.Exists(ContentFilePath))
                    {
                        try
                        {
                            content = File.ReadAllText(ContentFilePath);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log?.Warn($"Unable to read file {ContentFilePath} for {name}: {ex.Message}");
                            Logger.Log?.Debug(ex);
                        }
                    }

                    if (string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(resourceName))
                    {
                        if (ContentFilePath != null)
                        {
#if HRVC_DEBUG
                            Logger.Log.Warn($"No content from file {ContentFilePath}, using resource {resourceName}");
#endif
                        }

                        content = Utilities.GetResourceContent(GetType().Assembly, resourceName);
                    }
                }

                return content;
            }
        }

        public bool ContentChanged { get; protected set; }

        public string ContentFilePath { get; }

        string WatcherGroup.IHotReloadableController.Name => name;

        void WatcherGroup.IHotReloadableController.MarkDirty()
        {
            ContentChanged = true;
            content = null;
        }

        void WatcherGroup.IHotReloadableController.Refresh(bool forceReload)
        {
            if (!isActiveAndEnabled)
            {
#if HRVC_DEBUG
                Logger.Log.Warn($"Trying to refresh {GetInstanceID()}:{name} when it isn't ActiveAndEnabled.");
#endif
                return;
            }

            if (ContentChanged || forceReload)
            {
                try
                {
                    __Deactivate(false, false, false);
                    ClearContents();
                    __Activate(false, false);
                }
                catch (Exception ex)
                {
                    Logger.Log?.Error(ex);
                }
            }
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (!string.IsNullOrEmpty(ContentFilePath))
            {
#if HRVC_DEBUG
                Logger.Log.Notice($"DidActivate: {GetInstanceID()}:{name}");
#endif

                if (ContentChanged && !firstActivation)
                {
                    ContentChanged = false;
                    ParseWithFallback();
                }
                else if (firstActivation)
                {
                    ParseWithFallback();
                }

                bool registered = WatcherGroup.RegisterViewController(this);
#if HRVC_DEBUG
                if (registered)
                {
                    Logger.Log.Info($"Registered view controller {this.name} to watcher");
                }
                else
                {
                    Logger.Log.Error($"Failed to register view controller {this.name} to watcher");
                }
#endif
            }
            else if (firstActivation)
            {
                ParseWithFallback();
            }

#pragma warning disable CS0618
            didActivate?.Invoke(firstActivation, addedToHierarchy, screenSystemEnabling);
#pragma warning restore CS0618
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            if (!string.IsNullOrEmpty(ContentFilePath))
            {
                content = null;
#if HRVC_DEBUG
                Logger.Log.Notice($"DidDeactivate: {GetInstanceID()}:{name}");
#endif
                if (!WatcherGroup.UnregisterViewController(this))
                {
#if HRVC_DEBUG
                    Logger.Log.Warn($"Failed to Unregister {GetInstanceID()}:{name}");
#endif
                }
            }

            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }

        private static string GetDefaultResourceName(Type type)
        {
            string ns = type.Namespace;
            string name = type.Name;
            string resourceNoExtension = (ns.Length > 0 ? ns + "." : string.Empty) + name;

            // First we check with no extension in case DependentUpon is being used on the embedded resource
            return type.Assembly.GetManifestResourceNames().Contains(resourceNoExtension) ? resourceNoExtension : $"{resourceNoExtension}.bsml";
        }
    }
}
