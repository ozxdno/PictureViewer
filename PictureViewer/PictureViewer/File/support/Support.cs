using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 所支持文件
    /// </summary>
    class Support
    {
        /// <summary>
        /// 本文件是否是图片文件
        /// </summary>
        public bool IsPicture
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是 GIF 文件
        /// </summary>
        public bool IsGif
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是音频文件
        /// </summary>
        public bool IsMusic
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是视频文件
        /// </summary>
        public bool IsVideo
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是 ZIP 文件
        /// </summary>
        public bool IsZip
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是不是不支持类型
        /// </summary>
        public bool IsUnsupport
        {
            set;
            get;
        }
        /// <summary>
        /// 客户自定义类型
        /// </summary>
        public bool IsUserDefine
        {
            set;
            get;
        }
        
        /// <summary>
        /// 显示后缀
        /// </summary>
        public string ShowExtension
        {
            get;
            set;
        }
        /// <summary>
        /// 隐式后缀
        /// </summary>
        public string HideExtension
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public Support()
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="extraExtension">额外的扩展名</param>
        public Support(string type, string extraExtension)
        {

        }
    }
}
