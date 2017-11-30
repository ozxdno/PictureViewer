using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Tools
{
    /// <summary>
    /// 屏幕参数
    /// </summary>
    public class Screen
    {
        /// <summary>
        /// 高，单位：像素
        /// </summary>
        public static int Height
        {
            get { return System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height; }
        }
        /// <summary>
        /// 宽，单位：像素
        /// </summary>
        public static int Width
        {
            get { return System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width; }
        }

        /// <summary>
        /// 窗体最小的比例
        /// </summary>
        public static double MinWindowRate
        {
            get { return 0.10; }
        }
        /// <summary>
        /// 窗体最大的比例
        /// </summary>
        public static double MaxWindowRate
        {
            get { return 1.00; }
        }

        /// <summary>
        /// 控件最小的比例
        /// </summary>
        public static double MinControlRate
        {
            get { return 0.10; }
        }
        /// <summary>
        /// 控件最大的比例
        /// </summary>
        public static double MaxControlRate
        {
            get { return double.MaxValue; }
        }

        /// <summary>
        /// 窗体最小允许高度
        /// </summary>
        public static int MinWindowHeight
        {
            get { return (int)(Height * MinWindowRate); }
        }
        /// <summary>
        /// 窗体最小允许宽度
        /// </summary>
        public static int MinWindowWidth
        {
            get { return (int)(Width * MinWindowRate); }
        }
        /// <summary>
        /// 窗体最小允许边长
        /// </summary>
        public static int MinWindowSide
        {
            get { return Math.Max(MinWindowHeight, MinWindowWidth); }
        }

        /// <summary>
        /// 窗体最大允许高度
        /// </summary>
        public static int MaxWindowHeight
        {
            get { return (int)(Height * MaxWindowRate); }
        }
        /// <summary>
        /// 窗体最大允许宽度
        /// </summary>
        public static int MaxWindowWidth
        {
            get { return (int)(Width * MaxWindowRate); }
        }
        /// <summary>
        /// 窗体最大允许边长
        /// </summary>
        public static int MaxWindowSide
        {
            get { return Math.Min(MaxWindowHeight, MaxWindowWidth); }
        }
    }
}
