using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Tools
{
    /// <summary>
    /// 常用对话框工具
    /// </summary>
    public class Dialog
    {
        /// <summary>
        /// 确认对话框（只有确认键）
        /// </summary>
        /// <param name="text">确认文本</param>
        public static void Confirm(string text)
        {

        }
        /// <summary>
        /// 确认对话框（有取消按键）
        /// </summary>
        /// <param name="text">确认文本</param>
        /// <returns></returns>
        public static bool OK(string text)
        {
            return true;
        }
        /// <summary>
        /// 打开指定文件夹
        /// </summary>
        /// <param name="folder">指定文件夹</param>
        public static void OpenFolder(string folder)
        {

        }
        /// <summary>
        /// 打开资源管理器，转到指定文件。
        /// </summary>
        /// <param name="file">指定文件</param>
        public static void OpenFile(string file)
        {

        }
        /// <summary>
        /// 用指定的 EXE 程序打开指定的文件
        /// </summary>
        /// <param name="exeFile">EXE 程序</param>
        /// <param name="targetFile">待打开的文件</param>
        public static void OpenExe(string exeFile, string targetFile = "")
        {

        }
        /// <summary>
        /// 导入一个文件夹
        /// </summary>
        /// <param name="initPath">起始路径</param>
        /// <returns></returns>
        public static string InputFolder(string initPath = "")
        {
            return initPath;
        }
        /// <summary>
        /// 导入一个文件
        /// </summary>
        /// <param name="initPath">起始路径</param>
        /// <returns></returns>
        public static string InputFile(string initPath = "")
        {
            return initPath;
        }
        /// <summary>
        /// 输入一串字符串
        /// </summary>
        /// <param name="initString">起始字符串</param>
        /// <returns></returns>
        public static string InputString(string initString = "")
        {
            Form_Input input = new Form_Input(initString);
            input.Location = System.Windows.Forms.Control.MousePosition;
            input.ShowDialog();

            return input.Input;
        }
        /// <summary>
        /// 获取保存位置
        /// </summary>
        /// <param name="extensions">可选扩展名</param>
        /// <returns></returns>
        public static string SaveTo(List<string> extensions = null)
        {
            return "";
        }
    }
}
