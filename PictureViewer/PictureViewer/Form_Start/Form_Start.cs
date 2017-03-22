using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PictureViewer
{
    public partial class Form_Start : Form
    {
        public Form_Start()
        {
            // 初始化窗口
            InitializeComponent();
            this.Location = new Point(120, 62);
            if (!PVproject.CFG.FirstTime) { this.Welcom.Visible = false; }
            
            // 读取文件
            try { Method.ReadFile.ReadFile_DB(); } catch { PVproject.srClose(); }

            // 更新窗口
            this.listbox_list.Items.Clear();
            for (int i = 0; i < PVproject.tag.list.Count; i++) { this.listbox_list.Items.Add(PVproject.tag.list[i].name); }
            if (PVproject.tag.list.Count > 0)
            {
                this.listbox_list.SelectedIndex = 0;
                this.listbox_tag.Items.Clear();
                foreach (string itag in PVproject.tag.list[0].tags) { this.listbox_tag.Items.Add(itag); }
            }
        }

        private bool PermitChangeTagExp = false;
        public static bool OpenViewer = false;

        private void CFG_Click(object sender, EventArgs e)
        {
            if (PVproject.CFG.FirstTime) { return; }

            this.CFG.BackColor = Color.LightBlue;
            int x = this.Location.X + 12;
            int y = this.Location.Y + 120;

            Form_Config.Form_Config config = new Form_Config.Form_Config();
            config.Location = new Point(x, y);
            config.ShowDialog();

            this.CFG.BackColor = this.DEL.BackColor;
        }

        private void FormStart_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { Method.OutputFile.OutputFile_CFG(); } catch { PVproject.swClose(); MessageBox.Show("Output CFG error !"); }
            try { Method.OutputFile.OutputFile_DB(); } catch { PVproject.swClose(); MessageBox.Show("Output DB error !"); }
        }

        private void listbox_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            PermitChangeTagExp = false;

            int index = this.listbox_list.SelectedIndex;
            if (index == -1) { return; }
            
            this.listbox_tag.Items.Clear();
            foreach (string itag in PVproject.tag.list[index].tags) { this.listbox_tag.Items.Add(itag); }

            if (PVproject.tag.list[index].tags.Count <= 0) { return; }

            this.listbox_tag.SelectedIndex = 0;
        }

        private void listbox_tag_SelectedIndexChanged(object sender, EventArgs e)
        {
            PermitChangeTagExp = false;

            int iT = this.listbox_tag.SelectedIndex;
            if (iT == -1) { return; }

            int iL = this.listbox_list.SelectedIndex;
            if (iL == -1) { return; }

            string TagStr = this.listbox_list.Text + ":" + this.listbox_tag.Text;
            for (int i = 0; i < PVproject.tag.TagStrs.Count; i++)
            {
                if (TagStr.Equals(PVproject.tag.TagStrs[i]))
                {
                    this.TagExp.Text = PVproject.tag.TagExps[i]; break;
                }
            }
        }

        private void listbox_tagstr_SelectedIndexChanged(object sender, EventArgs e)
        {
            PermitChangeTagExp = true;

            int index = this.listbox_tagstr.SelectedIndex;
            if (index == -1) { return; }

            string TagStr = this.listbox_tagstr.Text;

            for (int i = 0; i < PVproject.tag.TagStrs.Count; i++)
            {
                if (TagStr.Equals(PVproject.tag.TagStrs[i]))
                {
                    this.TagExp.Text = PVproject.tag.TagExps[i]; break;
                }
            }
        }

        private void TagExp_TextChanged(object sender, EventArgs e)
        {
            if (!PermitChangeTagExp) { return; }
            if (this.listbox_tagstr.SelectedIndex == -1) { return; }

            string tagexp = this.TagExp.Text;
            if (tagexp.Length == 0 || tagexp.Last() != '\n') { return; }
            tagexp = tagexp.Remove(tagexp.Length - 1);

            string tagstr = this.listbox_tagstr.Text;
            for (int i = 0; i < PVproject.tag.TagStrs.Count; i++)
            {
                if (!tagstr.Equals(PVproject.tag.TagStrs[i])) { continue; }
                PVproject.tag.TagExps[i] = tagexp; break;
            }

            this.TagExp.Text = tagexp;
        }

        private void DEL_Click(object sender, EventArgs e)
        {
            if (PVproject.CFG.FirstTime) { return; }

            int index = this.listbox_tagstr.SelectedIndex;
            if (index == -1) { return; }

            PVproject.tag.SelectedTags.RemoveAt(index);
            this.listbox_tagstr.Items.RemoveAt(index);
            this.listbox_tagstr.SelectedIndex = index - 1;
        }

        private void ADD_Click(object sender, EventArgs e)
        {
            if (PVproject.CFG.FirstTime) { return; }

            int iT = this.listbox_tag.SelectedIndex;
            if (iT == -1) { return; }

            int iL = this.listbox_list.SelectedIndex;
            if (iL == -1) { return; }

            string tagstr = this.listbox_list.Text + ":" + this.listbox_tag.Text;
            foreach (string itag in PVproject.tag.SelectedTags) { if (itag.Equals(tagstr)) { return; } }
            PVproject.tag.SelectedTags.Add(tagstr);
            this.listbox_tagstr.Items.Add(tagstr);
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (PVproject.CFG.FirstTime) { return; }

            if (PVproject.showDB.indexofbase.Count == 0) { return; }
            if (PVproject.showDB.indexofshow > PVproject.showDB.indexofbase.Count - 1) { return; }
            int index = PVproject.showDB.indexofbase[PVproject.showDB.indexofshow];

            if (PVproject.DB.Count == 0) { return; }
            if (index > PVproject.DB.Count - 1) { return; }
            if (index < 0) { return; }

            PVproject.DATABASE db = PVproject.DB[index];
            db.tags.Clear();
            foreach (string itag in PVproject.tag.SelectedTags) { db.tags.Add(itag); }
            PVproject.DB[index] = db;

            MessageBox.Show("OK !");
        }

        private void VIEW_Click(object sender, EventArgs e)
        {
            if (PVproject.CFG.FirstTime) { return; }
            if (OpenViewer) { return; }

            OpenViewer = true;
            PVproject.showDB.filepath = PVproject.CFG.o_path;
            PVproject.showIN.filepath = PVproject.CFG.i_path;

            PVproject.showIN.indexofbase = new List<int>();
            PVproject.showIN.indexofshow = 0;
            for (int i = 0; i < PVproject.IN.Count; i++) { PVproject.showIN.indexofbase.Add(i); }

            PVproject.showDB.indexofbase = new List<int>();
            PVproject.showDB.indexofshow = 0;
            for (int i = 0; i < PVproject.DB.Count; i++) { PVproject.showDB.indexofbase.Add(i); }

            Form_View.Form_View view = new Form_View.Form_View(this);
            view.Location = new Point(this.Location.X + 220, this.Location.Y);
            view.Show();
        }

        private void Form_Start_Load(object sender, EventArgs e)
        {
            
        }

        private void Welcom_MouseClick(object sender, MouseEventArgs e)
        {
            if (OpenViewer) { return; }
            OpenViewer = true;

            Form_View.Form_View view = new Form_View.Form_View(this);
            view.Location = new Point(this.Location.X + 220, this.Location.Y);
            view.ShowDialog();

            PVproject.CFG.FirstTime = false;
            System.Threading.Thread.Sleep(300);
            this.Welcom.Visible = false;
        }
    }
}