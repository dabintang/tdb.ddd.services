using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.ddd.account.application.contracts.Remote;
using tdb.ddd.account.domain.Authority;
using tdb.ddd.account.domain.BusMediatR;
using tdb.ddd.infrastructure;

namespace tdb.ddd.account.application.BusMediatR
{
    /// <summary>
    /// 确认临时文件
    /// </summary>
    public class ConfirmFileRequest : IRequest<bool>
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public long FileID { get; set; }
    }

    /// <summary>
    /// 确认临时文件
    /// </summary>
    public class ConfirmFileRequestHandler : IRequestHandler<ConfirmFileRequest, bool>
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(ConfirmFileRequest request, CancellationToken cancellationToken)
        {
            var filesApp = TdbIOC.GetService<IFilesApp>();
            var res = await filesApp.ConfirmFileAsync(request.FileID);
            return res.Data;
        }
    }
}
