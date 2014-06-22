using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace imc
{
    public class IPInfo
    {
        public static string Host;
        public static string HostPort;
        public static string LocalIP;
        public IPInfo()
        {
            try
            {
                LoadXML();
            }
            catch
            {
                Log.SaveLog(DateTime.Now + " 加载IP信息失败");
            }
        }

        public static string GetFullHost()
        {
            return string.Format("{0}:{1}", Host, HostPort);
        }

        void LoadXML()
        {
            XmlTextReader reader = new XmlTextReader("Settings.xml");
            while (reader.Read())
            {
                if (reader.Name == "Server")
                {
                    Host = reader.GetAttribute("Host");
                    HostPort = reader.GetAttribute("Port");
                }
                if (reader.Name == "Local")
                {
                    LocalIP = reader.GetAttribute("Address");
                }
            }
            reader.Close();
        }
    }
}
