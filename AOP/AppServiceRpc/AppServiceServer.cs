using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;

namespace XJK.AOP.AppServiceRpc
{
    public abstract class AppServiceServer : AppServiceBase
    {
        private BackgroundTaskDeferral AppServiceDeferral = null;

        public void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails)
            {
                AppServiceDeferral = args.TaskInstance.GetDeferral();
                args.TaskInstance.Canceled += TaskInstance_Canceled;
                if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails details)
                {
                    Connection = details.AppServiceConnection;
                    Log.Info("Connected [OnBackgroundActivated]");
                }
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            OnConnectionClosed(AppServiceClosedStatus.Canceled);
        }

        protected override void OnConnectionClosed(AppServiceClosedStatus status)
        {
            base.OnConnectionClosed(status);
            AppServiceDeferral.Complete();
        }
    }
}
