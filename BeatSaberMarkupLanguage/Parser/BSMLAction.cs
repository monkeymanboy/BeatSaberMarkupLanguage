using System.Reflection;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLAction
    {
        protected object host;
        internal MethodInfo methodInfo;
        public bool FromUIAction { get; internal set; }
        public string MemberName => methodInfo?.Name;
        public BSMLAction(object host, MethodInfo methodInfo, bool fromUiAction = true)
        {
            this.host = host;
            this.methodInfo = methodInfo;
            FromUIAction = fromUiAction;
        }

        public object Invoke(params object[] parameters)
        {
            return methodInfo.Invoke(host, parameters);
        }
    }
}
