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
            while (Config.Loading != null && Config.Loading.ThreadState == System.Threading.ThreadState.Running) ;

            // 开始新的线程
            Config.Loading = new System.Threading.Thread(load);
            Config.Loading.Start();
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        public static void load()
        {
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
        }
    }
}
