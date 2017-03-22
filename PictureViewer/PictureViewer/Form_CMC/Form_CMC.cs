using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;

namespace PictureViewer.Form_CMC
{
    public partial class Form_CMC : Form
    {
        public Form_CMC(string folder)
        {
            InitializeComponent();

            Folder = folder;
            fullpath = new List<string>();
            indexofshow = 0;
            sizes = new List<long>();
            picboxH = this.pictureBox.Height;
            picboxW = this.pictureBox.Width;

            if (!Directory.Exists(folder)) { return; }
            DirectoryInfo dir = new DirectoryInfo(folder);
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo ifile in files)
            {
                fullpath.Add(ifile.FullName);
                sizes.Add(ifile.Length >> 10);
            }

            ShowFile_CMC(0);
        }

        private string Folder;
        private List<string> fullpath;
        private int indexofshow;
        private List<long> sizes;
        private int picboxH;
        private int picboxW;
        private bool doubleClicked = false;

        private void ShowFile_CMC(int index)
        {
            if (fullpath.Count == 0)
            {
                this.GoTo.Text = "0";
                this.total.Text = "Total: 0 / Size: 0";
                this.pictureBox.Image = null;
                return;
            }

            doubleClicked = false;

            // 限幅
            if (index < 0) { index = fullpath.Count - 1; }
            if (index > fullpath.Count - 1) { index = 0; }
            indexofshow = index;

            // 当前编号
            this.total.Text = "Total: " + fullpath.Count.ToString() +
                " / Size: " + sizes[index].ToString();

            // 加载名称
            this.Text = fullpath[index].Substring(Folder.Length + 1);

            // 加载图片
            string ipath = fullpath[index];
            if (!File.Exists(ipath)) { ipath = PVproject.CFG.exe_path + "\\pvc\\unk.pv"; }
            Image picS = Image.FromFile(ipath); ;
            Bitmap picbit = new Bitmap(picS);
            picS.Dispose();

            // 调整图片大小
            double shortW = picbit.Width / (double)picboxW;
            double shortH = picbit.Height / (double)picboxH;

            int reSizeH = 0, reSizeW = 0;
            if (shortW > shortH) { shortH = shortW; }

            reSizeW = (int)(picbit.Width / shortH);
            reSizeH = (int)(picbit.Height / shortH);

            Bitmap showS = ReSizePiture(picbit, reSizeH, reSizeW);

            // 加载图片
            this.pictureBox.Size = new Size(showS.Width + 1, showS.Height + 1);

            this.pictureBox.Image = showS;
            this.pictureBox.Location = new Point((this.Width - reSizeW) / 2, (picboxH - reSizeH) / 2);
            this.GoTo.Text = (index + 1).ToString();
        }
        private Bitmap ReSizePiture(Bitmap sour, int h, int w)
        {
            Bitmap dest = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(dest);

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(sour, new Rectangle(0, 0, w, h), new Rectangle(0, 0, sour.Width, sour.Height), GraphicsUnit.Pixel);

            g.Dispose(); sour.Dispose();
            return dest;
        }

        private void Previous_Click(object sender, EventArgs e)
        {
            ShowFile_CMC(indexofshow - 1);
        }

        private void GoTo_TextChanged(object sender, EventArgs e)
        {
            int go = 0;
            try { go = int.Parse(this.GoTo.Text); } catch { return; }
            if (go - 1 < 0) { return; }
            if (go > fullpath.Count) { go = fullpath.Count; }
            if (go == indexofshow + 1) { return; }
            indexofshow = go - 1;
            ShowFile_CMC(indexofshow);
        }

        private void Next_Click(object sender, EventArgs e)
        {
            ShowFile_CMC(indexofshow + 1);
        }

