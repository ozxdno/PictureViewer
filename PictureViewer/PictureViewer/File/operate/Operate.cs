using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    class Operate
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
        /// 是否支持该类型的文件
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool IsSupport(string extension)
        {
            for (int i = 0; i < Config.Supports.Count; i++)
            {
                if (Config.Supports[i].ShowExtension == extension) { return true; }
                if (!Config.IsSupportHide) { continue; }
                if (Config.Supports[i].HideExtension == extension) { return true; }
            }

            return false;
        }

        /// <summary>
        /// 文件移动的结果。
        /// </summary>
        public enum FILE_MOVE_RESULT
        {
            /// <summary>
            /// 文件移动成功
            /// </summary>
            SUCCESSED,
            /// <summary>
            /// 取消了文件的移动
            /// </summary>
            CANCLED,

            /// <summary>
            /// 源文件不存在
            /// </summary>
            NOT_EXIST_SOUR_FILE,
            /// <summary>
            /// 源文件的文件夹不存在
            /// </summary>
            NOT_EXIST_SOUR_PATH,
            /// <summary>
            /// 目标文件不存在
            /// </summary>
            NOT_EXIST_DEST_FILE,
            /// <summary>
            /// 目标文件夹不存在
            /// </summary>
            NOT_EXIST_DEST_PATH,

            /// <summary>
            /// 目标文件已存在
            /// </summary>
            EXISTED_DEST_FILE,

            /// <summary>
            /// 源文件正在使用中
            /// </summary>
            SOUR_FILE_IS_USING
        }
        /// <summary>
        /// 移动文件（包括重命名），返回文件移动操作的结果。
        /// </summary>
        /// <param name="sour">源文件</param>
        /// <param name="dest">目标文件</param>
        /// <param name="showConfirm">显示确认提示框</param>
        /// <param name="showError">显示错误提示框</param>
        /// <returns></returns>
        public static FILE_MOVE_RESULT FileMove(
            string sour,
            string dest,
            bool showConfirm = false,
            bool showError = false)
        {
            FILE_MOVE_RESULT res = FILE_MOVE_RESULT.SUCCESSED;

            if (res == FILE_MOVE_RESULT.SUCCESSED && !System.IO.File.Exists(sour))
            { res = FILE_MOVE_RESULT.NOT_EXIST_DEST_FILE; }
            if (res == FILE_MOVE_RESULT.SUCCESSED && System.IO.File.Exists(dest))
            { res = FILE_MOVE_RESULT.EXISTED_DEST_FILE; }

            if (res == FILE_MOVE_RESULT.SUCCESSED)
            {
                if (showConfirm)
                {
                    if (!OK(Strings.ConfirmFileMove)) { return FILE_MOVE_RESULT.CANCLED; }
                }

                try
                {
                    System.IO.File.Move(sour, dest);
                }
                catch
                {
                    res = FILE_MOVE_RESULT.SOUR_FILE_IS_USING;
                }
            }

            if (res != FILE_MOVE_RESULT.SUCCESSED)
            {
                ShowFileMoveError(res);
            }

            return res;
        }
        /// <summary>
        /// 显示文件移动错误的提示框
        /// </summary>
        /// <param name="res">文件移动结果</param>
        public static void ShowFileMoveError(FILE_MOVE_RESULT res)
        {

        }

        /// <summary>
        /// 确认窗口
        /// </summary>
        /// <param name="text">确认窗口的显示文本</param>
        /// <returns></returns>
        public static bool OK(string text)
        {
            return System.Windows.Forms.DialogResult.OK ==
                System.Windows.Forms.MessageBox.Show(
                    text,
                    Strings.Confirm + Strings.Exclamation,
                    System.Windows.Forms.MessageBoxButtons.OKCancel
                    );
        }

        /// <summary>
        /// 获取指定文件在文件集中的索引号
        /// </summary>
        /// <param name="full">指定文件</param>
        /// <returns></returns>
        public static int getIndex(string full)
        {
            for (int i = 0; i < Config.Files.Count; i++)
            {
                if (Config.Files[i].Full == full) { return i; }
            }

            return -1;
        }
        /// <summary>
        /// 获取指定文件在文件集中的索引号
        /// </summary>
        /// <param name="path">指定文件路径</param>
        /// <param name="name">指定文件名称</param>
        /// <returns></returns>
        public static int getIndex(string path, string name)
        {
            for (int i = 0; i < Config.Files.Count; i++)
            {
                if (Config.Files[i].Path == path && Config.Files[i].Name == name) { return i; }
            }

            return -1;
        }
        /// <summary>
        /// 获取指定文件在文件集中的索引号
        /// </summary>
        /// <param name="full">指定文件</param>
        /// <returns></returns>
        public static int getIndex(Index index)
        {
            return getIndex(index.Folder, index.File, index.Sub);
        }
        /// <summary>
        /// 获取指定文件在文件集中的索引号
        /// </summary>
        /// <param name="full">指定文件</param>
        /// <returns></returns>
        public static int getIndex(int folder, int file, int sub = -1)
        {

            return -1;
        }
    }
}
