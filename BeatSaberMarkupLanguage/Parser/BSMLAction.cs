using System.Reflection;

namespace BeatSaberMarkupLanguage.Parser
{
    public class BSMLAction
    {
        private object host;
        private MethodInfo methodInfo;

        public BSMLAction(object host, MethodInfo methodInfo)
        {
            this.host = host;
            this.methodInfo = methodInfo;
        }

        public object Invoke(params object[] parameters)
        {
            return methodInfo.Invoke(host, parameters);
        }
    }
}
