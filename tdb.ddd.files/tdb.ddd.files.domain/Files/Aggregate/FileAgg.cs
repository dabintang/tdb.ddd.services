using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using tdb.ddd.contracts;
using tdb.ddd.domain;
using tdb.ddd.files.domain.contracts.Enum;
using tdb.ddd.files.infrastructure.Config;
using tdb.ddd.infrastructure;

namespace tdb.ddd.files.domain.Files.Aggregate
{
    /// <summary>
    /// 文件聚合
    /// </summary>
    public class FileAgg : TdbAggregateRoot<long>
    {
        #region 仓储

        private IFileRepos? _fileRepos;
        /// <summary>
        /// 文件仓储
        /// </summary>
        private IFileRepos FileRepos
        {
            get
            {
                this._fileRepos ??= TdbIOC.GetService<IFileRepos>();
                if (this._fileRepos is null)
                {
                    throw new TdbException("文件仓储接口未实现");
                }

                return this._fileRepos;
            }
        }

        #endregion

        #region 值

        /// <summary>
        /// 文件名（含后缀）
        /// </summary>           
        public string Name { get; set; } = "";

        /// <summary>
        /// 文件地址(本地路径或url)
        /// </summary>
        [JsonInclude]
        public string Address { get; internal set; } = "";

        /// <summary>
        /// 存储类型（1：本地磁盘）
        /// </summary>
        public EnmStorageType StorageTypeCode { get; set; }

        /// <summary>
        /// 字节数
        /// </summary>
        [JsonInclude]
        public long Size { get; internal set; }

        /// <summary>
        /// 访问级别(1：仅创建者；2：授权；3：公开)
        /// </summary>
        public EnmAccessLevel AccessLevelCode { get; set; }

        /// <summary>
        /// 文件状态（1：临时文件；2：正式文件）
        /// </summary>
        public EnmTdbFileStatus FileStatusCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 创建信息
        /// </summary>
        public CreateInfoValueObject CreateInfo { get; set; } = new CreateInfoValueObject();

        /// <summary>
        /// 更新信息
        /// </summary>
        public UpdateInfoValueObject UpdateInfo { get; set; } = new UpdateInfoValueObject();

        /// <summary>
        /// 内容类型
        /// </summary>
        private string? _contentType;

        /// <summary>
        /// 数据内容
        /// </summary>
        private byte[]? _data;

        #endregion

        #region 行为

