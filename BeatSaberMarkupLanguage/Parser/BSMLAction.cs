using System.Reflection;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLAction
    {
        protected object host;

        public BSMLAction(object host, MethodInfo methodInfo, bool fromUiAction = true)
        {
            this.host = host;
            this.MethodInfo = methodInfo;
            FromUIAction = fromUiAction;
        }

        public bool FromUIAction { get; internal set; }

        public string MemberName => MethodInfo?.Name;

        internal MethodInfo MethodInfo { get; set; }

        public object Invoke(params object[] parameters)
        {
            return MethodInfo.Invoke(host, parameters);
        }
    }
}
