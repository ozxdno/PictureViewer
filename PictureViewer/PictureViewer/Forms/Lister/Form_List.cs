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

namespace PictureViewer.Forms
{
    public partial class Form_List : Form
    {
        public Form_List()
        {
            InitializeComponent();
        }

        #region 输出使用变量

        /// <summary>
        /// 转到根目录
        /// </summary>
        public int FolderIndex = -1;
        /// <summary>
        /// 转到文件
        /// </summary>
        public int FileIndex = -1;
        /// <summary>
        /// 转到子文件
        /// </summary>
        public int SubIndex = -1;
        /// <summary>
        /// 取消转到
        /// </summary>
        public bool Cancle = true;

        #endregion

        #region 内部变量
        
        /// <summary>
        /// 列表中所列文件名
        /// </summary>
        private List<string> FileNames = new List<string>();

        /// <summary>
        /// 正在初始化
        /// </summary>
        private bool IsInitialize = true;
        /// <summary>
        /// 是否选中子文件夹
        /// </summary>
        private bool IsSub = false;

        /// <summary>
        /// 当前选中文件的根目录序号 +1（添加了 All 选项）
        /// </summary>
        private int SelectedFolder = 0;
        /// <summary>
        /// 当前选中文件的序号
        /// </summary>
        private int SelectedFile = 0;
        /// <summary>
        /// 当前选中文件的子序号
        /// </summary>
        private int SelectedSub = 0;

        /// <summary>
        /// 列表中的每一个项目的根目录索引号
        /// </summary>
        private List<int> FolderIndexs = new List<int>();
        /// <summary>
        /// 列表中的每一个项目的文件索引号
        /// </summary>
        private List<int> FileIndexs = new List<int>();
        /// <summary>
        /// 列表中的每一个项目的子文件索引号
        /// </summary>
        private List<int> SubIndexs = new List<int>();

        #endregion

        private void Form_Search_Load(object sender, EventArgs e)
        {
            // 初始化
            IsInitialize = true;
            SelectedFolder = FileOperate.ExistFolder(Form_Main.config.FolderIndex) ? Form_Main.config.FolderIndex + 1 : -1;
            SelectedFile = FileOperate.ExistFile(Form_Main.config.FolderIndex, Form_Main.config.FileIndex) ? Form_Main.config.FileIndex : -1;

            string name = FileOperate.getIndexName(SelectedFolder - 1, SelectedFile);
            int type = FileOperate.getFileType(FileOperate.getExtension(name));
            IsSub = FileOperate.IsComic(type) && Form_Main.config.SubFiles.Count != 0;
            SelectedSub = IsSub ? Form_Main.config.SubIndex : -1;
            
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

            // 为 combox 添加选中项
            if (IsSub)
            {
                this.comboBox1.Text = this.comboBox1.Items[SelectedFolder].ToString() + "\\" + name;
            }
            else
            {
                this.comboBox1.SelectedIndex = SelectedFolder;
            }

            // 为 listbox 添加选项
            if (IsSub)
            {
                ShowSelectSubFolder();
            }
            else
            {
                if (SelectedFolder > 0) { ShowSelectFolder(); }
                if (SelectedFolder == 0) { ShowAllFoders(); }
            }

            // 为 listbox 添加选中项
            if (IsSub)
            {
                this.listBox1.SelectedIndex = SelectedSub;
            }
            else
            {
                this.listBox1.SelectedIndex = SelectedFile;
            }
            
            // 初始化提示
            ShowToolTip();

            // 初始化结束
            IsInitialize = false;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsInitialize) { return; }
            IsSub = false;

            int select = this.comboBox1.SelectedIndex;
            if (select == -1) { ShowEmpty(); return; }

            SelectedFolder = select;
            SelectedFile = -1;
            SelectedSub = -1;

            if (select == 0) { ShowAllFoders(); }
            if (select > 0) { ShowSelectFolder(); }
            ShowToolTip();
        }
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {

        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsInitialize) { return; }

            int select = this.listBox1.SelectedIndex;
            if (select < 0) { return; }

            SelectedFolder = FolderIndexs[select] + 1;
            SelectedFile = FileIndexs[select];
            SelectedSub = SubIndexs[select];

            ShowToolTip();

