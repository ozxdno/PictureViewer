using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using PictureViewer.Class;
using System.IO;

namespace PictureViewer
{
    public partial class Form_Find : Form
    {
        /////////////////////////////////////////////////////////// public attribute //////////////////////////////////////////////////

        /// <summary>
        /// 需要找的图片
        /// </summary>
        public static Bitmap SourPic;
        /// <summary>
        /// 正在显示的寻找结果图片
        /// </summary>
        public static Bitmap DestPic;
        /// <summary>
        /// 查找是否已经结束
        /// </summary>
        public static bool IsFinish = false;
        /// <summary>
        /// 查找模式
        /// </summary>
        public enum MODE : ushort
        {
            /// <summary>
            /// 默认：FULL + SAME + NO_TURN
            /// </summary>
            DEFAULT = 0x0000,
            /// <summary>
            /// 大小必须相同
            /// </summary>
            FULL = 0x0001,
            /// <summary>
            /// 大小可以不同
            /// </summary>
            PART = 0x0002,
            /// <summary>
            /// 完全相同
            /// </summary>
            SAME = 0x0004,
            /// <summary>
            /// 部分相似
            /// </summary>
            LIKE = 0x0010,
            /// <summary>
            /// 经过了旋转和翻转
            /// </summary>
            TURN = 0x0020
        }

        /////////////////////////////////////////////////////////// private attribute //////////////////////////////////////////////////

        private System.Timers.Timer Timer = new System.Timers.Timer(100);

        private static List<string> Paths = new List<string>();
        private static List<string> Names = new List<string>();
        private static CONFIG config;
        private static THREAD[] Threads = new THREAD[4];

        private struct THREAD
        {
            public Thread thread;
            public Bitmap sour;
            public bool finish;
            public int begin;
            public int end;
            public List<int> result;
        }
        private struct CONFIG
        {
            /// <summary>
            /// 当前的显示结果
            /// </summary>
            public List<int> Current;
            /// <summary>
            /// 当前显示结果的索引号
            /// </summary>
            public int Index;
            /// <summary>
            /// 源文件文件名
            /// </summary>
            public string Sour;
            /// <summary>
            /// 当前显示结果的路径
            /// </summary>
            public string Path;
            /// <summary>
            /// 当前显示结果的名称
            /// </summary>
            public string Name;

            /// <summary>
            /// 线程锁
            /// </summary>
            public object Lock;
            /// <summary>
            /// 匹配模式
            /// </summary>
            public MODE Mode;
            /// <summary>
            /// 最小比较元素个数
            /// </summary>
            public int MinCmpPix;
            /// <summary>
            /// 待比较的灰度值序列
            /// </summary>
            public int[] Grays;
            /// <summary>
            /// Grays 所在行
            /// </summary>
            public int Row;
            /// <summary>
            /// Grays 所在列
            /// </summary>
            public int Col;
            /// <summary>
            /// 比对结果
            /// </summary>
            public List<int> Results;
            
            /// <summary>
            /// 计时起点
            /// </summary>
            public long TimeBG;
            /// <summary>
            /// 计时终点
            /// </summary>
            public long TimeED;
            /// <summary>
            /// 时间计数器
            /// </summary>
            public long CountTime;
            /// <summary>
            /// 文件计数器
            /// </summary>
            public int CountFiles;
        }

        /////////////////////////////////////////////////////////// public method //////////////////////////////////////////////////

        public Form_Find(Image pic, string fullpath, MODE mode = MODE.DEFAULT)
        {
            InitializeComponent();

            SourPic = CopyPicture((Bitmap)pic, 1);
            config.Sour = fullpath;
            config.Mode = mode;
        }
        
        /////////////////////////////////////////////////////////// private method //////////////////////////////////////////////////

        private void Form_Load(object sender, EventArgs e)
        {
            InitializeForm();
            ShowSourPic();
            //ShowDestPic();
            
            Timer.Elapsed += new System.Timers.ElapsedEventHandler(Form_Updata);
            Timer.AutoReset = true;
            Timer.Start();

            Start();
        }
        private void Form_Close(object sender, FormClosedEventArgs e)
        {
            while (Threads[0].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[0].thread.Abort(); }
            while (Threads[1].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[1].thread.Abort(); }
            while (Threads[2].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[2].thread.Abort(); }
            while (Threads[3].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[3].thread.Abort(); }

            Timer.Close();

            try { SourPic.Dispose(); } catch { }
            try { DestPic.Dispose(); } catch { }
            for (int i = 0; i < Threads.Length; i++) { try { Threads[i].sour.Dispose(); } catch { } }
        }
        private void Form_Updata(object source, System.Timers.ElapsedEventArgs e)
        {
            this.BeginInvoke((EventHandler)delegate{

                config.CountTime++;

                bool prevState = IsFinish;
                IsFinish = Threads[0].finish && Threads[1].finish && Threads[2].finish && Threads[3].finish;

                if (!IsFinish) { config.TimeED = config.CountTime; }
                double usedTime = (double)(config.TimeED - config.TimeBG) / 10;
                string usedtime = usedTime.ToString();
                if (usedtime.Length < 2 || usedtime[usedtime.Length - 2] != '.') { usedtime += ".0"; }
                int findfiles = 0; lock (config.Lock) { findfiles = config.CountFiles; }
                this.Text = IsFinish ?
                    "[Find]: " + findfiles.ToString() + "/" + Names.Count.ToString() + " Files [State]: Finished [Used Time]: " + usedtime + " s" :
                    "[Find]: " + findfiles.ToString() + "/" + Names.Count.ToString() + " Files [State]: Finding... [Used Time]: " + usedtime + " s";

                ShowList();

                //if (config.Index != this.listBox1.SelectedIndex && this.listBox1.SelectedIndex == -1)
                //{ try { this.listBox1.SelectedIndex = config.Index; } catch { } }
                
                if (IsFinish && prevState != IsFinish)
                {
                    this.startToolStripMenuItem.Text = "Start";
                    config.Index = 0;
                    ShowDestPic();
                    //this.listBox1.SelectedIndex = 0;
                }

                int ptX = MousePosition.X - this.Location.X;
                int ptY = MousePosition.Y - this.Location.Y;
                int xbg, xed;
                int ybg, yed;

                xbg = this.label4.Location.X; xed = xbg + this.label4.Width;
                ybg = this.label4.Location.Y; yed = ybg + this.label4.Height;
                this.label4.Visible = xbg <= ptX && ptX <= xed && ybg <= ptY && ptY <= yed && config.Current.Count > 1;

                xbg = this.label5.Location.X; xed = xbg + this.label5.Width;
                ybg = this.label5.Location.Y; yed = ybg + this.label5.Height;
                this.label5.Visible = xbg <= ptX && ptX <= xed&& ybg <= ptY && ptY <= yed && config.Current.Count > 1;

                xbg = this.label1.Location.X + 10; xed = xbg + this.label1.Width;
                ybg = this.label1.Location.Y + 30; yed = ybg + this.label1.Height;
                this.label1.Visible = xbg <= ptX && ptX <= xed && ybg <= ptY && ptY <= yed && config.Current.Count > 0;

                xbg = this.label2.Location.X + 10; xed = xbg + this.label2.Width;
                ybg = this.label2.Location.Y + 30; yed = ybg + this.label2.Height;
                this.label2.Visible = xbg <= ptX && ptX <= xed && ybg <= ptY && ptY <= yed && config.Current.Count > 0;

                //if (this.label1.Visible || this.label2.Visible || this.label4.Visible || this.label5.Visible)
                //{
                //}
                //else {  }
            });
        }
        
