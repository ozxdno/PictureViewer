using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// unk.tip 文件信息表
    /// </summary>
    public class TipUnkInfo
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
        /// 创建 unk.tip 文件信息
        /// </summary>
        public TipUnkInfo()
        {
            Model = new TipModel(Tools.FileOperate.getExePath(), "unk.tip");
            Model.Type = FileType.Gif;
        }
    }
}
