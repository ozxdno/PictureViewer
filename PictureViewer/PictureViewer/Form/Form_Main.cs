using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using PictureViewer.Class;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PictureViewer
{
    public partial class Form_Main : Form
    {
        ///////////////////////////////////////////////////// public attribute ///////////////////////////////////////////////

        public static CONFIG config;
        public struct CONFIG
        {
            public string ConfigPath;
            public string ConfigName;
            
            public string ExportFolder;
            public List<string> SubFiles;

            public int FolderIndex;
            public int FileIndex;
            public int SubIndex;

            public Image SourPicture;
            public Image DestPicture;
        }

        ///////////////////////////////////////////////////// private attribute ///////////////////////////////////////////////

        private System.Timers.Timer Timer = new System.Timers.Timer(10);
        private bool MenuShowed_OnWMP = false;
        private bool ShowedBigPicture = false;
        private bool NoHide = false;
        private int LastKeyValue = 35;
        private KEY key;
        private struct KEY
        {
            public bool Down;
            public bool U;
            public bool D;
            public bool L;
            public bool R;
            public bool E;
        }
        
        ///////////////////////////////////////////////////// public method ///////////////////////////////////////////////
        
        public Form_Main()
        {
            InitializeComponent();
        }
        
        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////

        private void Form_Loaded(object sender, EventArgs e)
        {
            config.ConfigPath = FileOperate.getExePath();
            config.ConfigName = "pv.pvini";
            if (!Class.Load.Load_CFG()) { MessageBox.Show("配置文件（pv.pvini）不存在或已损坏"); }

            config.SubFiles = new List<string>();
            if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { config.FolderIndex = 0; }
            if (config.FileIndex < 0 || FileOperate.RootFiles.Count == 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count)
            { config.FileIndex = 0; }

            ShowCurrent();

            for (int i = FileOperate.RootFiles.Count - 1; i >= 0; i--)
            {
                ToolStripMenuItem menu = new ToolStripMenuItem(FileOperate.RootFiles[i].Path);
                menu.Click += RightMenu_Path;
                this.filePathToolStripMenuItem.DropDownItems.Insert(0, menu);
            }
            if (config.FolderIndex >= 0 && config.FolderIndex < FileOperate.RootFiles.Count)
            { ((ToolStripMenuItem)this.filePathToolStripMenuItem.DropDownItems[config.FolderIndex]).Checked = true; }

            this.pathToolStripMenuItem.Text = Directory.Exists(config.ExportFolder) ? config.ExportFolder : config.ConfigPath;
            
            Timer.Elapsed += new System.Timers.ElapsedEventHandler(Updata);
            Timer.AutoReset = true;
            Timer.Start();
        }
        private void Form_Closed(object sender, FormClosedEventArgs e)
        {
            Timer.Close(); Class.Save.Save_CFG();
        }
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            #region 上下左右翻动滚动条

            if (e.KeyValue == 37) { key.L = true; }
            if (e.KeyValue == 38) { key.U = true; }
            if (e.KeyValue == 39) { key.R = true; }
            if (e.KeyValue == 40) { key.D = true; }
            LastKeyValue = e.KeyValue;
            key.Down = key.L || key.R || key.U || key.D;
            if (e.KeyValue < 41 && e.KeyValue > 36 && !this.lockToolStripMenuItem.Checked && (this.HorizontalScroll.Visible || this.VerticalScroll.Visible)) { return; }

            #endregion

            #region 强制上一页

            if (e.KeyValue == 17)
            {
                if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
                if (config.FileIndex < 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { return; }

                if (config.SubFiles != null && config.SubFiles.Count != 0) { config.SubIndex--; if (config.SubIndex < 0) { config.FileIndex--; config.SubIndex = -1; } }
                else { config.FileIndex--; config.SubIndex = -1; }
                if (config.FileIndex < 0) { config.FolderIndex--; }
                if (config.FolderIndex < 0)
                {
                    MessageBox.Show("已经是第一个文件了！", "提示");
                    config.FolderIndex = 0;
                    config.FileIndex = 0;
                    config.SubIndex = 0; return;
                }

                if (config.FileIndex < 0) { config.FileIndex = FileOperate.RootFiles[config.FolderIndex].Name.Count - 1; }
                string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
                int type = FileOperate.getFileType(FileOperate.getExtension(name));

                if (type == 1)
                {
                    string path = FileOperate.RootFiles[config.FolderIndex].Path;
                    List<string> files = FileOperate.getSubFiles(path + "\\" + name);
                    if (config.SubIndex == -1) { config.SubIndex = files.Count - 1; }
                }
                ShowCurrent(); key.E = false;
            }

            #endregion

            #region 强制下一页
            if (e.KeyValue == 35)
            {
                if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
                if (config.FileIndex < 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { return; }

                if (config.SubFiles != null && config.SubIndex < config.SubFiles.Count - 1) { config.SubIndex++; if (config.SubIndex >= config.SubFiles.Count) { config.FileIndex++; config.SubIndex = -1; } }
                else { config.FileIndex++; config.SubIndex = -1; }
                if (config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { config.FolderIndex++; config.FileIndex = 0; }
                if (config.FolderIndex >= FileOperate.RootFiles.Count)
                {
                    MessageBox.Show("已经是最后一个文件了！", "提示");
                    config.FolderIndex = FileOperate.RootFiles.Count - 1;
                    config.FileIndex = FileOperate.RootFiles[config.FolderIndex].Name.Count - 1;
                    config.SubIndex = config.SubFiles == null ? 0 : config.SubFiles.Count - 1;
                    return;
                }
                
                string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
                int type = FileOperate.getFileType(FileOperate.getExtension(name));
                if (LastKeyValue != e.KeyValue && type != 1) { config.SubIndex = 0; }
                if (type == 1 && config.SubIndex == -1) { config.SubIndex = 0; }
                
                ShowCurrent(); key.E = false; return;
            }

            #endregion

            #region 前一个

            if (e.KeyValue == 37)
            {
                if (FileOperate.RootFiles.Count == 0) { return; }
                
                config.FileIndex--; if (config.FileIndex < 0) { config.FolderIndex--; }
                if (config.FolderIndex < 0)
                { config.FolderIndex = FileOperate.RootFiles.Count - 1; }
                if (config.FileIndex < 0)
                { config.FileIndex = FileOperate.RootFiles[config.FolderIndex].Name.Count - 1; }

                config.SubFiles = new List<string>(); config.SubIndex = 0; ShowCurrent();
            }

            #endregion

            #region 后一个
            if (e.KeyValue == 39)
            {
                if (FileOperate.RootFiles.Count == 0) { return; }
                
                config.FileIndex++;
                if (config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count)
                { config.FileIndex = 0; config.FolderIndex++; }
                if (config.FolderIndex >= FileOperate.RootFiles.Count) { config.FolderIndex = 0; }

                config.SubFiles = new List<string>(); config.SubIndex = 0; ShowCurrent();
            }

            #endregion

            #region 向上

            if (e.KeyValue == 38)
            {
                if (config.SubFiles == null || config.SubFiles.Count == 0) { return; }
                config.SubIndex--;
                if (config.SubIndex < 0) { config.SubIndex = config.SubFiles.Count - 1; }

                ShowCurrent();
            }

            #endregion

            #region 向下

            if (e.KeyValue == 40)
            {
                if (config.SubFiles == null || config.SubFiles.Count == 0) { return; }
                config.SubIndex++;
                if (config.SubIndex >= config.SubFiles.Count) { config.SubIndex = 0; }

                ShowCurrent();
            }

            #endregion

            #region 回车

            if (e.KeyValue == 13)
            {
                if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
                if (config.FileIndex < 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { return; }

                string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
                int type = FileOperate.getFileType(FileOperate.getExtension(name));

                bool lastE = key.E; key.E = false;

                #region 1

                if (type == 1)
                {
                    if (config.SubIndex < 0 || config.SubIndex >= config.SubFiles.Count) { return; }
                    name = config.SubFiles[config.SubIndex];
                    type = FileOperate.getFileType(FileOperate.getExtension(name));
                    if (type != 2) { ShowCurrent(); return; }

                    if (config.SourPicture == null) { return; }
                    if (ShowedBigPicture) { ShowCurrent(); return; }

                    // 聚焦点
                    //int xF = MousePosition.X - this.Location.X - this.pictureBox1.Location.X;
                    //int yF = MousePosition.Y - this.Location.Y - this.pictureBox1.Location.Y;

                    this.HorizontalScroll.Value = 0;
                    this.VerticalScroll.Value = 0;
                    ShowedBigPicture = true;

                    int X = (this.Width - 18 - config.SourPicture.Width) / 2;
                    int Y = (this.Height - 42 - config.SourPicture.Height) / 2;
                    key.E = X < 0 || Y < 0;
                    if (X < 1) { X = 1; }
                    if (Y < 1) { Y = 1; }

                    this.pictureBox1.Image = config.SourPicture;
                    this.pictureBox1.Location = new Point(X, Y);
                    this.pictureBox1.Height = config.SourPicture.Height;
                    this.pictureBox1.Width = config.SourPicture.Width; return;

                    // 把聚焦点放到屏幕中央
                    //int max = this.HorizontalScroll.Maximum;
                    //int min = this.HorizontalScroll.Minimum;
                    //double xRate = (double)xF / config.DestPicture.Width;
                    //if (xRate < 0) { xRate = 0; }
                    //if (xRate > 1) { xRate = 1; }
                    //int xS = (int)(min + xRate * (max - min)) - this.Width / 2;
                    //if (xS < 0) { xS = 0; }
                    //max = this.VerticalScroll.Maximum;
                    //min = this.VerticalScroll.Minimum;
                    //double yRate = (double)yF / config.DestPicture.Height;
                    //if (yRate < 0) { yRate = 0; }
                    //if (yRate > 1) { yRate = 1; }
                    //int yS = (int)(min + yRate * (max - min)) - this.Height / 2;
                    //if (yS < 0) { yS = 0; }
                    //if (X == 1) { this.HorizontalScroll.Value = xS; }
                    //if (Y == 1) { this.VerticalScroll.Value = yS; }
                    //if (X == 1) { this.HorizontalScroll.Value = xS; }
                    //if (Y == 1) { this.VerticalScroll.Value = yS; }
                }

                #endregion

                #region 2

                if (type == 2)
                {
                    if (config.SourPicture == null) { return; }
                    if (ShowedBigPicture) { ShowCurrent(); return; }

                    this.HorizontalScroll.Value = 0;
                    this.VerticalScroll.Value = 0;
                    ShowedBigPicture = true;

                    // 聚焦点
                    //int xF = MousePosition.X - this.Location.X - this.pictureBox1.Location.X;
                    //int yF = MousePosition.Y - this.Location.Y - this.pictureBox1.Location.Y;

                    int X = (this.Width - 18 - config.SourPicture.Width) / 2;
                    int Y = (this.Height - 42 - config.SourPicture.Height) / 2;
                    key.E = X < 0 || Y < 0;
                    if (X < 1) { X = 1; }
                    if (Y < 1) { Y = 1; }
                    
                    this.pictureBox1.Image = config.SourPicture;
                    this.pictureBox1.Location = new Point(X, Y);
                    this.pictureBox1.Height = config.SourPicture.Height;
                    this.pictureBox1.Width = config.SourPicture.Width; return;

                    // 把聚焦点放到屏幕中央
                    //int max = this.HorizontalScroll.Maximum;
                    //int min = this.HorizontalScroll.Minimum;
                    //double xRate = (double)xF / config.DestPicture.Width;
                    //if (xRate < 0) { xRate = 0; }
                    //if (xRate > 1) { xRate = 1; }
                    //int xS = (int)(min + xRate * (max - min)) - this.Width / 2;
                    //if (xS < 0) { xS = 0; }
                    //max = this.VerticalScroll.Maximum;
                    //min = this.VerticalScroll.Minimum;
                    //double yRate = (double)yF / config.DestPicture.Height;
                    //if (yRate < 0) { yRate = 0; }
                    //if (yRate > 1) { yRate = 1; }
                    //int yS = (int)(min + yRate * (max - min)) - this.Height / 2;
                    //if (yS < 0) { yS = 0; }
                    //if (X == 1) { this.HorizontalScroll.Value = xS; }
                    //if (Y == 1) { this.VerticalScroll.Value = yS; }
                    //if (X == 1) { this.HorizontalScroll.Value = xS; }
                    //if (Y == 1) { this.VerticalScroll.Value = yS; }
                }

                #endregion

                #region 3

                if (type == 3) { ShowCurrent(); return; }

                #endregion

                #region 4

                if (type == 4)
                {
                    ShowCurrent(); return;
                    //this.axWindowsMediaPlayer1.Height = this.Height - 41;
                    //this.axWindowsMediaPlayer1.Width = this.Width - 18;
                    //this.axWindowsMediaPlayer1.Ctlcontrols.play(); return;
                }

                #endregion

                #region 5

                if (type == 5)
                {
                    if (config.SubIndex < 0 || config.SubIndex >= config.SubFiles.Count) { return; }
                    name = config.SubFiles[config.SubIndex];
                    type = FileOperate.getFileType(FileOperate.getExtension(name));
                    if (type != 2) { ShowCurrent(); return; }

                    if (config.SourPicture == null) { return; }
                    if (ShowedBigPicture) { ShowCurrent(); return; }

                    // 聚焦点
                    //int xF = MousePosition.X - this.Location.X - this.pictureBox1.Location.X;
                    //int yF = MousePosition.Y - this.Location.Y - this.pictureBox1.Location.Y;

                    this.HorizontalScroll.Value = 0;
                    this.VerticalScroll.Value = 0;
                    ShowedBigPicture = true;

                    int X = (this.Width - 18 - config.SourPicture.Width) / 2;
                    int Y = (this.Height - 42 - config.SourPicture.Height) / 2;
                    key.E = X < 0 || Y < 0;
                    if (X < 1) { X = 1; }
                    if (Y < 1) { Y = 1; }

                    this.pictureBox1.Image = config.SourPicture;
                    this.pictureBox1.Location = new Point(X, Y);
                    this.pictureBox1.Height = config.SourPicture.Height;
                    this.pictureBox1.Width = config.SourPicture.Width; return;

                    // 把聚焦点放到屏幕中央
                    //int max = this.HorizontalScroll.Maximum;
                    //int min = this.HorizontalScroll.Minimum;
                    //double xRate = (double)xF / config.DestPicture.Width;
                    //if (xRate < 0) { xRate = 0; }
                    //if (xRate > 1) { xRate = 1; }
                    //int xS = (int)(min + xRate * (max - min));
                    //xRate = (double)this.Width / 2 / (double)config.SourPicture.Width;
                    //xS -= (int)(xRate * (max - min));
                    //if (xS < 0) { xS = 0; }
                    //if (xS > max) { xS = max; }
                    //max = this.VerticalScroll.Maximum;
                    //min = this.VerticalScroll.Minimum;
                    //double yRate = (double)yF / config.DestPicture.Height;
                    //if (yRate < 0) { yRate = 0; }
                    //if (yRate > 1) { yRate = 1; }
                    //int yS = (int)(min + yRate * (max - min));
                    //yRate = (double)this.Height / 2 / (double)config.SourPicture.Width;
                    //yS -= (int)(yRate * (max - min));
                    //if (yS < 0) { yS = 0; }
                    //if (yS > max) { yS = max; }

                    // 调整滑动量，使其可以被接受
                    //if (X == 1)
                    //{
                    //    int adjust = xS - this.HorizontalScroll.Value;
                    //    int maxChange = this.HorizontalScroll.LargeChange;
                    //    while (true)
                    //    {
                    //        if (adjust > maxChange) { this.HorizontalScroll.Value += maxChange; adjust = xS - this.HorizontalScroll.Value; continue; }
                    //        if (adjust < -maxChange) { this.HorizontalScroll.Value -= maxChange; adjust = xS - this.HorizontalScroll.Value; continue; }
                    //        this.HorizontalScroll.Value = xS; break;
                    //    }
                    //}
                    //if (Y == 1)
                    //{
                    //    int adjust = yS - this.VerticalScroll.Value;
                    //    int maxChange = this.VerticalScroll.LargeChange;
                    //    while (true)
                    //    {
                    //        if (adjust > maxChange) { this.VerticalScroll.Value += maxChange; adjust = yS - this.VerticalScroll.Value; continue; }
                    //        if (adjust < -maxChange) { this.VerticalScroll.Value -= maxChange; adjust = yS - this.VerticalScroll.Value; continue; }
                    //        this.VerticalScroll.Value = yS; break;
                    //    }
                    //}
                    //this.HorizontalScroll.Value = xS;
                    //this.VerticalScroll.Value = yS; return;
                }

                #endregion

                #region 其他

                ShowCurrent();

                #endregion
            }

            #endregion

            #region 删除

            if (e.KeyValue == 46)
            {
                RightMenu_Export(0, null);
            }

            #endregion

            #region 打开根目录

            if (e.KeyValue == 50)
            {
                bool ExistFolder = 0 <= Form_Main.config.FolderIndex && Form_Main.config.FolderIndex < FileOperate.RootFiles.Count;
                //bool ExistFile = ExistFolder && (0 <= Form_Main.config.FileIndex && Form_Main.config.FileIndex < FileOperate.RootFiles[Form_Main.config.FolderIndex].Name.Count);
                if (!ExistFolder) { MessageBox.Show("路径不存在，无法打开！", "提示"); return; }

                string root = FileOperate.RootFiles[config.FolderIndex].Path;
                System.Diagnostics.Process.Start("explorer.exe", root);
            }

            #endregion

            #region 打开子目录

            if (e.KeyValue == 51)
            {
                bool ExistFolder = 0 <= Form_Main.config.FolderIndex && Form_Main.config.FolderIndex < FileOperate.RootFiles.Count;
                bool ExistFile = ExistFolder && (0 <= Form_Main.config.FileIndex && Form_Main.config.FileIndex < FileOperate.RootFiles[Form_Main.config.FolderIndex].Name.Count);
                if (!ExistFile) { MessageBox.Show("路径不存在，无法打开！", "提示"); return; }
                
                string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
                int type = FileOperate.getFileType(FileOperate.getExtension(name));
                if (type != 1) { MessageBox.Show("该文件不是文件夹，无法打开！", "提示"); return; }

                string file = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
                System.Diagnostics.Process.Start("explorer.exe", file);
            }

            #endregion

            #region 打开输出目录

            if (e.KeyValue == 49)
            {
                string export = config.ExportFolder;
                if (!Directory.Exists(export)) { export = config.ConfigPath; }
                System.Diagnostics.Process.Start("explorer.exe", export);
            }

            #endregion

            #region PASSWORD

            if (e.KeyValue == 80)
            {
                // 计算输入框位置
                int X = this.Location.X + this.Width / 2;
                int Y = this.Location.Y + this.Height / 2; 

                // 给出输入框
                Form_Input input = new Form_Input();
                input.Location = new Point(X, Y);
                input.ShowDialog();

                // 判断指令是否正确
                if (input.Input == "#show") { ShowCurrentFolder(); return; }
                if (input.Input == "#hide") { HideCurrentFolder(); return; }
                if (input.Input == "#show2") { ShowFiles(); return; }
                if (input.Input == "#hide2") { HideFiles(); return; }
                if (input.Input == "#show hide") { NoHide = true; ShowCurrent(); return; }
                if (input.Input == "#hide hide") { NoHide = false; ShowCurrent();  return; }
                if (input.Input.Length != 0 && input.Input[0] != '-')
                { ZipOperate.A_PassWord(input.Input); ShowCurrent(); }
                if (input.Input.Length != 0 && input.Input[0] == '-')
                { ZipOperate.D_PassWord(input.Input); }
            }

            #endregion

            #region 退出

            if (e.KeyValue == 27) { this.Close(); }

            #endregion
        }
        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 37) { key.L = false; }
            if (e.KeyValue == 38) { key.U = false; }
            if (e.KeyValue == 39) { key.R = false; }
            if (e.KeyValue == 40) { key.D = false; }
            key.Down = key.L || key.R || key.U || key.D;
        }
        private void WMP_RightButtonClicked(object sender, AxWMPLib._WMPOCXEvents_MouseDownEvent e)
        {
            if (!MenuShowed_OnWMP) { this.contextMenuStrip1.Show(MousePosition); }
            if (MenuShowed_OnWMP) { this.contextMenuStrip1.Hide(); }

            MenuShowed_OnWMP = !MenuShowed_OnWMP;
        }

        private void Form_Main_DoubleClick(object sender, EventArgs e)
        {
            if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
            if (config.FileIndex < 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { return; }

            string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
            int type = FileOperate.getFileType(FileOperate.getExtension(name));

            bool lastE = key.E; key.E = false;

            #region 1 型文件的双击操作

            if (type == 1)
            {
                if (config.SubIndex < 0 || config.SubIndex >= config.SubFiles.Count) { ShowCurrent(); return; }
                name = config.SubFiles[config.SubIndex];
                type = FileOperate.getFileType(FileOperate.getExtension(name));
                if (type != 2) { ShowCurrent(); return; }

                if (config.SourPicture == null) { return; }
                if (ShowedBigPicture) { ShowCurrent(); return; }

                // 聚焦点
                int xF = MousePosition.X - this.Location.X - this.pictureBox1.Location.X;
                int yF = MousePosition.Y - this.Location.Y - this.pictureBox1.Location.Y;
                double xR = (double)xF / config.DestPicture.Width;
                if (xR < 0) { xR = 0; }
                if (xR > 1) { xR = 1; }
                double yR = (double)yF / config.DestPicture.Height;
                if (yR < 0) { yR = 0; }
                if (yR > 1) { yR = 1; }

                //this.HorizontalScroll.Value = 0;
                //this.VerticalScroll.Value = 0;
                ShowedBigPicture = true;

                int X = (this.Width - 18 - config.SourPicture.Width) / 2;
                int Y = (this.Height - 42 - config.SourPicture.Height) / 2;
                key.E = X < 0 || Y < 0;
                if (X < 1) { X = 1; }
                if (Y < 1) { Y = 1; }

                this.pictureBox1.Image = config.SourPicture;
                this.pictureBox1.Location = new Point(X, Y);
                this.pictureBox1.Height = config.SourPicture.Height;
                this.pictureBox1.Width = config.SourPicture.Width;

                // 把聚焦点放到屏幕中央
                int xS = (int)(config.SourPicture.Width * xR - this.Width / 2 + 40);
                if (xS < this.HorizontalScroll.Minimum) { xS = this.HorizontalScroll.Minimum; }
                if (xS > this.HorizontalScroll.Maximum) { xS = this.HorizontalScroll.Maximum; }

                int yS = (int)(config.SourPicture.Height * yR - this.Height / 2 + 20);
                if (yS < this.VerticalScroll.Minimum) { yS = this.VerticalScroll.Minimum; }
                if (yS > this.VerticalScroll.Maximum) { yS = this.VerticalScroll.Maximum; }

                // 调整滑动量，使其可以被接受
                if (X == 1)
                {
                    int adjust = xS - this.HorizontalScroll.Value;
                    int maxChange = this.HorizontalScroll.LargeChange;
                    while (true)
                    {
                        if (adjust > maxChange) { this.HorizontalScroll.Value += maxChange; adjust = xS - this.HorizontalScroll.Value; continue; }
                        if (adjust < -maxChange) { this.HorizontalScroll.Value -= maxChange; adjust = xS - this.HorizontalScroll.Value; continue; }
                        this.HorizontalScroll.Value = xS; break;
                    }
                }
                if (Y == 1)
                {
                    int adjust = yS - this.VerticalScroll.Value;
                    int maxChange = this.VerticalScroll.LargeChange;
                    while (true)
                    {
                        if (adjust > maxChange) { this.VerticalScroll.Value += maxChange; adjust = yS - this.VerticalScroll.Value; continue; }
                        if (adjust < -maxChange) { this.VerticalScroll.Value -= maxChange; adjust = yS - this.VerticalScroll.Value; continue; }
                        this.VerticalScroll.Value = yS; break;
                    }
                }
                this.HorizontalScroll.Value = xS;
                this.VerticalScroll.Value = yS; return;
            }

            #endregion

            #region 2 型文件的双击操作

            if (type == 2)
            {
                if (config.SourPicture == null) { return; }
                if (ShowedBigPicture) { ShowCurrent(); return; }

                // 聚焦点
                int xF = MousePosition.X - this.Location.X - this.pictureBox1.Location.X - 10;
                int yF = MousePosition.Y - this.Location.Y - this.pictureBox1.Location.Y - 30;
                double xR = (double)xF / config.DestPicture.Width;
                if (xR < 0) { xR = 0; }
                if (xR > 1) { xR = 1; }
                double yR = (double)yF / config.DestPicture.Height;
                if (yR < 0) { yR = 0; }
                if (yR > 1) { yR = 1; }

                //this.HorizontalScroll.Value = 0;
                //this.VerticalScroll.Value = 0;
                ShowedBigPicture = true;

                int X = (this.Width - 18 - config.SourPicture.Width) / 2;
                int Y = (this.Height - 42 - config.SourPicture.Height) / 2;
                key.E = X < 0 || Y < 0;
                if (X < 1) { X = 1; }
                if (Y < 1) { Y = 1; }
                
                this.pictureBox1.Height = config.SourPicture.Height;
                this.pictureBox1.Width = config.SourPicture.Width;
                this.pictureBox1.Location = new Point(X, Y);
                this.pictureBox1.Image = config.SourPicture;

                // 把聚焦点放到屏幕中央
                int xS = (int)(config.SourPicture.Width * xR - this.Width / 2 + 40);
                if (xS < this.HorizontalScroll.Minimum) { xS = this.HorizontalScroll.Minimum; }
                if (xS > this.HorizontalScroll.Maximum) { xS = this.HorizontalScroll.Maximum; }

                int yS = (int)(config.SourPicture.Height * yR - this.Height / 2 + 20);
                if (yS < this.VerticalScroll.Minimum) { yS = this.VerticalScroll.Minimum; }
                if (yS > this.VerticalScroll.Maximum) { yS = this.VerticalScroll.Maximum; }


                #region 调整滑动量，使其可以被接受

                if (X == 1)
                {
                    int adjust = xS - this.HorizontalScroll.Value;
                    int maxChange = this.HorizontalScroll.LargeChange;
                    while (true)
                    {
                        if (adjust > maxChange) { this.HorizontalScroll.Value += maxChange; adjust = xS - this.HorizontalScroll.Value; continue; }
                        if (adjust < -maxChange) { this.HorizontalScroll.Value -= maxChange; adjust = xS - this.HorizontalScroll.Value; continue; }
                        this.HorizontalScroll.Value = xS; break;
                    }
                }
                if (Y == 1)
                {
                    int adjust = yS - this.VerticalScroll.Value;
                    int maxChange = this.VerticalScroll.LargeChange;
                    while (true)
                    {
                        if (adjust > maxChange) { this.VerticalScroll.Value += maxChange; adjust = yS - this.VerticalScroll.Value; continue; }
                        if (adjust < -maxChange) { this.VerticalScroll.Value -= maxChange; adjust = yS - this.VerticalScroll.Value; continue; }
                        this.VerticalScroll.Value = yS; break;
                    }
                }
                this.HorizontalScroll.Value = xS;
                this.VerticalScroll.Value = yS; return;

                #endregion
            }

            #endregion

            #region 3 型文件的双击操作

            if (type == 3)
            {
                ShowCurrent(); return;
            }

            #endregion

            #region 4 型文件的双击操作

            if (type == 4)
            {
                ShowCurrent(); return;

                //this.axWindowsMediaPlayer1.Height = this.Height - 41;
                //this.axWindowsMediaPlayer1.Width = this.Width - 18;
                //this.axWindowsMediaPlayer1.Ctlcontrols.play(); return;
            }

            #endregion

            #region 5 型文件的双击操作

            if (type == 5)
            {
                if (config.SubIndex < 0 || config.SubIndex >= config.SubFiles.Count) { ShowCurrent(); return; }
                name = config.SubFiles[config.SubIndex];
                type = FileOperate.getFileType(FileOperate.getExtension(name));
                if (type != 2) { ShowCurrent(); return; }

                if (config.SourPicture == null) { return; }
                if (ShowedBigPicture) { ShowCurrent(); return; }

                // 聚焦点
                int xF = MousePosition.X - this.Location.X - this.pictureBox1.Location.X;
                int yF = MousePosition.Y - this.Location.Y - this.pictureBox1.Location.Y;
                double xR = (double)xF / config.DestPicture.Width;
                if (xR < 0) { xR = 0; }
                if (xR > 1) { xR = 1; }
                double yR = (double)yF / config.DestPicture.Height;
                if (yR < 0) { yR = 0; }
                if (yR > 1) { yR = 1; }

                //this.HorizontalScroll.Value = 0;
                //this.VerticalScroll.Value = 0;
                ShowedBigPicture = true;

                int X = (this.Width - 18 - config.SourPicture.Width) / 2;
                int Y = (this.Height - 42 - config.SourPicture.Height) / 2;
                key.E = X < 0 || Y < 0;
                if (X < 1) { X = 1; }
                if (Y < 1) { Y = 1; }

                this.pictureBox1.Image = config.SourPicture;
                this.pictureBox1.Location = new Point(X, Y);
                this.pictureBox1.Height = config.SourPicture.Height;
                this.pictureBox1.Width = config.SourPicture.Width;

                // 把聚焦点放到屏幕中央
                int xS = (int)(config.SourPicture.Width * xR - this.Width / 2 + 40);
                if (xS < this.HorizontalScroll.Minimum) { xS = this.HorizontalScroll.Minimum; }
                if (xS > this.HorizontalScroll.Maximum) { xS = this.HorizontalScroll.Maximum; }

                int yS = (int)(config.SourPicture.Height * yR - this.Height / 2 + 20);
                if (yS < this.VerticalScroll.Minimum) { yS = this.VerticalScroll.Minimum; }
                if (yS > this.VerticalScroll.Maximum) { yS = this.VerticalScroll.Maximum; }

                // 调整滑动量，使其可以被接受
                if (X == 1)
                {
                    int adjust = xS - this.HorizontalScroll.Value;
                    int maxChange = this.HorizontalScroll.LargeChange;
                    while (true)
                    {
                        if (adjust > maxChange) { this.HorizontalScroll.Value += maxChange; adjust = xS - this.HorizontalScroll.Value; continue; }
                        if (adjust < -maxChange) { this.HorizontalScroll.Value -= maxChange; adjust = xS - this.HorizontalScroll.Value; continue; }
                        this.HorizontalScroll.Value = xS; break;
                    }
                }
                if (Y == 1)
                {
                    int adjust = yS - this.VerticalScroll.Value;
                    int maxChange = this.VerticalScroll.LargeChange;
                    while (true)
                    {
                        if (adjust > maxChange) { this.VerticalScroll.Value += maxChange; adjust = yS - this.VerticalScroll.Value; continue; }
                        if (adjust < -maxChange) { this.VerticalScroll.Value -= maxChange; adjust = yS - this.VerticalScroll.Value; continue; }
                        this.VerticalScroll.Value = yS; break;
                    }
                }
                this.HorizontalScroll.Value = xS;
                this.VerticalScroll.Value = yS; return;
            }

            #endregion

            #region 其他文件双击操作

            ShowCurrent();

            #endregion
        }
        private void Page_U(object sender, EventArgs e)
        {
            if (config.SubFiles == null || config.SubFiles.Count == 0) { return; }
            config.SubIndex--;
            if (config.SubIndex < 0) { config.SubIndex = config.SubFiles.Count - 1; }

            ShowCurrent();
        }
        private void Page_D(object sender, EventArgs e)
        {
            if (config.SubFiles == null || config.SubFiles.Count == 0) { return; }
            config.SubIndex++;
            if (config.SubIndex >= config.SubFiles.Count) { config.SubIndex = 0; }

            ShowCurrent();
        }
        private void Page_L(object sender, EventArgs e)
        {
            if (FileOperate.RootFiles.Count == 0) { return; }

            config.FileIndex--; if (config.FileIndex < 0) { config.FolderIndex--; }
            if (config.FolderIndex < 0)
            { config.FolderIndex = FileOperate.RootFiles.Count - 1; }
            if (config.FileIndex < 0)
            { config.FileIndex = FileOperate.RootFiles[config.FolderIndex].Name.Count - 1; }

            config.SubFiles = new List<string>(); config.SubIndex = 0; ShowCurrent();
        }
        private void Page_R(object sender, EventArgs e)
        {
            if (FileOperate.RootFiles.Count == 0) { return; }

            config.FileIndex++;
            if (config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count)
            { config.FileIndex = 0; config.FolderIndex++; }
            if (config.FolderIndex >= FileOperate.RootFiles.Count) { config.FolderIndex = 0; }

            config.SubFiles = new List<string>(); config.SubIndex = 0; ShowCurrent();
        }

        private void Updata(object source, System.Timers.ElapsedEventArgs e)
        {
            this.BeginInvoke((EventHandler)delegate
            {
                bool hideU = this.hideUToolStripMenuItem.Checked;
                bool hideD = this.hideDToolStripMenuItem.Checked;
                bool hideL = this.hideLToolStripMenuItem.Checked;
                bool hideR = this.hideRToolStripMenuItem.Checked;
                this.hideToolStripMenuItem.Checked = hideL && hideR && hideU && hideD;

                int ptX = MousePosition.X - this.Location.X;
                int ptY = MousePosition.Y - this.Location.Y;
                bool showPageMark = this.Width > 150 && this.Height > 150;

                // 左翻页
                int setW = this.Width / 20;
                int setH = this.Height / 5;
                int font = setW * 3 / 4;
                int bgW = this.Width / 20;
                int edW = bgW + setW;
                int bgH = this.Height / 2 - setH / 2;
                int edH = bgH + setH;
                if (showPageMark && !hideL && bgW <= ptX && ptX <= edW && bgH <= ptY && ptY <= edH)
                {
                    this.label1.Location = new Point(bgW - 10, bgH - 30);
                    this.label1.Width = setW;
                    this.label1.Height = setH;
                    this.label1.Visible = true;
                    this.label1.Font = new Font("宋体", font);
                }
                else { this.label1.Visible = false; }
                // 右翻页
                setW = this.Width / 20;
                setH = this.Height / 5;
                font = setW * 3 / 4;
                bgW = this.Width - setW - this.Width / 20;
                if (this.VerticalScroll.Visible) { bgW -= 12; }
                edW = bgW + setW;
                bgH = this.Height / 2 - setH / 2;
                edH = bgH + setH;
                if (showPageMark && !hideR && bgW <= ptX && ptX <= edW && bgH <= ptY && ptY <= edH)
                {
                    this.label2.Location = new Point(bgW - 10, bgH - 30);
                    this.label2.Width = setW;
                    this.label2.Height = setH;
                    this.label2.Visible = true;
                    this.label2.Font = new Font("宋体", font);
                }
                else { this.label2.Visible = false; }
                // 上翻页
                setW = this.Width / 8;
                setH = this.Height / 12;
                font = setH * 3 / 4;
                bgW = this.Width / 2 - setW / 2;
                edW = bgW + setW;
                bgH = this.Height / 20 + 30;
                edH = bgH + setH;
                if (showPageMark && !hideU && bgW <= ptX && ptX <= edW && bgH <= ptY && ptY <= edH && config.SubFiles.Count != 0)
                {
                    this.label3.Location = new Point(bgW - 10, bgH - 30);
                    this.label3.Width = setW;
                    this.label3.Height = setH;
                    this.label3.Visible = true;
                    this.label3.Font = new Font("宋体", font);
                }
                else { this.label3.Visible = false; }
                // 下翻页
                setW = this.Width / 8;
                setH = this.Height / 12;
                font = setH * 3 / 4;
                bgW = this.Width / 2 - setW / 2;
                edW = bgW + setW;
                bgH = this.Height - this.Height / 20 - setH - 10;
                if (this.HorizontalScroll.Visible) { bgH -= 10; }
                edH = bgH + setH;
                if (showPageMark && !hideD && bgW <= ptX && ptX <= edW && bgH <= ptY && ptY <= edH && config.SubFiles.Count != 0)
                {
                    this.label4.Location = new Point(bgW - 10, bgH - 30);
                    this.label4.Width = setW;
                    this.label4.Height = setH;
                    this.label4.Visible = true;
                    this.label4.Font = new Font("宋体", font);
                }
                else { this.label4.Visible = false; }

                // 滚屏
                if (!key.Down || (!this.HorizontalScroll.Visible && !this.VerticalScroll.Visible)) { return; }
                if (this.lockToolStripMenuItem.Checked) { return; }

                if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
                if (config.FileIndex < 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { return; }

                //string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
                //int type = FileOperate.getFileType(FileOperate.getExtension(name));
                //if (type != 1 && type != 2) { return; }

                if (key.L) { KeyDown_Enter_L(); }
                if (key.R) { KeyDown_Enter_R(); }
                if (key.U) { KeyDown_Enter_U(); }
                if (key.D) { KeyDown_Enter_D(); }
            });
        }
        private void KeyDown_Enter_U()
        {
            if (!this.VerticalScroll.Visible) { return; }
            int max = this.VerticalScroll.Maximum;
            int min = this.VerticalScroll.Minimum;
            int value = this.VerticalScroll.Value;

            value -= 5;
            if (value > max) { value = max; }
            if (value < min) { value = min; }

            this.VerticalScroll.Value = value;
        }
        private void KeyDown_Enter_D()
        {
            if (!this.VerticalScroll.Visible) { return; }
            int max = this.VerticalScroll.Maximum;
            int min = this.VerticalScroll.Minimum;
            int value = this.VerticalScroll.Value;

            value += 5;
            if (value > max) { value = max; }
            if (value < min) { value = min; }

            this.VerticalScroll.Value = value;
        }
        private void KeyDown_Enter_L()
        {
            if (!this.HorizontalScroll.Visible) { return; }
            int max = this.HorizontalScroll.Maximum;
            int min = this.HorizontalScroll.Minimum;
            int value = this.HorizontalScroll.Value;

            value -= 5;
            if (value > max) { value = max; }
            if (value < min) { value = min; }

            this.HorizontalScroll.Value = value;
        }
        private void KeyDown_Enter_R()
        {
            if (!this.HorizontalScroll.Visible) { return; }
            int max = this.HorizontalScroll.Maximum;
            int min = this.HorizontalScroll.Minimum;
            int value = this.HorizontalScroll.Value;

            value += 5;
            if (value > max) { value = max; }
            if (value < min) { value = min; }

            this.HorizontalScroll.Value = value;
        }

        private void ShowCurrent()
        {
            // 关闭播放以前的文件
            if (config.SourPicture != null) { config.SourPicture.Dispose(); }
            if (config.DestPicture != null) { config.DestPicture.Dispose(); }
            if (!this.axWindowsMediaPlayer1.IsDisposed) { this.axWindowsMediaPlayer1.Ctlcontrols.stop(); }

            this.HorizontalScroll.Value = 0;
            this.VerticalScroll.Value = 0;

            ShowedBigPicture = false;
            config.SubFiles.Clear();

            // 切到当前文件夹
            for (int i = 0; i < this.filePathToolStripMenuItem.DropDownItems.Count; i++)
            {
                ToolStripMenuItem imenu = (ToolStripMenuItem)this.filePathToolStripMenuItem.DropDownItems[i];
                imenu.Checked = i == config.FolderIndex;
            }

            // 获取下一个文件
            bool Exist = true;
            if (config.FolderIndex < 0 || config.FileIndex < 0) { Exist = false; }
            if (Exist && config.FolderIndex >= FileOperate.RootFiles.Count) { Exist = false; }
            if (Exist && config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { Exist = false; }

            // 给出提示
            string Name = Exist ? FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex] : "[Not Exist]";
            string Index = (config.FileIndex + 1).ToString();
            string Total = FileOperate.RootFiles.Count == 0 || config.FolderIndex >= FileOperate.RootFiles.Count ?
                "0" : FileOperate.RootFiles[config.FolderIndex].Name.Count.ToString();
            this.Text = "[" + Index + "/" + Total + "] " + Name;
            if (!Exist) { ShowOff("err"); return; }

            // 读取文件
            string path = FileOperate.RootFiles[config.FolderIndex].Path;
            string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
            string full = path + "\\" + name;
            int type = FileOperate.getFileType(FileOperate.getExtension(name));

            bool isHide = FileOperate.IsSupportHide(FileOperate.getExtension(name));
            if(isHide && !NoHide) { this.Text = "[" + Index + "/" + Total + "] [Unknow] " + Name; ShowOff("unk"); return; }

            // 处理不同文件
            #region 0 型文件
            if (type == 0)
            {
                if (!File.Exists(full)) { this.Text = "[" + Index + "/" + Total + "] [Not Exist]"; ShowOff("err"); }
                this.Text = "[" + Index + "/" + Total + "] [Unsupport] " + Name; ShowOff("unp"); return;
            }
            #endregion

            #region 1 型文件（文件夹）

            if (type == 1)
            {
                if (!Directory.Exists(full)) { this.Text = "[" + Index + "/" + Total + "] [Not Exist] " + full; ShowOff("err"); return; }
                
                config.SubFiles = FileOperate.getSubFiles(full);
                if (config.SubIndex < 0 || config.SubIndex >= config.SubFiles.Count)
                {
                    this.Text = "[" + Index + "/" + Total + "] [" + (config.SubIndex + 1).ToString() + "/" + config.SubFiles.Count + "] [Not Exist]";
                    ShowOff("err"); return;
                }

                isHide = FileOperate.IsSupportHide(FileOperate.getExtension(config.SubFiles[config.SubIndex]));
                if (isHide && !NoHide)
                { this.Text = "[" + Index + "/" + Total + "] [" + (config.SubIndex + 1).ToString() + "/" + config.SubFiles.Count + "] [Unknow] " + name + "：" + config.SubFiles[config.SubIndex]; ShowOff("unk"); return; }

                this.Text = "[" + Index + "/" + Total + "] [" + (config.SubIndex + 1).ToString() + "/" + config.SubFiles.Count + "] " + name + "：" + config.SubFiles[config.SubIndex];

                name = config.SubFiles[config.SubIndex];
                type = FileOperate.getFileType(FileOperate.getExtension(name));

                if (type == 2) { ShowPicture(full, name); return; }
                if (type == 3) { ShowGif(full, name); return; }
                if (type == 4) { ShowVideo(full, name); return; }
                this.Text = "[" + Index + "/" + Total + "] [" + (config.SubIndex + 1).ToString() + "/" + config.SubFiles.Count + "] [Unsupport] " + Name + "：" + config.SubFiles[config.SubIndex];
                ShowOff("unp"); return;
            }

            #endregion

            #region 2 型文件（图片）

            if (type == 2)
            {
                if (!File.Exists(full)) { this.Text = "[" + Index + "/" + Total + "] [Not Exist] " + name; ShowOff("err"); return; }
                ShowPicture(path, name); return;
            }

            #endregion

            #region 3 型文件（GIF）

            if (type == 3)
            {
                if (!File.Exists(full)) { this.Text = "[" + Index + "/" + Total + "] [Not Exist] " + name; ShowOff("err"); return; }
                ShowGif(path,name); return;
            }

            #endregion

            #region 4 型文件（视频）

            if (type == 4)
            {
                if (!File.Exists(full)) { this.Text = "[" + Index + "/" + Total + "] [Not Exist] " + name; ShowOff("err"); return; }
                ShowVideo(path,name); return;
            }

            #endregion

            #region 5 型文件（ZIP）

            if (type == 5)
            {
                if (!File.Exists(full)) { this.Text = "[" + Index + "/" + Total + "] [Not Exist] " + name; ShowOff("err"); return; }

                int err = ZipOperate.ReadZipEX(full);
                if (!isHide && err == 1) { this.Text = "[" + Index + "/" + Total + "] [Wrong PassWord] " + name; ShowOff("err"); return; }
                
                if (config.SubIndex < 0 || config.SubIndex >= config.SubFiles.Count)
                {
                    this.Text = "[" + Index + "/" + Total + "] [" + (config.SubIndex + 1).ToString() + "/" + config.SubFiles.Count + "] [Not Exist]";
                    ShowOff("err"); return;
                }

                isHide = FileOperate.IsSupportHide(FileOperate.getExtension(config.SubFiles[config.SubIndex]));
                if (isHide && !NoHide)
                { this.Text = "[" + Index + "/" + Total + "] [" + (config.SubIndex + 1).ToString() + "/" + config.SubFiles.Count + "] [Unknow] " + name + "：" + config.SubFiles[config.SubIndex]; ShowOff("unk"); return; }
                
                this.Text = "[" + Index + "/" + Total + "] [" + (config.SubIndex + 1).ToString() + "/" + config.SubFiles.Count + "] " + name + "：" + config.SubFiles[config.SubIndex];

                name = config.SubFiles[config.SubIndex];
                type = FileOperate.getFileType(FileOperate.getExtension(name));

                if (type == 2 && ZipOperate.LoadPictureEX()) { ShowPicture(full, name, false); return; }
                if (type == 3 && ZipOperate.LoadGifEX()) { ShowGif(full, name, false); return; }

                this.Text = "[" + Index + "/" + Total + "] [" + (config.SubIndex + 1).ToString() + "/" + config.SubFiles.Count + "] [Unsupport] " + Name + "：" + config.SubFiles[config.SubIndex];
                ShowOff("unp"); return;
            }

            #endregion

            #region 其他文件（不支持）

            this.Text = "[" + Index + "/" + Total + "] [Unknow] " + name; ShowOff("unk");

            #endregion
        }
        private void ShowPicture(string path, string name, bool load = true)
        {
            if (load) { config.SourPicture = Image.FromFile(path + "\\" + name); }
            
            int sourX = config.SourPicture.Width;
            int sourY = config.SourPicture.Height;
            int destX = sourX;
            int destY = sourY;
            int limitX = this.Width - 18;
            int limitY = this.Height - 41;
            
            double rate = Math.Max((double)sourX / limitX, (double)sourY / limitY);
            if (rate > 1) { destX = (int)(config.SourPicture.Width / rate); }
            if (rate > 1) { destY = (int)(config.SourPicture.Height / rate); }

            config.DestPicture = (Image)new Bitmap(destX, destY);
            Graphics g = Graphics.FromImage(config.DestPicture);
            g.DrawImage(config.SourPicture, new Rectangle(0, 0, destX, destY), new Rectangle(0, 0, sourX, sourY), GraphicsUnit.Pixel);
            g.Dispose();
            
            this.pictureBox1.Width = destX;
            this.pictureBox1.Height = destY;
            this.pictureBox1.Location = new Point((this.Width - 18 - destX) / 2, (this.Height - 42 - destY) / 2);
            this.pictureBox1.Image = config.DestPicture;
            
            this.axWindowsMediaPlayer1.Visible = false;
            this.pictureBox1.Visible = true;
        }
        private void ShowGif(string path, string name, bool load = true)
        {
            if (load) { config.SourPicture = Image.FromFile(path + "\\" + name); }
            
            int H = config.SourPicture.Height;
            int W = config.SourPicture.Width;
            int X = (this.Width - W - 18) / 2;
            int Y = (this.Height - H - 42) / 2;
            key.E = X < 0 || Y < 0;
            if (X < 1) { X = 1; }
            if (Y < 1) { Y = 1; }

            this.pictureBox1.Height = config.SourPicture.Height;
            this.pictureBox1.Width = config.SourPicture.Width;
            this.pictureBox1.Image = config.SourPicture;
            this.pictureBox1.Location = new Point(X, Y);
            
            this.axWindowsMediaPlayer1.Visible = false;
            this.pictureBox1.Visible = true;
        }
        private void ShowVideo(string path, string name)
        {
            this.axWindowsMediaPlayer1.Height = this.Height - 41;
            this.axWindowsMediaPlayer1.Width = this.Width - 18;
            this.axWindowsMediaPlayer1.Location = new Point(1, 1);

            axWindowsMediaPlayer1.URL = path + "\\" + name;
            axWindowsMediaPlayer1.Ctlcontrols.play();

            this.axWindowsMediaPlayer1.Visible = true;
            this.pictureBox1.Visible = false;
        }
        private void ShowOff(string type = null)
        {
            if (type == "unk") { ShowUnk(); return; }
            if (type == "unp") { ShowUnp(); return; }
            if (type == "err") { ShowErr(); return; }

            this.pictureBox1.Height = this.Height - 41;
            this.pictureBox1.Width = this.Width - 18;
            this.pictureBox1.Location = new Point(1, 1);
            this.pictureBox1.Visible = false;

            this.axWindowsMediaPlayer1.Visible = false;
        }
        private void ShowUnk()
        {
            string unkpath = FileOperate.getExePath();
            string unkname = "unk.tip";
            if (!File.Exists(unkpath + "\\" + unkname))
            {
                this.pictureBox1.Height = this.Height - 41;
                this.pictureBox1.Width = this.Width - 18;
                this.pictureBox1.Location = new Point(1, 1);
                this.pictureBox1.Visible = false;
                this.axWindowsMediaPlayer1.Visible = false;
                return;
            }
            ShowPicture(unkpath, unkname);
        }
        private void ShowUnp()
        {
            string unppath = FileOperate.getExePath();
            string unpname = "unp.tip";
            if (!File.Exists(unppath + "\\" + unpname))
            {
                this.pictureBox1.Height = this.Height - 41;
                this.pictureBox1.Width = this.Width - 18;
                this.pictureBox1.Location = new Point(1, 1);
                this.pictureBox1.Visible = false;
                this.axWindowsMediaPlayer1.Visible = false;
                return;
            }
            ShowPicture(unppath, unpname);
        }
        private void ShowErr()
        {
            string errpath = FileOperate.getExePath();
            string errname = "err.tip";
            if (!File.Exists(errpath + "\\" + errname))
            {
                this.pictureBox1.Height = this.Height - 41;
                this.pictureBox1.Width = this.Width - 18;
                this.pictureBox1.Location = new Point(1, 1);
                this.pictureBox1.Visible = false;
                this.axWindowsMediaPlayer1.Visible = false;
                return;
            }
            ShowPicture(errpath, errname);
        }

        private void HideFiles()
        {
            // 关断当前
            try { this.axWindowsMediaPlayer1.Ctlcontrols.stop(); } catch { }
            try { config.SourPicture.Dispose(); } catch { }
            try { config.DestPicture.Dispose(); } catch { }

            this.axWindowsMediaPlayer1.Visible = false;
            this.pictureBox1.Visible = false;

            // 更改文件后缀
            for (int i = 0; i < FileOperate.RootFiles.Count; i++) { FileOperate.HideFiles(FileOperate.RootFiles[i].Path); }

            // 重新加载文件
            Class.Save.Save_CFG();
            Class.Load.Load_CFG();

            // 刷新列表
            for (int i = FileOperate.RootFiles.Count - 1; i >= 0; i--)
            {
                ToolStripMenuItem menu = new ToolStripMenuItem(FileOperate.RootFiles[i].Path);
                menu.Click += RightMenu_Path;
                this.filePathToolStripMenuItem.DropDownItems.Insert(0, menu);
            }

            // 显示
            if (FileOperate.RootFiles.Count > 0)
            { ((ToolStripMenuItem)this.filePathToolStripMenuItem.DropDownItems[0]).Checked = true; }
            ShowCurrent();
        }
        private void ShowFiles()
        {
            // 更改文件后缀
            for (int i = 0; i < FileOperate.RootFiles.Count; i++) { FileOperate.ShowFiles(FileOperate.RootFiles[i].Path); }

            // 重新加载文件
            //int folder = config.FolderIndex, file = config.FileIndex, sub = config.SubIndex;
            //Class.Load.Load_CFG();
            //config.FolderIndex = folder; config.FileIndex = file; config.SubIndex = sub;
            Class.Save.Save_CFG();
            Class.Load.Load_CFG();
            
            // 刷新列表
            for (int i = FileOperate.RootFiles.Count - 1; i >= 0; i--)
            {
                ToolStripMenuItem menu = new ToolStripMenuItem(FileOperate.RootFiles[i].Path);
                menu.Click += RightMenu_Path;
                this.filePathToolStripMenuItem.DropDownItems.Insert(0, menu);
            }

            // 显示
            if (FileOperate.RootFiles.Count > 0)
            { ((ToolStripMenuItem)this.filePathToolStripMenuItem.DropDownItems[0]).Checked = true; }
            ShowCurrent();
        }
        private void HideCurrentFolder()
        {
            // 关断当前
            try { this.axWindowsMediaPlayer1.Ctlcontrols.stop(); } catch { }
            try { config.SourPicture.Dispose(); } catch { }
            try { config.DestPicture.Dispose(); } catch { }

            this.axWindowsMediaPlayer1.Visible = false;
            this.pictureBox1.Visible = false;

            // 更改文件后缀
            if (config.FolderIndex < 0) { return; }
            if (config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
            FileOperate.HideFiles(FileOperate.RootFiles[config.FolderIndex].Path);

            // 重新加载文件
            int folder = config.FolderIndex, file = config.FileIndex, sub = config.SubIndex;
            FileOperate.Cover(FileOperate.RootFiles[config.FolderIndex].Path);
            config.FolderIndex = folder; config.FileIndex = file; config.SubIndex = sub;

            // 显示
            ShowCurrent();
        }
        private void ShowCurrentFolder()
        {
            // 更改文件后缀
            if (config.FolderIndex < 0) { return; }
            if (config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
            FileOperate.ShowFiles(FileOperate.RootFiles[config.FolderIndex].Path);

            // 重新加载文件
            int folder = config.FolderIndex, file = config.FileIndex, sub = config.SubIndex;
            FileOperate.Cover(FileOperate.RootFiles[config.FolderIndex].Path);
            config.FolderIndex = folder; config.FileIndex = file; config.SubIndex = sub;

            // 显示
            ShowCurrent();
        }

        private void RightMenu_Refresh(object sender, EventArgs e)
        {
            ShowCurrent();
        }
        private void RightMenu_Input(object sender, EventArgs e)
        {
            string path = FileOperate.Input(); if (path == "") { return; }

            foreach(ToolStripMenuItem imenu in this.filePathToolStripMenuItem.DropDownItems)
            { imenu.Checked = false; }

            ToolStripMenuItem menu = new ToolStripMenuItem(path);
            menu.Checked = true;
            menu.Click += RightMenu_Path;
            this.filePathToolStripMenuItem.DropDownItems.Add(menu);
            
            ShowCurrent();
        }
        private void RightMenu_Goto(object sender, EventArgs e)
        {
            Form_Input input = new Form_Input();
            input.Location = MousePosition;
            input.ShowDialog();

            string indexStr = input.Input.Replace(" ", "");
            string subindexStr = "0";
            if (indexStr.Length == 0) { return; }

            int cut = indexStr.IndexOf(':');
            if (cut == -1) { cut = indexStr.IndexOf('：'); }
            if (cut != -1) { subindexStr = indexStr.Substring(cut + 1); indexStr = indexStr.Substring(0, cut); }
            if (indexStr.Length == 0) { indexStr = (config.FileIndex + 1).ToString(); }

            int index = 0, subindex = 0;
            try { index = int.Parse(indexStr); } catch { MessageBox.Show("输入错误位置！"); return; }
            try { subindex = int.Parse(subindexStr); } catch { MessageBox.Show("输入错误位置！"); return; }
            
            if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
            if (index < 1) { index = 1; }
            if (index > FileOperate.RootFiles[config.FolderIndex].Name.Count) { index = FileOperate.RootFiles[config.FolderIndex].Name.Count; }
            if (subindex < 1) { subindex = 1; }
            //if (subindex > config.SubFiles.Count) { subindex = config.SubFiles.Count; }

            if (index - 1 == config.FileIndex && config.SubIndex == subindex - 1) { return; }
            if (index - 1 == config.FileIndex && config.SubFiles.Count == 0) { return; }

            config.FileIndex = index - 1; config.SubIndex = subindex - 1;
            ShowCurrent();
        }
        private void RightMenu_Delete(object sender, EventArgs e)
        {
            int indexofSelect = -1; bool found = false;
            foreach (ToolStripMenuItem imenu in this.filePathToolStripMenuItem.DropDownItems)
            { indexofSelect++; if (imenu.Checked) { found = true; break; } }
            if (!found) { return; }

            if (DialogResult.Cancel == MessageBox.Show("把 “" + FileOperate.RootFiles[config.FolderIndex].Path + "” 移出当前浏览？", "确认移出", MessageBoxButtons.OKCancel))
            { return; }

            this.filePathToolStripMenuItem.DropDownItems.RemoveAt(indexofSelect);
            FileOperate.RootFiles.RemoveAt(indexofSelect);

            indexofSelect--;
            if (indexofSelect < 0) { indexofSelect = 0; }
            if (indexofSelect >= FileOperate.RootFiles.Count) { indexofSelect = FileOperate.RootFiles.Count - 1; }

            if (indexofSelect > 0)
            { ((ToolStripMenuItem)this.filePathToolStripMenuItem.DropDownItems[indexofSelect]).Checked = true; }

            config.FolderIndex = indexofSelect;
            config.FileIndex = 0;
            ShowCurrent();
        }
        private void RightMenu_Path(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem imenu in this.filePathToolStripMenuItem.DropDownItems)
            { imenu.Checked = false; }

            ToolStripMenuItem menu = sender as ToolStripMenuItem;
            menu.Checked = true;
            string rootpath = menu.Text;
            int indexofSelect = FileOperate.Search(rootpath);

            if (indexofSelect == config.FolderIndex) { return; }
            config.FolderIndex = indexofSelect;
            config.FileIndex = 0;
            ShowCurrent();
        }
        private void RightMenu_Export(object sender, EventArgs e)
        {
            // 判断是否存在文件
            if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
            if (config.FileIndex < 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { return; }

            this.contextMenuStrip1.Hide();
            
            // 获取输出文件进行输出
            string path = FileOperate.RootFiles[config.FolderIndex].Path;
            string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
            int type = FileOperate.getFileType(FileOperate.getExtension(name));

            // 是否已经存在文件或文件夹
            string expath = config.ExportFolder;
            if (!Directory.Exists(expath)) { expath = config.ConfigPath; }

            bool Exist = false;
            if (type == 1) { Exist = Directory.Exists(expath + "\\" + name); }
            if (type != 1) { Exist = File.Exists(expath + "\\" + name); }
            if (Exist) { MessageBox.Show("本文件已经存在于 “"+ expath + "” 中！"); return; }

            // 确认是否输出
            if (DialogResult.Cancel == MessageBox.Show("把 “" + name + "” 导出？", "确认导出", MessageBoxButtons.OKCancel))
            { return; }

            // 释放资源
            //if (this.axWindowsMediaPlayer1 != null) { this.axWindowsMediaPlayer1.Dispose(); }
            if (config.SourPicture != null) { config.SourPicture.Dispose(); }
            if (config.DestPicture != null) { config.DestPicture.Dispose(); }

            // 输出文件
            if (type != 1)
            {
                File.Move(path + "\\" + name, expath + "\\" + name);
                FileOperate.RootFiles[config.FolderIndex].Name.RemoveAt(config.FileIndex);
                ShowCurrent();
            }
            if (type == 1)
            {
                string newPath = expath + "\\" + name;
                Directory.CreateDirectory(newPath);

                for (int i = 0; i < config.SubFiles.Count; i++)
                { File.Move(path + "\\" + name + "\\" + config.SubFiles[i], newPath + "\\" + config.SubFiles[i]); }
                Directory.Delete(path + "\\" + name);
                FileOperate.RootFiles[config.FolderIndex].Name.RemoveAt(config.FileIndex);
                ShowCurrent();
            }
        }
        private void RightMenu_Export_Path(object sender, EventArgs e)
        {
            FolderBrowserDialog Folder = new FolderBrowserDialog();

            string SelectedPath = Form_Main.config.ExportFolder;
            if (!Directory.Exists(SelectedPath)) { SelectedPath = Form_Main.config.ConfigPath; }
            Folder.SelectedPath = SelectedPath;

            if (Folder.ShowDialog() != DialogResult.OK) { return; }
            config.ExportFolder = Folder.SelectedPath;
            this.pathToolStripMenuItem.Text = config.ExportFolder;
        }
        private void RightMenu_Rename(object sender, EventArgs e)
        {
            // 判断是否存在文件
            if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
            if (config.FileIndex < 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { return; }

            // 判断当前文件类型
            string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
            int type = FileOperate.getFileType(FileOperate.getExtension(name));
            if (type != 1) { MessageBox.Show("只有文件夹允许重命名！"); return; }

            // 获取源路径
            string sourpath = FileOperate.RootFiles[config.FolderIndex].Path;
            string sourname = name;

            // 给出输入框
            Form_Input input = new Form_Input(sourname);
            input.Location = MousePosition;
            input.ShowDialog();

            // 名称相同，无需重命名
            if (input.Input == sourname) { return; }

            // 点号被保留
            if (input.Input.IndexOf('.') >= 0) { MessageBox.Show("文件夹重命名失败！"); return; }

            // 更改文件夹名称
            DirectoryInfo dir = new DirectoryInfo(sourpath + "\\" + sourname);
            if (config.SourPicture != null) { config.SourPicture.Dispose(); }
            if (config.DestPicture != null) { config.DestPicture.Dispose(); }
            try { dir.MoveTo(sourpath + "\\" + input.Input); } catch { MessageBox.Show("文件夹重命名失败！"); ShowCurrent(); return; }

            // 更新当前文件夹名称
            FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex] = input.Input;
            ShowCurrent();
        }
        private void RightMenu_Search(object sender, EventArgs e)
        {
            Form_Search search = new Form_Search();
            //search.Location = MousePosition;
            search.ShowDialog();
            if (search.Cancle) { return; }

            config.FolderIndex = search.FolderIndex;
            config.FileIndex = search.FileIndex;
            config.SubIndex = 0;
            ShowCurrent();
        }
        private void RightMenu_OpenExport(object sender, EventArgs e)
        {
            string export = config.ExportFolder;
            if (!Directory.Exists(export)) { export = config.ConfigPath; }
            System.Diagnostics.Process.Start("explorer.exe", export);
        }
        private void RightMenu_OpenRoot(object sender, EventArgs e)
        {
            bool ExistFolder = 0 <= Form_Main.config.FolderIndex && Form_Main.config.FolderIndex < FileOperate.RootFiles.Count;
            //bool ExistFile = ExistFolder && (0 <= Form_Main.config.FileIndex && Form_Main.config.FileIndex < FileOperate.RootFiles[Form_Main.config.FolderIndex].Name.Count);
            if (!ExistFolder) { MessageBox.Show("路径不存在，无法打开！", "提示"); return; }

            string root = FileOperate.RootFiles[config.FolderIndex].Path;
            System.Diagnostics.Process.Start("explorer.exe", root);
        }
        private void RightMenu_OpenComic(object sender, EventArgs e)
        {
            bool ExistFolder = 0 <= Form_Main.config.FolderIndex && Form_Main.config.FolderIndex < FileOperate.RootFiles.Count;
            bool ExistFile = ExistFolder && (0 <= Form_Main.config.FileIndex && Form_Main.config.FileIndex < FileOperate.RootFiles[Form_Main.config.FolderIndex].Name.Count);
            if (!ExistFile) { MessageBox.Show("路径不存在，无法打开！", "提示"); return; }

            string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
            int type = FileOperate.getFileType(FileOperate.getExtension(name));
            if (type != 1) { MessageBox.Show("该文件不是文件夹，无法打开！", "提示"); return; }

            string file = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
            System.Diagnostics.Process.Start("explorer.exe", file);
        }
        private void RightMenu_Hide(object sender, EventArgs e)
        {
            bool curr = this.hideToolStripMenuItem.Checked;
            bool next = !curr;

            this.hideUToolStripMenuItem.Checked = next;
            this.hideDToolStripMenuItem.Checked = next;
            this.hideLToolStripMenuItem.Checked = next;
            this.hideRToolStripMenuItem.Checked = next;

            this.hideToolStripMenuItem.Checked = next;
        }

        private void Form_DragEntre(object sender, DragEventArgs e)
        {
            string filename = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            int type = FileOperate.getFileType(FileOperate.getExtension(filename));
            if (type != 2 && type != 3 && type != 4) { e.Effect = DragDropEffects.None; }
            else { e.Effect = DragDropEffects.Link; }
        }
        private void Form_DragDrop(object sender, DragEventArgs e)
        {
            // 判断文件夹是否有效
            if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
            if (config.FileIndex < 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { return; }

            // 判断移动的目标路径
            string destpath = FileOperate.RootFiles[config.FolderIndex].Path;
            string destname = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
            int desttype = FileOperate.getFileType(FileOperate.getExtension(destname));
            if (desttype == 1) { destpath += "\\" + destname; }

            string sourfile = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            int indexofcut = sourfile.LastIndexOf('\\');
            string sourname = sourfile.Substring(indexofcut + 1);
            int sourtype = FileOperate.getFileType(FileOperate.getExtension(sourname));
            if (desttype == 1 && sourtype == 1) { MessageBox.Show("子文件夹中不能再存文件夹！"); return; }

            // 开始移动文件
            try { File.Move(sourfile, destpath + "\\" + sourname); } catch { MessageBox.Show("移动失败！"); return; }

            if (desttype == 1) { config.SubFiles.Insert(config.SubIndex,sourname); }
            if (desttype != 1) { FileOperate.RootFiles[config.FolderIndex].Name.Insert(config.FileIndex, sourname); }
            ShowCurrent();
        }
    }
}
