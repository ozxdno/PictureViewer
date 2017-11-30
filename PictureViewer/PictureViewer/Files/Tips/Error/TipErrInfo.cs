using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// err.tip 文件信息表
    /// </summary>
    public class TipErrInfo
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
        /// 创建 err.tip 文件信息
        /// </summary>
        public TipErrInfo()
        {
            Model = new TipModel(Tools.FileOperate.getExePath(), "err.tip");
            Model.Type = FileType.Gif;
        }
    }
}
