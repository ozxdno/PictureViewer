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
            /// 大小必须相同（没有经过缩放）
            /// </summary>
            FULL = 0x0001,
            /// <summary>
            /// 大小可以不同（经过缩放）
            /// </summary>
            PART = 0x0002,
            /// <summary>
            /// 完全相同（两幅图内容完全一样）
            /// </summary>
            SAME = 0x0004,
            /// <summary>
            /// 部分相似（源图为要寻找图的一部分）
            /// </summary>
            LIKE = 0x0008,
            /// <summary>
            /// 经过了旋转和翻转
            /// </summary>
            TURN = 0x0010,

            FULL_SAME_NOTURN = FULL + SAME,
            FULL_SAME_TURN = FULL + SAME + TURN,
            PART_SAME_NOTURN = PART + SAME,
            PART_SAME_TURN = PART + SAME + TURN,
            FULL_LIKE_NOTURN = FULL + LIKE,
            FULL_LIKE_TURN = FULL + LIKE + TURN,
            PART_LIKE_NOTURN = PART + LIKE,
            PART_LIKE_TURN = PART + LIKE + TURN
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
            public bool abort;
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
            /// 相似程度
            /// </summary>
            public int Degree;
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

        /// <summary>
        /// 查找图片
        /// </summary>
        /// <param name="pic">源图</param>
        /// <param name="fullpath">源图所在路径</param>
        /// <param name="mode">查找模式</param>
        public Form_Find(Image pic, string fullpath, MODE mode = MODE.DEFAULT)
        {
            InitializeComponent();

            double rate = 1000.0 / Math.Min(pic.Height, pic.Width);
            if (rate > 1) { rate = 1; }
            SourPic = CopyPicture((Bitmap)pic, rate);
            config.Sour = fullpath == null ? "" : fullpath;
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
            Class.Save.settings.Form_Main_Find_Full = this.fullToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Part = this.partToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Same = this.sameToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Like = this.likeToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Turn = this.turnToolStripMenuItem.Checked;
            Class.Save.settings.Form_Find_Degree = config.Degree;
            Class.Save.settings.Form_Find_Pixes = config.MinCmpPix;

            Stop(); Timer.Close();

            try { SourPic.Dispose(); } catch { }
            try { DestPic.Dispose(); } catch { }
            for (int i = 0; i < Threads.Length; i++) { try { Threads[i].sour.Dispose(); } catch { } }
        }
        private void Form_Updata(object source, System.Timers.ElapsedEventArgs e)
        {
            this.BeginInvoke((EventHandler)delegate{
                
                bool prevState = IsFinish;
                IsFinish = Threads[0].finish && Threads[1].finish && Threads[2].finish && Threads[3].finish;

                if (!IsFinish) { config.CountTime++; }

                if (!IsFinish) { config.TimeED = config.CountTime; }
                double usedTime = (double)(config.TimeED - config.TimeBG) / 10;
                string usedtime = usedTime.ToString();
                if (usedtime.Length < 2 || usedtime[usedtime.Length - 2] != '.') { usedtime += ".0"; }
                int findfiles = 0;
                int found = 0;
                lock (config.Lock) { findfiles = config.CountFiles; found = config.Results.Count; }
                string findstr = "[Find]: " + findfiles.ToString() + "/" + Names.Count.ToString() + " files";
                string resultstr = "[Result]: " + found.ToString();
                string statestr = IsFinish ? "[State]: Finished" : "[State]: Finding...";
                string timestr = "[Used Time]: " + usedtime + " s";
                this.Text = findstr + " " + timestr + " " + resultstr;

                if (!prevState || !IsFinish) { ShowList(); }
                
                if (IsFinish && prevState != IsFinish)
                {
                    if (findfiles == Names.Count) { this.startToolStripMenuItem.Text = "Start"; }
                    if (this.listBox1.SelectedIndex == -1)
                    {
                        if (config.Current.Count == 0) { ShowDestPic(); }
                        else { this.listBox1.SelectedIndex = 0; }
                    }
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
                this.label1.Visible = false;// xbg <= ptX && ptX <= xed && ybg <= ptY && ptY <= yed && config.Current.Count > 0;

                xbg = this.label2.Location.X + 10; xed = xbg + this.label2.Width;
                ybg = this.label2.Location.Y + 30; yed = ybg + this.label2.Height;
                this.label2.Visible = false;// xbg <= ptX && ptX <= xed && ybg <= ptY && ptY <= yed && config.Current.Count > 0;

                //if (this.label1.Visible || this.label2.Visible || this.label4.Visible || this.label5.Visible)
                //{
                //}
                //else {  }
            });
        }
        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27) { this.Close(); return; }
        }
        
        private void Previous(object sender, EventArgs e)
        {
            int index = this.listBox1.SelectedIndex - 1;
            if (index < 0) { index = this.listBox1.Items.Count - 1; }
            this.listBox1.SelectedIndex = index;
        }
        private void Next(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count == 0) { return; }
            int index = this.listBox1.SelectedIndex + 1;
            if (index >= this.listBox1.Items.Count) { index = 0; }
            this.listBox1.SelectedIndex = index;
        }
        private void ListClicked(object sender, EventArgs e)
        {
            int index = this.listBox1.SelectedIndex;
            //if (index == -1) { return; }
            config.Index = index;
            ShowDestPic();
        }
        private void DoubleClickedToSwitch(object sender, EventArgs e)
        {
            this.toolTip1.Hide(this.listBox1);

            if (config.Current.Count == 0) { MessageBox.Show("文件不存在，无法转到！", "提示"); return; }
            int index = config.Index;
            if (index < 0 || index >= config.Current.Count) { MessageBox.Show("文件不存在，无法转到！", "提示"); return; }

            Stop();

            string path = Paths[config.Current[index]];
            string name = Names[config.Current[index]];

            for (int i = 0; i < FileOperate.RootFiles.Count; i++)
            {
                string ipath = FileOperate.RootFiles[i].Path;
                for (int j = 0; j < FileOperate.RootFiles[i].Name.Count; j++)
                {
                    string jname = FileOperate.RootFiles[i].Name[j];
                    int type = FileOperate.getFileType(FileOperate.getExtension(jname));

                    if (type == 1)
                    {
                        string newpath = ipath + "\\" + jname;
                        List<string> subnames = FileOperate.getSubFiles(newpath);

                        for (int k = 0; k < subnames.Count; k++)
                        {
                            jname = subnames[k];
                            if (newpath == path && jname == name)
                            {
                                if (DialogResult.Cancel == MessageBox.Show("转到当前选中图片？", "确认", MessageBoxButtons.OKCancel))
                                { return; }

                                Form_Main.config.FolderIndex = i;
                                Form_Main.config.FileIndex = j;
                                Form_Main.config.SubIndex = k;

                                this.Close(); return;
                            }
                        }
                    }
                    else
                    {
                        if (ipath == path && jname == name)
                        {
                            if (DialogResult.Cancel == MessageBox.Show("转到当前选中图片？", "确认", MessageBoxButtons.OKCancel))
                            { return; }

                            Form_Main.config.FolderIndex = i;
                            Form_Main.config.FileIndex = j;
                            Form_Main.config.SubIndex = 0;

                            this.Close(); return;
                        }
                    }
                }
            }

            MessageBox.Show("未找到该文件！", "提示");
        }

        private void RightMenu_Start(object sender, EventArgs e)
        {
            if (this.startToolStripMenuItem.Text == "Start") { Start(); return; }
            if (this.startToolStripMenuItem.Text == "Continue") { Continue(); return; }
            if (this.startToolStripMenuItem.Text == "Stop") { Stop(); return; }
            this.startToolStripMenuItem.Text = "Start";
        }
        private void RightMenu_Export(object sender, EventArgs e)
        {
            if (!IsFinish) { MessageBox.Show("正在搜索，请勿操作！", "提示"); return; }
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

            try { DestPic.Dispose(); } catch {  }
            try { File.Move(sour, dest); } catch { MessageBox.Show("移动失败！", "提示"); return; }
            RemoveCurrent(config.Index);
        }
        private void RightMenu_Export2(object sender, EventArgs e)
        {
            if (!IsFinish) { MessageBox.Show("正在搜索，请勿操作！", "提示"); return; }
            if (config.Current.Count == 0) { MessageBox.Show("文件不存在！", "提示"); return; }

            string sourpath;
            string sourname;
            string sour;
            string destpath = Form_Main.config.ExportFolder; if (!Directory.Exists(destpath)) { destpath = FileOperate.getExePath(); }
            string destname;
            string dest;

            if (DialogResult.Cancel == MessageBox.Show("把所有重复文件导出到：\n" + destpath + "？", "确认导出", MessageBoxButtons.OKCancel))
            { return; }

            List<string> fails = new List<string>();
            List<string> reasons = new List<string>();
            
            for (int i = config.Current.Count - 1; i >= 0; i--)
            {
                int index = config.Current[i];
                sourpath = Paths[index];
                sourname = Names[index];
                destname = sourname;

                sour = sourpath + "\\" + sourname;
                dest = destpath + "\\" + destname;
                if (!File.Exists(sour)) { fails.Add(sour); reasons.Add("源文件不存在！"); continue; }
                if (File.Exists(dest)) { fails.Add(sour); reasons.Add("目标文件已存在！"); continue; }

                try { DestPic.Dispose(); } catch { }
                try { File.Move(sour, dest); } catch { fails.Add(sour); reasons.Add("移动失败！"); continue; }
                RemoveCurrent(i);
            }
            
            if (fails.Count == 0) { return; }
            string msg = "错误列表：";
            for (int i = 0; i < fails.Count; i++) { msg += "\n[" + i.ToString() + "] " + reasons[i] + " " + fails[i]; }
            MessageBox.Show(msg, "错误");
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
            //if (!IsFinish) { MessageBox.Show("正在搜索，请勿操作！", "提示"); return; }

            Form_Input input = new Form_Input(config.MinCmpPix.ToString());
            input.Location = MousePosition;
            input.ShowDialog();

            int MinCmpPixes = 0;
            try { MinCmpPixes = int.Parse(input.Input); } catch { MessageBox.Show("必须输入正整数！", "提示"); return; }
            if (MinCmpPixes < 0) { MessageBox.Show("必须输入正整数！", "提示"); return; }

            //if (!IsFinish) { MessageBox.Show("正在搜索，请勿操作！", "提示"); return; }
            if (config.MinCmpPix == MinCmpPixes) { return; }
            if (IsFinish) { config.MinCmpPix = MinCmpPixes; return; }

            Stop();
            config.MinCmpPix = MinCmpPixes;
            Continue();
        }
        private void RightMenu_Switch(object sender, EventArgs e)
        {
            //if (!IsFinish) { MessageBox.Show("正在搜索，请勿操作！", "提示"); return; }

            if (config.Current.Count == 0) { MessageBox.Show("文件不存在，无法转到！", "提示"); return; }
            int index = config.Index;
            if (index < 0 || index >= config.Current.Count) { MessageBox.Show("文件不存在，无法转到！", "提示"); return; }

            Stop();

            string path = Paths[config.Current[index]];
            string name = Names[config.Current[index]];

            for (int i = 0; i < FileOperate.RootFiles.Count; i++)
            {
                string ipath = FileOperate.RootFiles[i].Path;
                for (int j = 0; j < FileOperate.RootFiles[i].Name.Count; j++)
                {
                    string jname = FileOperate.RootFiles[i].Name[j];
                    int type = FileOperate.getFileType(FileOperate.getExtension(jname));

                    if (type == 1)
                    {
                        string newpath = ipath + "\\" + jname;
                        List<string> subnames = FileOperate.getSubFiles(newpath);

                        for (int k = 0; k < subnames.Count; k++)
                        {
                            jname = subnames[k];
                            if (newpath == path && jname == name)
                            {
                                Form_Main.config.FolderIndex = i;
                                Form_Main.config.FileIndex = j;
                                Form_Main.config.SubIndex = k;

                                this.Close(); return;
                            }
                        }
                    }
                    else
                    {
                        if (ipath == path && jname == name)
                        {
                            Form_Main.config.FolderIndex = i;
                            Form_Main.config.FileIndex = j;
                            Form_Main.config.SubIndex = 0;

                            this.Close(); return;
                        }
                    }
                }
            }

            MessageBox.Show("未找到该文件！", "提示");
        }
        private void RightMenu_Degree(object sender, EventArgs e)
        {
            //if (!IsFinish) { MessageBox.Show("正在搜索，请勿操作！", "提示"); return; }

            Form_Input input = new Form_Input(config.Degree.ToString());
            input.Location = MousePosition;
            input.ShowDialog();

            double Degree = 0;
            try { Degree = double.Parse(input.Input); } catch { MessageBox.Show("必须输入 0-100 之间的数！", "提示"); return; }
            if (Degree < 0) { MessageBox.Show("必须输入 0-100 之间的数！", "提示"); return; }
            if (Degree > 100) { MessageBox.Show("必须输入 0-100 之间的数！", "提示"); return; }

            if (!IsFinish) { MessageBox.Show("正在搜索，请勿操作！", "提示"); return; }
            if (config.Degree == (int)Degree) { return; }
            if (IsFinish) { config.Degree = (int)Degree; return; }

            Stop();
            config.Degree = (int)Degree;
            Continue();
        }
        private void RightMenu_Restart(object sender, EventArgs e)
        {
            Stop();
            Start();
        }
        private void RightMenu_Mode(object sender, EventArgs e)
        {

        }
        private void RightMenu_Mode_Full(object sender, EventArgs e)
        {
            this.fullToolStripMenuItem.Checked = !this.fullToolStripMenuItem.Checked;
            this.partToolStripMenuItem.Checked = !this.fullToolStripMenuItem.Checked;

            SetMode();
        }
        private void RightMenu_Mode_Part(object sender, EventArgs e)
        {
            this.partToolStripMenuItem.Checked = !this.partToolStripMenuItem.Checked;
            this.fullToolStripMenuItem.Checked = !this.partToolStripMenuItem.Checked;

            SetMode();
        }
        private void RightMenu_Mode_Same(object sender, EventArgs e)
        {
            this.sameToolStripMenuItem.Checked = !this.sameToolStripMenuItem.Checked;
            this.likeToolStripMenuItem.Checked = !this.sameToolStripMenuItem.Checked;

            SetMode();
        }
        private void RightMenu_Mode_Like(object sender, EventArgs e)
        {
            this.likeToolStripMenuItem.Checked = !this.likeToolStripMenuItem.Checked;
            this.sameToolStripMenuItem.Checked = !this.likeToolStripMenuItem.Checked;

            SetMode();
        }
        private void RightMenu_Mode_Turn(object sender, EventArgs e)
        {
            this.turnToolStripMenuItem.Checked = !this.turnToolStripMenuItem.Checked;

            SetMode();
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

            string size = "?";
            try { FileInfo file = new FileInfo(config.Sour); size = (file.Length / 1000).ToString(); } catch { }
            
            this.toolTip2.ToolTipTitle = path;
            this.toolTip2.SetToolTip(this.pictureBox2, "[" + size + " KB] " + name);
        }
        private void ShowDestPic()
        {
            int Index = 0;
            if (config.Index < 0 || config.Index >= config.Current.Count) { Index = -1; }
            else { Index = config.Current[config.Index]; }
            
            if (Index < 0 || Index >= Paths.Count)
            {
                config.Path = "Not Exist "; config.Name = " Unselect File";
                if (DestPic != null) { DestPic.Dispose(); }
                string unkfile = FileOperate.getExePath() + "\\unk.tip";
                if (File.Exists(unkfile)) { DestPic = (Bitmap)Image.FromFile(unkfile); }
                else
                {
                    this.pictureBox1.BackgroundImage = null;
                    this.toolTip1.ToolTipTitle = config.Path;
                    this.toolTip1.SetToolTip(this.listBox1, config.Name); return;
                }

                int hBox = this.pictureBox1.Height;
                int wBox = this.pictureBox1.Width;
                int hPic = DestPic.Height;
                int wPic = DestPic.Width;

                double rate = Math.Min((double)hBox / hPic, (double)wBox / wPic);
                Bitmap dest = CopyPicture(DestPic, rate);
                this.pictureBox1.BackgroundImage = dest;

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
            config.CountFiles = 0;

            FillPathsNames();

            while (Threads[0].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[0].thread.Abort(); }
            while (Threads[1].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[1].thread.Abort(); }
            while (Threads[2].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[2].thread.Abort(); }
            while (Threads[3].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) { Threads[3].thread.Abort(); }

            double pace = (double)Paths.Count / Threads.Length;
            double bg = 0, ed = 0;
            //double rate = 1000.0 / Math.Min(SourPic.Height, SourPic.Width);
            //if (rate > 1) { rate = 1; }
            double rate = 1;

            bg = ed; ed = bg + pace;
            Threads[0].thread = new Thread(TH_CMP1);
            Threads[0].sour = CopyPicture(SourPic, rate);
            Threads[0].abort = false;
            Threads[0].finish = false;
            Threads[0].begin = 1;
            Threads[0].end = (int)ed;
            Threads[0].result = new List<int>();

            bg = ed; ed = bg + pace;
            Threads[1].thread = new Thread(TH_CMP2);
            Threads[1].sour = CopyPicture(SourPic, rate);
            Threads[1].abort = false;
            Threads[1].finish = false;
            Threads[1].begin = (int)bg + 1;
            Threads[1].end = (int)ed;
            Threads[1].result = new List<int>();

            bg = ed; ed = bg + pace;
            Threads[2].thread = new Thread(TH_CMP3);
            Threads[2].sour = CopyPicture(SourPic, rate);
            Threads[2].abort = false;
            Threads[2].finish = false;
            Threads[2].begin = (int)bg + 1;
            Threads[2].end = (int)ed;
            Threads[2].result = new List<int>();

            bg = ed; ed = bg + pace;
            Threads[3].thread = new Thread(TH_CMP4);
            Threads[3].sour = CopyPicture(SourPic, rate);
            Threads[3].abort = false;
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

            //PicMatch pm = new PicMatch();
            //pm.Picture = CopyPicture(SourPic, rate);
            //pm.Mode = PicMatch.MODE.PART_LIKE_NOTURN;
            //pm.IndexCmp = 0;
            //pm.Degree = config.Degree;
            //pm.Pixes = config.MinCmpPix;
            //pm.Row = config.Row;
            //pm.Col = config.Col;
            //pm.SetGrays();
            //pm.CmpFiles.Clear();
            //for (int i = 0; i < Names.Count; i++) { pm.CmpFiles.Add(Paths[i] + "\\" + Names[i]); }
            //Thread t = new Thread(pm.Start);
            //t.Start();

            this.startToolStripMenuItem.Text = "Stop";
        }
        private void Stop()
        {
            lock (config.Lock) { for (int i = 0; i < Threads.Length; i++) { Threads[i].abort = true; } }

            while (Threads[0].thread != null && Threads[0].thread.ThreadState == ThreadState.Running) ;
            while (Threads[1].thread != null && Threads[1].thread.ThreadState == ThreadState.Running) ;
            while (Threads[2].thread != null && Threads[2].thread.ThreadState == ThreadState.Running) ;
            while (Threads[3].thread != null && Threads[3].thread.ThreadState == ThreadState.Running) ;

            this.startToolStripMenuItem.Text = "Continue";
        }
        private void Continue()
        {
            for (int i = 0; i < Threads.Length; i++) { Threads[i].abort = false; Threads[i].finish = false; }

            Threads[0].thread = new Thread(TH_CMP1);
            Threads[1].thread = new Thread(TH_CMP2);
            Threads[2].thread = new Thread(TH_CMP3);
            Threads[3].thread = new Thread(TH_CMP4);

            Threads[0].thread.Start();
            Threads[1].thread.Start();
            Threads[2].thread.Start();
            Threads[3].thread.Start();

            this.startToolStripMenuItem.Text = "Stop";
        }

        private void InitializeForm()
        {
            #region 填充 config

            config.Current = new List<int>();
            config.Index = 0;
            config.Path = "Not Exist";
            config.Name = "Unselect File";

            config.Lock = new object();
            if (config.Mode == MODE.DEFAULT) { config.Mode = MODE.FULL_SAME_NOTURN; }
            config.Degree = Class.Load.settings.Form_Find_Degree;
            config.MinCmpPix = Class.Load.settings.Form_Find_Pixes;
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

            #region 比较方式
            
            GetBestRow();
            GetBestCol();

            #endregion

            #region 初始化模式

            if (((ushort)config.Mode & (ushort)MODE.FULL) > 0) { this.fullToolStripMenuItem.Checked = true; }
            if (((ushort)config.Mode & (ushort)MODE.PART) > 0) { this.partToolStripMenuItem.Checked = true; }
            if (((ushort)config.Mode & (ushort)MODE.SAME) > 0) { this.sameToolStripMenuItem.Checked = true; }
            if (((ushort)config.Mode & (ushort)MODE.LIKE) > 0) { this.likeToolStripMenuItem.Checked = true; }
            if (((ushort)config.Mode & (ushort)MODE.TURN) > 0) { this.turnToolStripMenuItem.Checked = true; }

            #endregion
        }
        private void FillPathsNames()
        {
            Paths.Clear(); Names.Clear();

            #region 挑选出符合要求的类型

            for (int i = 0; i < FileOperate.RootFiles.Count; i++)
            {
                string root = FileOperate.RootFiles[i].Path;
                for (int j = 0; j < FileOperate.RootFiles[i].Name.Count; j++)
                {
                    string name = FileOperate.RootFiles[i].Name[j];
                    string extension = FileOperate.getExtension(name);
                    if (FileOperate.IsSupportHide(extension) && !Form_Main.NoHide) { continue; }
                    int type = FileOperate.getFileType(extension);

                    if (type == 2) { Paths.Add(root); Names.Add(name); continue; }
                    if (type == 3) { Paths.Add(root); Names.Add(name); continue; }
                    if (type != 1) { continue; }

                    List<string> subnames = FileOperate.getSubFiles(root + "\\" + name);
                    for (int k = 0; k < subnames.Count; k++)
                    {
                        string subextension = FileOperate.getExtension(subnames[k]);
                        int subtype = FileOperate.getFileType(subextension);
                        if (FileOperate.IsSupportHide(subextension) && !Form_Main.NoHide) { continue; }

                        if (subtype == 2) { Paths.Add(root + "\\" + name); Names.Add(subnames[k]); continue; }
                        if (subtype == 3) { Paths.Add(root + "\\" + name); Names.Add(subnames[k]); continue; }
                    }
                }
            }

            #endregion

            #region 去除自己

            for (int i = Names.Count - 1; i >= 0; i--)
            {
                if (Paths[i] + "\\" + Names[i] == config.Sour) { Paths.RemoveAt(i); Names.RemoveAt(i); }
            }

            #endregion

            #region 去除重复的文件

            for (int i = Names.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if (Names[i] != Names[j] || Paths[i] != Paths[j]) { continue; }
                    Paths.RemoveAt(i);
                    Names.RemoveAt(i); break;
                }
            }

            #endregion
        }
        private void RemoveCurrent(int index)
        {
            config.Current.RemoveAt(index);
            int next = this.listBox1.SelectedIndex - 1;
            if (next < 0) { next = 0; }
            if (next >= config.Current.Count) { next = config.Current.Count - 1; }
            this.listBox1.SelectedIndex = -1;

            this.listBox1.Items.Clear();
            for (int i = 0; i < config.Current.Count; i++)
            {
                string sequence = "[" + (i + 1).ToString() + "] ";
                string name = Names[config.Current[i]];
                if (name.Length > 24) { name = "." + name.Substring(name.Length - 24); }

                this.listBox1.Items.Add(sequence + name);
            }
            this.listBox1.SelectedIndex = next;
        }
        private void SetMode()
        {
            if (this.likeToolStripMenuItem.Checked)
            {
                MessageBox.Show("不支持 LIKE 模式的查找", "提示");
                this.sameToolStripMenuItem.Checked = true;
                this.likeToolStripMenuItem.Checked = false;
            }

            ushort mode = 0;
            if (this.fullToolStripMenuItem.Checked) { mode |= (ushort)MODE.FULL; }
            if (this.partToolStripMenuItem.Checked) { mode |= (ushort)MODE.PART; }
            if (this.sameToolStripMenuItem.Checked) { mode |= (ushort)MODE.SAME; }
            if (this.likeToolStripMenuItem.Checked) { mode |= (ushort)MODE.LIKE; }
            if (this.turnToolStripMenuItem.Checked) { mode |= (ushort)MODE.TURN; }
            
            if (IsFinish) { config.Mode = (MODE)mode; return; }

            Stop();
            config.Mode = (MODE)mode;
            Continue();
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

            for (int i = Threads[0].begin - 1; i < Threads[0].end && !Threads[0].abort; ++i, ++Threads[0].begin)
            {
                #region 初始化变量

                dest = (Bitmap)Image.FromFile(Paths[i] + "\\" + Names[i]);
                desth = dest.Height;
                destw = dest.Width;
                found = false;

                //if (Names[i].Substring(0, 2) == "1%")
                //{
                //    int stop = 1; stop++;
                //}

                #endregion

                #region 行比较

                if (config.Row != -1)
                {
                    #region FULL SAME NO_TURN

                    if (config.Mode == MODE.FULL_SAME_NOTURN)
                    {
                        if (sourh != desth || sourw != destw) { goto END_ROW_CMP; }

                        double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                        int permitcnterr = sourw > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            sourw * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p=0; p < sourw; p += pace)
                        {
                            Color sc = sour.GetPixel((int)p, config.Row);
                            Color dc = dest.GetPixel((int)p, config.Row);
                            int sgray = sc.R + sc.B + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_ROW_CMP; }
                    }
                    
                    #endregion

                    #region FULL SAME TURN

                    if (config.Mode == MODE.FULL_SAME_TURN)
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel((int)p, config.Row);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int cmprow = sourh - 1 - config.Row;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel(sourw - 1 - (int)p, cmprow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel(config.Row, sourw - 1 - (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int cmprow = sourh - 1 - config.Row;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel(cmprow, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        goto END_ROW_CMP;
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    if (config.Mode == MODE.FULL_LIKE_NOTURN)
                    {
                        
                    }

                    #endregion

                    #region FULL LIKE TURN

                    if (config.Mode == MODE.FULL_LIKE_TURN)
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    if (config.Mode == MODE.PART_SAME_NOTURN)
                    {
                        int errparallel = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errparallel > 1) { goto END_ROW_CMP; }

                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        if (sourh < desth) { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errparallel > 1) { goto END_ROW_CMP; }

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

                        double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                        int permitcnterr = destw > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            destw * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < destw; p += pace)
                        {
                            Color sc = smap.GetPixel((int)p, cmprow);
                            Color dc = dmap.GetPixel((int)p, cmprow);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        if (sourh > dest.Height) { smap.Dispose(); }
                        if (sourh < dest.Height) { dmap.Dispose(); }
                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_ROW_CMP; }
                    }

                    #endregion

                    #region PART SAME TURN

                    if (config.Mode == MODE.PART_SAME_TURN)
                    {
                        int errparallel = 0, errcross = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (sourh > destw) { h2h = (double)sourh / destw; errcross = (int)(sourw / h2h) - desth; }
                        else { h2h = (double)destw / sourh; errcross = (int)(desth / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errcross < 0) { errcross = -errcross; }

                        #region 原样

                        desth = dest.Height;
                        destw = dest.Width;
                        if (errparallel < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < destw; p += pace)
                            {
                                Color sc = smap.GetPixel((int)p, cmprow);
                                Color dc = dmap.GetPixel((int)p, cmprow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (errparallel < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double x = 0; x < destw; x += pace)
                            {
                                Color sc = smap.GetPixel((int)x, srow);
                                Color dc = dmap.GetPixel(destw - 1 - (int)x, drow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (errcross < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double y = 0; y < desth; y += pace)
                            {
                                Color sc = smap.GetPixel((int)y, srow);
                                Color dc = dmap.GetPixel(drow, desth - 1 - (int)y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (errcross < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < desth; p += pace)
                            {
                                Color sc = smap.GetPixel((int)p, srow);
                                Color dc = dmap.GetPixel(drow, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        goto END_ROW_CMP;
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    if (config.Mode == MODE.PART_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    if (config.Mode == MODE.PART_LIKE_TURN)
                    {
                        goto END_ROW_CMP;
                    }

                    #endregion
                }

                END_ROW_CMP:
                #endregion

                #region 列比较

                if (config.Col != -1)
                {
                    #region FULL SAME NO_TURN

                    if (config.Mode == MODE.FULL_SAME_NOTURN)
                    {
                        if (sourh != desth || sourw != destw) { goto END_COL_CMP; }

                        double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                        int permitcnterr = sourh > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            sourh * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p=0; p < sourh; p += pace)
                        {
                            Color sc = sour.GetPixel(config.Col, (int)p);
                            Color dc = dest.GetPixel(config.Col, (int)p);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_COL_CMP; }
                    }

                    #endregion

                    #region FULL SAME TURN

                    if (config.Mode == MODE.FULL_SAME_TURN)
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p=0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel(config.Col, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int cmpcol = sourw - config.Col - 1;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel(cmpcol, sourh - 1 - (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int cmpcol = sourw - config.Col - 1;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel((int)p, cmpcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel(sourh - 1 - (int)p, config.Col);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        goto END_COL_CMP;
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    if (config.Mode == MODE.FULL_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    if (config.Mode == MODE.FULL_LIKE_TURN)
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    if (config.Mode == MODE.PART_SAME_NOTURN)
                    {
                        int errparallel = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errparallel > 1) { goto END_COL_CMP; }

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

                        double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                        int permitcnterr = desth > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            desth * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < desth; p += pace)
                        {
                            Color sc = smap.GetPixel(cmpcol, (int)p);
                            Color dc = dmap.GetPixel(cmpcol, (int)p);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            //errlist.Add(ierr);
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }
                        
                        if (sourh > dest.Height) { smap.Dispose(); }
                        if (sourh < dest.Height) { dmap.Dispose(); }
                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_COL_CMP; }
                    }

                    #endregion

                    #region PART SAME TURN

                    if (config.Mode == MODE.PART_SAME_TURN)
                    {
                        int errparallel = 0, errcross = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (sourh > destw) { h2h = (double)sourh / destw; errcross = (int)(sourw / h2h) - desth; }
                        else { h2h = (double)destw / sourh; errcross = (int)(desth / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errcross < 0) { errcross = -errcross; }

                        #region 原样

                        desth = dest.Height; destw = dest.Width;
                        if (errparallel < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < desth; p += pace)
                            {
                                Color sc = smap.GetPixel(cmpcol, (int)p);
                                Color dc = dmap.GetPixel(cmpcol, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度
                        
                        desth = dest.Height; destw = dest.Width;
                        if (errparallel < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < desth; p += pace)
                            {
                                Color sc = smap.GetPixel(scol, (int)p);
                                Color dc = dmap.GetPixel(dcol, desth - 1 - (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height; destw = dest.Width;
                        if (errcross < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < destw; p += pace)
                            {
                                Color sc = smap.GetPixel(scol, (int)p);
                                Color dc = dmap.GetPixel((int)p, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height; destw = dest.Width; 
                        if (errcross < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < destw; p += pace)
                            {
                                Color sc = smap.GetPixel(scol, (int)p);
                                Color dc = dmap.GetPixel(destw - 1 - (int)p, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        goto END_COL_CMP;
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    if (config.Mode == MODE.PART_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    if (config.Mode == MODE.PART_LIKE_TURN)
                    {
                        goto END_COL_CMP;
                    }

                    #endregion
                }

                END_COL_CMP:
                #endregion

                END:
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

            for (int i = Threads[1].begin - 1; i < Threads[1].end && !Threads[1].abort; ++i, ++Threads[1].begin)
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

                    if (config.Mode == MODE.FULL_SAME_NOTURN)
                    {
                        if (sourh != desth || sourw != destw) { goto END_ROW_CMP; }

                        double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                        int permitcnterr = sourw > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            sourw * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < sourw; p += pace)
                        {
                            Color sc = sour.GetPixel((int)p, config.Row);
                            Color dc = dest.GetPixel((int)p, config.Row);
                            int sgray = sc.R + sc.B + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_ROW_CMP; }
                    }

                    #endregion

                    #region FULL SAME TURN

                    if (config.Mode == MODE.FULL_SAME_TURN)
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel((int)p, config.Row);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int cmprow = sourh - 1 - config.Row;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel(sourw - 1 - (int)p, cmprow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel(config.Row, sourw - 1 - (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int cmprow = sourh - 1 - config.Row;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel(cmprow, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        goto END_ROW_CMP;
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    if (config.Mode == MODE.FULL_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    if (config.Mode == MODE.FULL_LIKE_TURN)
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    if (config.Mode == MODE.PART_SAME_NOTURN)
                    {
                        int errparallel = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errparallel > 1) { goto END_ROW_CMP; }

                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        if (sourh < desth) { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errparallel > 1) { goto END_ROW_CMP; }

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

                        double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                        int permitcnterr = destw > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            destw * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < destw; p += pace)
                        {
                            Color sc = smap.GetPixel((int)p, cmprow);
                            Color dc = dmap.GetPixel((int)p, cmprow);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        if (sourh > dest.Height) { smap.Dispose(); }
                        if (sourh < dest.Height) { dmap.Dispose(); }
                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_ROW_CMP; }
                    }

                    #endregion

                    #region PART SAME TURN

                    if (config.Mode == MODE.PART_SAME_TURN)
                    {
                        int errparallel = 0, errcross = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (sourh > destw) { h2h = (double)sourh / destw; errcross = (int)(sourw / h2h) - desth; }
                        else { h2h = (double)destw / sourh; errcross = (int)(desth / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errcross < 0) { errcross = -errcross; }

                        #region 原样

                        desth = dest.Height;
                        destw = dest.Width;
                        if (errparallel < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < destw; p += pace)
                            {
                                Color sc = smap.GetPixel((int)p, cmprow);
                                Color dc = dmap.GetPixel((int)p, cmprow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (errparallel < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double x = 0; x < destw; x += pace)
                            {
                                Color sc = smap.GetPixel((int)x, srow);
                                Color dc = dmap.GetPixel(destw - 1 - (int)x, drow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (errcross < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double y = 0; y < desth; y += pace)
                            {
                                Color sc = smap.GetPixel((int)y, srow);
                                Color dc = dmap.GetPixel(drow, desth - 1 - (int)y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (errcross < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < desth; p += pace)
                            {
                                Color sc = smap.GetPixel((int)p, srow);
                                Color dc = dmap.GetPixel(drow, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        goto END_ROW_CMP;
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    if (config.Mode == MODE.PART_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    if (config.Mode == MODE.PART_LIKE_TURN)
                    {
                        goto END_ROW_CMP;
                    }

                    #endregion
                }

                END_ROW_CMP:
                #endregion

                #region 列比较

                if (config.Col != -1)
                {
                    #region FULL SAME NO_TURN

                    if (config.Mode == MODE.FULL_SAME_NOTURN)
                    {
                        if (sourh != desth || sourw != destw) { goto END_COL_CMP; }

                        double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                        int permitcnterr = sourh > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            sourh * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < sourh; p += pace)
                        {
                            Color sc = sour.GetPixel(config.Col, (int)p);
                            Color dc = dest.GetPixel(config.Col, (int)p);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_COL_CMP; }
                    }

                    #endregion

                    #region FULL SAME TURN

                    if (config.Mode == MODE.FULL_SAME_TURN)
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel(config.Col, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int cmpcol = sourw - config.Col - 1;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel(cmpcol, sourh - 1 - (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int cmpcol = sourw - config.Col - 1;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel((int)p, cmpcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel(sourh - 1 - (int)p, config.Col);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        goto END_COL_CMP;
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    if (config.Mode == MODE.FULL_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    if (config.Mode == MODE.FULL_LIKE_TURN)
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    if (config.Mode == MODE.PART_SAME_NOTURN)
                    {
                        int errparallel = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errparallel > 1) { goto END_COL_CMP; }

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

                        double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                        int permitcnterr = desth > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            desth * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < desth; p += pace)
                        {
                            Color sc = smap.GetPixel(cmpcol, (int)p);
                            Color dc = dmap.GetPixel(cmpcol, (int)p);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            //errlist.Add(ierr);
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        if (sourh > dest.Height) { smap.Dispose(); }
                        if (sourh < dest.Height) { dmap.Dispose(); }
                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_COL_CMP; }
                    }

                    #endregion

                    #region PART SAME TURN

                    if (config.Mode == MODE.PART_SAME_TURN)
                    {
                        int errparallel = 0, errcross = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (sourh > destw) { h2h = (double)sourh / destw; errcross = (int)(sourw / h2h) - desth; }
                        else { h2h = (double)destw / sourh; errcross = (int)(desth / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errcross < 0) { errcross = -errcross; }

                        #region 原样

                        desth = dest.Height; destw = dest.Width;
                        if (errparallel < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < desth; p += pace)
                            {
                                Color sc = smap.GetPixel(cmpcol, (int)p);
                                Color dc = dmap.GetPixel(cmpcol, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height; destw = dest.Width;
                        if (errparallel < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < desth; p += pace)
                            {
                                Color sc = smap.GetPixel(scol, (int)p);
                                Color dc = dmap.GetPixel(dcol, desth - 1 - (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height; destw = dest.Width;
                        if (errcross < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < destw; p += pace)
                            {
                                Color sc = smap.GetPixel(scol, (int)p);
                                Color dc = dmap.GetPixel((int)p, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height; destw = dest.Width;
                        if (errcross < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < destw; p += pace)
                            {
                                Color sc = smap.GetPixel(scol, (int)p);
                                Color dc = dmap.GetPixel(destw - 1 - (int)p, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        goto END_COL_CMP;
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    if (config.Mode == MODE.PART_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    if (config.Mode == MODE.PART_LIKE_TURN)
                    {
                        goto END_COL_CMP;
                    }

                    #endregion
                }

                END_COL_CMP:
                #endregion

                END:
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

            for (int i = Threads[2].begin - 1; i < Threads[2].end && !Threads[2].abort; ++i, ++Threads[2].begin)
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

                    if (config.Mode == MODE.FULL_SAME_NOTURN)
                    {
                        if (sourh != desth || sourw != destw) { goto END_ROW_CMP; }

                        double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                        int permitcnterr = sourw > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            sourw * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < sourw; p += pace)
                        {
                            Color sc = sour.GetPixel((int)p, config.Row);
                            Color dc = dest.GetPixel((int)p, config.Row);
                            int sgray = sc.R + sc.B + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_ROW_CMP; }
                    }

                    #endregion

                    #region FULL SAME TURN

                    if (config.Mode == MODE.FULL_SAME_TURN)
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel((int)p, config.Row);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int cmprow = sourh - 1 - config.Row;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel(sourw - 1 - (int)p, cmprow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel(config.Row, sourw - 1 - (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int cmprow = sourh - 1 - config.Row;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel(cmprow, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        goto END_ROW_CMP;
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    if (config.Mode == MODE.FULL_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    if (config.Mode == MODE.FULL_LIKE_TURN)
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    if (config.Mode == MODE.PART_SAME_NOTURN)
                    {
                        int errparallel = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errparallel > 1) { goto END_ROW_CMP; }

                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        if (sourh < desth) { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errparallel > 1) { goto END_ROW_CMP; }

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

                        double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                        int permitcnterr = destw > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            destw * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < destw; p += pace)
                        {
                            Color sc = smap.GetPixel((int)p, cmprow);
                            Color dc = dmap.GetPixel((int)p, cmprow);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        if (sourh > dest.Height) { smap.Dispose(); }
                        if (sourh < dest.Height) { dmap.Dispose(); }
                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_ROW_CMP; }
                    }

                    #endregion

                    #region PART SAME TURN

                    if (config.Mode == MODE.PART_SAME_TURN)
                    {
                        int errparallel = 0, errcross = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (sourh > destw) { h2h = (double)sourh / destw; errcross = (int)(sourw / h2h) - desth; }
                        else { h2h = (double)destw / sourh; errcross = (int)(desth / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errcross < 0) { errcross = -errcross; }

                        #region 原样

                        desth = dest.Height;
                        destw = dest.Width;
                        if (errparallel < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < destw; p += pace)
                            {
                                Color sc = smap.GetPixel((int)p, cmprow);
                                Color dc = dmap.GetPixel((int)p, cmprow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (errparallel < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double x = 0; x < destw; x += pace)
                            {
                                Color sc = smap.GetPixel((int)x, srow);
                                Color dc = dmap.GetPixel(destw - 1 - (int)x, drow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (errcross < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double y = 0; y < desth; y += pace)
                            {
                                Color sc = smap.GetPixel((int)y, srow);
                                Color dc = dmap.GetPixel(drow, desth - 1 - (int)y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (errcross < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < desth; p += pace)
                            {
                                Color sc = smap.GetPixel((int)p, srow);
                                Color dc = dmap.GetPixel(drow, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        goto END_ROW_CMP;
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    if (config.Mode == MODE.PART_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    if (config.Mode == MODE.PART_LIKE_TURN)
                    {
                        goto END_ROW_CMP;
                    }

                    #endregion
                }

                END_ROW_CMP:
                #endregion

                #region 列比较

                if (config.Col != -1)
                {
                    #region FULL SAME NO_TURN

                    if (config.Mode == MODE.FULL_SAME_NOTURN)
                    {
                        if (sourh != desth || sourw != destw) { goto END_COL_CMP; }

                        double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                        int permitcnterr = sourh > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            sourh * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < sourh; p += pace)
                        {
                            Color sc = sour.GetPixel(config.Col, (int)p);
                            Color dc = dest.GetPixel(config.Col, (int)p);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_COL_CMP; }
                    }

                    #endregion

                    #region FULL SAME TURN

                    if (config.Mode == MODE.FULL_SAME_TURN)
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel(config.Col, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int cmpcol = sourw - config.Col - 1;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel(cmpcol, sourh - 1 - (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int cmpcol = sourw - config.Col - 1;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel((int)p, cmpcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel(sourh - 1 - (int)p, config.Col);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        goto END_COL_CMP;
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    if (config.Mode == MODE.FULL_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    if (config.Mode == MODE.FULL_LIKE_TURN)
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    if (config.Mode == MODE.PART_SAME_NOTURN)
                    {
                        int errparallel = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errparallel > 1) { goto END_COL_CMP; }

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

                        double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                        int permitcnterr = desth > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            desth * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < desth; p += pace)
                        {
                            Color sc = smap.GetPixel(cmpcol, (int)p);
                            Color dc = dmap.GetPixel(cmpcol, (int)p);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            //errlist.Add(ierr);
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        if (sourh > dest.Height) { smap.Dispose(); }
                        if (sourh < dest.Height) { dmap.Dispose(); }
                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_COL_CMP; }
                    }

                    #endregion

                    #region PART SAME TURN

                    if (config.Mode == MODE.PART_SAME_TURN)
                    {
                        int errparallel = 0, errcross = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (sourh > destw) { h2h = (double)sourh / destw; errcross = (int)(sourw / h2h) - desth; }
                        else { h2h = (double)destw / sourh; errcross = (int)(desth / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errcross < 0) { errcross = -errcross; }

                        #region 原样

                        desth = dest.Height; destw = dest.Width;
                        if (errparallel < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < desth; p += pace)
                            {
                                Color sc = smap.GetPixel(cmpcol, (int)p);
                                Color dc = dmap.GetPixel(cmpcol, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height; destw = dest.Width;
                        if (errparallel < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < desth; p += pace)
                            {
                                Color sc = smap.GetPixel(scol, (int)p);
                                Color dc = dmap.GetPixel(dcol, desth - 1 - (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height; destw = dest.Width;
                        if (errcross < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < destw; p += pace)
                            {
                                Color sc = smap.GetPixel(scol, (int)p);
                                Color dc = dmap.GetPixel((int)p, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height; destw = dest.Width;
                        if (errcross < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < destw; p += pace)
                            {
                                Color sc = smap.GetPixel(scol, (int)p);
                                Color dc = dmap.GetPixel(destw - 1 - (int)p, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        goto END_COL_CMP;
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    if (config.Mode == MODE.PART_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    if (config.Mode == MODE.PART_LIKE_TURN)
                    {
                        goto END_COL_CMP;
                    }

                    #endregion
                }

                END_COL_CMP:
                #endregion

                END:
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

            for (int i = Threads[3].begin - 1; i < Threads[3].end && !Threads[3].abort; ++i, ++Threads[3].begin)
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

                    if (config.Mode == MODE.FULL_SAME_NOTURN)
                    {
                        if (sourh != desth || sourw != destw) { goto END_ROW_CMP; }

                        double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                        int permitcnterr = sourw > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            sourw * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < sourw; p += pace)
                        {
                            Color sc = sour.GetPixel((int)p, config.Row);
                            Color dc = dest.GetPixel((int)p, config.Row);
                            int sgray = sc.R + sc.B + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_ROW_CMP; }
                    }

                    #endregion

                    #region FULL SAME TURN

                    if (config.Mode == MODE.FULL_SAME_TURN)
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel((int)p, config.Row);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int cmprow = sourh - 1 - config.Row;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel(sourw - 1 - (int)p, cmprow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel(config.Row, sourw - 1 - (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourw > config.MinCmpPix ? (double)sourw / config.MinCmpPix : 1;
                            int permitcnterr = sourw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourw * (100 - config.Degree) / 100;
                            int cmprow = sourh - 1 - config.Row;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourw; p += pace)
                            {
                                Color sc = sour.GetPixel((int)p, config.Row);
                                Color dc = dest.GetPixel(cmprow, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        goto END_ROW_CMP;
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    if (config.Mode == MODE.FULL_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    if (config.Mode == MODE.FULL_LIKE_TURN)
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    if (config.Mode == MODE.PART_SAME_NOTURN)
                    {
                        int errparallel = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errparallel > 1) { goto END_ROW_CMP; }

                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        if (sourh < desth) { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errparallel > 1) { goto END_ROW_CMP; }

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

                        double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                        int permitcnterr = destw > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            destw * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < destw; p += pace)
                        {
                            Color sc = smap.GetPixel((int)p, cmprow);
                            Color dc = dmap.GetPixel((int)p, cmprow);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        if (sourh > dest.Height) { smap.Dispose(); }
                        if (sourh < dest.Height) { dmap.Dispose(); }
                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_ROW_CMP; }
                    }

                    #endregion

                    #region PART SAME TURN

                    if (config.Mode == MODE.PART_SAME_TURN)
                    {
                        int errparallel = 0, errcross = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (sourh > destw) { h2h = (double)sourh / destw; errcross = (int)(sourw / h2h) - desth; }
                        else { h2h = (double)destw / sourh; errcross = (int)(desth / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errcross < 0) { errcross = -errcross; }

                        #region 原样

                        desth = dest.Height;
                        destw = dest.Width;
                        if (errparallel < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < destw; p += pace)
                            {
                                Color sc = smap.GetPixel((int)p, cmprow);
                                Color dc = dmap.GetPixel((int)p, cmprow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (errparallel < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double x = 0; x < destw; x += pace)
                            {
                                Color sc = smap.GetPixel((int)x, srow);
                                Color dc = dmap.GetPixel(destw - 1 - (int)x, drow);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (errcross < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double y = 0; y < desth; y += pace)
                            {
                                Color sc = smap.GetPixel((int)y, srow);
                                Color dc = dmap.GetPixel(drow, desth - 1 - (int)y);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height;
                        destw = dest.Width;

                        if (errcross < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < desth; p += pace)
                            {
                                Color sc = smap.GetPixel((int)p, srow);
                                Color dc = dmap.GetPixel(drow, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_ROW_CMP; }
                        }

                        #endregion

                        goto END_ROW_CMP;
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    if (config.Mode == MODE.PART_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    if (config.Mode == MODE.PART_LIKE_TURN)
                    {
                        goto END_ROW_CMP;
                    }

                    #endregion
                }

                END_ROW_CMP:
                #endregion

                #region 列比较

                if (config.Col != -1)
                {
                    #region FULL SAME NO_TURN

                    if (config.Mode == MODE.FULL_SAME_NOTURN)
                    {
                        if (sourh != desth || sourw != destw) { goto END_COL_CMP; }

                        double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                        int permitcnterr = sourh > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            sourh * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < sourh; p += pace)
                        {
                            Color sc = sour.GetPixel(config.Col, (int)p);
                            Color dc = dest.GetPixel(config.Col, (int)p);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_COL_CMP; }
                    }

                    #endregion

                    #region FULL SAME TURN

                    if (config.Mode == MODE.FULL_SAME_TURN)
                    {
                        #region 原样

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel(config.Col, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        if (sourh == desth && sourw == destw)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int cmpcol = sourw - config.Col - 1;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel(cmpcol, sourh - 1 - (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int cmpcol = sourw - config.Col - 1;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel((int)p, cmpcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        if (sourh == destw && sourw == desth)
                        {
                            double pace = sourh > config.MinCmpPix ? (double)sourh / config.MinCmpPix : 1;
                            int permitcnterr = sourh > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                sourh * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < sourh; p += pace)
                            {
                                Color sc = sour.GetPixel(config.Col, (int)p);
                                Color dc = dest.GetPixel(sourh - 1 - (int)p, config.Col);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        goto END_COL_CMP;
                    }

                    #endregion

                    #region FULL LIKE NO_TURN

                    if (config.Mode == MODE.FULL_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region FULL LIKE TURN

                    if (config.Mode == MODE.FULL_LIKE_TURN)
                    {

                    }

                    #endregion

                    #region PART SAME NO_TURN

                    if (config.Mode == MODE.PART_SAME_NOTURN)
                    {
                        int errparallel = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errparallel > 1) { goto END_COL_CMP; }

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

                        double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                        int permitcnterr = desth > config.MinCmpPix ?
                            config.MinCmpPix * (100 - config.Degree) / 100 :
                            desth * (100 - config.Degree) / 100;
                        int permiterr = 10;
                        int cnterr = 0;

                        for (double p = 0; p < desth; p += pace)
                        {
                            Color sc = smap.GetPixel(cmpcol, (int)p);
                            Color dc = dmap.GetPixel(cmpcol, (int)p);
                            int sgray = sc.R + sc.G + sc.B;
                            int dgray = dc.R + dc.G + dc.B;

                            int ierr = (sgray - dgray) / 3;
                            //errlist.Add(ierr);
                            if (ierr < 0) { ierr = -ierr; }
                            if (ierr > permiterr) { ++cnterr; }
                            if (cnterr > permitcnterr) { break; }
                        }

                        if (sourh > dest.Height) { smap.Dispose(); }
                        if (sourh < dest.Height) { dmap.Dispose(); }
                        found = cnterr <= permitcnterr;
                        if (found) { goto END; } else { goto END_COL_CMP; }
                    }

                    #endregion

                    #region PART SAME TURN

                    if (config.Mode == MODE.PART_SAME_TURN)
                    {
                        int errparallel = 0, errcross = 0;
                        double h2h = 1;
                        if (sourh > desth) { h2h = (double)sourh / desth; errparallel = (int)(sourw / h2h) - destw; }
                        else { h2h = (double)desth / sourh; errparallel = (int)(destw / h2h) - sourw; }
                        if (sourh > destw) { h2h = (double)sourh / destw; errcross = (int)(sourw / h2h) - desth; }
                        else { h2h = (double)destw / sourh; errcross = (int)(desth / h2h) - sourw; }
                        if (errparallel < 0) { errparallel = -errparallel; }
                        if (errcross < 0) { errcross = -errcross; }

                        #region 原样

                        desth = dest.Height; destw = dest.Width;
                        if (errparallel < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < desth; p += pace)
                            {
                                Color sc = smap.GetPixel(cmpcol, (int)p);
                                Color dc = dmap.GetPixel(cmpcol, (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 180 度

                        desth = dest.Height; destw = dest.Width;
                        if (errparallel < 2)
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

                            double pace = desth > config.MinCmpPix ? (double)desth / config.MinCmpPix : 1;
                            int permitcnterr = desth > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                desth * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < desth; p += pace)
                            {
                                Color sc = smap.GetPixel(scol, (int)p);
                                Color dc = dmap.GetPixel(dcol, desth - 1 - (int)p);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Height) { smap.Dispose(); }
                            if (sourh < dest.Height) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        #region 逆时针旋转90度

                        desth = dest.Height; destw = dest.Width;
                        if (errcross < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < destw; p += pace)
                            {
                                Color sc = smap.GetPixel(scol, (int)p);
                                Color dc = dmap.GetPixel((int)p, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; }
                        }

                        #endregion

                        #region 逆时针旋转 270 度

                        desth = dest.Height; destw = dest.Width;
                        if (errcross < 2)
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

                            double pace = destw > config.MinCmpPix ? (double)destw / config.MinCmpPix : 1;
                            int permitcnterr = destw > config.MinCmpPix ?
                                config.MinCmpPix * (100 - config.Degree) / 100 :
                                destw * (100 - config.Degree) / 100;
                            int permiterr = 10;
                            int cnterr = 0;

                            for (double p = 0; p < destw; p += pace)
                            {
                                Color sc = smap.GetPixel(scol, (int)p);
                                Color dc = dmap.GetPixel(destw - 1 - (int)p, dcol);
                                int sgray = sc.R + sc.G + sc.B;
                                int dgray = dc.R + dc.G + dc.B;

                                int ierr = (sgray - dgray) / 3;
                                //errlist.Add(ierr);
                                if (ierr < 0) { ierr = -ierr; }
                                if (ierr > permiterr) { ++cnterr; }
                                if (cnterr > permitcnterr) { break; }
                            }

                            if (sourh > dest.Width) { smap.Dispose(); }
                            if (sourh < dest.Width) { dmap.Dispose(); }

                            found = cnterr <= permitcnterr;
                            if (found) { goto END; } else { goto END_COL_CMP; }
                        }

                        #endregion

                        goto END_COL_CMP;
                    }

                    #endregion

                    #region PATR LIKE NO_TURN

                    if (config.Mode == MODE.PART_LIKE_NOTURN)
                    {

                    }

                    #endregion

                    #region PART LIKE TURN

                    if (config.Mode == MODE.PART_LIKE_TURN)
                    {
                        goto END_COL_CMP;
                    }

                    #endregion
                }

                END_COL_CMP:
                #endregion

                END:
                lock (config.Lock) { if (found) { config.Results.Add(i); } config.CountFiles++; }
                dest.Dispose();
            }

            Threads[3].finish = true;
        }
    }
}
