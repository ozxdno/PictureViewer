using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// int.tip 文件信息表
    /// </summary>
    public class TipIntInfo
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
        /// 创建 int.tip 文件信息
        /// </summary>
        public TipIntInfo()
        {
            Model = new TipModel(Tools.FileOperate.getExePath(), "int.tip");
            Model.Type = FileType.Gif;
        }
    }
}
