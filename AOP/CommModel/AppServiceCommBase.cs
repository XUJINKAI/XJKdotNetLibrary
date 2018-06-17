using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace XJK.AOP.CommunicationModel
{
    public abstract class AppServiceCommBase : RpcCommBase
    {
        public event Action Connected;
        public event Action<AppServiceClosedStatus> Closed;
        public event Action<AppServiceRequestReceivedEventArgs> Recived;

        protected abstract void DispatchInvoke(Action action);
        public override bool IsConnceted() => Connection != null;

        private AppServiceConnection _connection = null;
        protected AppServiceConnection Connection
        {
            get => _connection;
            set
            {
                if (_connection == value)
                {
                    Trace.TraceWarning($"[AppServiceCommBase] set_Connection: _connection == value;");
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
                    Debug.WriteLine("AppsService new Connection");
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
            Trace.TraceInformation($"[AppServiceCommBase] DisposeConnection, has connection: {IsConnceted()}");
            if (IsConnceted())
            {
                var t = Connection;
                Connection = null;
                t.Dispose();
                t = null;
                Trace.WriteLine("AppsService Connection Disposed");
            }
        }

        protected async Task<bool> TryNewConnection(string AppServiceName, string PackageFamilyName, bool force = false)
        {
            Trace.TraceInformation($"[AppServiceCommBase] TryNewConnection, IsConnceted  = {IsConnceted()}, force = {force}");
            if (force && IsConnceted())
            {
                DisposeConnection();
            }
            if (!IsConnceted())
            {
                Trace.TraceInformation("AppService Connecting...");
                Connection = new AppServiceConnection
                {
                    AppServiceName = AppServiceName,
                    PackageFamilyName = PackageFamilyName,
                };
                AppServiceConnectionStatus status = await Connection.OpenAsync();
                if (status != AppServiceConnectionStatus.Success)
                {
                    DisposeConnection();
                }
                Trace.TraceInformation($"AppService Connection status [{status}]");
            }
            return IsConnceted();
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

        protected virtual void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Recived?.Invoke(args);
            var set = args.Request.Message;
            var methodinfo = set.ToMethodCall();
            DispatchInvoke(async () =>
            {
                async Task<string> response_func(MethodCallInfo response)
                {
                    Log.Debug($"[AppServiceCommBase.OnRequestReceived] response_func({response.Dump()})");
                    var re_set = response.ToValueSet();
                    Log.Debug($"[AppServiceCommBase.OnRequestReceived] response set:{C.LF}{set.Dump()}");
                    var result = await args.Request.SendResponseAsync(re_set);
                    Log.Debug($"[AppServiceCommBase.OnRequestReceived] response result '{result}'");
                    return result.ToString();
                }
                await OnReceiveMethodCallAsync(methodinfo, response_func);
            });
        }

        protected override async Task<MethodCallInfo> SendMessageAsync(MethodCallInfo methodCallInfo)
        {
            ValueSet set = methodCallInfo.ToValueSet();
            AppServiceResponse response = await Connection.SendMessageAsync(set);
            MethodCallInfo result = response.Message.ToMethodCall();
            if (response.Status != AppServiceResponseStatus.Success)
            {
                Log.Error($"SendMessageAsync {response.Status}");
                Debugger.Break();
            }
            else if (!response.Message.ContainsKey(ValueSetExtension.FuncMethodBase64Key))
            {
                Log.Error($"SendMessageAsync Response Message ValueSet void");
                Debugger.Break();
            }
            return result;
        }

    }
}
