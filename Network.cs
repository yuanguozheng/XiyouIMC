using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;

namespace imc
{
    public enum NetworkState
    {
        AVALIABLE,NEED_IMC,CRASHED
    }

    public delegate void NetworkCrashedEventHandler(NetworkInfo info);

    /// <summary>
    /// 每半分钟检测一次网络连通情况，若可用则休眠30秒，若被iMC限制则拨号，若网络不通则放弃本次拨号
    /// </summary>
    public class Network
    {
        public Network()
        {
            Thread thread = new Thread(DoVaild);
            thread.Start();
        }

        protected void DoVaild()
        {
            NetworkInfo state;
            while (true)
            {
                state = IsNetworkAvaliable();
                if (state.IsAvalible == NetworkState.NEED_IMC)
                {
                    OnNetworkCrashed(state);
                    Thread.Sleep(new TimeSpan(0, 0, 10));
                    continue;
                }
                else if (state.IsAvalible == NetworkState.CRASHED)
                {
                    OnNetworkCrashed(state);
                    Thread.Sleep(new TimeSpan(0, 0, 10));
                    continue;
                }
                else
                { 
                    Thread.Sleep(new TimeSpan(0, 0, 10));
                }
            }
        }

        public static NetworkCrashedEventHandler OnNetworkCrashed;
        /// <summary>
        /// 网络是否可用，当前情况如何
        /// </summary>
        /// <returns>含两个属性，IsAvalible用于说明网络是否正常连通，Message用于说明情况</returns>
        NetworkInfo IsNetworkAvaliable()
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://baidu.com");
            req.Timeout = 5000;
            try
            {
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                if (res.Headers["Connection"] == "close")
                {
                    Log.SaveLog(DateTime.Now + " 掉线");
                    return new NetworkInfo
                    {
                        IsAvalible = NetworkState.NEED_IMC,
                        Message = "Killed_By_iMC"
                    };
                }
            }
            catch
            {
                req = (HttpWebRequest)HttpWebRequest.Create("http://cn.bing.com");
                req.Timeout = 5000;
                try
                {
                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                }
                catch
                {
                    Log.SaveLog(DateTime.Now + " 网络异常");
                    return new NetworkInfo
                    {
                        IsAvalible = NetworkState.CRASHED,
                        Message = "Network_Crash"
                    };
                }
            }
            //Log.SaveLog(DateTime.Now + " 网络正常");
            return new NetworkInfo
            {
                IsAvalible = NetworkState.AVALIABLE,
                Message = "Avalible"
            };
        }



        public event NetworkCrashedEventHandler NetworkCrashed
        {
            add
            {
                OnNetworkCrashed += new NetworkCrashedEventHandler(value);
            }
            remove
            {
                OnNetworkCrashed -= new NetworkCrashedEventHandler(value);
            }
        }
    }
}
