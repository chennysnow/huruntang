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
            try
            {
                _dbFactory = new OrmLiteConnectionFactory(ZnmDBConnectionString, SqlServerDialect.Provider);
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.CreateTable<WeiXinConfig>();
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBerror");
            }
        }

        public void AddConfig(WeiXinConfig con)
        {
            try
            {
                using (var db = _dbFactory.OpenDbConnection())
                {
                    db.Save(con);
                    //return db.Scalar<ClassInfo, int>(x => Sql.Max(x.Id));
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex, "DBerror");
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
