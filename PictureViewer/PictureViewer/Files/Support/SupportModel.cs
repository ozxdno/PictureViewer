using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 文件后缀信息
    /// </summary>
    public class SupportModel
    {
        /// <summary>
        /// 是否支持隐藏后缀
        /// </summary>
        public static bool SupportHide
        {
            get;
            set;
        }

        /// <summary>
        /// 后缀类型
        /// </summary>
        public FileType Type
        {
            set;
            get;
        }
        /// <summary>
        /// 显式后缀
        /// </summary>
        public string ShowExtension
        {
            set;
            get;
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
        /// 是否是用户自定义后缀
        /// </summary>
        public bool IsUserDefined
        {
            set;
            get;
        }

        /// <summary>
        /// 创建空的支持模型
        /// </summary>
        public SupportModel()
        {
            Type = FileType.Unsupport;
            ShowExtension = null;
            HideExtension = null;
            IsUserDefined = false;
        }
        /// <summary>
        /// 创建支持文件的模型
        /// </summary>
        /// <param name="showExtension">显式后缀</param>
        /// <param name="hideExtension">隐式后缀</param>
        /// <param name="type">后缀类型</param>
        /// <param name="isUserDefined">是否是用户自定义后缀</param>
        public SupportModel(string showExtension, string hideExtension, FileType type, bool isUserDefined)
        {
            Type = type;
            ShowExtension = showExtension;
            HideExtension = hideExtension;
            IsUserDefined = isUserDefined;
        }
    }
}
