using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;

namespace XJK.AOP.CommunicationModel
{
    public abstract class AppServiceCommBase : RpcCommBase
    {
        public event Action Connected;
        public event Action<AppServiceClosedStatus> Closed;
        public event Action<AppServiceRequestReceivedEventArgs> Recived;

        protected abstract void DispatchInvoke(Action action);

        private AppServiceConnection _connection = null;
        protected AppServiceConnection Connection
        {
            get => _connection;
            set
            {
                if (_connection != null)
                {
                    _connection.RequestReceived -= _connection_RequestReceived;
                    _connection.ServiceClosed -= _connection_ServiceClosed;
                    _connection.Dispose();
                }
                _connection = value;
                if (_connection != null)
                {
                    _connection.RequestReceived += _connection_RequestReceived;
                    _connection.ServiceClosed += _connection_ServiceClosed;
                    Connected?.Invoke();
                    Debug.WriteLine("AppsService new Connection");
                }
            }
        }

        private void _connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            OnRequestReceived(sender, args);
            Recived?.Invoke(args);
        }

        private void _connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            OnConnectionClosed(sender, args.Status);
            Closed?.Invoke(args.Status);
        }

        public void DisposeConnection()
        {
            Trace.TraceInformation($"[AppServiceCommBase] DisposeConnection, has connection: {_connection != null}");
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
                Trace.WriteLine("AppsService Connection Disposed");
            }
        }

        protected virtual void OnConnectionClosed(AppServiceConnection sender, AppServiceClosedStatus status)
        {
            DisposeConnection();
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

        public override bool IsConnceted()
        {
            return Connection != null;
        }

        protected virtual void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Log.TagDebugger();
            var set = args.Request.Message;
            Log.TagDebugger();
            var methodinfo = set.ToMethodCall();
            Log.TagDebugger();
            DispatchInvoke(async () =>
            {
                async Task<string> response_func(MethodCallInfo response)
                {
                    Log.TagDebugger();
                    var result = await args.Request.SendResponseAsync(response.ToValueSet());
                    Log.TagDebugger();
                    return result.ToString();
                }
                Log.TagDebugger();
                await OnReceiveMethodCallAsync(methodinfo, response_func);
            });
        }

        protected override async Task<MethodCallInfo> SendMessageAsync(MethodCallInfo methodCallInfo)
        {
            var set = methodCallInfo.ToValueSet();
            Log.TagDebugger();
            var response = await Connection.SendMessageAsync(set);
            Log.TagDebugger();
            var result = response.Message.ToMethodCall();
            Log.TagDebugger();
            return result;
        }

    }
}
