using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace imc
{    
    /// <summary>
    /// iMC拨号类
    /// </summary>
    public class iMCPortal
    {       
        public void StartiMCPortalThread()
        {
            Thread thread = new Thread(DoRequest);
            thread.Start();
        }
        /// <summary>
        /// 每半分钟检测一次网络连通情况，若可用则放弃本次拨号，并休眠30秒，若被iMC限制则拨号，若网络不通则放弃本次拨号
        /// </summary>
        public void DoRequest()
        {
            int i = Accounts.i;
            if (i == -1)
            {
                Accounts.i = 0;
                i = 0;
            }
            while(true)
            {
                string username = Accounts.AccountsData[i].UserName;
                string despassword = Accounts.AccountsData[i].Password;
                if(DoVefify(username, despassword))
                {
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                }
                else
                {
                    i++;
                    i = i % Accounts.AccountsData.Count;
                    Accounts.i = i;
                }
            }
        }

        public void DoSingleRequest()
        {
            Accounts.i = Accounts.i % Accounts.AccountsData.Count;
            string username = Accounts.AccountsData[Accounts.i].UserName;
            string despassword = Accounts.AccountsData[Accounts.i].Password;
            DoVefify(username, despassword);
        }

        /// <summary>
        /// 构建拨号认证的HTTP请求
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="despassword">经过Javascript加密后的用户密码</param>
        /// <param name="hello3">Cookies之一，经过简单加密</param>
        bool DoVefify(string username, string despassword)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://" + IPInfo.GetFullHost() + "/portal/pws?t=li");
            req.CookieContainer = new CookieContainer();
            CookieContainer cookiecontainer = new CookieContainer();

            cookiecontainer.SetCookies(new Uri("http://" + IPInfo.GetFullHost() + "/portal/pws?t=li"), "hello1=;");
            cookiecontainer.SetCookies(new Uri("http://" + IPInfo.GetFullHost() + "/portal/pws?t=li"), "hello2=;");
            cookiecontainer.SetCookies(new Uri("http://" + IPInfo.GetFullHost() + "/portal/pws?t=li"), "hello3=;");
            cookiecontainer.SetCookies(new Uri("http://" + IPInfo.GetFullHost() + "/portal/pws?t=li"), "hello4=;");
            cookiecontainer.SetCookies(new Uri("http://" + IPInfo.GetFullHost() + "/portal/pws?t=li"), "i_p_pl=;");

            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.CookieContainer = cookiecontainer;
            req.UserAgent = "SB! Fuck You!";
            req.Headers["Accept-Language"] = "Mars Language";

            string Params =
                "userName=" + username + "&userPwd=" + despassword
                + "&serviceType=&isSavePwd=on&userurl=http%3A%2F%2Fxupt.edu.cn&userip="
                + IPInfo.LocalIP + "&basip=&language=Chinese&portalProxyIP="
                + IPInfo.Host + "&portalProxyPort=50200&dcPwdNeedEncrypt=1&assignIpType=0&appRootUrl=http%3A%2F%2F" 
                + IPInfo.Host + "%3A" 
                + IPInfo.HostPort + "%2Fportal%2F&manualUrl=&manualUrlEncryptKey=rTCZGLy2wJkfobFEj0JF8A%3D%3D";

            using (Stream stream = req.GetRequestStream())
            {
                byte[] bin = Encoding.UTF8.GetBytes(Params);
                stream.Write(bin, 0, bin.Length);
            }

            string resStr = "";
            try
            {
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                using (Stream stream = res.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    resStr = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Log.SaveLog(DateTime.Now + " 认证异常" + " 账号：" + username + " 错误：" + e.Message);
                //Console.WriteLine(DateTime.Now + " 认证异常" + " 账号：" + username + " 错误：" + e.Message);
                return false;
            }
            if (!string.IsNullOrEmpty(resStr))
            {
                if(resStr.Contains("portServIncludeFailedCode"))
                {
                    Log.SaveLog(DateTime.Now + " 认证失败" + " 账号：" + username);
                    return false;
                }
                else
                {
                    //Log.SaveLog(DateTime.Now + " 认证成功" + " 账号：" + username);
                    return true;
                }
                //Console.WriteLine(DateTime.Now + " 认证成功" + " 账号：" + username);
            }
            else
            {
                Log.SaveLog(DateTime.Now + " 认证失败" + " 账号：" + username);
                return false;
                //Console.WriteLine(DateTime.Now + " 认证失败" + " 账号：" + username);
            }
        }
    }
}
