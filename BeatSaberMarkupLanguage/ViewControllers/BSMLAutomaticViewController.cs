using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public abstract class BSMLAutomaticViewController : BSMLViewController
    {
        private static string GetDefaultResourceName(Type type)
        {
            var ns = type.Namespace;
            var name = type.Name;
            return (ns.Length > 0 ? ns + "." : "") + name + ".bsml";
        }

        private string _resourceName;
        public override string Content 
        {
            get
            {
                if (_resourceName == null)
                {
                    var viewDef = GetType().GetCustomAttribute<ViewDefinitionAttribute>();
                    if (viewDef != null) _resourceName = viewDef.Definition;
                    else _resourceName = GetDefaultResourceName(GetType());
                }
                return Utilities.GetResourceContent(GetType().Assembly, _resourceName);
            } 
        }

        private readonly string hotReloadFrom;
        public BSMLAutomaticViewController() : base()
        {
            var hotReloadAttr = GetType().GetCustomAttribute<HotReloadAttribute>();
            if (hotReloadAttr == null) hotReloadFrom = null;
            else hotReloadFrom = Path.ChangeExtension(hotReloadAttr.Path, ".bsml");
        }
    }
}
