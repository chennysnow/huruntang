using System;
using System.IO;
using System.Text;
using System.Web;
using Bll;
using Comment;
using Mode;

namespace WebWeixin
{
    /// <summary>
    /// WxCallBack 的摘要说明
    /// </summary>
    public class WxCallBack : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            saveRequentInfo(context.Request);
  
            
            if (context.Request.HttpMethod == "GET")
            {
                string signature = context.Request["signature"];
                string timestamp = context.Request["timestamp"];
                string nonce = context.Request["nonce"];
                string echostr = context.Request["echostr"];
                var tempstring = WxBase.GetSignature(timestamp, nonce);
                //get method - 仅在微信后台填写URL验证时触发
                if (signature == tempstring)
                {
                    context.Response.Output.Write(echostr); //返回随机字符串则表示验证通过
                    LogServer.WriteLog("success" + context.Request.Url, "weixin");
                }
                else
                {
                    var msg = "failed:" + signature + "," + WxBase.GetSignature(timestamp, nonce);
                    context.Response.Output.Write(echostr);
                    LogServer.WriteLog("failed:"+ msg+ "\tRequesturl:" + context.Request.Url,"weixin");

                }

            }
            else if (context.Request.HttpMethod.ToLower() == "post")
            {
                Stream s = System.Web.HttpContext.Current.Request.InputStream;
                byte[] b = new byte[s.Length];
                s.Read(b, 0, (int) s.Length);
                string postStr = Encoding.UTF8.GetString(b);
                new WeixinApiServer().SaveWxMsg(postStr);

                LogServer.WriteLog(postStr, "wxpost");
            }
            else
            {
                //判断Post或其他方式请求
            }




        }
        public void saveRequentInfo(HttpRequest Request)
        {
            Stream s = HttpContext.Current.Request.InputStream;
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int)s.Length);
            string postStr = Encoding.UTF8.GetString(b);
            string postinfo = Request.Form.ToString();
            string tempquery = Request.QueryString.ToString();
            try
            {
                var requent = new RequestLog
                {
                    AcceptTypes = string.Join(";", Request.AcceptTypes ?? new string[1]),
                    AnonymousID = Request.AnonymousID,
                    ApplicationPath = Request.ApplicationPath,
                    Browser = postinfo,
                    ContentType = Request.ContentType,
                    Headers = Request.Headers.ToString(),
                    HttpMethod = Request.HttpMethod,
                    InputStream = postStr,
                    IsAuthenticated = Request.IsAuthenticated,
                    IsLocal = Request.IsLocal,
                    IsSecureConnection = Request.IsSecureConnection,
                    PhysicalApplicationPath = Request.PhysicalApplicationPath,
                    PhysicalPath = Request.PhysicalPath,
                    QueryString = tempquery,
                    RawUrl = Request.RawUrl,
                    RequestContext = Request.RequestContext.ToString(),
                    RequestType = Request.RequestType,
                    Url = Request.Url.ToString(),
                    UrlReferrer = Request.UrlReferrer?.ToString() ?? "",
                    UserAgent = Request.UserAgent,
                    UserHostAddress = Request.UserHostAddress,
                    UserHostName = Request.UserHostName,
                    UserLanguages = string.Join(";", Request.UserLanguages ?? new string[1]),
                    CreateDate = DateTime.Now

                };

                new RequestLogBll().AddLog(requent);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "RequestError");
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        //protected void getToken(HttpContext context)
        //{
   
        //    context.Response.Write(WeixinApiServer.Config.appID);


        //}
    }
}