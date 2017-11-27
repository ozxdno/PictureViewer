using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    class Save_pvdata
    {
        /// <summary>
        /// 利用读写专用线程保存数据
        /// </summary>
        public static void thread()
        {
            while (Config.IObusy) ;
            Config.tIO = new System.Threading.Thread(save);
            Config.tIO.Start();
        }

        /// <summary>
        /// 保存当前的文件信息
        /// </summary>
        public static void save()
        {
            try
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(Config.ConstFiles.pvdataFull, false);
                for (int i = 0; i < Config.Files.Count; i++)
                {
                    sw.WriteLine(Config.Files[i].ToString());
                }
            }
            catch
            {

            }
        }
    }
}
