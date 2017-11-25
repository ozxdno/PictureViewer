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
        /// 文件的索引号
        /// </summary>
        public int Base
        {
            get
            {
                if (Indexes.Count == 0) { return -1; }
                return Indexes[0].Base;
            }
        }
        /// <summary>
        /// 使用次数计数
        /// </summary>
        public int Count
        {
            get { return count < 0 ? 0 : count; }
            set { count = value; }
        }
        private int count;
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
            set { path = value; }
            get { return path == null ? "" : path; }
        }
        private string path;
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name
        {
            set { name = value; }
            get { return name == null ? "" : name; }
        }
        private string name;
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
            get { return Tools.Tools.getNameWithoutExtension(Name); }
        }
        /// <summary>
        /// 文件后缀
        /// </summary>
        public string Extension
        {
            get { return Tools.Tools.getExtension(Name); }
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        public bool Exist
        {
            get
            {
                if (IsZip) { return System.IO.File.Exists(Path); }
                return System.IO.File.Exists(Full);
            }
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
        /// 类型
        /// </summary>
        public Support.TYPE Type
        {
            get;
            set;
        }
        /// <summary>
        /// 本文件是否是图片文件
        /// </summary>
        public bool IsPicture
        {
            get { return Type == Support.TYPE.PICTURE; }
        }
        /// <summary>
        /// 本文件是否是 GIF 文件
        /// </summary>
        public bool IsGif
        {
            get { return Type == Support.TYPE.GIF; }
        }
        /// <summary>
        /// 本文件是否是音频文件
        /// </summary>
        public bool IsMusic
        {
            get { return Type == Support.TYPE.MUSIC; }
        }
        /// <summary>
        /// 本文件是否是视频文件
        /// </summary>
        public bool IsVideo
        {
            get { return Type == Support.TYPE.VIDEO; }
        }
        /// <summary>
        /// 本文件是否是 ZIP 文件中的文件
        /// </summary>
        public bool IsZip
        {
            get
            {
                return System.IO.File.Exists(Path) &&
                    Support.GetType(Tools.Tools.getExtension(Path)) == Support.TYPE.ZIP;
            }
        }
        /// <summary>
        /// 本文件是不是不支持类型
        /// </summary>
        public bool IsUnsupport
        {
            get { return Type == Support.TYPE.UNSUPPORT; }
        }

        /// <summary>
        /// 本文件是不是子目录中的文件
        /// </summary>
        public bool IsSub
        {
            get
            {
                return Config.RootPathes.IndexOf(Path) == -1;
            }
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

            Count = 0;
            Dispose = false;

            Path = "";
            Name = "";

            Time = 0;
            Loaded = false;
            Height = 0;
            Width = 0;
            GraysR = null;
            GraysC = null;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="full"></param>
        public BaseFileInfo(string full)
        {
            Indexes = new List<Index>();
            Count = 0;
            Dispose = false;
            Path = Tools.Tools.getPath(full);
            Name = Tools.Tools.getName(full);

            System.IO.FileInfo file = new System.IO.FileInfo(Full);
            Time = file.LastWriteTime.ToFileTime();
            Loaded = false;
            Length = file.Length;
            Type = Support.GetType(Extension);

            Height = 0;
            Width = 0;
            GraysR = null;
            GraysC = null;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public BaseFileInfo(string path, string name)
        {
            Indexes = new List<Index>();
            Count = 0;
            Dispose = false;
            Path = path;
            Name = name;

            System.IO.FileInfo file = new System.IO.FileInfo(Full);
            Time = file.LastWriteTime.ToFileTime();
            Loaded = false;
            Length = file.Length;
            Type = Support.GetType(Extension);

            Height = 0;
            Width = 0;
            GraysR = null;
            GraysC = null;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="file"></param>
        public BaseFileInfo(System.IO.FileInfo file)
        {
            Indexes = new List<Index>();
            Count = 0;
            Dispose = false;
            Path = file.DirectoryName;
            Name = file.Name;

            Time = file.LastWriteTime.ToFileTime();
            Loaded = false;
            Length = file.Length;
            Type = Support.GetType(Extension);

            Height = 0;
            Width = 0;
            GraysR = null;
            GraysC = null;
        }

        /// <summary>
        /// 文件操作的结果
        /// </summary>
        public enum FILE_MOVE_RESULT
        {
            /// <summary>
            /// 成功操作
            /// </summary>
            SUCCESSED,
            /// <summary>
            /// 取消操作
            /// </summary>
            CANCLED,

            /// <summary>
            /// 源文件已存在
            /// </summary>
            SOUR_FILE_EXISTED,
            /// <summary>
            /// 源文件不存在
            /// </summary>
            SOUR_FILE_NOT,
            /// <summary>
            /// 源文件正在使用
            /// </summary>
            SOUR_FILE_USING,
            /// <summary>
            /// 源文件非法命名
            /// </summary>
            SOUR_FILE_ILLEGAL,
            /// <summary>
            /// 源路径已存在
            /// </summary>
            SOUR_PATH_EXISTED,
            /// <summary>
            /// 源路径不存在
            /// </summary>
            SOUR_PATH_NOT,
            /// <summary>
            /// 源路径非法
            /// </summary>
            SOUR_PATH_ILLEGAL,
            /// <summary>
            /// 源文件的文件名称已存在
            /// </summary>
            SOUR_NAME_EXISTED,
            /// <summary>
            /// 源文件的文件名称不存在
            /// </summary>
            SOUR_NAME_NOT,
            /// <summary>
            /// 源文件的文件名称非法
            /// </summary>
            SOUR_NAME_ILLEGAL,

            /// <summary>
            /// 目标文件已存在
            /// </summary>
            DEST_FILE_EXISTED,
            /// <summary>
            /// 目标文件不存在
            /// </summary>
            DEST_FILE_NOT,
            /// <summary>
            /// 目标文件正在使用
            /// </summary>
            DEST_FILE_USING,
            /// <summary>
            /// 目标文件非法命名
            /// </summary>
            DEST_FILE_ILLEGAL,
            /// <summary>
            /// 目标路径已存在
            /// </summary>
            DEST_PATH_EXISTED,
            /// <summary>
            /// 目标路径不存在
            /// </summary>
            DEST_PATH_NOT,
            /// <summary>
            /// 目标路径非法
            /// </summary>
            DEST_PATH_ILLEGAL,
            /// <summary>
            /// 目标文件的文件名称已存在
            /// </summary>
            DEST_NAME_EXISTED,
            /// <summary>
            /// 目标文件的文件名称不存在
            /// </summary>
            DEST_NAME_NOT,
            /// <summary>
            /// 目标文件的文件名称非法
            /// </summary>
            DEST_NAME_ILLEGAL
        }
        /// <summary>
        /// 导出该文件
        /// </summary>
        /// <param name="showConfirm">显示确认界面</param>
        /// <param name="showError">显示错误信息</param>
        /// <returns></returns>
        public FILE_MOVE_RESULT Export(bool showConfirm = false, bool showError = false)
        {
            string dest = Config.ExportPath + "\\" + Name;
            return MoveTo(dest, showConfirm, showError);
        }
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="dest">新名称</param>
        /// <param name="showConfirm">显示确认界面</param>
        /// <param name="showError">显示错误信息</param>
        /// <returns></returns>
        public FILE_MOVE_RESULT Rename(string dest, bool showConfirm = false, bool showError = false)
        {
            if (dest == null) { dest = ""; }
            string destExtension = Tools.Tools.getExtension(dest);
            if (destExtension != Extension) { dest += Extension; }

            if (Tools.Tools.IsCorrectFile(dest))
            {
                if (showError)
                {
                    System.Windows.Forms.MessageBox.Show(
                        Strings.FileStrings.Dest_Name_Illegal + "\n" +
                        Strings.FileStrings.Dest_Name + dest,
                        Strings.BoxStrings.Title_Warning
                        );
                }
                return FILE_MOVE_RESULT.DEST_NAME_ILLEGAL;
            }

            return MoveTo(Path + "\\" + dest, showConfirm, showError);
        }
        /// <summary>
        /// 移动文件到目标位置
        /// </summary>
        /// <param name="dest">目标位置</param>
        /// <param name="showConfirm">显示确认界面</param>
        /// <param name="showError">显示错误信息</param>
        /// <returns></returns>
        public FILE_MOVE_RESULT MoveTo(string dest, bool showConfirm = false, bool showError = false)
        {
            if (dest == null) { dest = ""; }
            if (!Tools.Tools.IsCorrectFile(dest))
            {
                if (showError)
                {
                    System.Windows.Forms.MessageBox.Show(
                        Strings.FileStrings.Dest_File_Illegal + "\n" +
                        Strings.FileStrings.Dest_File + dest,
                        Strings.BoxStrings.Title_Warning
                        );
                }
                return FILE_MOVE_RESULT.DEST_FILE_ILLEGAL;
            }

            if (!System.IO.File.Exists(Full))
            {
                if (showError)
                {
                    System.Windows.Forms.MessageBox.Show(
                        Strings.FileStrings.Sour_File_Not + "\n" +
                        Strings.FileStrings.Sour_File + Full,
                        Strings.BoxStrings.Title_Warning
                        );
                }
                return FILE_MOVE_RESULT.SOUR_FILE_NOT;
            }

            if (System.IO.File.Exists(dest))
            {
                if (showError)
                {
                    System.Windows.Forms.MessageBox.Show(
                        Strings.FileStrings.Dest_File_Existed + "\n" +
                        Strings.FileStrings.Dest_File + dest,
                        Strings.BoxStrings.Title_Warning
                        );
                }
                return FILE_MOVE_RESULT.DEST_FILE_EXISTED;
            }

            if (Count > 0)
            {
                if (showError)
                {
                    System.Windows.Forms.MessageBox.Show(
                        Strings.FileStrings.Sour_File_Using + "\n" +
                        Strings.FileStrings.Sour_File + Full,
                        Strings.BoxStrings.Title_Warning
                        );
                }
                return FILE_MOVE_RESULT.SOUR_FILE_USING;
            }

            if (showConfirm)
            {
                bool ok = System.Windows.Forms.DialogResult.OK ==
                    System.Windows.Forms.MessageBox.Show(
                        Strings.FileStrings.Ask_Confirm_Move + "\n" +
                        Strings.FileStrings.Sour_File + Full + "\n" +
                        Strings.FileStrings.Dest_File + dest,
                        Strings.BoxStrings.Title_Confirm,
                        System.Windows.Forms.MessageBoxButtons.OKCancel
                        );
                if (!ok) { return FILE_MOVE_RESULT.CANCLED; }
            }

            try
            {
                System.IO.File.Move(Full, dest);
            }
            catch
            {
                if (showError)
                {
                    System.Windows.Forms.MessageBox.Show(
                        Strings.FileStrings.Sour_File_Using + "\n" +
                        Strings.FileStrings.Sour_File + Full,
                        Strings.BoxStrings.Title_Warning
                        );
                }
                return FILE_MOVE_RESULT.SOUR_FILE_USING;
            }

            Reload();
            return FILE_MOVE_RESULT.SUCCESSED;
        }
        /// <summary>
        ///  删除，成功删除返回 TRUE
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            if (Base == -1) { return false; }

            Config.Files.RemoveAt(Base);
            bool[] loaded = new bool[Indexes.Count];
            for (int i = 0; i < Indexes.Count; i++) { loaded[i] = false; }

            for (int i = 0; i < Indexes.Count; i++)
            {
                if (loaded[i]) { continue; }
                Load_files.load(Indexes[i].Folder);
                for (int j = i + 1; j < Indexes.Count; j++)
                {
                    loaded[j] = Indexes[i].Folder == Indexes[j].Folder;
                }
            }

            return true;
        }

        /// <summary>
        /// 添加索引号
        /// </summary>
        /// <param name="index">树形索引</param>
        public void addTreeIndex(Index index)
        {
            for (int i = 0; i < Indexes.Count; i++) { if (index == Indexes[i]) { return; } }
            Indexes.Add(index);
        }
        /// <summary>
        /// 添加索引号
        /// </summary>
        /// <param name="folder">根目录索引号</param>
        /// <param name="file">文件索引号</param>
        /// <param name="sub">子目录索引号</param>
        public void addTreeIndex(int folder, int file, int sub)
        {
            addTreeIndex(new Index(folder, file, sub));
        }
        /// <summary>
        /// 重新加载文件信息
        /// </summary>
        public void Reload()
        {
            System.IO.FileInfo file = new System.IO.FileInfo(Full);

            Time = file.LastWriteTime.ToFileTime();
            Length = file.Length;
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
            return false;

            if (str == null || str.Length == 0) { return false; }

            string[] items = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length != 2 * FindForm.Config.Pixes + 8) { return false; }


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

        /// <summary>
        /// 利用文件名来获取该文件的索引号。
        /// </summary>
        /// <param name="full">全称</param>
        /// <returns></returns>
        public static int getBaseIndex(string full)
        {
            for (int i = 0; i < Config.Files.Count; i++)
            {
                if (Config.Files[i].Full == full) { return i; }
            }

            return -1;
        }
    }
}
