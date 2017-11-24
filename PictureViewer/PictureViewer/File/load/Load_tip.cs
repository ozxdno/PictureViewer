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
        public static void Load_all()
        {
            Load_Unk();
            Load_Unp();
            Load_Err();
            Load_Not();
            Load_Ini();
        }

        /// <summary>
        /// 加载 unk.tip
        /// </summary>
        public static void Load_Unk()
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
        public static void Load_Not()
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
        public static void Load_Unp()
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
        public static void Load_Err()
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
        /// <summary>
        /// 加载 ini.tip
        /// </summary>
        public static void Load_Ini()
        {
            try
            {
                Config.IniTip = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(Config.ConstFiles.iniTipFull);
            }
            catch
            {
                Config.IniTip = null;
            }
        }
    }
}
