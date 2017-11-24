using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 当个文件所具有的属性
    /// </summary>
    class BaseFileInfo
    {
        /// <summary>
        /// 文件的索引号
        /// </summary>
        public List<Index> Indexes
        {
            get;
            set;
        }
        /// <summary>
        /// 文件的索引号（getIndex 以后才能使用）
        /// </summary>
        public int Index
        {
            get;
            set;
        }
        /// <summary>
        /// 使用次数计数
        /// </summary>
        public int Count
        {
            get { return Count < 0 ? 0 : Count; }
            set { Count = value; }
        }
        /// <summary>
        /// 请求释放该文件
        /// </summary>
        public bool Dispose
        {
            get;
            set;
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path
        {
            set { }
            get { return Path == null ? "" : Path; }
        }
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name
        {
            set { Name = value; }
            get { return Name == null ? "" : Name; }
        }
        /// <summary>
        /// 文件全路径
        /// </summary>
        public string Full
        {
            get { return (Path == null || Name == null) ? "" : Path + "\\" + Name; }
        }
        /// <summary>
        /// 不带后缀的文件名
        /// </summary>
        public string NameWithoutExtension
        {
            get { return Operate.getNameWithoutExtension(Name); }
        }
        /// <summary>
        /// 文件后缀
        /// </summary>
        public string Extension
        {
            get { return Operate.getExtension(Name); }
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        public bool Exist
        {
            get { return System.IO.File.Exists(Full); }
        }
        /// <summary>
        /// 最后一次的修改时间
        /// </summary>
        public long Time
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件的缓存信息是否已经被加载
        /// </summary>
        public bool Loaded
        {
            get;
            set;
        }
        /// <summary>
        /// 文件的大小，单位：B（字节）
        /// </summary>
        public long Length
        {
            get;
            set;
        }

        /// <summary>
        /// 本文件是否具有隐藏属性
        /// </summary>
        public bool IsHide
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是子目录中的文件
        /// </summary>
        public bool IsSub
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是图片文件
        /// </summary>
        public bool IsPicture
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是 GIF 文件
        /// </summary>
        public bool IsGif
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是音频文件
        /// </summary>
        public bool IsMusic
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是视频文件
        /// </summary>
        public bool IsVideo
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是 ZIP 文件中的文件
        /// </summary>
        public bool IsZip
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是不是不支持类型
        /// </summary>
        public bool IsUnsupport
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是不是错误的文件
        /// </summary>
        public bool IsError
        {
            set;
            get;
        }

        /// <summary>
        /// 文件的高度，单位：像素
        /// </summary>
        public int Height
        {
            get;
            set;
        }
        /// <summary>
        /// 文件的宽度，单位：像素
        /// </summary>
        public int Width
        {
            get;
            set;
        }
        /// <summary>
        /// 图片的中间行灰度值（匹配用）
        /// </summary>
        public int[] GraysR
        {
            set;
            get;
        }
        /// <summary>
        /// 图片的中间列灰度值（匹配用）
        /// </summary>
        public int[] GraysC
        {
            set;
            get;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public BaseFileInfo()
        {
            Indexes = new List<Files.Index>();

            Path = "";
            Name = "";

            Time = 0;
            Loaded = false;
            IsHide = false;
            IsSub = false;
            IsPicture = false;
            IsGif = false;
            IsMusic = false;
            IsVideo = false;
            IsZip = false;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="full"></param>
        public BaseFileInfo(string full)
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public BaseFileInfo(string path, string name)
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="file"></param>
        public BaseFileInfo(System.IO.FileInfo file)
        {
            Path = file.DirectoryName;
            Name = file.Name;

            Time = file.LastWriteTime.ToFileTime();
            Loaded = false;
            Length = file.Length;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="file"></param>
        /// <param name="sub"></param>
        public BaseFileInfo(int folder, int file, int sub = -1)
        {

        }

        /// <summary>
        /// 导出该文件
        /// </summary>
        /// <param name="failedReason"></param>
        /// <param name="showConfirm"></param>
        /// <param name="showError"></param>
        /// <returns></returns>
        public bool Export(ref string failedReason, bool showConfirm = false, bool showError = false)
        {
            Operate.FILE_MOVE_RESULT res = Operate.FileMove(Full, Config.ExportPath + "\\" + Name);

            return false;
        }
        /// <summary>
        /// 导出该文件
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="failedReason"></param>
        /// <param name="showConfirm"></param>
        /// <param name="showError"></param>
        /// <returns></returns>
        public bool Export(string dest, ref string failedReason, bool showConfirm = false, bool showError = false)
        {
            if (dest == null) { dest = ""; }
            Operate.FILE_MOVE_RESULT res = Operate.FileMove(Full, dest + Name);

            return false;
        }
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="failedReason"></param>
        /// <returns></returns>
        public bool Rename(string dest, ref string failedReason, bool showConfirm = false, bool showError = false)
        {
            if (dest == null) { dest = ""; }
            string destExtension = Operate.getExtension(dest);
            if (destExtension != Extension) { dest += Extension; }

            Operate.FILE_MOVE_RESULT res = Operate.FileMove(Full, Path + dest);

            return false;
        }
        /// <summary>
        /// 永久删除
        /// </summary>
        /// <param name="failedReason"></param>
        /// <returns></returns>
        public bool Delete(ref string failedReason, bool showConfirm = false, bool showError = false)
        {
            return false;
        }

        /// <summary>
        /// 获取该文件在文件总集中的索引号
        /// </summary>
        public void getIndex()
        {
            Index = Operate.getIndex(Path, Name);
        }

        /// <summary>
        /// 转换为文本记录
        /// </summary>
        /// <returns></returns>
        public string ToString()
        {
            return "";
        }
        /// <summary>
        /// 文本记录转换为 BaseFileInfo，失败返回 FALSE
        /// </summary>
        /// <param name="str">文本记录</param>
        public bool ToBaseFileInfo(string str)
        {
            if (str == null || str.Length == 0) { return false; }

            string[] items = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length != 2 * Find.Config.Pixes + 8) { return false; }


            return true;
        }

        /// <summary>
        /// 判断两个文件是否相同
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(BaseFileInfo left, BaseFileInfo right)
        {
            return left.Full == right.Full;
        }
        /// <summary>
        /// 判断两个文件是否不同
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(BaseFileInfo left, BaseFileInfo right)
        {
            return left.Full != right.Full;
        }
    }
}