        private void Form_CMC_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void GoTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 37) { ShowFile_CMC(indexofshow - 1); return; }
            if (e.KeyValue == 39) { ShowFile_CMC(indexofshow + 1); return; }
            if (e.KeyValue == 13)
            {
                if (indexofshow < 0) { return; }
                if (indexofshow > fullpath.Count - 1) { return; }

                string sour = fullpath[indexofshow];
                if (!File.Exists(sour)) { sour = PVproject.CFG.exe_path + "\\unk.pv"; }

                Image pic = Image.FromFile(sour);
                Bitmap picbit = new Bitmap(pic);
                pic.Dispose();
                this.pictureBox.Location = new Point(0, 0);
                this.pictureBox.Size = new Size(picbit.Width, picbit.Height);
                this.pictureBox.Image = picbit; return;
            }
        }

        private void SetTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                string sour = "";
                string dest = "";
                try { dest = fullpath[int.Parse(this.SetTo.Text) - 1]; } catch { MessageBox.Show("Input Error !"); return; }
                try { sour = fullpath[indexofshow]; } catch { return; }

                if (!File.Exists(sour)) { MessageBox.Show("Error in Sour Address !"); return; }
                if (!File.Exists(dest)) { MessageBox.Show("Error in Dest Address !"); return; }

                string TempFile = sour + ".pv";
                FileInfo fileSour = new FileInfo(sour);
                fileSour.MoveTo(TempFile);

                FileInfo fileDest = new FileInfo(dest);
                fileDest.MoveTo(sour);
                fileSour.MoveTo(dest);

                ShowFile_CMC(int.Parse(this.SetTo.Text) - 1);
            }
        }

        private void Form_CMC_ResizeEnd(object sender, EventArgs e)
        {
            
        }

        private void Form_CMC_SizeChanged(object sender, EventArgs e)
        {
            int H = this.Height, W = this.Width, resizedX, resizedY, resizedH, resizedW;

            // panel_pic
            resizedH = H - 66;
            resizedW = W - 40;
            resizedX = 20;
            resizedY = 0;
            this.panel_pic.Location = new Point(resizedX, resizedY);
            this.panel_pic.Size = new Size(resizedW, resizedH);

            // pictureBox
            resizedH = this.panel_pic.Height;
            resizedW = this.panel_pic.Width;
            resizedX = 0;
            resizedY = 0;
            this.pictureBox.Location = new Point(resizedX, resizedY);
            this.pictureBox.Size = new Size(resizedW, resizedH);
            picboxH = resizedH;
            picboxW = resizedW;

            // SetToLabel
            resizedX = 14;
            resizedY = H - 63;
            this.SetToLabel.Location = new Point(resizedX, resizedY + 4);

            // SetTo
            resizedX = 61;
            this.SetTo.Location = new Point(resizedX, resizedY);

            // Total
            resizedX = W / 2 + 5;
            this.total.Location = new Point(resizedX, resizedY + 1);

            // goto
            resizedX = this.total.Location.X - 58;
            this.GoTo.Location = new Point(resizedX, resizedY + 1);

            // previous
            resizedX = this.total.Location.X - 131;
            this.Previous.Location = new Point(resizedX, resizedY);

            // next
            resizedX = this.total.Location.X + 164;
            this.Next.Location = new Point(resizedX, resizedY);

            // 再次显示
            ShowFile_CMC(indexofshow);
        }

        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            doubleClicked = !doubleClicked;
            if (!doubleClicked) { ShowFile_CMC(indexofshow); return; }

            if (indexofshow < 0) { return; }
            if (indexofshow > fullpath.Count - 1) { return; }

            string sour = fullpath[indexofshow];
            if (!File.Exists(sour)) { sour = PVproject.CFG.exe_path + "\\unk.pv"; }

            Image pic = Image.FromFile(sour);
            Bitmap picbit = new Bitmap(pic);
            pic.Dispose();
            this.pictureBox.Location = new Point(0, 0);
            this.pictureBox.Size = new Size(picbit.Width, picbit.Height);
            this.pictureBox.Image = picbit; return;
        }
    }
}
