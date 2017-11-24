using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.SearchForm
{
    class Config
    {
        /// <summary>
        /// 当前选中的索引号
        /// </summary>
        public static Files.Index Index
        {
            set;
            get;
        }

        public static int FastKey_Esc
        {
            set;
            get;
        }
        public static int FastKey_Enter
        {
            set;
            get;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {

        }
        /// <summary>
        /// 默认值
        /// </summary>
        public static void SetDefault()
        {

        }
    }
}
