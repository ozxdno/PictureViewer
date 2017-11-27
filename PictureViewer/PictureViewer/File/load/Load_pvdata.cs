using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PictureViewer.Files
{
    class Load_pvdata
    {
        /// <summary>
        /// 在新线程中读取缓存数据
        /// </summary>
        public static void thread()
        {
            // 等待读取完毕
            while (Config.IObusy) ;

            // 开始新的线程
            Config.Loading = new System.Threading.Thread(load);
            Config.Loading.Start();
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        public static void load()
        {
            #region 加载 pvdata 中已有的文件
            try
            {
                StreamReader sr = new StreamReader(Config.ConstFiles.pvdataFull);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    Load_vars.ToBaseFileInfo(line);
                }
            }
            catch
            {

            }

            #endregion

            #region 加载 pvdata 中没有的文件

            for (int i = 0; i < Config.RootPathes.Count; i++) { Config.Trees.Add(new List<List<int>>()); }
            Load_files.load();

            #endregion
        }
    }
}
