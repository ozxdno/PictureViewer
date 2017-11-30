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
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

using PictureViewer.Files;
using PictureViewer.Tools;

namespace PictureViewer.Forms
{
    /// <summary>
    /// 主查看器
    /// </summary>
    public partial class Form_Main : Form
    {
        ///////////////////////////////////////////////////// public attribute ///////////////////////////////////////////////

        /// <summary>
        /// 基本索引号
        /// </summary>
        public BaseIndex BaseIndex
        {
            get { return TreeIndex.Base; }
        }
        /// <summary>
        /// 树形索引号
        /// </summary>
        public TreeIndex TreeIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 切换到该树形索引号
        /// </summary>
        public TreeIndex SwitchIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 已经对释放请求做出回答
        /// </summary>
        public bool Answered
        {
            set;
            get;
        }
        
        /// <summary>
        /// 当前显示文件的文件类型
        /// </summary>
        public FileType Type
        {
            set;
            get;
        }

        ///////////////////////////////////////////////////// private attribute ///////////////////////////////////////////////

        private bool initializing = true;
        private System.Timers.Timer timer = new System.Timers.Timer(10);
        private Image sourPicture = null;
        private Image destPicture = null;
        private bool nextShowBig = false;
        private bool useSmallWindow = false;
        private bool useBoard = false;
        private int boardH = 0;
        private int boardW = 0;
        private int clientH
        {
            get { return useBoard ? this.Height - boardH : this.Height; }
            set { this.Height = useBoard ? value + boardH : value; }
        }
        private int clientW
        {
            get { return useBoard ? this.Width + boardW : this.Width; }
            set { this.Width = useBoard ? value + boardW : value; }
        }
        private int clientT = 0;
        private int clientL = 0;
        private bool notEnoughH
        {
            get
            {
                if (!this.axWindowsMediaPlayer1.Visible && !this.pictureBox1.Visible)
                { return false; }

                if (Type == FileType.Music || Type == FileType.Video)
                { return this.Height < this.axWindowsMediaPlayer1.Height; }
                return this.Height < this.pictureBox1.Height;
            }
        }
        private bool notEnoughW
        {
            get
            {
                if (!this.axWindowsMediaPlayer1.Visible && !this.pictureBox1.Visible)
                { return false; }

                if (Type == FileType.Music || Type == FileType.Video)
                { return this.Width < this.axWindowsMediaPlayer1.Width; }
                return this.Width < this.pictureBox1.Width;
            }
        }
        private bool notEnough
        {
            get { return notEnoughH || notEnoughW; }
        }
        private bool useShape = false;
        private double maxWindowRate = Tools.Screen.MaxWindowRate;
        private double minWindowRate = Tools.Screen.MinWindowRate;
        private double maxControlRate = Tools.Screen.MaxControlRate;
        private double minControlRate = Tools.Screen.MinControlRate;
        private double shapeWindowRate = 0;
        private double shapeControlRate = 0;
        private Cursor cursor = Cursors.Default;
        private bool canResize
        {
            get { return canResizeL || canResizeR || canResizeU || canResizeD; }
        }
        private bool canResizeL
        {
            get { return !useBoard && disL <= 3 && disL >= 0; }
        }
        private bool canResizeR
        {
            get { return !useBoard && disR <= 3 && disR >= 0 && !canResizeL; }
        }
        private bool canResizeU
        {
            get { return !useBoard && disU <= 3 && disU >= 0; }
        }
        private bool canResizeD
        {
            get { return !useBoard && disD <= 3 && disD >= 0 && !canResizeU; }
        }
        private int disL = 0;
        private int disR = 0;
        private int disU = 0;
        private int disD = 0;
        private bool isResizing = false;
        private int beforeResizeH = 0;
        private int beforeResizeW = 0;
        private bool isActive = true;
        private Tools.MouseModel mouse = new MouseModel();
        private Tools.KeyModel keyL = null;
        private Tools.KeyModel keyR = null;
        private Tools.KeyModel keyU = null;
        private Tools.KeyModel keyD = null;
        private PlayModel play = new PlayModel();
        private int fastkey_Esc = 0;
        private int fastkey_Enter = 0;
        private int fastkey_Board = 0;
        private int fastkey_U = 0;
        private int fastkey_D = 0;
        private int fastkey_L = 0;
        private int fastkey_R = 0;
        private int fastkey_PageU = 0;
        private int fastkey_PageD = 0;
        private int fastkey_Export = 0;
        private int fastkey_OpenExport = 0;
        private int fastkey_OpenRoot = 0;
        private int fastkey_OpenSubroot = 0;
        private int fastkey_OpenFile = 0;
        private int fastkey_Password = 0;
        private int fastkey_Rotate = 0;
        private int fastkey_FlipX = 0;
        private int fastkey_FlipY = 0;
        private object locker = new object();

        ///////////////////////////////////////////////////// public method ///////////////////////////////////////////////

