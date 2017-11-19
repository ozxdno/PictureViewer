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

namespace PictureViewer
{
    public partial class Form_Image : Form
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path;
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name;
        /// <summary>
        /// 初始位置
        /// </summary>
        public Point InitLoc = new Point(0, 0);

        /// <summary>
        /// 打开一个 图片/GIF 文件的副本。location - 打开位置：
        /// 0 - 当前鼠标位置位于图片中间；
        /// 1 - 当前鼠标位置位于图片左上角；
        /// 2 - 自己设定位置；
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="name">文件名称</param>
        /// <param name="rate">缩放比率</param>
        /// <param name="location">打开位置</param>
        public Form_Image(string path, string name, double rate = 80, int location = 0)
        {
            InitializeComponent();

            this.Path = path;
            this.Name = name;

            this.sourpath = path == null ? "" : path;
            this.sourname = name == null ? "" : name;
            this.rate = rate;
            this.location = location;

            this.MouseWheel += Form_Image_MouseWheel;
        }

        private string sourpath;
        private string sourname;
        private string path;
        private string name;
        private string full { get { return path + "\\" + name; } }
        private int type;
        private double rate = 80;
        private bool nextShowBig = false;
        private bool exist;
        private bool error;
        private bool isPic;
        private bool isGif;
        private int location;
        private bool initLoc = true;

        private Image sour = null;
        private Image dest = null;
        private string tip1 = null;
        private string tip2 = null;

        private MOUSE mouse;
        private struct MOUSE
        {
            public int CurrentDown;
            public int CurrentUp;

            public bool Down1;
            public bool Down2;
            public bool Down3;

            public bool Up1;
            public bool Up2;
            public bool Up3;

            public Point pDown1;
            public Point pDown2;
            public Point pDown3;

            public Point pUp1;
            public Point pUp2;
            public Point pUp3;

            public Point pWindow;
            public int xScroll;
            public int yScroll;
        }