        /// <summary>
        /// 存储文件
        /// </summary>
        /// <param name="data">文件信息</param>
        /// <returns></returns>
        public async Task StorageFileAsync(byte[] data)
        {
            switch (this.StorageTypeCode)
            {
                case EnmStorageType.Local:
                    //保存文件到本地磁盘
                    await this.StorageFileToLocalAsync(data);
                    break;
                default:
                    throw new TdbException($"不支持的文件存储类型[{this.StorageTypeCode}]");
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> ReadFileAsync()
        {
            if (this._data is null)
            {
                switch (this.StorageTypeCode)
                {
                    case EnmStorageType.Local:
                        //从本地磁盘读取文件
                        this._data = await this.ReadFileFromLocalAsync();
                        break;
                    default:
                        throw new TdbException($"不支持的文件存储类型.[{this.StorageTypeCode}]");
                }
            }

            return this._data;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <returns></returns>
        public async Task DeleteFile()
        {
            switch (this.StorageTypeCode)
            {
                case EnmStorageType.Local:
                    //从本地磁盘删除文件
                    this.DeleteFileFromLocal();
                    await Task.CompletedTask;
                    break;
                default:
                    throw new TdbException($"不支持的文件存储类型.[{this.StorageTypeCode}]");
            }
        }

        /// <summary>
        /// 判断指定用户是否有访问权限
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="lstRoleID">用户角色ID</param>
        /// <returns></returns>
        public bool IsAuthorizedAccess(long userID, IEnumerable<long> lstRoleID)
        {
            //是否公开
            if (this.AccessLevelCode == EnmAccessLevel.Public)
            {
                return true;
            }

            //是否文件创建者/超级管理员
            if (this.CreateInfo?.CreatorID == userID || (lstRoleID?.Contains(TdbCst.RoleID.SuperAdmin) ?? false))
            {
                return true;
            }

            //是否被授权（TODO：未实现）

            return false;
        }

        /// <summary>
        /// 判断指定用户是否有修改权限
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="lstRoleID">用户角色ID</param>
        /// <returns></returns>
        public bool IsAuthorizedModify(long userID, IEnumerable<long> lstRoleID)
        {
            //是否文件创建者/超级管理员
            if (this.CreateInfo?.CreatorID == userID || (lstRoleID?.Contains(TdbCst.RoleID.SuperAdmin) ?? false))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取内容类型
        /// </summary>
        /// <returns></returns>
        public string GetContentType()
        {
            if (this._contentType is null)
            {
                //后缀
                var extension = Path.GetExtension(this.Address)?.TrimStart('.');

                //内容类型
                this._contentType = FilesConfig.Mime.FindContentType(extension);
            }
           
            return this._contentType;
        }

        /// <summary>
        /// 是否图片
        /// </summary>
        /// <returns></returns>
        public bool IsImage()
        {
            //后缀
            var extension = Path.GetExtension(this.Address) ?? "";
            //图片后缀
            var lstImgExtension = new List<string> { ".jpg", ".jpeg", ".bmp", ".png", ".gif" };
            if (lstImgExtension.Contains(extension) == false)
            {
                return false;
            }

            //文件头2个字节为文件类型
            var fileClass = "";
            if (this._data != null)
            {
                fileClass = $"{this._data[0]}{this._data[1]}";
            }
            else
            {
                if (File.Exists(this.Address) == false)
                {
                    throw new TdbException($"文件不存在[{this.Address}]");
                }

                var data2 = new byte[2];
                using (var stream = File.OpenRead(this.Address))
                {
                    stream.Read(data2, 0, 2);
                    fileClass = $"{data2[0]}{data2[1]}";
                }
            }

            //255216是jpg;7173是gif;6677是BMP,13780是PNG
            if (fileClass == "255216" || fileClass == "7173" || fileClass == "13780" || fileClass == "6677")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await this.FileRepos.SaveAsync(this);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAsync()
        {
            //从数据库总删除
            await this.FileRepos.DeleteFileAsync(this.ID);
            //删除文件
            await this.DeleteFile();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 存储文件到本地磁盘
        /// </summary>
        /// <param name="data">文件信息</param>
        /// <returns></returns>
        private async Task StorageFileToLocalAsync(byte[] data)
        {
            //无后缀的文件名
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this.Name);
            //后缀
            var extension = Path.GetExtension(this.Name);

            //文件名
            var fileName = $"{fileNameWithoutExtension}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";

            //文件夹
            var path = FilesConfig.Distributed.FilesPath;
            //文件夹不存在则创建
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            //完整文件名
            var fullFileName = Path.Combine(path, fileName);

            //保存文件
            await File.WriteAllBytesAsync(fullFileName, data);

            this.Address = fullFileName;
            this.Size = data.Length;
        }

        /// <summary>
        /// 从本地磁盘读取文件
        /// </summary>
        /// <returns></returns>
        private async Task<byte[]> ReadFileFromLocalAsync()
        {
            if (File.Exists(this.Address) == false)
            {
                throw new TdbException($"文件不存在[{this.Address}]");
            }

            var data = await File.ReadAllBytesAsync(this.Address);
            return data;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <returns></returns>
        private void DeleteFileFromLocal()
        {
            if (File.Exists(this.Address) == false)
            {
                return;
            }

            File.Delete(this.Address);
        }

        #endregion

        /// <summary>
        /// 析构函数
        /// </summary>
        ~FileAgg()
        {
            if (this._data is not null && this._data.Length > 10240)
            {
                this._data = null;
                GC.Collect();
            }
        }
    }
}
