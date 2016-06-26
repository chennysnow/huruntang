using System;
using Bll;
using Comment;

namespace WebWeixin 
{
    public partial class Weixin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string signature = Request["signature"];
            string timestamp = Request["timestamp"];
            string nonce = Request["nonce"];
            string echostr = Request["echostr"];

            if (Request.HttpMethod == "GET")
            {
                var tempstring = WxBase.GetSignature(signature, timestamp, nonce);
                LogServer.WriteLog(Request.UrlReferrer.ToString());
                //get method - 仅在微信后台填写URL验证时触发
                if (signature== tempstring)
                {
                    WriteContent(echostr); //返回随机字符串则表示验证通过
                }
                else
                {
                    WriteContent("failed:" + signature + "," + WxBase.GetSignature(timestamp, nonce));
                }

            }
            else
            {
                //判断Post或其他方式请求
            }

            Response.End();
            new WeixinApiServer().CreateNemu();

        }


        private void WriteContent(string str)
        {
            Response.Output.Write(str);
        }
    }
}