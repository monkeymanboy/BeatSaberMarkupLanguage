#if DEBUG
#define HRVC_DEBUG
#endif
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public abstract class BSMLAutomaticViewController : BSMLViewController, WatcherGroup.IHotReloadableController
    {
        private static string GetDefaultResourceName(Type type)
        {
            string ns = type.Namespace;
            string name = type.Name;
            return (ns.Length > 0 ? ns + "." : "") + name + ".bsml";
        }
        public virtual string FallbackContent => @"<bg>
                                                     <vertical child-control-height='false' child-control-width='true' child-align='UpperCenter' pref-width='110' pad-left='3' pad-right='3'>
                                                       <horizontal bg='panel-top' pad-left='10' pad-right='10' horizontal-fit='PreferredSize' vertical-fit='PreferredSize'>
                                                         <text text='Invalid BSML' font-size='10'/>
                                                       </horizontal>
                                                     </vertical>
                                                     <text-page text='{0}' anchor-min-x='0.1' anchor-max-x='0.9' anchor-max-y='0.8'/>
                                                   </bg>";

        private string _resourceName;
        private string _content;
        public override string Content 
        {
            get
            {
                if (_resourceName == null)
                {
                    ViewDefinitionAttribute viewDef = GetType().GetCustomAttribute<ViewDefinitionAttribute>();
                    if (viewDef != null) _resourceName = viewDef.Definition;
                    else _resourceName = GetDefaultResourceName(GetType());
                }
                if (string.IsNullOrEmpty(_content))
                {
                    if (!string.IsNullOrEmpty(ContentFilePath) && File.Exists(ContentFilePath))
                    {
                        try
                        {
                            _content = File.ReadAllText(ContentFilePath);
                        }
                        catch (Exception ex)
                        {
                            Logger.log?.Warn($"Unable to read file {ContentFilePath} for {name}: {ex.Message}");
                            Logger.log?.Debug(ex);
                        }
                    }
                    if (string.IsNullOrEmpty(_content) && !string.IsNullOrEmpty(_resourceName))
                    {
                        if (ContentFilePath != null)
                        {
#if HRVC_DEBUG
                            Logger.log.Warn($"No content from file {ContentFilePath}, using resource {_resourceName}");
#endif
                        }
                        _content = Utilities.GetResourceContent(GetType().Assembly, _resourceName);
                    }
                }
                return _content;
            } 
        }

        public bool ContentChanged { get; protected set; }

        public string ContentFilePath { get; }

        string WatcherGroup.IHotReloadableController.Name => name;

        protected BSMLAutomaticViewController() : base()
        {
            HotReloadAttribute hotReloadAttr = GetType().GetCustomAttribute<HotReloadAttribute>();
            if (hotReloadAttr == null) ContentFilePath = null;
            else ContentFilePath = Path.ChangeExtension(hotReloadAttr.Path, ".bsml");
        }

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            if (!string.IsNullOrEmpty(ContentFilePath))
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
            }
            else
            {
                if(firstActivation)
                    ParseWithFallback();
            }

            didActivate?.Invoke(firstActivation, type);
        }


        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            if (!string.IsNullOrEmpty(ContentFilePath))
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
                BSMLParser.instance.Parse(string.Format(FallbackContent, Utilities.EscapeXml(ex.Message)), gameObject, null);
            }
        }

        void WatcherGroup.IHotReloadableController.MarkDirty()
        {
            ContentChanged = true;
            _content = null;
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
                    __Deactivate(ViewController.DeactivationType.NotRemovedFromHierarchy, false);
                    for (int i = 0; i < transform.childCount; i++)
                        GameObject.Destroy(transform.GetChild(i).gameObject);
                    __Activate(ViewController.ActivationType.NotAddedToHierarchy);
                }
                catch (Exception ex)
                {
                    Logger.log?.Error(ex);
                }
            }
        }
    }
}
