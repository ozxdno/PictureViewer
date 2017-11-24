using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 加载提示文件
    /// </summary>
    class Load_tip
    {
        /// <summary>
        /// 加载所有提示文件
        /// </summary>
        public Load_tip()
        {
            Load_Unk();
            Load_Unp();
            Load_Err();
            Load_Not();
        }

        /// <summary>
        /// 加载 unk.tip
        /// </summary>
        public void Load_Unk()
        {
            try
            {
                Config.UnkTip = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(Config.ConstFiles.unkTipFull);
            }
            catch
            {
                Config.UnkTip = null;
            }
        }
        /// <summary>
        /// 加载 not.tip
        /// </summary>
        public void Load_Not()
        {
            try
            {
                Config.UnkTip = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(Config.ConstFiles.notTipFull);
            }
            catch
            {
                Config.UnkTip = null;
            }
        }
        /// <summary>
        /// 加载 unp.tip
        /// </summary>
        public void Load_Unp()
        {
            try
            {
                Config.UnkTip = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(Config.ConstFiles.unpTipFull);
            }
            catch
            {
                Config.UnkTip = null;
            }
        }
        /// <summary>
        /// 加载 err.tip
        /// </summary>
        public void Load_Err()
        {
            try
            {
                Config.UnkTip = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(Config.ConstFiles.errTipFull);
            }
            catch
            {
                Config.UnkTip = null;
            }
        }
    }
}
