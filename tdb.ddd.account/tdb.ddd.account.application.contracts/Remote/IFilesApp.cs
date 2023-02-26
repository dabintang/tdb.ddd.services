using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.contracts;

namespace tdb.ddd.account.application.contracts.Remote
{
    /// <summary>
    /// 文件服务应用接口
    /// </summary>
    public interface IFilesApp : ITdbIOCScoped
    {
        /// <summary>
        /// 确认文件
        /// </summary>
        /// <param name="fileID">文件ID</param>
        /// <returns></returns>
        Task<TdbRes<bool>> ConfirmFileAsync(long fileID);
    }
}
