using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.FormMain
{
    class Tip
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public long Begin
        {
            get;
            set;
        }
        /// <summary>
        /// 结束时间
        /// </summary>
        public long End
        {
            get { return Begin + Lasting; }
        }
        /// <summary>
        /// 持续时间
        /// </summary>
        public long Lasting
        {
            set;
            get;
        }

        /// <summary>
        /// 鼠标盘旋的位置
        /// </summary>
        public System.Drawing.Point Position
        {
            set;
            get;
        }

        /// <summary>
        /// 窗体
        /// </summary>
        public Form_Tip TipForm
        {
            set;
            get;
        }
        /// <summary>
        /// 窗体是否可见
        /// </summary>
        public bool Visible
        {
            get { return TipForm.Visible; }
        }
        /// <summary>
        /// 隐藏一段时间
        /// </summary>
        public bool HideSomeTime
        {
            set;
            get;
        }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message
        {
            set;
            get;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public Tip()
        {
            Begin = 0;
            Lasting = 0;
            Position = new System.Drawing.Point(0, 0);
            TipForm = new Form_Tip();
            HideSomeTime = false;
            Message = "";
        }

        /// <summary>
        /// 是否到达最大显示时间
        /// </summary>
        /// <param name="time">当前时间</param>
        /// <returns></returns>
        public bool IsTimeOver(long time)
        {
            return time > End;
        }
        /// <summary>
        /// 是否仍在该位置盘旋
        /// </summary>
        /// <returns></returns>
        public bool IsHover()
        {
            System.Drawing.Point Now = System.Windows.Forms.Control.MousePosition;
            return Now.X == Position.X && Now.Y == Position.Y;
        }

        /// <summary>
        /// 展示提示信息
        /// </summary>
        public void Show()
        {
            TipForm.show(Message);
        }
        /// <summary>
        /// 隐藏提示信息
        /// </summary>
        public void Hide()
        {
            TipForm.hide();
        }
    }
}
