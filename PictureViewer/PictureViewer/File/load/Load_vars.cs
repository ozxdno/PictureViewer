using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    class Load_vars
    {
        public static string ToDirectory(string str)
        {
            bool exist = System.IO.Directory.Exists(str);
            string value = "";
            if (exist) { value = str; } else { value = ""; }
            return value;
        }
        /// <summary>
        /// 把读入字符串转换为 BOOL 值
        /// </summary>
        /// <param name="str">读入字符串</param>
        /// <param name="value">转换值</param>
        /// <returns></returns>
        public static bool ToBool(string str, ref bool value)
        {
            try
            {
                value = int.Parse(str) != 0;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static int ToInt(string str)
        {
            try
            {
                int value = int.Parse(str);
                return value;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 把读入字符串转换为 LONG 值
        /// </summary>
        /// <param name="str">读入字符串</param>
        /// <param name="value">转换值</param>
        /// <returns></returns>
        public static bool ToLong(string str, ref long value)
        {
            try
            {
                value = long.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 把读入字符串转换为 DOUBLE 值
        /// </summary>
        /// <param name="str">读入字符串</param>
        /// <param name="value">转换值</param>
        /// <returns></returns>
        public static bool ToDouble(string str, ref double value)
        {
            try
            {
                value = double.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 把读入字符串转换为表
        /// </summary>
        /// <param name="str">读入字符串</param>
        /// <param name="value">转换值</param>
        /// <returns></returns>
        public static bool ToIntList(string str, ref List<int> value)
        {
            try
            {
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 把读入字符串转换为数组
        /// </summary>
        /// <param name="str">读入字符串</param>
        /// <param name="value">转换值</param>
        /// <returns></returns>
        public static bool ToIntArray(string str, ref List<int> value)
        {
            try
            {
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<string> ToStringList(string str)
        {
            List<string> temp = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return temp;
        }
        public static List<Support> ToSupportList(string str)
        {
            List<string> temp = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return temp;
        }
        /// <summary>
        /// 把读入字符串转换为文件信息
        /// </summary>
        /// <param name="str">读入字符串</param>
        /// <param name="value">转换值</param>
        /// <returns></returns>
        public static bool ToFileInfo(string str, ref BaseFileInfo value)
        {
            try
            {
                return value.ToBaseFileInfo(str);
            }
            catch
            {
                return false;
            }
        }
    }
}
