using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PictureViewer
{
    static class StartProject
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            new PVproject();

            // 读取配置文件
            try
            {
                Method.ReadFile.ReadFile_CFG();
            }
            catch
            {
                PVproject.srClose(); MessageBox.Show("CFG Error !"); return;
            }

            // 程序初始化
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            // 登录信息
            //Form_Error.Form_Error error = new Form_Error.Form_Error();
            //error.ShowDialog();
            //if (!PVproject.Username.Equals("ozxdno")) { return; }
            //if (!PVproject.Password.Equals("ani1357658uiu")) { return; }

            // 运行程序
            Application.Run(new Form_Start());
        }
    }

    /// <summary>
    /// 全局变量
    /// </summary>
    public class PVproject
    {
        public PVproject()
        {
            Username = "";
            Password = "";

            sr = null;
            sw = null;

            string exe_path = Application.ExecutablePath;
            int indexofmark = exe_path.LastIndexOf('\\');
            exe_path = exe_path.Substring(0, indexofmark);

            CFG.exe_path = exe_path;
            CFG.LastFileID = 0;
            CFG.viewIndex = 0;
            CFG.FirstTime = false;
            CFG.pic_process = "";
            CFG.gif_process = "";
            CFG.mov_process = "";
            CFG.i_path = "";
            CFG.o_path = "";
            CFG.e_path = "";
            CFG.database_path = exe_path;
            CFG.database_name = "pvdata.pvdb";

            DB = new List<DATABASE>();
            IN = new List<DATABASE>();

            tag.list = new List<TAG.LIST>();
            tag.SelectedTags = new List<string>();
            tag.TagStrs = new List<string>();
            tag.TagExps = new List<string>();

            showIN.edit = false;
            showIN.filepath = "";
            showIN.filename = "";
            showIN.indexofbase = new List<int>();
            showIN.indexofshow = 0;
            showIN.tags = new List<string>();
            showIN.type = "";
            showIN.extension = "";
            showIN.process = "";

            showDB.edit = false;
            showDB.filepath = "";
            showDB.filename = "";
            showDB.indexofbase = new List<int>();
            showDB.indexofshow = 0;
            showDB.tags = new List<string>();
            showDB.type = "";
            showDB.extension = "";
            showDB.process = "";
        }

        public static string Username;
        public static string Password;

        public static StreamReader sr;
        public static StreamWriter sw;

        public struct CONFIG
        {
            public string exe_path;

            public long LastFileID;
            public int viewIndex;
            public bool FirstTime;

            public string pic_process;
            public string gif_process;
            public string mov_process;
            
            public string i_path;
            public string o_path;
            public string e_path;
            public string database_path;
            public string database_name;
        }
        public struct DATABASE
        {
            public string name;
            public string type;
            public string extension;
            public long size;
            public List<string> tags;
        }
        public struct SHOWFILE
        {
            public bool edit;

            public string filepath;
            public string filename;

            public List<int> indexofbase;
            public int indexofshow;
            
            public long size;
            public List<string> tags;
            public string type;
            public string extension;
            public string process;
        }
        public struct VIEWER
        {
            public bool Open;

            public List<int> index_of_db;
            public List<int> index_of_in;
            public SHOWFILE i_file;
            public SHOWFILE o_file;
        }
        public struct TAG
        {
            public List<LIST> list;

            public List<string> SelectedTags;
            public List<string> TagStrs;
            public List<string> TagExps;
            
            public struct LIST
            {
                public string name;
                public List<string> tags;
            }
        }

        public static CONFIG CFG = new CONFIG();
        public static List<DATABASE> DB = new List<DATABASE>();
        public static List<DATABASE> IN = new List<DATABASE>();
        public static TAG tag = new TAG();
        public static SHOWFILE showIN = new SHOWFILE();
        public static SHOWFILE showDB = new SHOWFILE();

        public static void srClose()
        {
            if (sr != null) { sr.Close(); }
        }
        public static void swClose()
        {
            if (sw != null) { sw.Close(); }
        }
        public static string GetProcess(string type)
        {
            if (type.Equals("PIC")) { return CFG.pic_process; }
            if (type.Equals("GIF")) { return CFG.gif_process; }
            if (type.Equals("MOV")) { return CFG.mov_process; }
            if (type.Equals("CMC")) { return CFG.pic_process; }
            return "";
        }
        public static string GetExtension(string type)
        {
            if (type.Equals("PIC")) { return ".pv1"; }
            if (type.Equals("GIF")) { return ".pv2"; }
            if (type.Equals("MOV")) { return ".pv3"; }
            if (type.Equals("CMC")) { return ".pv0"; }
            return ".pv";
        }
        public static string GetType(string extension)
        {
            if (extension.Equals(".pv0")) { return "CMC"; }
            if (extension.Equals(".pv1")) { return "PIC"; }
            if (extension.Equals(".pv2")) { return "GIF"; }
            if (extension.Equals(".pv3")) { return "MOV"; }
            return "CMC";
        }
        public static string GetCMCpic1st(string folder)
        {
            if (!Directory.Exists(folder)) { return folder; }

            DirectoryInfo dir = new DirectoryInfo(folder);
            FileInfo[] files = dir.GetFiles();
            if (files.Length == 0) { return folder; }

            return files[0].FullName;
        }

        public static void OpenFile_S()
        {
            if (showIN.type.Equals("PIC")) { return; }
            if (showIN.type.Equals("GIF"))
            {
                string fullpath = showIN.filepath + "\\" + showIN.filename + showIN.extension;
                if (!(File.Exists(fullpath))) { return; }

                if (showIN.process.Length == 0)
                {
                    System.Diagnostics.Process.Start(fullpath); return;
                }

                try { System.Diagnostics.Process.Start(showIN.process, fullpath); } catch { }
                return;
            }
            if (showIN.type.Equals("MOV"))
            {
                string fullpath = showIN.filepath + "\\" + showIN.filename + showIN.extension;
                if (!(File.Exists(fullpath))) { return; }

                if (showIN.process.Length == 0)
                {
                    System.Diagnostics.Process.Start(fullpath); return;
                }

                try { System.Diagnostics.Process.Start(showIN.process, fullpath); } catch { }
                return;
            }
            if (showIN.type.Equals("CMC"))
            {
                string fullpath = showIN.filepath + "\\" + showIN.filename;
                Form_CMC.Form_CMC cmc = new Form_CMC.Form_CMC(fullpath);
                cmc.Show();
            }
        }
        public static void OpenFile_B()
        {
            if (showDB.type.Equals("PIC")) { return; }
            if (showDB.type.Equals("GIF"))
            {
                string fullpath = showDB.filepath + "\\" + showDB.filename + ".pv2";
                if (!(File.Exists(fullpath))) { return; }

                if (showDB.process.Length == 0)
                {
                    System.Diagnostics.Process.Start(fullpath); return;
                }

                try { System.Diagnostics.Process.Start(showDB.process, fullpath); } catch { }
                return;
            }
            if (showDB.type.Equals("MOV"))
            {
                string fullpath = showDB.filepath + "\\" + showDB.filename + ".pv3";
                if (!(File.Exists(fullpath))) { return; }

                if (showDB.process.Length == 0)
                {
                    System.Diagnostics.Process.Start(fullpath); return;
                }

                try { System.Diagnostics.Process.Start(showDB.process, fullpath); } catch { }
                return;
            }
            if (showDB.type.Equals("CMC"))
            {
                string fullpath = showDB.filepath + "\\" + showDB.filename;
                Form_CMC.Form_CMC cmc = new Form_CMC.Form_CMC(fullpath);
                cmc.Show();
            }
        }
        public static void MoveFile_S2B()
        {
            // 基本条件
            if (showIN.indexofshow < 0) { return; }
            if (showIN.indexofshow > showIN.indexofbase.Count - 1) { return; }

            int index = showIN.indexofbase[showIN.indexofshow];
            if (index < 0) { return; }
            if (index > IN.Count - 1) { return; }
            
            // 不存在目标路径
            if (!Directory.Exists(showDB.filepath)) { MessageBox.Show("No DB Path !"); return; }

            // 移动PIC、GIF、MOV文件
            if (showIN.type.Equals("PIC") || showIN.type.Equals("GIF") || showIN.type.Equals("MOV"))
            {
                // 跟新文件名
                string NewFileName = "pvdata" + (CFG.LastFileID + 1).ToString();

                // 判断文件的存在性，可移动性
                string sourpath = showIN.filepath + "\\" + showIN.filename + showIN.extension;
                if (!File.Exists(sourpath)) { MessageBox.Show("IN File Not Exist !"); return; }
                string destpath = showDB.filepath + "\\" + NewFileName + showIN.extension;
                if (File.Exists(destpath)) { MessageBox.Show("File Exists in DB !"); return; }

                // 复制和删除文件
                FileInfo file = new FileInfo(sourpath);
                file.CopyTo(destpath);
                file.Delete();

                // 更新显示信息，删除IN，新增DB
                IN.RemoveAt(showIN.indexofbase[showIN.indexofshow]);
                showIN.indexofbase = new List<int>();
                for (int i = 0; i < IN.Count; i++) { showIN.indexofbase.Add(i); }

                DATABASE db = new DATABASE();
                db.name = NewFileName;
                db.type = showIN.type;
                db.extension = showIN.extension;
                db.size = showIN.size;
                db.tags = new List<string>();
                db.tags.Add(showIN.tags[0]);
                DB.Add(db);
                showDB.indexofbase = new List<int>();
                for (int i = 0; i < DB.Count; i++) { showDB.indexofbase.Add(i); }

                CFG.LastFileID++;
                return;
            }

            // 移动CMC文件
            if (showIN.type.Equals("CMC"))
            {
                // 创建目标路径
                string sourpath = showIN.filepath + "\\" + showIN.filename;
                string destname = "pv0" + (CFG.LastFileID + 1).ToString();
                string destpath = showDB.filepath + "\\" + destname;
                if (Directory.Exists(destpath)) { MessageBox.Show("CMC File Existed !"); return; }
                Directory.CreateDirectory(destpath);
                CFG.LastFileID++;

                // 读取原路径下所有文件
                DirectoryInfo dir = new DirectoryInfo(sourpath);
                FileInfo[] files = dir.GetFiles();

                // 移动文件
                foreach (FileInfo ifile in files)
                {
                    CFG.LastFileID++;
                    string NewFileName = "pvdata" + (CFG.LastFileID).ToString();
                    ifile.CopyTo(destpath + "\\" + NewFileName + showIN.extension, true);
                }
                dir.Delete(true);

                // 删除 IN ，增加 DB。
                IN.RemoveAt(showIN.indexofbase[showIN.indexofshow]);
                showIN.indexofbase = new List<int>();
                for (int i = 0; i < IN.Count; i++) { showIN.indexofbase.Add(i); }

                DATABASE db = new DATABASE();
                db.name = destname;
                db.type = showIN.type;
                db.extension = showIN.extension;
                db.size = showIN.size;
                db.tags = new List<string>();
                db.tags.Add(showIN.tags[0]);
                DB.Add(db);
                showDB.indexofbase = new List<int>();
                for (int i = 0; i < DB.Count; i++) { showDB.indexofbase.Add(i); }
                return;
            }
        }
        public static void MoveFile_B2E()
        {
            // 基本条件
            if (showDB.indexofshow < 0) { return; }
            if (showDB.indexofshow > showDB.indexofbase.Count - 1) { return; }

            int indexB = showDB.indexofbase[showDB.indexofshow];
            if (indexB < 0) { return; }
            if (indexB > DB.Count - 1) { return; }

            // 不存在目标路径
            if (!Directory.Exists(CFG.e_path)) { MessageBox.Show("No Export Path !"); return; }

            // 移动PIC、GIF、MOV文件
            if (showDB.type.Equals("PIC") || showDB.type.Equals("GIF") || showDB.type.Equals("MOV"))
            {
                // 获取文件名
                string sour = showDB.filepath + "\\" + showDB.filename + showDB.extension;
                string dest = CFG.e_path + "\\" + showDB.filename + showDB.extension;
                if (File.Exists(dest)) { MessageBox.Show("File Exists in Export !"); return; }

                // 复制和删除文件
                FileInfo file = new FileInfo(sour);
                file.CopyTo(dest);
                file.Delete();

                // 更新显示信息，删除IN，新增DB
                DB.RemoveAt(showDB.indexofbase[showDB.indexofshow]);
                showDB.indexofbase = new List<int>();
                for (int i = 0; i < DB.Count; i++) { showDB.indexofbase.Add(i); }
                return;
            }

            // 移动CMC文件
            if (showDB.type.Equals("CMC"))
            {
                // 创建目标路径
                string sourpath = showDB.filepath + "\\" + showDB.filename;
                string destpath = CFG.e_path + "\\" + showDB.filename;
                if (Directory.Exists(destpath)) { MessageBox.Show("CMC File Existed in Export !"); return; }
                Directory.CreateDirectory(destpath);

                // 读取原路径下所有文件
                DirectoryInfo dir = new DirectoryInfo(sourpath);
                FileInfo[] files = dir.GetFiles();

                // 移动文件
                foreach (FileInfo ifile in files)
                {
                    string filename = ifile.Name.Substring(0, ifile.Name.Length - ifile.Extension.Length);
                    ifile.CopyTo(destpath + "\\" + filename + showDB.extension, true);
                }
                dir.Delete(true);

                // 删除 IN ，增加 DB。
                DB.RemoveAt(showDB.indexofbase[showDB.indexofshow]);
                showDB.indexofbase = new List<int>();
                for (int i = 0; i < DB.Count; i++) { showDB.indexofbase.Add(i); }
                return;
            }
        }
        public static bool DeleteFile_S()
        {
            return false;
        }
        public static void DeleteFile_B()
        {
            // 存在对应的文件变量
            if (showDB.indexofshow < 0) { return; }
            if (showDB.indexofshow > showDB.indexofbase.Count - 1) { return; }

            int index = showDB.indexofbase[showDB.indexofshow];
            if (index < 0) { return; }
            if (index > DB.Count - 1) { return; }

            // 删除 PIC/GIF/MOV 文件
            if (showDB.type.Equals("PIC") || showDB.type.Equals("GIF") || showDB.type.Equals("MOV"))
            {
                string fullpath = showDB.filepath + "\\" + showDB.filename + GetExtension(showDB.type);
                if (!File.Exists(fullpath)) { MessageBox.Show("No File !"); return; }

                DialogResult dr = MessageBox.Show
                    ("Do you want to Delete this File ?", "Q", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Cancel) { return; }

                FileInfo file = new FileInfo(fullpath);
                file.Delete();

                // 修改表
                DB.RemoveAt(index);
                showDB.indexofbase = new List<int>();
                for (int i = 0; i < DB.Count; i++) { showDB.indexofbase.Add(i); }
            }

            // 删除 CMC 文件
            if (showDB.type.Equals("CMC"))
            {
                string fullpath = showDB.filepath + "\\" + showDB.filename;
                if (!Directory.Exists(fullpath)) { MessageBox.Show("No CMC File !"); }

                DialogResult dr = MessageBox.Show
                    ("Do you want to Delete this Folder ?", "Q", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Cancel) { return; }

                DirectoryInfo dir = new DirectoryInfo(fullpath);
                dir.Delete(true);

                // 修改表
                DB.RemoveAt(index);
                showDB.indexofbase = new List<int>();
                for (int i = 0; i < DB.Count; i++) { showDB.indexofbase.Add(i); }
            }
        }
        public static void RemoveFile_S()
        {
            // 确认索引号
            if (showIN.indexofshow < 0) { return; }
            if (showIN.indexofshow > showIN.indexofbase.Count - 1) { return; }

            int index = showIN.indexofbase[showIN.indexofshow];
            if (index < 0) { return; }
            if (index > IN.Count) { return; }

            // 修改显示表
            IN.RemoveAt(index);
            showIN.indexofbase = new List<int>();
            for (int i = 0; i < IN.Count; i++) { showIN.indexofbase.Add(i); }
        }
        public static void RemoveFile_B()
        {
            // 确认索引号
            if (showDB.indexofshow < 0) { return; }
            if (showDB.indexofshow > showDB.indexofbase.Count - 1) { return; }

            showDB.indexofbase.RemoveAt(showDB.indexofshow);
        }
        public static void ReplaceFile_S2B()
        {
            // 基本条件
            if (showIN.indexofshow < 0) { return; }
            if (showIN.indexofshow > showIN.indexofbase.Count - 1) { return; }

            int indexS = showIN.indexofbase[showIN.indexofshow];
            if (indexS < 0) { return; }
            if (indexS > IN.Count - 1) { return; }

            // 基本条件
            if (showDB.indexofshow < 0) { return; }
            if (showDB.indexofshow > showDB.indexofbase.Count - 1) { return; }

            int indexB = showDB.indexofbase[showDB.indexofshow];
            if (indexB < 0) { return; }
            if (indexB > DB.Count - 1) { return; }

            //if (!showIN.extension.Equals(showDB.extension)) { return; }
            //if (!showIN.filename.Equals(showDB.filename)) { return; }

            // 替换 PIC/GIF/MOV 文件
            if (showIN.type.Equals("PIC") || showIN.type.Equals("GIF") || showIN.type.Equals("MOV"))
            {
                // 确认文件存在
                string sour = showIN.filepath + "\\" + showIN.filename + showIN.extension;
                string dest = showDB.filepath + "\\" + showDB.filename + showIN.extension;

                if (!File.Exists(sour)) { MessageBox.Show("IN File not Exist !"); return; }
                if (!File.Exists(dest)) { MessageBox.Show("DB File not Exist !"); return; }

                // 确认替换
                DialogResult dr = MessageBox.Show
                        ("Do you want to Replace this File ?", "Q", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Cancel) { return; }

                // 移动文件
                FileInfo file = new FileInfo(sour);
                file.CopyTo(dest, true);
                //file.Delete();

                // 更新SIZE
                DATABASE db = DB[indexB];
                db.size = IN[indexS].size;
                db.extension = IN[indexS].extension;
                db.type = IN[indexS].type;
                DB[indexB] = db;
                return;
            }

            // 替换CMC文件
            if (showIN.type.Equals("CMC"))
            {
                // 确认文件路径
                string sour = showIN.filepath + "\\" + showIN.filename;
                string dest = showDB.filepath + "\\" + showDB.filename;
                if (!Directory.Exists(sour)) { MessageBox.Show("No IN Folder !"); return; }
                if (!Directory.Exists(dest)) { MessageBox.Show("No DB Folder !"); return; }

                // 确认替换
                DialogResult dr = MessageBox.Show
                        ("Do you want to Replace this File ?", "Q", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Cancel) { return; }

                // 移动文件
                DirectoryInfo dir_dest = new DirectoryInfo(dest);
                dir_dest.Delete(true);
                Directory.CreateDirectory(dest);
                
                DirectoryInfo dir_sour = new DirectoryInfo(sour);
                FileInfo[] files = dir_sour.GetFiles();
                foreach (FileInfo ifile in files)
                {
                    string filename = ifile.Name.Substring(0, ifile.Name.Length - showIN.extension.Length);
                    ifile.CopyTo(dest + "\\" + filename + showIN.extension);
                }

                // 更新SIZE
                DATABASE db = DB[indexB];
                db.size = IN[indexS].size;
                db.extension = IN[indexS].extension;
                db.type = IN[indexS].type;
                DB[indexB] = db;
                return;
            }    
        }
    }
}
