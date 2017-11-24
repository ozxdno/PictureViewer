using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.ImageForm
{
    class Config
    {
        public static int FastKey_Esc
        {
            set;
            get;
        }
        public static int FastKey_Enter
        {
            set;
            get;
        }
        public static int FastKey_Rotate
        {
            set;
            get;
        }
        public static int FastKey_FlipX
        {
            set;
            get;
        }
        public static int FastKey_FlipY
        {
            set;
            get;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {

        }
        /// <summary>
        /// 默认值
        /// </summary>
        public static void SetDefault()
        {
            if (FastKey_Esc <= 0) { FastKey_Esc = (int)System.ConsoleKey.Escape; }
            if (FastKey_Enter <= 0) { FastKey_Enter = (int)System.ConsoleKey.Enter; }
            if (FastKey_Rotate <= 0) { FastKey_Rotate = (int)System.ConsoleKey.R; }
            if (FastKey_FlipX <= 0) { FastKey_FlipX = (int)System.ConsoleKey.X; }
            if (FastKey_FlipY <= 0) { FastKey_FlipY = (int)System.ConsoleKey.Y; }
        }
    }
}
