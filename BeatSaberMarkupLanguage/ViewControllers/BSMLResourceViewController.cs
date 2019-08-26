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
                return Utilities.GetResourceContent(Assembly.GetAssembly(this.GetType()), ResourceName);
            }
        }
    }
}
