using System.Reflection;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLAction
    {
        public BSMLAction(object host, MethodInfo methodInfo, bool fromUiAction = true)
        {
            this.Host = host;
            this.MethodInfo = methodInfo;
            FromUIAction = fromUiAction;
        }

        public bool FromUIAction { get; internal set; }

        public string MemberName => MethodInfo?.Name;

        internal MethodInfo MethodInfo { get; set; }

        protected object Host { get; }

        public object Invoke(params object[] parameters)
        {
            return MethodInfo.Invoke(Host, parameters);
        }
    }
}
