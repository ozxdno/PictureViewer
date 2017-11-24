﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.FindForm
{
    class Config
    {
        /// <summary>
        /// 比较的结果
        /// </summary>
        public static List<List<int>> Results
        {
            get;
            set;
        }
        private static List<List<int>> results;
        /// <summary>
        /// 匹配的源图
        /// </summary>
        public static List<int> Sour
        {
            get { lock (Lock) { return sour; } }
            set { lock (Lock) { sour = value; } }
        }
        private static List<int> sour;
        /// <summary>
        /// 匹配的
        /// </summary>
        public static List<int> Dest
        {
            get { lock (Lock) { return dest; } }
            set { lock (Lock) { dest = value; } }
        }
        private static List<int> dest;
        /// <summary>
        /// 使用到的源图
        /// </summary>
        public static System.Drawing.Bitmap SourImage;
        /// <summary>
        /// 使用到的目标图
        /// </summary>
        public static System.Drawing.Bitmap DestImage;
        
        /// <summary>
        /// 源图的文件信息
        /// </summary>
        public static Files.BaseFileInfo SourFile = new Files.BaseFileInfo();
        /// <summary>
        /// 目标图的文件信息
        /// </summary>
        public static Files.BaseFileInfo DestFile = new Files.BaseFileInfo();
        
        /// <summary>
        /// 比较像素个数
        /// </summary>
        public static int Pixes
        {
            set;
            get;
        }
        private static int pixes;
        /// <summary>
        /// 相似程度
        /// </summary>
        public static int Degree
        {
            set { lock (Lock) { degree = value; } }
            get { lock (Lock) { return degree; } }
        }
        private static int degree;
        /// <summary>
        /// 需要比较行
        /// </summary>
        public static bool NeedCmpRow;
        /// <summary>
        /// 需要比较列
        /// </summary>
        public static bool NeedCmpCol;

        /// <summary>
        /// 图片窗口
        /// </summary>
        public static List<Form_Image> ImageForms = new List<Form_Image>();

        /// <summary>
        /// 尺寸必须相同
        /// </summary>
        public static bool Find_Full
        {
            set { lock (Lock) { find_full = value; } }
            get { lock (Lock) { return find_full; } }
        }
        private static bool find_full;
        /// <summary>
        /// 尺寸可以不同
        /// </summary>
        public static bool Find_Part;
        /// <summary>
        /// 比例必须相同
        /// </summary>
        public static bool Find_Same;
        /// <summary>
        /// 比例可以不同
        /// </summary>
        public static bool Find_Like;
        /// <summary>
        /// 经过旋转/翻转
        /// </summary>
        public static bool Find_Turn
        {
            set { lock (Lock) { find_turn = value; } }
            get { lock (Lock) { return find_turn; } }
        }
        private static bool find_turn;

        /// <summary>
        /// 查找的方式
        /// </summary>
        public static METHOD Method = METHOD.FIND_CURRENT;
        /// <summary>
        /// 查找的方式
        /// </summary>
        public enum METHOD
        {
            /// <summary>
            /// 查找当前文件
            /// </summary>
            FIND_CURRENT,
            /// <summary>
            /// 查找相似文件
            /// </summary>
            FIND_LIKE,
            /// <summary>
            /// 比较两个文件夹
            /// </summary>
            FIND_COMPARE
        }

        /// <summary>
        /// 快捷键 退出
        /// </summary>
        public static int FastKey_Esc
        {
            get;
            set;
        }
        /// <summary>
        /// 快捷键 导出
        /// </summary>
        public static int FastKey_Export
        {
            set;
            get;
        }
        /// <summary>
        /// 快捷键 左翻页
        /// </summary>
        public static int FastKey_L
        {
            set;
            get;
        }
        /// <summary>
        /// 快捷键 右翻页
        /// </summary>
        public static int FastKey_R
        {
            set;
            get;
        }
        /// <summary>
        /// 快捷键 上翻页
        /// </summary>
        public static int FastKey_U
        {
            set;
            get;
        }
        /// <summary>
        /// 快捷键 下翻页
        /// </summary>
        public static int FastKey_D
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

        /// <summary>
        /// 互斥锁
        /// </summary>
        private static object Lock = new object();
    }
}
