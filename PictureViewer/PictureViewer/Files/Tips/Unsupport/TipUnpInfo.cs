using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// unp.tip 文件信息表
    /// </summary>
    public class TipUnpInfo
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
        /// 创建 unp.tip 文件信息
        /// </summary>
        public TipUnpInfo()
        {
            Model = new TipModel(Tools.FileOperate.getExePath(), "unp.tip");
            Model.Type = FileType.Gif;
        }
    }
}
