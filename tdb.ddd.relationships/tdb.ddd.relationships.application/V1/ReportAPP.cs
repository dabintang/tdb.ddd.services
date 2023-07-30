using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.relationships.application.contracts.V1.DTO.Report;
using tdb.ddd.relationships.application.contracts.V1.Interface;
using tdb.ddd.relationships.domain.contracts.Enum;
using tdb.ddd.relationships.domain.Personnel;
using tdb.ddd.relationships.repository.DBEntity;
using tdb.ddd.repository.sqlsugar;

namespace tdb.ddd.relationships.application.V1
{
    /// <summary>
    /// 报表应用
    /// </summary>
    public class ReportAPP : IReportAPP
    {
        #region 实现接口

        /// <summary>
        /// 查询人际圈列表
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        public async Task<TdbPageRes<QueryCircleListRes>> QueryCircleListAsync(QueryCircleListReq req)
        {
            var db = TdbDBContext.GetDBContext();
            var query = db.Queryable<CircleInfo>().LeftJoin<CircleMemberInfo>((ci, cmi) => ci.ID == cmi.CircleID);

            //[可选]人员ID（查询该人员加入的人际圈）
            query = query.WhereIF(req.PersonnelID.HasValue, (ci, cmi) => cmi.PersonnelID == req.PersonnelID!.Value);
            //[可选]人际圈名称（模糊匹配）
            query = query.WhereIF(!string.IsNullOrWhiteSpace(req.CircleName), ci => ci.Name.Contains(req.CircleName!));

            //排序
            if (req.LstSortItem is not null)
            {
                foreach (var sort in req.LstSortItem)
                {
                    var orderTypeCode = sort.SortCode == EnmTdbSort.Asc ? OrderByType.Asc : OrderByType.Desc;
                    switch (sort.FieldCode)
                    {
                        case QueryCircleListReq.EnmSortField.ID:
                            query = query.OrderBy(ci => ci.ID, orderTypeCode);
                            break;
                        case QueryCircleListReq.EnmSortField.CircleName:
                            query = query.OrderBy(ci => ci.Name, orderTypeCode);
                            break;
                    }
                }
            }

            //查询
            var total = new RefAsync<int>();
            var lstInfo = await query.Select(ci => new QueryCircleListRes()
            {
                ID = ci.ID,
                CircleName = ci.Name,
                ImageID = ci.ImageID,
                MaxMembers = ci.MaxMembers,
                MembersCount = SqlFunc.Subqueryable<CircleMemberInfo>().Where(subCmi => ci.ID == subCmi.CircleID).Count(),
                CreatorID = ci.CreatorID
            }).Distinct().ToOffsetPageAsync(req.PageNO, req.PageSize, total);

            return new TdbPageRes<QueryCircleListRes>(TdbComResMsg.Success, lstInfo, total);
        }

        /// <summary>
        /// 查询我加入了的人际圈列表
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        public async Task<TdbPageRes<QueryMyCircleListRes>> QueryMyCircleListAsync(TdbOperateReq<QueryMyCircleListReq> req)
        {
            //根据用户ID获取人员聚合
            var personnelService = new PersonnelService();
            var personnelAgg = await personnelService.GetByUserIDAsync(req.OperatorID);
            if (personnelAgg is null)
            {
                return new TdbPageRes<QueryMyCircleListRes>(TdbComResMsg.Success, new List<QueryMyCircleListRes>(), 0);
            }

            //参数
            var param = req.Param;

            var db = TdbDBContext.GetDBContext();
            var query = db.Queryable<CircleInfo>().LeftJoin<CircleMemberInfo>((ci, cmi) => ci.ID == cmi.CircleID);

            query = query.Where((ci, cmi) => cmi.PersonnelID == personnelAgg.ID);
            //[可选]人际圈名称（模糊匹配）
            query = query.WhereIF(!string.IsNullOrWhiteSpace(param.CircleName), ci => ci.Name.Contains(param.CircleName!));

            //排序
            if (param.LstSortItem is not null)
            {
                foreach (var sort in param.LstSortItem)
                {
                    var orderTypeCode = sort.SortCode == EnmTdbSort.Asc ? OrderByType.Asc : OrderByType.Desc;
                    switch (sort.FieldCode)
                    {
                        case QueryMyCircleListReq.EnmSortField.ID:
                            query = query.OrderBy(ci => ci.ID, orderTypeCode);
                            break;
                        case QueryMyCircleListReq.EnmSortField.CircleName:
                            query = query.OrderBy(ci => ci.Name, orderTypeCode);
                            break;
                    }
                }
            }

            //查询
            var total = new RefAsync<int>();
            var lstInfo = await query.Select(ci => new QueryMyCircleListRes()
            {
                ID = ci.ID,
                CircleName = ci.Name,
                ImageID = ci.ImageID,
                MaxMembers = ci.MaxMembers,
                MembersCount = SqlFunc.Subqueryable<CircleMemberInfo>().Where(subCmi => ci.ID == subCmi.CircleID).Count(),
                CreatorID = ci.CreatorID
            }).ToOffsetPageAsync(param.PageNO, param.PageSize, total);

            return new TdbPageRes<QueryMyCircleListRes>(TdbComResMsg.Success, lstInfo, total);
        }

