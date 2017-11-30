using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Tools
{
    /// <summary>
    /// 对文件名的一些操作。
    /// </summary>
    public class FileOperate
    {
        /// <summary>
        /// 获取 EXE 文件所在的路径
        /// </summary>
        /// <returns></returns>
        public static string getExePath()
        {
            return System.Windows.Forms.Application.StartupPath;
        }
        /// <summary>
        /// 从完整的文件名中获取文件路径
        /// </summary>
        /// <param name="full">文件的完整文件名</param>
        /// <returns></returns>
        public static string getPath(string full)
        {
            if (full == null || full.Length == 0) { return ""; }
            int cut = full.LastIndexOf('\\'); if (cut == -1) { return ""; }
            return full.Substring(0, cut);
        }
        /// <summary>
        /// 从完整的文件名中获取文件名称
        /// </summary>
        /// <param name="full">文件的完整文件名</param>
        public static string getName(string full)
        {
            if (full == null || full.Length == 0) { return ""; }
            int cut = full.LastIndexOf('\\'); if (cut == -1) { return full; }
            return full.Substring(cut + 1);
        }
        /// <summary>
        /// 从文件名中获取不带后缀的文件名
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public static string getNameWithoutExtension(string name)
        {
            if (name == null || name.Length == 0) { return ""; }
            int dot = name.LastIndexOf('.'); if (dot == -1) { return name; }
            return name.Substring(0, dot);
        }
        /// <summary>
        /// 获取文件后缀
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public static string getExtension(string name)
        {
            if (name == null || name.Length == 0) { return ""; }
            int dot = name.LastIndexOf('.'); if (dot == -1) { return ""; }
            return name.Substring(dot).ToLower();
        }
        /// <summary>
        /// 某个文件（或路径）的名称中是否存在非法字符
        /// </summary>
        /// <param name="file">文件/路径</param>
        /// <returns></returns>
        public static bool isIllegal(string file)
        {
            if (file.IndexOf('?') != -1) { return false; }
            if (file.IndexOf('\\') != -1) { return false; }
            if (file.IndexOf('/') != -1) { return false; }
            if (file.IndexOf(':') != -1) { return false; }
            if (file.IndexOf('*') != -1) { return false; }
            if (file.IndexOf('"') != -1) { return false; }
            if (file.IndexOf('<') != -1) { return false; }
            if (file.IndexOf('>') != -1) { return false; }
            if (file.IndexOf('|') != -1) { return false; }

            return true;
        }
    }
}
