using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PictureViewer.Class;

namespace PictureViewer
{
    public partial class Form_Search : Form
    {
        public Form_Search()
        {
            InitializeComponent();
        }

        public int FolderIndex = -1;
        public int FileIndex = -1;
        public bool Cancle = true;

        private int LastSelectFolder;
        private int LastSelectFile;
        private List<int> FolderIndexs = new List<int>();
        private List<int> FileIndexs = new List<int>();
        private List<string> FileNames = new List<string>();

        private void Form_Search_Load(object sender, EventArgs e)
        {
            // 文件判断
            bool ExistFolder = 0 <= Form_Main.config.FolderIndex && Form_Main.config.FolderIndex < FileOperate.RootFiles.Count;
            bool ExistFile = ExistFolder && (0 <= Form_Main.config.FileIndex && Form_Main.config.FileIndex < FileOperate.RootFiles[Form_Main.config.FolderIndex].Name.Count);

            // 为combox 添加选项
            this.comboBox1.Items.Clear();
            this.comboBox1.Items.Add("All");
            for (int i = 0; i < FileOperate.RootFiles.Count; i++)
            {
                string upperPath = "";
                string lowerPath = "";
                FileOperate.getPathName(FileOperate.RootFiles[i].Path, ref upperPath, ref lowerPath);
                this.comboBox1.Items.Add(lowerPath);
            }

            if (ExistFolder)
            { this.comboBox1.SelectedIndex = Form_Main.config.FolderIndex + 1; }
            else { this.comboBox1.SelectedIndex = 0; }

            // 为 listbox 添加选项
            LastSelectFolder = this.comboBox1.SelectedIndex;
            int select = LastSelectFolder - 1;
            if (select < 0) { ShowAllFoders(); } else { ShowSelectFolder(select); }

            if (ExistFile)
            { this.listBox1.SelectedIndex = Form_Main.config.FileIndex; }
            else { this.listBox1.SelectedIndex = -1; }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int select = this.comboBox1.SelectedIndex;
            ShowToolTip(true);
            if (select == -1) { return; }

            LastSelectFolder = select; select--;
            if (select < 0) { ShowAllFoders(); } else { ShowSelectFolder(select); }

            if (this.listBox1.SelectedIndex >= FileIndexs.Count)
            { this.listBox1.SelectedIndex = FileIndexs.Count - 1; }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowToolTip(false);
        }
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            int select = this.listBox1.SelectedIndex;
            if (select < 0) { Cancle = true; this.Close(); return; }
            Cancle = false;
            FolderIndex = FolderIndexs[select];
            FileIndex = FileIndexs[select];
            this.Close();
        }
        private void Form_Search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 27) { Cancle = true; this.Close(); return; }
            if (e.KeyValue != 13) { return; }

            int select = this.comboBox1.SelectedIndex - 1;
            if (select < 0) { ShowAllFoders(); } else { ShowSelectFolder(select); }
            
            Search(this.textBox1.Text);
            if (this.listBox1.SelectedIndex >= FileIndexs.Count)
            { this.listBox1.SelectedIndex = FileIndexs.Count - 1; }
        }

        private void ShowAllFoders()
        {
            this.listBox1.Items.Clear();
            FolderIndexs.Clear();
            FileIndexs.Clear();
            FileNames.Clear();
            int cnt = 1;

            for (int i = 0; i < FileOperate.RootFiles.Count; i++)
            {
                for (int j = 0; j < FileOperate.RootFiles[i].Name.Count; j++, cnt++)
                {
                    this.listBox1.Items.Add("[" + cnt.ToString() + "] " + FileOperate.RootFiles[i].Name[j]);
                    FolderIndexs.Add(i);
                    FileIndexs.Add(j);
                    FileNames.Add(FileOperate.RootFiles[i].Name[j]);
                }
            }
        }
        private void ShowSelectFolder(int select)
        {
            this.listBox1.Items.Clear();
            FolderIndexs.Clear();
            FileIndexs.Clear();
            FileNames.Clear();
            int cnt = 1;

            for (int i = 0; i < FileOperate.RootFiles[select].Name.Count; i++, cnt++)
            {
                this.listBox1.Items.Add("[" + cnt.ToString() + "] " + FileOperate.RootFiles[select].Name[i]);
                FolderIndexs.Add(select);
                FileIndexs.Add(i);
                FileNames.Add(FileOperate.RootFiles[select].Name[i]);
            }
        }
        private void ShowToolTip(bool pathChanged)
        {
            int sfolder = this.comboBox1.SelectedIndex;
            int sfile = this.listBox1.SelectedIndex;

            string path = "Not Exist";
            string name = "Unselect File";

            if (sfolder == 0) { path = "All"; }
            if (sfolder > 0 && sfolder <= FileOperate.RootFiles.Count) { path = FileOperate.RootFiles[sfolder - 1].Path; }

            if (pathChanged)
            {
                this.toolTip1.ToolTipTitle = path;
                this.toolTip1.SetToolTip(this.listBox1, name);
                return;
            }

            if (sfile >= 0 && sfile < FileNames.Count)
            { path = FileOperate.RootFiles[FolderIndexs[sfile]].Path; name = FileNames[sfile]; }

            this.toolTip1.ToolTipTitle = path;
            this.toolTip1.SetToolTip(this.listBox1, name);
        }
        private void Search(string exp)
        {
            if (exp == null || exp == "") { return; }
            exp = exp.ToLower();
            
            for (int i = FileNames.Count - 1; i >= 0; i--)
            {
                int index = FileNames[i].ToLower().IndexOf(exp);
                if (index != -1) { continue; }
                FolderIndexs.RemoveAt(i);
                FileIndexs.RemoveAt(i);
                FileNames.RemoveAt(i);
                this.listBox1.Items.RemoveAt(i);
            }

            //for (int i = 0; i < FileNames.Count; i++)
            //{ this.listBox1.Items[i] = "[" + (i + 1).ToString() + "] " + FileNames[i]; }

            //this.listBox1.Items.Clear();
            //for (int i = 0; i < FileIndexs.Count; i++)
            //{ this.listBox1.Items.Add("[" + (i + 1).ToString() + "] " + FileNames[i]); }
        }
    }
}