        private void Previous(object sender, EventArgs e)
        {
            config.Index--;
            int total = 0; lock (config.Lock) { total = config.Results.Count; }
            if (config.Index < 0) { config.Index = total - 1; }

            ShowDestPic();
            try { this.listBox1.SelectedIndex = config.Index; } catch { }
        }
        private void Next(object sender, EventArgs e)
        {
            config.Index++;
            int total = 0; lock (config.Lock) { total = config.Results.Count; }
            if (config.Index >= total) { config.Index = 0; }

            ShowDestPic();
            try { this.listBox1.SelectedIndex = config.Index; } catch { }
        }
        private void ListClicked(object sender, EventArgs e)
        {
            int index = this.listBox1.SelectedIndex;
            if (index == -1) { return; }
            config.Index = index;
            ShowDestPic();
        }

        private void RightMenu_Start(object sender, EventArgs e)
        {
            if (this.startToolStripMenuItem.Text == "Start") { Start(); return; }
            if (this.startToolStripMenuItem.Text == "Stop") { Stop(); return; }
            this.startToolStripMenuItem.Text = "Start";
        }
        private void RightMenu_Export(object sender, EventArgs e)
        {
            if (config.Index < 0 || config.Index >= config.Current.Count) { MessageBox.Show("文件不存在！", "提示"); return; }
            int index = config.Current[config.Index];
            if (index < 0 || index >= Names.Count) { MessageBox.Show("文件不存在！", "提示"); return; }

            string sourpath = Paths[index];
            string sourname = Names[index];
            string destpath = Form_Main.config.ExportFolder;
            string destname = sourname;

            if (!Directory.Exists(destpath)) { destpath = FileOperate.getExePath(); }

            string sour = sourpath + "\\" + sourname;
            string dest = destpath + "\\" + destname;

            //if (!Directory.Exists(destpath)) { MessageBox.Show("输出路径不存在！", "提示"); return; }
            if ( File.Exists(dest)) { MessageBox.Show("目标文件夹存在同名文件！", "提示"); return; }
            if (!File.Exists(sour)) { MessageBox.Show("该文件不存在！", "提示"); return; }

            if (DialogResult.Cancel == MessageBox.Show("把 “" + sour  + "” 导出到：\n" + dest  + "？", "确认导出", MessageBoxButtons.OKCancel))
            { return; }

            if (DestPic != null) { DestPic.Dispose(); }
            File.Move(sour, dest);

            lock (config.Lock) { config.Results.RemoveAt(config.Index); }
            config.Current.RemoveAt(config.Index);
            index = this.listBox1.SelectedIndex;
            if (index < 0) { index = 0; }
            if (index >= config.Current.Count) { index = config.Current.Count - 1; }
            this.listBox1.Items.RemoveAt(config.Index);
            this.listBox1.SelectedIndex = index;

            config.Index = index;
            ShowDestPic();
        }
        private void RightMenu_Export2(object sender, EventArgs e)
        {
            MessageBox.Show("功能暂时没有实现！", "提示");
        }
        private void RightMenu_Open(object sender, EventArgs e)
        {
            if (config.Index < 0 || config.Index >= config.Current.Count) { MessageBox.Show("文件不存在！", "提示"); return; }
            int index = config.Current[config.Index];
            if (index < 0 || index >= Names.Count) { MessageBox.Show("文件不存在！", "提示"); return; }

            string sourpath = Paths[index];
            string sourname = Names[index];
            string sour = sourpath + "\\" + sourname;

            if (!File.Exists(sour)) { MessageBox.Show("该文件不存在！", "提示"); return; }

            System.Diagnostics.Process.Start("Explorer", "/select," + sour);
        }
        private void RightMenu_Pixes(object sender, EventArgs e)
        {
            Form_Input input = new Form_Input();
            input.Location = MousePosition;
            input.ShowDialog();

            int MinCmpPixes = 0;
            try { MinCmpPixes = int.Parse(input.Input); } catch { MessageBox.Show("必须输入正整数！", "提示"); return; }
            if (MinCmpPixes < 0) { MessageBox.Show("必须输入正整数！", "提示"); return; }

            config.MinCmpPix = Math.Min(Math.Max(SourPic.Height, SourPic.Width), MinCmpPixes);
        }

        private void ShowSourPic()
        {
            int hBox = this.pictureBox2.Height;
            int wBox = this.pictureBox2.Width;
            int hPic = SourPic.Height;
            int wPic = SourPic.Width;

            double rate = Math.Min((double)hBox / hPic, (double)wBox / wPic);
            Bitmap dest = CopyPicture(SourPic, rate);
            this.pictureBox2.BackgroundImage = dest;

            int cut = config.Sour.LastIndexOf('\\');
            string path = config.Sour.Substring(0, cut);
            string name = config.Sour.Substring(cut + 1);

            FileInfo file = new FileInfo(config.Sour);
            long size = file.Length / 1000;

            this.toolTip2.ToolTipTitle = path;
            this.toolTip2.SetToolTip(this.pictureBox2, "[" + size.ToString() + " KB] " + name);
        }
        private void ShowDestPic()
        {
            int Index = 0;
            if (config.Index < 0 || config.Index >= config.Current.Count) { Index = -1; }
            else { Index = config.Current[config.Index]; }
            
            if (Index < 0 || Index >= Paths.Count)
            {
                config.Path = "Not Exist "; config.Name = " Unknow";
                if (DestPic != null) { DestPic.Dispose(); }
                string unkfile = FileOperate.getExePath() + "\\unk.tip";
                if (File.Exists(unkfile)) { DestPic = (Bitmap)Image.FromFile(unkfile); }
                else { DestPic = null; }

                this.pictureBox1.BackgroundImage = DestPic;

                this.toolTip1.ToolTipTitle = config.Path;
                this.toolTip1.SetToolTip(this.listBox1, config.Name);
            }
            else
            {
                config.Path = Paths[Index]; config.Name = Names[Index];
                if (DestPic != null) { DestPic.Dispose(); }
                DestPic = (Bitmap)Image.FromFile(config.Path + "\\" + config.Name);

                int hBox = this.pictureBox1.Height;
                int wBox = this.pictureBox1.Width;
                int hPic = DestPic.Height;
                int wPic = DestPic.Width;

                double rate = Math.Min((double)hBox / hPic, (double)wBox / wPic);
                Bitmap dest = CopyPicture(DestPic, rate);
                this.pictureBox1.BackgroundImage = dest;

                FileInfo file = new FileInfo(config.Path + "\\" + config.Name);
                long size = file.Length / 1000;

                this.toolTip1.ToolTipTitle = config.Path;
                this.toolTip1.SetToolTip(this.listBox1, "[" + size.ToString() + " KB] " + config.Name);
            }
        }
        private void ShowList()
        {
            List<int> newResult = new List<int>();
            lock (config.Lock)
            {
                int nOld = config.Current.Count;
                int nNew = config.Results.Count;

                for (int i = nOld; i < nNew; i++) { newResult.Add(config.Results[i]); }
            }
            foreach (int index in newResult)
            {
                config.Current.Add(index);
                string sequence = "[" + config.Current.Count.ToString() + "] ";
                string name = Names[index];
                if (name.Length > 24) { name = "." + name.Substring(name.Length - 24); }

                this.listBox1.Items.Add(sequence + name);
            }
        }

