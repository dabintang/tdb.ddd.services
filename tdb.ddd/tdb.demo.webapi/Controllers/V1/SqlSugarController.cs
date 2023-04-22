using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Diagnostics;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.repository.sqlsugar;
using tdb.ddd.webapi;
using tdb.demo.webapi.DB;

namespace tdb.demo.webapi.Controllers.V1
{
    /// <summary>
    /// 尝试使用SqlSugar
    /// </summary>
    [TdbApiVersion(1)]
    [AllowAnonymous]
    public class SqlSugarController : BaseController
    {
        #region 数据库准备

        //1.创建数据库：tdb.ddd.demo

        //2.创建表：table1
        /*
DROP TABLE IF EXISTS `table1`;
CREATE TABLE `table1`  (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(24) NOT NULL,
  PRIMARY KEY (`ID`) USING BTREE
) ;
         */

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlSugarController()
        {
        }

        #region 接口

        /// <summary>
        /// 清空表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public TdbRes<bool> Truncate()
        {
            var repo = new Table1Repository();
            repo.Truncate();
            return TdbRes.Success(true);
        }

        /// <summary>
        /// 不开事务的情况
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public TdbRes<bool> NoTran()
        {
            var info = new Table1()
            {
                ID = 1,
                Name = "名称1"
            };

            var repo1 = new Table1Repository();
            repo1.InsertOrUpdate(info);

            var repo2 = new Table1Repository();
            var infoRead = repo2.GetById(info.ID);
            TdbDemoAssert.Equals(info.Name, infoRead.Name);

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 开启事务并提交的情况
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public TdbRes<bool> UseTranAndCommit()
        {
            var info = new Table1()
            {
                ID = 2,
                Name = "名称2"
            };

            TdbRepositoryTran.BeginTranOnAsyncFunc();

            var repo1 = new Table1Repository();
            repo1.InsertOrUpdate(info);

            var repo2 = new Table1Repository();
            var info2 = repo2.GetById(info.ID);
            TdbDemoAssert.Equals(info.Name, info2.Name);
            info2.Name = "名称22";
            repo2.Update(info2);

            TdbRepositoryTran.CommitTran();

            var repo3 = new Table1Repository();
            var info3 = repo3.GetById(info.ID);
            TdbDemoAssert.Equals(info3.Name, info2.Name);

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 开启事务并回滚的情况
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public TdbRes<bool> UseTranAndRollback()
        {
            var info = new Table1()
            {
                ID = 3,
                Name = "名称3"
            };

            TdbRepositoryTran.BeginTranOnAsyncFunc();

            var repo1 = new Table1Repository();
            repo1.InsertOrUpdate(info);

            var repo2 = new Table1Repository();
            var info2 = repo2.GetById(info.ID);
            TdbDemoAssert.Equals(info.Name, info2.Name);
            info2.Name = "名称33";
            repo2.Update(info2);

            var repo3 = new Table1Repository();
            var info3 = repo3.GetById(info.ID);
            TdbDemoAssert.Equals(info3.Name, info2.Name);

            TdbRepositoryTran.RollbackTran();

            var repo4 = new Table1Repository();
            var info4 = repo4.GetById(info.ID);
            TdbDemoAssert.Equals(null, info4);

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 开启多次事务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public TdbRes<bool> UseTranManyTimes()
        {
            var info = new Table1()
            {
                ID = 10,
                Name = "名称10"
            };

            var repo = new Table1Repository();

            for (int i = 0; i < 3; i++)
            {
                TdbRepositoryTran.BeginTranOnAsyncFunc();
                info.Name = $"名称{i + 10}";
                repo.InsertOrUpdate(info);
                TdbRepositoryTran.CommitTran();
            }

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 同一请求下是否用的同一个数据库连接（是）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<TdbRes<bool>> IsSameDBConnectAsync()
        {
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            await PrintRepositoryContextID();
            await PrintRepositoryContextID();
            await PrintRepositoryContextID();

            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 同一请求下是否用的同一个数据库连接2（是）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public TdbRes<bool> IsSameDBConnect2()
        {
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            var task1 = PrintRepositoryContextID();
            var task2 = PrintRepositoryContextID();
            var task3 = PrintRepositoryContextID();

            Task.WhenAll(task1, task2, task3);

            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 同一请求下是否用的同一个数据库连接3（不是）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public TdbRes<bool> IsSameDBConnect3()
        {
            //Parallel.For(0, 10, (index, state) =>
            //{
            //    PrintRepositoryContextID3();
            //});

            Parallel.Invoke(() => { PrintRepositoryContextID3(); }, () => { PrintRepositoryContextID3(); });

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 同一请求下是否用的同一个数据库连接4（同一次方法调用内：是）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public TdbRes<bool> IsSameDBConnect4()
        {
            //Parallel.For(0, 10, (index, state) =>
            //{
            //    PrintRepositoryContextID4();
            //});

            Parallel.Invoke(async () => { await PrintRepositoryContextID4(); }, async () => { await PrintRepositoryContextID4(); });

            return TdbRes.Success(true);
        }


        /// <summary>
        /// 
        /// </summary>
        private static void PrintRepositoryContextID3()
        {
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            var task1 = PrintRepositoryContextID();
            var task2 = PrintRepositoryContextID();
            var task3 = PrintRepositoryContextID();

            Task.WhenAll(task1, task2, task3);

            TdbRepositoryTran.CommitTran();
        }


        /// <summary>
        /// 
        /// </summary>
        private static async Task PrintRepositoryContextID4()
        {
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            await PrintRepositoryContextID();
            await PrintRepositoryContextID();
            await PrintRepositoryContextID();

            TdbRepositoryTran.CommitTran();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task PrintRepositoryContextID()
        {
            await Task.Delay(300);
            //await Task.CompletedTask;
            var repo = new Table1Repository();
            repo.GetContextID();
            //await Task.CompletedTask;
        }

        #endregion
    }
}
