using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 根目录信息表
    /// </summary>
    public class RootInfo
    {
        /// <summary>
        /// 根目录
        /// </summary>
        public List<string> RootPath
        {
            get;
            set;
        }
        /// <summary>
        /// 导出目录
        /// </summary>
        public string ExportPath
        {
            get { return (exportPath == null || exportPath.Length == 0) ? Tools.FileOperate.getExePath() : exportPath; }
            set { exportPath = value; }
        }
        private string exportPath;

        /// <summary>
        /// 搜索根目录
        /// </summary>
        /// <param name="rootpath"></param>
        /// <returns></returns>
        public int Search(string rootpath)
        {
            return RootPath.IndexOf(rootpath);
        }

        /// <summary>
        /// 刷新根目录信息，把不存在的目录删除。
        /// </summary>
        public void Refresh()
        {
            for (int i = RootPath.Count - 1; i >= 0; i--)
            {
                if (!System.IO.Directory.Exists(RootPath[i])) { Delete(i); }
            }

            for (int i = 0; i < RootPath.Count; i++)
            {
                Reload(i);
            }
        }

        /// <summary>
        /// 重新加载所有根目录
        /// </summary>
        public void Reload()
        {
            for (int i = 0; i < RootPath.Count; i++) { Reload(i); }
        }
        /// <summary>
        /// 重新加载指定根目录
        /// </summary>
        /// <param name="rootpath">根目录</param>
        public void Reload(string rootpath)
        {
            Reload(RootPath.IndexOf(rootpath));
        }
        /// <summary>
        /// 重新加载指定根目录
        /// </summary>
        /// <param name="folder">根目录</param>
        public void Reload(int folder)
        {
            if (folder < 0) { return; }
            if (folder > RootPath.Count - 1) { return; }

            Manager.Index.IndexPairs[folder].Clear();
            if (!System.IO.Directory.Exists(RootPath[folder])) { return; }

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(RootPath[folder]);
            System.IO.DirectoryInfo[] folders = dir.GetDirectories();
            System.IO.FileInfo[] files = dir.GetFiles();
            MediaModel model = null;

            foreach (System.IO.DirectoryInfo f in folders)
            {
                System.IO.FileInfo[] subfiles = dir.GetFiles();
                List<int> sub = new List<int>();

                foreach (System.IO.FileInfo s in subfiles)
                {
                    if (!Manager.Support.IsSupport(s.Extension)) { continue; }
                    sub.Add(sub.Count);
                }

                Manager.Index.IndexPairs[folder].Add(sub);
            }

            foreach (System.IO.FileInfo f in files)
            {
                if (!Manager.Support.IsSupport(f.Extension)) { continue; }
                List<int> sub = new List<int>();
                sub.Add(0);
                Manager.Index.IndexPairs[folder].Add(sub);
            }
        }

        /// <summary>
        /// 导入新的根目录
        /// </summary>
        /// <param name="rootpath">根目录</param>
        public void Input(string rootpath)
        {
            int index = RootPath.IndexOf(rootpath);
            if (index != -1) { Tools.Dialog.Confirm(""); return; }

            RootPath.Add(rootpath);
            Manager.Index.IndexPairs.Add(new List<List<int>>());
            Reload(rootpath);
        }
        /// <summary>
        /// 删除指定的根目录。
        /// </summary>
        /// <param name="rootpath">根目录</param>
        public void Delete(string rootpath)
        {
            Delete(RootPath.IndexOf(rootpath));
        }
        /// <summary>
        /// 删除指定的根目录。
        /// </summary>
        /// <param name="folder">根目录</param>
        public void Delete(int folder)
        {
            if (folder < 0) { return; }
            if (folder > RootPath.Count - 1) { return; }

            RootPath.RemoveAt(folder);
            Manager.Index.IndexPairs.RemoveAt(folder);
        }

        /// <summary>
        /// 把所有根目录下的隐藏文件显示出来
        /// </summary>
        public void Show()
        {
            for (int i = 0; i < RootPath.Count; i++) { Show(i); }
        }
        /// <summary>
        /// 把指定根目录下的隐藏文件显示出来
        /// </summary>
        /// <param name="folder">根目录</param>
        public void Show(int folder)
        {
            if (folder < 0) { return; }
            if (folder > RootPath.Count - 1) { return; }

            for (int i = 0; i < Manager.Index.IndexPairs[folder].Count; i++)
            {
                for (int j = 0; j < Manager.Index.IndexPairs[folder][i].Count; j++)
                {
                    int index = Manager.Index.IndexPairs[folder][i][j];
                    Manager.Media.Models[index].Show();
                }
            }
        }
        /// <summary>
        /// 把指定根目录下的隐藏文件显示出来
        /// </summary>
        /// <param name="rootpath">根目录</param>
        public void Show(string rootpath)
        {
            Show(RootPath.IndexOf(rootpath));
        }
        /// <summary>
        /// 把所有根目录下的显示文件隐藏起来
        /// </summary>
        public void Hide()
        {
            for (int i = 0; i < RootPath.Count; i++) { Hide(i); }
        }
        /// <summary>
        /// 把指定根目录下的显示文件隐藏起来
        /// </summary>
        /// <param name="folder">根目录</param>
        public void Hide(int folder)
        {
            if (folder < 0) { return; }
            if (folder > RootPath.Count - 1) { return; }

            for (int i = 0; i < Manager.Index.IndexPairs[folder].Count; i++)
            {
                for (int j = 0; j < Manager.Index.IndexPairs[folder][i].Count; j++)
                {
                    int index = Manager.Index.IndexPairs[folder][i][j];
                    Manager.Media.Models[index].Hide();
                }
            }
        }
        /// <summary>
        /// 把指定根目录下的显示文件隐藏起来
        /// </summary>
        /// <param name="rootpath">根目录</param>
        public void Hide(string rootpath)
        {
            Hide(RootPath.IndexOf(rootpath));
        }

        /// <summary>
        /// 创建根目录信息
        /// </summary>
        public RootInfo()
        {
            RootPath = new List<string>();
            ExportPath = Tools.FileOperate.getExePath();
        }
    }
}
