using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Tools
{
    /// <summary>
    /// 语言
    /// </summary>
    public enum Language
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
    /// 设置当前语言
    /// </summary>
    public class SetLanguage
    {
        /// <summary>
        /// 当前选择的语言
        /// </summary>
        public static Language Current
        {
            get { return current; }
            set { current = value; }
        }
        private static Language current = Language.Chinese;
    }
}
