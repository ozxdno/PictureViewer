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

namespace PictureViewer.Form_View
{
    public partial class Form_View : Form
    {
        public Form_View(Form_Start start)
        {
            InitializeComponent();

            this.form_start = start;
            widthS = this.pic_S.Width;
            heightS = this.pic_S.Height;
            widthB = this.pic_B.Width;
            heightB = this.pic_B.Height;

            if (!PVproject.CFG.FirstTime) { this.Panel_B.BackgroundImage = null; }
            if (PVproject.CFG.FirstTime) { this.pic_B.Visible = false; }

            GetShowFileB_toShow(PVproject.CFG.viewIndex);
            GetShowFileS_toShow(0);
        }
        
        private Form_Start form_start = null;
        private int widthS;
        private int heightS;
        private int widthB;
        private int heightB;
        private List<string> TempCopyTags = new List<string>();
        private bool doubleClicked = false;

        private void GetShowFileS_toShow(int index)
        {
            if (PVproject.showIN.indexofbase.Count == 0)
            {
                this.gotoS.Text = "0";
                this.totalS.Text = "Total: 0 / Size: 0";
                this.pic_S.Image = null;
                return;
            }

            if (index > PVproject.showIN.indexofbase.Count - 1) { index = 0; }
            if (index < 0) { index = PVproject.showIN.indexofbase.Count - 1; }

            PVproject.showIN.indexofshow = index;
            index = PVproject.showIN.indexofbase[index];
            PVproject.showIN.filepath = PVproject.CFG.i_path;
            PVproject.showIN.filename = PVproject.IN[index].name;
            PVproject.showIN.size = PVproject.IN[index].size;
            PVproject.showIN.tags = PVproject.IN[index].tags;
            PVproject.showIN.type = PVproject.IN[index].type;
            PVproject.showIN.extension = PVproject.IN[index].extension;
            PVproject.showIN.process = PVproject.GetProcess(PVproject.showIN.type);

            ShowFileS(PVproject.showIN);
        }
        private void GetShowFileB_toShow(int index)
        {
            if (PVproject.showDB.indexofbase.Count == 0)
            {
                this.gotoB.Text = "0";
                this.totalB.Text = "Total: 0 / Size: 0";
                this.pic_B.Image = null;
                return;
            }

            if (index > PVproject.showDB.indexofbase.Count - 1) { index = 0; }
            if (index < 0) { index = PVproject.showDB.indexofbase.Count - 1; }

            PVproject.showDB.indexofshow = index;
            index = PVproject.showDB.indexofbase[index];
            PVproject.CFG.viewIndex = index;
            
            PVproject.showDB.filepath = PVproject.CFG.o_path;
            PVproject.showDB.filename = PVproject.DB[index].name;
            PVproject.showDB.size = PVproject.DB[index].size;
            PVproject.showDB.tags = PVproject.DB[index].tags;
            PVproject.showDB.type = PVproject.DB[index].type;
            PVproject.showDB.extension = PVproject.DB[index].extension;
            PVproject.showDB.process = PVproject.GetProcess(PVproject.showDB.type);

            ShowFileB(PVproject.showDB);
        }
        private void ShowFileS(PVproject.SHOWFILE ShowFile)
        {
            this.totalS.Text = "Total: " + ShowFile.indexofbase.Count.ToString() +
                " / Size: " + ShowFile.size.ToString();

            string fullpath = ShowFile.filepath + "\\" + ShowFile.filename + ShowFile.extension;
            if (ShowFile.type.Equals("CMC")) { fullpath = PVproject.CFG.exe_path + "\\pvc\\cmc.pv"; }
            if (ShowFile.type.Equals("MOV")) { fullpath = PVproject.CFG.exe_path + "\\pvc\\mov.pv"; }

            // 不存在文件
            bool NotExist = !File.Exists(fullpath);
            if (NotExist && ShowFile.type.Equals("PIC")) { fullpath = PVproject.CFG.exe_path + "\\pvc\\up.pv"; }
            if (NotExist && ShowFile.type.Equals("GIF")) { fullpath = PVproject.CFG.exe_path + "\\pvc\\ug.pv"; }
            if (NotExist && ShowFile.type.Equals("CMC")) { fullpath = PVproject.CFG.exe_path + "\\pvc\\uc.pv"; }
            if (NotExist && ShowFile.type.Equals("MOV")) { fullpath = PVproject.CFG.exe_path + "\\pvc\\um.pv"; }

            Image picS = Image.FromFile(fullpath);
            Bitmap picbit = new Bitmap(picS);
            picS.Dispose();
            
            double shortW = picbit.Width / (double)widthS;
            double shortH = picbit.Height / (double)heightS;

            int reSizeH = 0, reSizeW = 0;
            if (shortW > shortH) { shortH = shortW; }

            reSizeW = (int)(picbit.Width / shortH);
            reSizeH = (int)(picbit.Height / shortH);

            Bitmap showS = ReSizePiture(picbit, reSizeH, reSizeW);
            this.pic_S.Size = new Size(showS.Width + 1, showS.Height + 1);
            this.pic_S.Image = showS;
            this.pic_S.Location = new Point((this.Panel_S.Width - reSizeW) / 2, (heightS - reSizeH) / 2 + 3);
            this.gotoS.Text = (ShowFile.indexofshow + 1).ToString();
        }
        private void ShowFileB(PVproject.SHOWFILE ShowFile)
        {
            this.totalB.Text = "Total: " + ShowFile.indexofbase.Count.ToString() +
                " / Size: " + ShowFile.size.ToString(); ;

            doubleClicked = false;
            this.Text = ShowFile.filename + PVproject.GetExtension(ShowFile.type);

            // 获取文件
            string fullpath = ShowFile.filepath + "\\" + ShowFile.filename + PVproject.GetExtension(ShowFile.type);
            string coverpath = PVproject.CFG.exe_path + "\\pv0\\" + ShowFile.filename + ".cov";
            if (ShowFile.type.Equals("CMC")) { fullpath = PVproject.GetCMCpic1st(ShowFile.filepath + "\\" + ShowFile.filename); }
            if (ShowFile.type.Equals("MOV")) { fullpath = coverpath; }

            // 不存在文件
            bool NotExist = !File.Exists(fullpath);
            if (NotExist && ShowFile.type.Equals("PIC")) { fullpath = PVproject.CFG.exe_path + "\\pvc\\up.pv"; }
            if (NotExist && ShowFile.type.Equals("GIF")) { fullpath = PVproject.CFG.exe_path + "\\pvc\\ug.pv"; }
            if (NotExist && ShowFile.type.Equals("CMC")) { fullpath = PVproject.CFG.exe_path + "\\pvc\\uc.pv"; }
            if (NotExist && ShowFile.type.Equals("MOV")) { fullpath = PVproject.CFG.exe_path + "\\pvc\\um.pv"; }

            // 加载图片
            Image picB = Image.FromFile(fullpath);
            Bitmap picbit = new Bitmap(picB);
            picB.Dispose();

            // 显示现在有的标签
            form_start.listbox_tagstr.Items.Clear();
            PVproject.tag.SelectedTags.Clear();
            foreach (string itag in ShowFile.tags)
            {
                form_start.listbox_tagstr.Items.Add(itag);
                PVproject.tag.SelectedTags.Add(itag);
            }
            
            // 显示图片
            double shortW = picbit.Width / (double)widthB;
            double shortH = picbit.Height / (double)heightB;

            if (shortW > shortH) { shortH = shortW; }

            int reSizeW = (int)(picbit.Width / shortH);
            int reSizeH = (int)(picbit.Height / shortH);

            Bitmap showB = ReSizePiture(picbit, reSizeH, reSizeW);
            this.pic_B.Size = new Size(showB.Width + 1, showB.Height + 1);
            this.pic_B.Image = showB;
            this.pic_B.Location = new Point((this.Panel_B.Width - reSizeW) / 2, (heightB - reSizeH) / 2);
            
            this.gotoB.Text = (ShowFile.indexofshow + 1).ToString();
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
        private void SearchIndex(List<int> index)
        {
            PVproject.showDB.indexofbase.Clear();
            bool found = false;
            List<int> Correct = new List<int>();

            for (int i = 0; i < index.Count; i++)
            {
                found = true;
                foreach (string reqtag in PVproject.tag.SelectedTags)
                {
                    found = false;
                    foreach (string itag in PVproject.DB[index[i]].tags)
                    {
                        if (itag.Equals(reqtag)) { found = true; break; }
                    }
                    if (!found) { break; }
                }
                if (found) { Correct.Add(i); }
            }

            PVproject.showDB.indexofbase = Correct;
            GetShowFileB_toShow(PVproject.showDB.indexofshow);
        }
        
        private void Form_View_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form_Start.OpenViewer = false;
        }

