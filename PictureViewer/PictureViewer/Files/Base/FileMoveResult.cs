using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 文件操作的结果
    /// </summary>
    public enum FileMoveResult
    {
        /// <summary>
        /// 成功操作
        /// </summary>
        SUCCESSED,
        /// <summary>
        /// 取消操作
        /// </summary>
        CANCLED,

        /// <summary>
        /// 源文件已存在
        /// </summary>
        SOUR_FILE_EXISTED,
        /// <summary>
        /// 源文件不存在
        /// </summary>
        SOUR_FILE_NOT,
        /// <summary>
        /// 源文件正在使用
        /// </summary>
        SOUR_FILE_USING,
        /// <summary>
        /// 源文件非法命名
        /// </summary>
        SOUR_FILE_ILLEGAL,
        /// <summary>
        /// 源路径已存在
        /// </summary>
        SOUR_PATH_EXISTED,
        /// <summary>
        /// 源路径不存在
        /// </summary>
        SOUR_PATH_NOT,
        /// <summary>
        /// 源路径非法
        /// </summary>
        SOUR_PATH_ILLEGAL,
        /// <summary>
        /// 源文件的文件名称已存在
        /// </summary>
        SOUR_NAME_EXISTED,
        /// <summary>
        /// 源文件的文件名称不存在
        /// </summary>
        SOUR_NAME_NOT,
        /// <summary>
        /// 源文件的文件名称非法
        /// </summary>
        SOUR_NAME_ILLEGAL,

        /// <summary>
        /// 目标文件已存在
        /// </summary>
        DEST_FILE_EXISTED,
        /// <summary>
        /// 目标文件不存在
        /// </summary>
        DEST_FILE_NOT,
        /// <summary>
        /// 目标文件正在使用
        /// </summary>
        DEST_FILE_USING,
        /// <summary>
        /// 目标文件非法命名
        /// </summary>
        DEST_FILE_ILLEGAL,
        /// <summary>
        /// 目标路径已存在
        /// </summary>
        DEST_PATH_EXISTED,
        /// <summary>
        /// 目标路径不存在
        /// </summary>
        DEST_PATH_NOT,
        /// <summary>
        /// 目标路径非法
        /// </summary>
        DEST_PATH_ILLEGAL,
        /// <summary>
        /// 目标文件的文件名称已存在
        /// </summary>
        DEST_NAME_EXISTED,
        /// <summary>
        /// 目标文件的文件名称不存在
        /// </summary>
        DEST_NAME_NOT,
        /// <summary>
        /// 目标文件的文件名称非法
        /// </summary>
        DEST_NAME_ILLEGAL
    }
}
