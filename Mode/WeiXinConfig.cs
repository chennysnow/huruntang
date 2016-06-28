using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class WeiXinConfig
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public  string appID { get; set; }
        public  string appsecret { get; set; }
        public  string URL { get; set; }
        public  string AccessToken { get; set; }
        public string WebToken { get; set; }
        public DateTime StopTime { get; set; }
    }
}
