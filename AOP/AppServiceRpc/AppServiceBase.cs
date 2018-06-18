using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;

namespace XJK.AOP.AppServiceRpc
{
    public abstract class AppServiceBase : IInvokerProxy
    {
        public event Action Connected;
        public event Action<AppServiceClosedStatus> Closed;
        public event Action<AppServiceRequestReceivedEventArgs> Recived;

        protected abstract void DispatchInvoke(Action action);
        protected abstract object GetExcuteObject();
        protected abstract void OnInvokeNoConnection();

        public bool IsConnceted() => Connection != null;

        private AppServiceConnection _connection = null;
        protected AppServiceConnection Connection
        {
            get => _connection;
            set
            {
                if (_connection == value)
                {
                    Log.Trace($"[AppServiceBase] set_Connection return because _connection == value;");
                    return;
                }
                if (_connection != null)
                {
                    _connection.RequestReceived -= _connection_RequestReceived;
                    _connection.ServiceClosed -= _connection_ServiceClosed;
                }
                _connection = value;
                if (_connection != null)
                {
                    _connection.ServiceClosed += _connection_ServiceClosed;
                    _connection.RequestReceived += _connection_RequestReceived;
                    OnConnectionConnected();
                    Log.Info("[AppServiceBase] New AppService Connection");
                }
            }
        }

        private void _connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            OnRequestReceived(sender, args);
        }

        private void _connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            OnConnectionClosed(args.Status);
        }

        public void DisposeConnection()
        {
            if (IsConnceted())
            {
                var t = Connection;
                Connection = null;
                t.Dispose();
                t = null;
                Log.Trace("[AppServiceBase] Connection Disposed");
            }
        }

        protected virtual void OnConnectionConnected()
        {
            Connected?.Invoke();
        }

        protected virtual void OnConnectionClosed(AppServiceClosedStatus status)
        {
            DisposeConnection();
            Closed?.Invoke(status);
        }

        protected async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Recived?.Invoke(args);
            var set = args.Request.Message;
            var methodCallInfo = set.ToMethodCall();
            var invoke_result = await methodCallInfo.InvokeAsync(GetExcuteObject());
            var response = new MethodCallInfo()
            {
                Name = methodCallInfo.Name,
                Result = invoke_result,
            };
            var re_set = response.ToValueSet();
            var response_status = await args.Request.SendResponseAsync(re_set);
            DispatchInvoke(() => { Log.Verbose(response_status); });
        }

        private async Task<MethodCallInfo> SendMessageAsync(MethodCallInfo methodCallInfo)
        {
            var set = methodCallInfo.ToValueSet();
            var response = await Connection.SendMessageAsync(set);
            var result = response.Message.ToMethodCall();
            return result;
        }

        public object Invoke(MethodInfo targetMethod, object[] args)
        {
            MethodCallInfo methodCall = new MethodCallInfo()
            {
                Name = targetMethod.Name,
                Args = new List<object>(args),
            };
            object result = SendMessageAsync(methodCall).ContinueWith(
                async task =>
                {
                    return (await task).Result;
                }).Unwrap();
            return result;
        }
        
        public void BeforeInvoke(object sender, BeforeInvokeEventArgs args)
        {
            if (!IsConnceted())
            {
                OnInvokeNoConnection();
                if (!IsConnceted())
                {
                    args.Handle();
                }
            }
        }

        public void AfterInvoke(object sender, AfterInvokeEventArgs args)
        {

        }
    }
}