        /// <summary>
        /// 创建主查看器
        /// </summary>
        public Form_Main()
        {
            InitializeComponent();
            this.MouseWheel += Form_MouseWheel;
        }
        /// <summary>
        /// 加载配置信息
        /// </summary>
        public void LoadConfig()
        {
            Files.Manager.Pvini.Model.Load();
            Files.Load pvini = Files.Manager.Pvini.Model.LoadContent;
            bool found = false;
            bool tempBool = false;
            int tempInt = 0;
            string tempString = "";
            long tempLong = 0;

            tempInt = -1; found = pvini.Get("Main_FolderIndex",ref tempInt);
            TreeIndex.Folder = tempInt;
            tempInt = -1; found = pvini.Get("Main_FileIndex", ref tempInt);
            TreeIndex.File = tempInt;
            tempInt = -1; found = pvini.Get("Main_SubIndex", ref tempInt);
            TreeIndex.Sub = tempInt;

            found = pvini.Get("Main_ExportPath", ref tempString);
            if (System.IO.Directory.Exists(tempString)) { Files.Manager.Root.ExportPath = tempString; }
            {
                Files.Manager.Root.ExportPath = Tools.FileOperate.getExePath();
            }

            while (pvini.Get("Main_RootPath", ref tempString))
            {
                Files.Manager.Root.Input(tempString);
            }

            tempBool = false; pvini.Get("Main_HideL", ref tempBool);
            this.hideLToolStripMenuItem.Checked = tempBool;
            tempBool = false; pvini.Get("Main_HideR", ref tempBool);
            this.hideRToolStripMenuItem.Checked = tempBool;
            tempBool = false; pvini.Get("Main_HideU", ref tempBool);
            this.hideUToolStripMenuItem.Checked = tempBool;
            tempBool = false; pvini.Get("Main_HideD", ref tempBool);
            this.hideDToolStripMenuItem.Checked = tempBool;

            boardW = this.Width - this.ClientSize.Width;
            boardH = this.Height - this.ClientSize.Height;
            clientL = this.PointToScreen(new Point(0, 0)).X - this.Location.X;
            clientT = this.PointToScreen(new Point(0, 0)).Y - this.Location.Y;

            found = pvini.Get("Main_SmallWindow", ref useSmallWindow);
            if (!found) { useSmallWindow = false; }
            found = pvini.Get("Main_ShowBoard", ref useBoard);
            if (!found) { useBoard = false; }
            found = pvini.Get("Main_ShapeWindow", ref useShape);
            if (!found) { useShape = true; }

            if (!useBoard) { this.FormBorderStyle = FormBorderStyle.None; }

            pvini.Get("Main_ShapeWindowRate", ref shapeWindowRate);
            if (shapeWindowRate < Tools.Screen.MinWindowRate) { shapeWindowRate = Tools.Screen.MinWindowRate; }
            if (shapeWindowRate > Tools.Screen.MaxWindowRate) { shapeWindowRate = Tools.Screen.MaxWindowRate; }
            pvini.Get("Main_ShapeControlRate", ref shapeControlRate);
            if (shapeControlRate < Tools.Screen.MinControlRate) { shapeControlRate = Tools.Screen.MinControlRate; }
            if (shapeControlRate > Tools.Screen.MaxControlRate) { shapeControlRate = Tools.Screen.MaxControlRate; }
            
            tempInt = 0; pvini.Get("Main_Height", ref tempInt);
            if (tempInt < Tools.Screen.MinWindowHeight) { tempInt = Tools.Screen.MinWindowHeight; }
            if (tempInt > Tools.Screen.MaxWindowHeight) { tempInt = Tools.Screen.MaxWindowHeight; }
            this.Height = useSmallWindow ? Tools.Screen.MinWindowHeight : tempInt;
            tempInt = 0; pvini.Get("Main_Width", ref tempInt);
            if (tempInt < Tools.Screen.MinWindowWidth) { tempInt = Tools.Screen.MinWindowWidth; }
            if (tempInt > Tools.Screen.MaxWindowWidth) { tempInt = Tools.Screen.MaxWindowWidth; }
            this.Width = useSmallWindow ? Tools.Screen.MinWindowWidth : tempInt;

            int xloc = 0; pvini.Get("Main_LocationX", ref xloc);
            int yloc = 0; pvini.Get("Main_LocationY", ref yloc);
            if (xloc < 0 || xloc > Tools.Screen.Width || yloc < 0 || yloc > Tools.Screen.Height)
            {
                this.Location = Tools.WinOperate.GetLocation_ToCentre(this.Width, this.Height);
            }
            else
            {
                this.Location = new Point(xloc, yloc);
            }

            tempBool = false; pvini.Get("Main_Lock", ref tempBool);
            this.lockToolStripMenuItem.Checked = tempBool;

            tempBool = false; pvini.Get("Main_PlayTotal", ref tempBool); play.Total = tempBool;
            tempBool = false; pvini.Get("Main_PlayRoot", ref tempBool); play.Root = tempBool;
            tempBool = false; pvini.Get("Main_PlaySubroot", ref tempBool); play.Subroot = tempBool;
            if (!play.Total && !play.Root && !play.Subroot) { play.Root = true; }
            if (play.Root) { play.Total = false; play.Subroot = false; }
            if (play.Subroot) { play.Total = false; play.Root = false; }
            if (play.Total) { play.Root = false; play.Subroot = false; }
            this.totalToolStripMenuItem.Checked = play.Total;
            this.rootToolStripMenuItem.Checked = play.Root;
            this.subrootToolStripMenuItem.Checked = play.Subroot;

            tempBool = false; pvini.Get("Main_PlayForward", ref tempBool); play.Forward = tempBool;
            tempBool = false; pvini.Get("Main_PlayBackward", ref tempBool); play.Backward = tempBool;
            tempBool = false; pvini.Get("Main_PlayRand", ref tempBool); play.Rand = tempBool;
            if (!play.Forward && !play.Backward && !play.Rand) { play.Forward = true; }
            if (play.Forward) { play.Backward = false; play.Rand = false; }
            if (play.Backward) { play.Forward = false; play.Rand = false; }
            if (play.Rand) { play.Forward = false; play.Backward = false; }
            this.forwardToolStripMenuItem.Checked = play.Forward;
            this.backwardToolStripMenuItem.Checked = play.Backward;
            this.randToolStripMenuItem.Checked = play.Rand;

            tempBool = false; pvini.Get("Main_PlayPicture", ref tempBool); play.Picture = tempBool;
            tempBool = false; pvini.Get("Main_PlayGif", ref tempBool); play.Gif = tempBool;
            tempBool = false; pvini.Get("Main_PlayMusic", ref tempBool); play.Music = tempBool;
            tempBool = false; pvini.Get("Main_PlayVideo", ref tempBool); play.Video = tempBool;
            this.pictureToolStripMenuItem.Checked = play.Picture;
            this.gifToolStripMenuItem.Checked = play.Gif;
            this.musicToolStripMenuItem.Checked = play.Music;
            this.videoToolStripMenuItem.Checked = play.Video;

            tempBool = false; pvini.Get("Main_PlaySingle", ref tempBool); play.Single = tempBool;
            tempBool = false; pvini.Get("Main_PlayOrder", ref tempBool); play.Order = tempBool;
            tempBool = false; pvini.Get("Main_PlayCircle", ref tempBool); play.Circle = tempBool;
            if (!play.Single && !play.Order && !play.Circle) { play.Circle = true; }
            if (play.Circle) { play.Single = false; play.Order = false; }
            if (play.Order) { play.Single = false; play.Circle = false; }
            if (play.Single) { play.Order = false; play.Circle = false; }
            this.singleToolStripMenuItem.Checked = play.Single;
            this.orderToolStripMenuItem.Checked = play.Order;
            this.circleToolStripMenuItem.Checked = play.Circle;

            tempLong = 0; pvini.Get("Main_PlayLasting", ref tempLong);
            play.Lasting = tempLong;
            
            pvini.Get("Main_FastKey_Esc", ref fastkey_Esc);
            pvini.Get("Main_FastKey_Enter", ref fastkey_Enter);
            pvini.Get("Main_FastKey_Board", ref fastkey_Board);
            pvini.Get("Main_FastKey_U", ref fastkey_U);
            pvini.Get("Main_FastKey_D", ref fastkey_D);
            pvini.Get("Main_FastKey_L", ref fastkey_L);
            pvini.Get("Main_FastKey_R", ref fastkey_R);
            pvini.Get("Main_FastKey_PageU", ref fastkey_PageU);
            pvini.Get("Main_FastKey_PageD", ref fastkey_PageD);
            pvini.Get("Main_FastKey_Export", ref fastkey_Export);
            pvini.Get("Main_FastKey_OpenExport", ref fastkey_OpenExport);
            pvini.Get("Main_FastKey_OpenRoot", ref fastkey_OpenRoot);
            pvini.Get("Main_FastKey_OpenSubroot", ref fastkey_OpenSubroot);
            pvini.Get("Main_FastKey_OpenFile", ref fastkey_OpenFile);
            pvini.Get("Main_FastKey_Password", ref fastkey_Password);
            pvini.Get("Main_FastKey_Rotate", ref fastkey_Rotate);
            pvini.Get("Main_FastKey_FlipX", ref fastkey_FlipX);
            pvini.Get("Main_FastKey_FlipY", ref fastkey_FlipY);
            if (fastkey_Esc <= 0) { fastkey_Esc = (int)System.ConsoleKey.Escape; }
            if (fastkey_Enter <= 0) { fastkey_Enter = (int)System.ConsoleKey.Enter; }
            if (fastkey_Board <= 0) { fastkey_Board = (int)System.ConsoleKey.A; }
            if (fastkey_U <= 0) { fastkey_U = (int)System.ConsoleKey.UpArrow; }
            if (fastkey_D <= 0) { fastkey_D = (int)System.ConsoleKey.DownArrow; }
            if (fastkey_L <= 0) { fastkey_L = (int)System.ConsoleKey.LeftArrow; }
            if (fastkey_R <= 0) { fastkey_R = (int)System.ConsoleKey.RightArrow; }
            if (fastkey_PageU <= 0) { fastkey_PageU = (int)System.ConsoleKey.PageUp; }
            if (fastkey_PageD <= 0) { fastkey_PageD = (int)System.ConsoleKey.PageDown; }
            if (fastkey_Export <= 0) { fastkey_Export = (int)System.ConsoleKey.Delete; }
            if (fastkey_OpenExport <= 0) { fastkey_OpenExport = (int)System.ConsoleKey.D1; }
            if (fastkey_OpenRoot <= 0) { fastkey_OpenRoot = (int)System.ConsoleKey.D2; }
            if (fastkey_OpenSubroot <= 0) { fastkey_OpenSubroot = (int)System.ConsoleKey.D4; }
            if (fastkey_OpenFile <= 0) { fastkey_OpenFile = (int)System.ConsoleKey.D3; }
            if (fastkey_Password <= 0) { fastkey_Password = (int)System.ConsoleKey.P; }
            if (fastkey_Rotate <= 0) { fastkey_Rotate = (int)System.ConsoleKey.R; }
            if (fastkey_FlipX <= 0) { fastkey_FlipX = (int)System.ConsoleKey.X; }
            if (fastkey_FlipY <= 0) { fastkey_FlipY = (int)System.ConsoleKey.Y; }

            this.Text = Tools.MainStrings.WindowTitle;
            this.refreshToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Refresh;
            this.playToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Play;
            this.startToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlayStart;
            this.totalToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlayTotal;
            this.rootToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlayRoot;
            this.subrootToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlaySubroot;
            this.forwardToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlayForward;
            this.backwardToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlayBackward;
            this.randToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlayRand;
            this.pictureToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlayPicture;
            this.gifToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlayGif;
            this.musicToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlayMusic;
            this.videoToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlayVideo;
            this.singleToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlaySingle;
            this.orderToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlayOrder;
            this.circleToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlayCircle;
            this.lastingToolStripMenuItem.Text = Tools.MainStrings.RightMenu_PlayLasting +
                Tools.NumForm.ToString((double)play.Lasting / 100, -1, 1) + "s";
            this.renameToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Rename;
            this.inputToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Input;
            this.exportToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Export;
            this.exportPathToolStripMenuItem.Text = Tools.MainStrings.RightMenu_ExportPath;
            this.exportFolderToolStripMenuItem.Text = Tools.MainStrings.RightMenu_ExportFolder;
            this.mainToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Main;
            this.infoToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Info;
            this.previousToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Previous;
            this.nextToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Next;
            this.shapeToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Shape;
            this.lockToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Lock;
            this.hideToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Hide;
            this.hideLToolStripMenuItem.Text = Tools.MainStrings.RightMenu_HideL;
            this.hideRToolStripMenuItem.Text = Tools.MainStrings.RightMenu_HideR;
            this.hideUToolStripMenuItem.Text = Tools.MainStrings.RightMenu_HideU;
            this.hideDToolStripMenuItem.Text = Tools.MainStrings.RightMenu_HideD;
            this.rootPathToolStripMenuItem.Text = Tools.MainStrings.RightMenu_RootPath;
            this.deleteToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Delete;
            this.searchToolStripMenuItem.Text = Tools.MainStrings.RightMenu_Search;
            this.listToolStripMenuItem.Text = Tools.MainStrings.RightMenu_List;
            this.openExportToolStripMenuItem.Text = Tools.MainStrings.RightMenu_OpenExport;
            this.openRootToolStripMenuItem.Text = Tools.MainStrings.RightMenu_OpenRoot;
            this.openFileToolStripMenuItem.Text = Tools.MainStrings.RightMenu_OpenFile;
            this.openSubrootToolStripMenuItem.Text = Tools.MainStrings.RightMenu_OpenSubroot;
        }
        /// <summary>
        /// 保存配置信息
        /// </summary>
        public void SaveConfig()
        {
            Save pvini = Files.Manager.Pvini.Model.SaveContent;

            pvini.Set("Main_FolderIndex", TreeIndex.Folder);
            pvini.Set("Main_FileIndex", TreeIndex.File);
            pvini.Set("Main_SubIndex", TreeIndex.Sub);

            pvini.Set("Main_ExportPath", Files.Manager.Root.ExportPath);
            foreach (string str in Files.Manager.Root.RootPath)
            {
                pvini.Set("Main_RootPath", str);
            }

            pvini.Set("Main_HideL", this.hideLToolStripMenuItem.Checked);
            pvini.Set("Main_HideR", this.hideRToolStripMenuItem.Checked);
            pvini.Set("Main_HideU", this.hideUToolStripMenuItem.Checked);
            pvini.Set("Main_HideD", this.hideDToolStripMenuItem.Checked);

            pvini.Set("Main_SmallWindow", useSmallWindow);
            pvini.Set("Main_ShowBoard", useBoard);
            pvini.Set("Main_ShapeWindow", useShape);

            pvini.Set("Main_ShapeControlRate", shapeControlRate);
            pvini.Set("Main_ShapeWindowRate", shapeWindowRate);

            pvini.Set("Main_Height", this.Height);
            pvini.Set("Main_Width", this.Width);
            pvini.Set("Main_LocationX", this.Location.X);
            pvini.Set("Main_LocationY", this.Location.Y);
            pvini.Set("Main_Lock", this.lockToolStripMenuItem.Checked);

            pvini.Set("Main_PlayTotal", play.Total);
            pvini.Set("Main_PlayRoot", play.Root);
            pvini.Set("Main_PlaySubroot", play.Subroot);
            pvini.Set("Main_PlayForward", play.Forward);
            pvini.Set("Main_PlayBackward", play.Backward);
            pvini.Set("Main_PlayRand", play.Rand);
            pvini.Set("Main_PlayPicture", play.Picture);
            pvini.Set("Main_PlayGif", play.Gif);
            pvini.Set("Main_PlayMusic", play.Music);
            pvini.Set("Main_PlayVideo", play.Video);
            pvini.Set("Main_PlaySingle", play.Single);
            pvini.Set("Main_PlayOrder", play.Order);
            pvini.Set("Main_PlayCircle", play.Circle);
            pvini.Set("Main_PlayLasting", play.Lasting);

            pvini.Set("Main_FastKey_Esc", fastkey_Esc);
            pvini.Set("Main_FastKey_Enter", fastkey_Enter);
            pvini.Set("Main_FastKey_Board", fastkey_Board);
            pvini.Set("Main_FastKey_U", fastkey_U);
            pvini.Set("Main_FastKey_D", fastkey_D);
            pvini.Set("Main_FastKey_L", fastkey_L);
            pvini.Set("Main_FastKey_R", fastkey_R);
            pvini.Set("Main_FastKey_PageU", fastkey_PageU);
            pvini.Set("Main_FastKey_PageD", fastkey_PageD);
            pvini.Set("Main_FastKey_Export", fastkey_Export);
            pvini.Set("Main_FastKey_OpenExport", fastkey_OpenExport);
            pvini.Set("Main_FastKey_OpenRoot", fastkey_OpenRoot);
            pvini.Set("Main_FastKey_OpenSubroot", fastkey_OpenSubroot);
            pvini.Set("Main_FastKey_OpenFile", fastkey_OpenFile);
            pvini.Set("Main_FastKey_Password", fastkey_Password);
            pvini.Set("Main_FastKey_Rotate", fastkey_Rotate);
            pvini.Set("Main_FastKey_FlipX", fastkey_FlipX);
            pvini.Set("Main_FastKey_FlipY", fastkey_FlipY);
        }

        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////

