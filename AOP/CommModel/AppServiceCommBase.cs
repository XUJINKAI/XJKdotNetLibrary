using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using XJK.MethodWrapper;
using XJK.ObjectExtension;

namespace XJK.CommunicationModel
{
    public abstract class AppServiceCommBase : RpcCommBase
    {
        public event Action Connected;
        public event Action<AppServiceClosedStatus> Closed;
        public event Action<AppServiceRequestReceivedEventArgs> Recived;

        protected abstract void DispatchInvoke(Action action);

        private AppServiceConnection _connection;
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
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        protected virtual void OnConnectionClosed(AppServiceConnection sender, AppServiceClosedStatus status)
        {
            DisposeConnection();
        }

        protected virtual void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var set = args.Request.Message;
            var methodinfo = set.ToMethodCall();
            DispatchInvoke(async () =>
            {
                await OnReceiveMethodCallAsync(methodinfo, async (response) =>
                {
                    var result = await args.Request.SendResponseAsync(response.ToValueSet());
                    return result.ToString();
                });
            });
        }

        protected async Task<bool> TryNewConnection(string AppServiceName, string PackageFamilyName, bool force = false)
        {
            if (force && IsConnceted())
            {
                DisposeConnection();
            }
            if (!IsConnceted())
            {
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
            }
            return IsConnceted();
        }

        public override bool IsConnceted()
        {
            return Connection != null;
        }

        protected virtual Task BeforeSendMessage(MethodCallInfo methodCallInfo)
        {
            return Task.FromResult<object>(null);
        }

        protected override async Task<MethodCallInfo> SendMessageAsync(MethodCallInfo methodCallInfo)
        {
            await BeforeSendMessage(methodCallInfo);
            var set = methodCallInfo.ToValueSet();
            var response = await Connection.SendMessageAsync(set);
            var result = response.Message.ToMethodCall();
            return result;
        }

    }
}
