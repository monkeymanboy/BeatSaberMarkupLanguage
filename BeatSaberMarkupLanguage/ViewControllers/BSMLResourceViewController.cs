using System.Reflection;

namespace BeatSaberMarkupLanguage.ViewControllers
{
    public abstract class BSMLResourceViewController : BSMLViewController
    {
        public abstract string ResourceName { get; }

        public override string Content
        {
            get => Utilities.GetResourceContent(Assembly.GetAssembly(this.GetType()), ResourceName);
        }
    }
}
