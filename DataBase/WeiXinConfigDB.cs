using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Comment;
using ServiceStack.OrmLite;
using Mode;

namespace DataBase
{
    public class WeiXinConfigDB : OrmLiteFactory
    {
        private static OrmLiteConnectionFactory _dbFactory;

        public WeiXinConfigDB()
        {
            _dbFactory = new OrmLiteConnectionFactory(ZnmDBConnectionString, SqlServerDialect.Provider);
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.CreateTable<WeiXinConfig>();
            }
        }

        public void AddConfig(WeiXinConfig con)
        {
            using (var db = _dbFactory.OpenDbConnection())
            {
                db.Save(con);
                //return db.Scalar<ClassInfo, int>(x => Sql.Max(x.Id));
            }

        }

        public WeiXinConfig GetFirstInfo(string appId)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                   return db.Single<WeiXinConfig>(x => x.appID==appId);
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex,"DBerror");
                return null;
            }
        }
    }
}
