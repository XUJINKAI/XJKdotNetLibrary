using System.Windows;

namespace Win32_Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.ShutdownMode = ShutdownMode.OnLastWindowClose;
            new WinMsgListenerWindow().Show();
            new SysXCmdRunWindow().Show();
        }
    }
}