        private void PreviousS_Click(object sender, EventArgs e)
        {
            GetShowFileS_toShow(PVproject.showIN.indexofshow - 1);
        }

        private void OriginalS_Click(object sender, EventArgs e)
        {
            if (PVproject.showIN.indexofshow < 0) { return; }
            if (PVproject.showIN.indexofshow > PVproject.showIN.indexofbase.Count - 1) { return; }

            int index = PVproject.showIN.indexofbase[PVproject.showIN.indexofshow];
            if (index < 0) { return; }
            if (index > PVproject.IN.Count - 1) { return; }

            PVproject.OpenFile_S();
        }

        private void OriginalB_Click(object sender, EventArgs e)
        {
            if (PVproject.showDB.indexofshow < 0) { return; }
            if (PVproject.showDB.indexofshow > PVproject.showDB.indexofbase.Count - 1) { return; }

            int index = PVproject.showDB.indexofbase[PVproject.showDB.indexofshow];
            if (index < 0) { return; }
            if (index > PVproject.DB.Count - 1) { return; }

            if (PVproject.showDB.type.Equals("PIC"))
            {
                string sour = PVproject.showDB.filepath + "\\" + PVproject.showDB.filename + ".pv1";
                if (!File.Exists(sour)) { sour = PVproject.CFG.exe_path + "\\unk.pv"; }

                Image pic = Image.FromFile(sour);
                Bitmap picbit = new Bitmap(pic);
                pic.Dispose();
                this.pic_B.Location = new Point(3, 0);
                this.pic_B.Size = new Size(picbit.Width, picbit.Height);
                this.pic_B.Image = picbit; return;
            }
            PVproject.OpenFile_B();
        }

