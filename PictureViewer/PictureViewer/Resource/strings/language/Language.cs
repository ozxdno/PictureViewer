using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Strings
{
    /// <summary>
    /// 语种
    /// </summary>
    class Language
    {
        /// <summary>
        /// 语种（默认中文）
        /// </summary>
        public static LANGUAGE language
        {
            get;
            set;
        }
        /// <summary>
        /// 语种
        /// </summary>
        public enum LANGUAGE
        {
            /// <summary>
            /// 中文
            /// </summary>
            Chinese,
            /// <summary>
            /// 英文
            /// </summary>
            English,
            /// <summary>
            /// 日文
            /// </summary>
            Japanese
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {
            language = LANGUAGE.Chinese;
        }
        /// <summary>
        /// 默认值
        /// </summary>
        public static void SetDefault()
        {

        }
    }
}