            //FolderIndex = FolderIndexs[select];
            //FileIndex = FileIndexs[select];
            //SubIndex = SubIndexs[select];
            //Cancle = false;
        }
        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (IsInitialize) { return; }
            //return;

            // 双击空列表
            int select = this.listBox1.SelectedIndex;
            if (select < 0) { return; }

            // 加载选中项
            SelectedFolder = FolderIndexs[select] + 1;
            SelectedFile = FileIndexs[select];
            SelectedSub = SubIndexs[select];

            // 双击文件夹
            string name = FileNames[select];
            int type = FileOperate.getFileType(FileOperate.getExtension(name));
            if (!IsSub && (type == 1 || type == 5))
            {
                IsInitialize = true;
                IsSub = true;
                ShowSelectSubFolder();
                if (!IsSub)
                {
                    HideToolTip();
                    if (DialogResult.OK == MessageBox.Show("该文件为空文件，是否转到该文件？", "提示", MessageBoxButtons.OKCancel))
                    {
                        FolderIndex = SelectedFolder - 1;
                        FileIndex = SelectedFile;
                        SubIndex = SelectedSub;
                        Cancle = false;
                        IsInitialize = false;
                        //this.Close();
                        return;
                    }
                }
                IsInitialize = false;
                ShowToolTip(); return;
            }

            // 双击转到
            FolderIndex = SelectedFolder - 1;
            FileIndex = SelectedFile;
            SubIndex = SelectedSub;
            Cancle = false;
            //this.Close();
        }
        private void Form_Search_KeyDown(object sender, KeyEventArgs e)
        {
            // ESC ENTER
            if (e.KeyValue == Class.Load.settings.FastKey_Search_Esc) { Cancle = true; this.Close(); return; }
            if (e.KeyValue != Class.Load.settings.FastKey_Search_Enter) { return; }

            // 初始化
            if (IsInitialize) { return; }

            // 搜索
            Search(this.textBox1.Text);
        }
        
        private void ShowAllFoders()
        {
            this.listBox1.Items.Clear();
            FolderIndexs.Clear();
            FileIndexs.Clear();
            SubIndexs.Clear();
            FileNames.Clear();
            int cnt = 1;

            for (int i = 0; i < FileOperate.RootFiles.Count; i++)
            {
                for (int j = 0; j < FileOperate.RootFiles[i].Name.Count; j++, cnt++)
                {
                    this.listBox1.Items.Add("[" + cnt.ToString() + "] " + FileOperate.RootFiles[i].Name[j]);
                    FolderIndexs.Add(i);
                    FileIndexs.Add(j);
                    SubIndexs.Add(-1);
                    FileNames.Add(FileOperate.RootFiles[i].Name[j]);
                }
            }
        }
        private void ShowSelectFolder()
        {
            if (!FileOperate.ExistFolder(SelectedFolder - 1)) { return; }

            this.listBox1.Items.Clear();
            FolderIndexs.Clear();
            FileIndexs.Clear();
            SubIndexs.Clear();
            FileNames.Clear();
            int cnt = 1;

            for (int i = 0; i < FileOperate.RootFiles[SelectedFolder - 1].Name.Count; i++, cnt++)
            {
                this.listBox1.Items.Add("[" + cnt.ToString() + "] " + FileOperate.RootFiles[SelectedFolder - 1].Name[i]);
                FolderIndexs.Add(SelectedFolder - 1);
                FileIndexs.Add(i);
                SubIndexs.Add(-1);
                FileNames.Add(FileOperate.RootFiles[SelectedFolder - 1].Name[i]);
            }
        }
        private void ShowSelectSubFolder()
        {
            string path = FileOperate.getIndexPath(SelectedFolder - 1);
            string name = FileOperate.getIndexName(SelectedFolder - 1, SelectedFile);
            int type = FileOperate.getFileType(FileOperate.getExtension(name));
            if (!FileOperate.IsComic(type)) { return; }

            List<string> subfiles = new List<string>();
            if (type == 1) { subfiles = FileOperate.getSubFiles(path + "\\" + name); }
            if (type == 5) { subfiles = ZipOperate.getZipFileEX(path + "\\" + name); }
            if (subfiles.Count == 0) { IsSub = false; return; }
            FileNames = subfiles;

            string rootpath = "", rootname = "";
            FileOperate.getPathName(path, ref rootpath, ref rootname);

            this.comboBox1.Text = rootname + "\\" + name;
            this.listBox1.Items.Clear();
            FolderIndexs.Clear();
            FileIndexs.Clear();
            SubIndexs.Clear();

            for (int i = 0; i < FileNames.Count; i++)
            {
                this.listBox1.Items.Add("[" + (i + 1).ToString() + "] " + FileNames[i]);
                FolderIndexs.Add(SelectedFolder - 1);
                FileIndexs.Add(SelectedFile);
                SubIndexs.Add(i);
            }
        }
        private void ShowEmpty()
        {
            this.listBox1.Items.Clear();
            FolderIndexs.Clear();
            FileIndexs.Clear();
            SubIndexs.Clear();
            FileNames.Clear();
            HideToolTip();
        }
        private void ShowToolTip()
        {
            string folder = FileOperate.ExistFolder(SelectedFolder - 1) ?
                FileOperate.getIndexPath(SelectedFolder - 1) :
                (SelectedFolder == 0 ? "All" : "Not Select Folder");
            string file = FileOperate.ExistFile(SelectedFolder - 1, SelectedFile) ?
                FileOperate.getIndexName(SelectedFolder - 1, SelectedFile) :
                "Not Select File";
            string sub = (IsSub && FileNames.Count != 0 && this.listBox1.SelectedIndex != -1) ?
                FileNames[this.listBox1.SelectedIndex] :
                "Not Select File";

            this.toolTip1.ToolTipTitle = IsSub ? folder + "\\" + file : folder;
            this.toolTip1.SetToolTip(this.listBox1, IsSub ? sub : file);
        }
        private void HideToolTip()
        {
            this.toolTip1.Hide(this.listBox1);
        }
        private void Search(string exp)
        {
            if (IsSub) { ShowSelectSubFolder(); }
            else if (SelectedFolder <= 0) { ShowAllFoders(); }
            else { ShowSelectFolder(); }

            if (exp == null || exp == "") { return; }
            exp = exp.ToLower();
            
            for (int i = FileNames.Count - 1; i >= 0; i--)
            {
                int index = FileNames[i].ToLower().IndexOf(exp);
                if (index != -1) { continue; }
                FolderIndexs.RemoveAt(i);
                FileIndexs.RemoveAt(i);
                SubIndexs.RemoveAt(i);
                FileNames.RemoveAt(i);
                this.listBox1.Items.RemoveAt(i);
            }

            this.toolTip1.ToolTipTitle = "Search Result : " + this.listBox1.Items.Count.ToString();
            this.toolTip1.SetToolTip(this.listBox1, "Not Select File");
        }
    }
}