        private void Start()
        {
            this.listBox1.Items.Clear();
            config.Results.Clear();
            config.Current.Clear();

            while (Threads[0].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[0].thread.Abort(); }
            while (Threads[1].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[1].thread.Abort(); }
            while (Threads[2].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[2].thread.Abort(); }
            while (Threads[3].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[3].thread.Abort(); }

            double pace = (double)Paths.Count / Threads.Length;
            double bg = 0, ed = 0;

            bg = ed; ed = bg + pace;
            Threads[0].thread = new Thread(TH_CMP1);
            Threads[0].sour = CopyPicture(SourPic, 1);
            Threads[0].finish = false;
            Threads[0].begin = 1;
            Threads[0].end = (int)ed;
            Threads[0].result = new List<int>();

            bg = ed; ed = bg + pace;
            Threads[1].thread = new Thread(TH_CMP2);
            Threads[1].sour = CopyPicture(SourPic, 1);
            Threads[1].finish = false;
            Threads[1].begin = (int)bg + 1;
            Threads[1].end = (int)ed;
            Threads[1].result = new List<int>();

            bg = ed; ed = bg + pace;
            Threads[2].thread = new Thread(TH_CMP3);
            Threads[2].sour = CopyPicture(SourPic, 1);
            Threads[2].finish = false;
            Threads[2].begin = (int)bg + 1;
            Threads[2].end = (int)ed;
            Threads[2].result = new List<int>();

            bg = ed; ed = bg + pace;
            Threads[3].thread = new Thread(TH_CMP4);
            Threads[3].sour = CopyPicture(SourPic, 1);
            Threads[3].finish = false;
            Threads[3].begin = (int)bg + 1;
            Threads[3].end = (int)ed;
            Threads[3].result = new List<int>();

            config.TimeBG = config.CountTime;

            //Threads[0].begin = 1;
            //Threads[0].end = Names.Count;
            //Threads[1].begin = 1;
            //Threads[1].end = 0;
            //Threads[2].begin = 1;
            //Threads[2].end = 0;
            //Threads[3].begin = 1;
            //Threads[3].end = 0;
            //Threads[1].finish = true;
            //Threads[2].finish = true;
            //Threads[3].finish = true;

            Threads[0].thread.Start();
            Threads[1].thread.Start();
            Threads[2].thread.Start();
            Threads[3].thread.Start();

            this.startToolStripMenuItem.Text = "Stop"; return;
        }
        private void Stop()
        {
            while (Threads[0].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[0].thread.Abort(); }
            while (Threads[1].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[1].thread.Abort(); }
            while (Threads[2].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[2].thread.Abort(); }
            while (Threads[3].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[3].thread.Abort(); }

            this.startToolStripMenuItem.Text = "Start"; return;
        }

        private void InitializeForm()
        {
            #region 填充 config

            config.Current = new List<int>();
            config.Index = 0;
            config.Path = "Not Exist";
            config.Name = "Unknow";

            config.Lock = new object();
            if ((config.Mode & MODE.PART) == 0) { config.Mode |= MODE.FULL; }
            else { config.Mode &= (~MODE.FULL); }
            if ((config.Mode & MODE.LIKE) == 0) { config.Mode |= MODE.SAME; }
            else { config.Mode &= (~MODE.SAME); }
            config.MinCmpPix = Math.Min(Math.Max(SourPic.Height, SourPic.Width), 50);
            config.Grays = new int[config.MinCmpPix];
            config.Row = -1;
            config.Col = -1;
            config.Results = new List<int>();
            
            config.TimeBG = 0;
            config.TimeED = 0;
            config.CountTime = 0;
            config.CountFiles = 0;

            #endregion

            #region 填充 THREADS

            Threads[0].finish = true;
            Threads[1].finish = true;
            Threads[2].finish = true;
            Threads[3].finish = true;

            #endregion

            #region 填充 Paths 和 Names

            string path;
            string name;
            string ex;
            bool NoHide = Form_Main.NoHide;
            Paths.Clear();
            Names.Clear();

            for (int p = 0; p < FileOperate.RootFiles.Count; p++)
            {
                path = FileOperate.RootFiles[p].Path;

                for (int n = 0; n < FileOperate.RootFiles[p].Name.Count; n++)
                {
                    name = FileOperate.RootFiles[p].Name[n];
                    ex = FileOperate.getExtension(name);
                    int type = FileOperate.getFileType(ex);

                    if (type == 1)
                    {
                        string newpath = path + "\\" + name;
                        List<string> files = FileOperate.getSubFiles(newpath);
                        foreach (string f in files)
                        {
                            name = f;
                            ex = FileOperate.getExtension(name);
                            type = FileOperate.getFileType(ex);

                            if (type == 2)
                            {
                                if (!NoHide && FileOperate.IsSupportHide(ex)) { continue; }
                                if (config.Sour == newpath + "\\" + name) { continue; }
                                Paths.Add(newpath); Names.Add(name);
                            }
                        }

                        continue;
                    }
                    if (type == 2)
                    {
                        if (!NoHide && FileOperate.IsSupportHide(ex)) { continue; }
                        if (config.Sour == path + "\\" + name) { continue; }
                        Paths.Add(path); Names.Add(name);
                        continue;
                    }
                }
            }
            
            #endregion

            #region 填充灰度值

            int H = SourPic.Height, W = SourPic.Width;
            if (H > W) { GetBestCol(); } else { GetBestRow(); }

            #endregion
        }
        private Bitmap CopyPicture(Bitmap sour, double rate)
        {
            if (rate > 1) { rate = 1; }
            if (rate < 0) { rate = 0; }
            int H = (int)(sour.Height * rate), W = (int)(sour.Width * rate);
            Bitmap dest = new Bitmap(W, H);

            Graphics g = Graphics.FromImage(dest);
            g.DrawImage(sour, new Rectangle(0, 0, W, H), new Rectangle(0, 0, sour.Width, sour.Height), GraphicsUnit.Pixel);

            try { g.ReleaseHdc(); } catch { }
            try { g.Dispose(); } catch { }

            return dest;
        }
        private Bitmap CopyPicture(Bitmap sour, int destW, int destH)
        {
            Bitmap dest = new Bitmap(destW, destH);

            Graphics g = Graphics.FromImage(dest);
            g.DrawImage(sour, new Rectangle(0, 0, destW, destH), new Rectangle(0, 0, sour.Width, sour.Height), GraphicsUnit.Pixel);

            try { g.ReleaseHdc(); } catch { }
            try { g.Dispose(); } catch { }

            return dest;
        }

