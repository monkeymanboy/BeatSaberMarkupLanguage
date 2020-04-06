using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using IPA.Utilities;

namespace BeatSaberMarkupLanguage.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Conditional("DEBUG"), Conditional("USE_HOT_RELOAD")]
    public sealed class HotReloadAttribute : Attribute
    {
        public string GivenPath { get; }
        public string[][] Aliases { get; set; }

        private string _path = null;
        public string Path
        {
            get
            {
                if (_path == null)
                {
                    foreach (var alias in Aliases)
                    {
                        if (alias.Length < 2) continue;
                        if (GivenPath.StartsWith(alias[0]))
                        {
                            _path = alias[1] + GivenPath.Substring(alias[0].Length);
                            break;
                        }
                    }
                }
                return _path;
            }
        }

        public HotReloadAttribute([CallerFilePath] string basePath = null)
            => GivenPath = basePath;
    }
}
