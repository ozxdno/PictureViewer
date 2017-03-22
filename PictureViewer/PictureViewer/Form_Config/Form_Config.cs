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

namespace PictureViewer.Form_Config
{
    public partial class Form_Config : Form
    {
        public Form_Config()
        {
            InitializeComponent();

            // 初始化变量
            config = new PVproject.CONFIG();
            config.exe_path = PVproject.CFG.exe_path;
            config.LastFileID = PVproject.CFG.LastFileID;
            config.pic_process = PVproject.CFG.pic_process;
            config.gif_process = PVproject.CFG.gif_process;
            config.mov_process = PVproject.CFG.mov_process;
            config.i_path = PVproject.CFG.i_path;
            config.o_path = PVproject.CFG.o_path;
            config.e_path = PVproject.CFG.e_path;
            config.database_path = PVproject.CFG.database_path;
            config.database_name = PVproject.CFG.database_name;
            
            // 初始化界面
            this.PATH_input.Text = config.i_path;
            this.PATH_output.Text = config.o_path;
            this.PATH_Export.Text = config.e_path;
            this.NAME_database.Text = config.database_name;
            this.PIC_process.Text = config.pic_process;
            this.GIF_process.Text = config.gif_process;
            this.MOV_process.Text = config.mov_process;
            this.LastFileID.Text = config.LastFileID.ToString();

            // 刷新Input
            RefreshInput = false;
            RefreshDB = false;
        }

        private PVproject.CONFIG config = new PVproject.CONFIG();
        private bool RefreshInput = false;
        private bool RefreshDB = false;

        private void Input_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Folder = new FolderBrowserDialog();

            string SelectedPath = PVproject.CFG.i_path;
            if (!Directory.Exists(SelectedPath)) { SelectedPath = PVproject.CFG.exe_path; }
            Folder.SelectedPath = SelectedPath;

            if (Folder.ShowDialog() != DialogResult.OK) { return; }

            config.i_path = Folder.SelectedPath;
            this.PATH_input.Text = Folder.SelectedPath;
            RefreshInput = true;
        }

        private void Cancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Confirm_Click(object sender, EventArgs e)
        {
            PVproject.CFG = config;

            if (true)
            {
                try
                {
                    Method.ReadFile.ReadFile_IN();
                }
                catch
                {
                    PVproject.CFG.i_path = "";
                    //MessageBox.Show("IN error !");
                }
            }
            if (RefreshDB)
            {
                try
                {
                    Method.ReadFile.ReadFile_DB();
                }
                catch
                {
                    PVproject.CFG.database_path = "";
                    PVproject.CFG.database_name = "";
                    MessageBox.Show("PVDB error !");
                    this.NAME_database.Text = "";
                }
            }

            this.Close();
        }

