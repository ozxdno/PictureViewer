using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 静态类，文件管理器的所有数据。
    /// </summary>
    public class Manager
    {
        /// <summary>
        /// 索引号信息
        /// </summary>
        public static IndexInfo Index
        {
            get;
            set;
        }
        /// <summary>
        /// 根目录信息
        /// </summary>
        public static RootInfo Root
        {
            get;
            set;
        }
        /// <summary>
        /// 多媒体文件信息
        /// </summary>
        public static MediaInfo Media
        {
            get;
            set;
        }
        /// <summary>
        /// 支持文件信息
        /// </summary>
        public static SupportInfo Support
        {
            get;
            set;
        }
        /// <summary>
        /// Pvini 文件信息
        /// </summary>
        public static PviniInfo Pvini
        {
            set;
            get;
        }
        /// <summary>
        /// Pvdata 文件信息
        /// </summary>
        public static PvdataInfo Pvdata
        {
            get;
            set;
        }
        /// <summary>
        /// err.tip 文件信息
        /// </summary>
        public static TipErrInfo TipErr
        {
            get;
            set;
        }
        /// <summary>
        /// unp.tip 文件信息
        /// </summary>
        public static TipUnpInfo TipUnp
        {
            get;
            set;
        }
        /// <summary>
        /// unk.tip 文件信息
        /// </summary>
        public static TipUnkInfo TipUnk
        {
            get;
            set;
        }
        /// <summary>
        /// not.tip 文件信息
        /// </summary>
        public static TipNotInfo TipNot
        {
            get;
            set;
        }
        /// <summary>
        /// int.tip 文件信息
        /// </summary>
        public static TipIntInfo TipInt
        {
            get;
            set;
        }
        /// <summary>
        /// mus.tip 文件信息
        /// </summary>
        public static TipMusInfo TipMus
        {
            get;
            set;
        }

        /// <summary>
        /// 打开文件管理器
        /// </summary>
        public static void Open()
        {
            Index = new IndexInfo();
            Root = new RootInfo();
            Media = new MediaInfo();
            Support = new SupportInfo();
            Pvini = new PviniInfo();
            Pvdata = new PvdataInfo();
            TipErr = new TipErrInfo();
            TipUnp = new TipUnpInfo();
            TipUnk = new TipUnkInfo();
            TipNot = new TipNotInfo();
            TipInt = new TipIntInfo();
            TipMus = new TipMusInfo();
            
            TipMus.Model.Load();
            TipErr.Model.Load();
            TipUnp.Model.Load();
            TipUnk.Model.Load();
            TipNot.Model.Load();
            TipInt.Model.Load();
            TipMus.Model.Load();
        }
        /// <summary>
        /// 关闭文件管理器
        /// </summary>
        public static void Close()
        {
            Pvini.Model.Dispose();
            Pvdata.Model.Dispose();
            TipMus.Model.Dispose();
            TipErr.Model.Dispose();
            TipUnp.Model.Dispose();
            TipUnk.Model.Dispose();
            TipNot.Model.Dispose();
            TipInt.Model.Dispose();
            TipMus.Model.Dispose();

            Pvini.Model.Save();
            Pvdata.Model.Save();
        }

        /// <summary>
        /// 请求释放指定文件
        /// </summary>
        /// <param name="full"></param>
        /// <returns></returns>
        public static void Asking(string full)
        {
            
        }
        /// <summary>
        /// 是否已经释放完成
        /// </summary>
        /// <returns></returns>
        public static bool Answered()
        {
            return true;
        }
    }
}