        private void Form_Main_Loaded(object sender, EventArgs e)
        {
            LoadConfig();

            SwitchIndex = new TreeIndex();
            keyL = new KeyModel(fastkey_L);
            keyR = new KeyModel(fastkey_R);
            keyU = new KeyModel(fastkey_U);
            keyD = new KeyModel(fastkey_D);

            timer.Elapsed += new System.Timers.ElapsedEventHandler(Form_Main_Updata);
            timer.AutoReset = true;
            timer.Start();
        }
        private void Form_Main_Closed(object sender, FormClosedEventArgs e)
        {
            SaveConfig();
        }
        private void Form_Main_Updata(object source, System.Timers.ElapsedEventArgs e)
        {
            this.BeginInvoke((EventHandler)delegate
            {
                #region 判断是否正在初始化

                initializing = Files.Manager.Pvdata.Model.Loaded;

                #endregion

                #region 上下左右翻动控件

                if (notEnough && (keyL.Down || keyR.Down || keyU.Down || keyD.Down))
                {
                    if (keyL.Down) { SetScrollGainW(-5); }
                    if (keyR.Down) { SetScrollGainW(5); }
                    if (keyU.Down) { SetScrollGainH(-5); }
                    if (keyD.Down) { SetScrollGainH(5); }
                }

                #endregion

                if (Tools.Time.Ticks % 100 != 0){ return; }

                #region 释放请求

                if (!Answered && TreeIndex.Exist && TreeIndex.Model.Asking)
                {
                    ShowCurrent();
                    TreeIndex.Model.Dispose(BaseIndex);
                }

                #endregion

                #region 初始化完成

                #endregion

                #region 能否拉动窗体

                disL = MousePosition.X - this.Location.X;
                disR = this.Location.X + this.Width - MousePosition.X;
                disU = MousePosition.Y - this.Location.Y;
                disD = this.Location.Y + this.Height - MousePosition.Y;

                if (!mouse.Left.Down && !mouse.Right.Down && isActive && canResize)
                {
                    if (canResizeL || canResizeR) { cursor = Cursors.SizeWE; }
                    if (canResizeU || canResizeD) { cursor = Cursors.SizeNS; }
                    if (canResizeL && canResizeU) { cursor = Cursors.SizeNESW; }
                    if (canResizeR && canResizeD) { cursor = Cursors.SizeNESW; }
                    if (canResizeL && canResizeD) { cursor = Cursors.SizeNWSE; }
                    if (canResizeR && canResizeU) { cursor = Cursors.SizeNWSE; }
                }

                #endregion

                #region 拉伸窗体

                if (isResizing && mouse.Left.Down)
                {
                    int xmove = MousePosition.X - mouse.Left.pMouseDown.X;
                    int ymove = MousePosition.Y - mouse.Left.pMouseDown.Y;

                    if (xmove != 0 || ymove != 0)
                    {
                        int resizeh = beforeResizeH;
                        int resizew = beforeResizeW;
                        int xloc = mouse.Left.pForm.X;
                        int yloc = mouse.Left.pForm.Y;

                        if (canResizeL) { resizew -= xmove; xloc += xmove; }
                        if (canResizeR) { resizew += xmove; }
                        if (canResizeU) { resizeh -= ymove; yloc += ymove; }
                        if (canResizeD) { resizeh += ymove; }

                        this.Height = resizeh;
                        this.Width = resizew;
                        this.Location = new Point(xloc, yloc);
                    }
                }

                #endregion

                #region 转到

                if (!initializing && SwitchIndex.Exist)
                {
                    play.IsPlaying = false;
                    TreeIndex = SwitchIndex.Copy();
                    ShowCurrent();
                }

                #endregion
                
                #region 播放

                #region 菜单栏刷新

                this.playToolStripMenuItem.Checked = play.IsPlaying =
                    !initializing &&
                    play.IsPlaying &&
                    play.SortList.Count != 0;
                this.startToolStripMenuItem.Text = play.IsPlaying ?
                    Tools.MainStrings.RightMenu_PlayStart :
                    Tools.MainStrings.RightMenu_PlayStop;

                #endregion

                if (play.IsPlaying)
                {
                    #region 是否播放下一个文件

                    bool playNext = true;

                    if (Type == FileType.Music || Type == FileType.Video)
                    {
                        playNext = this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped;
                    }
                    else
                    {
                        playNext = Tools.Time.Ticks > play.End;
                    }
                    
                    #endregion

                    #region 是否存在下一个

                    TreeIndex next = new TreeIndex();
                    playNext = play.GetNext(ref next);

                    #endregion

                    #region 下一个

                    if (playNext)
                    {
                        if (play.Rand && play.IsCircleEnd) { play.SetRandList(); play.GetCurrent(ref next); }
                        TreeIndex = next;
                        play.Index++;
                        play.Begin = Tools.Time.Ticks;
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
                
                bool hidePageMark =
                    mouse.Left.Down ||
                    mouse.Right.Down ||
                    clientH < 150 ||
                    clientW < 150 ||
                    this.contextMenuStrip1.Visible ||
                    canResize ||
                    !isActive;

                #region 左翻页
                
                if (this.hideLToolStripMenuItem.Checked || hidePageMark) { this.label1.Visible = false; }
                else
                {
                    int h = clientH / 5;
                    int w = clientW / 20;
                    int f = w * 3 / 4;

                    int xbg = clientW / 20;
                    int xed = xbg + w;
                    int ybg = clientH / 2 - h / 2;
                    int yed = ybg + h;

                    int xm = this.PointToClient(MousePosition).X;
                    int ym = this.PointToClient(MousePosition).Y;

                    bool inRange = xbg <= xm && xm <= xed && ybg <= ym && ym <= yed;
                    if (!inRange) { this.label1.Visible = false; }
                    else
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

                if (this.hideRToolStripMenuItem.Checked || hidePageMark) { this.label2.Visible = false; }
                else
                {
                    int h = clientH / 5;
                    int w = clientW / 20;
                    int f = w * 3 / 4;

                    int xbg = clientW - clientW / 20 - w;
                    int xed = xbg + w;
                    int ybg = clientH / 2 - h / 2;
                    int yed = ybg + h;

                    int xm = this.PointToClient(MousePosition).X;
                    int ym = this.PointToClient(MousePosition).Y;

                    bool inRange = xbg <= xm && xm <= xed && ybg <= ym && ym <= yed;
                    if (!inRange) { this.label2.Visible = false; }
                    else
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

                if (this.hideUToolStripMenuItem.Checked || hidePageMark) { this.label3.Visible = false; }
                else
                {
                    int h = clientH / 12;
                    int w = clientW / 8;
                    int f = h * 3 / 4;

                    int xbg = clientW / 2 - w / 2;
                    int xed = xbg + w;
                    int ybg = clientH / 20;
                    int yed = ybg + h;

                    int xm = this.PointToClient(MousePosition).X;
                    int ym = this.PointToClient(MousePosition).Y;

                    bool inRange = xbg <= xm && xm <= xed && ybg <= ym && ym <= yed;
                    if (!inRange) { this.label3.Visible = false; }
                    else
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

                if (this.hideDToolStripMenuItem.Checked || hidePageMark) { this.label4.Visible = false; }
                else
                {
                    int h = clientH / 12;
                    int w = clientW / 8;
                    int f = h * 3 / 4;

                    int xbg = clientW / 2 - w / 2;
                    int xed = xbg + w;
                    int ybg = clientH - clientH / 20 - h;
                    int yed = ybg + h;

                    int xm = this.PointToClient(MousePosition).X;
                    int ym = this.PointToClient(MousePosition).Y;

                    bool inRange = xbg <= xm && xm <= xed && ybg <= ym && ym <= yed;
                    if (!inRange) { this.label4.Visible = false; }
                    else
                    {
                        this.label4.Height = h;
                        this.label4.Width = w;
                        this.label4.Font = new Font("宋体", f);
                        this.label4.Location = new Point(xbg, ybg);
                        this.label4.Visible = true;
                    }
                }

                #endregion

                #region 中间提示键

                this.label5.Visible = isActive &&
                    !mouse.Left.Down &&
                    !mouse.Right.Down &&
                    !canResize &&
                    !isResizing &&
                    !this.contextMenuStrip1.Visible &&
                    clientH > 150 &&
                    clientW > 150 &&
                    Math.Abs(clientH / 2 - this.PointToClient(MousePosition).Y) <= this.label5.Height &&
                    Math.Abs(clientW / 2 - this.PointToClient(MousePosition).X) <= this.label5.Width;

                if (this.label5.Visible && Files.Manager.Index.IndexPairs.Count == 0)
                {
                    this.label5.Text = Tools.MainStrings.ButtonText_Input;
                    int x = clientW / 2 - this.label5.Width / 2;
                    int y = clientH / 2 - this.label5.Height / 2;
                    this.label5.Location = new Point(x, y);
                }
                else if (this.label5.Visible && play.IsPlaying)
                {
                    this.label5.Text = Tools.MainStrings.ButtonText_Stop;
                    int x = clientW / 2 - this.label5.Width / 2;
                    int y = clientH / 2 - this.label5.Height / 2;
                    this.label5.Location = new Point(x, y);
                }
                else if (this.label5.Visible && (Type == FileType.Music || Type == FileType.Video))
                {
                    this.label5.Text = this.axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying ?
                        Tools.MainStrings.ButtonText_Pause :
                        Tools.MainStrings.ButtonText_Start;

                    int x = clientW / 2 - this.label5.Width / 2;
                    int y = clientH / 2 - this.label5.Height / 2;
                    this.label5.Location = new Point(x, y);
                }

                #endregion

                #region 进度条

                this.trackBar1.Visible =
                    isActive &&
                    !mouse.Left.Down &&
                    !mouse.Right.Down &&
                    !canResize &&
                    !isResizing &&
                    clientW > 150 &&
                    clientH > 150 &&
                    !this.contextMenuStrip1.Visible &&
                    this.PointToClient(MousePosition).Y >= clientH - this.trackBar1.Height &&
                    this.PointToClient(MousePosition).Y <= clientH;

                if (this.trackBar1.Visible)
                {
                    int x = 0;
                    int y = clientH - this.trackBar1.Height;
                    this.trackBar1.Location = new Point(x, y);
                    this.trackBar1.Width = clientW;
                    this.trackBar1.Maximum = clientW;

                    x = 0;
                    y = y - this.label6.Height;
                    this.label6.Location = new Point(x, y);
                }

                #endregion

                #region 拖拽窗体

                if (!canResize && !isResizing && !notEnough && mouse.Left.Down)
                {
                    int xmove = MousePosition.X - mouse.Left.pMouseDown.X;
                    int ymove = MousePosition.Y - mouse.Left.pMouseDown.Y;
                    this.Location = new Point(mouse.Left.pForm.X + xmove, mouse.Left.pForm.Y + ymove);
                }

                #endregion

                #region 右键拖拽窗体

                if (!canResize && !isResizing && mouse.Right.Down && notEnough)
                {
                    int xmove = MousePosition.X - mouse.Right.pMouseDown.X;
                    int ymove = MousePosition.Y - mouse.Right.pMouseDown.Y;
                    this.Location = new Point(mouse.Right.pForm.X + xmove, mouse.Right.pForm.Y + ymove);
                }

                #endregion

                #region 拖拽控件

                if (!canResize && !isResizing && notEnough && mouse.Left.Down)
                {
                    int xS = MousePosition.X - mouse.Left.pMouseDown.X;
                    int yS = MousePosition.Y - mouse.Left.pMouseDown.Y;

                    SetScrollW(mouse.Left.xScroll - xS);
                    SetScrollH(mouse.Left.yScroll - yS);
                }


                #endregion

                #region 刷新播放时间
                if (Type == FileType.Music || Type == FileType.Video)
                {
                    string index = "[" + (TreeIndex.File + 1).ToString() + "/" + TreeIndex.TotalFiles.ToString() + "]";
                    string subindex = "[" + (TreeIndex.Sub + 1).ToString() + "/" + TreeIndex.TotalSubs.ToString() + "]";
                    string curpos = "";
                    string total = "";
                    try { curpos = this.axWindowsMediaPlayer1.Ctlcontrols.currentPositionString; } catch { }
                    try { total = this.axWindowsMediaPlayer1.currentMedia.durationString; } catch { }

                    string title = TreeIndex.Model.InFolder ?
                        index + " " + subindex + " [" + curpos + "/" + total + "] " + TreeIndex.Model.Subfolder + " : " + TreeIndex.Model.Name :
                        index + " [" + curpos + "/" + total + "] " + TreeIndex.Model.Name;
                    if (curpos.Length != 0 && total.Length != 0)
                    {
                        this.Text = title;
                    }
                }
                
                #endregion
                #region 刷新鼠标形状

                this.Cursor = cursor;

                #endregion
            });
        }
        private void Form_Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == fastkey_L && notEnough)
            {
                keyL.KeyDown(this.Location, GetScrollW(), GetScrollH()); return;
            }
            if (e.KeyValue == fastkey_R && notEnough)
            {
                keyR.KeyDown(this.Location, GetScrollW(), GetScrollH()); return;
            }
            if (e.KeyValue == fastkey_U && notEnough)
            {
                keyU.KeyDown(this.Location, GetScrollW(), GetScrollH()); return;
            }
            if (e.KeyValue == fastkey_D && notEnough)
            {
                keyD.KeyDown(this.Location, GetScrollW(), GetScrollH()); return;
            }
        }
        private void Form_Main_KeyUp(object sender, KeyEventArgs e)
        {
            #region 上下左右键

            if (e.KeyValue == fastkey_L && notEnough)
            {
                keyL.KeyUp(this.Location, GetScrollW(), GetScrollH()); return;
            }
            if (e.KeyValue == fastkey_R && notEnough)
            {
                keyR.KeyUp(this.Location, GetScrollW(), GetScrollH()); return;
            }
            if (e.KeyValue == fastkey_U && notEnough)
            {
                keyU.KeyUp(this.Location, GetScrollW(), GetScrollH()); return;
            }
            if (e.KeyValue == fastkey_D && notEnough)
            {
                keyD.KeyUp(this.Location, GetScrollW(), GetScrollH()); return;
            }

            #endregion

            #region 上一项

            if (e.KeyValue == fastkey_PageU)
            {
                RightMenu_Previous(null, null); return;
            }

            #endregion

            #region 下一项

            if (e.KeyValue == fastkey_PageD)
            {
                RightMenu_Next(null, null); return;
            }

            #endregion

            #region 左一个

            if (e.KeyValue == fastkey_L)
            {
                Page_L(null, null); return;
            }

            #endregion

            #region 右一个

            if (e.KeyValue == fastkey_R)
            {
                Page_R(null, null); return;
            }

            #endregion

            #region 上一个

            if (e.KeyValue == fastkey_U)
            {
                Page_U(null, null); return;
            }

            #endregion

            #region 下一个

            if (e.KeyValue == fastkey_D)
            {
                Page_D(null, null); return;
            }

            #endregion

            #region 回车

            if (e.KeyValue == fastkey_Enter)
            {
                Form_Main_DoubleClick(null, null);
            }

            #endregion

            #region 删除

            if (e.KeyValue == fastkey_Export)
            {
                RightMenu_Export(0, null);
            }

            #endregion

            #region 打开输出目录

            if (e.KeyValue == fastkey_OpenExport)
            {
                RightMenu_OpenExport(null, null); return;
            }

            #endregion

            #region 打开根目录

            if (e.KeyValue == fastkey_OpenRoot)
            {
                RightMenu_OpenRoot(null, null); return;
            }

            #endregion

            #region 打开漫画目录

            if (e.KeyValue == fastkey_OpenSubroot)
            {
                RightMenu_OpenComic(null, null); return;
            }

            #endregion

            #region 打开当前文件

            if (e.KeyValue == fastkey_OpenFile)
            {
                RightMenu_OpenCurrent(null, null); return;
            }

            #endregion

            #region P PASSWORD

            if (e.KeyValue == fastkey_Password)
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
                if (input.Input == "#hide hide") { FileSupport.SupportHide = false; FileOperate.Reload(); ShowCurrent(); return; }
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

            if (e.KeyValue == fastkey_Esc)
            {
                Application.ExitThread();
            }

            #endregion

            #region A 显示/关闭 窗口外框

            if (e.KeyValue == fastkey_Board)
            {
                ShowBoard(!useBoard); return;
            }

            #endregion

            #region 旋转

            if (e.KeyValue == fastkey_Rotate)
            {
                if (Type != FileType.Picture && Type != FileType.Gif) { return; }
                TreeIndex.Model.Rotate090();
                return;
            }

            #endregion

            #region 翻转 X

            if (e.KeyValue == fastkey_FlipX)
            {
                if (Type != FileType.Picture && Type != FileType.Gif) { return; }
                TreeIndex.Model.FlipX();
                return;
            }

            #endregion

            #region 翻转 Y

            if (e.KeyValue == fastkey_FlipY)
            {
                if (Type != FileType.Picture && Type != FileType.Gif) { return; }
                TreeIndex.Model.FlipY();
                return;
            }

            #endregion
        }
        private void Form_Main_DoubleClick(object sender, EventArgs e)
        {
            #region 播放时的双击操作

            if (play.IsPlaying && Type != FileType.Music && Type != FileType.Video)
            { play.IsPlaying = false; return; }

            #endregion

            #region 判断位置是否摆正

            bool PicWindowIsOK = !NeedCorrectPictureBox();
            bool VidWindowIsOK = !NeedCorrectWMP();

            #endregion

            #region 图片文件的双击操作

            if (Type == FileType.Picture)
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
                if (state == WMPLib.WMPPlayState.wmppsPlaying) { this.axWindowsMediaPlayer1.Ctlcontrols.pause(); }
                return;
            }

            #endregion

            #region 其他文件双击操作

            ShowCurrent();

            #endregion
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

            for (int i = 0; i < this.rootPathToolStripMenuItem.DropDownItems.Count; i++)
            {
                ToolStripMenuItem imenu = (ToolStripMenuItem)this.rootPathToolStripMenuItem.DropDownItems[i];
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
        private void SetScrollGainH(int value)
        {

        }
        private void SetScrollGainW(int value)
        {

        }
        private int GetScrollH()
        {
            if (this.axWindowsMediaPlayer1.Visible) { return this.axWindowsMediaPlayer1.Location.Y; }
            if (this.pictureBox1.Visible) { return this.pictureBox1.Location.Y; }
            return 0;
        }
        private int GetScrollW()
        {
            if (this.axWindowsMediaPlayer1.Visible) { return this.axWindowsMediaPlayer1.Location.X; }
            if (this.pictureBox1.Visible) { return this.pictureBox1.Location.X; }
            return 0;
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
                this.rootPathToolStripMenuItem.DropDownItems.Insert(0, menu);
            }

            // 显示
            if (FileOperate.RootFiles.Count > 0)
            { ((ToolStripMenuItem)this.rootPathToolStripMenuItem.DropDownItems[0]).Checked = true; }
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
                this.rootPathToolStripMenuItem.DropDownItems.Insert(0, menu);
            }

            // 显示
            if (FileOperate.RootFiles.Count > 0)
            { ((ToolStripMenuItem)this.rootPathToolStripMenuItem.DropDownItems[0]).Checked = true; }
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

            foreach(ToolStripMenuItem imenu in this.rootPathToolStripMenuItem.DropDownItems)
            { imenu.Checked = false; }

            ToolStripMenuItem menu = new ToolStripMenuItem(path);
            menu.Checked = true;
            menu.Click += RightMenu_Path;
            this.rootPathToolStripMenuItem.DropDownItems.Add(menu);
            
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
            foreach (ToolStripMenuItem imenu in this.rootPathToolStripMenuItem.DropDownItems)
            { indexofSelect++; if (imenu.Checked) { found = true; break; } }
            if (!found) { return; }

            if (DialogResult.Cancel == MessageBox.Show("把 “" + FileOperate.RootFiles[config.FolderIndex].Path + "” 移出当前浏览？", "确认移出", MessageBoxButtons.OKCancel))
            { return; }

            this.rootPathToolStripMenuItem.DropDownItems.RemoveAt(indexofSelect);
            FileOperate.RootFiles.RemoveAt(indexofSelect);

            indexofSelect--;
            if (indexofSelect < 0) { indexofSelect = 0; }
            if (indexofSelect >= FileOperate.RootFiles.Count) { indexofSelect = FileOperate.RootFiles.Count - 1; }

            if (indexofSelect > 0)
            { ((ToolStripMenuItem)this.rootPathToolStripMenuItem.DropDownItems[indexofSelect]).Checked = true; }

            config.FolderIndex = indexofSelect;
            config.FileIndex = 0;
            ShowCurrent();
        }
        private void RightMenu_Path(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem imenu in this.rootPathToolStripMenuItem.DropDownItems)
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
            this.exportPathToolStripMenuItem.Text = config.ExportFolder;
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

            this.totalToolStripMenuItem.Checked = !this.totalToolStripMenuItem.Checked;
            if (this.totalToolStripMenuItem.Checked)
            {
                this.rootToolStripMenuItem.Checked = false;
                this.subrootToolStripMenuItem.Checked = false;
            }

            play.TotalRoots = this.totalToolStripMenuItem.Checked;
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
                this.totalToolStripMenuItem.Checked = false;
                this.subrootToolStripMenuItem.Checked = false;
            }

            play.TotalRoots = this.totalToolStripMenuItem.Checked;
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
                this.totalToolStripMenuItem.Checked = false;
                this.rootToolStripMenuItem.Checked = false;
            }

            play.TotalRoots = this.totalToolStripMenuItem.Checked;
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
            this.lastingToolStripMenuItem.Text = "时间：" + play.ShowTime.ToString();
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
                this.totalToolStripMenuItem.Checked = false;
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
                this.totalToolStripMenuItem.Checked = false;

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