        private void ToDataBase_Click(object sender, EventArgs e)
        {
            PVproject.MoveFile_S2B();

            GetShowFileS_toShow(PVproject.showIN.indexofshow);
            GetShowFileB_toShow(PVproject.showDB.indexofshow);
        }

        private void PreviousB_Click(object sender, EventArgs e)
        {
            GetShowFileB_toShow(PVproject.showDB.indexofshow - 1);
        }

        private void SearchDB_Click(object sender, EventArgs e)
        {
            List<int> searchIndex = new List<int>();
            for (int i = 0; i < PVproject.DB.Count; i++) { searchIndex.Add(i); }
            SearchIndex(searchIndex);
        }

        private void SearchB_Click(object sender, EventArgs e)
        {
            List<int> searchIndex = new List<int>();
            foreach (int index in PVproject.showDB.indexofbase) { searchIndex.Add(index); }
            SearchIndex(searchIndex);
        }

        private void gotoS_TextChanged(object sender, EventArgs e)
        {
            int go = 0;
            try { go = int.Parse(this.gotoS.Text); } catch { return; }
            if (go - 1 < 0) { return; }
            if (go > PVproject.showIN.indexofbase.Count) { this.gotoS.Text = PVproject.showIN.indexofbase.Count.ToString(); }
            if (go == PVproject.showIN.indexofshow + 1) { return; }
            GetShowFileS_toShow(go - 1);
        }

        private void gotoB_TextChanged(object sender, EventArgs e)
        {
            int go = 0;
            try { go = int.Parse(this.gotoB.Text); } catch { return; }
            if (go - 1 < 0) { return; }
            if (go > PVproject.showDB.indexofbase.Count) { this.gotoB.Text = PVproject.showDB.indexofbase.Count.ToString(); return; }
            if (go == PVproject.showDB.indexofshow + 1) { return; }
            GetShowFileB_toShow(go - 1);
        }

        private void NextS_Click(object sender, EventArgs e)
        {
            GetShowFileS_toShow(PVproject.showIN.indexofshow + 1);
        }

        private void NextB_Click(object sender, EventArgs e)
        {
            GetShowFileB_toShow(PVproject.showDB.indexofshow + 1);
        }

        private void RemoveS_Click(object sender, EventArgs e)
        {
            PVproject.RemoveFile_S();
            GetShowFileS_toShow(PVproject.showIN.indexofshow);
        }

