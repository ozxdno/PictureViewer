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
        public static List<BaseFileInfo> Files
        {
            get;
            set;
        }
        /// <summary>
        /// 文件总集（文件树形表）
        /// </summary>
        public static List<List<List<int>>> Trees
        {
            get;
            set;
        }
        
        /// <summary>
        /// 是否需要加载隐藏文件
        /// </summary>
        public static bool IsSupportHide
        {
            get;
            set;
        }
        /// <summary>
        /// 是否需要加载 ZIP 文件
        /// </summary>
        public static bool IsSupportZip
        {
            get;
            set;
        }

        /// <summary>
        /// 支持文件列表
        /// </summary>
        public static List<Support> Supports
        {
            set;
            get;
        }

        /// <summary>
        /// 必要的常量文件
        /// </summary>
        public static ConstFiles ConstFiles
        {
            set;
            get;
        }

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
        public static List<string> RootPathes
        {
            set;
            get;
        }

        /// <summary>
        /// 提示：错误
        /// </summary>
        public static System.Drawing.Bitmap ErrTip
        {
            set;
            get;
        }
        /// <summary>
        /// 提示：文件不支持
        /// </summary>
        public static System.Drawing.Bitmap UnpTip
        {
            set;
            get;
        }
        /// <summary>
        /// 提示：未知错误
        /// </summary>
        public static System.Drawing.Bitmap UnkTip
        {
            set;
            get;
        }
        /// <summary>
        /// 提示：文件不存在
        /// </summary>
        public static System.Drawing.Bitmap NotTip
        {
            set;
            get;
        }

        /// <summary>
        /// 加载文件专用线程
        /// </summary>
        public static System.Threading.Thread Loading
        {
            set;
            get;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {
            Files = new List<BaseFileInfo>();
            Trees = new List<List<List<int>>>();

            IsSupportHide = false;
            IsSupportZip = false;

            Supports = new List<Support>();
            ConstFiles = new ConstFiles();

            ExportPath = "";
            RootPathes = new List<string>();

            ErrTip = null;
            UnkTip = null;
            UnpTip = null;
            NotTip = null;

            Loading = null;

            new Load_pvini();
            Support.SetDefault();
        }

        /// <summary>
        /// 互斥锁
        /// </summary>
        private static object Lock = new object();
    }
}
