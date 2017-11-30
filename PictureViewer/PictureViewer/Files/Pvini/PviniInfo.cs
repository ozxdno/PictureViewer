using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// Pvini 文件信息表
    /// </summary>
    public class PviniInfo
    {
        /// <summary>
        /// 文件信息
        /// </summary>
        public TextModel Model
        {
            get;
            set;
        }

        /// <summary>
        /// 创建 Pvini 文件信息
        /// </summary>
        public PviniInfo()
        {
            Model = new TextModel(Tools.FileOperate.getExePath(), "pv.pvini");
        }
    }
}
