using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// not.tip 文件信息表
    /// </summary>
    public class TipNotInfo
    {
        /// <summary>
        /// 文件信息
        /// </summary>
        public TipModel Model
        {
            get;
            set;
        }

        /// <summary>
        /// 创建 not.tip 文件信息
        /// </summary>
        public TipNotInfo()
        {
            Model = new TipModel(Tools.FileOperate.getExePath(), "not.tip");
            Model.Type = FileType.Gif;
        }
    }
}
