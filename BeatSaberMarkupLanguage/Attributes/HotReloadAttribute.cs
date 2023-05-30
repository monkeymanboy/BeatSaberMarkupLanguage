using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BeatSaberMarkupLanguage.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Conditional("DEBUG")]
    [Conditional("USE_HOT_RELOAD")]
    public sealed class HotReloadAttribute : Attribute
    {
        public string GivenPath { get; }

        /// <summary>
        /// Gets or sets the path map. There should always be an even number of elements, where the first is the thing to map from, and the second of each pair is the target.
        /// </summary>
        public string[] PathMap { get; set; }

        /// <summary>
        /// Gets or sets the path to the layout (BSML) file relative to the path of class in which the attribute is being used.
        /// </summary>
        public string RelativePathToLayout { get; set; }

        private string path = null;

        public string Path
        {
            get
            {
                if (path == null)
                {
                    if (PathMap != null)
                    {
                        for (int i = 0; i < PathMap.Length; i += 2)
                        {
                            if (i + 1 >= PathMap.Length)
                            {
                                break;
                            }

                            if (GivenPath.StartsWith(PathMap[i], StringComparison.Ordinal))
                            {
                                path = PathMap[i + 1] + GivenPath.Substring(PathMap[i].Length);
                                break;
                            }
                        }
                    }

                    path ??= RelativePathToLayout != null
                        ? System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(GivenPath) + System.IO.Path.DirectorySeparatorChar + RelativePathToLayout)
                        : GivenPath;
                }

                return path;
            }
        }

        public HotReloadAttribute([CallerFilePath] string basePath = null)
            => GivenPath = basePath;
    }
}
