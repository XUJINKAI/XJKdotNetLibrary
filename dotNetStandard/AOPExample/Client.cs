﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XJK;
using XJK.AOP;
using XJK.Serializers;

namespace AOPExample
{
    public interface IClient
    {
        string ToBase64(string str);
        Task WriteLog(string str);
        Task<string> ToBase64Async(string str);
    }

    public class Client : IClient
    {
        public static IClient GetProxy()
        {
            var ClientInvoker = new ClientInvoker();
            return MethodProxy.CreateProxy<IClient>(ClientInvoker);
        }

        public string ToBase64(string str)
        {
            Thread.Sleep(2000);
            Log.Info("ToBase64");
            return str.ToBase64BinaryString();
        }

        public async Task<string> ToBase64Async(string str)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(2000);
                Log.Info("ToBase64Async");
                return Task.FromResult(str.ToBase64BinaryString());
            });
        }

        public async Task WriteLog(string str)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(2000);
                Log.Info("WriteLog: " + str);
            });
        }
    }
}