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
        public static System.Drawing.Image SourPicture;
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
        public static CommonForm.Mouse Mouse = new CommonForm.Mouse();
        /// <summary>
        /// 播放设置
        /// </summary>
        public static Play Play = new Play();
        /// <summary>
        /// 方向键
        /// </summary>
        public static DirectionKey DirKey = new DirectionKey();

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
        public static int FastKey_PageUp
        {
            set;
            get;
        }
        public static int FastKey_PageDown
        {
            set;
            get;
        }

    }
}