        private void RemoveB_Click(object sender, EventArgs e)
        {
            PVproject.RemoveFile_B();
            GetShowFileB_toShow(PVproject.showDB.indexofshow);
        }

        private void DeleteB_Click(object sender, EventArgs e)
        {
            PVproject.DeleteFile_B();
            GetShowFileB_toShow(PVproject.showDB.indexofshow);
        }

        private void Replace_Click(object sender, EventArgs e)
        {
            PVproject.ReplaceFile_S2B();
            GetShowFileB_toShow(PVproject.showDB.indexofshow);
        }

        private void Form_View_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void ToDesk_Click(object sender, EventArgs e)
        {
            // 基本条件
            if (PVproject.showDB.indexofshow < 0) { return; }
            if (PVproject.showDB.indexofshow > PVproject.showDB.indexofbase.Count - 1) { return; }

            int indexB = PVproject.showDB.indexofbase[PVproject.showDB.indexofshow];
            if (indexB < 0) { return; }
            if (indexB > PVproject.DB.Count - 1) { return; }

            // 移动 PIC/MOV/GIF 文件
            if (PVproject.showDB.type.Equals("PIC") || PVproject.showDB.type.Equals("GIF") || PVproject.showDB.type.Equals("MOV"))
            {
                // 原文件
                string sourpath = PVproject.showDB.filepath + "\\" + PVproject.showDB.filename +
                PVproject.GetExtension(PVproject.showDB.type);
                if (!File.Exists(sourpath)) { MessageBox.Show("source File Not Exists !"); return; }

                // 目标地址
                string destpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" +
                    PVproject.showDB.filename + PVproject.showDB.extension;
                if (File.Exists(destpath))
                {
                    DialogResult dr = MessageBox.Show
                            ("Do you want to Replace source File ?", "Q", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (dr == DialogResult.Cancel) { return; }
                }

                // 移动文件
                FileInfo file = new FileInfo(sourpath);
                file.CopyTo(destpath, true);
            }

            // 移动 CMC 文件    
            if (PVproject.showDB.type.Equals("CMC"))
            {
                // 原路径
                string sourpath = PVproject.showDB.filepath + "\\" + PVproject.showDB.filename;
                if (!Directory.Exists(sourpath)) { MessageBox.Show("source Folder Not Exists !"); return; }

                // 目标路径
                string destpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" +
                    PVproject.showDB.filename;
                if (Directory.Exists(destpath))
                {
                    DialogResult dr = MessageBox.Show
                            ("Do you want to Replace source Folder ?", "Q", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (dr == DialogResult.Cancel) { return; }

                    DirectoryInfo folder = new DirectoryInfo(destpath);
                    folder.Delete(true);
                }
                Directory.CreateDirectory(destpath);

                // 把所有文件复制过去
                DirectoryInfo sour = new DirectoryInfo(sourpath);
                FileInfo[] files = sour.GetFiles();
                string extension = PVproject.showDB.extension;

                foreach (FileInfo ifile in files)
                {
                    string filename = ifile.Name.Substring(0, ifile.Name.Length - ifile.Extension.Length);
                    ifile.CopyTo(destpath + "\\" + filename + extension);
                }
            }

            // 给出提示
            MessageBox.Show("Export Finished !");
        }

        private void ExchangeIndex_TextChanged(object sender, EventArgs e)
        {
        }

        private void totalB_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void ExchangeIndex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                // 目标编号可用
                int indexD = 0;
                try { indexD = int.Parse(this.ExchangeIndex.Text); } catch { return; }
                if (indexD < 1) { return; }
                if (indexD > PVproject.DB.Count) { indexD = PVproject.DB.Count; }
                indexD--;

                // 当前编号可用
                if (PVproject.showDB.indexofshow < 0) { return; }
                if (PVproject.showDB.indexofshow > PVproject.showDB.indexofbase.Count - 1) { return; }

                int indexS = PVproject.showDB.indexofbase[PVproject.showDB.indexofshow];
                if (indexS < 0) { return; }
                if (indexS > PVproject.DB.Count - 1) { return; }

                // 替换
                if (indexS == indexD) { return; }
                PVproject.DATABASE db = new PVproject.DATABASE();
                db = PVproject.DB[indexD];
                PVproject.DB[indexD] = PVproject.DB[indexS];
                PVproject.DB[indexS] = db;

                // 刷新显示
                GetShowFileB_toShow(indexD);
                return;
            }
        }

