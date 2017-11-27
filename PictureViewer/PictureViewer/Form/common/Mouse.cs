using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.CommonForm
{
    class Mouse
    {
        /// <summary>
        /// 是否有键被按下
        /// </summary>
        public bool IsDown
        {
            get { return Down1 || Down2 || Down3; }
        }
        /// <summary>
        /// 仅按下左键
        /// </summary>
        public bool OnlyDown1
        {
            get { return Down1 && !Down2 && !Down3; }
        }
        /// <summary>
        /// 仅按下左键
        /// </summary>
        public bool OnlyDown2
        {
            get { return !Down1 && Down2 && !Down3; }
        }
        /// <summary>
        /// 仅按下左键
        /// </summary>
        public bool OnlyDown3
        {
            get { return !Down1 && !Down2 && Down3; }
        }

        /// <summary>
        /// 左键按下
        /// </summary>
        public bool Down1
        {
            set;
            get;
        }
        /// <summary>
        /// 右键按下
        /// </summary>
        public bool Down2
        {
            set;
            get;
        }
        /// <summary>
        /// 中间键按下
        /// </summary>
        public bool Down3
        {
            set;
            get;
        }

        /// <summary>
        /// 左键抬起
        /// </summary>
        public bool Up1
        {
            set;
            get;
        }
        /// <summary>
        /// 右键抬起
        /// </summary>
        public bool Up2
        {
            set;
            get;
        }
        /// <summary>
        /// 中间键抬起
        /// </summary>
        public bool Up3
        {
            set;
            get;
        }

        /// <summary>
        /// 左键按下时鼠标的位置
        /// </summary>
        public System.Drawing.Point ptDown1
        {
            set;
            get;
        }
        /// <summary>
        /// 右键按下时鼠标的位置
        /// </summary>
        public System.Drawing.Point ptDown2
        {
            set;
            get;
        }
        /// <summary>
        /// 中间键按下时鼠标的位置
        /// </summary>
        public System.Drawing.Point ptDown3
        {
            set;
            get;
        }

        /// <summary>
        /// 左键抬起时鼠标的位置
        /// </summary>
        public System.Drawing.Point ptUp1
        {
            set;
            get;
        }
        /// <summary>
        /// 左键抬起时鼠标的位置
        /// </summary>
        public System.Drawing.Point ptUp2
        {
            set;
            get;
        }
        /// <summary>
        /// 左键抬起时鼠标的位置
        /// </summary>
        public System.Drawing.Point ptUp3
        {
            set;
            get;
        }

        /// <summary>
        /// 窗体位置
        /// </summary>
        public System.Drawing.Point posWindow
        {
            set;
            get;
        }
        /// <summary>
        /// X 方向滑动量
        /// </summary>
        public int xScroll
        {
            set;
            get;
        }
        /// <summary>
        /// Y 方向滑动量
        /// </summary>
        public int yScroll
        {
            set;
            get;
        }

        /// <summary>
        /// 鼠标滚轮滚动的滚动量
        /// </summary>
        public int Delta
        {
            set;
            get;
        }
        /// <summary>
        /// 鼠标滚轮滚动时的鼠标位置
        /// </summary>
        public System.Drawing.Point ptWheel
        {
            set;
            get;
        }
        /// <summary>
        /// 鼠标滚轮滚动时的时间
        /// </summary>
        public long tWheel
        {
            set;
            get;
        }

        /// <summary>
        /// 左键按下次数
        /// </summary>
        public int cntDown1
        {
            get;
            set;
        }
        /// <summary>
        /// 右键按下次数
        /// </summary>
        public int cntDown2
        {
            get;
            set;
        }
        /// <summary>
        /// 中间键按下次数
        /// </summary>
        public int cntDown3
        {
            get;
            set;
        }

        /// <summary>
        /// 左键抬起次数
        /// </summary>
        public int cntUp1
        {
            get;
            set;
        }
        public int cntUp2
        {
            get;
            set;
        }
        public int cntUp3
        {
            get;
            set;
        }

        public long tDown1
        {
            set;
            get;
        }
        public long tDown2
        {
            set;
            get;
        }
        public long tDown3
        {
            set;
            get;
        }

        public long tUp1
        {
            set;
            get;
        }
        public long tUp2
        {
            set;
            get;
        }
        public long tUp3
        {
            set;
            get;
        }

        /// <summary>
        /// 是否被双击
        /// </summary>
        public bool IsDoubleClicked
        {
            set;
            get;
        }
        /// <summary>
        /// 是否被单击
        /// </summary>
        public bool IsClicked
        {
            set;
            get;
        }
        /// <summary>
        /// 是否被拖拽
        /// </summary>
        public bool IsDraged
        {
            set;
            get;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public Mouse()
        {
            Down1 = false;
            Down2 = false;
            Down3 = false;
            Up1 = false;
            Up2 = false;
            Up3 = false;

            ptDown1 = new System.Drawing.Point(0, 0);
            ptDown2 = new System.Drawing.Point(0, 0);
            ptDown3 = new System.Drawing.Point(0, 0);
            ptUp1 = new System.Drawing.Point(0, 0);
            ptUp2 = new System.Drawing.Point(0, 0);
            ptUp3 = new System.Drawing.Point(0, 0);

            posWindow = new System.Drawing.Point(0, 0);
            xScroll = 0;
            yScroll = 0;

            Delta = 0;
            ptWheel = new System.Drawing.Point(0, 0);
            tWheel = 0;

            cntDown1 = 0;
            cntDown2 = 0;
            cntDown3 = 0;
            cntUp1 = 0;
            cntUp2 = 0;
            cntUp3 = 0;

            IsDoubleClicked = false;
            IsClicked = false;
            IsDraged = false;
        }

        /// <summary>
        /// 鼠标键被按下。键号：
        /// 1 - 左键；
        /// 2 - 右键；
        /// 3 - 中间键；
        /// </summary>
        /// <param name="button">键号</param>
        /// <param name="time">时间</param>
        public virtual void Down(int button, long time = 0)
        {
            if (IsDown) { return; }

            if (button == 1)
            {
                Down1 = true;
                Up1 = false;
                ptDown1 = System.Windows.Forms.Control.MousePosition;
                cntDown1++;
                tDown1 = time;
                return;
            }
            if (button == 2)
            {
                Down2 = true;
                Up2 = false;
                ptDown2 = System.Windows.Forms.Control.MousePosition;
                cntDown2++;
                tDown2 = time;
                return;
            }
            if (button == 3)
            {
                Down3 = true;
                Up3 = false;
                ptDown3 = System.Windows.Forms.Control.MousePosition;
                cntDown3++;
                tDown3 = time;
                return;
            }
        }
        /// <summary>
        /// 鼠标键已经抬起。键号：
        /// 1 - 左键；
        /// 2 - 右键；
        /// 3 - 中间键；
        /// </summary>
        /// <param name="button">键号</param>
        /// <param name="time">时间</param>
        public virtual void Up(int button, long time = 0)
        {
            if (button == 1 && OnlyDown1)
            {
                IsDoubleClicked = 0 < time - tUp1 && time - tUp1 < 200;
                IsDraged = IsDrag(System.Windows.Forms.Control.MousePosition, ptUp1);
                IsClicked = !IsDraged;

                Down1 = false;
                Up1 = true;
                ptUp1 = System.Windows.Forms.Control.MousePosition;
                cntUp1++;
                tUp1 = time;
                return;
            }
            if (button == 2)
            {
                Down2 = false;
                Up2 = true;
                ptUp2 = System.Windows.Forms.Control.MousePosition;
                cntUp2++;
                tUp2 = time;
                return;
            }
            if (button == 3)
            {
                Down3 = false;
                Up3 = true;
                ptUp3 = System.Windows.Forms.Control.MousePosition;
                cntUp3++;
                tUp3 = time;
                return;
            }
        }
        /// <summary>
        /// 鼠标滚轮滚动发生
        /// </summary>
        /// <param name="delta">移动量</param>
        /// <param name="time">时间</param>
        public virtual void Wheel(int delta, long time = 0)
        {
            Delta = delta;
            tWheel = time;
            ptWheel = System.Windows.Forms.Control.MousePosition;
        }
        


        private bool IsDrag(System.Drawing.Point pt1, System.Drawing.Point pt2)
        {
            int xmove = pt1.X - pt2.X;
            int ymove = pt1.Y - pt2.Y;

            return xmove > 10 || ymove > 10;
        }
    }
}
