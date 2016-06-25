using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mode;
using System.Net;
using System.Configuration;
using Bll;

namespace WebWeixin 
{
    public partial class Weixin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            new WeixinApiServer().CreateNemu();

            //WeiXinConfig config = new WeiXinConfig();
            //config.appID = ConfigurationManager.AppSettings["WeiXin_appID"];// "wxd4bdcef094cca006";
            //config.appsecret = ConfigurationManager.AppSettings["WeiXin_appsecret"]; //"774da5284911073cb443e0f396ca7651";
            //config.URL = ConfigurationManager.AppSettings["WeiXin_CallBackURL"];

            //var db = new WeiXinConfigDB();
            //var tempconfig = db.GetFirstInfo(config.appID);

            //if (tempconfig.StopTime < DateTime.Now.AddMinutes(5))
            //{
            //    string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", config.appID, config.appsecret);
            //    var page = new WebClient().DownloadString(url);

            //    config.Token = ValidateBase.RegGroupsX<string>(page, "\"access_token\":\"(?<x>.*?)\"");
            //    var time = ValidateBase.RegGroupsX<int>(page, "\"expires_in\":(?<x>\\d+)");
            //    config.StopTime = DateTime.Now.AddSeconds(time);
            //    db.AddConfig(config);
            //}



        }
    }
}