using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberMarkupLanguage.Extensions
{
    public delegate void PresentFlowCoordinatorDelegate(FlowCoordinator current, FlowCoordinator flowCoordinator, Action finishedCallback = null, bool immediately = false, bool replaceTopViewController = false);

    public static class UIExtensions
    {
        public static void PresentFlowCoordinator(this FlowCoordinator current, FlowCoordinator flowCoordinator, Action finishedCallback = null, bool immediately = false, bool replaceTopViewController = false)
        {
            PresentFlowCoordinatorDelegate(current, flowCoordinator, finishedCallback, immediately, replaceTopViewController);
        }

        #region Delegate Creation
        private static PresentFlowCoordinatorDelegate _presentFlowCoordinatorDelegate;
        private static PresentFlowCoordinatorDelegate PresentFlowCoordinatorDelegate
        {
            get
            {
                if (_presentFlowCoordinatorDelegate == null)
                {
                    var presentMethod = typeof(FlowCoordinator).GetMethod("PresentFlowCoordinator", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    _presentFlowCoordinatorDelegate = (PresentFlowCoordinatorDelegate)Delegate.CreateDelegate(typeof(PresentFlowCoordinatorDelegate), presentMethod);
                }
                return _presentFlowCoordinatorDelegate;
            }
        }
        #endregion
    }
}
