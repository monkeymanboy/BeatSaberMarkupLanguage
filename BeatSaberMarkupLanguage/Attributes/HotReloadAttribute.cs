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
        public string[] Aliases { get; set; }

        private string _path = null;
        public string Path
        {
            get
            {
                if (_path == null)
                {
                    for (int i = 0; i < Aliases.Length; i += 2)
                    {
                        if (i + 1 >= Aliases.Length) break;
                        if (GivenPath.StartsWith(Aliases[i]))
                        {
                            _path = Aliases[i + 1] + GivenPath.Substring(Aliases[i].Length);
                            break;
                        }
                    }
                    if (_path == null) _path = GivenPath;
                }
                return _path;
            }
        }

        public HotReloadAttribute([CallerFilePath] string basePath = null)
            => GivenPath = basePath;
    }
}
