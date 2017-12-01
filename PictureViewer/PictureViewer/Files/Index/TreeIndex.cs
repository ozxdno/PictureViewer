using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 文件的树形索引
    /// </summary>
    public class TreeIndex
    {
        /// <summary>
        /// 根目录索引号
        /// </summary>
        public int Folder
        {
            set;
            get;
        }
        /// <summary>
        /// 文件索引号（在根目录中）
        /// </summary>
        public int File
        {
            get;
            set;
        }
        /// <summary>
        /// 子目录索引号
        /// </summary>
        public int Sub
        {
            get;
            set;
        }
        /// <summary>
        /// 该索引号是否存在
        /// </summary>
        public bool Exist
        {
            get
            {
                if (Folder < 0) { return false; }
                if (Folder > Manager.Index.IndexPairs.Count - 1) { return false; }

                if (File < 0) { return false; }
                if (File > Manager.Index.IndexPairs[Folder].Count - 1) { return false; }

                if (Sub < 0) { return false; }
                if (Sub > Manager.Index.IndexPairs[Folder][File].Count - 1) { return false; }

                return true;
            }
        }
        /// <summary>
        /// 是否为空子目录
        /// </summary>
        public bool Empty
        {
            get
            {
                return TotalSubs == 0;
            }
        }
        /// <summary>
        /// 索引号的根目录是否存在
        /// </summary>
        public bool ExistFolder
        {
            get
            {
                if (Folder < 0) { return false; }
                if (Folder > Manager.Index.IndexPairs.Count - 1) { return false; }
                return true;
            }
        }
        /// <summary>
        /// 索引号的文件是否存在
        /// </summary>
        public bool ExistFile
        {
            get
            {
                if (Folder < 0) { return false; }
                if (Folder > Manager.Index.IndexPairs.Count - 1) { return false; }

                if (File < 0) { return false; }
                if (File > Manager.Index.IndexPairs[Folder].Count - 1) { return false; }

                return true;
            }
        }
        /// <summary>
        /// 索引号的子文件是否存在
        /// </summary>
        public bool ExistSub
        {
            get { return Exist; }
        }

        /// <summary>
        /// 基本索引号
        /// </summary>
        public BaseIndex Base
        {
            get
            {
                if (!Exist) { return new BaseIndex(-1); }
                return new BaseIndex(Manager.Index.IndexPairs[Folder][File][Sub]);
            }
        }
        /// <summary>
        /// 文件信息
        /// </summary>
        public MediaModel Model
        {
            get { return Base.Model; }
        }

        /// <summary>
        /// 总的根目录数
        /// </summary>
        public int TotalFolders
        {
            get { return Manager.Index.IndexPairs.Count; }
        }
        /// <summary>
        /// 总的文件数
        /// </summary>
        public int TotalFiles
        {
            get
            {
                if (Folder < 0 || Folder > Manager.Index.IndexPairs.Count - 1) { return 0; }
                return Manager.Index.IndexPairs[Folder].Count;
            }
        }
        /// <summary>
        /// 子目录总的文件数
        /// </summary>
        public int TotalSubs
        {
            get
            {
                if (Folder < 0 || Folder > Manager.Index.IndexPairs.Count - 1) { return 0; }
                if (File < 0 || File > Manager.Index.IndexPairs[Folder].Count - 1) { return 0; }
                return Manager.Index.IndexPairs[Folder][File].Count;
            }
        }

        /// <summary>
        /// 上一个
        /// </summary>
        public TreeIndex Previous
        {
            get
            {
                int folder = Folder;
                int file = File;
                int sub = Sub;

                sub--;
                if (sub < 0) { file--; }
                if (file < 0) { folder--; }

                if (folder < 0) { folder = Manager.Index.IndexPairs.Count - 1; }
                if (folder < 0) { return new TreeIndex(); }

                if (file < 0) { file = Manager.Index.IndexPairs[folder].Count - 1; }
                if (file < 0) { return new TreeIndex(folder); }

                if (sub < 0) { sub = Manager.Index.IndexPairs[folder][file].Count - 1; }
                if (sub < 0) { return new TreeIndex(folder, file); }

                return new TreeIndex(folder, file, sub);
            }
        }
        /// <summary>
        /// 下一个
        /// </summary>
        public TreeIndex Next
        {
            get
            {
                int folder = Folder;
                int file = File;
                int sub = Sub;

                if (folder >= Manager.Index.IndexPairs.Count) { folder = Manager.Index.IndexPairs.Count - 1; }
                if (folder < 0) { return new TreeIndex(); }

                if (file >= Manager.Index.IndexPairs[folder].Count) { file = Manager.Index.IndexPairs[folder].Count - 1; }
                if (file < 0) { return new TreeIndex(folder); }

                if (sub >= Manager.Index.IndexPairs[folder][file].Count) { sub = Manager.Index.IndexPairs[folder][file].Count - 1; }
                if (sub < 0) { return new TreeIndex(folder, file); }

                sub++;
                if (sub >= Manager.Index.IndexPairs[folder][file].Count) { sub = 0; file++; }
                if (file >= Manager.Index.IndexPairs[folder].Count) { file = 0; folder++; }
                if (folder >= Manager.Index.IndexPairs.Count) { folder = 0; }

                return new TreeIndex(folder, file, sub);
            }
        }

        /// <summary>
        /// 构建树形索引号
        /// </summary>
        /// <param name="folder">根目录索引号</param>
        /// <param name="file">文件索引号（在根目录中）</param>
        /// <param name="sub">子目录索引号</param>
        public TreeIndex(int folder = -1, int file = -1, int sub = -1)
        {
            Folder = folder;
            File = file;
            Sub = sub;
        }

        /// <summary>
        /// 调整索引号
        /// </summary>
        public void Fit()
        {
            if (Folder < 0) { Folder = 0; }
            if (Folder > Manager.Index.IndexPairs.Count - 1) { Folder = Manager.Index.IndexPairs.Count - 1; }
            if (Folder < 0) { return; }

            if (File < 0) { File = 0; }
            if (File > Manager.Index.IndexPairs[Folder].Count - 1) { File = Manager.Index.IndexPairs[Folder].Count - 1; }
            if (File < 0) { return; }

            if (Sub < 0) { Sub = 0; }
            if (Sub > Manager.Index.IndexPairs[Folder][File].Count - 1) { Sub = Manager.Index.IndexPairs[Folder][File].Count - 1; }
            if (Sub < 0) { return; }
        }

        /// <summary>
        /// 复制
        /// </summary>
        public TreeIndex Copy()
        {
            return new TreeIndex(Folder, File, Sub);
        }

        /// <summary>
        /// 转换为字符串，格式：[1/20] [1/1]
        /// </summary>
        /// <returns></returns>
        public string ToString()
        {
            return "[" + (File + 1).ToString() + " / " + TotalFiles.ToString() + "] [" +
                (Sub + 1).ToString() + " / " + TotalSubs.ToString() + "]";
        }

        public static bool operator ==(TreeIndex left, TreeIndex right)
        {
            return left.Folder == right.Folder && 
                left.File == right.File &&
                left.Sub == right.Sub;
        }
        public static bool operator !=(TreeIndex left, TreeIndex right)
        {
            return left.Folder != right.Folder ||
                left.File != right.File ||
                left.Sub != right.Sub;
        }
    }
}
