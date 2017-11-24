using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    class Load_files
    {
        /// <summary>
        /// 重新加载所有根目录下的文件
        /// </summary>
        public static void load()
        {
            foreach (string rootpath in Config.RootPathes)
            {
                load(rootpath);
            }
        }
        /// <summary>
        /// 加载指定根目录下的所有文件。
        /// </summary>
        /// <param name="rootpath">根目录</param>
        public static void load(string rootpath)
        {
            int index = -1;
            for (int i = 0; i < Config.RootPathes.Count; i++)
            {
                if (Config.RootPathes[i] == rootpath) { index = i; break; }
            }
            if (index == -1)
            {
                if (!System.IO.Directory.Exists(rootpath)) { return; }

                index = Config.RootPathes.Count;
                Config.RootPathes.Add(rootpath);
                Config.Trees.Add(new List<List<int>>());
            }

            Config.Trees[index].Clear();
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(rootpath);
            System.IO.DirectoryInfo[] folders = dir.GetDirectories();
            System.IO.FileInfo[] files = dir.GetFiles();

            foreach (System.IO.DirectoryInfo folder in folders)
            {
                List<int> subfiles = new List<int>();
                files = folder.GetFiles();
                foreach (System.IO.FileInfo file in files)
                {
                    if (Support.IsZipExtension(file.Extension)) { continue; }
                    if (!Support.IsSupport(file.Extension)) { continue; }
                    
                    int baseIndex = BaseFileInfo.getBaseIndex(file.FullName);
                    if (baseIndex == -1)
                    {
                        baseIndex = Config.Files.Count;
                        Config.Files.Add(new BaseFileInfo(file));
                    }

                    Config.Files[baseIndex].addTreeIndex(new Index(index, Config.Trees[index].Count, subfiles.Count));
                    subfiles.Add(baseIndex);
                }

                Config.Trees[index].Add(subfiles);
            }

            foreach (System.IO.FileInfo file in files)
            {
                if (!Support.IsSupport(file.Extension)) { continue; }

                List<int> subfiles = new List<int>();
                int baseIndex = BaseFileInfo.getBaseIndex(file.FullName);
                if (baseIndex == -1)
                {
                    baseIndex = Config.Files.Count;
                    Config.Files.Add(new BaseFileInfo(file));
                }

                Config.Files[baseIndex].addTreeIndex(new Index(index, Config.Trees[index].Count, 0));
                subfiles.Add(baseIndex);
                Config.Trees[index].Add(subfiles);
            }
        }
        /// <summary>
        /// 加载指定根目录下的所有文件。
        /// </summary>
        /// <param name="folder">根目录</param>
        public static void load(int folder)
        {
            if (folder < 0) { return; }
            if (folder >= Config.RootPathes.Count) { return; }

            load(Config.RootPathes[folder]);
        }
        
    }
}
