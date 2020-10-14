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
    [Obsolete("It is now recommended that you use BSMLAutomaticViewController and it's associated attributes", false)]
    public abstract class HotReloadableViewController : BSMLViewController, WatcherGroup.IHotReloadableController
    {

        public static void RefreshViewController(HotReloadableViewController viewController, bool forceReload = false)
        {
            if (viewController == null)
            {
#if HRVC_DEBUG
                Logger.log.Warn($"Trying to refresh a HotReloadableViewController when it doesn't exist.");
#endif
                return;
            }
            (viewController as WatcherGroup.IHotReloadableController).Refresh(forceReload);
        }

        void WatcherGroup.IHotReloadableController.Refresh(bool forceReload)
        {
            if (!isActiveAndEnabled)
            {
#if HRVC_DEBUG
                Logger.log.Warn($"Trying to refresh {GetInstanceID()}:{name} when it isn't ActiveAndEnabled.");
#endif
                return;
            }
            if (ContentChanged || forceReload)
            {
                try
                {
                    __Deactivate(false, false, false);
                    for (int i = 0; i < transform.childCount; i++)
                        GameObject.Destroy(transform.GetChild(i).gameObject);
                    __Activate(false, false);
                }
                catch (Exception ex)
                {
                    Logger.log?.Error(ex);
                }
            }
        }

        public abstract string ResourceName { get; }
        public abstract string ContentFilePath { get; }

        public virtual string FallbackContent => @"<vertical child-control-height='false' child-control-width='true' child-align='UpperCenter' pref-width='110' pad-left='3' pad-right='3'>
                                                      <horizontal bg='panel-top' pad-left='10' pad-right='10' horizontal-fit='PreferredSize' vertical-fit='PreferredSize'>
                                                        <text text='Invalid BSML' font-size='10'/>
                                                      </horizontal>
                                                      <text text='{0}' font-size='5'/>
                                                    </vertical>";

        private string _content;
        public override string Content
        {
            get
            {
                if (string.IsNullOrEmpty(_content))
                {
                    if (!string.IsNullOrEmpty(ContentFilePath) && File.Exists(ContentFilePath))
                    {
                        try
                        {
                            _content = File.ReadAllText(ContentFilePath);
                        }
                        catch(Exception ex)
                        {
                            Logger.log?.Warn($"Unable to read file {ContentFilePath} for {name}: {ex.Message}");
                            Logger.log?.Debug(ex);
                        }
                    }
                    if (string.IsNullOrEmpty(_content) && !string.IsNullOrEmpty(ResourceName))
                    {
#if HRVC_DEBUG
                        Logger.log.Warn($"No content from file {ContentFilePath}, using resource {ResourceName}");
#endif
                        _content = Utilities.GetResourceContent(Assembly.GetAssembly(this.GetType()), ResourceName);
                    }
                }
                return _content;
            }
        }

        public bool ContentChanged { get; protected set; }

        string WatcherGroup.IHotReloadableController.Name => name;

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (ContentChanged && !firstActivation)
            {
                ContentChanged = false;
                ParseWithFallback();
            }
            else if (firstActivation)
                ParseWithFallback();
            bool registered = WatcherGroup.RegisterViewController(this);
#if HRVC_DEBUG
            if (registered)
                Logger.log.Info($"Registered {this.name}");
            else
                Logger.log.Error($"Failed to register {this.name}");
#endif
            
            didActivate?.Invoke(firstActivation, addedToHierarchy, screenSystemEnabling);
        }


        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _content = null;
#if HRVC_DEBUG
            Logger.log.Warn($"DidDeactive: {GetInstanceID()}:{name}");
#endif
            if (!WatcherGroup.UnregisterViewController(this))
            {
#if HRVC_DEBUG
                Logger.log.Warn($"Failed to Unregister {GetInstanceID()}:{name}");
#endif
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

