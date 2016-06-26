using System.Web;
using Bll;
using Comment;

namespace WebWeixin
{
    /// <summary>
    /// WxCallBack 的摘要说明
    /// </summary>
    public class WxCallBack : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            LogServer.WriteLog(context.Request.UrlReferrer.ToString());
            return;
            context.Response.ContentType = "text/plain";
            getToken(context);



        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        protected void getToken(HttpContext context)
        {
   
            context.Response.Write(WeixinApiServer.Config.appID);


        }
    }
}