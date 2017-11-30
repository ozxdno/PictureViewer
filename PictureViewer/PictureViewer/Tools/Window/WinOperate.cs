using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Tools
{
    /// <summary>
    /// 窗体的一些操作
    /// </summary>
    public class WinOperate
    {
        /// <summary>
        /// 把窗体移到屏幕中央时，窗体所处的位置
        /// </summary>
        /// <param name="width">窗体宽</param>
        /// <param name="height">窗体高</param>
        /// <returns></returns>
        public static System.Drawing.Point GetLocation_ToCentre(int width, int height)
        {
            System.Drawing.Point centre = new System.Drawing.Point(Screen.Width / 2, Screen.Height / 2);
            return GetLocation_ToPoint(width, height, centre);
        }
        /// <summary>
        /// 把窗体中心移动到指定点后，窗体所处的位置。
        /// </summary>
        /// <param name="width">窗体宽</param>
        /// <param name="height">窗体高</param>
        /// <param name="centre">指定窗体中心位置</param>
        /// <returns></returns>
        public static System.Drawing.Point GetLocation_ToPoint(int width, int height, System.Drawing.Point centre)
        {
            int x = centre.X - width / 2;
            int y = centre.Y - height / 2;
            return new System.Drawing.Point(x, y);
        }
    }
}
