using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Tools
{
    /// <summary>
    /// 按键事件模型
    /// </summary>
    public class KeyModel
    {
        /// <summary>
        /// 键值（按键的唯一判别号）
        /// </summary>
        public int KeyValue
        {
            set;
            get;
        }

        /// <summary>
        /// 是否按下
        /// </summary>
        public bool Down
        {
            set;
            get;
        }
        /// <summary>
        /// 是否抬起
        /// </summary>
        public bool Up
        {
            set;
            get;
        }
        /// <summary>
        /// 按下次数
        /// </summary>
        public int nDown
        {
            set;
            get;
        }
        /// <summary>
        /// 抬起次数
        /// </summary>
        public int nUp
        {
            set;
            get;
        }
        /// <summary>
        /// 按下的时间
        /// </summary>
        public long tDown
        {
            set;
            get;
        }
        /// <summary>
        /// 抬起的时间
        /// </summary>
        public long tUp
        {
            set;
            get;
        }

        /// <summary>
        /// 在对按下键进行计数时，计数的持续时间。单位：ms
        /// </summary>
        public long CntDownLasting
        {
            set;
            get;
        }
        private long cntDownBegin
        {
            set;
            get;
        }
        private long cntDownEnd
        {
            get { return cntDownBegin + CntDownLasting; }
        }

        /// <summary>
        /// 在对抬起键进行计数时，计数的持续时间。单位：ms
        /// </summary>
        public long CntUpLasting
        {
            set;
            get;
        }
        private long cntUpBegin
        {
            set;
            get;
        }
        private long cntUpEnd
        {
            get { return cntUpBegin + CntUpLasting; }
        }

        /// <summary>
        /// 是否被明显的拖动了
        /// </summary>
        public bool IsDraged
        {
            get
            {
                System.Drawing.Point now = System.Windows.Forms.Control.MousePosition;
                int xmove = now.X - pMouseDown.X;
                int ymove = now.Y - pMouseDown.Y;

                return Math.Abs(xmove) > 10 || Math.Abs(ymove) > 10;
            }
        }
        /// <summary>
        /// 是否被连续点击两次以上
        /// </summary>
        public bool IsDoubleClicked
        {
            get { return nUp > 1; }
        }

        /// <summary>
        /// 窗体 / 控件的位置
        /// </summary>
        public System.Drawing.Point pForm
        {
            set;
            get;
        }
        /// <summary>
        /// 键按下时的鼠标位置
        /// </summary>
        public System.Drawing.Point pMouseDown
        {
            set;
            get;
        }
        /// <summary>
        /// 键按抬起的鼠标位置
        /// </summary>
        public System.Drawing.Point pMouseUp
        {
            set;
            get;
        }
        /// <summary>
        /// X 轴控件的滑动量
        /// </summary>
        public int xScroll
        {
            set;
            get;
        }
        /// <summary>
        /// Y 轴控件的滑动量
        /// </summary>
        public int yScroll
        {
            set;
            get;
        }

        /// <summary>
        /// 创建按键事件的模型
        /// </summary>
        /// <param name="keyValue">键值</param>
        public KeyModel(int keyValue = -1)
        {
            KeyValue = keyValue;
            Down = false;
            Up = false;
            nDown = 0;
            nUp = 0;
            tDown = 0;
            tUp = 0;
            cntDownBegin = 0;
            CntDownLasting = 200;
            cntUpBegin = 0;
            CntUpLasting = 200;
            pForm = new System.Drawing.Point(0, 0);
            pMouseDown = new System.Drawing.Point(0, 0);
            pMouseUp = new System.Drawing.Point(0, 0);
            xScroll = 0;
            yScroll = 0;
        }
        /// <summary>
        /// 创建按键事件的模型
        /// </summary>
        /// <param name="key">按键</param>
        public KeyModel(System.ConsoleKey key)
        {
            KeyValue = (int)key;
            Down = false;
            Up = false;
            nDown = 0;
            nUp = 0;
            tDown = 0;
            tUp = 0;
            cntDownBegin = 0;
            CntDownLasting = 200;
            cntUpBegin = 0;
            CntUpLasting = 200;
            pForm = new System.Drawing.Point(0, 0);
            pMouseDown = new System.Drawing.Point(0, 0);
            pMouseUp = new System.Drawing.Point(0, 0);
            xScroll = 0;
            yScroll = 0;
        }

        /// <summary>
        /// 按键按下
        /// </summary>
        public void KeyDown()
        {
            Down = true;
            Up = false;
            tDown = Tools.Time.Ticks;
            if (tDown < cntDownEnd) { nDown++; } else { nDown = 1; }
            pMouseDown = System.Windows.Forms.Control.MousePosition;
        }
        /// <summary>
        /// 按键按下
        /// </summary>
        /// <param name="pForm">更新窗体 / 控件的位置</param>
        public void KeyDown(System.Drawing.Point pForm)
        {
            this.pForm = pForm;
            KeyDown();
        }
        /// <summary>
        /// 按键按下
        /// </summary>
        /// <param name="xScroll">更新 X 轴滑动量</param>
        /// <param name="yScroll">更新 Y 轴滑动量</param>
        public void KeyDown(int xScroll, int yScroll)
        {
            this.xScroll = xScroll;
            this.yScroll = yScroll;
            KeyDown();
        }
        /// <summary>
        /// 按键按下
        /// </summary>
        /// <param name="pForm">更新窗体 / 控件的位置</param>
        /// <param name="xScroll">更新 X 轴滑动量</param>
        /// <param name="yScroll">更新 Y 轴滑动量</param>
        public void KeyDown(System.Drawing.Point pForm, int xScroll, int yScroll)
        {
            this.pForm = pForm;
            this.xScroll = xScroll;
            this.yScroll = yScroll;
            KeyDown();
        }


        /// <summary>
        /// 按键抬起
        /// </summary>
        public void KeyUp()
        {
            Down = false;
            Up = true;
            tUp = Tools.Time.Ticks;
            if (tUp < cntUpEnd) { nUp++; } else { nUp = 1; }
            pMouseUp = System.Windows.Forms.Control.MousePosition;
        }
        /// <summary>
        /// 按键抬起
        /// </summary>
        /// <param name="pForm">更新窗体 / 控件的位置</param>
        public void KeyUp(System.Drawing.Point pForm)
        {
            this.pForm = pForm;
            KeyUp();
        }
        /// <summary>
        /// 按键抬起
        /// </summary>
        /// <param name="xScroll">更新 X 轴滑动量</param>
        /// <param name="yScroll">更新 Y 轴滑动量</param>
        public void KeyUp(int xScroll, int yScroll)
        {
            this.xScroll = xScroll;
            this.yScroll = yScroll;
            KeyUp();
        }
        /// <summary>
        /// 按键抬起
        /// </summary>
        /// <param name="pForm">更新窗体 / 控件的位置</param>
        /// <param name="xScroll">更新 X 轴滑动量</param>
        /// <param name="yScroll">更新 Y 轴滑动量</param>
        public void KeyUp(System.Drawing.Point pForm, int xScroll, int yScroll)
        {
            this.pForm = pForm;
            this.xScroll = xScroll;
            this.yScroll = yScroll;
            KeyUp();
        }
    }
}
