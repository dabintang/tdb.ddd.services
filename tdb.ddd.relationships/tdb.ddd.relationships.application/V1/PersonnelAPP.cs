using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.relationships.application.contracts.V1.DTO;
using tdb.ddd.relationships.application.contracts.V1.Interface;
using tdb.ddd.relationships.domain.Personnel;
using tdb.ddd.relationships.domain.Personnel.Aggregate;
using tdb.ddd.relationships.infrastructure;
using tdb.ddd.relationships.infrastructure.Config;
using tdb.ddd.repository.sqlsugar;
using static tdb.ddd.contracts.TdbCst;

namespace tdb.ddd.relationships.application.V1
{
    /// <summary>
    /// 人员应用
    /// </summary>
    public class PersonnelAPP : IPersonnelAPP
    {
        #region 实现接口

        /// <summary>
        /// 添加人员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新人员ID</returns>
        public async Task<TdbRes<AddPersonnelRes>> AddPersonnelAsync(TdbOperateReq<AddPersonnelReq> req)
        {
            //参数
            var param = req.Param;

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //生成人员ID
            var personnelID = RelationshipsUniqueIDHelper.CreateID();
            //人员聚合
            var personnelAgg = new PersonnelAgg()
            {
                ID = personnelID,
                Name = param.Name,
                GenderCode = param.GenderCode,
                Birthday = param.Birthday,
                MobilePhone = param.MobilePhone ?? "",
                Email = param.Email ?? "",
                Remark = param.Remark ?? "",
                UserID = null,
                IsDeleted = false,
                CreateInfo = new CreateInfoValueObject() { CreatorID = req.OperatorID, CreateTime = DateTime.Now },
                UpdateInfo = new UpdateInfoValueObject() { UpdaterID = req.OperatorID, UpdateTime = DateTime.Now }
            };

            //保存
            await personnelAgg.SaveChangedAsync();

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(new AddPersonnelRes() { ID = personnelAgg.ID });
        }

        /// <summary>
        /// 更新人员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> UpdatePersonnelAsync(TdbOperateReq<UpdatePersonnelReq> req)
        {
            //参数
            var param = req.Param;

            //人员领域服务
            var personnelService = new PersonnelService();

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //获取人员聚合
            var personnelAgg = await personnelService.GetByIDAsync(param.ID);
            if (personnelAgg is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.PersonnelNotExist, false);
            }

            //判断权限（只有创建人修改其人员信息）
            if (req.OperatorID != personnelAgg.CreateInfo.CreatorID)
            {
                return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions, false);
            }

            //更新人员信息
            personnelAgg.Name = param.Name;
            personnelAgg.GenderCode = param.GenderCode;
            personnelAgg.Birthday = param.Birthday;
            personnelAgg.MobilePhone = param.MobilePhone ?? "";
            personnelAgg.Email = param.Email ?? "";
            personnelAgg.Remark = param.Remark ?? "";

            //保存
            await personnelAgg.SaveChangedAsync();

            //提交事务
            TdbRepositoryTran.CommitTran();

            //TODO：与账户服务同步数据

            return TdbRes.Success(true);
        }

        #endregion
    }
}