        private void Form_Image_Load(object sender, EventArgs e)
        {
            type = Class.FileOperate.getFileType(Class.FileOperate.getExtension(sourname));
            path = sourpath;
            name = sourname;
            if (!File.Exists(full)) { path = Class.FileOperate.getExePath(); name = "err.tip"; }
            exist = File.Exists(full);

            isPic = Class.FileOperate.IsPicture(type);
            isGif = Class.FileOperate.IsGif(type);

            FillTipError();
            if (!error) { sour = Image.FromFile(full); }

            if (location == 0)
            {
                int xinit = MousePosition.X - this.Width / 2;
                int yinit = MousePosition.Y - this.Height / 2;
                this.Location = new Point(xinit, yinit);
            }
            if (location == 1)
            {
                this.Location = MousePosition;
            }
            
            ShowPictureS();

            this.toolTip1.ToolTipTitle = tip1;
            this.toolTip1.SetToolTip(this.pictureBox1, tip2);
            this.Text = tip2;
        }
        private void Form_Image_Close(object sender, FormClosedEventArgs e)
        {
            if (sour != null) { try { sour.Dispose(); } catch { } }
            if (dest != null) { try { dest.Dispose(); } catch { } }
        }
        private void Form_Image_MouseWheel(object sender, MouseEventArgs e)
        {
            if (isGif)
            {
                if (sour == null) { return; }

                // 屏幕参数
                int sh = Screen.PrimaryScreen.Bounds.Height;
                int sw = Screen.PrimaryScreen.Bounds.Width;

                // 当前缩放
                double rate1 = (double)this.Height / sh * 100;
                double rate2 = (double)this.Width / sw * 100;
                double currRate = Math.Max(rate1, rate2);

                // 下一次的显示比例
                if (e.Delta > 0) { rate = currRate + 5; }
                if (e.Delta < 0) { rate = currRate - 5; }
                if (rate <= 10) { rate = 10; }
                if (rate >= 100) { rate = 100; }

                // 显示图片
                ShowPictureS();
            }
            else
            {
                if (this.HorizontalScroll.Visible || this.VerticalScroll.Visible) { return; }
                if (sour == null) { return; }

                // 屏幕参数
                int sh = Screen.PrimaryScreen.Bounds.Height;
                int sw = Screen.PrimaryScreen.Bounds.Width;

                // 当前缩放
                double rate1 = (double)this.pictureBox1.Height / sh * 100;
                double rate2 = (double)this.pictureBox1.Width / sw * 100;
                double currRate = Math.Max(rate1, rate2);

                // 下一次的显示比例
                if (e.Delta > 0) { rate = currRate + 5; }
                if (e.Delta < 0) { rate = currRate - 5; }
                if (rate <= 10) { rate = 10; }
                if (rate >= 100) { rate = 100; }

                // 显示图片
                ShowPictureR();
            }
        }
        private void Form_Image_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !mouse.Down2)
            {
                mouse.CurrentDown = 1;
                mouse.Down1 = true;
                mouse.Up1 = false;
                mouse.pDown1 = MousePosition;
                mouse.pWindow = this.Location;
                mouse.xScroll = this.HorizontalScroll.Value;
                mouse.yScroll = this.VerticalScroll.Value;
            }
            if (e.Button == MouseButtons.Right && !mouse.Down1)
            {
                mouse.CurrentDown = 2;
                mouse.Down2 = true;
                mouse.Up2 = false;
                mouse.pDown2 = MousePosition;
                mouse.pWindow = this.Location;
            }
        }
        private void Form_Image_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouse.CurrentUp = 1;
                mouse.Down1 = false;
                mouse.Up1 = true;
                mouse.pUp1 = MousePosition;
            }
            if (e.Button == MouseButtons.Right)
            {
                mouse.CurrentUp = 2;
                mouse.Down2 = false;
                mouse.Up2 = true;
                mouse.pUp2 = MousePosition;
            }
        }
        private void Form_Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouse.Down1 && !this.VerticalScroll.Visible && !this.HorizontalScroll.Visible)
            {
                int xmove = MousePosition.X - mouse.pDown1.X;
                int ymove = MousePosition.Y - mouse.pDown1.Y;
                this.Location = new Point(mouse.pWindow.X + xmove, mouse.pWindow.Y + ymove);
            }
            if (mouse.Down1 && (this.VerticalScroll.Visible || this.HorizontalScroll.Visible))
            {
                int xmove = MousePosition.X - mouse.pDown1.X;
                int ymove = MousePosition.Y - mouse.pDown1.Y;
                SetScrollW(mouse.xScroll - xmove);
                SetScrollH(mouse.yScroll - ymove);
            }
            if (mouse.Down2 && (this.VerticalScroll.Visible || this.HorizontalScroll.Visible))
            {
                int xmove = MousePosition.X - mouse.pDown2.X;
                int ymove = MousePosition.Y - mouse.pDown2.Y;
                this.Location = new Point(mouse.pWindow.X + xmove, mouse.pWindow.Y + ymove);
            }
        }
        private void Form_Image_DoubleClicked(object sender, EventArgs e)
        {
            if (mouse.CurrentDown != 1) { return; }
            if (sour == null) { return; }
            if (isGif) { return; }

            int ph = this.Height;
            int pw = this.Width;
            if (ph >= sour.Height && pw >= sour.Width) { ShowPictureS(); return; }

            nextShowBig = !nextShowBig;
            if (nextShowBig) { ShowPictureB(); } else { ShowPictureS(); }
        }
        private void Form_Image_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == Class.Load.settings.FastKey_Image_Esc)
            {
                this.Close();
                return;
            }
            if (e.KeyValue == Class.Load.settings.FastKey_Image_Rotate)
            {
                if (this.HorizontalScroll.Visible || this.VerticalScroll.Visible) { return; }
                sour.RotateFlip(RotateFlipType.Rotate90FlipNone);
                ShowPictureS();
                return;
            }
            if (e.KeyValue == Class.Load.settings.FastKey_Image_FlipX)
            {
                if (this.HorizontalScroll.Visible || this.VerticalScroll.Visible) { return; }
                sour.RotateFlip(RotateFlipType.RotateNoneFlipX);
                ShowPictureS();
                return;
            }
            if (e.KeyValue == Class.Load.settings.FastKey_Image_FlipY)
            {
                if (this.HorizontalScroll.Visible || this.VerticalScroll.Visible) { return; }
                sour.RotateFlip(RotateFlipType.RotateNoneFlipY);
                ShowPictureS();
                return;
            }
            if (e.KeyValue == Class.Load.settings.FastKey_Image_Enter)
            {
                Form_Image_DoubleClicked(null, null);
                return;
            }
        }

        private void ShowPictureB()
        {
            if (sour == null || dest == null) { return; }
            int xfocus = this.pictureBox1.PointToClient(MousePosition).X;
            int yfocus = this.pictureBox1.PointToClient(MousePosition).Y;

            this.pictureBox1.Height = sour.Height;
            this.pictureBox1.Width = sour.Width;
            this.pictureBox1.Image = sour;

            double xrate = (double)xfocus / dest.Width;
            if (xrate < 0) { xrate = 0; }
            if (xrate > 1) { xrate = 1; }
            double yrate = (double)yfocus / dest.Height;
            if (yrate < 0) { yrate = 0; }
            if (yrate > 1) { yrate = 1; }
            
            int xS = (int)(sour.Width * xrate - this.Width / 2);
            if (xS < this.HorizontalScroll.Minimum) { xS = this.HorizontalScroll.Minimum; }
            if (xS > this.HorizontalScroll.Maximum) { xS = this.HorizontalScroll.Maximum; }
            
            int yS = (int)(sour.Height * yrate - this.Height / 2);
            if (yS < this.VerticalScroll.Minimum) { yS = this.VerticalScroll.Minimum; }
            if (yS > this.VerticalScroll.Maximum) { yS = this.VerticalScroll.Maximum; }

            SetScrollW(xS); SetScrollH(yS);
        }
        private void ShowPictureS()
        {
            if (sour == null) { return; }

            #region 窗体大小
            
            int sh = (int)(Screen.PrimaryScreen.Bounds.Height * rate / 100);
            int sw = (int)(Screen.PrimaryScreen.Bounds.Width * rate / 100);
            int centerw = this.Location.X + this.Width / 2;
            int centerh = this.Location.Y + this.Height / 2;

            double h2h = (double)sh / sour.Height;
            double w2w = (double)sw / sour.Width;
            double x2x = Math.Min(h2h, w2w); if (x2x > 1) { x2x = 1; }
            int ph = (int)(sour.Height * x2x);
            int pw = (int)(sour.Width * x2x);
            
            this.Height = ph; this.Width = pw;
            if (initLoc && location == 1) { initLoc = false; }
            else if (initLoc && location == 2) { initLoc = false; this.Location = InitLoc; }
            else
            {
                this.Location = new Point(centerw - pw / 2, centerh - ph / 2);
            }
            this.pictureBox1.Height = isGif ? sour.Height : ph;
            this.pictureBox1.Width = isGif ? sour.Width : pw;

            #endregion

            #region 填充图片

            if (isGif) { this.pictureBox1.Image = sour; return; }

            try { dest.Dispose(); } catch { }
            this.pictureBox1.BackgroundImage = null;

            dest = new Bitmap(pw, ph);
            Graphics g = Graphics.FromImage(dest);
            g.DrawImage(sour, new Rectangle(0, 0, pw, ph), new Rectangle(0, 0, sour.Width, sour.Height), GraphicsUnit.Pixel);
            g.Dispose();

            this.pictureBox1.Image = dest;

            #endregion
        }
        private void ShowPictureR()
        {
            if (sour == null) { return; }

            #region 窗体大小

            int sh = (int)(Screen.PrimaryScreen.Bounds.Height * rate / 100);
            int sw = (int)(Screen.PrimaryScreen.Bounds.Width * rate / 100);
            int centerw = this.Location.X + this.Width / 2;
            int centerh = this.Location.Y + this.Height / 2;

            double h2h = (double)sh / sour.Height;
            double w2w = (double)sw / sour.Width;
            double x2x = Math.Min(h2h, w2w);

            int ph = (int)(sour.Height * x2x);
            int pw = (int)(sour.Width * x2x);

            this.Height = ph; this.Width = pw;
            this.Location = new Point(centerw - pw / 2, centerh - ph / 2);
            this.pictureBox1.Height = ph;
            this.pictureBox1.Width = pw;

            #endregion

            #region 填充图片

            try { dest.Dispose(); } catch { }
            this.pictureBox1.BackgroundImage = null;

            dest = new Bitmap(pw, ph);
            Graphics g = Graphics.FromImage(dest);
            g.DrawImage(sour, new Rectangle(0, 0, pw, ph), new Rectangle(0, 0, sour.Width, sour.Height), GraphicsUnit.Pixel);
            g.Dispose();

            this.pictureBox1.Image = dest;

            #endregion
        }

        private void FillTipError()
        {
            if (!Class.FileOperate.IsPicture(type) && !Class.FileOperate.IsGif(type))
            {
                tip1 = "[Error]: Select is not a Picture or Gif File";
                tip2 = name + " : " + path;
                error = true;
                return;
            }

            if (!File.Exists(sourpath + "\\" + sourname))
            {
                tip1 = "[Error]: Not Exist Such File !";
                tip2 = name + " => " + path;
                error = true;
                return;
            }

            tip1 = path;
            tip2 = name;
            error = false;
        }
        private void LoadPicture()
        {
            try { sour.Dispose(); } catch { }
            try { dest.Dispose(); } catch { }
            this.pictureBox1.Image = null;

            if (!File.Exists(full)) { return; }
            sour = Image.FromFile(full);
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
    }
}
