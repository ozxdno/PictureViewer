using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 必要常量文件的文件信息
    /// </summary>
    class ConstFiles
    {
        /// <summary>
        /// 配置文件所在目录
        /// </summary>
        public string pviniPath
        {
            get { return Tools.Tools.getExePath(); }
        }
        /// <summary>
        /// 配置文件名称
        /// </summary>
        public string pviniName
        {
            get { return "pv.pvini"; }
        }
        /// <summary>
        /// 配置文件全称
        /// </summary>
        public string pviniFull
        {
            get { return pviniPath + "\\" + pviniName; }
        }

        /// <summary>
        /// 缓存数据文件的文件路径
        /// </summary>
        public string pvdataPath
        {
            get { return Tools.Tools.getExePath(); }
        }
        /// <summary>
        /// 缓存数据文件的文件名称
        /// </summary>
        public string pvdataName
        {
            get { return "pvdata"; }
        }
        /// <summary>
        /// 缓存数据文件的文件全称
        /// </summary>
        public string pvdataFull
        {
            get { return pvdataPath + "\\" + pvdataName; }
        }

        /// <summary>
        /// 错误提示图片的路径
        /// </summary>
        public string errTipPath
        {
            get { return Tools.Tools.getExePath(); }
        }
        /// <summary>
        /// 错误提示图片的名称
        /// </summary>
        public string errTipName
        {
            get { return "err.tip"; }
        }
        /// <summary>
        /// 错误提示图片的全称
        /// </summary>
        public string errTipFull
        {
            get { return errTipPath + "\\" + errTipName; }
        }

        /// <summary>
        /// 错误提示图片的路径
        /// </summary>
        public string unpTipPath
        {
            get { return Tools.Tools.getExePath(); }
        }
        /// <summary>
        /// 错误提示图片的名称
        /// </summary>
        public string unpTipName
        {
            get { return "unp.tip"; }
        }
        /// <summary>
        /// 错误提示图片的全称
        /// </summary>
        public string unpTipFull
        {
            get { return unpTipPath + "\\" + unpTipName; }
        }

        /// <summary>
        /// 错误提示图片的路径
        /// </summary>
        public string unkTipPath
        {
            get { return Tools.Tools.getExePath(); }
        }
        /// <summary>
        /// 错误提示图片的名称
        /// </summary>
        public string unkTipName
        {
            get { return "unk.tip"; }
        }
        /// <summary>
        /// 错误提示图片的全称
        /// </summary>
        public string unkTipFull
        {
            get { return unkTipPath + "\\" + unkTipName; }
        }

        /// <summary>
        /// 错误提示图片的路径
        /// </summary>
        public string notTipPath
        {
            get { return Tools.Tools.getExePath(); }
        }
        /// <summary>
        /// 错误提示图片的名称
        /// </summary>
        public string notTipName
        {
            get { return "not.tip"; }
        }
        /// <summary>
        /// 错误提示图片的全称
        /// </summary>
        public string notTipFull
        {
            get { return notTipPath + "\\" + notTipName; }
        }

        /// <summary>
        /// 正在加载提示图片的路径
        /// </summary>
        public string iniTipPath
        {
            get { return Tools.Tools.getExePath(); }
        }
        /// <summary>
        /// 正在加载提示图片的名称
        /// </summary>
        public string iniTipName
        {
            get { return "ini.tip"; }
        }
        /// <summary>
        /// 正在加载提示图片的全称
        /// </summary>
        public string iniTipFull
        {
            get { return iniTipPath + "\\" + iniTipName; }
        }
    }
}
