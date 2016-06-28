using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using Bll;
using Comment;
using Mode;

namespace WebWeixin 
{
    public partial class Weixin : System.Web.UI.Page
    {
        //微信请求类
        public class RequestXML
        {
            private string toUserName;
            /// <summary>
            /// 消息接收方微信号，一般为公众平台账号微信号
            /// </summary>
            public string ToUserName
            {
                get { return toUserName; }
                set { toUserName = value; }
            }

            private string fromUserName;
            /// <summary>
            /// 消息发送方微信号
            /// </summary>
            public string FromUserName
            {
                get { return fromUserName; }
                set { fromUserName = value; }
            }

            private string createTime;
            /// <summary>
            /// 创建时间
            /// </summary>
            public string CreateTime
            {
                get { return createTime; }
                set { createTime = value; }
            }

            private string msgType;
            /// <summary>
            /// 信息类型 地理位置:location,文本消息:text,消息类型:image
            /// </summary>
            public string MsgType
            {
                get { return msgType; }
                set { msgType = value; }
            }

            private string content;
            /// <summary>
            /// 信息内容
            /// </summary>
            public string Content
            {
                get { return content; }
                set { content = value; }
            }

            private string location_X;
            /// <summary>
            /// 地理位置纬度
            /// </summary>
            public string Location_X
            {
                get { return location_X; }
                set { location_X = value; }
            }

            private string location_Y;
            /// <summary>
            /// 地理位置经度
            /// </summary>
            public string Location_Y
            {
                get { return location_Y; }
                set { location_Y = value; }
            }

            private string scale;
            /// <summary>
            /// 地图缩放大小
            /// </summary>
            public string Scale
            {
                get { return scale; }
                set { scale = value; }
            }

            private string label;
            /// <summary>
            /// 地理位置信息
            /// </summary>
            public string Label
            {
                get { return label; }
                set { label = value; }
            }

            private string picUrl;
            /// <summary>
            /// 图片链接，开发者可以用HTTP GET获取
            /// </summary>
            public string PicUrl
            {
                get { return picUrl; }
                set { picUrl = value; }
            }

            private string _event;

            public string Event
            {
                get { return _event; }
                set { _event = value; }
            }
        }


        public void saveRequentInfo()
        {
            Stream s = System.Web.HttpContext.Current.Request.InputStream;
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int) s.Length);
            string postStr = Encoding.UTF8.GetString(b);

