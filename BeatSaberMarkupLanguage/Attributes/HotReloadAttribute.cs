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
        /// <summary>
        /// There should always be an even number of elements, where the first is the thing to map from, 
        /// and the second of each pair is the target.
        /// </summary>
        public string[] PathMap { get; set; }

        private string _path = null;
        public string Path
        {
            get
            {
                if (_path == null)
                {
                    if (PathMap != null)
                    {
                        for (int i = 0; i < PathMap.Length; i += 2)
                        {
                            if (i + 1 >= PathMap.Length) break;
                            if (GivenPath.StartsWith(PathMap[i]))
                            {
                                _path = PathMap[i + 1] + GivenPath.Substring(PathMap[i].Length);
                                break;
                            }
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
