using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Class
{
    /// <summary>
    /// 对 FileOperate 中的一些函数进行了更新，更新后的函数替换原来的函数。
    /// </summary>
    class FileSupport
    {
        ///////////////////////////////////////////////////// public attribute ///////////////////////////////////////////////


        ///////////////////////////////////////////////////// private attribute ///////////////////////////////////////////////
        
        private static List<string> ShowExtensions = new List<string>();
        private static List<string> HideExtensions = new List<string>();
        private static List<int> Types = new List<int>();

        ///////////////////////////////////////////////////// public method ///////////////////////////////////////////////
        
        /// <summary>
        /// 初始化所支持的类型文件。
        /// </summary>
        public static void Initialize()
        {
            // 0 - 等待开发支持程序
            // 1 - 文件夹
            // 2 - 图片
            // 3 - GIF
            // 4 - 音频 / 视频
            // 5 - ZIP

            ShowExtensions.Add(""); HideExtensions.Add(""); Types.Add(1);
            ShowExtensions.Add(".jpg"); HideExtensions.Add(".pv1"); Types.Add(2);
            ShowExtensions.Add(".jpeg"); HideExtensions.Add(".pv2"); Types.Add(2);
            ShowExtensions.Add(".png"); HideExtensions.Add(".pv3"); Types.Add(2);
            ShowExtensions.Add(".bmp"); HideExtensions.Add(".pv4"); Types.Add(2);
            ShowExtensions.Add(".gif"); HideExtensions.Add(".pv5"); Types.Add(3);
            ShowExtensions.Add(".avi"); HideExtensions.Add(".pv6"); Types.Add(4);
            ShowExtensions.Add(".mp4"); HideExtensions.Add(".pv7"); Types.Add(4);
            ShowExtensions.Add(".webm"); HideExtensions.Add(".pv8"); Types.Add(0);
            ShowExtensions.Add(".zip"); HideExtensions.Add(".pv9"); Types.Add(5);
            ShowExtensions.Add(".mp3"); HideExtensions.Add(".pv10"); Types.Add(4);
        }
        
        /// <summary>
        /// 获取文件类型，不存在该类型返回 -1。
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        public static int getFileType(string extension)
        {
            for (int i = 0; i < Types.Count; i++)
            {
                if (ShowExtensions[i] == extension) { return Types[i]; }
                if (HideExtensions[i] == extension) { return Types[i]; }
            }

            return -1;
        }

        /// <summary>
        /// 判断文件是否支持（文件夹类型返回 FALSE ）
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        public static bool IsSupport(string extension)
        {
            if (extension == "") { return false; }

            for (int i = 0; i < Types.Count; i++)
            {
                if (ShowExtensions[i] == extension) { return true; }
                if (HideExtensions[i] == extension) { return true; }
            }

            return false;
        }

        /// <summary>
        /// 判断文件是否支持（文件夹类型返回 FALSE ）
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        public static bool IsSupportHide(string extension)
        {
            if (extension == "") { return false; }

            for (int i = 0; i < Types.Count; i++)
            {
                //if (ShowExtensions[i] == extension) { return true; }
                if (HideExtensions[i] == extension) { return true; }
            }

            return false;
        }

        /// <summary>
        /// 把隐式后缀转换为显式后缀。
        /// </summary>
        /// <param name="hideExtension">隐式后缀</param>
        /// <returns></returns>
        public static string getShowExtension(string hideExtension)
        {
            for (int i = 0; i < Types.Count; i++)
            {
                if (ShowExtensions[i] == hideExtension) { return ShowExtensions[i]; }
                if (HideExtensions[i] == hideExtension) { return ShowExtensions[i]; }
            }

            return null;
        }

        /// <summary>
        /// 把显式后缀转换为隐式后缀。
        /// </summary>
        /// <param name="showExtension">显式后缀</param>
        /// <returns></returns>
        public static string getHideExtension(string showExtension)
        {
            for (int i = 0; i < Types.Count; i++)
            {
                if (ShowExtensions[i] == showExtension) { return HideExtensions[i]; }
                if (HideExtensions[i] == showExtension) { return HideExtensions[i]; }
            }

            return null;
        }

        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////
    }
}
