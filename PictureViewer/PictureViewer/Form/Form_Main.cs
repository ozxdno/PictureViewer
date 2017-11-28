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
        /// 边框尺寸
        /// </summary>
        private Size BoardSize;
        /// <summary>
        /// 客户区长度
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
        /// 客户区距窗体顶端的距离
        /// </summary>
        private int ClientTop;
        /// <summary>
        /// 客户区距窗体左端的距离
        /// </summary>
        private int ClientLeft;
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
        private double ShapeWindowRate = 80;
        /// <summary>
        /// 窗体最大大小
        /// </summary>
        private int MaxWindowSize;
        /// <summary>
        /// 窗体最小大小
        /// </summary>
        private int MinWindowSize;
        /// <summary>
        /// 鼠标位于拉伸窗体的位置
        /// </summary>
        private bool Resizable;
        /// <summary>
        /// 该窗体是否被激活
        /// </summary>
        private bool IsActive;
        /// <summary>
        /// 图片窗口
        /// </summary>
        private List<Form_Image> Images = new List<Form_Image>();
        /// <summary>
        /// 搜索界面。
        /// </summary>
        private Form_Search Search = null;
        /// <summary>
        /// 查找界面。
        /// </summary>
        private Form_Find Find = null;

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
        /// 播放设置
        /// </summary>
        private PLAY play;

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
            public Point pDown2;
            public Point pUp2;

            public ulong tWheel;
            
            public Point pWindow;
            public int xScroll;
            public int yScroll;

            public bool Resizing;
            public Size Size;
            public bool ResizeL;
            public bool ResizeR;
            public bool ResizeU;
            public bool ResizeD;

            public int xImage;
            public int yImage;
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
        /// <summary>
        /// 播放设置
        /// </summary>
        private struct PLAY
        {
            /// <summary>
            /// 是否需要播放
            /// </summary>
            public bool IsPlaying;
            /// <summary>
            /// 播放序号
            /// </summary>
            public int Index;
            /// <summary>
            /// 正在显示的文件类型
            /// 1 - 保留；
            /// 2 - 图片；
            /// 3 - Gif；
            /// 4 - 音乐；
            /// 5 - 视频；
            /// </summary>
            public int Type;
            /// <summary>
            /// 起始播放时间
            /// </summary>
            public ulong Begin;
            
            public List<int> FolderIndexes;
            public List<int> FileIndexes;
            public List<int> SubIndexes;
            public List<int> PlayIndexes;

            public bool TotalRoots;
            public bool Root;
            public bool Subroot;

            public bool Forward;
            public bool Backward;

            public bool Picture;
            public bool Gif;
            public bool Music;
            public bool Video;

            public bool Single;
            public bool Order;
            public bool Circle;
            public bool Rand;

            public ulong ShowTime;
            public bool FoundNext;
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
            #region 配置文件、图片文件

            config.ConfigPath = FileOperate.getExePath();
            config.ConfigName = "pv.pvini";
            Class.Load.Load_CFG();

            //Class.Load.Load_PIC();
            Class.Load.Initialize.Start();

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

            this.fullToolStripMenuItem.Visible = false;
            this.partToolStripMenuItem.Visible = false;
            this.sameToolStripMenuItem.Visible = false;
            this.likeToolStripMenuItem.Visible = false;
            this.turnToolStripMenuItem.Visible = false;

            #endregion

            #region 边框，缩放，窗体

            BoardSize = this.Size - this.ClientSize;
            Point ptTL = this.PointToScreen(new Point(0, 0));
            ClientTop = ptTL.Y - this.Location.Y;
            ClientLeft = ptTL.X - this.Location.X;

            if (!Class.Load.settings.Form_Main_UseBoard) { this.FormBorderStyle = FormBorderStyle.None; }
            UseBoard = Class.Load.settings.Form_Main_UseBoard;
            UseShapeWindow = Class.Load.settings.Form_Main_ShapeWindow;
            ShapeWindowRate = Class.Load.settings.Form_Main_ShapeWindowRate;
            this.shapeToolStripMenuItem.Checked = UseShapeWindow;
            
            MaxWindowSize= Class.Load.settings.Form_Main_MaxWindowSize;
            MinWindowSize = Class.Load.settings.Form_Main_MinWindowSize;

            this.UseSmallWindowOpen = Class.Load.settings.Form_Main_UseSmallWindowOpen;
            this.Height = Class.Load.settings.Form_Main_Height;
            this.Width = Class.Load.settings.Form_Main_Width;
            this.Location = new Point(Class.Load.settings.Form_Main_Location_X, Class.Load.settings.Form_Main_Location_Y);

            if (UseSmallWindowOpen)
            {
                int sh = Screen.PrimaryScreen.Bounds.Height;
                int sw = Screen.PrimaryScreen.Bounds.Width;

                int side = Math.Min(MinWindowSize * sh / 100, MinWindowSize * sw / 100);
                this.Height = side;
                this.Width = side;
                this.Location = new Point((sw - this.Width) / 2, (sh - this.Height) / 2);
                ShapeWindowRate = MinWindowSize;
            }

            #endregion
            
            #region 其他，参数初始化

            this.lockToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Lock;

            tip.Previous = new Point(0, 0);
            tip.Message = "";
            tip.Form = new Form_Tip();
            tip.Begin = 0;
            tip.Visible = false;
            this.tipToolStripMenuItem.Checked = Class.Load.settings.Form_Main_Tip;

            #endregion

            #region play

            play.IsPlaying = false;
            play.Index = 0;
            play.Type = -1;
            play.Begin = 0;
            play.TotalRoots = Class.Load.settings.Form_Main_Play_TotalRoots;
            play.Root = Class.Load.settings.Form_Main_Play_Root;
            play.Subroot = Class.Load.settings.Form_Main_Play_Subroot;
            play.FolderIndexes = new List<int>();
            play.FileIndexes = new List<int>();
            play.SubIndexes = new List<int>();
            play.PlayIndexes = new List<int>();
            play.Forward = Class.Load.settings.Form_Main_Play_Forward;
            play.Backward = Class.Load.settings.Form_Main_Play_Backward;
            play.Picture = Class.Load.settings.Form_Main_Play_Picture;
            play.Gif = Class.Load.settings.Form_Main_Play_Gif;
            play.Music = Class.Load.settings.Form_Main_Play_Music;
            play.Video = Class.Load.settings.Form_Main_Play_Video;
            play.Single = Class.Load.settings.Form_Main_Play_Single;
            play.Order = Class.Load.settings.Form_Main_Play_Order;
            play.Circle = Class.Load.settings.Form_Main_Play_Circle;
            play.Rand = Class.Load.settings.Form_Main_Play_Rand;
            play.ShowTime = (ulong)(Class.Load.settings.Form_Main_Play_ShowTime);
            play.FoundNext = false;

            this.forwardToolStripMenuItem.Checked = play.Forward;
            this.backwardToolStripMenuItem.Checked = play.Backward;
            this.totalRootsToolStripMenuItem.Checked = play.TotalRoots;
            this.rootToolStripMenuItem.Checked = play.Root;
            this.subrootToolStripMenuItem.Checked = play.Subroot;
            this.pictureToolStripMenuItem.Checked = play.Picture;
            this.gifToolStripMenuItem.Checked = play.Gif;
            this.musicToolStripMenuItem.Checked = play.Music;
            this.videoToolStripMenuItem.Checked = play.Video;
            this.singleToolStripMenuItem.Checked = play.Single;
            this.orderToolStripMenuItem.Checked = play.Order;
            this.circleToolStripMenuItem.Checked = play.Circle;
            this.randToolStripMenuItem.Checked = play.Rand;
            this.showTimeToolStripMenuItem.Text = "时间：" + play.ShowTime.ToString();

            #endregion

            #region 判断索引号是否有效，调整索引号，显示初始文件

            config.SubFiles = new List<string>();
            //if (config.FolderIndex < 0 || config.FolderIndex >= FileOperate.RootFiles.Count) { config.FolderIndex = 0; }
            //if (config.FileIndex < 0 || FileOperate.RootFiles.Count == 0 || config.FileIndex >= FileOperate.RootFiles[config.FolderIndex].Name.Count)
            //{ config.FileIndex = 0; }

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
            mouse.tWheel = 0;

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
            if (Search != null && !Search.IsDisposed) { Search.Visible = false; }
            if (Find != null && !Find.IsDisposed) { Find.Visible = false; }
            CloseImages();

            Class.Save.settings.Form_Main_Hide = this.hideToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Hide_L = this.hideLToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Hide_R = this.hideRToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Hide_U = this.hideUToolStripMenuItem.Checked;
            Class.Save.settings.Form_Main_Hide_D = this.hideDToolStripMenuItem.Checked;

            //Class.Save.settings.Form_Main_Find_Full = this.fullToolStripMenuItem.Checked;
            //Class.Save.settings.Form_Main_Find_Part = this.partToolStripMenuItem.Checked;
            //Class.Save.settings.Form_Main_Find_Same = this.sameToolStripMenuItem.Checked;
            //Class.Save.settings.Form_Main_Find_Like = this.likeToolStripMenuItem.Checked;
            //Class.Save.settings.Form_Main_Find_Turn = this.turnToolStripMenuItem.Checked;
            if (Find != null && !Find.IsDisposed) { Find.SaveConfig(); }

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

            Class.Save.settings.Form_Main_Play_Forward = play.Forward;
            Class.Save.settings.Form_Main_Play_Backward = play.Backward;
            Class.Save.settings.Form_Main_Play_TotalRoots = play.TotalRoots;
            Class.Save.settings.Form_Main_Play_Root = play.Root;
            Class.Save.settings.Form_Main_Play_Subroot = play.Subroot;
            Class.Save.settings.Form_Main_Play_Picture = play.Picture;
            Class.Save.settings.Form_Main_Play_Gif = play.Gif;
            Class.Save.settings.Form_Main_Play_Music = play.Music;
            Class.Save.settings.Form_Main_Play_Video = play.Video;
            Class.Save.settings.Form_Main_Play_Single = play.Single;
            Class.Save.settings.Form_Main_Play_Order = play.Order;
            Class.Save.settings.Form_Main_Play_Circle = play.Circle;
            Class.Save.settings.Form_Main_Play_Rand = play.Rand;
            Class.Save.settings.Form_Main_Play_ShowTime = (int)play.ShowTime;
            
            Timer.Close();
            Class.Save.Save_CFG();
            Class.Save.Save_PIC();
        }
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            #region 上下左右翻动滚动条

            if (e.KeyValue == Class.Load.settings.FastKey_Main_L) { key.L = true; }
            if (e.KeyValue == Class.Load.settings.FastKey_Main_U) { key.U = true; }
            if (e.KeyValue == Class.Load.settings.FastKey_Main_R) { key.R = true; }
            if (e.KeyValue == Class.Load.settings.FastKey_Main_D) { key.D = true; }
            key.Down = key.L || key.R || key.U || key.D;
            if (key.Down && !this.lockToolStripMenuItem.Checked && (this.HorizontalScroll.Visible || this.VerticalScroll.Visible))
            { return; }

            #endregion

            #region 上一项

            if (e.KeyValue == Class.Load.settings.FastKey_Main_PageU)
            {
                RightMenu_Previous(null, null); return;
            }

            #endregion

            #region 下一项

            if (e.KeyValue == Class.Load.settings.FastKey_Main_PageD)
            {
                RightMenu_Next(null, null); return;
            }

            #endregion

            #region 前一个

            if (e.KeyValue == Class.Load.settings.FastKey_Main_L)
            {
                Page_L(null, null); return;
            }

            #endregion

            #region 后一个

            if (e.KeyValue == Class.Load.settings.FastKey_Main_R)
            {
                Page_R(null, null); return;
            }

            #endregion

            #region 向上

            if (e.KeyValue == Class.Load.settings.FastKey_Main_U)
            {
                Page_U(null, null); return;
            }

            #endregion

            #region 向下

            if (e.KeyValue == Class.Load.settings.FastKey_Main_D)
            {
                Page_D(null, null); return;
            }

            #endregion

            #region 回车

            if (e.KeyValue == Class.Load.settings.FastKey_Main_Enter)
            {
                Form_Main_DoubleClick(null, null);
            }

            #endregion

            #region 删除

            if (e.KeyValue == Class.Load.settings.FastKey_Main_Export)
            {
                RightMenu_Export(0, null);
            }

            #endregion

            #region 打开输出目录

            if (e.KeyValue == Class.Load.settings.FastKey_Main_OpenExport)
            {
                RightMenu_OpenExport(null, null); return;
            }

            #endregion

            #region 打开根目录

            if (e.KeyValue == Class.Load.settings.FastKey_Main_OpenRoot)
            {
                RightMenu_OpenRoot(null, null); return;
            }

            #endregion

            #region 打开漫画目录

            if (e.KeyValue == Class.Load.settings.FastKey_Main_OpenComic)
            {
                RightMenu_OpenComic(null, null); return;
            }

            #endregion

            #region 打开当前文件

            if (e.KeyValue == Class.Load.settings.FastKey_Main_OpenCurrent)
            {
                RightMenu_OpenCurrent(null, null); return;
            }

            #endregion

            #region P PASSWORD

            if (e.KeyValue == Class.Load.settings.FastKey_Main_Password)
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
                if (input.Input.Length > 5 && input.Input.Substring(0, 5) == "#disk") { ReplaceDisk(input.Input); return; }

                if (ZipOperate.SupportZip && input.Input.Length != 0 && input.Input[0] != '-')
                { ZipOperate.A_PassWord(input.Input); ShowCurrent(); }
                if (ZipOperate.SupportZip && input.Input.Length != 0 && input.Input[0] == '-')
                { ZipOperate.D_PassWord(input.Input); ShowCurrent(); }

                return;
            }

            #endregion

            #region ESC 退出

            if (e.KeyValue == Class.Load.settings.FastKey_Main_Esc)
            {
                int type = config.IsSub ? config.SubType : config.Type;
                if (FileOperate.IsPicture(type) || FileOperate.IsGif(type))
                {
                    if (Images.Count != 0)
                    {
                        //if (DialogResult.Cancel == MessageBox.Show("该界面为主界面，是否退出？", "确认退出", MessageBoxButtons.OKCancel))
                        //{ return; }
                    }
                }

                this.Visible = false; Form_Closed(null, null);
                Application.ExitThread();
            }

            #endregion

            #region A 显示/关闭 窗口外框

            if (e.KeyValue == Class.Load.settings.FastKey_Main_Board)
            {
                ShowBoard(!UseBoard); return;
            }

            #endregion

            #region 旋转

            if (e.KeyValue == Class.Load.settings.FastKey_Main_Rotate)
            {
                if (this.HorizontalScroll.Visible || this.VerticalScroll.Visible) { return; }

                int type = config.IsSub ? config.SubType : config.Type;
                if (type != 2) { return; }

                config.SourPicture.RotateFlip(RotateFlipType.Rotate90FlipNone);
                string path = config.IsSub ? config.Path + "\\" + config.Name : config.Path;
                string name = config.IsSub ? config.SubName : config.Name;
                try { config.SourPicture.Save(path + "\\" + name); } catch { }

                FilePicture.delete(FilePicture.getIndex(path, name));

                ShowPicture(null, null, false);
                return;
            }

            #endregion

            #region 翻转 X

            if (e.KeyValue == Class.Load.settings.FastKey_Main_FlipX)
            {
                if (this.HorizontalScroll.Visible || this.VerticalScroll.Visible) { return; }

                int type = config.IsSub ? config.SubType : config.Type;
                if (!FileOperate.IsPicture(type)) { return; }

                config.SourPicture.RotateFlip(RotateFlipType.RotateNoneFlipX);
                string path = config.IsSub ? config.Path + "\\" + config.Name : config.Path;
                string name = config.IsSub ? config.SubName : config.Name;
                try { config.SourPicture.Save(path + "\\" + name); } catch { }

                FilePicture.delete(FilePicture.getIndex(path, name));

                ShowPicture(null, null, false);
                return;
            }

            #endregion

            #region 翻转 Y

            if (e.KeyValue == Class.Load.settings.FastKey_Main_FlipY)
            {
                if (this.HorizontalScroll.Visible || this.VerticalScroll.Visible) { return; }

                int type = config.IsSub ? config.SubType : config.Type;
                if (!FileOperate.IsPicture(type)) { return; }

                config.SourPicture.RotateFlip(RotateFlipType.RotateNoneFlipY);
                string path = config.IsSub ? config.Path + "\\" + config.Name : config.Path;
                string name = config.IsSub ? config.SubName : config.Name;
                try { config.SourPicture.Save(path + "\\" + name); } catch { }

                FilePicture.delete(FilePicture.getIndex(path, name));

                ShowPicture(null, null, false);
                return;
            }

            #endregion
        }
        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == Class.Load.settings.FastKey_Main_L) { key.L = false; }
            if (e.KeyValue == Class.Load.settings.FastKey_Main_U) { key.U = false; }
            if (e.KeyValue == Class.Load.settings.FastKey_Main_R) { key.R = false; }
            if (e.KeyValue == Class.Load.settings.FastKey_Main_D) { key.D = false; }
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
                mouse.Down = true;
                mouse.Up = false;
                mouse.pDown = MousePosition;
                mouse.tDown = TimeCount;
                mouse.nDown++;

                mouse.pWindow = this.Location;
                mouse.xScroll = this.HorizontalScroll.Value;
                mouse.yScroll = this.VerticalScroll.Value;

                mouse.Resizing = Resizable;
                mouse.Size = this.Size;
            }

            if (e.nButton == 2)
            {
                mouse.Down2 = true;
                mouse.Up2 = false;
                mouse.pDown2 = MousePosition;
                mouse.tDown2 = TimeCount;
                mouse.pWindow = this.Location;
            }
        }
        private void WMP_MouseUp(object sender, AxWMPLib._WMPOCXEvents_MouseUpEvent e)
        {
            if (e.nButton == 1)
            {
                if (mouse.tUp != 0 && TimeCount - mouse.tUp < 20)
                { Form_Main_DoubleClick(null, null); }
                
                mouse.Down = false;
                mouse.Up = true;
                mouse.pUp = MousePosition;
                mouse.tUp = TimeCount;
                mouse.nUp++;

                mouse.Resizing = false;
            }
            if (e.nButton == 2)
            {
                mouse.Down2 = false;
                mouse.Up2 = true;
                mouse.pUp2 = MousePosition;
                mouse.tUp2 = TimeCount;

                bool showMenu = Math.Abs(mouse.pDown2.X - mouse.pUp2.X) < 10 &&
                    Math.Abs(mouse.pDown2.Y - mouse.pUp2.Y) < 10;
                if (showMenu) { this.contextMenuStrip1.Show(MousePosition); }
            }
        }
        
        private void Form_Main_DoubleClick(object sender, EventArgs e)
        {
            #region 播放时的双击操作

            if (play.IsPlaying && play.Type != 4 && play.Type != 5) { play.IsPlaying = false; return; }

            #endregion

            #region 判断位置是否摆正

            bool PicWindowIsOK = !NeedCorrectPictureBox();
            bool VidWindowIsOK = !NeedCorrectWMP();
            
            #endregion

            int type = config.IsSub ? config.SubType : config.Type;
            
            #region 2 型文件的双击操作

            if (FileOperate.IsPicture(type) || FileOperate.IsUnsupport(type) || FileOperate.IsError(type))
            {
                if (!PicWindowIsOK) { ShowPictureS(); NextShowBigPicture = false; return; }
                if (config.SourPicture.Height <= ClientHeight && config.SourPicture.Width <= ClientWidth)
                { ShowPictureS(); return; }
                
                NextShowBigPicture = !NextShowBigPicture;
                if (NextShowBigPicture) { ShowPictureB(); } else { ShowPictureS(); }
                return;
            }

            #endregion

            #region 3 型文件的双击操作

            if (FileOperate.IsGif(type))
            {
                ShowGif(null, null, false); return;
            }

            #endregion

            #region 4 型文件的双击操作

            if (FileOperate.IsStream(type))
            {
                string name = config.IsSub ? config.SubName : config.Name;
                if (!VidWindowIsOK) { ShowVideo(null, name, false); return; }

                WMPLib.WMPPlayState state = this.axWindowsMediaPlayer1.playState;
                if (state == WMPLib.WMPPlayState.wmppsPaused) { this.axWindowsMediaPlayer1.Ctlcontrols.play(); }
                if (state == WMPLib.WMPPlayState.wmppsStopped) { this.axWindowsMediaPlayer1.Ctlcontrols.play(); }
                if (state == WMPLib.WMPPlayState.wmppsReady) { this.axWindowsMediaPlayer1.Ctlcontrols.play(); }
                if (state == WMPLib.WMPPlayState.wmppsPlaying) { this.axWindowsMediaPlayer1.Ctlcontrols.pause(); }
                return;
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
            if (e.Button == MouseButtons.Left && !mouse.Down2)
            {
                mouse.Down = true;
                mouse.Up = false;
                mouse.pDown = MousePosition;
                mouse.tDown = TimeCount;

                mouse.pWindow = this.Location;
                mouse.xScroll = this.HorizontalScroll.Value;
                mouse.yScroll = this.VerticalScroll.Value;

                mouse.Resizing = Resizable;
                mouse.Size = this.Size;
            }

            if (e.Button == MouseButtons.Right && !mouse.Down)
            {
                mouse.Down2 = true;
                mouse.Up2 = false;
                mouse.pDown2 = MousePosition;
                mouse.tDown2 = TimeCount;
                mouse.pWindow = this.Location;

                if (this.pictureBox1.Visible)
                {
                    mouse.xImage = this.pictureBox1.PointToClient(MousePosition).X;
                    mouse.yImage = this.pictureBox1.PointToClient(MousePosition).Y;
                }
            }
        }
        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !mouse.Down2)
            {
                mouse.Down = false;
                mouse.Up = true;
                mouse.pUp = MousePosition;
                mouse.tUp = TimeCount;

                mouse.Resizing = false;
            }

            if (e.Button == MouseButtons.Right && !mouse.Down)
            {
                mouse.Down2 = false;
                mouse.Up2 = true;
                mouse.pUp2 = MousePosition;
                mouse.tUp2 = TimeCount;

                // 显示右键菜单
                bool showMenu = Math.Abs(mouse.pDown2.X - mouse.pUp2.X) < 10 &&
                    Math.Abs(mouse.pDown2.Y - mouse.pUp2.Y) < 10;
                this.contextMenuStrip1.Visible = showMenu;

                if (!showMenu && this.pictureBox1.Visible && !this.HorizontalScroll.Visible && !this.VerticalScroll.Visible)
                {
                    Point pt = this.pictureBox1.PointToClient(MousePosition);
                    bool inPic = pt.X >= 0 && pt.X < this.pictureBox1.Width &&
                        pt.Y >= 0 && pt.Y < this.pictureBox1.Height;
                    if (inPic) { return; }

                    int type = config.IsSub ? config.SubType : config.Type;
                    if (!FileOperate.IsPicture(type) && !FileOperate.IsGif(type)) { return; }
                    string path = config.IsSub ? config.Path + "\\" + config.Name : config.Path;
                    string name = config.IsSub ? config.SubName : config.Name;
                    Images.Add(new Form_Image(path, name, ShapeWindowRate, 2));
                    Images[Images.Count - 1].InitLoc = new Point(MousePosition.X - mouse.xImage, MousePosition.Y - mouse.yImage);
                    Images[Images.Count - 1].Show();
                }
            }
        }
        private void Form_MouseWheel(object sender, MouseEventArgs e)
        {
            int type = config.IsSub ? config.SubType : config.Type;
            bool xs = this.HorizontalScroll.Visible;
            bool ys = this.VerticalScroll.Visible;

            #region 滑动滚轮上下翻页

            if (this.lockToolStripMenuItem.Checked && !xs && !ys)
            {
                if (FileOperate.IsStream(type)) { return; }
                if (TimeCount - mouse.tWheel < 20) { return; }
                mouse.tWheel = TimeCount;
                
                if (e.Delta > 0) { RightMenu_Previous(null, null); }
                if (e.Delta < 0) { RightMenu_Next(null, null); }
                return;
            }

            #endregion

            #region 图片型文件滑动滚轮操作

            if (FileOperate.IsError(type) || FileOperate.IsUnsupport(type) || FileOperate.IsPicture(type))
            {
                if (config.SourPicture == null) { return; }
                if (xs || ys) { return; }

                int sh = Screen.PrimaryScreen.Bounds.Height;
                int sw = Screen.PrimaryScreen.Bounds.Width;
                int ch = ClientHeight;
                int cw = ClientWidth;

                // 当前缩放
                double rate1 = (double)this.pictureBox1.Height / sh * 100;
                double rate2 = (double)this.pictureBox1.Width / sw * 100;
                double currRate = Math.Max(rate1, rate2);

                // 下一次的显示比例
                if (e.Delta > 0) { ShapeWindowRate = currRate + 5; }
                if (e.Delta < 0) { ShapeWindowRate = currRate - 5; }
                
                // 不自动裁剪窗体时，显示图片的比例不能大于当前窗口（出现滚动条）
                if (!UseShapeWindow)
                {
                    rate1 = (double)ch / config.SourPicture.Height;
                    rate2 = (double)cw / config.SourPicture.Width;
                    double maxRate = Math.Min(rate1, rate2);

                    int maxh = (int)(config.SourPicture.Height * maxRate);
                    int maxw = (int)(config.SourPicture.Width * maxRate);

                    rate1 = (double)maxh / sh * 100;
                    rate2 = (double)maxw / sw * 100;
                    maxRate = Math.Max(rate1, rate2);
                    if (ShapeWindowRate > maxRate) { ShapeWindowRate = maxRate; }
                }

                // 显示图片
                if (ShapeWindowRate <= MinWindowSize) { ShapeWindowRate = MinWindowSize; }
                if (ShapeWindowRate >= MaxWindowSize) { ShapeWindowRate = MaxWindowSize; }
                ShowPictureR();
            }

            #endregion

            #region 当目前是 GIF 时，缩放窗体。

            if (FileOperate.IsGif(type))
            {
                //return; // 该功能会造成歧义，暂时取消。
                if (this.lockToolStripMenuItem.Checked) { return; }
                if (config.SourPicture == null) { return; }
                if (!UseShapeWindow) { return; }
                
                int sh = Screen.PrimaryScreen.Bounds.Height;
                int sw = Screen.PrimaryScreen.Bounds.Width;
                int ph = ClientHeight;
                int pw = ClientWidth;

                double rate1 = (double)ph / sh * 100;
                double rate2 = (double)pw / sw * 100;
                double currRate = Math.Max(rate1, rate2);

                // 下一次的显示比例
                if (e.Delta > 0) { ShapeWindowRate = currRate + 5; }
                if (e.Delta < 0) { ShapeWindowRate = currRate - 5; }
                if (ShapeWindowRate <= MinWindowSize) { ShapeWindowRate = MinWindowSize; }
                if (ShapeWindowRate >= MaxWindowSize) { ShapeWindowRate = MaxWindowSize; }
                
                // 显示 GIF
                ShowGif(null, null, false);
                return;
            }

            #endregion
            
            #region 播放音/视频

            if (FileOperate.IsStream(type))
            {
                if (xs || ys) { return; }

                string extension = config.IsSub ? config.SubExtension : config.Extension;
                bool isVideo = FileOperate.IsVideo(extension);
                bool isMusic = FileOperate.IsMusic(extension);

                if (isVideo)
                {
                    if (!UseShapeWindow) { return; }

                    int sh = Screen.PrimaryScreen.Bounds.Height;
                    int sw = Screen.PrimaryScreen.Bounds.Width;
                    int ph = ClientHeight;
                    int pw = ClientWidth;

                    double rate1 = (double)ph / sh * 100;
                    double rate2 = (double)pw / sw * 100;
                    double currRate = Math.Max(rate1, rate2);

                    // 下一次的显示比例
                    if (e.Delta > 0) { ShapeWindowRate = currRate + 5; }
                    if (e.Delta < 0) { ShapeWindowRate = currRate - 5; }
                    if (ShapeWindowRate <= MinWindowSize) { ShapeWindowRate = MinWindowSize; }
                    if (ShapeWindowRate >= MaxWindowSize) { ShapeWindowRate = MaxWindowSize; }
                    
                    ShowVideo(null, ".mp4", false);
                    return;
                }

                return;
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
                
                #region 整理 Image 窗口

                for (int i = Images.Count - 1; i >= 0; i--)
                {
                    if (Images[i].IsDisposed) { Images.RemoveAt(i); }
                }

                #endregion

                #region Search 窗口

                if (Search != null && !Search.IsDisposed && !Search.Cancle)
                {
                    play.IsPlaying = false;
                    Search.Cancle = true;

                    config.FolderIndex = Search.FolderIndex;
                    config.FileIndex = Search.FileIndex;
                    config.SubIndex = Search.SubIndex;
                    ShowCurrent();
                }

                #endregion

                #region Find 窗口

                if (Form_Find.IsSwitch)
                {
                    play.IsPlaying = false;
                    Form_Find.IsSwitch = false;
                    ShowCurrent();
                }

                #endregion

                #region 提示信息

                bool inX = MousePosition.X >= this.Location.X && MousePosition.X <= this.Location.X + this.Width;
                bool inY = MousePosition.Y >= this.Location.Y && MousePosition.Y <= this.Location.Y + this.Height;

                #region 提示

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
                        !Resizable &&
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
                        Resizable ||
                        TimeCount - mouse.tDown < 50 ||
                        TimeCount - mouse.tDown2 < 50 ||
                        TimeCount - mouse.tUp < 20 ||
                        TimeCount - mouse.tUp2 < 20)
                    {
                        if (sameText) { tip.Form.hide(); tip.Visible = false; }
                    }
                }

                #endregion

                #region 刷新

                if (MousePosition != tip.Previous || !IsActive)
                {
                    tip.Previous = MousePosition;
                    tip.Begin = TimeCount;
                    if (tip.Visible) { tip.Visible = false; tip.Form.hide(); }
                }

                #endregion

                #region 按键

                if (tip.Form.KeyValue != -1)
                {
                    if (tip.Form.KeyValue == Class.Load.settings.FastKey_Main_PageU ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_PageD ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_U ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_D ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_L ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_R ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_Board ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_Enter ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_Rotate ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_FlipX ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_FlipY)
                    {
                        KeyEventArgs eKey = new KeyEventArgs((Keys)tip.Form.KeyValue);
                        if (tip.Form.KeyState == 0) { Form_KeyDown(null, eKey); }
                        if (tip.Form.KeyState == 2) { Form_KeyUp(null, eKey); }
                        tip.Form.KeyValue = -1;
                    }

                    if (tip.Form.KeyValue == Class.Load.settings.FastKey_Main_Export ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_OpenComic ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_OpenCurrent ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_OpenExport ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_OpenRoot ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_Password ||
                        tip.Form.KeyValue == Class.Load.settings.FastKey_Main_Esc)
                    {
                        keybd_event((byte)tip.Form.KeyValue, 0, tip.Form.KeyState, 0);
                        tip.Form.KeyValue = -1;
                        tip.Visible = false; tip.Form.hide();
                    }
                }

                #endregion

                #endregion

                #region 播放

                #region 菜单栏刷新

                this.playToolStripMenuItem.Checked = play.IsPlaying = play.IsPlaying && play.PlayIndexes.Count != 0;
                this.startToolStripMenuItem.Text = play.IsPlaying ? "结束" : "开始";

                #endregion

                if (play.IsPlaying)
                {
                    #region 是否播放下一个文件

                    bool playNext = true; ResetPlayIndex();

                    if (playNext)
                    {
                        string name = (config.IsSub && config.SubFiles.Count != 0) ? config.SubFiles[config.SubIndex] : config.Name;
                        string extension = FileOperate.getExtension(name);
                        int type = FileOperate.getFileType(extension);
                        bool isMusic = FileOperate.IsMusic(extension);
                        bool isVideo = FileOperate.IsVideo(extension);
                        
                        if (type == 2) { play.Type = 2; playNext = true; }
                        if (type == 3) { play.Type = 3; playNext = true; }
                        if (type == 4 && isMusic) { play.Type = 4; playNext = true; }
                        if (type == 4 && isVideo) { play.Type = 5; playNext = true; }
                    }
                    if (playNext && (play.Type == 2 || play.Type == 3))
                    {
                        ulong showtime = play.ShowTime;
                        if (showtime < 4) { showtime = 4; }
                        playNext = TimeCount - play.Begin > showtime;
                    }
                    if (playNext && (play.Type == 4 || play.Type == 5))
                    {
                        playNext = this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped;
                    }
                    if (play.Index == -1)
                    {
                        playNext = true; play.Index = 0;
                    }

                    #endregion

                    #region 下一个

                    if (playNext)
                    {
                        if (play.Forward || play.Rand) { play.Index++; }
                        if (play.Backward) { play.Index--; }

                        playNext = play.IsPlaying = play.Circle|| play.Single || (play.Index >= 0 && play.Index < play.PlayIndexes.Count);
                    }
                    if (playNext)
                    {
                        if (play.Index < 0) { play.Index = play.PlayIndexes.Count - 1; }
                        if (play.Index >= play.PlayIndexes.Count) { play.Index = 0; }
                        int index = play.PlayIndexes[play.Index];

                        config.FolderIndex = play.FolderIndexes[index];
                        config.FileIndex = play.FileIndexes[index];
                        config.SubIndex = play.SubIndexes[index];
                        play.Begin = TimeCount;
                        ShowCurrent();
                    }

                    #endregion
                }

                #endregion

                #region 刷新隐藏翻页键菜单

                bool hideU = this.hideUToolStripMenuItem.Checked;
                bool hideD = this.hideDToolStripMenuItem.Checked;
                bool hideL = this.hideLToolStripMenuItem.Checked;
                bool hideR = this.hideRToolStripMenuItem.Checked;
                this.hideToolStripMenuItem.Checked = hideL || hideR || hideU || hideD;

                #endregion
                
                int scrollw = this.HorizontalScroll.Value;
                int scrollh = this.VerticalScroll.Value;

                #region 左翻页

                bool hidePageMark =
                    mouse.Down || 
                    mouse.Down2 || 
                    this.Width < 150 ||
                    this.Height < 150 ||
                    this.contextMenuStrip1.Visible ||
                    !IsActive;

                if (this.hideLToolStripMenuItem.Checked || hidePageMark) { this.label1.Visible = false; } else
                {
                    int h = ClientHeight / 5;
                    int w = ClientWidth / 20;
                    int f = w * 3 / 4;

                    int xbg = ClientWidth / 20;
                    int xed = xbg + w;
                    int ybg = ClientHeight / 2 - h / 2;
                    int yed = ybg + h;

                    int xm = this.PointToClient(MousePosition).X;
                    int ym = this.PointToClient(MousePosition).Y;

                    bool inRange = xbg <= xm && xm <= xed && ybg <= ym && ym <= yed;
                    if (!inRange) { this.label1.Visible = false; } else
                    {
                        this.label1.Height = h;
                        this.label1.Width = w;
                        this.label1.Font = new Font("宋体", f);
                        this.label1.Location = new Point(xbg, ybg);
                        this.label1.Visible = true;
                    }
                }

                #endregion

                #region 右翻页

                if (this.hideRToolStripMenuItem.Checked || hidePageMark) { this.label2.Visible = false; } else
                {
                    int h = ClientHeight / 5;
                    int w = ClientWidth / 20;
                    int f = w * 3 / 4;

                    int xbg = ClientWidth - ClientWidth / 20 - w; if (this.VerticalScroll.Visible) { xbg -= 15; }
                    int xed = xbg + w;
                    int ybg = ClientHeight / 2 - h / 2;
                    int yed = ybg + h;

                    int xm = this.PointToClient(MousePosition).X;
                    int ym = this.PointToClient(MousePosition).Y;

                    bool inRange = xbg <= xm && xm <= xed && ybg <= ym && ym <= yed;
                    if (!inRange) { this.label2.Visible = false; } else
                    {
                        this.label2.Height = h;
                        this.label2.Width = w;
                        this.label2.Font = new Font("宋体", f);
                        this.label2.Location = new Point(xbg, ybg);
                        this.label2.Visible = true;
                    }
                }

                #endregion

                #region 上翻页

                if (this.hideUToolStripMenuItem.Checked || hidePageMark) { this.label3.Visible = false; } else
                {
                    int h = ClientHeight / 12;
                    int w = ClientWidth / 8;
                    int f = h * 3 / 4;

                    int xbg = ClientWidth / 2 - w / 2;
                    int xed = xbg + w;
                    int ybg = ClientHeight / 20;
                    int yed = ybg + h;

                    int xm = this.PointToClient(MousePosition).X;
                    int ym = this.PointToClient(MousePosition).Y;

                    bool inRange = xbg <= xm && xm <= xed && ybg <= ym && ym <= yed;
                    if (!inRange) { this.label3.Visible = false; } else
                    {
                        this.label3.Height = h;
                        this.label3.Width = w;
                        this.label3.Font = new Font("宋体", f);
                        this.label3.Location = new Point(xbg, ybg);
                        this.label3.Visible = true;
                    }
                }

                #endregion

                #region 下翻页

                if (this.hideDToolStripMenuItem.Checked || hidePageMark) { this.label4.Visible = false; } else
                {
                    int h = ClientHeight / 12;
                    int w = ClientWidth / 8;
                    int f = h * 3 / 4;

                    int xbg = ClientWidth / 2 - w / 2;
                    int xed = xbg + w;
                    int ybg = ClientHeight - ClientHeight / 20 - h; if (this.HorizontalScroll.Visible) { ybg -= 15; }
                    int yed = ybg + h;

                    int xm = this.PointToClient(MousePosition).X;
                    int ym = this.PointToClient(MousePosition).Y;

                    bool inRange = xbg <= xm && xm <= xed && ybg <= ym && ym <= yed;
                    if (!inRange) { this.label4.Visible = false; } else
                    {
                        this.label4.Height = h;
                        this.label4.Width = w;
                        this.label4.Font = new Font("宋体", f);
                        this.label4.Location = new Point(xbg, ybg);
                        this.label4.Visible = true;
                    }
                }

                #endregion

                SetScrollW(scrollw); SetScrollH(scrollh);

                #region 鼠标距边框距离

                int disL = MousePosition.X - this.Location.X;
                int disR = this.Location.X + this.Width - MousePosition.X;
                int disU = MousePosition.Y - this.Location.Y;
                int disD = this.Location.Y + this.Height - MousePosition.Y;
                int dis0 = 3;

                Resizable =
                    (disL >= -dis0 && disL <= dis0) ||
                    ((disR >= -dis0 && disR <= dis0) && !this.VerticalScroll.Visible) ||
                    (disU >= -dis0 && disU <= dis0) ||
                    ((disD >= -dis0 && disD <= dis0) && !this.HorizontalScroll.Visible);
                Resizable =
                    Resizable &&
                    !UseBoard &&
                    !this.contextMenuStrip1.Visible &&
                    IsActive;

                #endregion

                #region 图标 - 可以调整窗体大小

                if (Resizable && !mouse.Down)
                {
                    bool resizeL = disL >= -dis0 && disL <= dis0;
                    bool resizeR = disR >= -dis0 && disR <= dis0 && !this.VerticalScroll.Visible;
                    bool resizeU = disU >= -dis0 && disU <= dis0;
                    bool resizeD = disD >= -dis0 && disD <= dis0 && !this.HorizontalScroll.Visible;
                    
                    if (resizeL && resizeR) { resizeL = disL < disR; resizeR = !resizeL; }
                    if (resizeU && resizeD) { resizeU = disU < disD; resizeD = !resizeU; }

                    if (resizeL && !resizeU && !resizeD) { this.Cursor = Cursors.SizeWE; }
                    if (resizeR && !resizeU && !resizeD) { this.Cursor = Cursors.SizeWE; }
                    if (resizeU && !resizeL && !resizeR) { this.Cursor = Cursors.SizeNS; }
                    if (resizeD && !resizeL && !resizeR) { this.Cursor = Cursors.SizeNS; }

                    if (resizeL && resizeU) { this.Cursor = Cursors.SizeNWSE; }
                    if (resizeL && resizeD) { this.Cursor = Cursors.SizeNESW; }
                    if (resizeR && resizeU) { this.Cursor = Cursors.SizeNESW; }
                    if (resizeR && resizeD) { this.Cursor = Cursors.SizeNWSE; }

                    mouse.ResizeL = resizeL;
                    mouse.ResizeR = resizeR;
                    mouse.ResizeU = resizeU;
                    mouse.ResizeD = resizeD;
                }

                if (!mouse.Down && !Resizable && !UseBoard)
                {
                    this.Cursor = Cursors.Default;
                }

                #endregion

                #region 更改窗体大小

                if (mouse.Resizing && mouse.Down)
                {
                    int xmove = MousePosition.X - mouse.pDown.X;
                    int ymove = MousePosition.Y - mouse.pDown.Y;

                    if (xmove != 0 || ymove != 0)
                    {
                        int resizeh = mouse.Size.Height;
                        int resizew = mouse.Size.Width;
                        int xloc = mouse.pWindow.X;
                        int yloc = mouse.pWindow.Y;
                        
                        if (mouse.ResizeL) { resizew -= xmove; xloc += xmove; }
                        if (mouse.ResizeR) { resizew += xmove; }
                        if (mouse.ResizeU) { resizeh -= ymove; yloc += ymove; }
                        if (mouse.ResizeD) { resizeh += ymove; }

                        this.Height = resizeh;
                        this.Width = resizew;
                        this.Location = new Point(xloc, yloc);
                    }
                }

                #endregion

                #region 拖拽窗体

                if (!mouse.Resizing && !this.HorizontalScroll.Visible && !this.VerticalScroll.Visible && mouse.Down)
                {
                    int xmove = MousePosition.X - mouse.pDown.X;
                    int ymove = MousePosition.Y - mouse.pDown.Y;
                    this.Location = new Point(mouse.pWindow.X + xmove, mouse.pWindow.Y + ymove);
                }

                #endregion

                #region 右键拖拽窗体

                if (!mouse.Resizing && mouse.Down2 && (this.HorizontalScroll.Visible || this.VerticalScroll.Visible))
                {
                    int xmove = MousePosition.X - mouse.pDown2.X;
                    int ymove = MousePosition.Y - mouse.pDown2.Y;
                    this.Location = new Point(mouse.pWindow.X + xmove, mouse.pWindow.Y + ymove);
                }

                #endregion

                #region 拖拽图片

                if (!mouse.Resizing && (this.HorizontalScroll.Visible || this.VerticalScroll.Visible) && mouse.Down)
                {
                    int xS = MousePosition.X - mouse.pDown.X;
                    int yS = MousePosition.Y - mouse.pDown.Y;

                    SetScrollW(mouse.xScroll - xS);
                    SetScrollH(mouse.yScroll - yS);
                }



                #endregion

                #region 刷新播放时间
                if (FileOperate.IsStream(config.IsSub ? config.SubType : config.Type))
                {
                    if (config.IsSub && FileOperate.IsZip(config.Type)) { } else
                    {
                        if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsReady)
                        { this.axWindowsMediaPlayer1.Ctlcontrols.play(); }

                        string index = "[" + (config.FileIndex + 1).ToString() + "/" + FileOperate.RootFiles[config.FolderIndex].Name.Count.ToString() + "]";
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
                        }
                    }
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
            config.IsSub = FileOperate.IsComic(config.Type);
            if (config.Type == 1) { config.ExistFile = Directory.Exists(config.Path + "\\" + config.Name); }
            else { config.ExistFile = File.Exists(config.Path + "\\" + config.Name); }
            if (!config.IsSub) { config.SubIndex = -1; }

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
                ZipOperate.ReadZipEX(config.Path + "\\" + config.Name);
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

            config.SourPicture = null;
            config.DestPicture = null;
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

            #endregion

            #region 提取显示文本，处理错误信息，若文件不存在，标题给出提示信息后直接返回

            string index = config.ExistFolder ?
                "[" + (config.FileIndex + 1).ToString() + "/" + FileOperate.RootFiles[config.FolderIndex].Name.Count.ToString() + "]" :
                "";
            string subindex = config.ExistFile && config.IsSub ?
                "[" + (config.SubIndex + 1).ToString() + "/" + config.SubFiles.Count.ToString() + "]" :
                "";
            
            if (FileOperate.RootFiles.Count == 0) { this.Text = "[不存在任何文件] 右键窗体，导入文件夹来开始！"; ShowNot(); return; }
            if (!config.ExistFolder) { this.Text = "[文件夹不存在] " + config.Path; ShowErr(); return; }
            if (FileOperate.RootFiles[config.FolderIndex].Name.Count == 0) { this.Text = "[0/0] [空文件夹]当前文件夹不存在任何文件！"; ShowNot(); return; }
            if (!config.ExistFile) { this.Text = index + " [文件/文件夹不存在] " + config.Name; ShowErr(); return; }
            if (FileOperate.IsFolder(config.Type) && config.SubFiles.Count == 0) { this.Text = index + " " + subindex + " [空文件夹] " + config.Name; ShowNot(); return; }
            if (FileOperate.IsZip(config.Type) && config.SubFiles.Count == 0) { this.Text = index + " " + subindex + " [空文件] " + config.Name; ShowNot(); return; }

            #endregion

            #region 隐藏文件不予显示

            if (!FileSupport.SupportHide && (!config.IsSub ? config.Hide : config.SubHide))
            {
                this.Text = config.IsSub ?
                    index + " " + subindex + " [未知文件] " + config.Name + " : " + config.SubName :
                    index + " [未知文件] " + config.Name;

                config.Type = -1; config.SubType = -1; ShowUnk(); return;
            }

            #endregion
            
            #region 0 型文件（暂不支持类型）

            if (FileOperate.IsUnsupport(config.Type))
            {
                this.Text = config.IsSub ?
                    index + " " + subindex + " [不支持] " + config.Name + " : " + config.SubName :
                    index + " [不支持] " + config.Name;
                ShowUnp(); return;
            }

            #endregion

            #region 1 型文件（文件夹）

            if (FileOperate.IsFolder(config.Type))
            {
                this.Text = config.SubType == 4 ?
                    index + " " + subindex + " " + config.Name + " : " + config.SubName :
                    index + " " + subindex + " " + config.Name + " : " + config.SubName;
                
                if (FileOperate.IsPicture(config.SubType)) { ShowPicture(config.Path + "\\" + config.Name, config.SubName); return; }
                if (FileOperate.IsGif(config.SubType)) { ShowGif(config.Path + "\\" + config.Name, config.SubName); return; }
                if (FileOperate.IsStream(config.SubType)) { ShowVideo(config.Path + "\\" + config.Name, config.SubName); return; }

                this.Text = index + " " + subindex + " [不支持] " + config.Name + " : " + config.SubName;
                config.SubType = -1;
                ShowUnp();
                return;
            }

            #endregion

            #region 2 型文件（图片）

            if (FileOperate.IsPicture(config.Type))
            {
                this.Text = index + " " + config.Name;
                ShowPicture(config.Path, config.Name); return;
            }

            #endregion

            #region 3 型文件（GIF）

            if (FileOperate.IsGif(config.Type))
            {
                this.Text = index + " " + config.Name;
                ShowGif(config.Path, config.Name); return;
            }

            #endregion

            #region 4 型文件（视频）

            if (FileOperate.IsStream(config.Type))
            {
                this.Text = index + " " + config.Name;
                ShowVideo(config.Path, config.Name); return;
            }

            #endregion

            #region 5 型文件（ZIP）

            if (FileOperate.IsZip(config.Type))
            {
                if (!ZipOperate.Known)
                {
                    this.Text = index + " " + subindex + " [密码错误] " + config.Name + " : " + config.SubName;
                    ShowErr(); return;
                }

                this.Text = index + " " + subindex + " " + config.Name + " : " + config.SubName;
                
                if (FileOperate.IsPicture(config.SubType) && ZipOperate.LoadPictureEX()) { ShowPicture(null, null, false); return; }
                if (FileOperate.IsGif(config.SubType) && ZipOperate.LoadGifEX()) { ShowGif(null, null, false); return; }

                this.Text = index + " " + subindex + " [不支持] " + config.Name + " : " + config.SubName;
                config.SubType = -1;
                ShowUnp();
                return;
            }

            #endregion

            #region 其他文件（不支持）

            this.Text = index + " [不支持] " + config.Name;
            ShowUnp();
            config.Type = -1;

            #endregion
        }
        private void ShowPicture(string path, string name, bool load = true)
        {
            #region 确保占用资源被释放
            
            if (load) { try { config.SourPicture.Dispose(); } catch { } }
            try { config.DestPicture.Dispose(); } catch { }
            if (load) { config.SourPicture = null; }
            config.DestPicture = null;
            if (load) { this.pictureBox1.Image = null; }

            if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            { this.axWindowsMediaPlayer1.Ctlcontrols.stop(); }

            #endregion

            #region 重新加载资源

            if (load) { config.SourPicture = Image.FromFile(path + "\\" + name); }
            if (config.SourPicture == null) { return; }
            this.axWindowsMediaPlayer1.Visible = false;
            this.pictureBox1.Visible = true;

            #endregion

            #region 加载并显示图片
            
            if (NextShowBigPicture) { ShowPictureB(); }
            else { ShowPictureS(); }

            #endregion

            #region 确保窗体打开

            this.axWindowsMediaPlayer1.Visible = false;
            this.pictureBox1.Visible = true;

            #endregion
        }
        private void ShowGif(string path, string name, bool load = true)
        {
            #region 确保资源被释放

            if (load) { try { config.SourPicture.Dispose(); } catch { } }
            try { config.DestPicture.Dispose(); } catch { }
            if (load) { config.SourPicture = null; }
            config.DestPicture = null;
            if (load) { this.pictureBox1.Image = null; }
            
            if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            { this.axWindowsMediaPlayer1.Ctlcontrols.stop(); }

            #endregion

            #region 重新加载资源

            if (load) { config.SourPicture = (Bitmap)Image.FromFile(path + "\\" + name); }
            if (config.SourPicture == null) { return; }
            this.axWindowsMediaPlayer1.Visible = false;
            this.pictureBox1.Visible = true;

            #endregion

            #region 获取窗体大小

            if (UseShapeWindow)
            {
                int sh = (int)(Screen.PrimaryScreen.Bounds.Height * ShapeWindowRate / 100);
                int sw = (int)(Screen.PrimaryScreen.Bounds.Width * ShapeWindowRate / 100);

                double rate1 = (double)sh / config.SourPicture.Height;
                double rate2 = (double)sw / config.SourPicture.Width;
                double rate = Math.Min(rate1, rate2);
                if (rate > 1) { rate = 1; }

                int shapeh = (int)(config.SourPicture.Height * rate);
                int shapew = (int)(config.SourPicture.Width * rate);

                // 当屏幕能够容纳 GIF 源图，则不做任何裁剪。
                if (sh >= config.SourPicture.Height && sw >= config.SourPicture.Width)
                {
                    shapeh = config.SourPicture.Height;
                    shapew = config.SourPicture.Width;
                }

                int centerh = this.Location.Y + this.Height / 2;
                int centerw = this.Location.X + this.Width / 2;

                SetScroll0();
                ClientHeight = shapeh;
                ClientWidth = shapew;
                this.Location = new Point(centerw - this.Width / 2, centerh - this.Height / 2);
            }

            #endregion

            #region 获取控件大小，加载源图

            if (true)
            {
                int ch = ClientHeight;
                int cw = ClientWidth;
                int sh = config.SourPicture.Height;
                int sw = config.SourPicture.Width;

                int px = (cw - sw) / 2; if (px < 0) { px = 0; }
                int py = (ch - sh) / 2; if (py < 0) { py = 0; }

                SetScroll0();
                this.pictureBox1.Location = new Point(px, py);
                this.pictureBox1.Height = config.SourPicture.Height;
                this.pictureBox1.Width = config.SourPicture.Width;
                SetScroll0();
                if (load) { this.pictureBox1.Image = config.SourPicture; }
            }

            #endregion

            #region 确保窗体打开

            this.axWindowsMediaPlayer1.Visible = false;
            this.pictureBox1.Visible = true;

            #endregion
        }
        private void ShowVideo(string path, string name, bool load = true)
        {
            #region 确保占用资源被释放

            try { config.SourPicture.Dispose(); } catch { }
            try { config.DestPicture.Dispose(); } catch { }
            config.SourPicture = null;
            config.DestPicture = null;
            this.pictureBox1.Image = null;

            if (load && this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            { this.axWindowsMediaPlayer1.Ctlcontrols.stop(); }

            #endregion

            #region 重新加载资源
            
            this.axWindowsMediaPlayer1.Visible = true;
            this.pictureBox1.Visible = false;
            
            if (load && axWindowsMediaPlayer1.URL != path + "\\" + name) { axWindowsMediaPlayer1.URL = path + "\\" + name; }
            if (load) { axWindowsMediaPlayer1.Ctlcontrols.play(); }
            
            #endregion

            #region 获取窗体大小

            bool isMusic = FileOperate.IsMusic(FileOperate.getExtension(name));
            bool isVideo = FileOperate.IsVideo(FileOperate.getExtension(name));

            if (isMusic && UseShapeWindow)
            {
                int sh = Screen.PrimaryScreen.Bounds.Height * MinWindowSize / 100;
                int sw = Screen.PrimaryScreen.Bounds.Width * MinWindowSize / 100;

                int shapeh = Math.Min(sh, sw);
                int shapew = shapeh;

                int centerh = this.Location.Y + this.Height / 2;
                int centerw = this.Location.X + this.Width / 2;

                SetScroll0();
                ClientHeight = shapeh;
                ClientWidth = shapew;
                this.Location = new Point(centerw - this.Width / 2, centerh - this.Height / 2);
            }
            if (isVideo && UseShapeWindow)
            {
                int sh = (int)(Screen.PrimaryScreen.Bounds.Height * ShapeWindowRate / 100);
                int sw = (int)(Screen.PrimaryScreen.Bounds.Width * ShapeWindowRate / 100);

                int shapeh = sh;
                int shapew = sw;

                int centerh = this.Location.Y + this.Height / 2;
                int centerw = this.Location.X + this.Width / 2;

                SetScroll0();
                ClientHeight = shapeh;
                ClientWidth = shapew;
                this.Location = new Point(centerw - this.Width / 2, centerh - this.Height / 2);
            }

            #endregion

            #region 获取控件大小

            if (isMusic)
            {
                int ch = ClientHeight;
                int cw = ClientWidth;

                int sh = Screen.PrimaryScreen.Bounds.Height * MinWindowSize / 100;
                int sw = Screen.PrimaryScreen.Bounds.Width * MinWindowSize / 100;
                int shapeh = Math.Min(sh, sw) - 2;
                int shapew = shapeh;

                SetScroll0();
                this.axWindowsMediaPlayer1.Height = shapeh;
                this.axWindowsMediaPlayer1.Width = shapew;
                this.axWindowsMediaPlayer1.Location = new Point((cw - shapew) / 2, (ch - shapeh) / 2);
            }
            if (isVideo)
            {
                int ch = ClientHeight;
                int cw = ClientWidth;

                int shapeh = ch - 2;
                int shapew = cw - 2;

                SetScroll0();
                this.axWindowsMediaPlayer1.Height = shapeh;
                this.axWindowsMediaPlayer1.Width = shapew;
                this.axWindowsMediaPlayer1.Location = new Point(1, 1);
            }

            #endregion

            #region 确保窗体打开

            this.axWindowsMediaPlayer1.Visible = true;
            this.pictureBox1.Visible = false;

            #endregion
        }
        private void ShowOff()
        {
            #region 确保占用资源被释放

            try { config.SourPicture.Dispose(); } catch { }
            try { config.DestPicture.Dispose(); } catch { }
            config.SourPicture = null;
            config.DestPicture = null;
            this.pictureBox1.Image = null;

            if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            { this.axWindowsMediaPlayer1.Ctlcontrols.stop(); }

            #endregion

            #region 确保窗体关闭

            this.axWindowsMediaPlayer1.Visible = false;
            this.pictureBox1.Visible = false;

            #endregion
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
        private void ShowPictureB()
        {
            if (config.SourPicture == null) { return; }
            if (ClientHeight >= config.SourPicture.Height && ClientWidth >= config.SourPicture.Width)
            { ShowPictureS(); return; }

            #region 获取聚焦点

            int xfocus = this.pictureBox1.PointToClient(MousePosition).X;
            int yfocus = this.pictureBox1.PointToClient(MousePosition).Y;
            if (config.DestPicture == null) { xfocus = 0; yfocus = 0; }

            double xrate = (double)xfocus / this.pictureBox1.Width;
            double yrate = (double)yfocus / this.pictureBox1.Height;

            if (xrate < 0) { xrate = 0; }
            if (xrate > 1) { xrate = 1; }
            if (yrate < 0) { yrate = 0; }
            if (yrate > 1) { yrate = 1; }

            #endregion

            #region 显示源图片

            SetScroll0();

            int xpic = (ClientWidth - config.SourPicture.Width) / 2;
            int ypic = (ClientHeight - config.SourPicture.Height) / 2;
            if (xpic < 0) { xpic = 0; }
            if (ypic < 0) { ypic = 0; }

            this.pictureBox1.Location = new Point(xpic, ypic);
            this.pictureBox1.Height = config.SourPicture.Height;
            this.pictureBox1.Width = config.SourPicture.Width;
            this.pictureBox1.Image = config.SourPicture;
            
            #endregion

            #region 获取移动量

            int xscroll = (int)(config.SourPicture.Width * xrate) - ClientWidth / 2;
            int yscroll = (int)(config.SourPicture.Height * yrate) - ClientHeight / 2;

            SetScrollW(xscroll);
            SetScrollH(yscroll);

            #endregion
        }
        private void ShowPictureS()
        {
            if (config.SourPicture == null) { return; }

            #region 获取窗体大小

            if (UseShapeWindow)
            {
                int sh = (int)(Screen.PrimaryScreen.Bounds.Height * ShapeWindowRate / 100);
                int sw = (int)(Screen.PrimaryScreen.Bounds.Width * ShapeWindowRate / 100);

                double rate1 = (double)sh / config.SourPicture.Height;
                double rate2 = (double)sw / config.SourPicture.Width;
                double rate = Math.Min(rate1, rate2);
                if (rate > 1) { rate = 1; }

                int shapeh = (int)(config.SourPicture.Height * rate);
                int shapew = (int)(config.SourPicture.Width * rate);
                
                int centerh = this.Location.Y + this.Height / 2;
                int centerw = this.Location.X + this.Width / 2;

                SetScroll0();
                ClientHeight = shapeh;
                ClientWidth = shapew;
                this.Location = new Point(centerw - this.Width / 2, centerh - this.Height / 2);
            }

            #endregion

            #region 获取控件大小，并填充目标图片

            if (true)
            {
                int ch = ClientHeight;
                int cw = ClientWidth;
                double rate1 = (double)ch / config.SourPicture.Height;
                double rate2 = (double)cw / config.SourPicture.Width;
                double rate = Math.Min(rate1, rate2);
                if (rate > 1) { rate = 1; }

                int shapeh = (int)(config.SourPicture.Height * rate);
                int shapew = (int)(config.SourPicture.Width * rate);

                // 无缝
                if (Math.Abs(shapeh - ch) < 3) { shapeh = ch; }
                if (Math.Abs(shapew - cw) < 3) { shapew = cw; }

                // 绘图
                try { config.DestPicture.Dispose(); } catch { }
                config.DestPicture = (Image)new Bitmap(shapew, shapeh);
                Graphics g = Graphics.FromImage(config.DestPicture);
                g.DrawImage(
                    config.SourPicture,
                    new Rectangle(0, 0, shapew, shapeh),
                    new Rectangle(0, 0, config.SourPicture.Width, config.SourPicture.Height),
                    GraphicsUnit.Pixel);
                g.Dispose();

                // 填充
                SetScroll0();
                //this.pictureBox1.Visible = false;
                this.pictureBox1.Location = new Point((cw - shapew) / 2, (ch - shapeh) / 2);
                this.pictureBox1.Height = shapeh;
                this.pictureBox1.Width = shapew;
                this.pictureBox1.Image = config.DestPicture;
                //this.pictureBox1.Visible = true;
            }

            #endregion
        }
        private void ShowPictureR()
        {
            if (config.SourPicture == null) { return; }

            #region 获取窗体大小

            if (UseShapeWindow)
            {
                int sh = (int)(Screen.PrimaryScreen.Bounds.Height * ShapeWindowRate / 100);
                int sw = (int)(Screen.PrimaryScreen.Bounds.Width * ShapeWindowRate / 100);

                double rate1 = (double)sh / config.SourPicture.Height;
                double rate2 = (double)sw / config.SourPicture.Width;
                double rate = Math.Min(rate1, rate2);

                int shapeh = (int)(config.SourPicture.Height * rate);
                int shapew = (int)(config.SourPicture.Width * rate);

                int centerh = this.Location.Y + this.Height / 2;
                int centerw = this.Location.X + this.Width / 2;

                SetScroll0();
                ClientHeight = shapeh;
                ClientWidth = shapew;
                this.Location = new Point(centerw - this.Width / 2, centerh - this.Height / 2);
            }

            #endregion

            #region 获取控件大小，并填充目标图片

            if (true)
            {
                int sh = (int)(Screen.PrimaryScreen.Bounds.Height * ShapeWindowRate / 100);
                int sw = (int)(Screen.PrimaryScreen.Bounds.Width * ShapeWindowRate / 100);
                int ch = ClientHeight;
                int cw = ClientWidth;

                double rate1 = (double)sh / config.SourPicture.Height;
                double rate2 = (double)sw / config.SourPicture.Width;
                double rate = Math.Min(rate1, rate2);

                int shapeh = (int)(config.SourPicture.Height * rate);
                int shapew = (int)(config.SourPicture.Width * rate);

                // 无缝
                if (Math.Abs(shapeh - ch) < 3) { shapeh = ch; }
                if (Math.Abs(shapew - cw) < 3) { shapew = cw; }

                // 绘图
                try { config.DestPicture.Dispose(); } catch { }
                config.DestPicture = (Image)new Bitmap(shapew, shapeh);
                Graphics g = Graphics.FromImage(config.DestPicture);
                g.DrawImage(
                    config.SourPicture,
                    new Rectangle(0, 0, shapew, shapeh),
                    new Rectangle(0, 0, config.SourPicture.Width, config.SourPicture.Height),
                    GraphicsUnit.Pixel);
                g.Dispose();

                // 填充
                SetScroll0();
                this.pictureBox1.Location = new Point((cw - shapew) / 2, (ch - shapeh) / 2);
                this.pictureBox1.Height = shapeh;
                this.pictureBox1.Width = shapew;
                this.pictureBox1.Image = config.DestPicture;
            }

            #endregion
        }
        private void ShowBoard(bool show)
        {
            if (UseBoard == show) { return; }

            int top = UseBoard ? this.Location.Y + ClientTop : this.Location.Y;
            int lef = UseBoard ? this.Location.X + ClientLeft : this.Location.X;

            int centerh = top + ClientHeight / 2;
            int centerw = lef + ClientWidth / 2;
            int clienth = ClientHeight;
            int clientw = ClientWidth;

            UseBoard = show;
            if (show) { this.FormBorderStyle = FormBorderStyle.Sizable; }
            else { this.FormBorderStyle = FormBorderStyle.None; }
            ClientHeight = clienth;
            ClientWidth = clientw;

            int pty = centerh - ClientHeight / 2 - (UseBoard ? ClientTop : 0);
            int ptx = centerw - ClientWidth / 2 - (UseBoard ? ClientLeft : 0);

            this.Location = new Point(ptx, pty);

            #region 判断位置

            bool PicWindowIsOK = !NeedCorrectPictureBox();
            bool VidWindowIsOK = !NeedCorrectWMP();
            int type = config.IsSub ? config.SubType : config.Type;
            string name = config.IsSub ? config.SubName : config.Name;
            if (FileOperate.IsError(type) && !PicWindowIsOK) { ShowPictureS(); }
            if (FileOperate.IsUnsupport(type) && !PicWindowIsOK) { ShowPictureS(); }
            if (FileOperate.IsPicture(type) && !PicWindowIsOK) { ShowPictureS(); }
            if (FileOperate.IsGif(type) && !PicWindowIsOK) { ShowGif(null, null, false); }
            if (FileOperate.IsStream(type) && !VidWindowIsOK) { ShowVideo(null, name, false); }

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
            if (!this.VerticalScroll.Visible) { return; }

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
            if (!this.HorizontalScroll.Visible) { return; }

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

        private bool NeedCorrectPictureBox()
        {
            if (!this.pictureBox1.Visible) { return false; }
            if (!NextShowBigPicture && (this.HorizontalScroll.Visible || this.VerticalScroll.Visible)) { return true; }

            int ph = this.pictureBox1.Height;
            int pw = this.pictureBox1.Width;
            int ch = ClientHeight;
            int cw = ClientWidth;

            int stdx = (cw - pw) / 2;
            int stdy = (ch - ph) / 2;
            if (stdx < 0) { stdx = 0; }
            if (stdy < 0) { stdy = 0; }

            int nowx = this.pictureBox1.Location.X;
            int nowy = this.pictureBox1.Location.Y;
            if (nowx < 0) { nowx = 0; }
            if (nowy < 0) { nowy = 0; }

            return stdx != nowx || stdy != nowy;
        }
        private bool NeedCorrectWMP()
        {
            if (!this.axWindowsMediaPlayer1.Visible) { return false; }
            if (this.HorizontalScroll.Visible || this.VerticalScroll.Visible) { return true; }

            string name = config.IsSub ? config.SubName : config.Name;
            bool isMusic = FileOperate.IsMusic(FileOperate.getExtension(name));
            bool isVideo = FileOperate.IsVideo(FileOperate.getExtension(name));

            if (isMusic)
            {
                int ph = this.axWindowsMediaPlayer1.Height;
                int pw = this.axWindowsMediaPlayer1.Width;
                int ch = ClientHeight;
                int cw = ClientWidth;

                int stdx = (cw - pw) / 2;
                int stdy = (ch - ph) / 2;
                int nowx = this.axWindowsMediaPlayer1.Location.X;
                int nowy = this.axWindowsMediaPlayer1.Location.Y;

                return UseBoard ? (stdx != nowx || stdy != nowy) : (ph + 2 != ch || pw + 2 != cw);
            }
            if (isVideo)
            {
                int ph = this.axWindowsMediaPlayer1.Height;
                int pw = this.axWindowsMediaPlayer1.Width;
                int ch = ClientHeight;
                int cw = ClientWidth;

                int nowx = this.axWindowsMediaPlayer1.Location.X;
                int nowy = this.axWindowsMediaPlayer1.Location.Y;

                return ch != ph + 2 || cw != pw + 2 || nowx != 1 || nowy != 1;
            }

            return false;
        }
        
        private void SearchPlayFile()
        {
            play.FolderIndexes = new List<int>();
            play.FileIndexes = new List<int>();
            play.SubIndexes = new List<int>();
            play.PlayIndexes = new List<int>();

            if (play.Single)
            {
                play.PlayIndexes.Add(0);
                play.FolderIndexes.Add(config.FolderIndex);
                play.FileIndexes.Add(config.FileIndex);
                play.SubIndexes.Add(config.SubIndex);
                return;
            }

            bool existFile = play.Picture || play.Gif || play.Music || play.Video;
            if (!existFile) { play.Index = -1; return; }
            
            int folderbg = play.TotalRoots ? 0 : config.FolderIndex;
            int foldered = play.TotalRoots ? FileOperate.RootFiles.Count : folderbg + 1;
            int filebg = play.Subroot ? config.FileIndex : 0;
            int fileed = play.Subroot ? filebg + 1 : (config.ExistFolder ? FileOperate.RootFiles[config.FolderIndex].Name.Count : 0);

            for (int i = folderbg; i < foldered; i++)
            {
                #region 文件夹

                if (!play.Subroot) { filebg = 0; fileed = FileOperate.RootFiles[i].Name.Count; }

                for (int j = filebg; j < fileed; j++)
                {
                    string path = FileOperate.RootFiles[i].Path;
                    string name = FileOperate.RootFiles[i].Name[j];
                    string extension = FileOperate.getExtension(name);
                    int type = FileOperate.getFileType(extension);
                    bool isMusic = FileOperate.IsMusic(extension);
                    bool isVideo = FileOperate.IsVideo(extension);

                    bool found =
                        (play.Picture && type == 2) ||
                        (play.Gif && type == 3) ||
                        (play.Music && isMusic) ||
                        (play.Video && isVideo);
                    if (found) { play.FolderIndexes.Add(i); play.FileIndexes.Add(j); play.SubIndexes.Add(-1); play.PlayIndexes.Add(play.FolderIndexes.Count - 1); continue; }
                    if (!FileOperate.IsComic(type)) { continue; }

                    List<string> subfiles = null;
                    if (type == 1) { subfiles = FileOperate.getSubFiles(path + "\\" + name); }
                    if (type == 5) { subfiles = ZipOperate.getZipFileEX(path + "\\" + name); }
                    if (subfiles == null || subfiles.Count == 0) { continue; }

                    #region 子文件夹

                    for (int k = 0; k < subfiles.Count; k++)
                    {
                        name = subfiles[k];
                        extension = FileOperate.getExtension(name);
                        type = FileOperate.getFileType(extension);
                        isMusic = FileOperate.IsMusic(extension);
                        isVideo = FileOperate.IsVideo(extension);

                        found =
                            (play.Picture && type == 2) ||
                            (play.Gif && type == 3) ||
                            (play.Music && isMusic) ||
                            (play.Video && isVideo);

                        if (found) { play.FolderIndexes.Add(i); play.FileIndexes.Add(j); play.SubIndexes.Add(k); play.PlayIndexes.Add(play.FolderIndexes.Count - 1); }
                    }

                    if (play.Subroot) { break; }

                    #endregion
                }

                if (play.Root) { break; }

                #endregion
            }

            if (play.Rand) { play.PlayIndexes = play.PlayIndexes.OrderBy(x => Guid.NewGuid()).ToList(); }
        }
        private void ResetPlayIndex()
        {
            int folderindex = config.FolderIndex, fileindex = config.FileIndex, subindex = config.SubIndex;
            play.Index = -1;

            for (int i = play.PlayIndexes.Count - 1; i >= 0; i--)
            {
                if (play.FolderIndexes[play.PlayIndexes[i]] != folderindex || play.FileIndexes[play.PlayIndexes[i]] != fileindex) { continue; }
                play.Index = i;
                if (subindex == -1 || play.SubIndexes[play.PlayIndexes[i]] == subindex) { break; }
            }
        }

        private void CloseImages()
        {
            for (int i = Images.Count - 1; i >= 0; i--)
            {
                if (!Images[i].IsDisposed) { Images[i].Close(); }
            }
        }
        private void CloseImages(string fullname)
        {
            for (int i = Images.Count - 1; i >= 0; i--)
            {
                if (!Images[i].IsDisposed && Images[i].Path + "\\" + Images[i].Name == fullname) { Images[i].Close(); }
            }
        }
        private void CloseImages(string path, string name)
        {
            for (int i = Images.Count - 1; i >= 0; i--)
            {
                if (!Images[i].IsDisposed && Images[i].Path == path && Images[i].Name == name)
                {
                    Images[i].Close();
                }
            }
        }
        private void ShowImages()
        {
            for (int i = 0; i < Images.Count; i++)
            {
                if (!Images[i].IsDisposed) { Images[i].Show(); }
            }
        }
        private void HideImages()
        {
            for (int i = 0; i < Images.Count; i++)
            {
                if (!Images[i].IsDisposed) { Images[i].Hide(); }
            }
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
            // 释放资源
            if (config.SourPicture != null) { try { config.SourPicture.Dispose(); } catch { } }
            if (config.DestPicture != null) { try { config.DestPicture.Dispose(); } catch { } }
            if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            { try { this.axWindowsMediaPlayer1.Ctlcontrols.stop(); } catch { } }

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
            FileOperate.Reload(config.FolderIndex);

            // 显示
            ShowCurrent();
        }
        private void ShowCurrentFolder()
        {
            // 释放资源
            if (config.SourPicture != null) { try { config.SourPicture.Dispose(); } catch { } }
            if (config.DestPicture != null) { try { config.DestPicture.Dispose(); } catch { } }
            if (this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            { try { this.axWindowsMediaPlayer1.Ctlcontrols.stop(); } catch { } }

            // 更改文件后缀
            if (config.FolderIndex < 0) { return; }
            if (config.FolderIndex >= FileOperate.RootFiles.Count) { return; }
            FileOperate.ShowFiles(FileOperate.RootFiles[config.FolderIndex].Path);

            // 重新加载文件
            FileOperate.Reload(config.FolderIndex);

            // 显示
            ShowCurrent();
        }
        private void ReplaceDisk(string input)
        {
            if (input.Length < 9) { return; }
            input = input.ToUpper();

            char sdisk = input[6];
            char ddisk = input[8];
            char _disk = ' ';
            if (sdisk == ddisk) { return; }

            if (Class.Load.Initialize.ThreadState == System.Threading.ThreadState.Running)
            { MessageBox.Show("尚未读取完成！", "提示"); return; }

            for (int i = 0; i < Form_Find.PictureFiles.Count; i++)
            {
                Form_Find.PICTURE p = Form_Find.PictureFiles[i];
                if (p.Path == null || p.Path.Length == 0) { continue; }
                _disk = p.Path.ToUpper()[0];
                if (_disk != sdisk) { continue; }

                p.Path = ddisk + p.Path.Substring(1);
                p.Full = ddisk + p.Full.Substring(1);

                Form_Find.PictureFiles[i] = p;
            }
        }

        private void RightMenu_Refresh(object sender, EventArgs e)
        {
            this.IsActive = true;
            FileOperate.Reload();

            int type = config.IsSub ? config.SubType : config.Type;
            if (FileOperate.IsStream(type)) { ShowVideo(null, null, false); return; }
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
            if (expath == null || expath.Length == 0) { expath = FileOperate.getExePath(); }
            if (File.Exists(expath + "\\" + name)) { MessageBox.Show("本文件已经存在于输出文件夹中：" + expath, "提示"); return; }

            // zip 文件不能导出内部文件
            if (config.Type == 5)
            {
                if (DialogResult.Cancel == MessageBox.Show("只能导出整个 ZIP 文件，是否导出？", "确认导出", MessageBoxButtons.OKCancel))
                { return; }
            }

            // 确认是否输出
            if (config.Type != 5 && DialogResult.Cancel == MessageBox.Show("把 “" + name + "” 导出？", "确认导出", MessageBoxButtons.OKCancel))
            { return; }

            CloseImages(path, name);

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

            string exportpath = config.ExportFolder;
            if (exportpath == null || exportpath.Length == 0) { exportpath = FileOperate.getExePath(); }
            exportpath = exportpath + "\\" + config.Name;
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
                CloseImages(config.Path + "\\" + config.Name + "\\" + config.SubFiles[i]);
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

                CloseImages(sour);

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
            if (Search == null || Search.IsDisposed) { Search = new Form_Search(); }
            Search.Show();
            Search.Focus();
            return;

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
        private void RightMenu_New(object sender, EventArgs e)
        {
            int type = config.IsSub ? config.SubType : config.Type;
            if (!FileOperate.IsPicture(type) && !FileOperate.IsGif(type)) { MessageBox.Show("新窗口不能打开 图片/GIF 以外的文件！", "提示"); return; }
            string path = config.IsSub ? config.Path + "\\" + config.Name : config.Path;
            string name = config.IsSub ? config.SubName : config.Name;
            Images.Add(new Form_Image(path, name, ShapeWindowRate));
            Images[Images.Count - 1].Show();
        }
        private void RightMenu_Find(object sender, EventArgs e)
        {
            //this.contextMenuStrip1.Hide();
            
            // 计算模式
            ushort mode = 0;
            if (Class.Load.settings.Form_Main_Find_Full) { mode += (ushort)Form_Find.MODE.FULL; }
            if (Class.Load.settings.Form_Main_Find_Part) { mode += (ushort)Form_Find.MODE.PART; }
            if (Class.Load.settings.Form_Main_Find_Same) { mode += (ushort)Form_Find.MODE.SAME; }
            if (Class.Load.settings.Form_Main_Find_Like) { mode += (ushort)Form_Find.MODE.LIKE; }
            if (Class.Load.settings.Form_Main_Find_Turn) { mode += (ushort)Form_Find.MODE.TURN; }

            // 开始查询
            if (Find == null || Find.IsDisposed) { Find = new Form_Find((Form_Find.MODE)mode); }
            Find.Show();
            Find.Focus();
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
            //bool outSub = config.IsSub && !emptySub && DialogResult.OK == MessageBox.Show("已经是该文件夹的第一个文件了，是否跳出当前文件夹？\n\n" + config.Name, "请确认", MessageBoxButtons.OKCancel);
            bool outSub = true;
            //WheelPageTime = TimeCount;
            if (config.IsSub && !outSub && !emptySub) { return; }

            nextFile--; nextSub = int.MaxValue;
            if (nextFile < 0) { nextFile = int.MaxValue; nextFolder--; }
            if (nextFolder < 0) { nextFolder = int.MaxValue; }
            //WheelPageTime = ulong.MaxValue;
            bool emptyFolder = FileOperate.getIndexName(currFolder, 0) == null;
            //bool outFolder = nextFolder != currFolder && !emptyFolder && DialogResult.OK == MessageBox.Show("已经是该路径的第一个文件了，是否跳出当前路径？\n\n " + config.Path, "请确认", MessageBoxButtons.OKCancel);
            bool outFolder = true;
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
            //bool outSub = config.IsSub && !emptySub && DialogResult.OK == MessageBox.Show("已经是该文件夹的最后一个文件了，是否跳出当前文件夹？\n\n" + config.Name, "请确认", MessageBoxButtons.OKCancel);
            bool outSub = true;
            //WheelPageTime = TimeCount;
            if (config.IsSub && !outSub && !emptySub) { return; }

            nextFile++; nextSub = 0;
            if (!FileOperate.ExistFile(nextFolder, nextFile)) { nextFile = 0; nextFolder++; }
            if (!FileOperate.ExistFolder(nextFolder)) { nextFolder = 0; }
            //WheelPageTime = ulong.MaxValue;
            bool emptyFolder = FileOperate.getIndexName(currFolder, 0) == null;
            //bool outFolder = nextFolder != currFolder && !emptyFolder && DialogResult.OK == MessageBox.Show("已经是该路径的最后一个文件了，是否跳出当前路径？\n\n" + config.Path, "请确认", MessageBoxButtons.OKCancel);
            bool outFolder = true;
            //WheelPageTime = TimeCount;
            if (nextFolder != currFolder && !outFolder && !emptyFolder) { return; }

            config.FolderIndex = nextFolder;
            config.FileIndex = nextFile;
            config.SubIndex = nextSub;
            ShowCurrent();
        }
        private void RightMenu_Play(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Hide();
            RightMenu_Play_Start(null, null);
        }
        private void RightMenu_Play_Forward(object sender, EventArgs e)
        {
            bool only = play.Forward && !play.Backward && !play.Rand;
            if (only) { return; }

            this.forwardToolStripMenuItem.Checked = !this.forwardToolStripMenuItem.Checked;
            if (this.forwardToolStripMenuItem.Checked)
            {
                this.backwardToolStripMenuItem.Checked = false;
                this.randToolStripMenuItem.Checked = false;
            }

            play.Forward = this.forwardToolStripMenuItem.Checked;
            play.Backward = this.backwardToolStripMenuItem.Checked;
            play.Rand = this.randToolStripMenuItem.Checked;

            SearchPlayFile();
        }
        private void RightMenu_Play_Backward(object sender, EventArgs e)
        {
            bool only = !play.Forward && play.Backward && !play.Rand;
            if (only) { return; }

            this.backwardToolStripMenuItem.Checked = !this.backwardToolStripMenuItem.Checked;
            if (this.backwardToolStripMenuItem.Checked)
            {
                this.forwardToolStripMenuItem.Checked = false;
                this.randToolStripMenuItem.Checked = false;
            }

            play.Forward = this.forwardToolStripMenuItem.Checked;
            play.Backward = this.backwardToolStripMenuItem.Checked;
            play.Rand = this.randToolStripMenuItem.Checked;

            SearchPlayFile();
        }
        private void RightMenu_Play_Order(object sender, EventArgs e)
        {
            bool only = play.Order && !play.Single && !play.Circle;
            if (only) { return; }

            this.orderToolStripMenuItem.Checked = !this.orderToolStripMenuItem.Checked;
            if (this.orderToolStripMenuItem.Checked)
            {
                this.circleToolStripMenuItem.Checked = false;
                this.singleToolStripMenuItem.Checked = false;
            }

            play.Order = this.orderToolStripMenuItem.Checked;
            play.Circle = this.circleToolStripMenuItem.Checked;
            play.Single = this.singleToolStripMenuItem.Checked;

            SearchPlayFile();
        }
        private void RightMenu_Play_Circle(object sender, EventArgs e)
        {
            bool only = !play.Order && !play.Single && play.Circle;
            if (only) { return; }

            this.circleToolStripMenuItem.Checked = !this.circleToolStripMenuItem.Checked;
            if (this.circleToolStripMenuItem.Checked)
            {
                this.orderToolStripMenuItem.Checked = false;
                this.singleToolStripMenuItem.Checked = false;
            }

            play.Order = this.orderToolStripMenuItem.Checked;
            play.Circle = this.circleToolStripMenuItem.Checked;
            play.Single = this.singleToolStripMenuItem.Checked;

            SearchPlayFile();
        }
        private void RightMenu_Play_Single(object sender, EventArgs e)
        {
            bool only = !play.Order && play.Single && !play.Circle;
            if (only) { return; }

            this.singleToolStripMenuItem.Checked = !this.singleToolStripMenuItem.Checked;
            if (this.singleToolStripMenuItem.Checked)
            {
                this.orderToolStripMenuItem.Checked = false;
                this.circleToolStripMenuItem.Checked = false;
            }

            play.Order = this.orderToolStripMenuItem.Checked;
            play.Circle = this.circleToolStripMenuItem.Checked;
            play.Single = this.singleToolStripMenuItem.Checked;

            SearchPlayFile();
        }
        private void RightMenu_Play_Rand(object sender, EventArgs e)
        {
            bool only = !play.Forward && !play.Backward && play.Rand;
            if (only) { return; }

            this.randToolStripMenuItem.Checked = !this.randToolStripMenuItem.Checked;
            if (this.randToolStripMenuItem.Checked)
            {
                this.forwardToolStripMenuItem.Checked = false;
                this.backwardToolStripMenuItem.Checked = false;
            }

            play.Forward = this.forwardToolStripMenuItem.Checked;
            play.Backward = this.backwardToolStripMenuItem.Checked;
            play.Rand = this.randToolStripMenuItem.Checked;

            SearchPlayFile();
        }
        private void RightMenu_Play_TotalRoots(object sender, EventArgs e)
        {
            bool only = play.TotalRoots && !play.Root && !play.Subroot;
            if (only) { return; }

            this.totalRootsToolStripMenuItem.Checked = !this.totalRootsToolStripMenuItem.Checked;
            if (this.totalRootsToolStripMenuItem.Checked)
            {
                this.rootToolStripMenuItem.Checked = false;
                this.subrootToolStripMenuItem.Checked = false;
            }

            play.TotalRoots = this.totalRootsToolStripMenuItem.Checked;
            play.Root = this.rootToolStripMenuItem.Checked;
            play.Subroot = this.subrootToolStripMenuItem.Checked;

            SearchPlayFile();
        }
        private void RightMenu_Play_Root(object sender, EventArgs e)
        {
            bool only = !play.TotalRoots && play.Root && !play.Subroot;
            if (only) { return; }

            this.rootToolStripMenuItem.Checked = !this.rootToolStripMenuItem.Checked;
            if (this.rootToolStripMenuItem.Checked)
            {
                this.totalRootsToolStripMenuItem.Checked = false;
                this.subrootToolStripMenuItem.Checked = false;
            }

            play.TotalRoots = this.totalRootsToolStripMenuItem.Checked;
            play.Root = this.rootToolStripMenuItem.Checked;
            play.Subroot = this.subrootToolStripMenuItem.Checked;

            SearchPlayFile();
        }
        private void RightMenu_Play_Subroot(object sender, EventArgs e)
        {
            bool only = !play.TotalRoots && !play.Root && play.Subroot;
            if (only) { return; }

            this.subrootToolStripMenuItem.Checked = !this.subrootToolStripMenuItem.Checked;
            if (this.subrootToolStripMenuItem.Checked)
            {
                this.totalRootsToolStripMenuItem.Checked = false;
                this.rootToolStripMenuItem.Checked = false;
            }

            play.TotalRoots = this.totalRootsToolStripMenuItem.Checked;
            play.Root = this.rootToolStripMenuItem.Checked;
            play.Subroot = this.subrootToolStripMenuItem.Checked;

            SearchPlayFile();
        }
        private void RightMenu_Play_Picture(object sender, EventArgs e)
        {
            this.pictureToolStripMenuItem.Checked = !this.pictureToolStripMenuItem.Checked;
            play.Picture = this.pictureToolStripMenuItem.Checked;
            SearchPlayFile();
        }
        private void RightMenu_Play_Gif(object sender, EventArgs e)
        {
            this.gifToolStripMenuItem.Checked = !this.gifToolStripMenuItem.Checked;
            play.Gif = this.gifToolStripMenuItem.Checked;
            SearchPlayFile();
        }
        private void RightMenu_Play_Music(object sender, EventArgs e)
        {
            this.musicToolStripMenuItem.Checked = !this.musicToolStripMenuItem.Checked;
            play.Music = this.musicToolStripMenuItem.Checked;
            SearchPlayFile();
        }
        private void RightMenu_Play_Video(object sender, EventArgs e)
        {
            this.videoToolStripMenuItem.Checked = !this.videoToolStripMenuItem.Checked;
            play.Video = this.videoToolStripMenuItem.Checked;
            SearchPlayFile();
        }
        private void RightMenu_Play_ShowTime(object sender, EventArgs e)
        {
            Form_Input input = new Form_Input(play.ShowTime.ToString());
            input.Location = MousePosition;
            input.ShowDialog();

            int showtime = 0;
            try { showtime = int.Parse(input.Input); } catch { return; }
            if (showtime < 0 || showtime > 0x0FFFFFFF) { return; }

            play.ShowTime = (ulong)showtime;
            this.showTimeToolStripMenuItem.Text = "时间：" + play.ShowTime.ToString();
        }
        private void RightMenu_Play_Start(object sender, EventArgs e)
        {
            if (play.IsPlaying) { play.IsPlaying = false; return; }

            string name = config.IsSub ? (config.SubFiles.Count == 0 ? "unknow" : config.SubFiles[config.SubIndex]) : config.Name;
            string extension = FileOperate.getExtension(name);
            int type = FileOperate.getFileType(extension);
            bool isMusic = FileOperate.IsMusic(extension);
            bool isVideo = FileOperate.IsVideo(extension);

            play.IsPlaying = true;
            play.Begin = TimeCount;
            play.Type = 2;
            if (type == 2) { play.Type = 2; }
            if (type == 3) { play.Type = 3; }
            if (type == 4 && isMusic) { play.Type = 4; }
            if (type == 4 && isVideo) { play.Type = 5; }

            if (play.Subroot && (!config.IsSub || config.SubFiles.Count == 0))
            {
                this.totalRootsToolStripMenuItem.Checked = false;
                this.rootToolStripMenuItem.Checked = true;
                this.subrootToolStripMenuItem.Checked = false;

                play.TotalRoots = false;
                play.Root = true;
                play.Subroot = false;
            }
            if (!play.Forward && !play.Backward && !play.Rand)
            {
                this.forwardToolStripMenuItem.Checked = true;
                this.backwardToolStripMenuItem.Checked = true;
                this.randToolStripMenuItem.Checked = true;

                play.Forward = true;
            }
            if (!play.TotalRoots && !play.Root && !play.Subroot)
            {
                this.subrootToolStripMenuItem.Checked = config.IsSub && config.SubFiles.Count != 0;
                this.rootToolStripMenuItem.Checked = !this.subrootToolStripMenuItem.Checked;
                this.totalRootsToolStripMenuItem.Checked = false;

                play.Root = this.rootToolStripMenuItem.Checked;
                play.Subroot = this.subrootToolStripMenuItem.Checked;
            }
            if (!play.Picture && !play.Gif && !play.Music && !play.Video)
            {
                if (play.Type == 2) { this.pictureToolStripMenuItem.Checked = true; }
                else if (play.Type == 3) { this.gifToolStripMenuItem.Checked = true; }
                else if (play.Type == 4) { this.musicToolStripMenuItem.Checked = true; }
                else if (play.Type == 5) { this.videoToolStripMenuItem.Checked = true; }
                else { this.pictureToolStripMenuItem.Checked = true; }

                play.Picture = play.Type == 2;
                play.Gif = play.Type == 3;
                play.Music = play.Type == 4;
                play.Video = play.Type == 5;
            }
            if (!play.Single && !play.Order && !play.Circle)
            {
                this.circleToolStripMenuItem.Checked = true;
                play.Circle = true;
            }

            SearchPlayFile();
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
