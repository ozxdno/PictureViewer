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
        /// 配置项
        /// </summary>
        public static CONFIG config;
        /// <summary>
        /// 配置项
        /// </summary>
        public struct CONFIG
        {
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
        
        /// <summary>
        /// 刷新界面的定时器
        /// </summary>
        private System.Timers.Timer Timer = new System.Timers.Timer(10);
        /// <summary>
        /// 计数器（代表时间，10ms一次）
        /// </summary>
        private ulong TimeCount;
        /// <summary>
        /// 下一张图片是否显示源图（否则显示自适应窗体的图）
        /// </summary>
        private bool NextShowBigPicture = false;
        /// <summary>
        /// 是否用小窗口打开
        /// </summary>
        private bool UseSmallWindowOpen = false;
        /// <summary>
        /// 是否使用边框
        /// </summary>
        private bool UseBoard = true;
        /// <summary>
        /// 是否自动调整窗体大小
        /// </summary>
        private bool UseShapeWindow = false;
        /// <summary>
        /// 窗体缩放率 / 图片缩放比率。计算方法如下：
        /// 首先获取与屏幕成比例的窗口，
        /// 该窗口裁剪成与显示 图片/视频 相同比例大小（如果有必要），
        /// 最后调整控件大小。
        /// </summary>
        private int ShapeWindowRate = 80;
        /// <summary>
        /// 窗体最大大小
        /// </summary>
        private Size MaxWindowSize;
        /// <summary>
        /// 窗体最小大小
        /// </summary>
        private Size MinWindowSize;
        /// <summary>
        /// 边框尺寸
        /// </summary>
        private Size BoardSize;
        /// <summary>
        /// 客户区高度
        /// </summary>
        private int ClientHeight
        {
            get { return UseBoard ? this.Height - BoardSize.Height : this.Height; }
            set { this.Height = UseBoard ? value + BoardSize.Height : value; }
        }
        /// <summary>
        /// 客户区宽度
        /// </summary>
        private int ClientWidth
        {
            get { return UseBoard ? this.Width - BoardSize.Width : this.Width; }
            set { this.Width = UseBoard ? value + BoardSize.Width : value; }
        }
        /// <summary>
        /// 开启无文件提示（仅一次）
        /// </summary>
        private bool TipForInput = false;
        /// <summary>
        /// 上一次滚轮翻页时间，最大值（ulong.MaxValue）表示正在翻页
        /// </summary>
        private ulong WheelPageTime;
        /// <summary>
        /// 缓存上一次 X 方向的滑动量
        /// </summary>
        private int PrevScrollX;
        /// <summary>
        /// 缓存上一次 Y 方向的滑动量
        /// </summary>
        private int PrevScrollY;
        /// <summary>
        /// 该窗体是否被激活
        /// </summary>
        private bool IsActive;
        /// <summary>
        /// 鼠标信息
        /// </summary>
        private MOUSE mouse;
        /// <summary>
        /// 方向键信息
        /// </summary>
        private KEY key;
        /// <summary>
        /// 提示信息
        /// </summary>
        private TIP tip;

        /// <summary>
        /// 鼠标信息
        /// </summary>
        private struct MOUSE
        {
            public bool Down;
            public bool Up;
            public Point pDown;
            public Point pUp;
            public ulong tDown;
            public ulong tUp;
            public uint nDown;
            public uint nUp;

            public bool Down2;
            public bool Up2;
            public ulong tDown2;
            public ulong tUp2;
        }
        /// <summary>
        /// 方向键信息
        /// </summary>
        private struct KEY
        {
            /// <summary>
            /// 是否有键按下
            /// </summary>
            public bool Down;

            /// <summary>
            /// 上键已按下
            /// </summary>
            public bool U;
            /// <summary>
            /// 下键已按下
            /// </summary>
            public bool D;
            /// <summary>
            /// 左键已按下
            /// </summary>
            public bool L;
            /// <summary>
            /// 右键已按下
            /// </summary>
            public bool R;
        }
        /// <summary>
        /// 提示信息
        /// </summary>
        private struct TIP
        {
            /// <summary>
            /// 上一次的位置
            /// </summary>
            public Point Previous;
            /// <summary>
            /// 悬停起始时间
            /// </summary>
            public ulong Begin;
            /// <summary>
            /// 提示信息
            /// </summary>
            public string Message;
            /// <summary>
            /// 提示窗口
            /// </summary>
            public Form_Tip Form;
            /// <summary>
            /// 是否已经开启
            /// </summary>
            public bool Visible;
            /// <summary>
            /// 强行隐藏提示
            /// </summary>
            public bool Hide;
        }
        
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo); // dwFlags = 0 按下 dwFlags = 2 抬起

        ///////////////////////////////////////////////////// public method ///////////////////////////////////////////////

        public Form_Main()
        {
            InitializeComponent();
            this.MouseWheel += Form_MouseWheel;
        }
        
        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////

        private void Form_Loaded(object sender, EventArgs e)
        {
            #region 配置文件

            config.ConfigPath = FileOperate.getExePath();
            config.ConfigName = "pv.pvini";
            //if (!Class.Load.Load_CFG()) { MessageBox.Show("配置文件（pv.pvini）不存在或已损坏"); }
            Class.Load.Load_CFG();

            #endregion

            #region 隐藏上下左右

            this.hideToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Hide;
            this.hideUToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Hide_U;
            this.hideDToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Hide_D;
            this.hideLToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Hide_L;
            this.hideRToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Hide_R;

            #endregion

            #region 寻找图片的模式

            this.fullToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Full;
            this.partToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Part;
            this.sameToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Same;
            this.likeToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Like;
            this.turnToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Find_Turn;

            #endregion

            #region 边框，缩放

            BoardSize = this.Size - this.ClientSize;

            if (!Class.Load.settings.Form_Main_UseBoard) { this.FormBorderStyle = FormBorderStyle.None; }
            UseBoard = Class.Load.settings.Form_Main_UseBoard;
            UseShapeWindow = Class.Load.settings.Form_Main_ShapeWindow;
            ShapeWindowRate = Class.Load.settings.Form_Main_ShapeWindowRate;
            this.shapeToolStripMenuItem.Checked = UseShapeWindow;

            int sh = Screen.PrimaryScreen.Bounds.Height;
            int sw = Screen.PrimaryScreen.Bounds.Width;
            int max = Class.Load.settings.Form_Main_MaxWindowSize;
            int min = Class.Load.settings.Form_Main_MinWindowSize;

            MaxWindowSize = new Size(sw * max / 100, sh * max / 100);
            MinWindowSize = new Size(sw * min / 100, sh * min / 100);

            #endregion

            #region 起始窗体大小，位置

            this.UseSmallWindowOpen = Class.Load.settings.Form_Main_UseSmallWindowOpen;
            this.Height = Class.Load.settings.Form_Main_Height;
            this.Width = Class.Load.settings.Form_Main_Width;
            this.Location = new Point(Class.Load.settings.Form_Main_Location_X, Class.Load.settings.Form_Main_Location_Y);

            #endregion

            #region 其他，参数初始化

            this.lockToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Lock;
            WheelPageTime = 0;

            tip.Previous = new Point(0, 0);
            tip.Message = "";
            tip.Form = new Form_Tip();
            tip.Begin = 0;
            tip.Visible = false;

            this.tipToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Tip;

            #endregion

            #region 判断索引号是否有效，调整索引号，显示初始文件

            config.SubFiles = new List<string>();
            if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { config.FolderIndex = 0; }
            if (config.FileIndex < 0 || FileOperate.RootFiles.Count == 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count)
            { config.FileIndex = 0; }

            ShowCurrent();

            #endregion

            #region 根目录列表

            for (int i = FileOperate.RootFiles.Count - 1; i >= 0; i--)
            {
                ToolStripMenuItem menu = new ToolStripMenuItem(FileOperate.RootFiles[i].Path);
                menu.Click += RightMenu_Path;
                this.filePathToolStripMenuItem.DropDownItems.Insert(0, menu);
            }
            if (config.FolderIndex >= 0 && config.FolderIndex < FileOperate.RootFiles.Count)
            { ((ToolStripMenuItem)this.filePathToolStripMenuItem.DropDownItems[config.FolderIndex]).Checked = true; }

            #endregion

            #region 输出目录

            this.pathToolStripMenuItem.Text = Directory.Exists(config.ExportFolder) ? config.ExportFolder : config.ConfigPath;

            #endregion

            #region 鼠标参数

            mouse.nDown = 0;
            mouse.nUp = 0;
            mouse.tDown = 0;
            mouse.tUp = 0;

            #endregion

            #region 计时器

            TimeCount = 0;
            Timer.Elapsed += new System.Timers.ElapsedEventHandler(Updata);
            Timer.AutoReset = true;
            Timer.Start();

            #endregion
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
            Class.Save.settings.Form_Main_Lock = this.lockToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Tip = this.tipToolStripMenuItem.Checked;
            
            Timer.Close(); Class.Save.Save_CFG();
        }
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            #region 上下左右翻动滚动条

            if (e.KeyValue == 37) { key.L = true; }
            if (e.KeyValue == 38) { key.U = true; }
            if (e.KeyValue == 39) { key.R = true; }
            if (e.KeyValue == 40) { key.D = true; }
            key.Down = key.L || key.R || key.U || key.D;
            if (e.KeyValue < 41 && e.KeyValue > 36 && !this.lockToolStripMenuItem.Checked && (this.HorizontalScroll.Visible || this.VerticalScroll.Visible))
            { return; }

            #endregion

            #region 上一项

            if (e.KeyValue == 33)
            {
                RightMenu_Previous(null, null); return;
            }

            #endregion

            #region 下一项

            if (e.KeyValue == 34)
            {
                RightMenu_Next(null, null); return;
            }

            #endregion

            #region 前一个

            if (e.KeyValue == 37)
            {
                Page_L(null, null); return;
            }

            #endregion

            #region 后一个

            if (e.KeyValue == 39)
            {
                Page_R(null, null); return;
            }

            #endregion

            #region 向上

            if (e.KeyValue == 38)
            {
                Page_U(null, null); return;
            }

            #endregion

            #region 向下

            if (e.KeyValue == 40)
            {
                Page_D(null, null); return;
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
                if (input.Input == "#show hide") { FileSupport.SupportHide = true; FileOperate.Reload(); ShowCurrent(); return; }
                if (input.Input == "#hide hide") { FileSupport.SupportHide = false; FileOperate.Reload(); ShowCurrent();  return; }
                if (input.Input == "#small") { UseSmallWindowOpen = true; return; }
                if (input.Input == "#big") { UseSmallWindowOpen = false; return; }
                if (input.Input.Length != 0 && input.Input[0] != '-')
                { ZipOperate.A_PassWord(input.Input); ShowCurrent(); }
                if (input.Input.Length != 0 && input.Input[0] == '-')
                { ZipOperate.D_PassWord(input.Input); }

                return;
            }

            #endregion

            #region ESC 退出

            if (e.KeyValue == 27)
            {
                this.Visible = false; Form_Closed(null, null);
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
                mouse.tDown = TimeCount;
                mouse.nDown++;
            }

            if (e.nButton == 2)
            {
                mouse.Down2 = true;
                mouse.Up2 = false;
                mouse.tDown2 = TimeCount;
            }
        }
        private void WMP_MouseUp(object sender, AxWMPLib._WMPOCXEvents_MouseUpEvent e)
        {
            if (e.nButton == 1)
            {
                if (mouse.tUp != 0 && TimeCount - mouse.tUp < 20)
                { Form_Main_DoubleClick(null, null); }

                this.Cursor = Cursors.Default;
                mouse.Down = false;
                mouse.Up = true;
                mouse.pUp = MousePosition;
                mouse.tUp = TimeCount;
                mouse.nUp++;
            }
            if (e.nButton == 2)
            {
                this.contextMenuStrip1.Show(MousePosition);

                mouse.Down2 = false;
                mouse.Up2 = true;
                mouse.tUp2 = TimeCount;
            }
        }
        
        private void Form_Main_DoubleClick(object sender, EventArgs e)
        {
            #region 判断位置是否摆正

            int rech = UseBoard ? this.Height - BoardSize.Height : this.Height;
            int recw = UseBoard ? this.Width - BoardSize.Width : this.Width;

            bool PicWindowIsOK = true;
            int picx = this.pictureBox1.Location.X;
            int picy = this.pictureBox1.Location.Y;
            int pich = this.pictureBox1.Height;
            int picw = this.pictureBox1.Width;
            if (PicWindowIsOK && pich >= rech) { PicWindowIsOK = picy <= 1; }
            if (PicWindowIsOK && picw >= recw) { PicWindowIsOK = picx <= 1; }
            if (PicWindowIsOK && pich < rech)
            {
                int stdy = UseBoard ? (this.Height - BoardSize.Height - pich) / 2 : (this.Height - pich) / 2;
                PicWindowIsOK = Math.Abs(stdy - picy) <= 1;
            }
            if (PicWindowIsOK && picw < recw)
            {
                int stdx = UseBoard ? (this.Width - BoardSize.Width - picw) / 2 : (this.Width - picw) / 2;
                PicWindowIsOK = Math.Abs(stdx - picx) <= 1;
            }

            bool VidWindowIsOK = true;
            VidWindowIsOK = rech - 2 == this.axWindowsMediaPlayer1.Height && recw - 2 == this.axWindowsMediaPlayer1.Width;

            #endregion

            #region -1 型文件的双击操作

            if (config.Type == -1 || (config.IsSub && config.SubType == -1))
            {
                if (!PicWindowIsOK) { ShowSmall(); return; }
                if (rech < config.SourPicture.Height || recw < config.SourPicture.Width)
                { NextShowBigPicture = !NextShowBigPicture; }
                if (NextShowBigPicture) { ShowBig(true); } else { ShowSmall(); } return;
            }

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
                    if (rech < config.SourPicture.Height || recw < config.SourPicture.Width)
                    { NextShowBigPicture = !NextShowBigPicture; }
                    if (NextShowBigPicture) { ShowBig(true); } else { ShowSmall(); } return;
                }
                if (config.SubType == 3)
                {
                    ShowBig(); return;
                }
                if (config.SubType == 4)
                {
                    if (!VidWindowIsOK) { ShapeControl(); return; }

                    WMPLib.WMPPlayState state = this.axWindowsMediaPlayer1.playState;
                    if (state == WMPLib.WMPPlayState.wmppsPaused) { this.axWindowsMediaPlayer1.Ctlcontrols.play(); }
                    if (state == WMPLib.WMPPlayState.wmppsStopped) { this.axWindowsMediaPlayer1.Ctlcontrols.play(); }
                    if (state == WMPLib.WMPPlayState.wmppsPlaying) { this.axWindowsMediaPlayer1.Ctlcontrols.pause(); }
                    return;
                }

                ShowCurrent(); return;
            }

            #endregion

            #region 2 型文件的双击操作

            // 位置不对时，双击调整图片位置。
            // 有边框时：双击大小图切换，当源图过小时，双击只显示源图且不改变下一次显示的状态。
            // 无边框时：双击大小图切换，当源图过小时，双击只显示源图且不改变下一次显示的状态。

            if (config.Type == 2)
            {
                if (!PicWindowIsOK) { ShowSmall(); return; }
                if (rech < config.SourPicture.Height || recw < config.SourPicture.Width)
                { NextShowBigPicture = !NextShowBigPicture; }
                if (NextShowBigPicture) { ShowBig(true); } else { ShowSmall(); }
                return;
            }

            #endregion

            #region 3 型文件的双击操作

            if (config.Type == 3)
            {
                ShowBig(); return;
            }

            #endregion

            #region 4 型文件的双击操作

            if (config.Type == 4)
            {
                if (!VidWindowIsOK) { ShapeControl(); return; }

                WMPLib.WMPPlayState state = this.axWindowsMediaPlayer1.playState;
                if (state == WMPLib.WMPPlayState.wmppsPaused) { this.axWindowsMediaPlayer1.Ctlcontrols.play(); }
                if (state == WMPLib.WMPPlayState.wmppsStopped) { this.axWindowsMediaPlayer1.Ctlcontrols.play(); }
                if (state == WMPLib.WMPPlayState.wmppsPlaying) { this.axWindowsMediaPlayer1.Ctlcontrols.pause(); }
                return;
            }

            #endregion

            #region 5 型文件的双击操作

            if (config.Type == 5)
            {
                if (config.SubType == 2)
                {
                    if (!PicWindowIsOK) { ShowSmall(); return; }
                    if (rech < config.SourPicture.Height || recw < config.SourPicture.Width)
                    { NextShowBigPicture = !NextShowBigPicture; }
                    if (NextShowBigPicture) { ShowBig(true); } else { ShowSmall(); }
                    return;
                }
                if (config.SubType == 3)
                {
                    ShowBig(); return;
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

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Cursor = Cursors.Default;
                mouse.Down = true;
                mouse.Up = false;
                mouse.pDown = MousePosition;
                mouse.tDown = TimeCount;

                PrevScrollX = this.HorizontalScroll.Value;
                PrevScrollY = this.VerticalScroll.Value;
            }

            if (e.Button == MouseButtons.Right)
            {
                mouse.Down2 = true;
                mouse.Up2 = false;
                mouse.tDown2 = TimeCount;
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
                mouse.tUp = TimeCount;
            }

            if (e.Button == MouseButtons.Right)
            {
                mouse.Down2 = false;
                mouse.Up2 = true;
                mouse.tUp2 = TimeCount;
            }
        }
        private void Form_MouseWheel(object sender, MouseEventArgs e)
        {
            int ch = UseBoard ? this.Height - BoardSize.Height : this.Height;
            int cw = UseBoard ? this.Width - BoardSize.Width : this.Width;
            
            #region 播放视频/音频时，滚轮不起作用

            if (config.Type == 4 || (config.IsSub && config.SubType == 4))
            {
                return;
            }

            #endregion

            #region 滚轮滚动屏幕

            if ((this.HorizontalScroll.Visible || this.VerticalScroll.Visible) ||
                (this.pictureBox1.Visible && (this.pictureBox1.Height > ch || this.pictureBox1.Width > cw)))
            {
                return;
            }

            #endregion

            #region 滑动滚轮上下翻页

            if (this.lockToolStripMenuItem.Checked ||
                (!this.axWindowsMediaPlayer1.Visible && !this.pictureBox1.Visible))
            {
                if (WheelPageTime == ulong.MaxValue) { return; }
                if (TimeCount - WheelPageTime < 20) { return; }
                
                WheelPageTime = ulong.MaxValue;
                bool prevState = tip.Hide;
                tip.Hide = true;

                if (e.Delta > 0) { RightMenu_Previous(null, null); }
                if (e.Delta < 0) { RightMenu_Next(null, null); }

                tip.Hide = prevState;
                WheelPageTime = TimeCount;

                return;
            }

            #endregion

            #region 根据文件类型确定待执行的代码段

            int codephase = config.IsSub ? config.SubType : config.Type;
            //if (UseBoard) { BoardSize = this.Size - this.ClientSize; }

            #endregion

            #region 图片型文件滑动滚轮操作

            if (codephase == -1 || codephase == 0 || codephase == 2)
            {
                // 必须存在源图
                if (config.SourPicture == null) { return; }
                
                // 屏幕参数
                int sh = Screen.PrimaryScreen.Bounds.Height;
                int sw = Screen.PrimaryScreen.Bounds.Width;

                // 当前缩放
                double rate1 = (double)this.pictureBox1.Height / sh * 100;
                double rate2 = (double)this.pictureBox1.Width / sw * 100;
                int currRate = (int)Math.Max(rate1, rate2);

                // 下一次的显示比例
                if (e.Delta > 0) { ShapeWindowRate = currRate + 5; }
                if (e.Delta < 0) { ShapeWindowRate = currRate - 5; }
                if (ShapeWindowRate <= Class.Load.settings.Form_Main_MinWindowSize) { ShapeWindowRate = Class.Load.settings.Form_Main_MinWindowSize; }
                if (ShapeWindowRate >= Class.Load.settings.Form_Main_MaxWindowSize) { ShapeWindowRate = Class.Load.settings.Form_Main_MaxWindowSize; }

                // 不自动裁剪窗体时，显示图片的比例不能大于当前窗口（出现滚动条）
                int nexth = sh * ShapeWindowRate / 100;
                int nextw = sw * ShapeWindowRate / 100;
                double h2w = (double)config.SourPicture.Height / config.SourPicture.Width;
                rate1 = (double)nexth / config.SourPicture.Height;
                rate2 = (double)nextw / config.SourPicture.Width;
                if (rate1 <= rate2) { nextw = (int)(nexth / h2w); }
                else { nexth = (int)(nextw * h2w); }
                if (!UseShapeWindow && (nexth >= ch || nextw >= cw))
                {
                    rate1 = (double)ch / config.SourPicture.Height;
                    rate2 = (double)cw / config.SourPicture.Width;
                    nexth = (int)(Math.Min(rate1, rate2) * config.SourPicture.Height);
                    nextw = (int)(Math.Min(rate1, rate2) * config.SourPicture.Width);
                    rate1 = (double)nexth / sh * 100;
                    rate2 = (double)nextw / sw * 100;
                    ShapeWindowRate = (int)Math.Max(rate1, rate2);
                }

                // 显示图片
                if (ShapeWindowRate <= Class.Load.settings.Form_Main_MinWindowSize) { ShapeWindowRate = Class.Load.settings.Form_Main_MinWindowSize; }
                if (ShapeWindowRate >= Class.Load.settings.Form_Main_MaxWindowSize) { ShapeWindowRate = Class.Load.settings.Form_Main_MaxWindowSize; }
                ShowRate();
            }

            #endregion
        }
        private void Form_MouseHover(object sender, MouseEventArgs e)
        {
            
        }

        private void Updata(object source, System.Timers.ElapsedEventArgs e)
        {
            this.BeginInvoke((EventHandler)delegate
            {
                #region 计数器

                TimeCount++;

                #endregion

                #region 提示信息

                bool inX = MousePosition.X >= this.Location.X && MousePosition.X <= this.Location.X + this.Width;
                bool inY = MousePosition.Y >= this.Location.Y && MousePosition.Y <= this.Location.Y + this.Height;

                if (this.tipToolStripMenuItem.Checked && inX && inY && MousePosition == tip.Previous)
                {
                    ulong hover = TimeCount - tip.Begin;
                    int type = config.IsSub ? config.SubType : config.Type;
                    ulong pause = 300;
                    bool sameText = this.Text == tip.Message;

                    bool mustOK =
                        !(tip.Hide) &&
                        hover > 20 &&
                        !mouse.Down &&
                        !mouse.Down2 &&
                        TimeCount - mouse.tDown > 50 &&
                        TimeCount - mouse.tDown2 > 50 &&
                        TimeCount - mouse.tUp > 20 &&
                        TimeCount - mouse.tUp2 > 20 &&
                        !this.contextMenuStrip1.Visible &&
                        !(!IsActive && !tip.Visible) &&
                        !this.label1.Visible &&
                        !this.label2.Visible &&
                        !this.label3.Visible &&
                        !this.label4.Visible;

                    if (sameText)
                    {
                        sameText = true;
                    }
                    
                    if (mustOK && !sameText)
                    {
                        tip.Begin = TimeCount;
                        tip.Message = this.Text;
                        tip.Visible = true;
                        tip.Form.show(tip.Message);
                        IsActive = true;
                    }

                    if (mustOK && tip.Message == this.Text &&
                        hover < pause &&
                         !(this.HorizontalScroll.Visible || this.VerticalScroll.Visible))
                    {
                        tip.Message = this.Text;
                        tip.Visible = true;
                        tip.Form.show(tip.Message);
                        IsActive = true;
                    }

                    if (hover > pause ||
                        TimeCount - mouse.tDown < 50 ||
                        TimeCount - mouse.tDown2 < 50 ||
                        TimeCount - mouse.tUp < 20 ||
                        TimeCount - mouse.tUp2 < 20)
                    {
                        if (sameText) { tip.Form.hide(); tip.Visible = false; }
                    }
                }
                if (tip.Hide ||
                    ((!inX || !inY) && tip.Visible) ||
                    (this.tipToolStripMenuItem.Checked && inX && inY && MousePosition != tip.Previous))
                {
                    tip.Previous = MousePosition;
                    tip.Begin = TimeCount;
                    if (tip.Visible) { tip.Visible = false; tip.Form.hide(); }
                }

                if (tip.Form.KeyValue != -1)
                {
                    if (tip.Form.KeyValue == 33 ||
                        tip.Form.KeyValue == 34 ||
                        tip.Form.KeyValue == 37 ||
                        tip.Form.KeyValue == 38 ||
                        tip.Form.KeyValue == 39 ||
                        tip.Form.KeyValue == 40 ||
                        tip.Form.KeyValue == 65 ||
                        tip.Form.KeyValue == 13)
                    {
                        KeyEventArgs eKey = new KeyEventArgs((Keys)tip.Form.KeyValue);
                        if (tip.Form.KeyState == 0) { Form_KeyDown(null, eKey); }
                        if (tip.Form.KeyState == 2) { Form_KeyUp(null, eKey); }
                        tip.Form.KeyValue = -1;
                    }

                    if (tip.Form.KeyValue == 46 ||
                        tip.Form.KeyValue == 49 ||
                        tip.Form.KeyValue == 50 ||
                        tip.Form.KeyValue == 51 ||
                        tip.Form.KeyValue == 52 ||
                        tip.Form.KeyValue == 80 ||
                        tip.Form.KeyValue == 27)
                    {
                        keybd_event((byte)tip.Form.KeyValue, 0, tip.Form.KeyState, 0);
                        tip.Form.KeyValue = -1;
                        tip.Visible = false; tip.Form.hide();
                    }
                }

                #endregion

                #region 播放

                if (this.playToolStripMenuItem.Checked)
                {
                    bool playNext = (config.IsSub ? config.SubType : config.Type) != 4;

                    if (!playNext)
                    {
                        playNext = this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped;
                    }
                    
                    if (playNext)
                    {
                        int nextFolder = config.FolderIndex;
                        int nextFile = config.IsSub ? config.FileIndex : config.FileIndex + 1;
                        int nextSub = config.IsSub ? 0 : config.SubIndex + 1;
                        
                        if (FileOperate.ExistFolder(nextFolder))
                        {
                            string path = FileOperate.RootFiles[nextFolder].Path;

                            for (; nextFile < FileOperate.RootFiles[nextFolder].Name.Count; nextFile++)
                            {
                                string name = FileOperate.RootFiles[nextFolder].Name[nextFile];
                                int type = FileOperate.getFileType(FileOperate.getExtension(name));
                                if (type != 4 && type != 1) { nextSub = 0; continue; }

                                if (type == 4) { config.FolderIndex = nextFolder; config.FileIndex = nextFile; ShowCurrent(); break; }

                                List<string> subfiles = FileOperate.getSubFiles(path + "\\" + name);
                                bool found = false;
                                for (; nextSub < subfiles.Count; nextSub++)
                                {
                                    int subtype = FileOperate.getFileType(FileOperate.getExtension(subfiles[nextSub]));
                                    if (subtype != 4) { continue; }
                                    config.FolderIndex = nextFolder; config.FileIndex = nextFile; config.SubIndex = nextSub;
                                    found = true; ShowCurrent(); break;
                                }
                                if (found) { break; }
                            }
                            if (nextFile >= FileOperate.RootFiles[nextFolder].Name.Count)
                            { this.playToolStripMenuItem.Checked = false; }
                        }
                    }
                }

                #endregion

                #region 刷新隐藏翻页键菜单

                bool hideU = this.hideUToolStripMenuItem.Checked;
                bool hideD = this.hideDToolStripMenuItem.Checked;
                bool hideL = this.hideLToolStripMenuItem.Checked;
                bool hideR = this.hideRToolStripMenuItem.Checked;
                this.hideToolStripMenuItem.Checked = hideL && hideR && hideU && hideD;

                #endregion

                #region 是否使用大图（源图）

                //this.bigPicToolStripMenuItem.Checked = NextShowBigPicture;

                #endregion

                #region 是否显示详细信息

                //this.infoToolStripMenuItem.Visible = !UseBoard;

                #endregion

                #region 鼠标位置，是否允许翻页

                int ptX = MousePosition.X - this.Location.X; if (!UseBoard) { ptX += 10; }
                int ptY = MousePosition.Y - this.Location.Y; if (!UseBoard) { ptY += 30; }

                bool showPageMark = this.Width > 150 && this.Height > 150 && !mouse.Down &&
                    !this.HorizontalScroll.Visible &&
                    !this.VerticalScroll.Visible;

                #endregion

                //int scrollw = this.HorizontalScroll.Value;
                //int scrollh = this.VerticalScroll.Value;

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
                    //int xvalue = this.HorizontalScroll.Value;
                    //int yvalue = this.VerticalScroll.Value;

                    this.label1.Location = new Point(bgW - 10, bgH - 30);
                    this.label1.Width = setW;
                    this.label1.Height = setH;
                    this.label1.Visible = true;
                    this.label1.Font = new Font("宋体", font);

                    //this.HorizontalScroll.Value = xvalue;
                    //this.VerticalScroll.Value = yvalue;
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
                    //int xvalue = this.HorizontalScroll.Value;
                    //int yvalue = this.VerticalScroll.Value;

                    this.label2.Location = new Point(bgW - 10, bgH - 30);
                    this.label2.Width = setW;
                    this.label2.Height = setH;
                    this.label2.Visible = true;
                    this.label2.Font = new Font("宋体", font);

                    //SetScrollW(xvalue); SetScrollH(yvalue);
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
                    //int xvalue = this.HorizontalScroll.Value;
                    //int yvalue = this.VerticalScroll.Value;

                    this.label3.Location = new Point(bgW - 10, bgH - 30);
                    this.label3.Width = setW;
                    this.label3.Height = setH;
                    this.label3.Visible = true;
                    this.label3.Font = new Font("宋体", font);

                    //this.HorizontalScroll.Value = xvalue;
                    //this.VerticalScroll.Value = yvalue;
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
                    //int xvalue = this.HorizontalScroll.Value;
                    //int yvalue = this.VerticalScroll.Value;

                    this.label4.Location = new Point(bgW - 10, bgH - 30);
                    this.label4.Width = setW;
                    this.label4.Height = setH;
                    this.label4.Visible = true;
                    this.label4.Font = new Font("宋体", font);

                    //this.HorizontalScroll.Value = xvalue;
                    //this.VerticalScroll.Value = yvalue;
                }
                else { this.label4.Visible = false; }

                #endregion

                //SetScrollW(scrollw); SetScrollH(scrollh);

                #region 拖拽窗体

                if (!this.HorizontalScroll.Visible && !this.VerticalScroll.Visible && mouse.Down)
                {
                    int xMove = MousePosition.X - mouse.pDown.X;
                    int yMove = MousePosition.Y - mouse.pDown.Y;
                    this.Location = new Point(this.Location.X + xMove, this.Location.Y + yMove);
                    mouse.pDown = MousePosition;
                }

                #endregion

                #region 拖拽图片
                
                if((this.HorizontalScroll.Visible || this.VerticalScroll.Visible) && mouse.Down)
                {
                    int xS = MousePosition.X - mouse.pDown.X;
                    int yS = MousePosition.Y - mouse.pDown.Y;

                    SetScrollW(PrevScrollX - xS);
                    SetScrollH(PrevScrollY - yS);
                }

                #endregion

                #region 刷新播放时间

                if ((config.Type == 4) || (config.IsSub && config.SubType == 4))
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

                if (key.Down && !this.lockToolStripMenuItem.Checked && (this.HorizontalScroll.Visible || this.VerticalScroll.Visible))
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

            if (config.FolderIndex < 0) { config.FolderIndex = 0; }
            if (config.FolderIndex >= FileOperate.RootFiles.Count) { config.FolderIndex = FileOperate.RootFiles.Count - 1; }
            if (config.FolderIndex >= 0 && config.FolderIndex < FileOperate.RootFiles.Count)
            {
                config.Path = FileOperate.RootFiles[config.FolderIndex].Path;

                if (config.FileIndex < 0) { config.FileIndex = 0; }
                if (config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count) { config.FileIndex = FileOperate.RootFiles[config.FolderIndex].Name.Count - 1; }
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
            if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
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

            //if (config.Error == 1) { config.Error = 0; this.Text = index + " [Wrong Password] " + config.Name; ShowErr(); return; }

            if (FileOperate.RootFiles.Count == 0) { this.Text = "[Empty] You can click right button to input a folder to start"; ShowNot(); return; }
            if (!config.ExistFolder) { this.Text = "[Not Exist] " + config.Path; ShowNot(); return; }
            if (FileOperate.RootFiles[config.FolderIndex].Name.Count == 0) { this.Text = "[0/0] [Empty Folder] Current Root Folder Is Empty !"; ShowNot(); return; }
            if (!config.ExistFile) { this.Text = index + " [Not Exist] " + config.Name; ShowNot(); return; }
            if (config.Type == 1 && config.SubFiles.Count == 0) { this.Text = index + " " + subindex + " [Empty Folder] " + config.Name; ShowNot(); return; }
            if (config.Type == 5 && config.SubFiles.Count == 0) { this.Text = index + " " + subindex + " [Empty File] " + config.Name; ShowNot(); return; }

            #endregion

            #region 隐藏文件不予显示

            if (!FileSupport.SupportHide && (!config.IsSub ? config.Hide : config.SubHide))
            {
                this.Text = config.IsSub ?
                    index + " " + subindex + " [Unknow] " + config.Name + " : " + config.SubName :
                    index + " [Unknow] " + config.Name;

                config.Type = -1; config.SubType = -1; ShowUnk(); return;
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
                if (!ZipOperate.Known)
                {
                    this.Text = index + " " + subindex + " [Wrong Password] " + config.Name + " : " + config.SubName;
                    ShowErr(); return;
                }

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

            this.titleToolStripMenuItem1.Text = config.ExistFolder ?
                this.toolTip1.ToolTipTitle = config.Path :
                "Not Exist";
            this.textToolStripMenuItem.Text = this.Text;

            this.axWindowsMediaPlayer1.Visible = false;
            this.pictureBox1.Visible = true;
            
            if (NextShowBigPicture) { ShowBig(); } else { ShowSmall(); }

            this.axWindowsMediaPlayer1.Visible = false;
            this.pictureBox1.Visible = true;
        }
        private void ShowGif(string path, string name, bool load = true)
        {
            if (load) { config.SourPicture = (Bitmap)Image.FromFile(path + "\\" + name); }

            this.titleToolStripMenuItem1.Text = config.ExistFolder ?
                this.toolTip1.ToolTipTitle = config.Path :
                "Not Exist";
            this.textToolStripMenuItem.Text = this.Text;

            this.axWindowsMediaPlayer1.Visible = false;
            this.pictureBox1.Visible = true;

            ShapeWindow();
            ShowBig();
            
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
            
            ShapeWindow();
            ShapeControl();

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
            if (File.Exists(errpath + "\\" + errname)) { ShowPicture(errpath, errname); }
            else { ShowOff(); }
        }
        private void ShowBig(bool focus = false)
        {
            // 该函数只有2个功能
            // 源图太大，则放大。
            // 源图不大，则显示源图

            if (config.SourPicture == null) { return; }
            if (!focus) { SetScroll0(); }
            ShapeWindow();
            
            // 聚焦点
            int xF = MousePosition.X - this.Location.X - this.pictureBox1.Location.X;
            if (UseBoard) { xF -= BoardSize.Width / 2; }
            int yF = MousePosition.Y - this.Location.Y - this.pictureBox1.Location.Y;
            if (UseBoard) { yF -= 30; }
            double xR = focus ? (double)xF / config.DestPicture.Width : 0;
            if (xR < 0) { xR = 0; }
            if (xR > 1) { xR = 1; }
            double yR = focus ? (double)yF / config.DestPicture.Height : 0;
            if (yR < 0) { yR = 0; }
            if (yR > 1) { yR = 1; }

            // 显示图片（能够显示源图则显示源图）
            this.pictureBox1.Image = config.SourPicture;
            ShapeControl();
            
            // 把聚焦点放到屏幕中央
            int width = UseBoard ? this.Width - BoardSize.Width : this.Width;
            int xS = (int)(config.SourPicture.Width * xR - width / 2);
            if (xS < this.HorizontalScroll.Minimum) { xS = this.HorizontalScroll.Minimum; }
            if (xS > this.HorizontalScroll.Maximum) { xS = this.HorizontalScroll.Maximum; }

            int height = UseBoard ? this.Height - BoardSize.Height : this.Height;
            int yS = (int)(config.SourPicture.Height * yR - height / 2);
            if (yS < this.VerticalScroll.Minimum) { yS = this.VerticalScroll.Minimum; }
            if (yS > this.VerticalScroll.Maximum) { yS = this.VerticalScroll.Maximum; }

            SetScrollW(xS); SetScrollH(yS);
        }
        private void ShowSmall()
        {
            // 按窗体大小显示图片。
            // 在打开边框时，若图片太小，窗体过大，则显示源图。

            if (config.SourPicture == null) { return; }
            SetScroll0();
            ShapeWindow();

            int ch = UseBoard ? this.Height - BoardSize.Height : this.Height;
            int cw = UseBoard ? this.Width - BoardSize.Width : this.Width;
            int sourX = config.SourPicture.Width;
            int sourY = config.SourPicture.Height;

            if (ch == 0 || cw == 0) { ch = MinWindowSize.Height; cw = MinWindowSize.Width; }

            double ratex = (double)cw / sourX;
            double ratey = (double)ch / sourY;
            double rate = Math.Min(ratex, ratey);

            int destX = (int)(sourX * rate);
            int destY = (int)(sourY * rate);
            if (Math.Abs(destX - cw) <= 2) { destX = cw; }
            if (Math.Abs(destY - ch) <= 2) { destY = ch; }

            if (destX > config.SourPicture.Width && destY > config.SourPicture.Height)
            {
                destX = config.SourPicture.Width;
                destY = config.SourPicture.Height;
            }

            config.DestPicture = (Image)new Bitmap(destX, destY);
            Graphics g = Graphics.FromImage(config.DestPicture);
            g.DrawImage(config.SourPicture, new Rectangle(0, 0, destX, destY), new Rectangle(0, 0, sourX, sourY), GraphicsUnit.Pixel);
            g.Dispose();
            
            this.pictureBox1.Image = config.DestPicture;
            SetScroll0();
            ShapeControl();
        }
        private void ShowRate()
        {
            // 强行按比例放缩图片
            // 不能放出滚动条

            int type = config.IsSub ? config.SubType : config.Type;
            if (type != 2 && type != -1 && type != 0) { return; }
            if (config.SourPicture == null) { return; }

            //if (UseShapeWindow) { ShowSmall(); return; }

            SetScroll0();
            ShapeWindow(true);

            int ch = UseBoard ? this.Height - BoardSize.Height : this.Height;
            int cw = UseBoard ? this.Width - BoardSize.Width : this.Width;
            int sh = Screen.PrimaryScreen.Bounds.Height;
            int sw = Screen.PrimaryScreen.Bounds.Width;
            int wh = sh * ShapeWindowRate / 100;
            int ww = sw * ShapeWindowRate / 100; // 虚拟窗口大小

            // 如果已经是裁剪窗口模式，则不必再虚拟窗口大小
            if (UseShapeWindow && !UseBoard) { ww = cw; wh = ch; }

            int sourX = config.SourPicture.Width;
            int sourY = config.SourPicture.Height;
            double ratex = (double)ww / sourX;
            double ratey = (double)wh / sourY;
            double rate = Math.Min(ratex, ratey);
            int destX = UseShapeWindow && !UseBoard ? cw : (int)(sourX * rate);
            int destY = UseShapeWindow && !UseBoard ? ch : (int)(sourY * rate);

            if (config.DestPicture != null) { config.DestPicture.Dispose(); }
            config.DestPicture = (Image)new Bitmap(destX, destY);
            Graphics g = Graphics.FromImage(config.DestPicture);
            g.DrawImage(config.SourPicture, new Rectangle(0, 0, destX, destY), new Rectangle(0, 0, sourX, sourY), GraphicsUnit.Pixel);
            g.Dispose();

            this.pictureBox1.Image = config.DestPicture;
            SetScroll0();
            ShapeControl(true);
        }
        private void ShowBoard(bool show)
        {
            if (UseBoard == show) { return; }
            
            int centreh = this.Location.Y + this.Height / 2;
            int centrew = this.Location.X + this.Width / 2;

            int clienth = UseBoard ? this.Height - BoardSize.Height : this.Height;
            int clientw = UseBoard ? this.Width - BoardSize.Width : this.Width;

            int xscroll = this.HorizontalScroll.Value;
            int yscroll = this.VerticalScroll.Value;

            UseBoard = show;
            if (show) { this.FormBorderStyle = FormBorderStyle.Sizable; }
            else { this.FormBorderStyle = FormBorderStyle.None; }

            this.Height = UseBoard ? clienth + BoardSize.Height : clienth;
            this.Width = UseBoard ? clientw + BoardSize.Width : clientw;

            if (show) { this.Location = new Point(this.Location.X - BoardSize.Width, this.Location.Y - BoardSize.Height); }
            else { this.Location = new Point(this.Location.X + BoardSize.Width, this.Location.Y + BoardSize.Height); }
            
            SetScrollW(xscroll); SetScrollH(yscroll);

            if (!show)
            {
                this.tipToolStripMenuItem.Checked = true;
                // 当不显示边框时
                // 音乐文件的窗体大小需要切换

                int type = config.IsSub ? config.SubType : config.Type;
                bool isMusic = config.IsSub ? FileOperate.IsMusic(config.SubExtension) : FileOperate.IsMusic(config.Extension);
                if (type == 4 && isMusic) { ShapeWindow(); ShapeControl(); }
            }
            else
            {
                this.tipToolStripMenuItem.Checked = false;
                // 当显示边框时
                // 音乐文件的窗体大小需要切换
                // 图片文件窗口过小，重新调整窗口

                int type = config.IsSub ? config.SubType : config.Type;
                bool isMusic = config.IsSub ? FileOperate.IsMusic(config.SubExtension) : FileOperate.IsMusic(config.Extension);

                int rech = UseBoard ? this.Height - BoardSize.Height : this.Height;
                int recw = UseBoard ? this.Width - BoardSize.Width : this.Width;
                bool PicWindowIsOK = true;
                int picx = this.pictureBox1.Location.X;
                int picy = this.pictureBox1.Location.Y;
                int pich = this.pictureBox1.Height;
                int picw = this.pictureBox1.Width;
                if (PicWindowIsOK && pich >= rech) { PicWindowIsOK = picy <= 1; }
                if (PicWindowIsOK && picw >= recw) { PicWindowIsOK = picx <= 1; }
                if (PicWindowIsOK && pich < rech)
                {
                    int stdy = UseBoard ? (this.Height - BoardSize.Height - pich) / 2 : (this.Height - pich) / 2;
                    PicWindowIsOK = Math.Abs(stdy - picy) <= 1;
                }
                if (PicWindowIsOK && picw < recw)
                {
                    int stdx = UseBoard ? (this.Width - BoardSize.Width - picw) / 2 : (this.Width - picw) / 2;
                    PicWindowIsOK = Math.Abs(stdx - picx) <= 1;
                }

                if (type == -1 || type == 0 || type == 2 || type == 3)
                {
                    if (!PicWindowIsOK) { ShowCurrent(); }
                }
                if (type == 4 && isMusic) { ShapeWindow(); ShapeControl(); }
            }

            //this.Location = new Point(centrew - this.Width / 2, centreh - this.Height / 2);
            if (show) { this.toolTip1.Dispose(); } else { this.toolTip1 = new ToolTip(); }
        }
        private void ShapeWindow(bool zoom = false)
        {
            if (!UseShapeWindow) { return; }
            int sh = Screen.PrimaryScreen.Bounds.Height;
            int sw = Screen.PrimaryScreen.Bounds.Width;

            int shapeh = Math.Min(sh / 4, sw / 4);
            int shapew = shapeh;

            int inith = this.Height;
            int initw = this.Width;

            int centreh = this.Location.Y + this.Height / 2;
            int centrew = this.Location.X + this.Width / 2;

            int type = config.IsSub ? config.SubType : config.Type;
            bool isMusic = config.IsSub ? FileOperate.IsMusic(config.SubExtension) : FileOperate.IsMusic(config.Extension);
            bool isVideo = config.IsSub ? FileOperate.IsVideo(config.SubExtension) : FileOperate.IsVideo(config.Extension);

            #region 隐藏文件、错误提示的大小

            if ((type == -1 || type == 0) && config.SourPicture != null)
            {
                int maxh = sh * ShapeWindowRate / 100;
                int maxw = sw * ShapeWindowRate / 100;
                int pich = config.SourPicture.Height;
                int picw = config.SourPicture.Width;

                double h2w = (double)pich / picw;
                double rate1 = (double)maxh / pich;
                double rate2 = (double)maxw / picw;

                if (rate1 <= rate2) { shapeh = maxh; shapew = (int)(shapeh / h2w); }
                else { shapew = maxw; shapeh = (int)(shapew * h2w); }

                //if (!zoom && MaxWindowSize.Height > config.SourPicture.Height && MaxWindowSize.Width > config.SourPicture.Width)
                //{ shapeh = config.SourPicture.Height; shapew = config.SourPicture.Width; }
                if (!zoom && shapeh > config.SourPicture.Height && shapew > config.SourPicture.Width)
                { shapeh = config.SourPicture.Height; shapew = config.SourPicture.Width; }
            }

            #endregion

            #region 图片文件自适应窗体大小

            if (type == 2 && config.SourPicture != null)
            {
                int maxh = sh * ShapeWindowRate / 100;
                int maxw = sw * ShapeWindowRate / 100;
                int pich = config.SourPicture.Height;
                int picw = config.SourPicture.Width;

                double h2w = (double)pich / picw;
                double rate1 = (double)maxh / pich;
                double rate2 = (double)maxw / picw;

                if (rate1 <= rate2) { shapeh = maxh; shapew = (int)(shapeh / h2w); }
                else { shapew = maxw; shapeh = (int)(shapew * h2w); }

                //if (!zoom && MaxWindowSize.Height > config.SourPicture.Height && MaxWindowSize.Width > config.SourPicture.Width)
                //{ shapeh = config.SourPicture.Height; shapew = config.SourPicture.Width; }
                if (!zoom && shapeh > config.SourPicture.Height && shapew > config.SourPicture.Width)
                { shapeh = config.SourPicture.Height; shapew = config.SourPicture.Width; }
            }

            #endregion

            #region GIF 文件自适应窗体大小

            if (type == 3 && config.SourPicture != null)
            {
                int maxh = sh * ShapeWindowRate / 100;
                int maxw = sw * ShapeWindowRate / 100;
                int pich = config.SourPicture.Height;
                int picw = config.SourPicture.Width;

                double h2w = (double)pich / picw;
                double rate1 = (double)maxh / pich;
                double rate2 = (double)maxw / picw;

                if (rate1 <= rate2) { shapeh = maxh; shapew = (int)(shapeh / h2w); }
                else { shapew = maxw; shapeh = (int)(shapew * h2w); }

                //if (!zoom && MaxWindowSize.Height > config.SourPicture.Height && MaxWindowSize.Width > config.SourPicture.Width)
                //{ shapeh = config.SourPicture.Height; shapew = config.SourPicture.Width; }
                if (!zoom && shapeh > config.SourPicture.Height && shapew > config.SourPicture.Width)
                { shapeh = config.SourPicture.Height; shapew = config.SourPicture.Width; }
            }

            #endregion

            #region 音频文件自适应窗体大小

            if (type == 4 && isMusic)
            {
                if (UseBoard) { shapeh = shapew = Math.Min(sh / 5, sw / 5); }
                else { shapeh = shapew = Math.Min(sh / 10, sw / 10); }
            }

            #endregion

            #region 视频文件自适应窗体大小

            if (type == 4 && isVideo)
            {
                shapeh = sh / 2;
                shapew = sw / 2;
            }

            #endregion

            
            int newh = UseBoard ? shapeh + BoardSize.Height : shapeh;
            int neww = UseBoard ? shapew + BoardSize.Width : shapew;
            this.Height = newh;
            this.Width = neww;
            int setx = centrew - this.Width / 2;
            int sety = centreh - this.Height / 2;
            this.Location = new Point(setx, sety);
        }
        private void ShapeControl(bool zoom = false)
        {
            int type = config.IsSub ? config.SubType : config.Type;
            int ch = UseBoard ? this.Height - BoardSize.Height : this.Height;
            int cw = UseBoard ? this.Width - BoardSize.Width : this.Width;

            #region picture box

            if (type == -1 || type == 0 || type == 2)
            {
                if (config.SourPicture == null) { return; }
                if (!NextShowBigPicture && config.DestPicture == null) { return; }

                int shapeh = 0, shapew = 0;
                if (NextShowBigPicture || type == 3) { shapeh = config.SourPicture.Height; shapew = config.SourPicture.Width; }
                else { shapeh = config.DestPicture.Height; shapew = config.DestPicture.Width; }
                //if (config.SourPicture.Height < MaxWindowSize.Height && config.SourPicture.Width < MaxWindowSize.Width)
                //{ shapeh = config.SourPicture.Height; shapew = config.SourPicture.Width; }
                if (config.SourPicture.Height < ch && config.SourPicture.Width < cw)
                { shapeh = config.SourPicture.Height; shapew = config.SourPicture.Width; }
                if (zoom) { shapeh = config.DestPicture.Height; shapew = config.DestPicture.Width; }
                
                int recth = UseBoard ? this.Height - BoardSize.Height : this.Height; ;
                int rectw = UseBoard ? this.Width - BoardSize.Width : this.Width; ;
                int x = shapew > rectw ? 0 : (rectw - shapew) / 2;
                int y = shapeh > recth ? 0 : (recth - shapeh) / 2;
                
                this.pictureBox1.Location = new Point(x, y);
                this.pictureBox1.Height = shapeh;
                this.pictureBox1.Width = shapew;
            }

            if (type == 3)
            {
                if (config.SourPicture == null) { return; }
                int shapeh = config.SourPicture.Height;
                int shapew = config.SourPicture.Width;

                int recth = UseBoard ? this.Height - BoardSize.Height : this.Height; ;
                int rectw = UseBoard ? this.Width - BoardSize.Width : this.Width; ;
                int x = shapew > rectw ? 0 : (rectw - shapew) / 2;
                int y = shapeh > recth ? 0 : (recth - shapeh) / 2;

                this.pictureBox1.Height = shapeh;
                this.pictureBox1.Width = shapew;
                this.pictureBox1.Location = new Point(x, y);
            }

            #endregion

            #region wmp

            if (type == 4)
            {
                this.axWindowsMediaPlayer1.Height = ch - 2;
                this.axWindowsMediaPlayer1.Width = cw - 2;
                this.axWindowsMediaPlayer1.Location = new Point(1, 1);
            }

            #endregion
        }
        private void SetScroll0()
        {
            this.VerticalScroll.Value = 0;
            this.HorizontalScroll.Value = 0;
        }
        private void SetScrollH(int value)
        {
            if (value < this.VerticalScroll.Minimum) { value = this.VerticalScroll.Minimum; }
            if (value > this.VerticalScroll.Maximum) { value = this.VerticalScroll.Maximum; }

            int current = this.VerticalScroll.Value;
            int pace = this.VerticalScroll.LargeChange;
            int adjust = value - current;

            while (adjust > pace || adjust < -pace)
            {
                int move = adjust > 0 ? pace : -pace;
                this.VerticalScroll.Value += move;
                adjust = value - this.VerticalScroll.Value;
            }

            this.VerticalScroll.Value = value;
            this.VerticalScroll.Value = value;
        }
        private void SetScrollW(int value)
        {
            if (value < this.HorizontalScroll.Minimum) { value = this.HorizontalScroll.Minimum; }
            if (value > this.HorizontalScroll.Maximum) { value = this.HorizontalScroll.Maximum; }

            int current = this.HorizontalScroll.Value;
            int pace = this.HorizontalScroll.LargeChange;
            int adjust = value - current;

            while (adjust > pace || adjust < -pace)
            {
                int move = adjust > 0 ? pace : -pace;
                this.HorizontalScroll.Value += move;
                adjust = value - this.HorizontalScroll.Value;
            }

            this.HorizontalScroll.Value = value;
            this.HorizontalScroll.Value = value;
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
            this.IsActive = true;

            if (config.Type == 4 || (config.IsSub && config.SubType == 4)) { ShapeControl(); return; }
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
            this.contextMenuStrip1.Hide();

            // 判断是否存在文件
            if (!config.ExistFile || !config.ExistFolder || (config.IsSub && !config.ExistSub))
            { MessageBox.Show("当前文件不存在！", "提示"); return; }
            
            // 获取输出文件进行输出
            string path = config.Type == 1 ? config.Path + "\\" + config.Name : config.Path;
            string name = config.Type == 1 ? config.SubName : config.Name;

            // 是否已经存在文件或文件夹
            string expath = config.ExportFolder;
            if (File.Exists(expath + "\\" + name)) { MessageBox.Show("本文件已经存在于输出文件夹中：" + expath, "提示"); return; }

            // 确认是否输出
            if (DialogResult.Cancel == MessageBox.Show("把 “" + name + "” 导出？", "确认导出", MessageBoxButtons.OKCancel))
            { return; }

            // 释放资源
            if (config.SourPicture != null) { config.SourPicture.Dispose(); }
            if (config.DestPicture != null) { config.DestPicture.Dispose(); }
            if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            { this.axWindowsMediaPlayer1.Ctlcontrols.stop(); }
            if (config.Type == 5) { ZipOperate.Dispose(); }

            // 输出文件
            File.Move(path + "\\" + name, expath + "\\" + name);
            if (config.Type != 1) { FileOperate.RootFiles[config.FolderIndex].Name.RemoveAt(config.FileIndex); }
            ShowCurrent();
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
        private void RightMenu_Export_ExportFolder(object sender, EventArgs e)
        {
            if (!config.ExistFolder || !config.ExistFile) { MessageBox.Show("文件夹不存在或者当前项目不是文件夹！", "提示"); return; }
            if (config.Type != 1) { MessageBox.Show("文件夹不存在或者当前项目不是文件夹！", "提示"); return; }

            string exportpath = config.ExportFolder + "\\" + config.Name;
            if (Directory.Exists(exportpath))
            { MessageBox.Show("输出目录已经存在：\n" + exportpath + "！", "错误"); return; }

            string sourpath = config.Path + "\\" + config.Name;
            if (Directory.Exists(exportpath))
            { MessageBox.Show("当前文件夹不存在！\n" + sourpath, "错误"); return; }
            DirectoryInfo dir = new DirectoryInfo(sourpath);
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] folders = dir.GetDirectories();

            bool makeSure = false;
            if (!makeSure && folders.Length != 0 || config.SubFiles.Count != files.Length)
            {
                if (DialogResult.Cancel == MessageBox.Show("该文件夹还存在其他文件夹或文件，导出后会删除未识别文件夹或文件。\n是否导出？", "确认导出", MessageBoxButtons.OKCancel))
                { return; }

                makeSure = true;
            }

            if (!makeSure)
            {
                if (DialogResult.Cancel == MessageBox.Show("把文件夹 “" + config.Name + "” 导出？", "确认导出", MessageBoxButtons.OKCancel))
                { return; }

                makeSure = true;
            }

            if (!makeSure && config.SubFiles.Count == 0)
            {
                if (DialogResult.Cancel == MessageBox.Show("该文件夹为空文件夹，是否导出？", "确认导出", MessageBoxButtons.OKCancel))
                { return; }
            }

            try { Directory.CreateDirectory(exportpath); } catch { MessageBox.Show("创建目录失败！", "提示"); return; }

            if (config.SourPicture != null) { config.SourPicture.Dispose(); }
            if (config.DestPicture != null) { config.DestPicture.Dispose(); }
            if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            { this.axWindowsMediaPlayer1.Ctlcontrols.stop(); }
            
            for (int i = 0; i < config.SubFiles.Count; i++)
            {
                File.Move(config.Path + "\\" + config.Name + "\\" + config.SubFiles[i], exportpath + "\\" + config.SubFiles[i]);
            }
            Directory.Delete(config.Path + "\\" + config.Name);
            FileOperate.RootFiles[config.FolderIndex].Name.RemoveAt(config.FileIndex);
            ShowCurrent();
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
            config.SubIndex = search.SubIndex;
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
            NextShowBigPicture = !this.bigPicToolStripMenuItem.Checked;
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
            //bool hide = config.IsSub ? config.SubHide : config.Hide;
            //hide = !NoHide && hide;
            //if (hide) { MessageBox.Show("不能匹配图片之外的文件", "提示"); return; }

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

            // 是否跳转
            if (Form_Find.IsSwitch) { ShowCurrent(); }
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
            if (FileOperate.RootFiles.Count == 0) { return; }
            
            int currFolder = config.FolderIndex;
            int currFile = config.FileIndex;
            int currSub = config.SubIndex;

            int nextFolder = currFolder;
            int nextFile = currFile;
            int nextSub = currSub;
            
            if (config.IsSub && currSub > 0) { config.SubIndex--; ShowCurrent(); return; }
            //WheelPageTime = ulong.MaxValue;
            bool emptySub = config.SubFiles.Count == 0;
            bool outSub = config.IsSub && !emptySub && DialogResult.OK == MessageBox.Show("已经是该文件夹的第一个文件了，是否跳出当前文件夹？\n\n" + config.Name, "请确认", MessageBoxButtons.OKCancel);
            //WheelPageTime = TimeCount;
            if (config.IsSub && !outSub && !emptySub) { return; }

            nextFile--; nextSub = int.MaxValue;
            if (nextFile < 0) { nextFile = int.MaxValue; nextFolder--; }
            if (nextFolder < 0) { nextFolder = int.MaxValue; }
            //WheelPageTime = ulong.MaxValue;
            bool emptyFolder = FileOperate.getIndexName(currFolder, 0) == null;
            bool outFolder = nextFolder != currFolder && !emptyFolder && DialogResult.OK == MessageBox.Show("已经是该路径的第一个文件了，是否跳出当前路径？\n\n " + config.Path, "请确认", MessageBoxButtons.OKCancel);
            //WheelPageTime = TimeCount;
            if (nextFolder != currFolder && !outFolder && !emptyFolder) { return; }

            config.FolderIndex = nextFolder;
            config.FileIndex = nextFile;
            config.SubIndex = nextSub;
            ShowCurrent();
        }
        private void RightMenu_Next(object sender, EventArgs e)
        {
            if (FileOperate.RootFiles.Count == 0) { return; }
            
            int currFolder = config.FolderIndex;
            int currFile = config.FileIndex;
            int currSub = config.SubIndex;

            int nextFolder = currFolder;
            int nextFile = currFile;
            int nextSub = currSub;

            if (config.IsSub && currSub != config.SubFiles.Count - 1) { config.SubIndex++; ShowCurrent(); return; }
            //WheelPageTime = ulong.MaxValue;
            bool emptySub = config.SubFiles.Count == 0;
            bool outSub = config.IsSub && !emptySub && DialogResult.OK == MessageBox.Show("已经是该文件夹的最后一个文件了，是否跳出当前文件夹？\n\n" + config.Name, "请确认", MessageBoxButtons.OKCancel);
            //WheelPageTime = TimeCount;
            if (config.IsSub && !outSub && !emptySub) { return; }

            nextFile++; nextSub = 0;
            if (!FileOperate.ExistFile(nextFolder, nextFile)) { nextFile = 0; nextFolder++; }
            if (!FileOperate.ExistFolder(nextFolder)) { nextFolder = 0; }
            //WheelPageTime = ulong.MaxValue;
            bool emptyFolder = FileOperate.getIndexName(currFolder, 0) == null;
            bool outFolder = nextFolder != currFolder && !emptyFolder && DialogResult.OK == MessageBox.Show("已经是该路径的最后一个文件了，是否跳出当前路径？\n\n" + config.Path, "请确认", MessageBoxButtons.OKCancel);
            //WheelPageTime = TimeCount;
            if (nextFolder != currFolder && !outFolder && !emptyFolder) { return; }

            config.FolderIndex = nextFolder;
            config.FileIndex = nextFile;
            config.SubIndex = nextSub;
            ShowCurrent();
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

        private void Form_Main_Activated(object sender, EventArgs e)
        {
            IsActive = true;
        }
        private void Form_Main_Deactivate(object sender, EventArgs e)
        {
            IsActive = false;
        }
    }
}
