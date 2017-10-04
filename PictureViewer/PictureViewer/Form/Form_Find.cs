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
        public static bool IsFinish;

        /////////////////////////////////////////////////////////// private attribute //////////////////////////////////////////////////

        private System.Timers.Timer Timer = new System.Timers.Timer(100);

        private static List<string> Paths = new List<string>();
        private static List<string> Names = new List<string>();
        private static CONFIG config;
        private static THREAD[] Threads = new THREAD[4];
    
        private struct THREAD
        {
            public Thread thread;
            public bool finish;
            public int begin;
            public int end;
            public List<int> result;
        }
        private struct CONFIG
        {
            /// <summary>
            /// 线程锁
            /// </summary>
            public object Lock;
            /// <summary>
            /// 是否比较行信息
            /// </summary>
            public bool IsRow;
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
            /// 最小比较元素个数
            /// </summary>
            public int MinCmpPix;
            /// <summary>
            /// 待比较的灰度值序列
            /// </summary>
            public int[] Grays;
            /// <summary>
            /// 比对结果
            /// </summary>
            public List<int> Results;
            /// <summary>
            /// 当前的显示结果
            /// </summary>
            public List<int> Current;
            /// <summary>
            /// 计时起点
            /// </summary>
            public long TimeBG;
            /// <summary>
            /// 计时终点
            /// </summary>
            public long TimeED;
            /// <summary>
            /// 计数器
            /// </summary>
            public long Time;
        }

        /////////////////////////////////////////////////////////// public method //////////////////////////////////////////////////

        public Form_Find(Image pic, string fullpath = "")
        {
            InitializeComponent();

            SourPic = CopyPicture((Bitmap)pic, 1);
            config.Sour = fullpath;
        }
        
        /////////////////////////////////////////////////////////// private method //////////////////////////////////////////////////

        private void Form_Load(object sender, EventArgs e)
        {
            config.Lock = new object();

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
        }
        private void Form_Updata(object source, System.Timers.ElapsedEventArgs e)
        {
            this.BeginInvoke((EventHandler)delegate{

                config.Time++;

                bool prevState = IsFinish;
                IsFinish = Threads[0].finish && Threads[1].finish && Threads[2].finish && Threads[3].finish;

                if (!IsFinish) { config.TimeED = config.Time; }
                double usedTime = (double)(config.TimeED - config.TimeBG) / 10;
                string usedtime = usedTime.ToString();
                if (usedtime.Length < 2 || usedtime[usedtime.Length - 2] != '.') { usedtime += ".0"; }
                this.Text = IsFinish ?
                    "[Total]: " + Names.Count.ToString() + " Files [State]: Finished [Used Time]: " + usedtime + " s" :
                    "[Total]: " + Names.Count.ToString() + " Files [State]: Finding... [Used Time]: " + usedtime + " s";

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

            config.MinCmpPix = MinCmpPixes;
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
            Threads[0].finish = false;
            Threads[0].begin = 1;
            Threads[0].end = (int)ed;
            Threads[0].result = new List<int>();

            bg = ed; ed = bg + pace;
            Threads[1].thread = new Thread(TH_CMP2);
            Threads[1].finish = false;
            Threads[1].begin = (int)bg + 1;
            Threads[1].end = (int)ed;
            Threads[1].result = new List<int>();

            bg = ed; ed = bg + pace;
            Threads[2].thread = new Thread(TH_CMP3);
            Threads[2].finish = false;
            Threads[2].begin = (int)bg + 1;
            Threads[2].end = (int)ed;
            Threads[2].result = new List<int>();

            bg = ed; ed = bg + pace;
            Threads[3].thread = new Thread(TH_CMP4);
            Threads[3].finish = false;
            Threads[3].begin = (int)bg + 1;
            Threads[3].end = (int)ed;
            Threads[3].result = new List<int>();

            config.TimeBG = config.Time;

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

            config.Index = 0;
            config.MinCmpPix = 50;
            config.Results = new List<int>();
            config.Current = new List<int>();
            config.TimeBG = 0;
            config.TimeED = 0;
            config.Time = 0;

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
            config.IsRow = W > H;

            if (config.IsRow)
            {
                config.Grays = new int[W];
                
                int row1 = H / 2;
                int row2 = H / 4;
                int row3 = H * 3 / 4;

                int d1 = GetDiffRow(row1);
                int d2 = GetDiffRow(row2);
                int d3 = GetDiffRow(row3);

                int srow = 0;
                if (d1 >= d2 && d1 >= d3) { srow = row1; }
                if (d2 >= d1 && d2 >= d3) { srow = row2; }
                if (d3 >= d2 && d3 >= d1) { srow = row3; }

                for (int i = 0; i < W; i++) { config.Grays[i] = ToGray(SourPic.GetPixel(i, srow)); }
            }
            else
            {
                config.Grays = new int[H];

                int col1 = W / 2;
                int col2 = W / 4;
                int col3 = W * 3 / 4;

                int d1 = GetDiffCol(col1);
                int d2 = GetDiffCol(col2);
                int d3 = GetDiffCol(col3);

                int scol = 0;
                if (d1 >= d2 && d1 >= d3) { scol = col1; }
                if (d2 >= d1 && d2 >= d3) { scol = col2; }
                if (d3 >= d2 && d3 >= d1) { scol = col3; }

                for (int i = 0; i < H; i++) { config.Grays[i] = ToGray(SourPic.GetPixel(scol,i)); }
            }

            if (config.Grays.Length > config.MinCmpPix)
            {
                double pace = (double)config.Grays.Length / config.MinCmpPix;
                List<int> grays = new List<int>();

                for (double i = 0; i < config.Grays.Length; i += pace)
                {
                    grays.Add(config.Grays[(int)i]);
                }

                config.Grays = grays.ToArray();
            }

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
            g.Dispose();

            return dest;
        }
        private int GetDiffRow(int row)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < SourPic.Width; i++) { indexes.Add(i); }

            for (int i = 0; i < indexes.Count; i++)
            {
                int c1 = ToGray(SourPic.GetPixel(indexes[i], row));
                for (int j = indexes.Count - 1; j > i; j--)
                {
                    int c2 = ToGray(SourPic.GetPixel(indexes[j], row));
                    if (c1 == c2) { indexes.RemoveAt(j); continue; }
                }
            }

            return indexes.Count;
        }
        private int GetDiffCol(int col)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < SourPic.Height; i++) { indexes.Add(i); }
            
            for (int i = 0; i < indexes.Count; i++)
            {
                int c1 = ToGray(SourPic.GetPixel(col, indexes[i]));
                for (int j = indexes.Count - 1; j > i; j--)
                {
                    int c2 = ToGray(SourPic.GetPixel(col, indexes[j]));
                    if (c1 == c2) { indexes.RemoveAt(j); continue; }
                }
            }

            return indexes.Count;
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
            int bg = Threads[0].begin, ed = Threads[0].end;
            string path;
            string name;
            bool IsSame;
            Image dest;

            for (int i = bg - 1; i < ed; i++)
            {
                path = Paths[i];
                name = Names[i];

                dest = Image.FromFile(path + "\\" + name);

                #region 比较两幅图片

                IsSame = false;

                if (config.IsRow && dest.Width > config.MinCmpPix + 20)
                {
                    for (int j = 10; j < dest.Height - 10; j += 1)
                    {
                        double err = CmpRow((Bitmap)dest, j);
                        if (err < 3) { IsSame = true; break; }
                    }
                }
                if (!config.IsRow && dest.Height > config.MinCmpPix + 20)
                {
                    for (int j = 10; j < dest.Width - 10; j += 1)
                    {
                        double err = CmpCol((Bitmap)dest, j);
                        if (err < 3) { IsSame = true; break; }
                    }
                }

                #endregion

                if (IsSame)
                {
                    Threads[0].result.Add(i);
                    int total = 0;
                    lock (config.Lock) { config.Results.Add(i); total = config.Results.Count; }
                }
                dest.Dispose();
            }

            Threads[0].finish = true;
            //if (Threads[0].finish && Threads[1].finish && Threads[2].finish && Threads[3].finish)
            //{ config.TimeED = config.Time; }
        }
        private void TH_CMP2()
        {
            int bg = Threads[1].begin, ed = Threads[1].end;
            string path;
            string name;
            bool IsSame;
            Image dest;

            for (int i = bg - 1; i < ed; i++)
            {
                path = Paths[i];
                name = Names[i];

                dest = Image.FromFile(path + "\\" + name);

                #region 比较两幅图片

                IsSame = false;

                if (config.IsRow && dest.Width > config.MinCmpPix + 20)
                {
                    for (int j = 10; j < dest.Height - 10; j += 1)
                    {
                        double err = CmpRow((Bitmap)dest, j);
                        if (err < 3) { IsSame = true; break; }
                    }
                }
                if (!config.IsRow && dest.Height > config.MinCmpPix + 20)
                {
                    for (int j = 10; j < dest.Width - 10; j += 1)
                    {
                        double err = CmpCol((Bitmap)dest, j);
                        if (err < 3) { IsSame = true; break; }
                    }
                }

                #endregion

                if (IsSame)
                {
                    Threads[0].result.Add(i);
                    int total = 0;
                    lock (config.Lock) { config.Results.Add(i); total = config.Results.Count; }
                }
                dest.Dispose();
            }

            Threads[1].finish = true;
            //if (Threads[0].finish && Threads[1].finish && Threads[2].finish && Threads[3].finish)
            //{ config.TimeED = config.Time; }
        }
        private void TH_CMP3()
        {
            int bg = Threads[2].begin, ed = Threads[2].end;
            string path;
            string name;
            bool IsSame;
            Image dest;

            for (int i = bg - 1; i < ed; i++)
            {
                path = Paths[i];
                name = Names[i];

                dest = Image.FromFile(path + "\\" + name);

                #region 比较两幅图片

                IsSame = false;

                if (config.IsRow && dest.Width > config.MinCmpPix + 20)
                {
                    for (int j = 10; j < dest.Height - 10; j += 1)
                    {
                        double err = CmpRow((Bitmap)dest, j);
                        if (err < 3) { IsSame = true; break; }
                    }
                }
                if (!config.IsRow && dest.Height > config.MinCmpPix + 20)
                {
                    for (int j = 10; j < dest.Width - 10; j += 1)
                    {
                        double err = CmpCol((Bitmap)dest, j);
                        if (err < 3) { IsSame = true; break; }
                    }
                }

                #endregion

                if (IsSame)
                {
                    Threads[0].result.Add(i);
                    int total = 0;
                    lock (config.Lock) { config.Results.Add(i); total = config.Results.Count; }
                }
                dest.Dispose();
            }

            Threads[2].finish = true;
            //if (Threads[0].finish && Threads[1].finish && Threads[2].finish && Threads[3].finish)
            //{ config.TimeED = config.Time; }
        }
        private void TH_CMP4()
        {
            int bg = Threads[3].begin, ed = Threads[3].end;
            string path;
            string name;
            bool IsSame;
            Image dest;

            for (int i = bg - 1; i < ed; i++)
            {
                path = Paths[i];
                name = Names[i];

                dest = Image.FromFile(path + "\\" + name);

                #region 比较两幅图片

                IsSame = false;

                if (config.IsRow && dest.Width > config.MinCmpPix + 20)
                {
                    for (int j = 10; j < dest.Height - 10; j += 1)
                    {
                        double err = CmpRow((Bitmap)dest, j);
                        if (err < 3) { IsSame = true; break; }
                    }
                }
                if (!config.IsRow && dest.Height > config.MinCmpPix + 20)
                {
                    for (int j = 10; j < dest.Width - 10; j += 1)
                    {
                        double err = CmpCol((Bitmap)dest, j);
                        if (err < 3) { IsSame = true; break; }
                    }
                }

                #endregion

                if (IsSame)
                {
                    Threads[0].result.Add(i);
                    int total = 0;
                    lock (config.Lock) { config.Results.Add(i); total = config.Results.Count; }
                }
                dest.Dispose();
            }

            Threads[3].finish = true;
            //if (Threads[0].finish && Threads[1].finish && Threads[2].finish && Threads[3].finish)
            //{ config.TimeED = config.Time; }
        }

        private double CmpRow(Bitmap pic, int row)
        {
            int[] grays = new int[config.MinCmpPix];
            int len = pic.Width;

            double err = 0;
            double ierr = 0;

            double pace = (double)len / config.MinCmpPix;
            int lastPix = config.Grays.Length - 1;
            int cntBigErr = 0;
            int cnt = 0;
            for (double i = 0; i < len; i += pace, cnt++)
            {
                if (cnt == lastPix) { break; }
                int index = (int)i; if (index >= len) { break; }
                grays[cnt] = ToGray(pic.GetPixel(index, row));

                ierr = Math.Abs(grays[cnt] - config.Grays[cnt]);
                if (ierr > 10) { cntBigErr++; }
                if (cntBigErr > 10) { return double.MaxValue; }
                err += ierr;
            }

            return err / lastPix;
        }
        private double CmpCol(Bitmap pic, int col)
        {
            int[] grays = new int[config.MinCmpPix];
            int len = pic.Height;

            double err = 0;
            double ierr = 0;

            double pace = (double)len / config.MinCmpPix;
            int lastPix = config.Grays.Length - 1;
            int cntBigErr = 0;
            int cnt = 0;
            for (double i = 0; i < len; i += pace, cnt++)
            {
                if (cnt == lastPix) { break; }
                int index = (int)i; if (index >= len) { break; }
                grays[cnt] = ToGray(pic.GetPixel(col, index));

                ierr = Math.Abs(grays[cnt] - config.Grays[cnt]);
                if (ierr > 10) { cntBigErr++; }
                if (cntBigErr > 10) { return double.MaxValue; }
                err += ierr;
            }
            
            return err / lastPix;
        }
    }
}
