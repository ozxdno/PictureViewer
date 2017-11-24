using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.MainForm
{
    class Config
    {
        /// <summary>
        /// 索引号
        /// </summary>
        public static Files.Index TreeIndex
        {
            set;
            get;
        }
        /// <summary>
        /// 索引号
        /// </summary>
        public static int FileIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 源图
        /// </summary>
        public static System.Drawing.Image SourPicture
        {
            get;
            set;
        }
        /// <summary>
        /// 目标图
        /// </summary>
        public static System.Drawing.Image DestPicture
        {
            set;
            get;
        }
        /// <summary>
        /// 图片信息
        /// </summary>
        public static Files.BaseFileInfo FileInfo
        {
            set;
            get;
        }

        /// <summary>
        /// 图片窗口
        /// </summary>
        public static List<Form_Image> ImageForms
        {
            set;
            get;
        }

        /// <summary>
        /// 鼠标信息
        /// </summary>
        public static CommonForm.Mouse Mouse
        {
            set;
            get;
        }
        /// <summary>
        /// 播放设置
        /// </summary>
        public static Play Play
        {
            set;
            get;
        }
        /// <summary>
        /// 方向键
        /// </summary>
        public static DirectionKey DirKey
        {
            set;
            get;
        }

        /// <summary>
        /// 隐藏左翻页键
        /// </summary>
        public static bool HideL
        {
            set;
            get;
        }
        /// <summary>
        /// 隐藏右翻页键
        /// </summary>
        public static bool HideR
        {
            set;
            get;
        }
        /// <summary>
        /// 隐藏上翻页键
        /// </summary>
        public static bool HideU
        {
            set;
            get;
        }
        /// <summary>
        /// 隐藏下翻页键
        /// </summary>
        public static bool HideD
        {
            set;
            get;
        }

        /// <summary>
        /// 控件缩放比例
        /// </summary>
        public static double ShapeControlRate
        {
            get;
            set;
        }
        /// <summary>
        /// 图片缩放比例
        /// </summary>
        public static double ShapeWindowRate
        {
            set;
            get;
        }
        /// <summary>
        /// 用小窗体打开
        /// </summary>
        public static bool SmallWindow
        {
            set;
            get;
        }
        /// <summary>
        /// 使用边框
        /// </summary>
        public static bool UseBoard
        {
            set;
            get;
        }
        /// <summary>
        /// 自动调整窗体大小
        /// </summary>
        public static bool ShapeWindow
        {
            set;
            get;
        }
        /// <summary>
        /// 锁定某些操作不被触发
        /// </summary>
        public static bool LockOpreate
        {
            set;
            get;
        }
        /// <summary>
        /// 最小缩放比例
        /// </summary>
        public static double MinRate
        {
            set;
            get;
        }
        /// <summary>
        /// 最大缩放比例
        /// </summary>
        public static double MaxRate
        {
            set;
            get;
        }


        /// <summary>
        /// 窗体高度
        /// </summary>
        public static int Height
        {
            set;
            get;
        }
        /// <summary>
        /// 窗体宽度
        /// </summary>
        public static int Width
        {
            get;
            set;
        }
        /// <summary>
        /// 窗体起始位置
        /// </summary>
        public static System.Drawing.Point Location
        {
            set;
            get;
        }
        /// <summary>
        /// 起始位置 X 坐标
        /// </summary>
        public static int LocationX
        {
            set { System.Drawing.Point pt = Location; pt.X = value; Location = pt; }
            get { return Location.X; }
        }
        /// <summary>
        /// 起始位置 Y 坐标
        /// </summary>
        public static int LocationY
        {
            set { System.Drawing.Point pt = Location; pt.Y = value; Location = pt; }
            get { return Location.Y; }
        }


        /// <summary>
        /// 切换到当前文件
        /// </summary>
        public static bool Switch
        {
            get;
            set;
        }
        /// <summary>
        /// 是否显示提示信息
        /// </summary>
        public static bool Tip
        {
            set;
            get;
        }
        /// <summary>
        /// 窗体是否被激活
        /// </summary>
        public static bool IsActive
        {
            get;
            set;
        }

        public static int FastKey_ESC
        {
            set;
            get;
        }
        public static int FastKey_Export
        {
            set;
            get;
        }
        public static int FastKey_L
        {
            set;
            get;
        }
        public static int FastKey_R
        {
            set;
            get;
        }
        public static int FastKey_U
        {
            set;
            get;
        }
        public static int FastKey_D
        {
            set;
            get;
        }
        public static int FastKey_PageU
        {
            set;
            get;
        }
        public static int FastKey_PageD
        {
            set;
            get;
        }
        public static int FastKey_Board
        {
            set;
            get;
        }
        public static int FastKey_Enter
        {
            set;
            get;
        }
        public static int FastKey_OpenComic
        {
            set;
            get;
        }
        public static int FastKey_OpenCurrent
        {
            set;
            get;
        }
        public static int FastKey_OpenRoot
        {
            set;
            get;
        }
        public static int FastKey_OpenExport
        {
            set;
            get;
        }
        public static int FastKey_Password
        {
            set;
            get;
        }
        public static int FastKey_Rotate
        {
            set;
            get;
        }
        public static int FastKey_FlipX
        {
            set;
            get;
        }
        public static int FastKey_FlipY
        {
            set;
            get;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {
            TreeIndex = new Files.Index();
            FileIndex = 0;
            SourPicture = null;
            DestPicture = null;
            FileInfo = new Files.BaseFileInfo();
            ImageForms = new List<Form_Image>();
            Mouse = new CommonForm.Mouse();
            Play = new Play();
            DirKey = new DirectionKey();
            HideL = false;
            HideR = false;
            HideU = false;
            HideD = false;
        }
        /// <summary>
        /// 默认值
        /// </summary>
        public static void SetDefault()
        {

        }

        /// <summary>
        /// 互斥锁
        /// </summary>
        private static object Lock = new object();
    }
}
