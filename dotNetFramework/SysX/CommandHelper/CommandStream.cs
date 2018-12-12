using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XJK.SysX.CommandHelper
{
    public class CommandStreamAsync : CommandStreamBase
    {
        public event Action<string> ReceiveOutputData;
        public event Action<string> ReceiveErrorData;

        public CommandStreamAsync(string cmd, string arg, Action<string> output = null, Action<string> error = null)
            : base(cmd, arg)
        {
            Process.OutputDataReceived += (sender, e) =>
            {
                ReceiveOutputData?.Invoke(e.Data + Environment.NewLine);
            };
            Process.ErrorDataReceived += (sender, e) =>
            {
                ReceiveErrorData?.Invoke(e.Data + Environment.NewLine);
            };
            Receive(output, error);
            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();
        }

        public void Receive(Action<string> Output, Action<string> Error)
        {
            ReceiveOutputData += Output;
            ReceiveErrorData += Error;
        }

    }

    public class CommandStreamSynchronous: CommandStreamBase
    {
        public readonly StreamReader Output;
        public readonly StreamReader Error;
        
        public CommandStreamSynchronous(string cmd, string arg)
            :base(cmd, arg)
        {
            Output = Process.StandardOutput;
            Error = Process.StandardError;
        }
    }

    public abstract class CommandStreamBase : IDisposable
    {
        protected readonly ProcessStartInfo ProcessStartInfo;
        protected readonly Process Process;
        protected readonly StreamWriter StandardInput;

        protected CommandStreamBase(string cmd, string arg)
        {
            ProcessStartInfo = new ProcessStartInfo()
            {
                FileName = cmd,
                Arguments = arg,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };
            Process = Process.Start(ProcessStartInfo);
            StandardInput = Process.StandardInput;
        }
        
        public void Write(string str) => StandardInput.Write(str);
        public void WriteLine(string str) => StandardInput.WriteLine(str);

        #region Dispose
        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Process.Dispose();
                handle.Dispose();
            }

            disposed = true;
        }
        #endregion
    }
}
