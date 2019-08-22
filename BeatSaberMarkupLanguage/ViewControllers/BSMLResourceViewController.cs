using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public abstract class BSMLResourceViewController : BSMLViewController
    {
        public abstract string ResourceName
        {
            get;
        }
        public override string Content {
            get
            {
                Assembly assembly = Assembly.GetCallingAssembly();
                
                using (Stream stream = assembly.GetManifestResourceStream(ResourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
