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
        private static string GetDefaultResourceName(Type type)
        {
            string ns = type.Namespace;
            string name = type.Name;
            string resourceNoExtension = (ns.Length > 0 ? ns + "." : string.Empty) + name;

            // First we check with no extension in case DependentUpon is being used on the embedded resource
            return type.Assembly.GetManifestResourceNames().Contains(resourceNoExtension) ? resourceNoExtension : $"{resourceNoExtension}.bsml";
        }

        public virtual string FallbackContent => @"<bg>
                                                     <vertical child-control-height='false' child-control-width='true' child-align='UpperCenter' pref-width='110' pad-left='3' pad-right='3'>
                                                       <horizontal bg='panel-top' pad-left='10' pad-right='10' horizontal-fit='PreferredSize' vertical-fit='PreferredSize'>
                                                         <text text='Invalid BSML' font-size='10'/>
                                                       </horizontal>
                                                     </vertical>
                                                     <text-page text='{0}' anchor-min-x='0.1' anchor-max-x='0.9' anchor-max-y='0.8'/>
                                                   </bg>";

        private string resourceName;
        private string content;

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

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (!string.IsNullOrEmpty(ContentFilePath))
            {
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
                    Logger.Log.Info($"Registered {this.name}");
                }
                else
                {
                    Logger.Log.Error($"Failed to register {this.name}");
                }
#endif
            }
            else if (firstActivation)
            {
                ParseWithFallback();
            }

            didActivate?.Invoke(firstActivation, addedToHierarchy, screenSystemEnabling);
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            if (!string.IsNullOrEmpty(ContentFilePath))
            {
                content = null;
#if HRVC_DEBUG
                Logger.Log.Warn($"DidDeactive: {GetInstanceID()}:{name}");
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

        private void ParseWithFallback()
        {
            try
            {
                BSMLParser.instance.Parse(Content, gameObject, this);
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Error parsing BSML: {ex.Message}");
                Logger.Log.Debug(ex);
                BSMLParser.instance.Parse(string.Format(FallbackContent, Utilities.EscapeXml(ex.Message)), gameObject, null);
            }
        }

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

                    for (int i = 0; i < transform.childCount; i++)
                    {
                        Destroy(transform.GetChild(i).gameObject);
                    }

                    __Activate(false, false);
                }
                catch (Exception ex)
                {
                    Logger.Log?.Error(ex);
                }
            }
        }
    }
}
