using System.Web;
using Bll;

namespace WebWeixin
{
    /// <summary>
    /// WxCallBack 的摘要说明
    /// </summary>
    public class WxCallBack : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
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