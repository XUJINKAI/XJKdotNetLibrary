using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;

namespace XJK.AOP.AppServiceRpc
{
    public abstract class AppServiceClient: AppServiceBase
    {
        public string PackageFamilyName { get; private set; }
        public string AppServiceName { get; private set; }
        
        public AppServiceClient(string packageName, string serverName)
        {
            PackageFamilyName = packageName;
            AppServiceName = serverName;
        }

        public async Task Connect()
        {
            if (!IsConnceted())
            {
                Log.Verbose("Connecting");
                var connect = new AppServiceConnection
                {
                    PackageFamilyName = PackageFamilyName,
                    AppServiceName = AppServiceName,
                };

                var status = await connect.OpenAsync();
                if (status == AppServiceConnectionStatus.Success)
                {
                    Connection = connect;
                }
                else
                {
                    connect.Dispose();
                }
                Log.Info($"AppService Connect {status}");
            }
            else
            {
                Log.Verbose("Already Connected");
            }
        }
    }
}
