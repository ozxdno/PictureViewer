using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// mus.tip 文件信息表
    /// </summary>
    public class TipMusInfo
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
        /// 创建 mus.tip 文件信息
        /// </summary>
        public TipMusInfo()
        {
            Model = new TipModel(Tools.FileOperate.getExePath(), "mus.tip");
            Model.Type = FileType.Gif;
        }
    }
}
