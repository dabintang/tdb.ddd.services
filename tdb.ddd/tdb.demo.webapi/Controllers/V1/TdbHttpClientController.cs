using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using tdb.ddd.application.contracts;
using tdb.ddd.contracts;
using tdb.ddd.infrastructure;
using tdb.ddd.infrastructure.Common.Http;
using tdb.ddd.webapi;
using tdb.demo.webapi.Configs;
using tdb.demo.webapi.MockData;
using static tdb.demo.webapi.Controllers.V1.UserController;
using static tdb.demo.webapi.Controllers.V2.UserController;

namespace tdb.demo.webapi.Controllers.V1
{
    /// <summary>
    /// TdbHttpClient使用测试
    /// </summary>
    public class TdbHttpClientController : BaseController
    {
        #region 接口

        /// <summary>
        /// 获取当前信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TdbAPILog]
        public async Task<TdbRes<UserRes>> GetCurrentUserInfo()
        {
            //获取头部身份认证信息
            var authentication = HttpContext.GetAuthenticationHeaderValue();

            var res = await TdbHttpClient.GetAsync<TdbRes<UserRes>>("http://127.0.0.1:31001/tdb.ddd.demo/v1/User/GetCurrentUserInfo", null, authentication);
            return res;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        [HttpGet]
        [TdbAPILog]
        public async Task<TdbRes<UserRes>> GetUserInfo([FromQuery] GetUserInfoReq req)
        {
            //获取头部身份认证信息
            var authentication = HttpContext.GetAuthenticationHeaderValue();
            var reqInner = new GetUserInfo2Req() { ID = req.ID };

            var res = await TdbHttpClient.GetAsync<TdbRes<UserRes>>("http://127.0.0.1:31001/tdb.ddd.demo/v2/User/GetUserInfo", reqInner, authentication);
            return res;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="req">条件</param>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<bool>> UpdateUserInfo([FromBody] UpdateUserInfoReq req)
        {
            //获取头部身份认证信息
            var authentication = HttpContext.GetAuthenticationHeaderValue();

            var res = await TdbHttpClient.PostAsJsonAsync<UpdateUserInfoReq, TdbRes<bool>>("http://127.0.0.1:31001/tdb.ddd.demo/v2/User/UpdateUserInfo", req, null, authentication);
            return res;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TdbAPILog]
        public async Task<TdbRes<List<UploadFilesRes>>> UploadFile()
        {
            //获取头部身份认证信息
            var authentication = HttpContext.GetAuthenticationHeaderValue();

            //条件
            var req = new UploadFilesReq()
            {
                Name = "mm1.jpeg",
                AccessLevelCode = 3,
                Remark = "上传文件测试"
            };
            using var fileStream = new FileStream(@"C:\Users\Administrator\Desktop\temp\image\big\mm1.jpeg", FileMode.Open, FileAccess.Read);

            var res = await TdbHttpClient.UploadFileAsync<UploadFilesReq, TdbRes<List<UploadFilesRes>>>("http://127.0.0.1:31020/tdb.ddd.files/v1/Files/UploadTempFiles", fileStream, req.Name, req, authentication);
            return res;
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TdbAPILog]
        public async Task<IActionResult> DownloadImage()
        {
            //获取头部身份认证信息
            var authentication = HttpContext.GetAuthenticationHeaderValue();

            //条件
            var req = new DownloadImageReq()
            {
                ID = 721553529280,
                Width = 800
            };

            var res = await TdbHttpClient.DownloadFileAsync<DownloadImageReq, TdbDownloadFileRes>("http://127.0.0.1:31020/tdb.ddd.files/v1/Files/DownloadImage", req, authentication);
            return new FileStreamResult(res.ContentStream, res.ContentType);
        }

        /// <summary>
        /// 从百度下载图片
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TdbAPILog]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadImageFromBaidu()
        {
            var res = await TdbHttpClient.DownloadFileAsync<object, TdbDownloadFileRes>("https://img2.baidu.com/it/u=383161466,2016311197&fm=253&fmt=auto&app=138&f=JPEG?w=500&h=1066", null);
            return new FileStreamResult(res.ContentStream, res.ContentType);
        }

        #endregion

        #region 参数定义

        /// <summary>
        /// 获取用户信息
        /// </summary>
        class GetUserInfo2Req
        {
            /// <summary>
            /// 用户编号
            /// </summary>
            [TdbHashIDJsonConverter]
            public long? ID { get; set; }
        }

        /// <summary>
        /// 上传文件 请求参数
        /// </summary>
        class UploadFilesReq
        {
            /// <summary>
            /// 文件名（含后缀）
            /// </summary>           
            public string Name { get; set; }

            /// <summary>
            /// 访问级别(1：仅创建者；2：授权；3：公开)
            /// </summary>
            public byte AccessLevelCode { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            public string Remark { get; set; } = "";
        }

        /// <summary>
        /// 上传文件 结果
        /// </summary>
        public class UploadFilesRes
        {
            /// <summary>
            /// 文件ID
            /// </summary>
            [TdbHashIDJsonConverter]
            public long ID { get; set; }

            /// <summary>
            /// 文件名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 是否上传成功
            /// </summary>
            public bool IsOK { get; set; }

            /// <summary>
            /// 结果描述
            /// </summary>
            public string Msg { get; set; }
        }
        
        /// <summary>
        /// 下载图片 请求条件
        /// </summary>
        class DownloadImageReq
        {
            /// <summary>
            /// [必须]文件ID
            /// </summary>
            [TdbHashIDJsonConverter]
            [TdbRequired("文件ID")]
            public long ID { get; set; }

            /// <summary>
            /// [可选]宽度（如果不传，则按高度比例缩放）
            /// </summary>
            public int Width { get; set; }

            /// <summary>
            /// [可选]高度（如果不传，则按宽度比例缩放）
            /// </summary>
            public int Height { get; set; }
        }
        #endregion
    }
}