            string postinfo = "";
            string tempquery = "";
            try
            {
                int loop1, loop2;
                // Load NameValueCollection object.
                var coll = Request.QueryString;
                // Get names of all keys into a string array.
                String[] arr1 = coll.AllKeys;
                for (loop1 = 0; loop1 < arr1.Length; loop1++)
                {
                    string tempkey = Server.HtmlEncode(arr1[loop1]);
                    String[] arr2 = coll.GetValues(arr1[loop1]);
                    string tempvalue = "";
                    for (loop2 = 0; loop2 < arr2.Length; loop2++)
                    {
                        tempvalue += loop2 + ":" + Server.HtmlEncode(arr2[loop2]);
                    }

                    tempquery = tempquery + "{" + tempkey + "=" + tempvalue + "}";
                }

                var post = Request.Form.AllKeys;


                for (loop1 = 0; loop1 < post.Length; loop1++)
                {
                    string tempkey = Server.HtmlEncode(post[loop1]);
                    String[] arr2 = coll.GetValues(post[loop1]);
                    string tempvalue = "";
                    for (loop2 = 0; loop2 < arr2.Length; loop2++)
                    {
                        tempvalue += loop2 + ":" + Server.HtmlEncode(arr2[loop2]);
                    }

                    postinfo = postinfo + "{" + tempkey + "=" + tempvalue + "}";
                }



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


        protected void Page_Load(object sender, EventArgs e)
        {
            saveRequentInfo();

            if (Request.HttpMethod == "GET")
            {
                string signature = Request["signature"];
                string timestamp = Request["timestamp"];
                string nonce = Request["nonce"];
                string echostr = Request["echostr"];
                var tempstring = WxBase.GetSignature(timestamp, nonce);
                
                //get method - 仅在微信后台填写URL验证时触发
                if (signature== tempstring)
                {
                    WriteContent(echostr); //返回随机字符串则表示验证通过
                    LogServer.WriteLog("success"+Request.Url);
                }
                else
                {
                    WriteContent(echostr);
                    LogServer.WriteLog("failed:" + signature + "," + tempstring + Request.Url);
                }

                LogServer.WriteLog("signature:" + signature + "timestamp:" + timestamp+ "nonce:"+ nonce+ "echostr:"+ echostr);

            }
            string postStr = "";
            if (Request.HttpMethod.ToLower() == "post")
            {
                Stream s = System.Web.HttpContext.Current.Request.InputStream;
                byte[] b = new byte[s.Length];
                s.Read(b, 0, (int)s.Length);
                postStr = Encoding.UTF8.GetString(b);
                LogServer.WriteLog(postStr,"wxpost");
                return;
                if (!string.IsNullOrEmpty(postStr))
                {
                    //封装请求类
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(postStr);
                    XmlElement rootElement = doc.DocumentElement;
                    XmlNode MsgType = rootElement.SelectSingleNode("MsgType");
                    RequestXML requestXML = new RequestXML();
                    requestXML.ToUserName = rootElement.SelectSingleNode("ToUserName").InnerText;
                    requestXML.FromUserName = rootElement.SelectSingleNode("FromUserName").InnerText;
                    requestXML.CreateTime = rootElement.SelectSingleNode("CreateTime").InnerText;
                    requestXML.MsgType = MsgType.InnerText;
                    try
                    {
                        requestXML.Event = rootElement.SelectSingleNode("Event").InnerText;
                    }
                    catch
                    {

                    }


                    if (requestXML.MsgType == "text")
                    {
                        requestXML.Content = rootElement.SelectSingleNode("Content").InnerText;
                    }
                    else if (requestXML.MsgType == "location")
                    {
                        requestXML.Location_X = rootElement.SelectSingleNode("Location_X").InnerText;
                        requestXML.Location_Y = rootElement.SelectSingleNode("Location_Y").InnerText;
                        requestXML.Scale = rootElement.SelectSingleNode("Scale").InnerText;
                        requestXML.Label = rootElement.SelectSingleNode("Label").InnerText;
                    }
                    else if (requestXML.MsgType == "image")
                    {
                        requestXML.PicUrl = rootElement.SelectSingleNode("PicUrl").InnerText;
                    }
                    //else if (requestXML.MsgType == "Event") 
                    //{
                    //    // 事件类型  
                    //    String eventType = requestXML.Event;
                    //    // 订阅  
                    //    if (eventType == "Subscribe")
                    //    {

                    //        //Response.Write(GetMainMenuMessage(toUserName, fromUserName, "谢谢您的关注！,"));

                    //    }
                    //    // 取消订阅  
                    //    else if (eventType =="Unsubscribe")
                    //    {
                    //        // TODO 取消订阅后用户再收不到公众号发送的消息，因此不需要回复消息  
                    //    }
                    //    // 自定义菜单点击事件  
                    //    else if (eventType =="CLICK")
                    //    {
                    //        // TODO 自定义菜单权没有开放，暂不处理该类消息  
                    //    }  
                    //}
                    else if (requestXML.MsgType == "Event")
                    {
                        // 事件类型    
                        String eventType = requestXML.Event;
                        // 订阅    
                        if (eventType == "Subscribe")
                        {

                            //Response.Write(GetMainMenuMessage(toUserName, fromUserName, "谢谢您的关注！,"));
                            requestXML.Content = rootElement.SelectSingleNode("Content").InnerText;
                        }
                        // 取消订阅    
                        else if (eventType == "Unsubscribe")
                        {
                            // TODO 取消订阅后用户再收不到公众号发送的消息，因此不需要回复消息    
                        }
                        // 自定义菜单点击事件    
                        else if (eventType == "CLICK")
                        {
                            // TODO 自定义菜单权没有开放，暂不处理该类消息    
                        }
                    }

                    //回复消息
                    ResponseMsg(requestXML);
                }
            }
            else
            {
                Valid();
            }
            new WeixinApiServer().CreateNemu();

        }

        private void Valid()
        {
            //// 微信加密签名  
            //string signature = Request.QueryString["signature"];
            //// 时间戳  
            //string timestamp = Request.QueryString["timestamp"];
            //// 随机数  
            //string nonce = Request.QueryString["nonce"];
            //// 随机字符串  
            //string echostr = Request.QueryString["echostr"];
            //string echoStr = Request.QueryString["echoStr"];
            //if (CheckSignature())
            //{
            //    if (!string.IsNullOrEmpty(echoStr))
            //    {
            //        Response.Write(echoStr);
            //        Response.End();
            //    }
            //}
        }




        private void ResponseMsg(RequestXML requestXML)
        {
            try
            {
                string resxml = "";
                if (requestXML.MsgType == "text")
                {
                    resxml = @"<xml>
                        <ToUserName><![CDATA[" + requestXML.FromUserName + @"]]></ToUserName>
                        <FromUserName><![CDATA[" + requestXML.ToUserName + @"]]></FromUserName>
                        <CreateTime>" + ConvertDateTimeInt(DateTime.Now) + @"</CreateTime>
                        <MsgType><![CDATA[text]]></MsgType>
                        <Content><![CDATA[" + requestXML.FromUserName + "对" + requestXML.ToUserName + "说:hello word!" + @"]]></Content>
                        <FuncFlag>0</FuncFlag></xml>";
                }
                else if (requestXML.MsgType == "location")
                {
                    string city = GetMapInfo(requestXML.Location_X, requestXML.Location_Y);
                    if (city == "0")
                    {
                        resxml = "<xml><ToUserName><![CDATA[" + requestXML.FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + requestXML.ToUserName + "]]></FromUserName><CreateTime>" + ConvertDateTimeInt(DateTime.Now) + "</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[Sorry，没有找到" + city + " 的相关产品信息]]></Content><FuncFlag>0</FuncFlag></xml>";
                    }
                    else
                    {
                        resxml = "<xml><ToUserName><![CDATA[" + requestXML.FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + requestXML.ToUserName + "]]></FromUserName><CreateTime>" + ConvertDateTimeInt(DateTime.Now) + "</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[Sorry，这是 " + city + " 的产品信息：....]]></Content><FuncFlag>0</FuncFlag></xml>";
                    }
                }
                else if (requestXML.MsgType == "image")
                {
                    //返回10以内条
                    int size = 10;
                    resxml = "<xml><ToUserName><![CDATA[" + requestXML.FromUserName + "]]></ToUserName><FromUserName><![CDATA[" + requestXML.ToUserName + "]]></FromUserName><CreateTime>" + ConvertDateTimeInt(DateTime.Now) + "</CreateTime><MsgType><![CDATA[news]]></MsgType><Content><![CDATA[]]></Content><ArticleCount>" + size + "</ArticleCount><Articles>";
                    List<string> list = new List<string>();
                    //假如有20条查询的返回结果
                    for (int i = 0; i < 20; i++)
                    {
                        list.Add("1");
                    }
                    string[] piclist = new string[] { "/Abstract_Pencil_Scribble_Background_Vector_main.jpg", "/balloon_tree.jpg", "/bloom.jpg", "/colorful_flowers.jpg", "/colorful_summer_flower.jpg", "/fall.jpg", "/fall_tree.jpg", "/growing_flowers.jpg", "/shoes_illustration.jpg", "/splashed_tree.jpg" };

                    for (int i = 0; i < size && i < list.Count; i++)
                    {
                        resxml += "<item><Title><![CDATA[沈阳-黑龙江]]></Title><Description><![CDATA[元旦特价：￥300 市场价：￥400]]></Description><PicUrl><![CDATA[" + "http://www.hougelou.com" + piclist[i] + "]]></PicUrl><Url><![CDATA[http://www.hougelou.com]]></Url></item>";
                    }
                    resxml += "</Articles><FuncFlag>1</FuncFlag></xml>";
                }
                WriteTxt(resxml);
                Response.Write(resxml);
            }
            catch (Exception ex)
            {
                WriteTxt("异常：" + ex.Message + "Struck:" + ex.StackTrace.ToString());
            }
        }
        /// <summary>
        /// datetime转换为unixtime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// 记录bug，以便调试
        /// </summary>
        /// <returns></returns>
        public bool WriteTxt(string str)
        {
            try
            {
                FileStream fs = new FileStream(Server.MapPath("/bugLog.txt"), FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                //开始写入
                sw.WriteLine(str);
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                fs.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 调用百度地图，返回坐标信息
        /// </summary>
        /// <param name="y">经度</param>
        /// <param name="x">纬度</param>
        /// <returns></returns>
        public string GetMapInfo(string x, string y)
        {
            try
            {
                string res = string.Empty;
                string parame = string.Empty;
                string url = "http://maps.googleapis.com/maps/api/geocode/xml";
                parame = "latlng=" + x + "," + y + "&language=zh-CN&sensor=false";//此key为个人申请
                res = webRequestPost(url, parame);

                XmlDocument doc = new XmlDocument();

                doc.LoadXml(res);
                XmlElement rootElement = doc.DocumentElement;
                string Status = rootElement.SelectSingleNode("status").InnerText;
                if (Status == "OK")
                {
                    //仅获取城市
                    XmlNodeList xmlResults = rootElement.SelectSingleNode("/GeocodeResponse").ChildNodes;
                    for (int i = 0; i < xmlResults.Count; i++)
                    {
                        XmlNode childNode = xmlResults[i];
                        if (childNode.Name == "status")
                        {
                            continue;
                        }

                        string city = "0";
                        for (int w = 0; w < childNode.ChildNodes.Count; w++)
                        {
                            for (int q = 0; q < childNode.ChildNodes[w].ChildNodes.Count; q++)
                            {
                                XmlNode childeTwo = childNode.ChildNodes[w].ChildNodes[q];

                                if (childeTwo.Name == "long_name")
                                {
                                    city = childeTwo.InnerText;
                                }
                                else if (childeTwo.InnerText == "locality")
                                {
                                    return city;
                                }
                            }
                        }
                        return city;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteTxt("map异常:" + ex.Message.ToString() + "Struck:" + ex.StackTrace.ToString());
                return "0";
            }

            return "0";
        }

        /// <summary>
        /// Post 提交调用抓取
        /// </summary>
        /// <param name="url">提交地址</param>
        /// <param name="param">参数</param>
        /// <returns>string</returns>
        public string webRequestPost(string url, string param)
        {
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(param);

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url + "?" + param);
            req.Method = "Post";
            req.Timeout = 120 * 1000;
            req.ContentType = "application/x-www-form-urlencoded;";
            req.ContentLength = bs.Length;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Flush();
            }
            using (WebResponse wr = req.GetResponse())
            {
                //在这里对接收到的页面内容进行处理 

                Stream strm = wr.GetResponseStream();

                StreamReader sr = new StreamReader(strm, System.Text.Encoding.UTF8);

                string line;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                while ((line = sr.ReadLine()) != null)
                {
                    sb.Append(line + System.Environment.NewLine);
                }
                sr.Close();
                strm.Close();
                return sb.ToString();
            }
        }


    


    private void WriteContent(string str)
        {
            Response.Output.Write(str);
        }
    }
}