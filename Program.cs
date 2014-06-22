using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace imc
{
    class Program
    {
        public static string FILE_NAME;
        static void Main(string[] args)
        {
            FILE_NAME = "Log_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            Accounts acc = new Accounts(); //加载用户信息
            IPInfo ip = new IPInfo();
            Network net = new Network();
            net.NetworkCrashed += net_NetworkCrashed;
            iMCPortal imcclient = new iMCPortal();  //实例化iMC拨号认证类
            imcclient.StartiMCPortalThread();
        }

        static void net_NetworkCrashed(NetworkInfo info)
        {
            Accounts.i++;
            iMCPortal imc = new iMCPortal();
            imc.DoSingleRequest();
        }
    }

}
