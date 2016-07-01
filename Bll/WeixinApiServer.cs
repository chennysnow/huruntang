using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Comment;
using DataBase;
using Mode;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.Entities;

namespace Bll
{
    public class WeixinApiServer: WxBase
    {
        public void CreateNemu()
        {
            string menu1 =
                "{\"button\":[{	\"type\":\"click\",\"name\":\"今日歌曲\",\"key\":\"V1001_TODAY_MUSIC\"},{\"name\":\"菜单\",\"sub_button\":[{\"type\":\"view\",\"name\":\"搜索\",\"url\":\"http://www.soso.com/\"},{\"type\":\"view\",\"name\":\"视频\",\"url\":\"http://v.qq.com/\"},{\"type\":\"click\",\"name\":\"赞一下我们\",\"key\":\"V1001_GOOD\"}]}]}";

            var result = RequestApiPost("menu/create", menu1);
        }
        public void GetNemu()
        {
            var result = RequestApiGet("menu/get","");
            
        }
        public void GetAllUser()
        {
            string userlist = RequestApiGet("user/get", "");
            var jusers = JObject.Parse(userlist);
            JToken jlist = jusers["data"]["openid"];
            foreach (JToken token in jlist)
            {
                GetUserInfo(token.ToString());
            }

        }
        public void GetUserInfo(string openid)
        {
            if (string.IsNullOrEmpty(openid))
                return;
            string userInfo = RequestApiPost("user/info", "openid="+ openid);
            var user = JsonConvert.DeserializeObject<WxUserinfo>(userInfo);
            new WxUserinfoDB().AddWxUser(user);

        }
        public void SaveWxMsg(string conten)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(conten);
                string json = JsonConvert.SerializeXmlNode(doc);
                var wx = JsonConvert.DeserializeObject<WxMessage>(json);
                new WxMessageDb().AddWxMessage(wx);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex);
            }
  
            
        }

    }
}