        private void GetBestRow()
        {
            int bg = SourPic.Height * 1 / 4;
            int ed = SourPic.Height * 3 / 4;

            int dif = 0, max = 0, idx = bg;
            for (int i = bg; i < ed; i++)
            {
                dif = GetDiffRow(i);
                if (dif > max) { max = dif; idx = i; }
            }

            config.Row = idx;
            List<int> grays = new List<int>(); GetRowGrays(idx, ref grays);
            config.Grays = grays.ToArray();
        }
        private void GetBestCol()
        {
            int bg = SourPic.Width * 1 / 4;
            int ed = SourPic.Width * 3 / 4;

            int dif = 0, max = 0, idx = bg;
            for (int i = bg; i < ed; i++)
            {
                dif = GetDiffCol(i);
                if (dif > max) { max = dif; idx = i; }
            }

            config.Col = idx;
            List<int> grays = new List<int>(); GetColGrays(idx, ref grays);
            config.Grays = grays.ToArray();
        }
        private int GetDiffRow(int row)
        {
            List<int> grays = new List<int>();
            GetRowGrays(row, ref grays);
            
            for (int i = 0; i < grays.Count; i++)
            {
                for (int j = grays.Count - 1; j > i; j--)
                {
                    if (grays[i] == grays[j]) { grays.RemoveAt(j); }
                }
            }

            return grays.Count;

            //List<int> indexes = new List<int>();
            //for (int i = 0; i < SourPic.Width; i++) { indexes.Add(i); }

            //for (int i = 0; i < indexes.Count; i++)
            //{
            //    int c1 = ToGray(SourPic.GetPixel(indexes[i], row));
            //    for (int j = indexes.Count - 1; j > i; j--)
            //    {
            //        int c2 = ToGray(SourPic.GetPixel(indexes[j], row));
            //        if (c1 == c2) { indexes.RemoveAt(j); continue; }
            //    }
            //}

            //return indexes.Count;
        }
        private int GetDiffCol(int col)
        {
            List<int> grays = new List<int>();
            GetColGrays(col, ref grays);

            for (int i = 0; i < grays.Count; i++)
            {
                for (int j = grays.Count - 1; j > i; j--)
                {
                    if (grays[i] == grays[j]) { grays.RemoveAt(j); }
                }
            }

            return grays.Count;

            //List<int> indexes = new List<int>();
            //for (int i = 0; i < SourPic.Height; i++) { indexes.Add(i); }

            //for (int i = 0; i < indexes.Count; i++)
            //{
            //    int c1 = ToGray(SourPic.GetPixel(col, indexes[i]));
            //    for (int j = indexes.Count - 1; j > i; j--)
            //    {
            //        int c2 = ToGray(SourPic.GetPixel(col, indexes[j]));
            //        if (c1 == c2) { indexes.RemoveAt(j); continue; }
            //    }
            //}

            //return indexes.Count;
        }
        private void GetRowGrays(int row, ref List<int> grays)
        {
            double pace = (double)SourPic.Width / config.MinCmpPix;
            if (grays == null) { grays = new List<int>(); } else { grays.Clear(); }
            int idx = 0;

            for (double i = 0; i < SourPic.Width; i += pace, idx++)
            {
                if (idx == config.MinCmpPix - 1) { idx--; break; }
                grays.Add(ToGray(SourPic.GetPixel((int)i, row)));
            }
        }
        private void GetColGrays(int col, ref List<int> grays)
        {
            double pace = (double)SourPic.Height / config.MinCmpPix;
            if (grays == null) { grays = new List<int>(); } else { grays.Clear(); }
            int idx = 0;

            for (double i = 0; i < SourPic.Height; i += pace, idx++)
            {
                if (idx == config.MinCmpPix - 1) { idx--; break; }
                grays.Add(ToGray(SourPic.GetPixel(col, (int)i)));
            }
        }
        
        private int ToGray(Color c)
        {
            return (c.R + c.G + c.B) / 3;
            //return (int)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B);
        }
        private int ToGray(int R, int G, int B)
        {
            return (R + B + G) / 3;
            //return (int)(0.3 * R + 0.59 * G + 0.11 * B);
        }

