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
        /// 初始化（在新线程中读取缓存数据）
        /// </summary>
        public Load_pvdata()
        {
            // 等待读取完毕
            while (Config.Loading != null && Config.Loading.ThreadState == System.Threading.ThreadState.Running) ;

            // 开始新的线程
            Config.Loading = new System.Threading.Thread(LoadFile);
            Config.Loading.Start();
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        public void LoadFile()
        {
            Config.Files.Clear();

            try
            {
                StreamReader sr = new StreamReader(Config.ConstFiles.pvdataFull);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    BaseFileInfo file = new BaseFileInfo();
                    bool ok = Load_vars.ToFileInfo(line, ref file);
                    if (!ok) { continue; }
                    Config.Files.Add(file);
                }
            }
            catch
            {

            }
        }
    }
}
