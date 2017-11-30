using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Tools
{
    /// <summary>
    /// 鼠标事件模型
    /// </summary>
    public class MouseModel
    {
        /// <summary>
        /// 左键
        /// </summary>
        public KeyModel Left
        {
            set;
            get;
        }
        /// <summary>
        /// 右键
        /// </summary>
        public KeyModel Right
        {
            set;
            get;
        }
        /// <summary>
        /// 中间键
        /// </summary>
        public KeyModel Centre
        {
            set;
            get;
        }
        /// <summary>
        /// 滚轮
        /// </summary>
        public KeyModel Wheel
        {
            set;
            get;
        }
        /// <summary>
        /// 滑动量
        /// </summary>
        public int Delta
        {
            set;
            get;
        }

        /// <summary>
        /// 创建鼠标事件模型
        /// </summary>
        public MouseModel()
        {
            Left = new KeyModel(1);
            Right = new KeyModel(1);
            Centre = new KeyModel(1);
            Wheel = new KeyModel(1);

            Delta = 0;
        }
    }
}
