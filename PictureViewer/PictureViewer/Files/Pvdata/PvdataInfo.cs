using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// Pvdata 文件信息表
    /// </summary>
    public class PvdataInfo
    {
        /// <summary>
        /// 信息模型
        /// </summary>
        public TextModel Model
        {
            set;
            get;
        }
        
        /// <summary>
        /// 创建 Pvdata 文件信息
        /// </summary>
        public PvdataInfo()
        {
            Model = new TextModel(Tools.FileOperate.getExePath(), "pv.pvdata");
        }
    }
}
