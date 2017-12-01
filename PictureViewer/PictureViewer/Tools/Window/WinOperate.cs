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

        /// <summary>
        /// 把宽 width 高 height 的方框放入宽 rectw 高 recth 的方框中，返回最大的尺寸。
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="rectw"></param>
        /// <param name="recth"></param>
        /// <returns></returns>
        public static System.Drawing.Size FitRect(int width, int height, int rectw, int recth)
        {
            double rate1 = (double)recth / height;
            double rate2 = (double)rectw / width;
            double rate = Math.Min(rate1, rate2);

            int h = (int)(height * rate);
            int w = (int)(width * rate);
            return new System.Drawing.Size(w, h);
        }
        /// <summary>
        /// 获取宽 width 高 height 方框的缩放比率。
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static double GetShapeRate(int width, int height)
        {
            double rate1 = (double)width / Screen.Width;
            double rate2 = (double)height / Screen.Height;
            double rate = Math.Max(rate1, rate2);

            return rate;
        }
    }
}
