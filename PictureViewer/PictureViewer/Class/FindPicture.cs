using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;

namespace PictureViewer.Class
{
    class FindPicture
    {
        ///////////////////////////////////////////////////// public attribute ///////////////////////////////////////////////

        public static List<string> Path = new List<string>();
        public static List<string> Name = new List<string>();
        
        ///////////////////////////////////////////////////// private attribute ///////////////////////////////////////////////

        private static Bitmap SourPic;
        private static Bitmap DestPic;

        private static int[] sr;
        private static int[] sg;
        private static int[] sb;
        private static int[] dr;
        private static int[] dg;
        private static int[] db;

        private static Thread TH1;
        private static Thread TH2;
        private static Thread TH3;
        private static Thread TH4;
        
        private static int BG1, ED1;
        private static int BG2, ED2;
        private static int BG3, ED3;
        private static int BG4, ED4;

        private static List<int> Result1 = new List<int>();
        private static List<int> Result2 = new List<int>();
        private static List<int> Result3 = new List<int>();
        private static List<int> Result4 = new List<int>();

        private static int FindOver = 0;

        ///////////////////////////////////////////////////// public method ///////////////////////////////////////////////

        /// <summary>
        /// 开始对当前图片进行查找。
        /// </summary>
        public static void Start()
        {
            // 清空结果缓存，结束线程。
            Path.Clear(); Name.Clear(); Stop();

            // 加载源文件
            int pidx = Form_Main.config.FolderIndex;
            int nidx = Form_Main.config.FileIndex;
            if (pidx < 0 || pidx >= FileOperate.RootFiles.Count) { return; }
            if (nidx < 0 || nidx >= FileOperate.RootFiles[pidx].Name.Count) { return; }
            string name = FileOperate.RootFiles[pidx].Name[nidx];
            if (FileOperate.getFileType(FileOperate.getExtension(name)) != 2) { return; }
            SourPic = (Bitmap)Form_Main.config.SourPicture.Clone();

            // 加载匹配项
            for (int i = 0; i < FileOperate.RootFiles.Count; i++)
            {
                for (int j = 0; j < FileOperate.RootFiles[i].Name.Count; j++)
                {
                    if (i == pidx && j == nidx) { continue; }
                    Path.Add(FileOperate.RootFiles[i].Path);
                    Name.Add(FileOperate.RootFiles[i].Name[j]);
                }
            }

            // 分配任务
            int averageN = Path.Count / 4 + 1;
            BG1 = 0; ED1 = BG1 + averageN;
            BG2 = ED1 + 1; ED2 = BG2 + averageN;
            BG3 = ED2 + 1; ED3 = BG3 + averageN;
            BG4 = ED3 + 1; ED4 = BG4 + averageN;
            if (ED4 >= Path.Count) { ED4 = Path.Count - 1; }

            // 开始寻找
            Result1.Clear(); Result2.Clear(); Result3.Clear(); Result4.Clear();
            TH1 = new Thread(cmp1);
            TH2 = new Thread(cmp2);
            TH3 = new Thread(cmp3);
            TH4 = new Thread(cmp4);

            while (FindOver < 4) ;
        }
        /// <summary>
        /// 停止对当前图片进行查找。
        /// </summary>
        public static void Stop()
        {
            while (TH1 != null && TH1.ThreadState == ThreadState.Running) { TH1.Abort(); }
            while (TH2 != null && TH2.ThreadState == ThreadState.Running) { TH2.Abort(); }
            while (TH3 != null && TH3.ThreadState == ThreadState.Running) { TH3.Abort(); }
            while (TH4 != null && TH4.ThreadState == ThreadState.Running) { TH4.Abort(); }

            while (TH1.ThreadState == ThreadState.Running) ;
            while (TH2.ThreadState == ThreadState.Running) ;
            while (TH3.ThreadState == ThreadState.Running) ;
            while (TH4.ThreadState == ThreadState.Running) ;

            SourPic.Dispose(); DestPic.Dispose();
        }

        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////

        private static void cmp1()
        {

        }
        private static void cmp2()
        {

        }
        private static void cmp3()
        {

        }
        private static void cmp4()
        {

        }
    }
}
