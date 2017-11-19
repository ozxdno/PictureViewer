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
        /// 源文件
        /// </summary>
        public static PICTURE SourFile;
        /// <summary>
        /// 正在显示的寻找结果图片
        /// </summary>
        public static Bitmap DestPic;
        /// <summary>
        /// 目标文件
        /// </summary>
        public static PICTURE DestFile;

        /// <summary>
        /// 查找是否已经结束
        /// </summary>
        public static bool IsFinish = false;
        /// <summary>
        /// 是否转到选择图片
        /// </summary>
        public static bool IsSwitch = false;
        /// <summary>
        /// 搜索的结果
        /// </summary>
        public static List<List<int>> Results
        {
            get { lock (Lock) { return results; } }
            set { lock (Lock) { results = value; } }
        }
        /// <summary>
        /// 源图片正在比较的序号
        /// </summary>
        public static int IndexS
        {
            get { lock (Lock) { return indexs; } }
            set { lock (Lock) { indexs = value; } }
        }
        /// <summary>
        /// 目标图片正在比较的序号
        /// </summary>
        public static int IndexD
        {
            get { lock (Lock) { return indexd; } }
            set { lock (Lock) { indexd = value; } }
        }

        /// <summary>
        /// 源文件
        /// </summary>
        public static List<PICTURE> PictureFiles = new List<PICTURE>();
        /// <summary>
        /// 其他信息
        /// </summary>
        public static CONFIG config;

        /// <summary>
        /// 图片的文件信息
        /// </summary>
        public struct PICTURE
        {
            /// <summary>
            /// 图片已经被加载了
            /// </summary>
            public bool Loaded;
            /// <summary>
            /// 上一次修改时间
            /// </summary>
            public long Time;

            /// <summary>
            /// 全称
            /// </summary>
            public string Full;
            /// <summary>
            /// 路径
            /// </summary>
            public string Path;
            /// <summary>
            /// 名称
            /// </summary>
            public string Name;

            /// <summary>
            /// 文件所在根目录序号
            /// </summary>
            public List<int> FolderIndexes;
            /// <summary>
            /// 文件序号
            /// </summary>
            public List<int> FileIndexes;
            /// <summary>
            /// 子文件序号
            /// </summary>
            public List<int> SubIndexes;

            /// <summary>
            /// 高，单位：像素
            /// </summary>
            public int Height;
            /// <summary>
            /// 宽，单位：像素
            /// </summary>
            public int Width;
            /// <summary>
            /// 大小，单位：B（字节）
            /// </summary>
            public long Length;
            /// <summary>
            /// 比较行
            /// </summary>
            public int Row;
            /// <summary>
            /// 比较列
            /// </summary>
            public int Col;
            /// <summary>
            /// 行灰度值
            /// </summary>
            public int[] GraysR;
            /// <summary>
            /// 列灰度值
            /// </summary>
            public int[] GraysC;
        }
        /// <summary>
        /// 其他信息
        /// </summary>
        public struct CONFIG
        {
            /// <summary>
            /// 初始化中
            /// </summary>
            public bool Initializing;

            /// <summary>
            /// 比对图
            /// </summary>
            public List<int> Standard;
            /// <summary>
            /// 当前的显示结果
            /// </summary>
            public List<int> Current;
            /// <summary>
            /// 比对图序列
            /// </summary>
            public List<int> Sour;
            /// <summary>
            /// 比对图序列
            /// </summary>
            public List<int> Dest;

            /// <summary>
            /// 匹配模式
            /// </summary>
            public MODE Mode;
            /// <summary>
            /// 比较方式：
            /// 0 - 寻找单图；
            /// 1 - 寻找重复图片；
            /// 2 - 寻找存在图片；
            /// </summary>
            public int Method;
            /// <summary>
            /// 最小比较元素个数
            /// </summary>
            public int MinCmpPix;
            /// <summary>
            /// 相似程度
            /// </summary>
            public int Degree;

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
        }
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

        private static object Lock = new object();
        private static List<List<int>> results = new List<List<int>>();
        private static int indexs = 0;
        private static int indexd = 0;

        private static int prevIndexS = -1;
        private static int prevIndexD = -1;

        private static Thread TH = null;
        private static PicMatch PM = new PicMatch();

        private static List<Form_Image> images = new List<Form_Image>();

        /////////////////////////////////////////////////////////// public method //////////////////////////////////////////////////

        /// <summary>
        /// 查找图片
        /// </summary>
        /// <param name="mode">查找模式</param>
        public Form_Find(MODE mode = MODE.DEFAULT)
        {
            InitializeComponent();
            config.Mode = mode;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig()
        {
            Class.Save.settings.Form_Main_Find_Full = this.fullToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Part = this.partToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Same = this.sameToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Like = this.likeToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Turn = this.turnToolStripMenuItem.Checked;
            Class.Save.settings.Form_Find_Degree = config.Degree;
            Class.Save.settings.Form_Find_Pixes = config.MinCmpPix;
        }

        /////////////////////////////////////////////////////////// private method //////////////////////////////////////////////////

        private void Form_Load(object sender, EventArgs e)
        {
            InitializeForm();
            ShowInitial();

            Timer.Elapsed += new System.Timers.ElapsedEventHandler(Form_Updata);
            Timer.AutoReset = true;
            Timer.Start();
            
            //Start();
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

            config.Initializing = false;
            Stop();
            Timer.Close();
            Results.Clear();
            CloseImage();

            try { SourPic.Dispose(); } catch { }
            try { DestPic.Dispose(); } catch { }
        }
        private void Form_Updata(object source, System.Timers.ElapsedEventArgs e)
        {
            this.BeginInvoke((EventHandler)delegate{
                
                if (!IsFinish) { config.CountTime++; config.TimeED = config.CountTime; }
                
                if (config.Initializing)
                {
                    this.Text = "[Preparing Files]: " + IndexS.ToString();
                }
                if (!config.Initializing && config.Method == -1)
                {
                    this.Text = "[Prepared Files]: " + PictureFiles.Count.ToString();
                }

                for (int i = images.Count - 1; i >= 0; i--)
                {
                    if (images[i].IsDisposed) { images.RemoveAt(i); }
                }

                bool s1 = false;
                foreach (ToolStripMenuItem i in this.source1ToolStripMenuItem.DropDownItems)
                { if (i.Checked) { s1 = true; break; } }
                this.source1ToolStripMenuItem.Checked = s1;

                bool s2 = false;
                foreach (ToolStripMenuItem i in this.source2ToolStripMenuItem.DropDownItems)
                { if (i.Checked) { s2 = true; break; } }
                this.source2ToolStripMenuItem.Checked = s2;

                if (config.Method != -1 && !config.Initializing)
                {
                    double usedTime = (double)(config.TimeED - config.TimeBG) / 10;
                    string usedtime = usedTime.ToString();
                    if (usedtime.Length < 2 || usedtime[usedtime.Length - 2] != '.') { usedtime += ".0"; }

                    string sourIndex = IndexS.ToString() + "/" + config.Sour.Count.ToString();
                    string destIndex = IndexD.ToString() + "/" + config.Dest.Count.ToString();

                    string cntResult = config.Method == 0 ?
                        (Results.Count == 0 ? "0" : Results[0].Count.ToString()) :
                        Results.Count.ToString();

                    this.Text = "[Find]: " + sourIndex + " " + destIndex + " [Used]: " + usedtime + " s [Result]: " + cntResult;
                }

                ShowCombo();
                ShowList();
                if (IsFinish && !PM.Abort) { this.startToolStripMenuItem.Text = "Start"; }

                Point ptMouse;
                ptMouse = this.label6.PointToClient(MousePosition);
                this.label6.Visible =
                    config.Standard.Count > 1 &&
                    ptMouse.X >= 0 && ptMouse.X < this.label6.Width &&
                    ptMouse.Y >= 0 && ptMouse.Y < this.label6.Height;
                ptMouse = this.label3.PointToClient(MousePosition);
                this.label3.Visible =
                    config.Standard.Count > 1 &&
                    ptMouse.X >= 0 && ptMouse.X < this.label3.Width &&
                    ptMouse.Y >= 0 && ptMouse.Y < this.label3.Height;
                ptMouse = this.label4.PointToClient(MousePosition);
                this.label4.Visible =
                    config.Current.Count > 1 &&
                    ptMouse.X >= 0 && ptMouse.X < this.label4.Width &&
                    ptMouse.Y >= 0 && ptMouse.Y < this.label4.Height;
                ptMouse = this.label5.PointToClient(MousePosition);
                this.label5.Visible =
                    config.Current.Count > 1 &&
                    ptMouse.X >= 0 && ptMouse.X < this.label5.Width &&
                    ptMouse.Y >= 0 && ptMouse.Y < this.label5.Height;
            });
        }
        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == Class.Load.settings.FastKey_Find_Esc) { this.Close(); return; }
            if (e.KeyValue == Class.Load.settings.FastKey_Find_U) { PreviousSour(null, null); return; }
            if (e.KeyValue == Class.Load.settings.FastKey_Find_D) { NextSour(null, null); return; }
            if (e.KeyValue == Class.Load.settings.FastKey_Find_L) { Previous(null, null); return; }
            if (e.KeyValue == Class.Load.settings.FastKey_Find_R) { Next(null, null); return; }
            if (e.KeyValue == Class.Load.settings.FastKey_Find_Export) { RightMenu_Export(null, null); return; }
        }
        
        private void Previous(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            int index = this.listBox1.SelectedIndex - 1;
            if (index < 0) { index = this.listBox1.Items.Count - 1; }
            this.listBox1.SelectedIndex = index;
        }
        private void Next(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            if (this.listBox1.Items.Count == 0) { return; }
            int index = this.listBox1.SelectedIndex + 1;
            if (index >= this.listBox1.Items.Count) { index = 0; }
            this.listBox1.SelectedIndex = index;
        }
        private void PreviousSour(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }
            if (config.Standard.Count == 0) { return; }

            int sel = this.comboBox1.SelectedIndex;
            int next = sel - 1; if (next < 0) { next = config.Standard.Count - 1; }
            this.comboBox1.SelectedIndex = next;
        }
        private void NextSour(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }
            if (config.Standard.Count == 0) { return; }

            int sel = this.comboBox1.SelectedIndex;
            int next = sel + 1; if (next >= config.Standard.Count) { next = 0; }
            this.comboBox1.SelectedIndex = next;
        }
        private void ListIndexChanged(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            int sel = this.listBox1.SelectedIndex;
            //if (sel == prevIndexD) { if (sel != -1) { ShowDestPic(); } return; }
            if (sel != -1) { DestFile = PictureFiles[config.Current[sel]]; }
            ShowDestPic();
            prevIndexD = sel;
        }
        private void ComboIndexChanged(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            int sel = this.comboBox1.SelectedIndex;
            if (prevIndexS == sel) { return; }
            if (sel != -1) { SourFile = PictureFiles[config.Standard[sel]]; }
            ShowSourPic();
            prevIndexS = sel;

            config.Current.Clear();
            this.listBox1.Items.Clear();
        }
        private void DoubleClickedToSwitch(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            int indexS = this.comboBox1.SelectedIndex;
            int indexD = this.listBox1.SelectedIndex;
            if (indexS == -1 || indexD == -1) { return; }
            if (Results.Count <= indexS || Results[indexS].Count <= indexD) { return; }

            PICTURE p = PictureFiles[config.Current[indexD]];
            //Stop();

            List<int> selectedfolders = new List<int>();
            for (int i = 0; i < this.source2ToolStripMenuItem.DropDownItems.Count; i++)
            {
                ToolStripMenuItem iMenu = (ToolStripMenuItem)this.source2ToolStripMenuItem.DropDownItems[i];
                if (iMenu.Checked) { selectedfolders.Add(i); }
            }
            int selected = 0;
            for (int i = 0; i < p.FolderIndexes.Count; i++)
            {
                if (selectedfolders.IndexOf(p.FolderIndexes[i]) != -1) { selected = i; break; }
            }

            Form_Main.config.FolderIndex = p.FolderIndexes[selected];
            Form_Main.config.FileIndex = p.FileIndexes[selected];
            Form_Main.config.SubIndex = p.SubIndexes[selected];
            IsSwitch = true;
            //this.Close();
        }

        private void RightMenu_Start(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            if (this.startToolStripMenuItem.Text == "Start") { Start(); return; }
            if (this.startToolStripMenuItem.Text == "Continue") { Continue(); return; }
            if (this.startToolStripMenuItem.Text == "Stop") { Stop(); return; }
            this.startToolStripMenuItem.Text = "Start";
        }
        private void RightMenu_Export(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            int indexS = this.comboBox1.SelectedIndex;
            int indexD = this.listBox1.SelectedIndex;
            if (indexS == -1 || indexD == -1) { MessageBox.Show("文件不存在！", "提示"); return; }
            if (Results.Count <= indexS || Results[indexS].Count <= indexD) { MessageBox.Show("文件不存在！", "提示"); return; }
            
            PICTURE p = PictureFiles[config.Current[indexD]];

            string sourpath = p.Path;
            string sourname = p.Name;
            string destpath = Form_Main.config.ExportFolder;
            string destname = sourname;

            if (!Directory.Exists(destpath)) { destpath = FileOperate.getExePath(); }

            string sour = sourpath + "\\" + sourname;
            string dest = destpath + "\\" + destname;

            //if (!Directory.Exists(destpath)) { MessageBox.Show("输出路径不存在！", "提示"); return; }
            if (File.Exists(dest)) { MessageBox.Show("目标文件夹存在同名文件！", "提示"); return; }
            if (!File.Exists(sour)) { MessageBox.Show("该文件不存在！", "提示"); return; }

            if (DialogResult.Cancel == MessageBox.Show("把当前文件导出？", "确认", MessageBoxButtons.OKCancel))
            { return; }

            //Stop();
            CloseImage(p.Path, p.Name);
            
            try { SourPic.Dispose(); } catch { }
            try { DestPic.Dispose(); } catch { }
            try {
                File.Move(sour, dest);
                Results[indexS].RemoveAt(indexD);
                ShowSourPic();
                ShowDestPic();
            } catch {
                ShowSourPic();
                ShowDestPic();
                MessageBox.Show("移动失败！", "提示");
            }
        }
        private void RightMenu_Export2(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            int indexS = this.comboBox1.SelectedIndex;
            if (indexS == -1) { MessageBox.Show("文件不存在！", "提示"); return; }
            if (Results.Count <= indexS || Results[indexS].Count == 0) { MessageBox.Show("文件不存在！", "提示"); return; }
            if (DialogResult.Cancel == MessageBox.Show("把当前所有文件导出？", "确认", MessageBoxButtons.OKCancel)) { return; }

            // 把当前文件导出并不会影响搜索
            //Stop();

            try { SourPic.Dispose(); } catch { }
            try { DestPic.Dispose(); } catch { }
            this.pictureBox1.BackgroundImage = null;
            this.pictureBox2.BackgroundImage = null;

            string export = Form_Main.config.ExportFolder;
            if (!Directory.Exists(export)) { export = FileOperate.getExePath(); }
            string reason = "";

            for (int i = Results[indexS].Count - 1; i >= 0; i--)
            {
                PICTURE p = PictureFiles[Results[indexS][i]];
                string sequ = "[" + (i + 1).ToString() + "] ";
                string sour = p.Full;
                string dest = export + "\\" + p.Name;
                
                if (!File.Exists(sour)) { reason += sequ + "文件不存在！\n"; continue; }
                if (File.Exists(dest)) { reason += sequ + "文件已经存在于输出文件夹中！\n"; continue; }

                CloseImage(p.Path, p.Name);

                try { File.Move(sour, dest); } catch { reason += sequ + "移动失败！\n"; continue; }

                Results[indexS].RemoveAt(i);
            }

            ShowSourPic();
            ShowDestPic();
            if (reason.Length != 0) { MessageBox.Show(reason, "移动失败！"); }
        }
        private void RightMenu_ExportPath(object sender, EventArgs e)
        {
            FolderBrowserDialog Folder = new FolderBrowserDialog();

            string SelectedPath = Form_Main.config.ExportFolder;
            if (!Directory.Exists(SelectedPath)) { SelectedPath = Form_Main.config.ConfigPath; }
            Folder.SelectedPath = SelectedPath;

            if (Folder.ShowDialog() != DialogResult.OK) { return; }
            Form_Main.config.ExportFolder = Folder.SelectedPath;
            this.exportPathToolStripMenuItem.Text = Folder.SelectedPath;
        }
        private void RightMenu_Remove(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            int indexS = this.comboBox1.SelectedIndex;
            int indexD = this.listBox1.SelectedIndex;
            if (indexS == -1 || indexD == -1) { MessageBox.Show("文件不存在！", "提示"); return; }
            if (Results.Count <= indexS || Results[indexS].Count <= indexD) { MessageBox.Show("文件不存在！", "提示"); return; }

            PICTURE p = PictureFiles[config.Current[indexD]];
            if (DialogResult.Cancel == MessageBox.Show("把 “" + p.Name  + "” 从结果中移除？", "确认", MessageBoxButtons.OKCancel))
            { return; }

            Results[indexS].RemoveAt(indexD);
        }
        private void RightMenu_Open(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            int indexS = this.comboBox1.SelectedIndex;
            int indexD = this.listBox1.SelectedIndex;
            if (indexS == -1 || indexD == -1) { MessageBox.Show("文件不存在！", "提示"); return; }
            if (Results.Count <= indexS || Results[indexS].Count <= indexD) { MessageBox.Show("文件不存在！", "提示"); return; }

            PICTURE p = PictureFiles[config.Current[indexD]];

            if (!File.Exists(p.Full)) { MessageBox.Show("该文件不存在！", "提示"); return; }
            System.Diagnostics.Process.Start("Explorer", "/select," + p.Full);
        }
        private void RightMenu_Pixes(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

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

            //Stop();
            config.MinCmpPix = MinCmpPixes;
            //Continue();

            Class.Save.settings.Form_Find_Pixes = config.MinCmpPix;
        }
        private void RightMenu_Switch(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            int indexS = this.comboBox1.SelectedIndex;
            int indexD = this.listBox1.SelectedIndex;
            if (indexS == -1 || indexD == -1) { MessageBox.Show("文件不存在！", "提示"); return; }
            if (Results.Count <= indexS || Results[indexS].Count <= indexD) { MessageBox.Show("文件不存在！", "提示"); return; }

            PICTURE p = PictureFiles[config.Current[indexD]];
            //Stop();

            List<int> selectedfolders = new List<int>();
            for (int i = 0; i < this.source2ToolStripMenuItem.DropDownItems.Count; i++)
            {
                ToolStripMenuItem iMenu = (ToolStripMenuItem)this.source2ToolStripMenuItem.DropDownItems[i];
                if (iMenu.Checked) { selectedfolders.Add(i); }
            }
            int selected = 0;
            for (int i = 0; i < p.FolderIndexes.Count; i++)
            {
                if (selectedfolders.IndexOf(p.FolderIndexes[i]) != -1) { selected = i; break; }
            }

            Form_Main.config.FolderIndex = p.FolderIndexes[selected];
            Form_Main.config.FileIndex = p.FileIndexes[selected];
            Form_Main.config.SubIndex = p.SubIndexes[selected];
            IsSwitch = true;
            //this.Close();
        }
        private void RightMenu_Degree(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            //if (!IsFinish) { MessageBox.Show("正在搜索，请勿操作！", "提示"); return; }
            Form_Input input = new Form_Input(config.Degree.ToString());
            input.Location = MousePosition;
            input.ShowDialog();

            double Degree = 0;
            try { Degree = double.Parse(input.Input); } catch { MessageBox.Show("必须输入 0-100 之间的数！", "提示"); return; }
            if (Degree < 0) { MessageBox.Show("必须输入 0-100 之间的数！", "提示"); return; }
            if (Degree > 100) { MessageBox.Show("必须输入 0-100 之间的数！", "提示"); return; }

            //if (!IsFinish) { MessageBox.Show("正在搜索，请勿操作！", "提示"); return; }
            if (config.Degree == (int)Degree) { return; }
            if (IsFinish) { config.Degree = (int)Degree; return; }

            //Stop();
            config.Degree = (int)Degree;
            //Continue();

            Class.Save.settings.Form_Find_Degree = config.Degree;
        }
        private void RightMenu_Restart(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            Stop();
            Start();
        }
        private void RightMenu_Mode(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }
        }
        private void RightMenu_Mode_Full(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            this.fullToolStripMenuItem.Checked = !this.fullToolStripMenuItem.Checked;
            this.partToolStripMenuItem.Checked = !this.fullToolStripMenuItem.Checked;

            SetMode();
        }
        private void RightMenu_Mode_Part(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            this.partToolStripMenuItem.Checked = !this.partToolStripMenuItem.Checked;
            this.fullToolStripMenuItem.Checked = !this.partToolStripMenuItem.Checked;

            SetMode();
        }
        private void RightMenu_Mode_Same(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            this.sameToolStripMenuItem.Checked = !this.sameToolStripMenuItem.Checked;
            this.likeToolStripMenuItem.Checked = !this.sameToolStripMenuItem.Checked;

            SetMode();
        }
        private void RightMenu_Mode_Like(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            this.likeToolStripMenuItem.Checked = !this.likeToolStripMenuItem.Checked;
            this.sameToolStripMenuItem.Checked = !this.likeToolStripMenuItem.Checked;

            SetMode();
        }
        private void RightMenu_Mode_Turn(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }

            this.turnToolStripMenuItem.Checked = !this.turnToolStripMenuItem.Checked;

            SetMode();
        }
        private void RightMenu_Source1(object sender, EventArgs e)
        {
            //if (config.Initializing) { return; }

            this.contextMenuStrip1.Hide();

            bool next = !this.source1ToolStripMenuItem.Checked;
            foreach (ToolStripMenuItem i in this.source1ToolStripMenuItem.DropDownItems)
            {
                i.Checked = next;
            }
        }
        private void RightMenu_Source2(object sender, EventArgs e)
        {
            //if (config.Initializing) { return; }

            this.contextMenuStrip1.Hide();

            bool next = !this.source2ToolStripMenuItem.Checked;
            foreach (ToolStripMenuItem i in this.source2ToolStripMenuItem.DropDownItems)
            {
                i.Checked = next;
            }
        }
        private void RightMenu_Rename(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }
            if (config.Current.Count == 0) { MessageBox.Show("文件不存在！", "提示"); return; }

            string name = System.DateTime.Now.ToLocalTime().ToString();
            name = name.Replace('/', '-');
            name = name.Replace(':', '-');
            if (DialogResult.Cancel == MessageBox.Show("把当前所有文件重命名为：" + name + "？", "确认", MessageBoxButtons.OKCancel))
            { return; }

            try { SourPic.Dispose(); } catch { }
            try { DestPic.Dispose(); } catch { }
            this.pictureBox1.BackgroundImage = null;
            this.pictureBox2.BackgroundImage = null;

            int sourIndex = this.comboBox1.SelectedIndex;
            if (sourIndex != -1) { sourIndex = config.Standard[sourIndex]; }

            for (int i = 0; i < config.Current.Count; i++)
            {
                PICTURE p = PictureFiles[config.Current[i]];

                CloseImage(p.Path, p.Name);

                string sour = p.Full;
                string dest = p.Path + "\\" + name + " " + (i + 1).ToString() + FileOperate.getExtension(p.Name);
                try { File.Move(sour, dest); } catch { continue; }

                p.Path = FileOperate.getPath(dest);
                p.Name = FileOperate.getName(dest);
                p.Full = dest;
                PictureFiles[config.Current[i]] = p;

                this.listBox1.Items[i] =
                    "[" + (i + 1).ToString() + "] [" +
                    (p.Length / 1000).ToString() + " KB] " +
                    (sourIndex == config.Current[i] ? "[Source] " + p.Name : p.Name);
            }

            ShowSourPic();
            ShowDestPic();
        }
        private void RightMenu_Rename2(object sender, EventArgs e)
        {
            if (config.Initializing) { return; }
            if (config.Standard.Count == 0) { MessageBox.Show("文件不存在！", "提示"); return; }
            if (DialogResult.Cancel == MessageBox.Show("把当前所有搜索结果重命名？", "确认", MessageBoxButtons.OKCancel))
            { return; }

            try { SourPic.Dispose(); } catch { }
            try { DestPic.Dispose(); } catch { }
            this.pictureBox1.BackgroundImage = null;
            this.pictureBox2.BackgroundImage = null;


            int sourIndex = this.comboBox1.SelectedIndex;
            int sourSelect = sourIndex;
            if (sourIndex != -1) { sourIndex = config.Standard[sourIndex]; }
            
            string data = DateTime.Now.ToString("yyyy-MM-dd");
            int h = System.DateTime.Now.Hour;
            int m = System.DateTime.Now.Minute;
            int s = System.DateTime.Now.Second;

            string nextName = "";
            string hstr = "";
            string mstr = "";
            string sstr = "";

            for (int i = 0; i < Results.Count; i++)
            {
                s++; if (s > 59) { s = 0; m++; }
                if (m > 59) { m = 0; h++; }
                if (h > 23) { h = 0; }

                sstr = s.ToString(); if (s < 10) { sstr = '0' + sstr; }
                mstr = m.ToString(); if (m < 10) { mstr = '0' + mstr; }
                hstr = h.ToString(); if (h < 10) { hstr = '0' + hstr; }

                nextName = data + " " + hstr + "-" + mstr + "-" + sstr;

                for (int j = 0; j < Results[i].Count; j++)
                {
                    PICTURE p = PictureFiles[Results[i][j]];

                    CloseImage(p.Path, p.Name);

                    string sour = p.Full;
                    string dest = p.Path + "\\" + nextName + " " + (j + 1).ToString() + FileOperate.getExtension(p.Name);
                    try { File.Move(sour, dest); } catch { continue; }

                    p.Path = FileOperate.getPath(dest);
                    p.Name = FileOperate.getName(dest);
                    p.Full = dest;
                    PictureFiles[Results[i][j]] = p;

                    if (i != sourSelect) { continue; }
                    this.listBox1.Items[j] =
                        "[" + (j + 1).ToString() + "] [" +
                        (p.Length / 1000).ToString() + " KB] " +
                        (sourIndex == config.Current[j] ? "[Source] " + p.Name : p.Name);
                }
            }

            ShowSourPic();
            ShowDestPic();
        }

        private void ShowInitial()
        {
            bool isSub = Form_Main.config.IsSub;
            string path = isSub ? Form_Main.config.Path + "\\" + Form_Main.config.Name : Form_Main.config.Path;
            string name = isSub ? Form_Main.config.SubName : Form_Main.config.Name;
            string full = path + "\\" + name;
            int type = isSub ? Form_Main.config.SubType : Form_Main.config.Type;
            if (File.Exists(full) && (FileOperate.IsPicture(type) || FileOperate.IsGif(type)))
            {
                Bitmap temp = (Bitmap)Image.FromFile(full);

                int hBox = this.pictureBox2.Height;
                int wBox = this.pictureBox2.Width;
                int hPic = temp.Height;
                int wPic = temp.Width;

                double rate = Math.Min((double)hBox / hPic, (double)wBox / wPic);
                SourPic = CopyPicture(temp, rate);
                temp.Dispose();

                FileInfo f = new FileInfo(full);

                string size = (f.Length / 1000).ToString();
                this.pictureBox2.BackgroundImage = SourPic;
                this.toolTip2.ToolTipTitle = path;
                this.toolTip2.SetToolTip(this.pictureBox2, "[" + size + " KB] " + name);
            }
            else
            {
                string tempName = name; if (tempName.Length == 0) { tempName = "Unselect File"; }
                path = FileOperate.getExePath();
                name = "err.tip";
                full = path + "\\" + name;
                if (!File.Exists(full)) { return; }

                Bitmap temp = (Bitmap)Image.FromFile(full);

                int hBox = this.pictureBox2.Height;
                int wBox = this.pictureBox2.Width;
                int hPic = temp.Height;
                int wPic = temp.Width;

                double rate = Math.Min((double)hBox / hPic, (double)wBox / wPic);
                SourPic = CopyPicture(temp, rate);
                temp.Dispose();
                
                this.pictureBox2.BackgroundImage = SourPic;
                this.toolTip2.ToolTipTitle = "[Error]: Source file is not a picture or not exist !";
                this.toolTip2.SetToolTip(this.pictureBox2, tempName);
            }
        }
        private void ShowSourPic(int index)
        {
            if (index < 0 || index >= PictureFiles.Count)
            {
                InitPicture(ref SourFile);
                this.pictureBox2.BackgroundImage = null;
                this.toolTip2.ToolTipTitle = null;
                this.toolTip2.SetToolTip(this.pictureBox2, "No Selected Any File");
                return;
            }

            SourFile = PictureFiles[index];
            string fullname = SourFile.Full;
            Bitmap temp;

            if (!File.Exists(fullname)) { fullname = FileOperate.getExePath() + "\\err.tip"; }
            if (!File.Exists(fullname)) { fullname = null; temp = null; }
            else { temp = (Bitmap)Image.FromFile(fullname); }

            if (temp != null)
            {
                int hBox = this.pictureBox2.Height;
                int wBox = this.pictureBox2.Width;
                int hPic = temp.Height;
                int wPic = temp.Width;

                double rate = Math.Min((double)hBox / hPic, (double)wBox / wPic);
                SourPic = CopyPicture(temp, rate);
                temp.Dispose();
            }

            string size = (SourPic == null || SourFile.Length == 0) ? "?" : (SourFile.Length / 1000).ToString();
            this.pictureBox2.BackgroundImage = SourPic;
            this.toolTip2.ToolTipTitle = SourFile.Path;
            this.toolTip2.SetToolTip(this.pictureBox2, "[" + size + " KB] " + SourFile.Name);
        }
        private void ShowSourPic()
        {
            int sel = this.comboBox1.SelectedIndex;
            if (sel < 0 || sel > config.Standard.Count) { ShowSourPic(-1); return; }
            ShowSourPic(config.Standard[sel]);
        }
        private void ShowDestPic(int index)
        {
            if (index < 0 || index >= PictureFiles.Count)
            {
                InitPicture(ref DestFile);
                this.pictureBox1.BackgroundImage = null;
                this.toolTip1.ToolTipTitle = null;
                this.toolTip1.SetToolTip(this.pictureBox1, "No Selected Any File");
                return;
            }

            DestFile = PictureFiles[index];
            string fullname = DestFile.Full;
            Bitmap temp;

            if (!File.Exists(fullname)) { fullname = FileOperate.getExePath() + "\\err.tip"; }
            if (!File.Exists(fullname)) { fullname = null; temp = null; }
            else { temp = (Bitmap)Image.FromFile(fullname); }

            if (temp != null)
            {
                int hBox = this.pictureBox1.Height;
                int wBox = this.pictureBox1.Width;
                int hPic = temp.Height;
                int wPic = temp.Width;

                double rate = Math.Min((double)hBox / hPic, (double)wBox / wPic);
                DestPic = CopyPicture(temp, rate);
                temp.Dispose();
            }

            string sourceTip = SourFile.Full == DestFile.Full ? "[Source] " + DestFile.Name : DestFile.Name;

            string size = (DestPic == null || DestFile.Length == 0) ? "?" : (DestFile.Length / 1000).ToString();
            this.pictureBox1.BackgroundImage = DestPic;
            this.toolTip1.ToolTipTitle = DestFile.Path;
            this.toolTip1.SetToolTip(this.pictureBox1, "[" + size + " KB] " + sourceTip);
        }
        private void ShowDestPic()
        {
            int sel = this.listBox1.SelectedIndex;
            if (sel < 0 || sel > config.Current.Count) { ShowDestPic(-1); return; }
            ShowDestPic(config.Current[sel]);
        }
        private void ShowList()
        {
            int sel = prevIndexS;
            int cnt = Results.Count;
            if (sel < 0 || sel >= cnt) { this.listBox1.Items.Clear(); return; }

            int cntR = Results[sel].Count;
            int cntC = config.Current.Count;
            if (cntR == cntC && cntC == 0) { return; }
            List<int> newCurrent = new List<int>();
            for (int i = 0; i < cntR; i++) { newCurrent.Add(Results[sel][i]); }
            int nLoop = Math.Max(cntC, cntR);
            for (int i = 0; i < nLoop; i++)
            {
                if (i >= cntC) // result 多，current 少
                {
                    string sequence = "[" + (i + 1).ToString() + "] ";
                    string size = "[" + (PictureFiles[newCurrent[i]].Length / 1000).ToString() + " KB] ";
                    string name = newCurrent[i] == config.Standard[this.comboBox1.SelectedIndex] ?
                        "[Source] " + PictureFiles[newCurrent[i]].Name :
                        PictureFiles[newCurrent[i]].Name;
                    config.Current.Add(newCurrent[i]);
                    this.listBox1.Items.Add(sequence + size + name);
                    continue;
                }
                if (i >= cntR) // result 少，current 多
                {
                    this.listBox1.Items.RemoveAt(this.listBox1.Items.Count - 1);
                    config.Current.RemoveAt(config.Current.Count - 1);
                    continue;
                }

                if (config.Current[i] != newCurrent[i])
                {
                    config.Current[i] = newCurrent[i];
                    string sequence = "[" + (i + 1).ToString() + "] ";
                    string size = "[" + (PictureFiles[newCurrent[i]].Length / 1000).ToString() + " KB] ";
                    string name = newCurrent[i] == config.Standard[this.comboBox1.SelectedIndex] ?
                        "[Source] "+ PictureFiles[newCurrent[i]].Name :
                        PictureFiles[newCurrent[i]].Name;
                    this.listBox1.Items[i] = sequence + size + name;
                    continue;
                }
            }

            if (this.listBox1.Items.Count != 0 && this.listBox1.SelectedIndex == -1)
            { this.listBox1.SelectedIndex = 0; }
        }
        private void ShowCombo()
        {
            int cntS = config.Standard.Count;
            int cntC = this.comboBox1.Items.Count;
            if (cntC == cntS && cntS == 0) { return; }
            int nLoop = Math.Max(cntC, cntS);
            for (int i = 0; i < nLoop; i++)
            {
                if (i >= cntS) // standard 少 combo 多
                {
                    this.comboBox1.Items.RemoveAt(this.comboBox1.Items.Count - 1);
                    continue;
                }
                if (i >= cntC) // standard 多 combo 少
                {
                    this.comboBox1.Items.Add(
                        "[" + (i + 1).ToString() + "] " +
                        PictureFiles[config.Standard[i]].Name);
                    continue;
                }

                this.comboBox1.Items[i] =
                    "[" + (i + 1).ToString() + "] " +
                    PictureFiles[config.Standard[i]].Name;
            }
            
            if (this.comboBox1.Items.Count != 0 && this.comboBox1.SelectedIndex == -1)
            { this.comboBox1.SelectedIndex = 0; }
        }

        private void Start()
        {
            Stop();

            bool cmp1 = this.source1ToolStripMenuItem.Checked && !this.source2ToolStripMenuItem.Checked;
            bool cmp2 = this.source1ToolStripMenuItem.Checked && this.source2ToolStripMenuItem.Checked;
            bool cmp0 = !this.source1ToolStripMenuItem.Checked && this.source2ToolStripMenuItem.Checked;

            config.Method = cmp0 ? 0 : (cmp1 ? 1 : (cmp2 ? 2 : -1));

            int type = Form_Main.config.IsSub ? Form_Main.config.SubType : Form_Main.config.Type;
            if (cmp0 && !FileOperate.IsPicture(type) && !FileOperate.IsGif(type))
            { MessageBox.Show("不能比较图片/GIF以外的文件！", "提示"); return; }

            CheckPictureFiles();
            GetSourList();
            GetDestList();

            Results.Clear();
            config.Current.Clear();
            this.listBox1.Items.Clear();
            config.Standard.Clear();
            this.comboBox1.Items.Clear();
            config.TimeBG = config.CountTime;
            IndexS = 0;
            IndexD = 0;
            prevIndexS = -1;
            prevIndexD = -1;

            TH = new Thread(PM.Start);
            PM.Initialize();
            TH.Start();

            this.startToolStripMenuItem.Text = "Stop";
        }
        private void Stop()
        {
            while (TH != null && TH.ThreadState == ThreadState.Running) { PM.Abort = true; }
            this.startToolStripMenuItem.Text = "Continue";
        }
        private void Continue()
        {
            TH = new Thread(PM.Start);
            TH.Start();

            this.startToolStripMenuItem.Text = "Stop";
        }
        
        private void InitializeForm()
        {
            config.Initializing = true;

            IsFinish = false;
            IsSwitch = false;
            Results.Clear();
            IndexS = 0;
            IndexD = 0;

            #region 填充 config
            
            config.Standard = new List<int>();
            config.Current = new List<int>();
            
            if (config.Mode == MODE.DEFAULT) { config.Mode = MODE.FULL_SAME_NOTURN; }
            config.Degree = Class.Load.settings.Form_Find_Degree;
            config.MinCmpPix = Class.Load.settings.Form_Find_Pixes;
            config.Method = -1;

            config.TimeBG = 0;
            config.TimeED = 0;
            config.CountTime = 0;

            #endregion

            #region 初始化模式

            if (((ushort)config.Mode & (ushort)MODE.FULL) > 0) { this.fullToolStripMenuItem.Checked = true; }
            if (((ushort)config.Mode & (ushort)MODE.PART) > 0) { this.partToolStripMenuItem.Checked = true; }
            if (((ushort)config.Mode & (ushort)MODE.SAME) > 0) { this.sameToolStripMenuItem.Checked = true; }
            if (((ushort)config.Mode & (ushort)MODE.LIKE) > 0) { this.likeToolStripMenuItem.Checked = true; }
            if (((ushort)config.Mode & (ushort)MODE.TURN) > 0) { this.turnToolStripMenuItem.Checked = true; }

            #endregion

            #region 填充可选路径
            
            for (int i = 0; i < FileOperate.RootFiles.Count; i++)
            {
                ToolStripMenuItem iMenu = new ToolStripMenuItem(FileOperate.RootFiles[i].Path);
                iMenu.CheckOnClick = true;
                iMenu.Checked = false;
                this.source1ToolStripMenuItem.DropDownItems.Add(iMenu);
            }
            for (int i = 0; i < FileOperate.RootFiles.Count; i++)
            {
                ToolStripMenuItem iMenu = new ToolStripMenuItem(FileOperate.RootFiles[i].Path);
                iMenu.CheckOnClick = true;
                iMenu.Checked = true;
                this.source2ToolStripMenuItem.DropDownItems.Add(iMenu);
            }

            this.source1ToolStripMenuItem.Checked = false;
            this.source2ToolStripMenuItem.Checked = true;

            this.exportPathToolStripMenuItem.Text = (Form_Main.config.ExportFolder == null || Form_Main.config.ExportFolder.Length == 0) ?
                FileOperate.getExePath() :
                Form_Main.config.ExportFolder;

            #endregion

            #region 初始化填充文件内容

            TH = new Thread(GetFiles);
            TH.Start();
            
            #endregion
        }
        private void GetFiles()
        {
            IndexS = 0; // Result 中文件个数
            IndexD = 0; // PictureFiles 中文件个数

            #region 判断文件是否仍然存在
            
            for (int i = PictureFiles.Count - 1; config.Initializing && i >= 0; i--)
            {
                if (!File.Exists(PictureFiles[i].Full)) { PictureFiles.RemoveAt(i); continue; }

                PictureFiles[i].FolderIndexes.Clear();
                PictureFiles[i].FileIndexes.Clear();
                PictureFiles[i].SubIndexes.Clear();
            }

            IndexD = PictureFiles.Count;

            #endregion

            #region 确保每个文件都存在于 PictureFiles 中

            for (int i = 0; config.Initializing && i < FileOperate.RootFiles.Count; i++)
            {
                for (int j = 0; config.Initializing && j < FileOperate.RootFiles[i].Name.Count; j++)
                {
                    string path = FileOperate.RootFiles[i].Path;
                    string name = FileOperate.RootFiles[i].Name[j];
                    int type = FileOperate.getFileType(FileOperate.getExtension(name));
                    int index = -1;

                    #region Folder

                    if (FileOperate.IsFolder(type))
                    {
                        if (!Directory.Exists(path + "\\" + name)) { continue; }

                        List<string> subfiles = FileOperate.getSubFiles(path + "\\" + name);
                        for (int k = 0; config.Initializing && k < subfiles.Count; k++)
                        {
                            type = FileOperate.getFileType(FileOperate.getExtension(subfiles[k]));
                            if (!FileOperate.IsPicture(type) && !FileOperate.IsGif(type))
                            { continue; }

                            index = GetFileIndex(path + "\\" + name + "\\" + subfiles[k]);
                            
                            if (index != -1)
                            {
                                PictureFiles[index].FolderIndexes.Add(i);
                                PictureFiles[index].FileIndexes.Add(j);
                                PictureFiles[index].SubIndexes.Add(k);
                                IndexS++;
                                continue;
                            }

                            PICTURE p = new PICTURE();
                            p.Loaded = false;
                            p.Path = path + "\\" + name;
                            p.Name = subfiles[k];
                            p.Full = p.Path + "\\" + p.Name;
                            p.FolderIndexes = new List<int>(); p.FolderIndexes.Add(i);
                            p.FileIndexes = new List<int>(); p.FileIndexes.Add(j);
                            p.SubIndexes = new List<int>(); p.SubIndexes.Add(k);
                            FileInfo f = new FileInfo(p.Full);
                            p.Length = f.Length;
                            p.Time = f.LastWriteTime.ToFileTime();
                            PictureFiles.Add(p);
                            IndexS++;
                            IndexD++;
                        }
                        continue;
                    }

                    #endregion

                    #region PIC/GIF

                    if (FileOperate.IsPicture(type) || FileOperate.IsGif(type))
                    {
                        if (!File.Exists(path + "\\" + name)) { continue; }

                        index = GetFileIndex(path + "\\" + name);
                        if (index != -1)
                        {
                            PictureFiles[index].FolderIndexes.Add(i);
                            PictureFiles[index].FileIndexes.Add(j);
                            PictureFiles[index].SubIndexes.Add(-1);
                            IndexS++;
                            continue;
                        }

                        PICTURE p = new PICTURE();
                        p.Loaded = false;
                        p.Path = path;
                        p.Name = name;
                        p.Full = p.Path + "\\" + p.Name;
                        p.FolderIndexes = new List<int>(); p.FolderIndexes.Add(i);
                        p.FileIndexes = new List<int>(); p.FileIndexes.Add(j);
                        p.SubIndexes = new List<int>(); p.SubIndexes.Add(-1);
                        FileInfo f = new FileInfo(p.Full);
                        p.Length = f.Length;
                        p.Time = f.LastWriteTime.ToFileTime();
                        PictureFiles.Add(p);
                        IndexS++;
                        IndexD++;
                        continue;
                    }

                    #endregion
                }
            }

            #endregion

            IndexS = 0;
            IndexD = 0;
            config.Initializing = false;
        }
        private void SetMode()
        {
            //if (this.likeToolStripMenuItem.Checked)
            //{
            //    MessageBox.Show("不支持 LIKE 模式的查找", "提示");
            //    this.sameToolStripMenuItem.Checked = true;
            //    this.likeToolStripMenuItem.Checked = false;
            //}

            ushort mode = 0;
            if (this.fullToolStripMenuItem.Checked) { mode |= (ushort)MODE.FULL; }
            if (this.partToolStripMenuItem.Checked) { mode |= (ushort)MODE.PART; }
            if (this.sameToolStripMenuItem.Checked) { mode |= (ushort)MODE.SAME; }
            if (this.likeToolStripMenuItem.Checked) { mode |= (ushort)MODE.LIKE; }
            if (this.turnToolStripMenuItem.Checked) { mode |= (ushort)MODE.TURN; }
            
            config.Mode = (MODE)mode;

            Class.Save.settings.Form_Main_Find_Full = this.fullToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Part = this.partToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Same = this.sameToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Like = this.likeToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Turn = this.turnToolStripMenuItem.Checked;
        }
        private int GetFileIndex(int folder, int file, int sub = -1)
        {
            int index = -1;
            for (int i = 0; i < PictureFiles.Count; i++)
            {
                for (int j = 0; j < PictureFiles[i].FolderIndexes.Count; j++)
                {
                    if (PictureFiles[i].FolderIndexes[j] != folder) { continue; }
                    if (PictureFiles[i].FileIndexes[j] != file) { continue; }

                    if (PictureFiles[i].SubIndexes[j] == -1) { index = i; }
                    if (PictureFiles[i].SubIndexes[j] == 0) { index = i; }
                    if (PictureFiles[i].SubIndexes[j] == sub) { return i; }
                }
            }
            return index;
        }
        private int GetFileIndex(string full)
        {
            if (full == null || full.Length == 0) { return -1; }

            for (int i = 0; i < PictureFiles.Count; i++)
            {
                if (full == PictureFiles[i].Full) { return i; }
            }
            return -1;
        }
        private void GetSourList()
        {
            if (config.Sour == null) { config.Sour = new List<int>(); }
            else { config.Sour.Clear(); }

            if (config.Method == 0)
            {
                int folder = Form_Main.config.FolderIndex;
                int file = Form_Main.config.FileIndex;
                int sub = Form_Main.config.SubIndex;
                int index = GetFileIndex(folder, file, sub);
                if (index != -1) { config.Sour.Add(index); }
                return;
            }
            
            for (int i = 0; i < this.source1ToolStripMenuItem.DropDownItems.Count; i++)
            {
                ToolStripMenuItem iMenu = (ToolStripMenuItem)this.source1ToolStripMenuItem.DropDownItems[i];
                if (!iMenu.Checked) { continue; }

                for (int j = 0; j < PictureFiles.Count; j++)
                {
                    if (PictureFiles[j].FolderIndexes.IndexOf(i) != -1) { config.Sour.Add(j); }
                }
            }
        }
        private void GetDestList()
        {
            if (config.Dest == null) { config.Dest = new List<int>(); }
            else { config.Dest.Clear(); }

            for (int i = 0; i < this.source2ToolStripMenuItem.DropDownItems.Count; i++)
            {
                ToolStripMenuItem iMenu = (ToolStripMenuItem)this.source2ToolStripMenuItem.DropDownItems[i];
                if (!iMenu.Checked) { continue; }

                for (int j = 0; j < PictureFiles.Count; j++)
                {
                    if (PictureFiles[j].FolderIndexes.IndexOf(i) != -1) { config.Dest.Add(j); }
                }
            }
        }
        private void CloseImage(string path, string name)
        {
            for (int i = images.Count - 1; i >= 0; i--)
            {
                if (!images[i].IsDisposed && images[i].Path == path && images[i].Name == name)
                {
                    images[i].Close();
                    images.RemoveAt(i);
                }
            }
        }
        private void CloseImage()
        {
            for (int i = images.Count - 1; i >= 0; i--)
            {
                if (!images[i].IsDisposed)
                {
                    images[i].Close();
                    images.RemoveAt(i);
                }
            }
        }
        private void CheckPictureFiles()
        {
            for (int i = PictureFiles.Count - 1; i >= 0; i--)
            {
                if (!File.Exists(PictureFiles[i].Full)) { PictureFiles.RemoveAt(i); }
            }
        }
        private void InitPicture(ref PICTURE p)
        {
            p.Loaded = false;
            p.Full = "";
            p.Path = "";
            p.Name = "";
            p.FolderIndexes = null;
            p.FileIndexes = null;
            p.SubIndexes = null;
            p.Height = 0;
            p.Width = 0;
            p.Length = 0;
            p.Row = 0;
            p.Col = 0;
            p.GraysR = null;
            p.GraysC = null;
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
    }
}
