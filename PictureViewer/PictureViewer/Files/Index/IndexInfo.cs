using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 文件索引号的对应关系
    /// </summary>
    public class IndexInfo
    {
        /// <summary>
        /// 配对信息
        /// </summary>
        public List<List<List<int>>> IndexPairs
        {
            get;
            set;
        }

        /// <summary>
        /// 创建 TreeIndex 与 BaseIndex 的对应关系。
        /// </summary>
        public IndexInfo()
        {
            IndexPairs = new List<List<List<int>>>();
        }
        
        /// <summary>
        /// 利用树形索引号获取基本索引号
        /// </summary>
        /// <param name="folder">根目录索引号</param>
        /// <param name="file">文件索引号</param>
        /// <param name="sub">子目录索引号</param>
        /// <returns></returns>
        public int Search(int folder, int file, int sub)
        {
            return Search(new TreeIndex(folder, file, sub));
        }
        /// <summary>
        /// 利用树形索引号获取基本索引号
        /// </summary>
        /// <param name="treeIndex">树形索引号</param>
        /// <returns></returns>
        public int Search(TreeIndex treeIndex)
        {
            if (!treeIndex.Exist) { return -1; }
            return IndexPairs[treeIndex.Folder][treeIndex.File][treeIndex.Sub];
        }
        /// <summary>
        /// 利用基本索引号获取图形索引号
        /// </summary>
        /// <param name="baseIndex">基本索引号</param>
        /// <returns></returns>
        public List<TreeIndex> Search(BaseIndex baseIndex)
        {
            return Search(baseIndex.Base);
        }
        /// <summary>
        /// 利用基本索引号获取图形索引号
        /// </summary>
        /// <param name="baseIndex">基本索引号</param>
        /// <returns></returns>
        public List<TreeIndex> Search(int baseIndex)
        {
            List<TreeIndex> trees = new List<TreeIndex>();

            for (int folder = 0; folder < IndexPairs.Count; folder++)
            {
                for (int file = 0; file < IndexPairs[folder].Count; file++)
                {
                    for (int sub = 0; sub < IndexPairs[folder][file].Count; sub++)
                    {
                        int index = IndexPairs[folder][file][sub];
                        if (baseIndex == index) { trees.Add(new TreeIndex(folder, file, sub)); }
                    }
                }
            }

            return trees;
        }

        /// <summary>
        /// 刷新全部配对信息
        /// </summary>
        public void Refresh()
        {
            Manager.Root.Reload();
        }
        /// <summary>
        /// 刷新根目录的配对信息
        /// </summary>
        /// <param name="folder">根目录</param>
        public void Refresh(int folder)
        {
            Manager.Root.Reload(folder);
        }
        /// <summary>
        /// 刷新根目录的配对信息
        /// </summary>
        /// <param name="rootpath">根目录</param>
        public void Refresh(string rootpath)
        {
            Manager.Root.Reload(rootpath);
        }
    }
}
