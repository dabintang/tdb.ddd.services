using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.relationships.application.contracts.V1.DTO.Circle;
using tdb.ddd.relationships.application.contracts.V1.DTO.Personnel;
using tdb.ddd.relationships.application.contracts.V1.Interface;
using tdb.ddd.webapi;

namespace tdb.ddd.relationships.webapi.Controllers.V1
{
    /// <summary>
    /// 人员
    /// </summary>
    [TdbApiVersion(1)]
    public class PersonnelController : BaseController
    {
        /// <summary>
        /// 人员应用
        /// </summary>
        private readonly IPersonnelAPP personnelAPP;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="personnelAPP">人员应用</param>
        public PersonnelController(IPersonnelAPP personnelAPP)
        {
            this.personnelAPP = personnelAPP;
        }

        #region 接口

        /// <summary>
        /// 获取人员信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<TdbRes<GetPersonnelRes>> GetPersonnel([FromQuery] GetPersonnelReq req)
        {
            var res = await this.personnelAPP.GetPersonnelAsync(req);
            return res;
        }

        /// <summary>
        /// 根据用户ID获取人员信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<TdbRes<GetPersonnelByUserIDRes>> GetPersonnelByUserID([FromQuery] GetPersonnelByUserIDReq req)
        {
            var res = await this.personnelAPP.GetPersonnelByUserIDAsync(req);
            return res;
        }

        /// <summary>
        /// 创建我的人员信息
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<CreateMyPersonnelInfoRes>> CreateMyPersonnelInfo([FromBody] CreateMyPersonnelInfoReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //创建我的人员信息
            var res = await this.personnelAPP.CreateMyPersonnelInfoAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 添加人员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns>新人员ID</returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<AddPersonnelRes>> AddPersonnel([FromBody] AddPersonnelReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //添加人员
            var res = await this.personnelAPP.AddPersonnelAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 更新人员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> UpdatePersonnel([FromBody] UpdatePersonnelReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //更新人员
            var res = await this.personnelAPP.UpdatePersonnelAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 删除人员
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> DeletePersonnel([FromBody] DeletePersonnelReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //删除人员
            var res = await this.personnelAPP.DeletePersonnelAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 添加人员照片
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> AddPersonnelPhoto([FromBody] AddPersonnelPhotoReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //添加人员照片
            var res = await this.personnelAPP.AddPersonnelPhotoAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 删除人员照片
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> DeletePersonnelPhoto([FromBody] DeletePersonnelPhotoReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //删除人员照片
            var res = await this.personnelAPP.DeletePersonnelPhotoAsync(reqOpe);
            return res;
        }

        /// <summary>
        /// 设置人员头像照片
        /// </summary>
        /// <param name="req">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> SetHeadImg([FromBody] SetHeadImgReq req)
        {
            //参数
            var reqOpe = this.CreateTdbOperateReq(req);

            //设置人员头像照片
            var res = await this.personnelAPP.SetHeadImgAsync(reqOpe);
            return res;
        }

        #endregion
    }
}