        private void gotoB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 37) { GetShowFileB_toShow(PVproject.showDB.indexofshow - 1); return; }
            if (e.KeyValue == 39) { GetShowFileB_toShow(PVproject.showDB.indexofshow + 1); return; }
            if (e.KeyValue == 13) { OriginalB_Click(sender, e); return; }
            if (e.KeyValue == 65)
            {
                //System.Threading.Thread addfiles = new System.Threading.Thread(GetAllFilesFromFolder);
                //addfiles.Start();
                //return;

                while (PVproject.showIN.indexofbase.Count != 0)
                {
                    PVproject.showIN.indexofshow = 0;
                    PVproject.showIN.filepath = PVproject.CFG.i_path;
                    PVproject.showIN.filename = PVproject.IN[PVproject.showIN.indexofbase[0]].name;
                    PVproject.showIN.type = PVproject.IN[PVproject.showIN.indexofbase[0]].type;
                    PVproject.showIN.size = PVproject.IN[PVproject.showIN.indexofbase[0]].size;
                    PVproject.showIN.extension = PVproject.IN[PVproject.showIN.indexofbase[0]].extension;
                    PVproject.showIN.tags = new List<string>();
                    foreach (string itag in PVproject.IN[PVproject.showIN.indexofbase[0]].tags) { PVproject.showIN.tags.Add(itag); }
                    PVproject.MoveFile_S2B();
                }

                GetShowFileS_toShow(PVproject.showIN.indexofshow);
                GetShowFileB_toShow(PVproject.showDB.indexofshow);
                MessageBox.Show("All Change Finished !"); return;
            }
        }

        private void Export1_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show
                            ("Do you want to Export this File ?", "Q", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Cancel) { return; }

            PVproject.MoveFile_B2E();
            GetShowFileB_toShow(PVproject.showDB.indexofshow);
            //MessageBox.Show("Export Finished !");
        }

        private void Export2_Click(object sender, EventArgs e)
        {
            // 弹出确认提示框
            DialogResult dr = MessageBox.Show
                            ("Do you want to Export all Files ?", "Q", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Cancel) { return; }

            // 移动文件
            while (PVproject.showDB.indexofbase.Count != 0)
            {
                PVproject.showDB.indexofshow = 0;
                PVproject.showDB.filepath = PVproject.CFG.o_path;
                PVproject.showDB.filename = PVproject.DB[PVproject.showDB.indexofbase[0]].name;
                PVproject.showDB.type = PVproject.DB[PVproject.showDB.indexofbase[0]].type;
                PVproject.showDB.extension = PVproject.DB[PVproject.showDB.indexofbase[0]].extension;
                PVproject.MoveFile_B2E();
            }

            // 显示结果
            GetShowFileB_toShow(PVproject.showDB.indexofshow);
            MessageBox.Show("All Files Exported !");
        }

        private void CopyTags_Click(object sender, EventArgs e)
        {
            this.PasteTags.BackColor = Color.LightGray;
            this.CopyTags.BackColor = Color.LightBlue;

            TempCopyTags = new List<string>();
            foreach (string itag in PVproject.tag.SelectedTags) { TempCopyTags.Add(itag); }
        }

        private void PasteTags_Click(object sender, EventArgs e)
        {
            this.CopyTags.BackColor = Color.LightGray;
            this.PasteTags.BackColor = Color.LightBlue;

            form_start.listbox_tagstr.Items.Clear();
            foreach (string itag in TempCopyTags) { form_start.listbox_tagstr.Items.Add(itag); }

            PVproject.tag.SelectedTags.Clear();
            foreach (string itag in TempCopyTags) { PVproject.tag.SelectedTags.Add(itag); }
        }

        private void pic_B_DoubleClick(object sender, EventArgs e)
        {
            bool IsPIC = PVproject.showDB.type.Equals("PIC");
            if (IsPIC) { doubleClicked = !doubleClicked; }
            if (!doubleClicked && IsPIC) { GetShowFileB_toShow(PVproject.showDB.indexofshow); return; }

            OriginalB_Click(sender, e);
        }
    }
}
