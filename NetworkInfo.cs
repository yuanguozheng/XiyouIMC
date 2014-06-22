using System;
using System.Collections.Generic;
using System.Text;

namespace imc
{
    /// <summary>
    /// 网络状态信息
    /// </summary>
    public class NetworkInfo
    {
        public NetworkState IsAvalible { get; set; }
        public string Message { get; set; }
    }

}
