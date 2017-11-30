using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// 不支持
        /// </summary>
        Unsupport,
        /// <summary>
        /// 图片
        /// </summary>
        Picture,
        /// <summary>
        /// 动图
        /// </summary>
        Gif,
        /// <summary>
        /// 音频
        /// </summary>
        Music,
        /// <summary>
        /// 视频
        /// </summary>
        Video,
        /// <summary>
        /// 配置文件
        /// </summary>
        Pvini,
        /// <summary>
        /// 缓存文件
        /// </summary>
        Pvdata,
        /// <summary>
        /// 提示：错误
        /// </summary>
        TipErr,
        /// <summary>
        /// 提示：不存在
        /// </summary>
        TipNot,
        /// <summary>
        /// 提示：未知
        /// </summary>
        TipUnk,
        /// <summary>
        /// 提示：不支持
        /// </summary>
        TipUnp,
        /// <summary>
        /// 提示：初始化
        /// </summary>
        TipInt,
        /// <summary>
        /// 提示：音频文件（封面）
        /// </summary>
        TipMus
    }
}