        private void OPEN_input_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(config.i_path)) { MessageBox.Show("Path Not Exist !"); return; }
            System.Diagnostics.Process.Start("explorer.exe", config.i_path);
        }

        private void PATH_input_TextChanged(object sender, EventArgs e)
        {
            config.i_path = this.PATH_input.Text;
            RefreshInput = true;
        }

        private void Output_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Folder = new FolderBrowserDialog();

            string SelectedPath = PVproject.CFG.o_path;
            if (!Directory.Exists(SelectedPath)) { SelectedPath = PVproject.CFG.exe_path; }
            Folder.SelectedPath = SelectedPath;

            if (Folder.ShowDialog() != DialogResult.OK) { return; }

            config.o_path = Folder.SelectedPath;
            this.PATH_output.Text = Folder.SelectedPath;
        }

        private void OPEN_output_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(config.o_path)) { MessageBox.Show("Path Not Exist !"); return; }
            System.Diagnostics.Process.Start("explorer.exe", config.o_path);
        }

        private void PATH_output_TextChanged(object sender, EventArgs e)
        {
            config.o_path = this.PATH_output.Text;
        }

        private void DataBase_Click(object sender, EventArgs e)
        {
            OpenFileDialog Open = new OpenFileDialog();
            Open.Filter = "PVDB file(*.pvdb)|*.pvdb|all files(*.*)|*.*";
            string InitPath = PVproject.CFG.database_path;
            if (!Directory.Exists(InitPath)) { InitPath = PVproject.CFG.exe_path; }
            Open.InitialDirectory = InitPath;

            if (Open.ShowDialog() != DialogResult.OK) { return; }

            int index = Open.FileName.LastIndexOf('\\');

            config.database_path = Open.FileName.Substring(0,index);
            config.database_name = Open.FileName.Substring(index + 1);
            RefreshDB = true;
            this.NAME_database.Text = config.database_name;
        }

        private void OPEN_database_Click(object sender, EventArgs e)
        {
            string fullpath = config.database_path + "\\" + config.database_name;
            if (!File.Exists(fullpath)) { MessageBox.Show("No DataBase File !"); return; }

            DialogResult dr = MessageBox.Show
                ("Do you want to open DataBase ?", "Open", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (dr == DialogResult.Cancel) { return; }
            System.Diagnostics.Process.Start("notepad.exe", fullpath);
        }

        private void NAME_database_TextChanged(object sender, EventArgs e)
        {
            config.database_name = this.NAME_database.Text;
            RefreshDB = true;
        }

        private void PIC_process_TextChanged(object sender, EventArgs e)
        {
            config.pic_process = this.PIC_process.Text;
        }

        private void GIF_process_TextChanged(object sender, EventArgs e)
        {
            config.gif_process = this.GIF_process.Text;
        }

        private void MOV_process_TextChanged(object sender, EventArgs e)
        {
            config.mov_process = this.MOV_process.Text;
        }

        private void LastFileID_TextChanged(object sender, EventArgs e)
        {
            try { config.LastFileID = long.Parse(this.LastFileID.Text); } catch { }
        }

        private void CheckPVDB_Click(object sender, EventArgs e)
        {
            // 必须存在输出路径
            if (!Directory.Exists(PVproject.CFG.o_path)) { return; }

            // 读取所有文件
            DirectoryInfo dir = new DirectoryInfo(PVproject.CFG.o_path);
            FileInfo[] Files = dir.GetFiles();
            DirectoryInfo[] Folders = dir.GetDirectories();

            // 源数据中能否找到
            List<int> foundinDB = new List<int>();
            for (int i = 0; i < PVproject.DB.Count; i++) { foundinDB.Add(0); }

            // 新数据
            List<PVproject.DATABASE> newDB = new List<PVproject.DATABASE>();

            // 提示
            bool updata = false;

            // 比较所有文件
            foreach (FileInfo ifile in Files)
            {
                string extension = ifile.Extension;
                if (extension.Equals(".pv")) { updata = true; }
                if (!extension.Equals(".pv1") && !extension.Equals(".pv2") && !extension.Equals(".pv3")) { continue; }

                string filename = ifile.Name.Substring(0, ifile.Name.Length - 4);
                bool found = false;
                for (int i = 0; i < PVproject.DB.Count; i++)
                {
                    if (filename.Equals(PVproject.DB[i].name)) { found = true; foundinDB[i] = 1; break; }
                }

                if (found) { continue; }

                // 添加新的数据
                PVproject.DATABASE new_db = new PVproject.DATABASE();
                new_db.name = filename;
                new_db.size = ifile.Length >> 10;
                new_db.tags = new List<string>();
                new_db.type = PVproject.GetType(extension);
                new_db.tags.Add("FileType:" + new_db.type.ToLower());
                new_db.extension = extension;
                newDB.Add(new_db);
            }

            // 比较所有文件夹
            foreach (DirectoryInfo idir in Folders)
            {
                string filename = idir.Name;
                bool found = false;
                for (int i = 0; i < PVproject.DB.Count; i++)
                {
                    if (filename.Equals(PVproject.DB[i].name)) { found = true; foundinDB[i] = 1; break; }
                }

                if (found) { continue; }

                // 添加新数据
                if (idir.GetFiles().Length == 0) { return; }
                PVproject.DATABASE new_db = new PVproject.DATABASE();
                new_db.name = filename;
                new_db.size = idir.GetFiles().Length;
                new_db.tags = new List<string>();
                new_db.type = "CMC";
                new_db.tags.Add("FileType:cmc");
                new_db.extension = ".pv";
                newDB.Add(new_db);
            }

            // 删除不存在数据
            for (int i = foundinDB.Count-1; i >= 0; i--)
            {
                if (foundinDB[i] == 0) { PVproject.DB.RemoveAt(i); }
            }

            // 添加数据
            foreach (PVproject.DATABASE db in newDB) { PVproject.DB.Add(db); }

            for (int i = 0; i < PVproject.DB.Count; i++)
            {
                if (PVproject.DB[i].tags.Count != 0) { continue; }
                PVproject.DB[i].tags.Add("FileType:" + PVproject.DB[i].type.ToLower());
            }

            // 提示扫描完毕
            string tip = "Check PVDB Finished !";
            if (updata) { tip += " (You may need to Updata)"; }
            MessageBox.Show(tip);
        }

        private void UpdataPVDB_Click(object sender, EventArgs e)
        {
            // 存在路径
            if (!Directory.Exists(PVproject.CFG.o_path)) { return; }

            // 更新文件
            int total_db = PVproject.DB.Count;
            for (int i = 0; i < total_db; i++)
            {
                string fullpath = PVproject.CFG.o_path + "\\" + PVproject.DB[i].name;

                // 更新 PIC 文件
                if (PVproject.DB[i].type.Equals("PIC"))
                {
                    string fullname = fullpath + ".pv";
                    if (!File.Exists(fullname)) { continue; }

                    FileInfo file = new FileInfo(fullname);
                    file.MoveTo(fullpath + ".pv1");
                    continue;
                }

                // 更新 GIF 文件
                if (PVproject.DB[i].type.Equals("GIF"))
                {
                    string fullname = fullpath + ".pv";
                    if (!File.Exists(fullname)) { continue; }

                    FileInfo file = new FileInfo(fullname);
                    file.MoveTo(fullpath + ".pv2");
                    continue;
                }

                // 更新 MOV 文件
                if (PVproject.DB[i].type.Equals("MOV"))
                {
                    string fullname = fullpath + ".pv";
                    if (!File.Exists(fullname)) { continue; }

                    FileInfo file = new FileInfo(fullname);
                    file.MoveTo(fullpath + ".pv3");
                    continue;
                }
            }

            // 给出提示
            MessageBox.Show("Updata Finished !");
        }

        private void Export_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Folder = new FolderBrowserDialog();

            string SelectedPath = PVproject.CFG.e_path;
            if (!Directory.Exists(SelectedPath)) { SelectedPath = PVproject.CFG.exe_path; }
            Folder.SelectedPath = SelectedPath;

            if (Folder.ShowDialog() != DialogResult.OK) { return; }

            config.e_path = Folder.SelectedPath;
            this.PATH_Export.Text = Folder.SelectedPath;
        }

        private void OPEN_Export_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(config.e_path)) { MessageBox.Show("Path Not Exist !"); return; }
            System.Diagnostics.Process.Start("explorer.exe", config.e_path);
        }

        private void PATH_Export_TextChanged(object sender, EventArgs e)
        {
            config.e_path = this.PATH_Export.Text;
        }
    }
}
