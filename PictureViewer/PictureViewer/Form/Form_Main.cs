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

        /// <summary>
        /// 显示隐藏文件
        /// </summary>
        public static bool NoHide = false;

        public static CONFIG config;
        public struct CONFIG
        {
            /// <summary>
            /// 计数器
            /// </summary>
            public ulong Time;

            /// <summary>
            /// 配置文件所在路径
            /// </summary>
            public string ConfigPath;
            /// <summary>
            /// 配置文件文件名
            /// </summary>
            public string ConfigName;
            /// <summary>
            /// 输出的目标文件夹
            /// </summary>
            public string ExportFolder;
            /// <summary>
            /// 某一文件夹下的文件名（支持文件）
            /// </summary>
            public List<string> SubFiles;

            /// <summary>
            /// 当前文件的所属文件夹序号
            /// </summary>
            public int FolderIndex;
            /// <summary>
            /// 当前文件在所属文件夹下的序号
            /// </summary>
            public int FileIndex;
            /// <summary>
            /// 当前文件的类型为文件或者ZIP压缩文件才存在该项，指示该文件夹下的子项目序号
            /// </summary>
            public int SubIndex;

            /// <summary>
            /// 文件夹是否存在
            /// </summary>
            public bool ExistFolder;
            /// <summary>
            /// 文件是否存在
            /// </summary>
            public bool ExistFile;
            /// <summary>
            /// 子文件是否存在
            /// </summary>
            public bool ExistSub;

            /// <summary>
            /// 当前文件的文件路径（所属文件夹）
            /// </summary>
            public string Path;
            /// <summary>
            /// 当前文件的文件名
            /// </summary>
            public string Name;
            /// <summary>
            /// 当前文件的后缀
            /// </summary>
            public string Extension;
            /// <summary>
            /// 当前文件的类型
            /// </summary>
            public int Type;
            /// <summary>
            /// 当前文件的是否具有隐藏属性（后缀为隐藏后缀）
            /// </summary>
            public bool Hide;
            /// <summary>
            /// 是否为文件夹类型文件
            /// </summary>
            public bool IsSub;
            /// <summary>
            /// 子文件文件名
            /// </summary>
            public string SubName;
            /// <summary>
            /// 子文件文件类型
            /// </summary>
            public int SubType;
            /// <summary>
            /// 子文件文件后缀
            /// </summary>
            public string SubExtension;
            /// <summary>
            /// 子文件是否具有隐藏属性（后缀为隐藏后缀）
            /// </summary>
            public bool SubHide;

            /// <summary>
            /// 源图（大图，不经过等比例改变大小）
            /// </summary>
            public Image SourPicture;
            /// <summary>
            /// 适应图（小图，改变了大小去匹配窗体的大小）
            /// </summary>
            public Image DestPicture;

            /// <summary>
            /// 错误类型：
            /// 0 - 无错误；
            /// 1 - 读取 ZIP 文件时密码错误；
            /// </summary>
            public int Error;
        }

        ///////////////////////////////////////////////////// private attribute ///////////////////////////////////////////////

        // H 39 W 16

        private System.Timers.Timer Timer = new System.Timers.Timer(10);
        private bool MenuShowed_OnWMP = false;
        //private bool ShowedBigPicture = false;
        private bool NextShowBigPicture = false;
        private int LastKeyValue = 35;
        private bool UseSmallWindowOpen = false;
        private bool UseBoard = true;
        private bool UseShapeWindow = false;
        private int ShapeWindowRate = 80;
        private bool TipForInput = false;
        private MOUSE mouse;
        private KEY key;
        private struct MOUSE
        {
            public bool Down;
            public bool Up;
            public Point pDown;
            public Point pUp;
            public Point Previous;
            public ulong tDown;
            public ulong tUp;
            public uint nDown;
            public uint nUp;
        }
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

            this.pictureBox1.MouseWheel += Form_MouseWheel;
        }
        
        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////

        private void Form_Loaded(object sender, EventArgs e)
        {
            FileSupport.Initialize();
            
            config.ConfigPath = FileOperate.getExePath();
            config.ConfigName = "pv.pvini";
            //if (!Class.Load.Load_CFG()) { MessageBox.Show("配置文件（pv.pvini）不存在或已损坏"); }
            Class.Load.Load_CFG();

            this.hideToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Hide;
            this.hideUToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Hide_U;
            this.hideDToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Hide_D;
            this.hideLToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Hide_L;
            this.hideRToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Hide_R;

            this.fullToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Full;
            this.partToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Part;
            this.sameToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Same;
            this.likeToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Like;
            this.turnToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Turn;

            if (!Class.Load.settings.Form_Main_UseBoard) { this.FormBorderStyle = FormBorderStyle.None; }
            UseBoard = Class.Load.settings.Form_Main_UseBoard;
            UseShapeWindow = Class.Load.settings.Form_Main_ShapeWindow;
            ShapeWindowRate = Class.Load.settings.Form_Main_ShapeWindowRate;
            this.shapeToolStripMenuItem.Checked = UseShapeWindow;

            this.UseSmallWindowOpen = Class.Load.settings.Form_Main_UseSmallWindowOpen;
            this.Height = Class.Load.settings.Form_Main_Height;
            this.Width = Class.Load.settings.Form_Main_Width;
            this.Location = new Point(Class.Load.settings.Form_Main_Location_X, Class.Load.settings.Form_Main_Location_Y);
            
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

            mouse.nDown = 0;
            mouse.nUp = 0;
            mouse.tDown = 0;
            mouse.tUp = 0;

            config.Time = 0;
            Timer.Elapsed += new System.Timers.ElapsedEventHandler(Updata);
            Timer.AutoReset = true;
            Timer.Start();
        }
        private void Form_Closed(object sender, FormClosedEventArgs e)
        {
            Class.Save.settings.Form_Main_Hide = this.hideToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Hide_L = this.hideLToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Hide_R = this.hideRToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Hide_U = this.hideUToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Hide_D = this.hideDToolStripMenuItem.Checked;

            Class.Save.settings.Form_Main_Find_Full = this.fullToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Part = this.partToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Same = this.sameToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Like = this.likeToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Find_Turn = this.turnToolStripMenuItem.Checked;

            Class.Save.settings.Form_Main_UseSmallWindowOpen = UseSmallWindowOpen;
            Class.Save.settings.Form_Main_Height = this.Height;
            Class.Save.settings.Form_Main_Width = this.Width;
            Class.Save.settings.Form_Main_Location_X = this.Location.X;
            Class.Save.settings.Form_Main_Location_Y = this.Location.Y;

            Class.Save.settings.Form_Main_UseBoard = UseBoard;
            Class.Save.settings.Form_Main_ShapeWindow = UseShapeWindow;
            Class.Save.settings.Form_Main_ShapeWindowRate = ShapeWindowRate;

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
                if (this.bigPicToolStripMenuItem.Checked) { NextShowBigPicture = true; }

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
                if (this.bigPicToolStripMenuItem.Checked) { NextShowBigPicture = true; }

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
                if (this.bigPicToolStripMenuItem.Checked) { NextShowBigPicture = true; }

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
                if (this.bigPicToolStripMenuItem.Checked) { NextShowBigPicture = true; }

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
                if (this.bigPicToolStripMenuItem.Checked) { NextShowBigPicture = true; }
                config.SubIndex--;
                if (config.SubIndex < 0) { config.SubIndex = config.SubFiles.Count - 1; }

                ShowCurrent();
            }

            #endregion

            #region 向下

            if (e.KeyValue == 40)
            {
                if (config.SubFiles == null || config.SubFiles.Count == 0) { return; }
                if (this.bigPicToolStripMenuItem.Checked) { NextShowBigPicture = true; }
                config.SubIndex++;
                if (config.SubIndex >= config.SubFiles.Count) { config.SubIndex = 0; }

                ShowCurrent();
            }

            #endregion

            #region 回车

            if (e.KeyValue == 13)
            {
                Form_Main_DoubleClick(null, null);
            }

            #endregion

            #region 删除

            if (e.KeyValue == 46)
            {
                RightMenu_Export(0, null);
            }

            #endregion

            #region 打开输出目录

            if (e.KeyValue == 49)
            {
                RightMenu_OpenExport(null, null); return;
            }

            #endregion

            #region 打开根目录

            if (e.KeyValue == 50)
            {
                RightMenu_OpenRoot(null, null); return;
            }

            #endregion

            #region 打开漫画目录

            if (e.KeyValue == 52)
            {
                RightMenu_OpenComic(null, null); return;
            }

            #endregion

            #region 打开当前文件

            if (e.KeyValue == 51)
            {
                RightMenu_OpenCurrent(null, null); return;
            }

            #endregion

            #region P PASSWORD

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
                if (input.Input == "#small window") { UseSmallWindowOpen = true; return; }
                if (input.Input == "#big window") { UseSmallWindowOpen = false; return; }
                if (input.Input.Length != 0 && input.Input[0] != '-')
                { ZipOperate.A_PassWord(input.Input); ShowCurrent(); }
                if (input.Input.Length != 0 && input.Input[0] == '-')
                { ZipOperate.D_PassWord(input.Input); }
            }

            #endregion

            #region ESC 退出

            if (e.KeyValue == 27)
            {
                Form_Closed(null, null);
                Application.ExitThread();
            }

            #endregion

            #region A 显示/关闭 窗口外框

            if (e.KeyValue == 65)
            {
                ShowBoard(!UseBoard); return;
            }

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
        private void WMP_DoubleClick(object sender, AxWMPLib._WMPOCXEvents_DoubleClickEvent e)
        {
            Form_Main_DoubleClick(null, null);
        }
        private void WMP_MouseDown(object sender, AxWMPLib._WMPOCXEvents_MouseDownEvent e)
        {
            if (e.nButton == 1)
            {
                this.Cursor = Cursors.Default;
                mouse.Down = true;
                mouse.Up = false;
                mouse.pDown = MousePosition;
                mouse.Previous = MousePosition;
                mouse.tDown = config.Time;
                mouse.nDown++;
            }
        }
        private void WMP_MouseUp(object sender, AxWMPLib._WMPOCXEvents_MouseUpEvent e)
        {
            if (e.nButton == 1)
            {
                if (mouse.tUp != 0 && config.Time - mouse.tUp < 20)
                { Form_Main_DoubleClick(null, null); }

                this.Cursor = Cursors.Default;
                mouse.Down = false;
                mouse.Up = true;
                mouse.pUp = MousePosition;
                mouse.tUp = config.Time;
                mouse.nUp++;
            }
            if (e.nButton == 2)
            {
                this.contextMenuStrip1.Show(MousePosition);
            }
        }

        private void Form_Main_DoubleClick(object sender, EventArgs e)
        {
            #region 判断位置是否摆正

            bool PicWindowIsOK = true;
            int picx = this.pictureBox1.Location.X;
            int picy = this.pictureBox1.Location.Y;
            int pich = this.pictureBox1.Height;
            int picw = this.pictureBox1.Width;
            int rech = UseBoard ? this.Height - 42: this.Height;
            int recw = UseBoard ? this.Width - 18 : this.Width;
            if (PicWindowIsOK && pich >= rech) { PicWindowIsOK = picy <= 1; }
            if (PicWindowIsOK && picw >= recw) { PicWindowIsOK = picx <= 1; }
            if (PicWindowIsOK && pich < rech)
            {
                int stdy = UseBoard ? (this.Height - 42 - pich) / 2 : (this.Height - pich) / 2;
                PicWindowIsOK = Math.Abs(stdy - picy) <= 1;
            }
            if (PicWindowIsOK && picw < recw)
            {
                int stdx = UseBoard ? (this.Width - 18 - picw) / 2 : (this.Width - picw) / 2;
                PicWindowIsOK = Math.Abs(stdx - picx) <= 1;
            }

            bool VidWindowIsOK = true;
            VidWindowIsOK = UseBoard ?
                this.Height - 41 == this.axWindowsMediaPlayer1.Height && this.Width - 18 == this.axWindowsMediaPlayer1.Width :
                this.Height - 2 == this.axWindowsMediaPlayer1.Height && this.Width - 2 == this.axWindowsMediaPlayer1.Width;

            #endregion

            #region 0 型文件双击操作

            if (config.Type == 0)
            {
                ShowCurrent(); return;
            }

            #endregion

            #region 1 型文件的双击操作

            if (config.Type == 1)
            {
                if (config.SubType == 2)
                {
                    if (!PicWindowIsOK) { ShowSmall(); return; }
                    NextShowBigPicture = !NextShowBigPicture;
                    if (NextShowBigPicture) { ShowBig(true); } else { ShowSmall(); }
                    return;
                }
                if (config.SubType == 3)
                {
                    ShowCurrent();
                    return;
                }
                if (config.SubType == 4)
                {
                    ShapeVideoWindow(VidWindowIsOK);
                    return;
                }

                ShowCurrent(); return;
            }

            #endregion

            #region 2 型文件的双击操作

            if (config.Type == 2)
            {
                if (!PicWindowIsOK) { ShowSmall(); return; }
                NextShowBigPicture = !NextShowBigPicture;
                if (NextShowBigPicture) { ShowBig(true); } else { ShowSmall(); }
                return;
            }

            #endregion

            #region 3 型文件的双击操作

            if (config.Type == 3)
            {
                ShowCurrent(); return;
            }

            #endregion

            #region 4 型文件的双击操作

            if (config.Type == 4)
            {
                ShapeVideoWindow(VidWindowIsOK);
                return;
            }

            #endregion

            #region 5 型文件的双击操作

            if (config.Type == 5)
            {
                if (config.SubType == 2)
                {
                    if (!PicWindowIsOK) { ShowSmall(); return; }
                    NextShowBigPicture = !NextShowBigPicture;
                    if (NextShowBigPicture) { ShowBig(true); } else { ShowSmall(); }
                    return;
                }
                if (config.SubType == 3)
                {
                    ShowCurrent(); return;
                }

                ShowCurrent(); return;
            }

            #endregion

            #region 其他文件双击操作

            ShowCurrent();

            #endregion
        }
        private void Page_U(object sender, EventArgs e)
        {
            if (config.SubFiles == null || config.SubFiles.Count == 0) { return; }
            if (this.bigPicToolStripMenuItem.Checked) { NextShowBigPicture = true; }
            config.SubIndex--;
            if (config.SubIndex < 0) { config.SubIndex = config.SubFiles.Count - 1; }

            ShowCurrent();
        }
        private void Page_D(object sender, EventArgs e)
        {
            if (config.SubFiles == null || config.SubFiles.Count == 0) { return; }
            if (this.bigPicToolStripMenuItem.Checked) { NextShowBigPicture = true; }
            config.SubIndex++;
            if (config.SubIndex >= config.SubFiles.Count) { config.SubIndex = 0; }

            ShowCurrent();
        }
        private void Page_L(object sender, EventArgs e)
        {
            if (FileOperate.RootFiles.Count == 0) { return; }
            if (this.bigPicToolStripMenuItem.Checked) { NextShowBigPicture = true; }

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
            if (this.bigPicToolStripMenuItem.Checked) { NextShowBigPicture = true; }

            config.FileIndex++;
            if (config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count)
            { config.FileIndex = 0; config.FolderIndex++; }
            if (config.FolderIndex >= FileOperate.RootFiles.Count) { config.FolderIndex = 0; }

            config.SubFiles = new List<string>(); config.SubIndex = 0; ShowCurrent();
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Cursor = Cursors.Default;
                mouse.Down = true;
                mouse.Up = false;
                mouse.pDown = MousePosition;
                mouse.Previous = MousePosition;
            }
        }
        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Cursor = Cursors.Default;
                mouse.Down = false;
                mouse.Up = true;
                mouse.pUp = MousePosition;
            }
        }
        private void Form_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!UseBoard && !NextShowBigPicture)
            {
                if (e.Delta > 0) { ShapeWindowRate += 5; }
                if (e.Delta < 0) { ShapeWindowRate -= 5; }
                if (ShapeWindowRate < 10) { ShapeWindowRate = 10; }
                if (ShapeWindowRate > 100) { ShapeWindowRate = 100; }
                ShowCurrent();
            }
        }
        private void Form_MouseEnter(object sender, EventArgs e)
        {

        }

        private void Updata(object source, System.Timers.ElapsedEventArgs e)
        {
            this.BeginInvoke((EventHandler)delegate
            {
                #region 计数器

                config.Time++;

                #endregion

                #region 刷新隐藏翻页键菜单

                bool hideU = this.hideUToolStripMenuItem.Checked;
                bool hideD = this.hideDToolStripMenuItem.Checked;
                bool hideL = this.hideLToolStripMenuItem.Checked;
                bool hideR = this.hideRToolStripMenuItem.Checked;
                this.hideToolStripMenuItem.Checked = hideL && hideR && hideU && hideD;

                #endregion

                #region 鼠标位置

                int ptX = MousePosition.X - this.Location.X; if (!UseBoard) { ptX += 10; }
                int ptY = MousePosition.Y - this.Location.Y; if (!UseBoard) { ptY += 30; }
                bool showPageMark = this.Width > 150 && this.Height > 150;

                #endregion

                #region 左翻页

                int setW = this.Width / 20;
                int setH = this.Height / 5;
                int font = setW * 3 / 4;
                int bgW = this.Width / 20; if (!UseBoard) { bgW += 5; }
                int edW = bgW + setW; 
                int bgH = this.Height / 2 - setH / 2; if (!UseBoard) { bgH += 15; }
                int edH = bgH + setH;
                if (showPageMark && !hideL && bgW <= ptX && ptX <= edW && bgH <= ptY && ptY <= edH)
                {
                    int xvalue = this.HorizontalScroll.Value;
                    int yvalue = this.VerticalScroll.Value;

                    this.label1.Location = new Point(bgW - 10, bgH - 30);
                    this.label1.Width = setW;
                    this.label1.Height = setH;
                    this.label1.Visible = true;
                    this.label1.Font = new Font("宋体", font);

                    this.HorizontalScroll.Value = xvalue;
                    this.VerticalScroll.Value = yvalue;
                }
                else { this.label1.Visible = false; }

                #endregion
                #region 右翻页

                setW = this.Width / 20;
                setH = this.Height / 5;
                font = setW * 3 / 4;
                bgW = this.Width - setW - this.Width / 20; if (!UseBoard) { bgW += 15; }
                if (this.VerticalScroll.Visible) { bgW -= 15; }
                edW = bgW + setW;
                bgH = this.Height / 2 - setH / 2; if (!UseBoard) { bgH += 15; }
                edH = bgH + setH;
                if (showPageMark && !hideR && bgW <= ptX && ptX <= edW && bgH <= ptY && ptY <= edH)
                {
                    int xvalue = this.HorizontalScroll.Value;
                    int yvalue = this.VerticalScroll.Value;

                    this.label2.Location = new Point(bgW - 10, bgH - 30);
                    this.label2.Width = setW;
                    this.label2.Height = setH;
                    this.label2.Visible = true;
                    this.label2.Font = new Font("宋体", font);

                    this.HorizontalScroll.Value = xvalue;
                    this.VerticalScroll.Value = yvalue;
                }
                else { this.label2.Visible = false; }

                #endregion
                #region 上翻页

                setW = this.Width / 8;
                setH = this.Height / 12;
                font = setH * 3 / 4;
                bgW = this.Width / 2 - setW / 2; if (!UseBoard) { bgW += 5; }
                edW = bgW + setW;
                bgH = this.Height / 20 + 30; //if (!UseBoard) { bgH += 15; }
                edH = bgH + setH;
                if (showPageMark && !hideU && bgW <= ptX && ptX <= edW && bgH <= ptY && ptY <= edH && config.SubFiles.Count > 1)
                {
                    int xvalue = this.HorizontalScroll.Value;
                    int yvalue = this.VerticalScroll.Value;

                    this.label3.Location = new Point(bgW - 10, bgH - 30);
                    this.label3.Width = setW;
                    this.label3.Height = setH;
                    this.label3.Visible = true;
                    this.label3.Font = new Font("宋体", font);

                    this.HorizontalScroll.Value = xvalue;
                    this.VerticalScroll.Value = yvalue;
                }
                else { this.label3.Visible = false; }

                #endregion
                #region 下翻页

                setW = this.Width / 8;
                setH = this.Height / 12;
                font = setH * 3 / 4;
                bgW = this.Width / 2 - setW / 2; if (!UseBoard) { bgW += 5; }
                edW = bgW + setW;
                bgH = this.Height - this.Height / 20 - setH - 10; if (!UseBoard) { bgH += 39; }
                if (this.HorizontalScroll.Visible) { bgH -= 10; }
                edH = bgH + setH;
                if (showPageMark && !hideD && bgW <= ptX && ptX <= edW && bgH <= ptY && ptY <= edH && config.SubFiles.Count > 1)
                {
                    int xvalue = this.HorizontalScroll.Value;
                    int yvalue = this.VerticalScroll.Value;

                    this.label4.Location = new Point(bgW - 10, bgH - 30);
                    this.label4.Width = setW;
                    this.label4.Height = setH;
                    this.label4.Visible = true;
                    this.label4.Font = new Font("宋体", font);

                    this.HorizontalScroll.Value = xvalue;
                    this.VerticalScroll.Value = yvalue;
                }
                else { this.label4.Visible = false; }

                #endregion

                #region 拖拽窗体

                if (!NextShowBigPicture && mouse.Down)
                {
                    int xMove = MousePosition.X - mouse.Previous.X;
                    int yMove = MousePosition.Y - mouse.Previous.Y;
                    this.Location = new Point(this.Location.X + xMove, this.Location.Y + yMove);
                    mouse.Previous = MousePosition;
                }

                #endregion

                #region 拖拽图片

                if (NextShowBigPicture && mouse.Down)
                {
                    Point Current = MousePosition;
                    int xMove = -(Current.X - mouse.Previous.X) / 10;
                    int yMove = -(Current.Y - mouse.Previous.Y) / 10;

                    //if (xMove > yMove) { yMove = 0; }
                    //if (yMove > xMove) { xMove = 0; }

                    int xScroll = this.HorizontalScroll.Value;
                    int yScroll = this.VerticalScroll.Value;
                    xScroll += xMove;
                    yScroll += yMove;
                    if (xScroll < this.HorizontalScroll.Minimum) { xScroll = this.HorizontalScroll.Minimum; }
                    if (xScroll > this.HorizontalScroll.Maximum) { xScroll = this.HorizontalScroll.Maximum; }
                    if (yScroll < this.VerticalScroll.Minimum) { yScroll = this.VerticalScroll.Minimum; }
                    if (yScroll > this.VerticalScroll.Maximum) { yScroll = this.VerticalScroll.Maximum; }

                    if (this.HorizontalScroll.Visible) { this.HorizontalScroll.Value = xScroll; }
                    if (this.VerticalScroll.Visible) { this.VerticalScroll.Value = yScroll; }
                    //mouse.Previous = Current;
                }

                #endregion

                #region 刷新播放时间

                if (config.Type == 4 || (config.IsSub && config.SubType == 4))
                {
                    string index = "[" + (config.FileIndex+1).ToString() + "/" + FileOperate.RootFiles[config.FolderIndex].Name.Count.ToString() + "]";
                    string subindex = "[" + (config.SubIndex + 1).ToString() + "/" + config.SubFiles.Count.ToString() + "]";
                    string curpos = "";
                    string total = "";
                    try { curpos = this.axWindowsMediaPlayer1.Ctlcontrols.currentPositionString; } catch { }
                    try { total = this.axWindowsMediaPlayer1.currentMedia.durationString; } catch { }

                    string title = config.IsSub ?
                        index + " " + subindex + " [" + curpos + "/" + total + "] " + config.Name + " : " + config.SubName :
                        index + " [" + curpos + "/" + total + "] " + config.Name;

                    if (curpos.Length != 0 && total.Length != 0)
                    {
                        this.Text = title;
                        this.textToolStripMenuItem.Text = title;
                    }
                }

                #endregion

                #region 当不存在任何文件时提示导入

                if (TipForInput && FileOperate.RootFiles.Count == 0)
                {
                    TipForInput = false;
                    if (DialogResult.OK == MessageBox.Show("当前不存在任何文件夹，是否导入文件夹？", "提示", MessageBoxButtons.OKCancel))
                    { RightMenu_Input(null, null); }
                }

                #endregion

                #region 按键滑动滚动条

                if (key.Down && (this.HorizontalScroll.Visible || this.VerticalScroll.Visible))
                {
                    if (key.L) { KeyDown_Enter_L(); }
                    if (key.R) { KeyDown_Enter_R(); }
                    if (key.U) { KeyDown_Enter_U(); }
                    if (key.D) { KeyDown_Enter_D(); }
                }

                #endregion
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
            #region 初始化当前文件的各种属性

            config.SubFiles.Clear();
            config.ExistFolder = false;
            config.ExistFile = false;
            config.ExistSub = false;
            config.Path = "";
            config.Name = "";
            config.Extension = "";
            config.Type = -1;
            config.Hide = false;
            config.IsSub = false;
            config.SubName = "";
            config.SubType = -1;
            config.SubExtension = "";
            config.SubHide = false;

            if (config.FolderIndex >= 0 && config.FolderIndex < FileOperate.RootFiles.Count)
            {
                config.Path = FileOperate.RootFiles[config.FolderIndex].Path;
                if (config.FileIndex >= 0 && config.FileIndex < FileOperate.RootFiles[config.FolderIndex].Name.Count)
                { config.Name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex]; }
            }
            
            config.ExistFolder = Directory.Exists(config.Path);
            config.Extension = FileOperate.getExtension(config.Name);
            config.Hide = FileOperate.IsSupportHide(config.Extension);
            config.Type = FileOperate.getFileType(config.Extension);
            config.IsSub = config.Type == 1 || config.Type == 5;
            if (config.Type == 1) { config.ExistFile = Directory.Exists(config.Path + "\\" + config.Name); }
            else { config.ExistFile = File.Exists(config.Path + "\\" + config.Name); }

            if (config.ExistFile && config.Type == 1)
            {
                config.SubFiles = FileOperate.getSubFiles(config.Path + "\\" + config.Name);
                if (config.SubIndex < 0) { config.SubIndex = 0; }
                if (config.SubIndex >= config.SubFiles.Count) { config.SubIndex = config.SubFiles.Count - 1; }
                if (config.SubFiles.Count != 0)
                {
                    config.SubName = config.SubFiles[config.SubIndex];
                    config.SubExtension = FileOperate.getExtension(config.SubName);
                    config.SubType = FileOperate.getFileType(config.SubExtension);
                    config.SubHide = FileOperate.IsSupportHide(config.SubExtension);
                    config.ExistSub = true;
                }
            }
            if (config.ExistFile && config.Type == 5)
            {
                config.Error = ZipOperate.ReadZipEX(config.Path + "\\" + config.Name);
                if (config.SubIndex < 0) { config.SubIndex = 0; }
                if (config.SubIndex >= config.SubFiles.Count) { config.SubIndex = config.SubFiles.Count - 1; }
                if (config.SubFiles.Count != 0)
                {
                    config.SubName = config.SubFiles[config.SubIndex];
                    config.SubExtension = FileOperate.getExtension(config.SubName);
                    config.SubType = FileOperate.getFileType(config.SubExtension);
                    config.SubHide = FileOperate.IsSupportHide(config.SubExtension);
                    config.ExistSub = true;
                }
            }

            #endregion

            #region 关闭播放以前的文件

            if (config.SourPicture != null) { try { config.SourPicture.Dispose(); } catch { } }
            if (config.DestPicture != null) { try { config.DestPicture.Dispose(); } catch { } }
            if (!this.axWindowsMediaPlayer1.IsDisposed)
            { try { this.axWindowsMediaPlayer1.Ctlcontrols.stop(); } catch { } }

            this.pictureBox1.Image = null;
            this.HorizontalScroll.Value = 0;
            this.VerticalScroll.Value = 0;

            #endregion

            #region 切到当前文件夹，无效文件信息

            for (int i = 0; i < this.filePathToolStripMenuItem.DropDownItems.Count; i++)
            {
                ToolStripMenuItem imenu = (ToolStripMenuItem)this.filePathToolStripMenuItem.DropDownItems[i];
                imenu.Checked = i == config.FolderIndex;
            }

            this.titleToolStripMenuItem1.Text = "Not Exist";
            this.textToolStripMenuItem.Text = "Unknow";

            #endregion

            #region 提取显示文本，处理错误信息，若文件不存在，标题给出提示信息后直接返回

            string index = config.ExistFolder ?
                "[" + (config.FileIndex + 1).ToString() + "/" + FileOperate.RootFiles[config.FolderIndex].Name.Count.ToString() + "]" :
                "";
            string subindex = config.ExistFile && config.IsSub ?
                "[" + (config.SubIndex + 1).ToString() + "/" + config.SubFiles.Count.ToString() + "]" :
                "";

            if (config.Error == 1) { config.Error = 0; this.Text = index + " [Wrong Password] " + config.Name; ShowErr(); return; }

            if (FileOperate.RootFiles.Count == 0) { this.Text = "[Empty] You can click right button to input a folder to start"; ShowNot(); return; }
            if (!config.ExistFolder) { this.Text = "[Not Exist] " + config.Path; ShowNot(); return; }
            if (FileOperate.RootFiles[config.FolderIndex].Name.Count == 0) { this.Text = "[0/0] [Empty Folder] Current Root Folder Is Empty !"; ShowNot(); return; }
            if (!config.ExistFile) { this.Text = index + " [Not Exist] " + config.Name; ShowNot(); return; }
            if (config.Type == 1 && config.SubFiles.Count == 0) { this.Text = index + " " + subindex + " [Empty Folder] " + config.Name; ShowNot(); return; }
            if (config.Type == 5 && config.SubFiles.Count == 0) { this.Text = index + " " + subindex + " [Empty File] " + config.Name; ShowNot(); return; }

            #endregion

            #region 隐藏文件不予显示

            if (!NoHide && (config.Hide || config.SubHide))
            {
                this.Text = config.IsSub ?
                    index + " " + subindex + " [Unknow] " + config.Name + " : " + config.SubName :
                    index + " [Unknow] " + config.Name;
                ShowUnk(); return;
            }

            #endregion
            
            #region 0 型文件（暂不支持类型）

            if (config.Type == 0)
            {
                this.Text = config.IsSub ?
                    index + " " + subindex + " [Unsupport] " + config.Name + " : " + config.SubName :
                    index + " [Unsupport] " + config.Name;
                ShowUnp(); return;
            }

            #endregion

            #region 1 型文件（文件夹）

            if (config.Type == 1)
            {
                this.Text = config.SubType == 4 ?
                    index + " " + subindex + " " + config.Name + " : " + config.SubName :
                    index + " " + subindex + " " + config.Name + " : " + config.SubName;
                
                if (config.SubType == 2) { ShowPicture(config.Path + "\\" + config.Name, config.SubName); return; }
                if (config.SubType == 3) { ShowGif(config.Path + "\\" + config.Name, config.SubName); return; }
                if (config.SubType == 4) { ShowVideo(config.Path + "\\" + config.Name, config.SubName); return; }

                this.Text = index + " " + subindex + " [Unsupport] " + config.Name + " : " + config.SubName;
                ShowUnp(); return;
            }

            #endregion

            #region 2 型文件（图片）

            if (config.Type == 2)
            {
                this.Text = index + " " + config.Name;
                ShowPicture(config.Path, config.Name); return;
            }

            #endregion

            #region 3 型文件（GIF）

            if (config.Type == 3)
            {
                this.Text = index + " " + config.Name;
                ShowGif(config.Path, config.Name); return;
            }

            #endregion

            #region 4 型文件（视频）

            if (config.Type == 4)
            {
                this.Text = index + " " + config.Name;
                ShowVideo(config.Path, config.Name); return;
            }

            #endregion

            #region 5 型文件（ZIP）

            if (config.Type == 5)
            {
                this.Text = index + " " + subindex + " " + config.Name + " : " + config.SubName;
                
                if (config.SubType == 2 && ZipOperate.LoadPictureEX()) { ShowPicture(null, null, false); return; }
                if (config.SubType == 3 && ZipOperate.LoadGifEX()) { ShowGif(null, null, false); return; }

                this.Text = index + " " + subindex + " [Unsupport] " + config.Name + " : " + config.SubName;
                ShowUnp(); return;
            }

            #endregion

            #region 其他文件（不支持）

            this.Text = index + " [Unsupport] " + config.Name;
            ShowUnp();

            #endregion
        }
        private void ShowPicture(string path, string name, bool load = true)
        {
            if (load) { config.SourPicture = (Bitmap)Image.FromFile(path + "\\" + name); }
            if (UseShapeWindow) { ShapeWindow(); }
            
            this.titleToolStripMenuItem1.Text = config.ExistFolder ?
                this.toolTip1.ToolTipTitle = config.Path :
                "Not Exist";
            this.textToolStripMenuItem.Text = this.Text;

            if (NextShowBigPicture) { ShowBig(); } else { ShowSmall(); }
            
            this.axWindowsMediaPlayer1.Visible = false;
            this.pictureBox1.Visible = true;
        }
        private void ShowGif(string path, string name, bool load = true)
        {
            if (load) { config.SourPicture = (Bitmap)Image.FromFile(path + "\\" + name); }
            if (UseShapeWindow) { ShapeWindow(); }
            
            this.titleToolStripMenuItem1.Text = config.ExistFolder ?
                this.toolTip1.ToolTipTitle = config.Path :
                "Not Exist";
            this.textToolStripMenuItem.Text = this.Text;

            int H = config.SourPicture.Height;
            int W = config.SourPicture.Width;
            int X = (this.Width - W - 18) / 2; if (!UseBoard) { X = (this.Width - W) / 2; }
            int Y = (this.Height - H - 42) / 2; if (!UseBoard) { Y = (this.Height - H) / 2; }
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
            this.titleToolStripMenuItem1.Text = config.ExistFolder ?
                this.toolTip1.ToolTipTitle = config.Path :
                "Not Exist";
            this.textToolStripMenuItem.Text = this.Text;

            this.axWindowsMediaPlayer1.Visible = true;
            this.pictureBox1.Visible = false;
            if (!UseBoard && FileOperate.IsMusic(FileOperate.getExtension(name))) { ShapeWindowForMusic(); }
            
            this.axWindowsMediaPlayer1.Height = UseBoard ? this.Height - 41 : this.Height - 2;
            this.axWindowsMediaPlayer1.Width = UseBoard ? this.Width - 18 : this.Width - 2;
            this.axWindowsMediaPlayer1.Location = new Point(1, 1);

            if (axWindowsMediaPlayer1.URL != path + "\\" + name) { axWindowsMediaPlayer1.URL = path + "\\" + name; }
            axWindowsMediaPlayer1.Ctlcontrols.play();

            this.axWindowsMediaPlayer1.Visible = true;
            this.pictureBox1.Visible = false;
        }
        private void ShowOff()
        {
            this.titleToolStripMenuItem1.Text = config.ExistFolder ?
                this.toolTip1.ToolTipTitle = config.Path :
                "Not Exist";
            this.textToolStripMenuItem.Text = this.Text;

            this.pictureBox1.Visible = false;
            this.axWindowsMediaPlayer1.Visible = false;
        }
        private void ShowUnk()
        {
            string unkpath = FileOperate.getExePath();
            string unkname = "unk.tip";
            if (File.Exists(unkpath + "\\" + unkname)) { ShowPicture(unkpath, unkname); }
            else { ShowOff(); }
        }
        private void ShowUnp()
        {
            string unppath = FileOperate.getExePath();
            string unpname = "unp.tip";
            if (File.Exists(unppath + "\\" + unpname)) { ShowPicture(unppath, unpname); }
            else { ShowOff(); }
        }
        private void ShowNot()
        {
            string notpath = FileOperate.getExePath();
            string notname = "not.tip";
            if (File.Exists(notpath + "\\" + notname)) { ShowPicture(notpath, notname); }
            else { ShowOff(); }
        }
        private void ShowErr()
        {
            string errpath = FileOperate.getExePath();
            string errname = "err.tip";
            if (!File.Exists(errpath + "\\" + errname)) { ShowPicture(errpath, errname); }
            else { ShowOff(); }
        }
        private void ShowBig(bool focus = false)
        {
            if (config.SourPicture == null) { return; }
            if (focus && config.DestPicture == null) { return; }
            if (!focus) { this.HorizontalScroll.Value = 0; this.VerticalScroll.Value = 0; }
            
            // 聚焦点
            int xF = MousePosition.X - this.Location.X - this.pictureBox1.Location.X - 10; if (!UseBoard) { xF += 10; }
            int yF = MousePosition.Y - this.Location.Y - this.pictureBox1.Location.Y - 30; if (!UseBoard) { yF += 30; }
            double xR = focus ? (double)xF / config.DestPicture.Width : 0;
            if (xR < 0) { xR = 0; }
            if (xR > 1) { xR = 1; }
            double yR = focus ? (double)yF / config.DestPicture.Height : 0;
            if (yR < 0) { yR = 0; }
            if (yR > 1) { yR = 1; }

            // 显示图片
            int X = (this.Width - 18 - config.SourPicture.Width) / 2; if (!UseBoard) { X += 9; }
            int Y = (this.Height - 42 - config.SourPicture.Height) / 2; if (!UseBoard) { Y += 21; }
            key.E = X < 0 || Y < 0;
            if (X < 1) { X = 1; }
            if (Y < 1) { Y = 1; }

            this.pictureBox1.Image = config.SourPicture;
            this.pictureBox1.Location = new Point(X, Y);
            this.pictureBox1.Height = config.SourPicture.Height;
            this.pictureBox1.Width = config.SourPicture.Width;

            // 把聚焦点放到屏幕中央
            int xS = (int)(config.SourPicture.Width * xR - this.Width / 2 + 40);
            if (!UseBoard) { xS -= 40; }
            if (xS < this.HorizontalScroll.Minimum) { xS = this.HorizontalScroll.Minimum; }
            if (xS > this.HorizontalScroll.Maximum) { xS = this.HorizontalScroll.Maximum; }

            int yS = (int)(config.SourPicture.Height * yR - this.Height / 2 + 20);
            if (!UseBoard) { yS -= 20; }
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
        private void ShowSmall()
        {
            this.HorizontalScroll.Value = 0;
            this.VerticalScroll.Value = 0;

            int sourX = config.SourPicture.Width;
            int sourY = config.SourPicture.Height;
            int destX = sourX;
            int destY = sourY;
            int limitX = this.Width - 18; if (!UseBoard) { limitX = this.Width; }
            int limitY = this.Height - 42; if (!UseBoard) { limitY = this.Height; }

            double rate = Math.Max((double)sourX / limitX, (double)sourY / limitY);
            if (rate > 1) { destX = (int)(config.SourPicture.Width / rate); }
            if (rate > 1) { destY = (int)(config.SourPicture.Height / rate); }

            config.DestPicture = (Image)new Bitmap(destX, destY);
            Graphics g = Graphics.FromImage(config.DestPicture);
            g.DrawImage(config.SourPicture, new Rectangle(0, 0, destX, destY), new Rectangle(0, 0, sourX, sourY), GraphicsUnit.Pixel);
            g.Dispose();

            this.pictureBox1.Width = destX;
            this.pictureBox1.Height = destY;
            Point location = UseBoard ? new Point((this.Width - 18 - destX) / 2, (this.Height - 42 - destY) / 2) : new Point((this.Width - destX) / 2, (this.Height - destY) / 2);
            this.pictureBox1.Location = location;
            this.pictureBox1.Image = config.DestPicture;
        }
        private void ShowBoard(bool show)
        {
            if (UseBoard == show) { return; }
            
            int ch = this.Height;
            int cw = this.Width;
            if (UseBoard) { ch -= 39; cw -= 16; }
            if (show) { ch += 39; cw += 16; }
            if (show) { this.FormBorderStyle = FormBorderStyle.Sizable; } else { this.FormBorderStyle = FormBorderStyle.None; }
            this.Height = ch; this.Width = cw;
            UseBoard = show;// ShowCurrent();

            if (show) { this.toolTip1.Dispose(); } else { this.toolTip1 = new ToolTip(); }
        }
        private void ShapeWindow()
        {
            if (config.SourPicture == null) { return; }
            int recth = this.Height; int rectw = this.Width;
            if (UseBoard) { recth -= 39; rectw -= 16; }

            int inith = recth;
            int initw = rectw;

            int sh = Screen.PrimaryScreen.Bounds.Height;
            int sw = Screen.PrimaryScreen.Bounds.Width;
            int ph = config.SourPicture.Height;
            int pw = config.SourPicture.Width;

            int xloc = this.Location.X;
            int yloc = this.Location.Y;

            int maxh = sh * ShapeWindowRate / 100, minh = sh * ShapeWindowRate / 100;
            int maxw = sw * ShapeWindowRate / 100, minw = sw * ShapeWindowRate / 100;
            if (recth > maxh) { recth = maxh; }
            if (recth < minh) { recth = minh; }
            if (rectw > maxw) { rectw = maxw; }
            if (rectw < minw) { rectw = minw; }

            if (ph <= maxh && pw <= maxw && sh > ph && sw > pw)
            {
                xloc -= (pw - initw) / 2; yloc -= (ph - inith) / 2;
                this.Location = new Point(xloc, yloc);

                if (UseBoard) { this.Height = ph + 39; this.Width = pw + 16; return; }
                this.Height = ph; this.Width = pw; return;
            }

            double h2w = (double)config.SourPicture.Height / config.SourPicture.Width;
            int shapeh = recth; int shapew = (int)(shapeh / h2w);
            if (shapew >= sw) { shapew = rectw; shapeh = (int)(h2w * shapew); }

            xloc -= (shapew - initw) / 2; yloc -= (shapeh - inith) / 2;
            this.Location = new Point(xloc, yloc);

            if (UseBoard) { this.Height = shapeh + 39; this.Width = shapew + 16; return; }
            this.Height = shapeh; this.Width = shapew;
        }
        private void ShapeWindowForMusic()
        {
            int recth = this.Height; int rectw = this.Width;
            if (UseBoard) { recth -= 39; rectw -= 16; }
            int rate = 40;

            int inith = recth;
            int initw = rectw;

            int sh = Screen.PrimaryScreen.Bounds.Height;
            int sw = Screen.PrimaryScreen.Bounds.Width;

            int shapew = Math.Min(sh, sw);
            int shapeh = shapew;
            shapew = shapew / 4 * rate / 100;
            shapeh = shapew;

            int xloc = this.Location.X;
            int yloc = this.Location.Y;
            xloc -= (shapew - initw) / 2; yloc -= (shapeh - inith) / 2;
            this.Location = new Point(xloc, yloc);

            if (UseBoard) { this.Height = shapeh + 39; this.Width = shapew + 16; return; }
            this.Height = shapeh; this.Width = shapew;
        }
        private void ShapeVideoWindow(bool change = false)
        {
            if (!this.axWindowsMediaPlayer1.Visible) { return; }

            int inith = this.axWindowsMediaPlayer1.Height;
            int initw = this.axWindowsMediaPlayer1.Width;

            this.axWindowsMediaPlayer1.Height = UseBoard ? this.Height - 41 : this.Height - 2;
            this.axWindowsMediaPlayer1.Width = UseBoard ? this.Width - 18 : this.Width - 2;
            this.axWindowsMediaPlayer1.Location = new Point(1, 1);

            if (inith != this.axWindowsMediaPlayer1.Height || initw != this.axWindowsMediaPlayer1.Width)
            { return; }

            WMPLib.WMPPlayState curState = this.axWindowsMediaPlayer1.playState;

            if (change && curState == WMPLib.WMPPlayState.wmppsPaused) { this.axWindowsMediaPlayer1.Ctlcontrols.play(); }
            if (change && curState == WMPLib.WMPPlayState.wmppsPlaying) { this.axWindowsMediaPlayer1.Ctlcontrols.pause(); }
            if (change && curState == WMPLib.WMPPlayState.wmppsStopped) { this.axWindowsMediaPlayer1.Ctlcontrols.play(); }
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
            if (config.Type == 4 || (config.IsSub && config.SubType == 4)) { ShapeVideoWindow(); return; }
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
            try { index = int.Parse(indexStr); } catch { MessageBox.Show("输入错误位置！","提示"); return; }
            try { subindex = int.Parse(subindexStr); } catch { MessageBox.Show("输入错误位置！", "提示"); return; }
            
            if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
            if (index < 1) { index = 1; }
            if (index > FileOperate.RootFiles[config.FolderIndex].Name.Count) { index = FileOperate.RootFiles[config.FolderIndex].Name.Count; }

            string path = FileOperate.RootFiles[config.FolderIndex].Path;
            string name = FileOperate.RootFiles[config.FolderIndex].Name[index - 1];
            List<string> subfiles = FileOperate.getSubFiles(path + "\\" + name);
            if (subfiles.Count == 0) { subfiles = ZipOperate.getZipFileEX(path + "\\" + name); }

            if (subindex < 1) { subindex = 1; }
            if (subindex > subfiles.Count) { subindex = subfiles.Count; }

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
            string path = FileOperate.RootFiles[config.FolderIndex].Path;
            string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
            int type = FileOperate.getFileType(FileOperate.getExtension(name));
            if (type == 1) { if (config.SubIndex < 0 || config.SubIndex >= config.SubFiles.Count) { return; } }

            // 获取提示信息
            string tip = name;
            if (type == 1) { tip += ":" + config.SubFiles[config.SubIndex]; }

            // 获取输入
            Form_Input input = new Form_Input(tip);
            input.Location = MousePosition;
            input.ShowDialog();
            if (input.Input == "" || input.Input == tip) { return; }

            int cut = input.Input.IndexOf(":");
            if (cut == -1) { cut = input.Input.IndexOf("："); }
            string foldername = cut == -1 ? "" : input.Input.Substring(0, cut);
            string filename = cut == -1 ? input.Input : input.Input.Substring(cut + 1);

            // 变量
            string sourpath, sourname, destpath, destname, sour, dest;

            // 重命名文件
            if (filename.Length != 0 && filename != name)
            {
                sourpath = type == 1 ? path + "\\" + name : path;
                sourname = type == 1 ? config.SubFiles[config.SubIndex] : name;
                destpath = sourpath;
                destname = filename;

                string sourex = FileOperate.getExtension(sourname);
                string destex = FileOperate.getExtension(destname);
                if (destex != sourex) { destname += sourex; }

                sour = sourpath + "\\" + sourname;
                dest = destpath + "\\" + destname;

                if (!File.Exists(sour)) { MessageBox.Show("源文件不存在！", "提示"); return; }
                if (File.Exists(dest)) { MessageBox.Show("目标文件已存在！", "提示"); return; }

                try { config.SourPicture.Dispose(); } catch { }
                try { config.DestPicture.Dispose(); } catch { }

                try { File.Move(sour, dest); } catch { MessageBox.Show("文件重命名失败！", "提示"); ShowCurrent(); return; }
                //try { File.Move(sour, dest); } catch { ShowCurrent(); return; }
                if (type != 1) { FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex] = destname; }
                else { config.SubFiles[config.SubIndex] = destname; }

                ShowCurrent();
            }

            // 重命名文件夹
            if (type == 1 && foldername.Length != 0 && foldername != name)
            {
                sourpath = path + "\\" + name;
                destpath = path + "\\" + foldername;

                if (!Directory.Exists(sourpath)) { MessageBox.Show("源文件夹不存在！", "提示"); return; }
                if (Directory.Exists(destpath)) { MessageBox.Show("文件夹已存在！", "提示"); return; }

                // 点号被保留
                if (foldername.IndexOf('.') >= 0) { MessageBox.Show("存在非法字符“.”，文件夹重命名失败！", "提示"); return; }
                if (foldername.IndexOf('\\') >= 0) { MessageBox.Show("存在非法字符“\\”，文件夹重命名失败！", "提示"); return; }
                if (foldername.IndexOf('/') >= 0) { MessageBox.Show("存在非法字符“/”，文件夹重命名失败！", "提示"); return; }
                if (foldername.IndexOf(':') >= 0) { MessageBox.Show("存在非法字符“:”，文件夹重命名失败！", "提示"); return; }
                if (foldername.IndexOf('?') >= 0) { MessageBox.Show("存在非法字符“?”，文件夹重命名失败！", "提示"); return; }
                if (foldername.IndexOf('*') >= 0) { MessageBox.Show("存在非法字符“*”，文件夹重命名失败！", "提示"); return; }
                if (foldername.IndexOf('<') >= 0) { MessageBox.Show("存在非法字符“<”，文件夹重命名失败！", "提示"); return; }
                if (foldername.IndexOf('>') >= 0) { MessageBox.Show("存在非法字符“>”，文件夹重命名失败！", "提示"); return; }
                if (foldername.IndexOf('|') >= 0) { MessageBox.Show("存在非法字符“|”，文件夹重命名失败！", "提示"); return; }

                DirectoryInfo dir = new DirectoryInfo(sourpath);
                try { config.SourPicture.Dispose(); } catch { }
                try { config.DestPicture.Dispose(); } catch { }
                try { dir.MoveTo(destpath); }
                catch
                {
                    //MessageBox.Show("文件夹重命名失败！", "提示");
                    ShowCurrent(); return;
                }

                FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex] = foldername;
                ShowCurrent();
            }

            // 以前的代码，只能重命名文件夹。
            //// 判断是否存在文件
            //if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
            //if (config.FileIndex < 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { return; }

            //// 判断当前文件类型
            //string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
            //int type = FileOperate.getFileType(FileOperate.getExtension(name));
            //if (type != 1) { MessageBox.Show("只有文件夹允许重命名！"); return; }

            //// 获取源路径
            //string sourpath = FileOperate.RootFiles[config.FolderIndex].Path;
            //string sourname = name;

            //// 给出输入框
            //Form_Input input = new Form_Input(sourname);
            //input.Location = MousePosition;
            //input.ShowDialog();

            //// 名称相同，无需重命名
            //if (input.Input == sourname) { return; }

            //// 点号被保留
            ////if (input.Input.IndexOf('.') >= 0) { MessageBox.Show("文件夹重命名失败！"); return; }

            //// 更改文件夹名称
            //DirectoryInfo dir = new DirectoryInfo(sourpath + "\\" + sourname);
            //if (config.SourPicture != null) { config.SourPicture.Dispose(); }
            //if (config.DestPicture != null) { config.DestPicture.Dispose(); }
            //try { dir.MoveTo(sourpath + "\\" + input.Input); } catch { MessageBox.Show("文件夹重命名失败！"); ShowCurrent(); return; }

            //// 更新当前文件夹名称
            //FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex] = input.Input;
            //ShowCurrent();
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

            string path = FileOperate.RootFiles[config.FolderIndex].Path;
            string file = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
            string full = path + "\\" + file;
            if (!Directory.Exists(full)) { MessageBox.Show("路径不存在，无法打开！", "提示"); return; }
            System.Diagnostics.Process.Start("explorer.exe", full);
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
            this.contextMenuStrip1.Hide();
        }
        private void RightMenu_BigPicture(object sender, EventArgs e)
        {
            this.bigPicToolStripMenuItem.Checked = !this.bigPicToolStripMenuItem.Checked;
            NextShowBigPicture = this.bigPicToolStripMenuItem.Checked;
            ShowCurrent();
        }
        private void RightMenu_Find(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Hide();

            // 文件不存在
            if (!config.ExistFolder || !config.ExistFile || (config.IsSub && !config.ExistSub))
            { MessageBox.Show("文件不存在！", "提示"); return; }

            // 只能匹配图片文件（包括GIF）
            string fullpath = config.Path + "\\" + config.Name;
            if (config.IsSub) { fullpath += "\\" + config.SubName; }
            int type = config.IsSub ? config.SubType : config.Type;
            if (type != 2 && type != 3)
            { MessageBox.Show("不能匹配图片之外的文件", "提示"); return; }
            
            // 不能匹配隐藏文件
            bool hide = config.IsSub ? config.SubHide : config.Hide;
            hide = !NoHide && hide;
            if (hide) { MessageBox.Show("不能匹配图片之外的文件", "提示"); return; }

            // 不支持的模式
            if (this.likeToolStripMenuItem.Checked)
            {
                if (DialogResult.Cancel == MessageBox.Show("尚不支持“LIKE”模式查找，是否使用“SAME”模式？", "模式不支持", MessageBoxButtons.OKCancel))
                { return; }
                this.likeToolStripMenuItem.Checked = false;
                this.sameToolStripMenuItem.Checked = true;
            }

            // 开始寻找
            ushort mode = 0;
            if (this.fullToolStripMenuItem.Checked) { mode += (ushort)Form_Find.MODE.FULL; }
            if (this.partToolStripMenuItem.Checked) { mode += (ushort)Form_Find.MODE.PART; }
            if (this.sameToolStripMenuItem.Checked) { mode += (ushort)Form_Find.MODE.SAME; }
            if (this.likeToolStripMenuItem.Checked) { mode += (ushort)Form_Find.MODE.LIKE; }
            if (this.turnToolStripMenuItem.Checked) { mode += (ushort)Form_Find.MODE.TURN; }

            Form_Find find = new Form_Find(config.SourPicture, fullpath, (Form_Find.MODE)mode);
            find.ShowDialog();

            // 更新主界面
            this.fullToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Full;
            this.partToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Part;
            this.sameToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Same;
            this.likeToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Like;
            this.turnToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Turn;
            ShowCurrent();
        }
        private void RightMenu_Find_Full(object sender, EventArgs e)
        {
            bool curr = this.fullToolStripMenuItem.Checked;

            this.fullToolStripMenuItem.Checked = !curr;
            this.partToolStripMenuItem.Checked = curr;
        }
        private void RightMenu_Find_Part(object sender, EventArgs e)
        {
            bool curr = this.partToolStripMenuItem.Checked;

            this.fullToolStripMenuItem.Checked = curr;
            this.partToolStripMenuItem.Checked = !curr;
        }
        private void RightMenu_Find_Same(object sender, EventArgs e)
        {
            bool curr = this.sameToolStripMenuItem.Checked;

            this.sameToolStripMenuItem.Checked = !curr;
            this.likeToolStripMenuItem.Checked = curr;
        }
        private void RightMenu_Find_Like(object sender, EventArgs e)
        {
            bool curr = this.likeToolStripMenuItem.Checked;

            this.sameToolStripMenuItem.Checked = curr;
            this.likeToolStripMenuItem.Checked = !curr;
        }
        private void RightMenu_Find_Turn(object sender, EventArgs e)
        {
            bool curr = this.turnToolStripMenuItem.Checked;
            
            this.turnToolStripMenuItem.Checked = !curr;
        }
        private void RightMenu_OpenCurrent(object sender, EventArgs e)
        {
            bool ExistFolder = 0 <= Form_Main.config.FolderIndex && Form_Main.config.FolderIndex < FileOperate.RootFiles.Count;
            bool ExistFile = ExistFolder && (0 <= Form_Main.config.FileIndex && Form_Main.config.FileIndex < FileOperate.RootFiles[Form_Main.config.FolderIndex].Name.Count);
            if (!ExistFile) { MessageBox.Show("文件不存在，无法打开！", "提示"); return; }

            string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
            int type = FileOperate.getFileType(FileOperate.getExtension(name));
            if (type == 1) { if (config.SubIndex < 0 || config.SubIndex >= config.SubFiles.Count) { MessageBox.Show("文件不存在，无法打开！", "提示"); return; } }

            string path = FileOperate.RootFiles[config.FolderIndex].Path;
            string file = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
            if (type == 1) { path += "\\" + file; file = config.SubFiles[config.SubIndex]; }
            string full = path + "\\" + file;
            if (!File.Exists(full)) { MessageBox.Show("文件不存在，无法打开！", "提示"); return; }
            System.Diagnostics.Process.Start("explorer.exe", "/select," + full);
        }
        private void RightMenu_Shape(object sender, EventArgs e)
        {
            UseShapeWindow = !this.shapeToolStripMenuItem.Checked;
            this.shapeToolStripMenuItem.Checked = UseShapeWindow;
        }
        private void RightMenu_Previous(object sender, EventArgs e)
        {
            if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
            if (config.FileIndex < 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { return; }
            if (this.bigPicToolStripMenuItem.Checked) { NextShowBigPicture = true; }

            int tFolder = config.FolderIndex;
            int tFile = config.FileIndex;
            int tSub = config.SubIndex;

            if (config.SubFiles != null && config.SubFiles.Count != 0) { config.SubIndex--; if (config.SubIndex < 0) { config.FileIndex--; config.SubIndex = -1; } }
            else { config.FileIndex--; config.SubIndex = -1; }
            if (config.FileIndex < 0) { config.FolderIndex--; }
            if (config.FolderIndex < 0)
            {
                //MessageBox.Show("已经是第一个文件了！", "提示");
                //config.FolderIndex = 0;
                //config.FileIndex = 0;
                //config.SubIndex = 0; return;

                config.FolderIndex = tFolder; config.FileIndex = tFile; config.SubIndex = tSub;
                if (DialogResult.Cancel == MessageBox.Show("已经是第一个文件了，是否转到上一个？", "确认转到", MessageBoxButtons.OKCancel))
                { return; }
                config.FolderIndex = FileOperate.RootFiles.Count - 1;
                config.FileIndex = FileOperate.RootFiles[config.FolderIndex].Name.Count - 1;
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
        private void RightMenu_Next(object sender, EventArgs e)
        {
            if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
            if (config.FileIndex < 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { return; }
            if (this.bigPicToolStripMenuItem.Checked) { NextShowBigPicture = true; }

            int tFolder = config.FolderIndex;
            int tFile = config.FileIndex;
            int tSub = config.SubIndex;

            if (config.SubFiles != null && config.SubIndex < config.SubFiles.Count - 1) { config.SubIndex++; if (config.SubIndex >= config.SubFiles.Count) { config.FileIndex++; config.SubIndex = -1; } }
            else { config.FileIndex++; config.SubIndex = -1; }
            if (config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { config.FolderIndex++; config.FileIndex = 0; }
            if (config.FolderIndex >= FileOperate.RootFiles.Count)
            {
                //MessageBox.Show("已经是最后一个文件了！", "提示");
                //config.FolderIndex = FileOperate.RootFiles.Count - 1;
                //config.FileIndex = FileOperate.RootFiles[config.FolderIndex].Name.Count - 1;
                //config.SubIndex = config.SubFiles == null ? 0 : config.SubFiles.Count - 1;
                //return;

                config.FolderIndex = tFolder; config.FileIndex = tFile; config.SubIndex = tSub;
                if (DialogResult.Cancel == MessageBox.Show("已经是最后一个文件了，是否转到下一个？", "确认转到", MessageBoxButtons.OKCancel))
                { return; }
                config.FolderIndex = 0;
                config.FileIndex = 0;
            }

            string name = FileOperate.RootFiles[config.FolderIndex].Name[config.FileIndex];
            int type = FileOperate.getFileType(FileOperate.getExtension(name));
            if (type != 1) { config.SubIndex = 0; }
            if (type == 1 && config.SubIndex == -1) { config.SubIndex = 0; }

            ShowCurrent(); key.E = false; return;
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
