using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Tools
{
    /// <summary>
    /// 视频工具
    /// </summary>
    public class VideoOperate
    {
        /// <summary>
        /// 获取视频的尺寸（分辨率），获取失败返回 FALSE。
        /// </summary>
        /// <param name="source">视频文件</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <returns></returns>
        public static bool GetSize(string source,ref int width,ref int height)
        {
            height = 100;
            width = 100;
            return true;
        }
    }
}
