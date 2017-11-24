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
