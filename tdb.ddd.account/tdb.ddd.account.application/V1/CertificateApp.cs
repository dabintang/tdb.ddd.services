using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.application.contracts.V1.DTO.Certificate;
using tdb.ddd.account.application.contracts.V1.DTO;
using tdb.ddd.account.application.contracts.V1.Interface;
using tdb.ddd.contracts;
using tdb.ddd.account.domain.Certificate;
using tdb.ddd.account.infrastructure.Config;
using tdb.ddd.account.domain.User;
using tdb.ddd.infrastructure;
using tdb.ddd.application.contracts;
using tdb.ddd.account.domain.Certificate.Aggregate;
using tdb.ddd.account.infrastructure;
using tdb.ddd.repository.sqlsugar;
using tdb.ddd.account.domain.User.Aggregate;

namespace tdb.ddd.account.application.V1
{
    /// <summary>
    /// 凭证应用
    /// </summary>
    public class CertificateApp : ICertificateApp
    {
        #region 实现接口

        /// <summary>
        /// 凭证登录
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<UserLoginRes>> LoginAsync(CertificateLoginReq req)
        {
            //凭证领域服务
            var certService = new CertificateService();
            //获取凭证聚合
            var certAgg = await certService.GetCertificateAggAsync(req.CertificateTypeCode, req.Credentials);
            if (certAgg is null)
            {
                return new TdbRes<UserLoginRes>(AccountConfig.Msg.CertificateNotExist, null);
            }

            //获取凭证绑定用户ID
            var userID = await certAgg.GetUserIDAsync();
            if (userID is null)
            {
                return new TdbRes<UserLoginRes>(AccountConfig.Msg.CertificateLoginFailed, null);
            }

            //用户领域服务
            var userService = new UserService();
            //获取用户聚合
            var userAgg = await userService.GetAsync(userID.Value);
            if (userAgg is null)
            {
                return new TdbRes<UserLoginRes>(AccountConfig.Msg.CertificateLoginFailed, null);
            }

            //用户应用
            var userApp = TdbIOC.GetService<IUserApp>();
            //登录
            var res = await userApp!.LoginAsync(new UserLoginReq()
            {
                LoginName = userAgg.LoginName,
                Password = userAgg.Password,
                ClientIP = req.ClientIP
            });

            return res;
        }

        /// <summary>
        /// 添加凭证
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> AddCertificateAsync(TdbOperateReq<AddCertificateReq> req)
        {
            var param = req.Param;

            //凭证领域服务
            var certService = new CertificateService();
            //获取凭证聚合
            var certAgg = await certService.GetCertificateAggAsync(param.CertificateTypeCode, param.Credentials);
            if (certAgg is not null)
            {
                return new TdbRes<bool>(AccountConfig.Msg.CertificateExist, false);
            }

            //生成凭证聚合
            var agg = new CertificateAgg(param.CertificateTypeCode, param.Credentials, req.OperatorID, req.OperationTime)
            {
                ID = AccountUniqueIDHelper.CreateID()
            };

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //保存
            await agg.SaveAsync();
            await agg.BindingUserAsync(req.OperatorID);

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 删除凭证
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> DeleteCertificateAsync(TdbOperateReq<DeleteCertificateReq> req)
        {
            var param = req.Param;

            //凭证领域服务
            var certService = new CertificateService();
            //获取凭证聚合
            var certAgg = await certService.GetCertificateAggAsync(param.CertificateTypeCode, param.Credentials);
            if (certAgg is null)
            {
                return TdbRes.Success(true);
            }

            //删除凭证
            await certAgg.DeleteAsync();

            return TdbRes.Success(true);
        }

        #endregion
    }
}
