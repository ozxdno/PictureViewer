using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 文件信息表，所有描述文件信息的基类，包含文件的基本属性和操作。
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// 文件所在的路径
        /// </summary>
        public string Path
        {
            get { return (path == null) ? "" : path; }
            set { path = value; }
        }
        private string path;
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name
        {
            set { name = value; }
            get { return (name == null) ? "" : name; }
        }
        private string name;
        /// <summary>
        /// 文件全部名称
        /// </summary>
        public string Full
        {
            get { return (Path.Length == 0 || Name.Length == 0) ? "" : Path + "\\" + Name; }
        }
        /// <summary>
        /// 后缀
        /// </summary>
        public string Extension
        {
            get { return Tools.FileOperate.getExtension(Name); }
        }
        /// <summary>
        /// 子目录
        /// </summary>
        public string Subfolder
        {
            get
            {
                return Tools.FileOperate.getName(Path);
            }
        }
        /// <summary>
        /// 不带后缀的文件名
        /// </summary>
        public string NameWithoutExtension
        {
            get { return Tools.FileOperate.getNameWithoutExtension(Name); }
        }
        /// <summary>
        /// 文件类型
        /// </summary>
        public FileType Type
        {
            get;
            set;
        }
        /// <summary>
        /// 文件是否仍然存在
        /// </summary>
        public bool Exist
        {
            get { return System.IO.File.Exists(Full); }
        }
        /// <summary>
        /// 最后一次修改的时间
        /// </summary>
        public long Time
        {
            get;
            set;
        }
        /// <summary>
        /// 文件大小，单位：字节（B）
        /// </summary>
        public long Length
        {
            get;
            set;
        }
        /// <summary>
        /// 当前正在使用该文件的地方的总计
        /// </summary>
        public int Count
        {
            set;
            get;
        }
        /// <summary>
        /// 正在请求释放该文件
        /// </summary>
        public bool Asking
        {
            get { return asking; }
            set { asking = value; if (asking) { Manager.Asking(Full); } }
        }
        private bool asking;
        /// <summary>
        /// 请求已被处理
        /// </summary>
        public bool Answered
        {
            get { return Manager.Answered(); }
        }

        /// <summary>
        /// 填充一个空文件
        /// </summary>
        public void FillBaseModel()
        {
            Path = "";
            Name = "";
            Type = FileType.Unsupport;
            Time = 0;
            Length = 0;
            Count = 0;
            Asking = false;
        }
        /// <summary>
        /// 填充文件信息表
        /// </summary>
        /// <param name="full">指定文件</param>
        public void FillBaseModel(string full)
        {
            Path = Tools.FileOperate.getPath(full);
            Name = Tools.FileOperate.getName(full);

            Type = Manager.Support.GetType(Extension);
            Time = 0;
            Length = 0;
            Count = 0;
            Asking = false;
        }
        /// <summary>
        /// 填充文件信息表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public void FillBaseModel(string path, string name)
        {

        }
        /// <summary>
        /// 填充文件信息表
        /// </summary>
        /// <param name="file"></param>
        public void FillBaseModel(System.IO.FileInfo file)
        {

        }

        /// <summary>
        /// 显示该文件
        /// </summary>
        /// <param name="showConfirm"></param>
        /// <param name="showError"></param>
        /// <returns></returns>
        public FileMoveResult Show(bool showConfirm = false, bool showError = false)
        {
            return FileMoveResult.SUCCESSED;
        }
        /// <summary>
        /// 隐藏该文件
        /// </summary>
        /// <param name="showConfirm"></param>
        /// <param name="showError"></param>
        /// <returns></returns>
        public FileMoveResult Hide(bool showConfirm = false, bool showError = false)
        {
            return FileMoveResult.SUCCESSED;
        }
        /// <summary>
        /// 导出文件
        /// </summary>
        /// <param name="showConfirm"></param>
        /// <param name="showError"></param>
        /// <returns></returns>
        public FileMoveResult Export(bool showConfirm = false, bool showError = false)
        {
            return FileMoveResult.SUCCESSED;
        }
        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="destName"></param>
        /// <param name="showConfirm"></param>
        /// <param name="showError"></param>
        /// <returns></returns>
        public FileMoveResult Rename(string destName, bool showConfirm = false, bool showError = false)
        {
            return FileMoveResult.SUCCESSED;
        }
        /// <summary>
        /// 移动到
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="showConfirm"></param>
        /// <param name="showError"></param>
        /// <returns></returns>
        public FileMoveResult MoveTo(string dest, bool showConfirm = false, bool showError = false)
        {
            Asking = true;
            do { System.Threading.Thread.Sleep(100); } while (Answered);

            //...

            Asking = false;
            return FileMoveResult.SUCCESSED;
        }
        /// <summary>
        /// 删除（不支持）
        /// </summary>
        /// <param name="showConfirm"></param>
        /// <param name="showError"></param>
        /// <returns></returns>
        public FileMoveResult Delete(bool showConfirm = false, bool showError = false)
        {
            return FileMoveResult.CANCLED;
        }


        public static bool operator==(BaseModel left, object right)
        {
            string l = left.Full;
            string r = null;

            if (right is string) { r = (string)right; }
            else if (right is BaseModel) { r = ((BaseModel)right).Full; }
            else { return false; }

            if (l == null || r == null) { return false; }
            return l == r;
        }
        public static bool operator!=(BaseModel left, object right)
        {
            string l = left.Full;
            string r = null;

            if (right is string) { r = (string)right; }
            else if (right is BaseModel) { r = ((BaseModel)right).Full; }
            else { return true; }

            if (l == null || r == null) { return false; }
            return l == r;
        }
    }
}
