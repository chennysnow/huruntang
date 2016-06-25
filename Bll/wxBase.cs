using Comment;
using System;
using System.Configuration;
using System.Net;
using DataBase;
using Mode;

namespace Bll
{
    public class WxBase
    {
        protected string RequestApiPost(string method, string param)
        {
            string url = string.Format(_requestUrlPostMode, method, Config.Token);
            var page = HtmlAnalysis.HttpRequestFromPost(url, param);
            if (page == "\"errcode\":0,\"errmsg\":\"ok\"}")
            {
                return "success";
            }
            return page;
        }

        protected string RequestApiGet(string method, string param)
        {
            string url = string.Format(_requestUrlPostMode, method, Config.Token);
            if (!string.IsNullOrEmpty(param))
            {
                url = url + "&" + param;
            }

            var page = HtmlAnalysis.Gethtmlcode(url);
            return page;
        }

        readonly string _requestUrlPostMode = "https://api.weixin.qq.com/cgi-bin/{0}?access_token={1}";

        private static WeiXinConfig _config;
        private static readonly object ConfigClock = new object();

        public static WeiXinConfig Config
        {
            get
            {
                lock (ConfigClock)
                {
                    if (_config == null || _config.StopTime < DateTime.Now.AddMinutes(10))
                    {
                        _config = new WeiXinConfig();
                        _config.appID = ConfigurationManager.AppSettings["WeiXin_appID"];

                        var tempconfig = new WeiXinConfigDB().GetFirstInfo(_config.appID);

                        if (tempconfig == null || tempconfig.StopTime < DateTime.Now.AddMinutes(10))
                        {
                            _config.appsecret = ConfigurationManager.AppSettings["WeiXin_appsecret"];
                            _config.URL = ConfigurationManager.AppSettings["WeiXin_CallBackURL"];
                            string url =
                                $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={_config.appID}&secret={_config.appsecret}";
                            var page = new WebClient().DownloadString(url);
                            _config.Token = ValidateBase.RegGroupsX<string>(page, "\"access_token\":\"(?<x>.*?)\"");
                            var time = ValidateBase.RegGroupsX<int>(page, "\"expires_in\":(?<x>\\d+)");
                            _config.StopTime = DateTime.Now.AddSeconds(time);

                            new WeiXinConfigDB().AddConfig(_config);
                        }
                        else
                        {
                            _config = tempconfig;
                        }
                    }
                    return _config;
                }


            }

        }
    }
}