        /// <summary>
        /// 查询人际圈内人员信息列表
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        public async Task<TdbPageRes<QueryCirclePersonnelListRes>> QueryCirclePersonnelListAsync(QueryCirclePersonnelListReq req)
        {
            var db = TdbDBContext.GetDBContext();
            var query = db.Queryable<CircleMemberInfo>().LeftJoin<PersonnelInfo>((cmi, pi) => cmi.PersonnelID == pi.ID);

            //[必须]人际圈ID
            query = query.Where(cmi => cmi.CircleID == req.CircleID);

            //排序
            if (req.LstSortItem is not null)
            {
                foreach (var sort in req.LstSortItem)
                {
                    var orderTypeCode = sort.SortCode == EnmTdbSort.Asc ? OrderByType.Asc : OrderByType.Desc;
                    switch (sort.FieldCode)
                    {
                        case QueryCirclePersonnelListReq.EnmSortField.Role:
                            query = query.OrderBy(cmi => cmi.RoleCode, orderTypeCode);
                            break;
                        case QueryCirclePersonnelListReq.EnmSortField.Name:
                            query = query.OrderBy((cmi, pi) => pi.Name, orderTypeCode);
                            break;
                    }
                }
            }

            //查询
            var total = new RefAsync<int>();
            var lstInfo = await query.Select((cmi, pi) => new QueryCirclePersonnelListRes()
            {
                CircleID = cmi.CircleID,
                PersonnelID = cmi.PersonnelID,
                HeadImgID = pi.HeadImgID,
                Name = pi.Name,
                GenderCode = (EnmGender)pi.GenderCode,
                RoleCode = (EnmRole)cmi.RoleCode,
                Identity = cmi.Identity,
                PersonnelCreatorID = pi.CreatorID
            }).ToOffsetPageAsync(req.PageNO, req.PageSize, total);

            return new TdbPageRes<QueryCirclePersonnelListRes>(TdbComResMsg.Success, lstInfo, total);
        }

        /// <summary>
        /// 查询我创建的人员信息列表
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        public async Task<TdbPageRes<QueryMyPersonnelListRes>> QueryMyPersonnelListAsync(TdbOperateReq<QueryMyPersonnelListReq> req)
        {
            //参数
            var param = req.Param;

            var db = TdbDBContext.GetDBContext();
            var query = db.Queryable<PersonnelInfo>();

            //[必须]创建者ID
            query = query.Where(pi => pi.CreatorID == req.OperatorID);

            //排序
            if (param.LstSortItem is not null)
            {
                foreach (var sort in param.LstSortItem)
                {
                    var orderTypeCode = sort.SortCode == EnmTdbSort.Asc ? OrderByType.Asc : OrderByType.Desc;
                    switch (sort.FieldCode)
                    {
                        case QueryMyPersonnelListReq.EnmSortField.PersonnelID:
                            query = query.OrderBy(pi => pi.ID, orderTypeCode);
                            break;
                        case QueryMyPersonnelListReq.EnmSortField.Name:
                            query = query.OrderBy(pi => pi.Name, orderTypeCode);
                            break;
                    }
                }
            }

            //查询
            var total = new RefAsync<int>();
            var lstInfo = await query.Select(pi => new QueryMyPersonnelListRes()
            {
                ID = pi.ID,
                HeadImgID = pi.HeadImgID,
                Name = pi.Name,
                GenderCode = (EnmGender)pi.GenderCode,
                Birthday = pi.Birthday,
                MobilePhone = pi.MobilePhone,
                Email = pi.Email,
                Remark = pi.Remark
            }).ToOffsetPageAsync(param.PageNO, param.PageSize, total);

            return new TdbPageRes<QueryMyPersonnelListRes>(TdbComResMsg.Success, lstInfo, total);
        }

        #endregion
    }
}
