using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 文件的索引号
    /// </summary>
    class Index
    {
        /// <summary>
        /// 根目录索引号
        /// </summary>
        public int Folder
        {
            get;
            set;
        }
        /// <summary>
        /// 在该根目录下的索引号
        /// </summary>
        public int File
        {
            get;
            set;
        }
        /// <summary>
        /// 在子目录下的索引号。（文件不存在子目录则为 -1）
        /// </summary>
        public int Sub
        {
            get;
            set;
        }

        /// <summary>
        /// 在文件信息总集中的索引号
        /// </summary>
        public int Base
        {
            get
            {
                return getBaseIndex(Folder, File, Sub);
            }
        }
        /// <summary>
        /// 在文件信息总集中的索引号（经过修正）
        /// </summary>
        public int FitBase
        {
            get { return getBaseIndex(Folder, File, Sub, true); }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public Index()
        {
            Folder = 0;
            File = 0;
            Sub = 0;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="file"></param>
        /// <param name="sub"></param>
        public Index(int folder, int file, int sub)
        {
            Folder = folder;
            File = file;
            Sub = sub;
        }

        /// <summary>
        /// 获取在文件信息总集中的索引号
        /// </summary>
        /// <param name="index">目录索引</param>
        /// <returns></returns>
        public static int getBaseIndex(Index index, bool fit = false)
        {
            return getBaseIndex(index.Folder, index.File, index.Sub, fit);
        }
        /// <summary>
        /// 获取在文件信息总集中的索引号
        /// </summary>
        /// <param name="folder">根目录</param>
        /// <param name="file">文件序号</param>
        /// <param name="sub">子序号</param>
        /// <param name="fit">是否自动修正</param>
        /// <returns></returns>
        public static int getBaseIndex(int folder, int file, int sub = 0, bool fit = false)
        {
            if (Config.Trees.Count == 0) { return -1; }
            if (folder < 0) { if (!fit) { return -1; } folder = 0; }
            int n = Config.Trees.Count;
            if (folder >= n) { if (!fit) { return -1; } folder = n - 1; }

            if (Config.Trees[folder].Count == 0) { return -1; }
            if (file < 0) { if (!fit) { return -1; } file = 0; }
            n = Config.Trees[folder].Count;
            if (file >= n) { if (!fit) { return -1; } file = n - 1; }

            if (Config.Trees[folder][file].Count == 0) { return -1; }
            if (sub < 0) { if (!fit) { return -1; } sub = 0; }
            n = Config.Trees[folder][file].Count;
            if (sub >= n) { if (!fit) { return -1; } sub = n - 1; }

            return Config.Trees[folder][file][sub];
        }

        /// <summary>
        /// 文件索引号是否相同
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Index left, Index right)
        {
            if (left.Folder != right.Folder) { return false; }
            if (left.File != right.File) { return false; }
            if (left.Sub != right.Sub) { return false; }
            return true;
        }
        /// <summary>
        /// 文件索引号是否不同
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Index left, Index right)
        {
            if (left.Folder != right.Folder) { return true; }
            if (left.File != right.File) { return true; }
            if (left.Sub != right.Sub) { return true; }
            return false;
        }
        /// <summary>
        /// 判断文件的前后顺序
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(Index left, Index right)
        {
            if (left.Folder < right.Folder) { return false; }
            if (left.Folder > right.Folder) { return true; }

            if (left.File < right.File) { return false; }
            if (left.File > right.File) { return true; }

            if (left.Sub < right.Sub) { return false; }
            if (left.Sub > right.Sub) { return true; }

            return false;
        }
        /// <summary>
        /// 判断文件的前后顺序
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(Index left, Index right)
        {
            if (left.Folder < right.Folder) { return true; }
            if (left.Folder > right.Folder) { return false; }

            if (left.File < right.File) { return true; }
            if (left.File > right.File) { return false; }

            if (left.Sub < right.Sub) { return true; }
            if (left.Sub > right.Sub) { return false; }

            return false;
        }
    }
}
