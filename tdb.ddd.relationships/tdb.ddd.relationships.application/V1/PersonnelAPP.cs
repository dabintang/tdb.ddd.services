﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.relationships.application.contracts.Remote.Interface;
using tdb.ddd.relationships.application.contracts.V1.DTO.Personnel;
using tdb.ddd.relationships.application.contracts.V1.Interface;
using tdb.ddd.relationships.domain.Circle;
using tdb.ddd.relationships.domain.Circle.Aggregate;
using tdb.ddd.relationships.domain.Personnel;
using tdb.ddd.relationships.domain.Personnel.Aggregate;
using tdb.ddd.relationships.infrastructure;
using tdb.ddd.relationships.infrastructure.Config;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.relationships.application.V1
{
    /// <summary>
    /// 人员应用
    /// </summary>
    public class PersonnelAPP : IPersonnelAPP
    {
        #region 实现接口

        /// <summary>
        /// 获取人员信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<GetPersonnelRes>> GetPersonnelAsync(GetPersonnelReq req)
        {
            //人员领域服务
            var personnelService = new PersonnelService();

            //获取人员聚合
            var personnelAgg = await personnelService.GetByIDAsync(req.ID);
            if (personnelAgg is null)
            {
                return new TdbRes<GetPersonnelRes>(RelationshipsConfig.Msg.PersonnelNotExist, null);
            }

            //类型转换
            var res = DTOMapper.Map<PersonnelAgg, GetPersonnelRes>(personnelAgg);

            return TdbRes.Success(res);
        }

        /// <summary>
        /// 根据用户ID获取人员信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<GetPersonnelByUserIDRes>> GetPersonnelByUserIDAsync(GetPersonnelByUserIDReq req)
        {
            //人员领域服务
            var personnelService = new PersonnelService();

            //根据用户ID获取人员聚合
            var personnelAgg = await personnelService.GetByUserIDAsync(req.UserID);
            if (personnelAgg is null)
            {
                return new TdbRes<GetPersonnelByUserIDRes>(RelationshipsConfig.Msg.PersonnelNotExist, null);
            }

            //类型转换
            var res = DTOMapper.Map<PersonnelAgg, GetPersonnelByUserIDRes>(personnelAgg);

            return TdbRes.Success(res);
        }

        /// <summary>
        /// 创建我的人员信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新人员ID</returns>
        public async Task<TdbRes<CreateMyPersonnelInfoRes>> CreateMyPersonnelInfoAsync(TdbOperateReq<CreateMyPersonnelInfoReq> req)
        {
            //根据用户ID获取人员聚合
            var personnelService = new PersonnelService();
            var personnelAgg = await personnelService.GetByUserIDAsync(req.OperatorID);
            if (personnelAgg is not null)
            {
                return TdbRes.Success(new CreateMyPersonnelInfoRes() { ID = personnelAgg.ID });
            }

            //获取我的用户信息 
            var accountApp = TdbIOC.GetService<IAccountApp>()!;
            var userInfo = await accountApp.GetUserInfoByIDAsync(req.OperatorID);
            if (userInfo is null) throw new TdbException("从账号微服务获取我的用户信息为空");

            //生成人员ID
            var personnelID = RelationshipsUniqueIDHelper.CreateID();
            //人员聚合
            personnelAgg = new PersonnelAgg()
            {
                ID = personnelID,
                Name = userInfo.Name,
                GenderCode = userInfo.GenderCode,
                Birthday = userInfo.Birthday,
                MobilePhone = userInfo.MobilePhone,
                Email = userInfo.Email,
                Remark = userInfo.Remark,
                UserID = null,
                CreateInfo = new CreateInfoValueObject() { CreatorID = req.OperatorID, CreateTime = DateTime.Now },
                UpdateInfo = new UpdateInfoValueObject() { UpdaterID = req.OperatorID, UpdateTime = DateTime.Now }
            };

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //保存
            await personnelAgg.SaveChangedAsync();

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(new CreateMyPersonnelInfoRes() { ID = personnelAgg.ID });
        }

        /// <summary>
        /// 添加人员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新人员ID</returns>
        public async Task<TdbRes<AddPersonnelRes>> AddPersonnelAsync(TdbOperateReq<AddPersonnelReq> req)
        {
            //参数
            var param = req.Param;

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
                CreateInfo = new CreateInfoValueObject() { CreatorID = req.OperatorID, CreateTime = DateTime.Now },
                UpdateInfo = new UpdateInfoValueObject() { UpdaterID = req.OperatorID, UpdateTime = DateTime.Now }
            };

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

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

            //判断权限（只有创建人可以修改）
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
            personnelAgg.UpdateInfo.UpdaterID = req.OperatorID;
            personnelAgg.UpdateInfo.UpdateTime = req.OperationTime;

            //保存
            await personnelAgg.SaveChangedAsync();

            //提交事务
            TdbRepositoryTran.CommitTran();

            //TODO：与账户服务同步数据

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 删除人员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> DeletePersonnelAsync(TdbOperateReq<DeletePersonnelReq> req)
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

            //判断权限（只有创建人可以删除）
            if (req.OperatorID != personnelAgg.CreateInfo.CreatorID)
            {
                return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions, false);
            }

            //人际圈领域服务
            var circleService = new CircleService();

            //获取该人员所在人际圈ID
            var lstCircleID = await personnelAgg.GetCircleIDsAsync();
            foreach (var circleID in lstCircleID)
            {
                //获取人际圈
                var circleAgg = await circleService.GetByIDAsync(circleID);
                if (circleAgg is not null)
                {
                    await circleAgg.RemoveMemberAsync(personnelAgg.ID);
                }
            }

            //TODO：
            //删除该人员所有照片


            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(true);
        }

        #endregion
    }
}
