using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Tools
{
    /// <summary>
    /// 调整数字转字符串的格式。
    /// </summary>
    public class NumForm
    {
        /// <summary>
        /// 把一个数字转成字符串，保留 bit1 位整数位，保留 bit2 位小数位。
        /// </summary>
        /// <param name="num">数字</param>
        /// <param name="bit1">保留整数位数</param>
        /// <param name="bit2">保留小数位数</param>
        /// <returns></returns>
        public static string ToString(double num, int bit1, int bit2)
        {
            string str = num.ToString();
            int cut = str.IndexOf('.'); if (cut == -1) { cut = str.Length; }
            string str1 = str.Substring(0, cut);
            string str2 = str.Substring(cut + 1);

            if (bit1 > 0)
            {
                while (str1.Length < bit1) { str1 = '0' + str1; }
                str1 = str1.Substring(str1.Length - bit1);
            }
            if (bit2 > 0)
            {
                while (str2.Length < bit2) { str2 = str2 + '0'; }
                str2 = str2.Substring(0, bit2);
            }
            
            return str2.Length != 0 ? str1 + '.' + str2 : str1;
        }
    }
}
