using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    class Config
    {
        /// <summary>
        /// 文件总集（文件序列表）
        /// </summary>
        public static List<BaseFileInfo> Files = new List<BaseFileInfo>();
        /// <summary>
        /// 文件总集（文件树形表）
        /// </summary>
        public static List<List<List<int>>> Trees = new List<List<List<int>>>();

        /// <summary>
        /// 文件序号总集
        /// </summary>
        public static List<Index> Indexes = new List<Index>();

        /// <summary>
        /// 是否需要加载隐藏文件
        /// </summary>
        public static bool IsSupportHide
        {
            get;
            set;
        }
        /// <summary>
        /// 支持文件列表
        /// </summary>
        public static List<Support> Supports = new List<Support>();

        /// <summary>
        /// 必要的常量文件
        /// </summary>
        public static ConstFiles ConstFiles = new ConstFiles();

        /// <summary>
        /// 输出路径
        /// </summary>
        public static string ExportPath
        {
            get { return (ExportPath == null || ExportPath.Length == 0) ? Operate.getExePath() : ExportPath; }
            set { ExportPath = value; }
        }
        /// <summary>
        /// 根目录
        /// </summary>
        public static List<string> RootPathes = new List<string>();

        /// <summary>
        /// 提示：错误
        /// </summary>
        public static System.Drawing.Bitmap ErrTip = null;
        /// <summary>
        /// 提示：文件不支持
        /// </summary>
        public static System.Drawing.Bitmap UnpTip = null;
        /// <summary>
        /// 提示：未知错误
        /// </summary>
        public static System.Drawing.Bitmap UnkTip = null;
        /// <summary>
        /// 提示：文件不存在
        /// </summary>
        public static System.Drawing.Bitmap NotTip = null;

        /// <summary>
        /// 加载文件专用线程
        /// </summary>
        public static System.Threading.Thread Loading;
    }
}
