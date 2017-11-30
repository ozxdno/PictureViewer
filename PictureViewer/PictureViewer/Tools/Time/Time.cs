using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Tools
{
    /// <summary>
    /// 时间标签
    /// </summary>
    public class Time
    {
        /// <summary>
        /// 同一时间，单位：ms
        /// </summary>
        public static long Ticks
        {
            get { return System.DateTime.Now.Ticks / 1000; }
        }
    }
}
