using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.common.Crypto;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.infrastructure;
using tdb.ddd.relationships.application.contracts.V1.DTO.Circle;
using tdb.ddd.relationships.application.contracts.V1.Interface;
using tdb.ddd.relationships.domain.Circle;
using tdb.ddd.relationships.domain.Circle.Aggregate;
using tdb.ddd.relationships.domain.contracts.Enum;
using tdb.ddd.relationships.domain.Personnel;
using tdb.ddd.relationships.infrastructure;
using tdb.ddd.relationships.infrastructure.Config;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.relationships.application.V1
{
    /// <summary>
    /// 人际圈应用
    /// </summary>
    public class CircleAPP : ICircleAPP
    {
        #region 实现接口

        /// <summary>
        /// 获取人际圈信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<GetCircleRes>> GetCircleAsync(GetCircleReq req)
        {
            //人际圈领域服务
            var circleService = new CircleService();
            //获取人际圈聚合
            var circleAgg = await circleService.GetByIDAsync(req.ID);
            if (circleAgg is null)
            {
                return new TdbRes<GetCircleRes>(RelationshipsConfig.Msg.CircleNotExist, null);
            }

            //转为DTO
            var res = DTOMapper.Map<CircleAgg, GetCircleRes>(circleAgg);

            return TdbRes.Success(res);
        }

        /// <summary>
        /// 创建人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新人员ID</returns>
        public async Task<TdbRes<AddCircleRes>> AddCircleAsync(TdbOperateReq<AddCircleReq> req)
        {
            //参数
            var param = req.Param;

            //生成人际圈ID
            var circleID = RelationshipsUniqueIDHelper.CreateID();
            //人员聚合
            var circleAgg = new CircleAgg()
            {
                ID = circleID,
                Name = param.Name,
                Remark = param.Remark ?? "",
                CreateInfo = new CreateInfoValueObject() { CreatorID = req.OperatorID, CreateTime = DateTime.Now },
                UpdateInfo = new UpdateInfoValueObject() { UpdaterID = req.OperatorID, UpdateTime = DateTime.Now }
            };

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //保存
            await circleAgg.SaveChangedAsync();

            //人员领域服务
            var personnelService = new PersonnelService();

            //获取我的人员信息
            var personnelInfo = await personnelService.GetByUserIDAsync(req.OperatorID) ?? throw new TdbException($"获取我的人员信息为空【UserID={req.OperatorID}】");

            //生成我的成员信息
            var memberInfo = new MemberEntity(personnelInfo.ID, EnmRole.Admin);

            //把自己加入人际圈
            await circleAgg.AddMemberAsync(memberInfo);

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(new AddCircleRes() { ID = circleAgg.ID });
        }

        /// <summary>
        /// 更新人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新人员ID</returns>
        public async Task<TdbRes<bool>> UpdateCircleAsync(TdbOperateReq<UpdateCircleReq> req)
        {
            //参数
            var param = req.Param;

            //人际圈领域服务
            var circleService = new CircleService();

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //获取人际圈聚合
            var circleAgg = await circleService.GetByIDAsync(param.ID);
            if (circleAgg is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.CircleNotExist, false);
            }

            //判断权限（只有创建人可以修改）
            if (req.OperatorID != circleAgg.CreateInfo.CreatorID)
            {
                return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions, false);
            }

            //更新人际圈信息
            circleAgg.Name = param.Name;
            circleAgg.Remark = param.Remark ?? "";
            circleAgg.UpdateInfo.UpdaterID = req.OperatorID;
            circleAgg.UpdateInfo.UpdateTime = req.OperationTime;

            //保存
            await circleAgg.SaveChangedAsync();

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 解散人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新人员ID</returns>
        public async Task<TdbRes<bool>> DeleteCircleAsync(TdbOperateReq<DeleteCircleReq> req)
        {
            //参数
            var param = req.Param;

            //人际圈领域服务
            var circleService = new CircleService();

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //获取人际圈聚合
            var circleAgg = await circleService.GetByIDAsync(param.ID);
            if (circleAgg is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.CircleNotExist, false);
            }

            //判断权限（只有创建人可以解散人际圈）
            if (req.OperatorID != circleAgg.CreateInfo.CreatorID)
            {
                return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions, false);
            }

            //移出所有成员
            await circleAgg.RemoveAllMembersAsync();

            //删除人际圈
            await circleAgg.DeleteAsync();

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> AddMemberAsync(TdbOperateReq<AddMemberReq> req)
        {
            //参数
            var param = req.Param;

            //人际圈领域服务
            var circleService = new CircleService();

            //获取人际圈聚合
            var circleAgg = await circleService.GetByIDAsync(param.CircleID);
            if (circleAgg is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.CircleNotExist, false);
            }

            //TODO：如果担心超员，应该加分布式锁
            //获取当前人际圈内成员数
            var memberCount = await circleAgg.CountMembersAsync();
            if (memberCount >= circleAgg.MaxMembers)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.ReachedMemberLimit, false);
            }

            //人员领域服务
            var personnelService = new PersonnelService();

            //获取我的人员信息
            var myPersonnelInfo = await personnelService.GetByUserIDAsync(req.OperatorID) ?? throw new TdbException($"获取我的人员信息为空【UserID={req.OperatorID}】");

            //获取我的成员信息
            var myMemberInfo = await circleAgg.GetMemberAsync(myPersonnelInfo.ID) ?? throw new TdbException($"获取我的成员信息为空【UserID={req.OperatorID}，CircleID={circleAgg.ID}】");

            //判断我是否有管理员权限
            if (myMemberInfo.RoleCode != EnmRole.Admin)
            {
                return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions, false);
            }

            //获取人员信息
            var personnelInfo = await personnelService.GetByIDAsync(param.PersonnelID);
            if (personnelInfo is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.PersonnelNotExist, false);
            }

            //权限判断（只能添加自己的人员）
            if (personnelInfo.CreateInfo.CreatorID != req.OperatorID)
            {
                return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions, false);
            }

            //生成成员信息
            var memberInfo = new MemberEntity(personnelInfo.ID, param.RoleCode, param.Identity ?? "");

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //添加成员
            await circleAgg.AddMemberAsync(memberInfo);

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 批量添加成员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<BatchAddMemberRes>> BatchAddMemberAsync(TdbOperateReq<BatchAddMemberReq> req)
        {
            //参数
            var param = req.Param;

            //人际圈领域服务
            var circleService = new CircleService();

            //获取人际圈聚合
            var circleAgg = await circleService.GetByIDAsync(param.CircleID);
            if (circleAgg is null)
            {
                return new TdbRes<BatchAddMemberRes>(RelationshipsConfig.Msg.CircleNotExist, null);
            }

            //TODO：如果担心超员，应该加分布式锁
            //获取当前人际圈内成员数
            var memberCount = await circleAgg.CountMembersAsync();
            //批量添加后达到人数
            var countAfterAdd = memberCount + param.LstPersonnelID.Count;
            if (countAfterAdd >= circleAgg.MaxMembers)
            {
                //还能添加人数
                var canAddCount = circleAgg.MaxMembers - memberCount;
                if (canAddCount > 0)
                {
                    return new TdbRes<BatchAddMemberRes>(RelationshipsConfig.Msg.ReachedMemberLimit.FromNewMsg($"只能添加{canAddCount}人，请重新选择"), null);
                }
                else
                {
                    return new TdbRes<BatchAddMemberRes>(RelationshipsConfig.Msg.ReachedMemberLimit, null);
                }
            }

            //人员领域服务
            var personnelService = new PersonnelService();

            //获取我的人员信息
            var myPersonnelInfo = await personnelService.GetByUserIDAsync(req.OperatorID) ?? throw new TdbException($"获取我的人员信息为空【UserID={req.OperatorID}】");

            //获取我的成员信息
            var myMemberInfo = await circleAgg.GetMemberAsync(myPersonnelInfo.ID) ?? throw new TdbException($"获取我的成员信息为空【UserID={req.OperatorID}，CircleID={circleAgg.ID}】");

            //判断我是否有管理员权限
            if (myMemberInfo.RoleCode != EnmRole.Admin)
            {
                return new TdbRes<BatchAddMemberRes>(TdbComResMsg.InsufficientPermissions, null);
            }

            //批量添加成员结果
            var res = new BatchAddMemberRes();

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //循环添加成员
            foreach (var personnelID in param.LstPersonnelID)
            {
                //获取人员信息
                var personnelInfo = await personnelService.GetByIDAsync(personnelID);
                if (personnelInfo is null)
                {
                    res.FailCount++;
                    res.LstFailInfo.Add(new BatchAddMemberRes.FailInfo() { PersonnelID = personnelID, Reason = RelationshipsConfig.Msg.PersonnelNotExist.Msg });
                    continue;
                }

                //权限判断（只能添加自己的人员）
                if (personnelInfo.CreateInfo.CreatorID != req.OperatorID)
                {
                    res.FailCount++;
                    res.LstFailInfo.Add(new BatchAddMemberRes.FailInfo() { PersonnelID = personnelID, Name = personnelInfo.Name, Reason = TdbComResMsg.InsufficientPermissions.Msg });
                    continue;
                }

                //生成成员信息
                var memberInfo = new MemberEntity(personnelID, EnmRole.Member, "");

                //添加成员
                await circleAgg.AddMemberAsync(memberInfo);

                res.SuccessCount++;
            }

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(res);
        }

        /// <summary>
        /// 移出成员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> RemoveMemberAsync(TdbOperateReq<RemoveMemberReq> req)
        {
            //参数
            var param = req.Param;

            //人际圈领域服务
            var circleService = new CircleService();

            //获取人际圈聚合
            var circleAgg = await circleService.GetByIDAsync(param.CircleID);
            if (circleAgg is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.CircleNotExist, false);
            }

            //不能移出人际圈创建者
            if (circleAgg.CreateInfo.CreatorID == param.PersonnelID)
            {
                return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions, false);
            }

            //权限判断（人际圈创建人可以移出所有其他成员、管理员可以移出自己添加的人员）
            if (circleAgg.CreateInfo.CreatorID != req.OperatorID)
            {
                //人员领域服务
                var personnelService = new PersonnelService();

                //获取人员信息
                var personnelInfo = await personnelService.GetByIDAsync(param.PersonnelID);
                //管理员可以移出自己添加的人员
                if (personnelInfo is not null && personnelInfo.CreateInfo.CreatorID != req.OperatorID)
                {
                    return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions, false);
                }
            }

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //移出成员
            await circleAgg.RemoveMemberAsync(param.PersonnelID);

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 设置成员角色
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> SetMemberRoleAsync(TdbOperateReq<SetMemberRoleReq> req)
        {
            //参数
            var param = req.Param;

            //人际圈领域服务
            var circleService = new CircleService();

            //获取人际圈聚合
            var circleAgg = await circleService.GetByIDAsync(param.CircleID);
            if (circleAgg is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.CircleNotExist, false);
            }

            //权限判断（人际圈创建人有权限）
            if (circleAgg.CreateInfo.CreatorID != req.OperatorID)
            {
                return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions, false);
            }

            //创建人本身为管理员角色且不能改成非管理员
            if (circleAgg.CreateInfo.CreatorID == param.PersonnelID)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.CreatorMustBeAdmin, false);
            }

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //设置角色
            var result = await circleAgg.SetMemberRole(param.PersonnelID, param.RoleCode);

            //提交事务
            if (result.Data)
            {
                TdbRepositoryTran.CommitTran();
            }

            return result;
        }

        /// <summary>
        /// 设置成员身份
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> SetMemberIdentityAsync(TdbOperateReq<SetMemberIdentityReq> req)
        {
            //参数
            var param = req.Param;

            //人际圈领域服务
            var circleService = new CircleService();

            //获取人际圈聚合
            var circleAgg = await circleService.GetByIDAsync(param.CircleID);
            if (circleAgg is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.CircleNotExist, false);
            }

            //权限判断（人际圈创建人有权限、人员创建者有权限）
            if (circleAgg.CreateInfo.CreatorID != req.OperatorID)
            {
                //人员领域服务
                var personnelService = new PersonnelService();

                //获取人员信息
                var personnelInfo = await personnelService.GetByIDAsync(param.PersonnelID);
                //是否人员创建者
                if (personnelInfo is not null && personnelInfo.CreateInfo.CreatorID != req.OperatorID)
                {
                    return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions, false);
                }
            }

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //设置身份
            var result = await circleAgg.SetMemberIdentity(param.PersonnelID, param.Identity ?? "");

            //提交事务
            if (result.Data)
            {
                TdbRepositoryTran.CommitTran();
            }

            return result;
        }

        /// <summary>
        /// 生成加入人际圈的邀请码
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<CreateInvitationCodeRes>> CreateInvitationCodeAsync(TdbOperateReq<CreateInvitationCodeReq> req)
        {
            //参数
            var param = req.Param;

            //人际圈领域服务
            var circleService = new CircleService();

            //获取人际圈聚合
            var circleAgg = await circleService.GetByIDAsync(param.CircleID);
            if (circleAgg is null)
            {
                return new TdbRes<CreateInvitationCodeRes>(RelationshipsConfig.Msg.CircleNotExist, null);
            }

            //人员领域服务
            var personnelService = new PersonnelService();

            //获取我的人员信息
            var myPersonnelInfo = await personnelService.GetByUserIDAsync(req.OperatorID) ?? throw new TdbException($"获取我的人员信息为空【UserID={req.OperatorID}】");

            //获取我的成员信息
            var myMemberInfo = await circleAgg.GetMemberAsync(myPersonnelInfo.ID) ?? throw new TdbException($"获取我的成员信息为空【UserID={req.OperatorID}，CircleID={circleAgg.ID}】");

            //判断我是否有管理员权限
            if (myMemberInfo?.RoleCode != EnmRole.Admin)
            {
                return new TdbRes<CreateInvitationCodeRes>(TdbComResMsg.InsufficientPermissions, null);
            }

            //邀请码信息
            var invCodeInfo = new InvitationCodeInfo
            {
                CircleID = param.CircleID,
                InviterID = req.OperatorID,
                InviterName = req.OperatorName,
                ExpireAt = DateTime.Now.AddMinutes(param.EffectiveMinutes)
            };

            //结果
            var res = new CreateInvitationCodeRes()
            {
                Code = invCodeInfo.ToCode(),
                ExpireAt = invCodeInfo.ExpireAt
            };

            return TdbRes.Success(res);
        }

        /// <summary>
        /// 读取加入人际圈的邀请码信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<ReadInvitationCodeRes>> ReadInvitationCodeAsync(TdbOperateReq<ReadInvitationCodeReq> req)
        {
            //参数
            var param = req.Param;

            //从邀请码解析成邀请码信息
            var invCodeInfo = InvitationCodeInfo.FromCode(param.Code);
            if (invCodeInfo is null)
            {
                return new TdbRes<ReadInvitationCodeRes>(RelationshipsConfig.Msg.InvalidInvitationCode, null);
            }

            //是否已过期
            if (invCodeInfo.ExpireAt > DateTime.Now)
            {
                return new TdbRes<ReadInvitationCodeRes>(RelationshipsConfig.Msg.ExpiredInvitationCode, null);
            }

            //人际圈领域服务
            var circleService = new CircleService();

            //获取人际圈聚合
            var circleAgg = await circleService.GetByIDAsync(invCodeInfo.CircleID);
            if (circleAgg is null)
            {
                return new TdbRes<ReadInvitationCodeRes>(RelationshipsConfig.Msg.InvalidInvitationCode, null);
            }

            //结果
            var res = new ReadInvitationCodeRes()
            {
                CircleName = circleAgg.Name,
                InviterName = invCodeInfo.InviterName,
                ExpireAt = invCodeInfo.ExpireAt
            };

            return TdbRes.Success(res);
        }

        /// <summary>
        /// 通过邀请码加入人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> JoinByInvitationCodeAsync(TdbOperateReq<JoinByInvitationCodeReq> req)
        {
            //参数
            var param = req.Param;

            //从邀请码解析成邀请码信息
            var invCodeInfo = InvitationCodeInfo.FromCode(param.Code);
            if (invCodeInfo is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.InvalidInvitationCode, false);
            }

            //是否已过期
            if (invCodeInfo.ExpireAt > DateTime.Now)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.ExpiredInvitationCode, false);
            }

            //人际圈领域服务
            var circleService = new CircleService();

            //获取人际圈聚合
            var circleAgg = await circleService.GetByIDAsync(invCodeInfo.CircleID);
            if (circleAgg is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.InvalidInvitationCode, false);
            }

            //TODO：如果担心超员，应该加分布式锁
            //获取当前人际圈内成员数
            var memberCount = await circleAgg.CountMembersAsync();
            if (memberCount >= circleAgg.MaxMembers)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.ReachedMemberLimit, false);
            }

            //人员领域服务
            var personnelService = new PersonnelService();

            //获取邀请人的人员信息
            var inviterPersonnelInfo = await personnelService.GetByUserIDAsync(invCodeInfo.InviterID);
            if (inviterPersonnelInfo is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.InvalidInvitationCode, false);
            }

            //获取邀请人的成员信息
            var inviterMemberInfo = await circleAgg.GetMemberAsync(inviterPersonnelInfo.ID);
            if (inviterMemberInfo is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.InvalidInvitationCode, false);
            }

            //判断邀请者是否有管理员权限
            if (inviterMemberInfo.RoleCode != EnmRole.Admin)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.InvalidInvitationCode, false);
            }

            //获取我的人员信息
            var myPersonnelInfo = await personnelService.GetByUserIDAsync(req.OperatorID);
            if (myPersonnelInfo is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.PersonnelNotExist.FromNewMsg("我的人员信息不存在"), false);
            }

            //生成成员信息
            var memberInfo = new MemberEntity(myPersonnelInfo.ID, EnmRole.Member, "");

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //添加成员
            await circleAgg.AddMemberAsync(memberInfo);

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(true);
        }

        /// <summary>
        /// 退出人际圈
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        public async Task<TdbRes<bool>> WithdrawCircleAsync(TdbOperateReq<WithdrawCircleReq> req)
        {
            //参数
            var param = req.Param;

            //人际圈领域服务
            var circleService = new CircleService();

            //获取人际圈聚合
            var circleAgg = await circleService.GetByIDAsync(param.CircleID);
            if (circleAgg is null)
            {
                return new TdbRes<bool>(RelationshipsConfig.Msg.CircleNotExist, false);
            }

            //不能移出人际圈创建者
            if (circleAgg.CreateInfo.CreatorID == req.OperatorID)
            {
                return new TdbRes<bool>(TdbComResMsg.InsufficientPermissions.FromNewMsg("不能退出自己创建的人际圈"), false);
            }

            //人员领域服务
            var personnelService = new PersonnelService();

            //获取我的人员信息
            var myPersonnelInfo = await personnelService.GetByUserIDAsync(req.OperatorID) ?? throw new TdbException($"获取我的人员信息为空【UserID={req.OperatorID}】");

            //开启事务
            TdbRepositoryTran.BeginTranOnAsyncFunc();

            //移出成员
            await circleAgg.RemoveMemberAsync(myPersonnelInfo.ID);

            //提交事务
            TdbRepositoryTran.CommitTran();

            return TdbRes.Success(true);
        }

        #endregion

        #region 内部类

        /// <summary>
        /// 邀请码信息
        /// </summary>
        class InvitationCodeInfo
        {
            /// <summary>
            /// 人际圈ID
            /// </summary>
            public long CircleID { get; set; }

            /// <summary>
            /// 邀请人用户ID
            /// </summary>
            public long InviterID { get; set; }

            /// <summary>
            /// 邀请人姓名
            /// </summary>
            public string InviterName { get; set; } = "";

            /// <summary>
            /// 过期时间点
            /// </summary>
            public DateTime ExpireAt { get; set; }

            /// <summary>
            /// 生成邀请码
            /// </summary>
            /// <returns>邀请码</returns>
            public string ToCode()
            {
                //邀请码
                var code = $"{this.CircleID}.{this.InviterID}.{CvtHelper.ToTimeStamp(this.ExpireAt)}";
                //加密
                var ciphertext = EncryptHelper.EncryptAES(RelationshipsConfig.Distributed.AES.Key, RelationshipsConfig.Distributed.AES.IV, code);
                return ciphertext ;
            }

            /// <summary>
            /// 从邀请码解析成邀请码信息
            /// </summary>
            /// <param name="ciphertext">加密的邀请码</param>
            /// <returns>邀请码信息</returns>
            public static InvitationCodeInfo? FromCode(string ciphertext)
            {
                var info = new InvitationCodeInfo();

                try
                {
                    //解密
                    var code = EncryptHelper.DecryptAES(RelationshipsConfig.Distributed.AES.Key, RelationshipsConfig.Distributed.AES.IV, ciphertext);

                    //拆分
                    var items = code.Split('.');

                    //邀请码信息
                    info.CircleID = Convert.ToInt64(items[0]);
                    info.InviterID = Convert.ToInt64(items[1]);
                    info.ExpireAt = CvtHelper.TimeStampToTime(Convert.ToInt64(items[2]));

                    return info;
                }
                catch (Exception ex)
                {
                    TdbLogger.Ins.Warn($"从邀请码解析成邀请码信息发生异常【ex={ex}，info={info.SerializeJson()}】");
                    return null;
                }
            }
        }

        #endregion
    }
}