        private void TH_CMP1()
        {
            Bitmap sour = Threads[0].sour;
            Bitmap dest;
            int sourh = sour.Height, sourw = sour.Width;
            int desth = 0, destw = 0;
            bool found = false;
            
            for (int i = Threads[0].begin - 1; i < Threads[0].end; ++i)
            {
                #region 初始化变量
                
                dest  = (Bitmap)Image.FromFile(Paths[i] + "\\" + Names[i]);
                desth = dest.Height;
                destw = dest.Width;
                found = false;

                #endregion

                #region 行比较

                if (config.Row != -1)
                {
                    #region FULL SAME NO_TURN

                    if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        if (sourh != desth || sourw != destw) { goto End; }
                        
                        double pace = (double)sourw / config.MinCmpPix;
                        int permitcnterr = config.MinCmpPix / 10;
                        int permiterr = 10;
                        int cnterr = 0;
                        double x = 0;
                        int cnt = 0;
                        int len = config.Grays.Length;

                        for (; cnt < len; x += pace, ++cnt)
                        {
                            Color dc = dest.GetPixel((int)x, config.Row);
                            int sgray = config.Grays[cnt];
                            int dgray = (dc.R + dc.G + dc.B) / 3;

                            int ierr = sgray - dgray;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                    }

                    #endregion

                    #region FULL SAME TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel((int)x, config.Row);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int cmprow = sourh - 1 - config.Row;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(sourw - 1 - (int)x, cmprow);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(config.Row, sourw - 1 - (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int cmprow = sourh - 1 - config.Row;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(cmprow, (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        // 比例相同
                        double h2w = (double)sourh / sourw;
                        int err = (int)(h2w * destw) - desth; if (err < 0) { err = -err; }
                        if (err > 3) { goto End; }

                        // 缩放
                        Bitmap smap, dmap;
                        double rate = 0;
                        if (sourh > desth)
                        {
                            smap = new Bitmap(destw, desth);
                            Graphics g = Graphics.FromImage(smap);
                            g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = sourh / desth;
                        }
                        else { smap = sour; }
                        if (sourh < desth)
                        {
                            dmap = new Bitmap(sourw, sourh);
                            Graphics g = Graphics.FromImage(dmap);
                            g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = desth / sourh;
                        }
                        else { dmap = dest; }

                        desth = dmap.Height;
                        destw = dmap.Width;
                        int cmprow = (int)((double)config.Row / sourh * desth);
                        
                        //List<long> errlist = new List<long>();
                        int permitcnterr = destw / 10 + (int)(rate * 10);
                        int permiterr = 10;
                        int cnterr = 0;
                        
                        for (int x = 0; x < destw; ++x)
                        {
                            Color sc = smap.GetPixel(x, cmprow);
                            Color dc = dmap.GetPixel(x, cmprow);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            //errlist.Add(ierr);
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (sourh > desth) { smap.Dispose(); }
                        if (sourh < desth) { dmap.Dispose(); }
                    }

                    #endregion

                    #region PART SAME TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        double h2w = (double)sourh / sourw;

                        #region 原样

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int cmprow = (int)((double)config.Row / sourh * desth);

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int x = 0; x < destw; ++x)
                            {
                                Color sc = smap.GetPixel(x, cmprow);
                                Color dc = dmap.GetPixel(x, cmprow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int srow = (int)((double)config.Row / sourh * desth);
                            int drow = desth - 1 - srow;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int x = 0; x < destw; ++x)
                            {
                                Color sc = smap.GetPixel(x, srow);
                                Color dc = dmap.GetPixel(destw - 1 - x, drow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int srow = (int)((double)config.Row / sourh * destw);
                            int drow = srow;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(y, srow);
                                Color dc = dmap.GetPixel(drow, desth - 1 - y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int srow = (int)((double)config.Row / sourh * destw);
                            int drow = destw - 1 - srow;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(y, srow);
                                Color dc = dmap.GetPixel(drow, y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion
                }

                #endregion

                #region 列比较

                if (config.Col != -1)
                {
                    #region FULL SAME NO_TURN

                    if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        if (sourh != desth || sourw != destw) { goto End; }

                        double pace = (double)sourh / config.MinCmpPix;
                        int permitcnterr = config.MinCmpPix / 10;
                        int permiterr = 10;
                        int cnterr = 0;
                        double y = 0;
                        int cnt = 0;
                        int len = config.Grays.Length;

                        for (; cnt < len; y += pace, ++cnt)
                        {
                            Color dc = dest.GetPixel(config.Col, (int)y);
                            int sgray = config.Grays[cnt];
                            int dgray = (dc.R + dc.G + dc.B) / 3;

                            int ierr = sgray - dgray;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                    }

                    #endregion

                    #region FULL SAME TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(config.Col, (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int cmpcol = sourw - config.Col - 1;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(cmpcol, sourh - 1 - (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int cmpcol = sourw - config.Col - 1;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel((int)x, cmpcol);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(sourh - 1 - (int)x, config.Col);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        // 比例相同
                        double h2w = (double)sourh / sourw;
                        int err = (int)(h2w * destw) - desth; if (err < 0) { err = -err; }
                        if (err > 3) { goto End; }

                        // 缩放
                        Bitmap smap, dmap;
                        double rate = 0;
                        if (sourh > desth)
                        {
                            smap = new Bitmap(destw, desth);
                            Graphics g = Graphics.FromImage(smap);
                            g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = sourh / desth;
                        }
                        else { smap = sour; }
                        if (sourh < desth)
                        {
                            dmap = new Bitmap(sourw, sourh);
                            Graphics g = Graphics.FromImage(dmap);
                            g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = desth / sourh;
                        }
                        else { dmap = dest; }

                        desth = dmap.Height;
                        destw = dmap.Width;
                        int cmpcol = (int)((double)config.Col / sourw * destw);

                        //List<long> errlist = new List<long>();
                        int permitcnterr = desth / 10 + (int)(rate * 10);
                        int permiterr = 10;
                        int cnterr = 0;

                        for (int y = 0; y < desth; ++y)
                        {
                            Color sc = smap.GetPixel(cmpcol, y);
                            Color dc = dmap.GetPixel(cmpcol, y);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            //errlist.Add(ierr);
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (sourh > desth) { smap.Dispose(); }
                        if (sourh < desth) { dmap.Dispose(); }
                    }

                    #endregion

                    #region PART SAME TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        double h2w = (double)sourh / sourw;

                        #region 原样

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int cmpcol = (int)((double)config.Col / sourw * destw);

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(cmpcol, y);
                                Color dc = dmap.GetPixel(cmpcol, y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int scol = (int)((double)config.Col / sourw * destw);
                            int dcol = destw - 1 - scol;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(scol, y);
                                Color dc = dmap.GetPixel(dcol, desth - 1 - y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int scol = (int)((double)config.Col / sourw * desth);
                            int dcol = desth - 1 - scol;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < destw; ++y)
                            {
                                Color sc = smap.GetPixel(scol, y);
                                Color dc = dmap.GetPixel(y, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int scol = (int)((double)config.Col / sourw * desth);
                            int dcol = scol;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < destw; ++y)
                            {
                                Color sc = smap.GetPixel(scol, y);
                                Color dc = dmap.GetPixel(destw - 1 - y, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion
                }

                #endregion

                End:
                lock (config.Lock) { if (found) { config.Results.Add(i); } config.CountFiles++; }
                dest.Dispose();
            }

            Threads[0].finish = true;
        }
        private void TH_CMP2()
        {
            Bitmap sour = Threads[1].sour;
            Bitmap dest;
            int sourh = sour.Height, sourw = sour.Width;
            int desth = 0, destw = 0;
            bool found = false;

            for (int i = Threads[1].begin - 1; i < Threads[1].end; ++i)
            {
                #region 初始化变量

                dest = (Bitmap)Image.FromFile(Paths[i] + "\\" + Names[i]);
                desth = dest.Height;
                destw = dest.Width;
                found = false;

                #endregion

                #region 行比较

                if (config.Row != -1)
                {
                    #region FULL SAME NO_TURN

                    if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        if (sourh != desth || sourw != destw) { goto End; }

                        double pace = (double)sourw / config.MinCmpPix;
                        int permitcnterr = config.MinCmpPix / 10;
                        int permiterr = 10;
                        int cnterr = 0;
                        double x = 0;
                        int cnt = 0;
                        int len = config.Grays.Length;

                        for (; cnt < len; x += pace, ++cnt)
                        {
                            Color dc = dest.GetPixel((int)x, config.Row);
                            int sgray = config.Grays[cnt];
                            int dgray = (dc.R + dc.G + dc.B) / 3;

                            int ierr = sgray - dgray;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                    }

                    #endregion

                    #region FULL SAME TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel((int)x, config.Row);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int cmprow = sourh - 1 - config.Row;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(sourw - 1 - (int)x, cmprow);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(config.Row, sourw - 1 - (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int cmprow = sourh - 1 - config.Row;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(cmprow, (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        // 比例相同
                        double h2w = (double)sourh / sourw;
                        int err = (int)(h2w * destw) - desth; if (err < 0) { err = -err; }
                        if (err > 3) { goto End; }

                        // 缩放
                        Bitmap smap, dmap;
                        double rate = 0;
                        if (sourh > desth)
                        {
                            smap = new Bitmap(destw, desth);
                            Graphics g = Graphics.FromImage(smap);
                            g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = sourh / desth;
                        }
                        else { smap = sour; }
                        if (sourh < desth)
                        {
                            dmap = new Bitmap(sourw, sourh);
                            Graphics g = Graphics.FromImage(dmap);
                            g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = desth / sourh;
                        }
                        else { dmap = dest; }

                        desth = dmap.Height;
                        destw = dmap.Width;
                        int cmprow = (int)((double)config.Row / sourh * desth);

                        //List<long> errlist = new List<long>();
                        int permitcnterr = destw / 10 + (int)(rate * 10);
                        int permiterr = 10;
                        int cnterr = 0;

                        for (int x = 0; x < destw; ++x)
                        {
                            Color sc = smap.GetPixel(x, cmprow);
                            Color dc = dmap.GetPixel(x, cmprow);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            //errlist.Add(ierr);
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (sourh > desth) { smap.Dispose(); }
                        if (sourh < desth) { dmap.Dispose(); }
                    }

                    #endregion

                    #region PART SAME TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        double h2w = (double)sourh / sourw;

                        #region 原样

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int cmprow = (int)((double)config.Row / sourh * desth);

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int x = 0; x < destw; ++x)
                            {
                                Color sc = smap.GetPixel(x, cmprow);
                                Color dc = dmap.GetPixel(x, cmprow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int srow = (int)((double)config.Row / sourh * desth);
                            int drow = desth - 1 - srow;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int x = 0; x < destw; ++x)
                            {
                                Color sc = smap.GetPixel(x, srow);
                                Color dc = dmap.GetPixel(destw - 1 - x, drow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int srow = (int)((double)config.Row / sourh * destw);
                            int drow = srow;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(y, srow);
                                Color dc = dmap.GetPixel(drow, desth - 1 - y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int srow = (int)((double)config.Row / sourh * destw);
                            int drow = destw - 1 - srow;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(y, srow);
                                Color dc = dmap.GetPixel(drow, y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion
                }

                #endregion

                #region 列比较

                if (config.Col != -1)
                {
                    #region FULL SAME NO_TURN

                    if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        if (sourh != desth || sourw != destw) { goto End; }

                        double pace = (double)sourh / config.MinCmpPix;
                        int permitcnterr = config.MinCmpPix / 10;
                        int permiterr = 10;
                        int cnterr = 0;
                        double y = 0;
                        int cnt = 0;
                        int len = config.Grays.Length;

                        for (; cnt < len; y += pace, ++cnt)
                        {
                            Color dc = dest.GetPixel(config.Col, (int)y);
                            int sgray = config.Grays[cnt];
                            int dgray = (dc.R + dc.G + dc.B) / 3;

                            int ierr = sgray - dgray;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                    }

                    #endregion

                    #region FULL SAME TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(config.Col, (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int cmpcol = sourw - config.Col - 1;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(cmpcol, sourh - 1 - (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int cmpcol = sourw - config.Col - 1;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel((int)x, cmpcol);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(sourh - 1 - (int)x, config.Col);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        // 比例相同
                        double h2w = (double)sourh / sourw;
                        int err = (int)(h2w * destw) - desth; if (err < 0) { err = -err; }
                        if (err > 3) { goto End; }

                        // 缩放
                        Bitmap smap, dmap;
                        double rate = 0;
                        if (sourh > desth)
                        {
                            smap = new Bitmap(destw, desth);
                            Graphics g = Graphics.FromImage(smap);
                            g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = sourh / desth;
                        }
                        else { smap = sour; }
                        if (sourh < desth)
                        {
                            dmap = new Bitmap(sourw, sourh);
                            Graphics g = Graphics.FromImage(dmap);
                            g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = desth / sourh;
                        }
                        else { dmap = dest; }

                        desth = dmap.Height;
                        destw = dmap.Width;
                        int cmpcol = (int)((double)config.Col / sourw * destw);

                        //List<long> errlist = new List<long>();
                        int permitcnterr = desth / 10 + (int)(rate * 10);
                        int permiterr = 10;
                        int cnterr = 0;

                        for (int y = 0; y < desth; ++y)
                        {
                            Color sc = smap.GetPixel(cmpcol, y);
                            Color dc = dmap.GetPixel(cmpcol, y);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            //errlist.Add(ierr);
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (sourh > desth) { smap.Dispose(); }
                        if (sourh < desth) { dmap.Dispose(); }
                    }

                    #endregion

                    #region PART SAME TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        double h2w = (double)sourh / sourw;

                        #region 原样

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int cmpcol = (int)((double)config.Col / sourw * destw);

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(cmpcol, y);
                                Color dc = dmap.GetPixel(cmpcol, y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int scol = (int)((double)config.Col / sourw * destw);
                            int dcol = destw - 1 - scol;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(scol, y);
                                Color dc = dmap.GetPixel(dcol, desth - 1 - y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int scol = (int)((double)config.Col / sourw * desth);
                            int dcol = desth - 1 - scol;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < destw; ++y)
                            {
                                Color sc = smap.GetPixel(scol, y);
                                Color dc = dmap.GetPixel(y, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int scol = (int)((double)config.Col / sourw * desth);
                            int dcol = scol;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < destw; ++y)
                            {
                                Color sc = smap.GetPixel(scol, y);
                                Color dc = dmap.GetPixel(destw - 1 - y, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion
                }

                #endregion

                End:
                lock (config.Lock) { if (found) { config.Results.Add(i); } config.CountFiles++; }
                dest.Dispose();
            }

            Threads[1].finish = true;
        }
        private void TH_CMP3()
        {
            Bitmap sour = Threads[2].sour;
            Bitmap dest;
            int sourh = sour.Height, sourw = sour.Width;
            int desth = 0, destw = 0;
            bool found = false;

            for (int i = Threads[2].begin - 1; i < Threads[2].end; ++i)
            {
                #region 初始化变量

                dest = (Bitmap)Image.FromFile(Paths[i] + "\\" + Names[i]);
                desth = dest.Height;
                destw = dest.Width;
                found = false;

                #endregion

                #region 行比较

                if (config.Row != -1)
                {
                    #region FULL SAME NO_TURN

                    if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        if (sourh != desth || sourw != destw) { goto End; }

                        double pace = (double)sourw / config.MinCmpPix;
                        int permitcnterr = config.MinCmpPix / 10;
                        int permiterr = 10;
                        int cnterr = 0;
                        double x = 0;
                        int cnt = 0;
                        int len = config.Grays.Length;

                        for (; cnt < len; x += pace, ++cnt)
                        {
                            Color dc = dest.GetPixel((int)x, config.Row);
                            int sgray = config.Grays[cnt];
                            int dgray = (dc.R + dc.G + dc.B) / 3;

                            int ierr = sgray - dgray;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                    }

                    #endregion

                    #region FULL SAME TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel((int)x, config.Row);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int cmprow = sourh - 1 - config.Row;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(sourw - 1 - (int)x, cmprow);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(config.Row, sourw - 1 - (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int cmprow = sourh - 1 - config.Row;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(cmprow, (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        // 比例相同
                        double h2w = (double)sourh / sourw;
                        int err = (int)(h2w * destw) - desth; if (err < 0) { err = -err; }
                        if (err > 3) { goto End; }

                        // 缩放
                        Bitmap smap, dmap;
                        double rate = 0;
                        if (sourh > desth)
                        {
                            smap = new Bitmap(destw, desth);
                            Graphics g = Graphics.FromImage(smap);
                            g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = sourh / desth;
                        }
                        else { smap = sour; }
                        if (sourh < desth)
                        {
                            dmap = new Bitmap(sourw, sourh);
                            Graphics g = Graphics.FromImage(dmap);
                            g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = desth / sourh;
                        }
                        else { dmap = dest; }

                        desth = dmap.Height;
                        destw = dmap.Width;
                        int cmprow = (int)((double)config.Row / sourh * desth);

                        //List<long> errlist = new List<long>();
                        int permitcnterr = destw / 10 + (int)(rate * 10);
                        int permiterr = 10;
                        int cnterr = 0;

                        for (int x = 0; x < destw; ++x)
                        {
                            Color sc = smap.GetPixel(x, cmprow);
                            Color dc = dmap.GetPixel(x, cmprow);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            //errlist.Add(ierr);
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (sourh > desth) { smap.Dispose(); }
                        if (sourh < desth) { dmap.Dispose(); }
                    }

                    #endregion

                    #region PART SAME TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        double h2w = (double)sourh / sourw;

                        #region 原样

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int cmprow = (int)((double)config.Row / sourh * desth);

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int x = 0; x < destw; ++x)
                            {
                                Color sc = smap.GetPixel(x, cmprow);
                                Color dc = dmap.GetPixel(x, cmprow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int srow = (int)((double)config.Row / sourh * desth);
                            int drow = desth - 1 - srow;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int x = 0; x < destw; ++x)
                            {
                                Color sc = smap.GetPixel(x, srow);
                                Color dc = dmap.GetPixel(destw - 1 - x, drow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int srow = (int)((double)config.Row / sourh * destw);
                            int drow = srow;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(y, srow);
                                Color dc = dmap.GetPixel(drow, desth - 1 - y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int srow = (int)((double)config.Row / sourh * destw);
                            int drow = destw - 1 - srow;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(y, srow);
                                Color dc = dmap.GetPixel(drow, y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion
                }

                #endregion

                #region 列比较

                if (config.Col != -1)
                {
                    #region FULL SAME NO_TURN

                    if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        if (sourh != desth || sourw != destw) { goto End; }

                        double pace = (double)sourh / config.MinCmpPix;
                        int permitcnterr = config.MinCmpPix / 10;
                        int permiterr = 10;
                        int cnterr = 0;
                        double y = 0;
                        int cnt = 0;
                        int len = config.Grays.Length;

                        for (; cnt < len; y += pace, ++cnt)
                        {
                            Color dc = dest.GetPixel(config.Col, (int)y);
                            int sgray = config.Grays[cnt];
                            int dgray = (dc.R + dc.G + dc.B) / 3;

                            int ierr = sgray - dgray;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                    }

                    #endregion

                    #region FULL SAME TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(config.Col, (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int cmpcol = sourw - config.Col - 1;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(cmpcol, sourh - 1 - (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int cmpcol = sourw - config.Col - 1;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel((int)x, cmpcol);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(sourh - 1 - (int)x, config.Col);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        // 比例相同
                        double h2w = (double)sourh / sourw;
                        int err = (int)(h2w * destw) - desth; if (err < 0) { err = -err; }
                        if (err > 3) { goto End; }

                        // 缩放
                        Bitmap smap, dmap;
                        double rate = 0;
                        if (sourh > desth)
                        {
                            smap = new Bitmap(destw, desth);
                            Graphics g = Graphics.FromImage(smap);
                            g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = sourh / desth;
                        }
                        else { smap = sour; }
                        if (sourh < desth)
                        {
                            dmap = new Bitmap(sourw, sourh);
                            Graphics g = Graphics.FromImage(dmap);
                            g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = desth / sourh;
                        }
                        else { dmap = dest; }

                        desth = dmap.Height;
                        destw = dmap.Width;
                        int cmpcol = (int)((double)config.Col / sourw * destw);

                        //List<long> errlist = new List<long>();
                        int permitcnterr = desth / 10 + (int)(rate * 10);
                        int permiterr = 10;
                        int cnterr = 0;

                        for (int y = 0; y < desth; ++y)
                        {
                            Color sc = smap.GetPixel(cmpcol, y);
                            Color dc = dmap.GetPixel(cmpcol, y);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            //errlist.Add(ierr);
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (sourh > desth) { smap.Dispose(); }
                        if (sourh < desth) { dmap.Dispose(); }
                    }

                    #endregion

                    #region PART SAME TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        double h2w = (double)sourh / sourw;

                        #region 原样

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int cmpcol = (int)((double)config.Col / sourw * destw);

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(cmpcol, y);
                                Color dc = dmap.GetPixel(cmpcol, y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int scol = (int)((double)config.Col / sourw * destw);
                            int dcol = destw - 1 - scol;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(scol, y);
                                Color dc = dmap.GetPixel(dcol, desth - 1 - y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int scol = (int)((double)config.Col / sourw * desth);
                            int dcol = desth - 1 - scol;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < destw; ++y)
                            {
                                Color sc = smap.GetPixel(scol, y);
                                Color dc = dmap.GetPixel(y, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int scol = (int)((double)config.Col / sourw * desth);
                            int dcol = scol;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < destw; ++y)
                            {
                                Color sc = smap.GetPixel(scol, y);
                                Color dc = dmap.GetPixel(destw - 1 - y, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion
                }

                #endregion

                End:
                lock (config.Lock) { if (found) { config.Results.Add(i); } config.CountFiles++; }
                dest.Dispose();
            }

            Threads[2].finish = true;
        }
        private void TH_CMP4()
        {
            Bitmap sour = Threads[3].sour;
            Bitmap dest;
            int sourh = sour.Height, sourw = sour.Width;
            int desth = 0, destw = 0;
            bool found = false;

            for (int i = Threads[3].begin - 1; i < Threads[3].end; ++i)
            {
                #region 初始化变量

                dest = (Bitmap)Image.FromFile(Paths[i] + "\\" + Names[i]);
                desth = dest.Height;
                destw = dest.Width;
                found = false;

                #endregion

                #region 行比较

                if (config.Row != -1)
                {
                    #region FULL SAME NO_TURN

                    if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        if (sourh != desth || sourw != destw) { goto End; }

                        double pace = (double)sourw / config.MinCmpPix;
                        int permitcnterr = config.MinCmpPix / 10;
                        int permiterr = 10;
                        int cnterr = 0;
                        double x = 0;
                        int cnt = 0;
                        int len = config.Grays.Length;

                        for (; cnt < len; x += pace, ++cnt)
                        {
                            Color dc = dest.GetPixel((int)x, config.Row);
                            int sgray = config.Grays[cnt];
                            int dgray = (dc.R + dc.G + dc.B) / 3;

                            int ierr = sgray - dgray;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                    }

                    #endregion

                    #region FULL SAME TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel((int)x, config.Row);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int cmprow = sourh - 1 - config.Row;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(sourw - 1 - (int)x, cmprow);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(config.Row, sourw - 1 - (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourw / config.MinCmpPix;
                            int cmprow = sourh - 1 - config.Row;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(cmprow, (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        // 比例相同
                        double h2w = (double)sourh / sourw;
                        int err = (int)(h2w * destw) - desth; if (err < 0) { err = -err; }
                        if (err > 3) { goto End; }

                        // 缩放
                        Bitmap smap, dmap;
                        double rate = 0;
                        if (sourh > desth)
                        {
                            smap = new Bitmap(destw, desth);
                            Graphics g = Graphics.FromImage(smap);
                            g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = sourh / desth;
                        }
                        else { smap = sour; }
                        if (sourh < desth)
                        {
                            dmap = new Bitmap(sourw, sourh);
                            Graphics g = Graphics.FromImage(dmap);
                            g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = desth / sourh;
                        }
                        else { dmap = dest; }

                        desth = dmap.Height;
                        destw = dmap.Width;
                        int cmprow = (int)((double)config.Row / sourh * desth);

                        //List<long> errlist = new List<long>();
                        int permitcnterr = destw / 10 + (int)(rate * 10);
                        int permiterr = 10;
                        int cnterr = 0;

                        for (int x = 0; x < destw; ++x)
                        {
                            Color sc = smap.GetPixel(x, cmprow);
                            Color dc = dmap.GetPixel(x, cmprow);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            //errlist.Add(ierr);
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (sourh > desth) { smap.Dispose(); }
                        if (sourh < desth) { dmap.Dispose(); }
                    }

                    #endregion

                    #region PART SAME TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        double h2w = (double)sourh / sourw;

                        #region 原样

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int cmprow = (int)((double)config.Row / sourh * desth);

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int x = 0; x < destw; ++x)
                            {
                                Color sc = smap.GetPixel(x, cmprow);
                                Color dc = dmap.GetPixel(x, cmprow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int srow = (int)((double)config.Row / sourh * desth);
                            int drow = desth - 1 - srow;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int x = 0; x < destw; ++x)
                            {
                                Color sc = smap.GetPixel(x, srow);
                                Color dc = dmap.GetPixel(destw - 1 - x, drow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int srow = (int)((double)config.Row / sourh * destw);
                            int drow = srow;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(y, srow);
                                Color dc = dmap.GetPixel(drow, desth - 1 - y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int srow = (int)((double)config.Row / sourh * destw);
                            int drow = destw - 1 - srow;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(y, srow);
                                Color dc = dmap.GetPixel(drow, y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion
                }

                #endregion

                #region 列比较

                if (config.Col != -1)
                {
                    #region FULL SAME NO_TURN

                    if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        if (sourh != desth || sourw != destw) { goto End; }

                        double pace = (double)sourh / config.MinCmpPix;
                        int permitcnterr = config.MinCmpPix / 10;
                        int permiterr = 10;
                        int cnterr = 0;
                        double y = 0;
                        int cnt = 0;
                        int len = config.Grays.Length;

                        for (; cnt < len; y += pace, ++cnt)
                        {
                            Color dc = dest.GetPixel(config.Col, (int)y);
                            int sgray = config.Grays[cnt];
                            int dgray = (dc.R + dc.G + dc.B) / 3;

                            int ierr = sgray - dgray;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                    }

                    #endregion

                    #region FULL SAME TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(config.Col, (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int cmpcol = sourw - config.Col - 1;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double y = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; y += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(cmpcol, sourh - 1 - (int)y);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int cmpcol = sourw - config.Col - 1;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel((int)x, cmpcol);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = (double)sourh / config.MinCmpPix;
                            int permitcnterr = config.MinCmpPix / 10;
                            int permiterr = 10;
                            int cnterr = 0;
                            double x = 0;
                            int cnt = 0;
                            int len = config.Grays.Length;

                            for (; cnt < len; x += pace, ++cnt)
                            {
                                Color dc = dest.GetPixel(sourh - 1 - (int)x, config.Col);
                                int sgray = config.Grays[cnt];
                                int dgray = (dc.R + dc.G + dc.B) / 3;

                                int ierr = sgray - dgray;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    else if (((config.Mode & MODE.FULL) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {
                        // 比例相同
                        double h2w = (double)sourh / sourw;
                        int err = (int)(h2w * destw) - desth; if (err < 0) { err = -err; }
                        if (err > 3) { goto End; }

                        // 缩放
                        Bitmap smap, dmap;
                        double rate = 0;
                        if (sourh > desth)
                        {
                            smap = new Bitmap(destw, desth);
                            Graphics g = Graphics.FromImage(smap);
                            g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = sourh / desth;
                        }
                        else { smap = sour; }
                        if (sourh < desth)
                        {
                            dmap = new Bitmap(sourw, sourh);
                            Graphics g = Graphics.FromImage(dmap);
                            g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                            g.Dispose();

                            rate = desth / sourh;
                        }
                        else { dmap = dest; }

                        desth = dmap.Height;
                        destw = dmap.Width;
                        int cmpcol = (int)((double)config.Col / sourw * destw);

                        //List<long> errlist = new List<long>();
                        int permitcnterr = desth / 10 + (int)(rate * 10);
                        int permiterr = 10;
                        int cnterr = 0;

                        for (int y = 0; y < desth; ++y)
                        {
                            Color sc = smap.GetPixel(cmpcol, y);
                            Color dc = dmap.GetPixel(cmpcol, y);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            //errlist.Add(ierr);
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (sourh > desth) { smap.Dispose(); }
                        if (sourh < desth) { dmap.Dispose(); }
                    }

                    #endregion

                    #region PART SAME TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.SAME) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {
                        double h2w = (double)sourh / sourw;

                        #region 原样

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int cmpcol = (int)((double)config.Col / sourw * destw);

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(cmpcol, y);
                                Color dc = dmap.GetPixel(cmpcol, y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * destw - desth < 3 && desth - h2w * destw < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > desth)
                            {
                                smap = new Bitmap(destw, desth);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, destw, desth), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / desth;
                            }
                            else { smap = sour; }
                            if (sourh < desth)
                            {
                                dmap = new Bitmap(sourw, sourh);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourw, sourh), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourh;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int scol = (int)((double)config.Col / sourw * destw);
                            int dcol = destw - 1 - scol;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = desth / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < desth; ++y)
                            {
                                Color sc = smap.GetPixel(scol, y);
                                Color dc = dmap.GetPixel(dcol, desth - 1 - y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > desth) { smap.Dispose(); }
                            if (sourh < desth) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int scol = (int)((double)config.Col / sourw * desth);
                            int dcol = desth - 1 - scol;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < destw; ++y)
                            {
                                Color sc = smap.GetPixel(scol, y);
                                Color dc = dmap.GetPixel(y, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (h2w * desth - destw < 3 && destw - h2w * desth < 3)
                        {
                            Bitmap smap, dmap;
                            double rate = 0;
                            if (sourh > destw)
                            {
                                smap = new Bitmap(desth, destw);
                                Graphics g = Graphics.FromImage(smap);
                                g.DrawImage(sour, new Rectangle(0, 0, desth, destw), new Rectangle(0, 0, sourw, sourh), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = sourh / destw;
                            }
                            else { smap = sour; }
                            if (sourh < destw)
                            {
                                dmap = new Bitmap(sourh, sourw);
                                Graphics g = Graphics.FromImage(dmap);
                                g.DrawImage(dest, new Rectangle(0, 0, sourh, sourw), new Rectangle(0, 0, destw, desth), GraphicsUnit.Pixel);
                                g.Dispose();

                                rate = desth / sourw;
                            }
                            else { dmap = dest; }

                            desth = dmap.Height;
                            destw = dmap.Width;
                            int scol = (int)((double)config.Col / sourw * desth);
                            int dcol = scol;

                            //List<long> errlist = new List<long>();
                            int permitcnterr = destw / 10 + (int)(rate * 10);
                            int permiterr = 10;
                            int cnterr = 0;

                            for (int y = 0; y < destw; ++y)
                            {
                                Color sc = smap.GetPixel(scol, y);
                                Color dc = dmap.GetPixel(destw - 1 - y, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > destw) { smap.Dispose(); }
                            if (sourh < destw) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr; if (found) { goto End; }
                        }

                        #endregion
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) == 0))
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    else if (((config.Mode & MODE.PART) != 0) && ((config.Mode & MODE.LIKE) != 0) && ((config.Mode & MODE.TURN) != 0))
                    {

                    }

                    #endregion
                }

                #endregion

                End:
                lock (config.Lock) { if (found) { config.Results.Add(i); } config.CountFiles++; }
                dest.Dispose();
            }

            Threads[3].finish = true;
        }
    }
}
