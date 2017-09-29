using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PictureViewer.Class
{
    class FileOperate
    {
        ///////////////////////////////////////////////////// public attribute ///////////////////////////////////////////////

        /// <summary>
        /// 存储不同根目录下的所有文件信息
        /// </summary>
        public static List<ROOT> RootFiles = new List<ROOT>();

        /// <summary>
        /// 根目录
        /// </summary>
        public struct ROOT
        {
            /// <summary>
            /// 根目录路径
            /// </summary>
            public string Path;
            /// <summary>
            /// 根目录文件（带后缀）
            /// </summary>
            public List<string> Name;
        } 

        ///////////////////////////////////////////////////// private attribute ///////////////////////////////////////////////

        

        ///////////////////////////////////////////////////// public method ///////////////////////////////////////////////
    
        /// <summary>
        /// 获取当前 EXE 文件所在路径。
        /// </summary>
        /// <returns></returns>
        public static string getExePath()
        {
            string exe_path = Application.ExecutablePath;
            int indexofmark = exe_path.LastIndexOf('\\');
            return exe_path.Substring(0, indexofmark);
        }

        /// <summary>
        /// 搜索根目录
        /// </summary>
        /// <param name="rootpath"></param>
        /// <returns></returns>
        public static int Search(string rootpath)
        {
            for (int i = 0; i < RootFiles.Count; i++) { if (rootpath == RootFiles[i].Path) { return i; } }
            return -1;
        }
        /// <summary>
        /// 在源字符串中搜索目标字符串，并返回目标字符串索引号。
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="target">目标字符串</param>
        /// <returns></returns>
        public static int Search(string[] source, string target)
        {
            if (source == null) { return -1; }
            for (int i = 0; i < source.Length; i++) { if (source[i] == target) { return i; } }
            return -1;
        }
        /// <summary>
        /// 在源字符串中搜索目标字符串，并返回目标字符串索引号。
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="target">目标字符串</param>
        /// <returns></returns>
        public static int Search(List<string> source, string target)
        {
            if (source == null) { return -1; }
            for (int i = 0; i < source.Count; i++) { if (source[i] == target) { return i; } }
            return -1;
        }
        
        /// <summary>
        /// 获取文件后缀
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public static string getExtension(string name)
        {
            int dot = name.LastIndexOf('.'); if (dot == -1) { return ""; }
            return name.Substring(dot).ToLower();
        }
    
        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        public static int getFileType(string extension)
        {
            if (extension == "") { return 1; }
            if (extension == ".jpg") { return 2; }
            if (extension == ".jpeg") { return 2; }
            if (extension == ".png") { return 2; }
            if (extension == ".bmp") { return 2; }
            if (extension == ".gif") { return 3; }
            if (extension == ".avi") { return 4; }
            if (extension == ".mp4") { return 4; }
            if (extension == ".webm") { return 0; } // 未被处理的文件
            return -1;
        }

        /// <summary>
        /// 判断文件是否支持（不能判断文件夹）
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        public static bool IsSupport(string extension)
        {
            if (extension == ".jpg") { return true; }
            if (extension == ".jpeg") { return true; }
            if (extension == ".png") { return true; }
            if (extension == ".bmp") { return true; }
            if (extension == ".gif") { return true; }
            if (extension == ".avi") { return true; }
            if (extension == ".mp4") { return true; }
            if (extension == ".webm") { return true; }
            return false;
        }

        /// <summary>
        /// 判断文件是否支持（不能判断文件夹）
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        public static bool IsSupportHide(string extension)
        {
            if (extension == ".pv1") { return true; }
            if (extension == ".pv2") { return true; }
            if (extension == ".pv3") { return true; }
            if (extension == ".pv4") { return true; }
            if (extension == ".pv5") { return true; }
            if (extension == ".pv6") { return true; }
            if (extension == ".pv7") { return true; }
            if (extension == ".pv8") { return true; }
            return false;
        }

        /// <summary>
        /// 导入根目录，返回路径名称。
        /// </summary>
        /// <returns></returns>
        public static string Input()
        {
            FolderBrowserDialog Folder = new FolderBrowserDialog();

            string SelectedPath = Form_Main.config.ConfigPath;
            if (Form_Main.config.FolderIndex >= 0 && Form_Main.config.FolderIndex < RootFiles.Count)
            { SelectedPath = RootFiles[Form_Main.config.FolderIndex].Path; }
            if (!Directory.Exists(SelectedPath)) { SelectedPath = Form_Main.config.ConfigPath; }
            Folder.SelectedPath = SelectedPath;

            if (Folder.ShowDialog() != DialogResult.OK) { return ""; }
            int indexofRoot = Search(Folder.SelectedPath);
            if (indexofRoot != -1) { MessageBox.Show("此路径已经存在！"); return ""; }

            ROOT root = new ROOT(); root.Path = Folder.SelectedPath; root.Name = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(root.Path);
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] folders = dir.GetDirectories();

            foreach (DirectoryInfo folder in folders) { root.Name.Add(folder.Name); }
            foreach (FileInfo file in files)
            { if (FileOperate.IsSupport(FileOperate.getExtension(file.Name))) { root.Name.Add(file.Name); } }

            RootFiles.Add(root);

            Form_Main.config.FolderIndex = RootFiles.Count - 1;
            Form_Main.config.FileIndex = root.Name.Count == 0 ? -1 : 0;
            Form_Main.config.SubIndex = 0;

            return root.Path;
        }

        /// <summary>
        /// 获取文件夹路径下的图片文件
        /// </summary>
        /// <param name="path">指定路径</param>
        /// <returns></returns>
        public static List<string> getSubFiles(string path)
        {
            List<string> Files = new List<string>();
            if (!Directory.Exists(path)) { return Files; }

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                if (getFileType(file.Extension) == 2) { Files.Add(file.Name); }
                if (getFileType(file.Extension) == 3) { Files.Add(file.Name); }
                if (getFileType(file.Extension) == 4) { Files.Add(file.Name); }
            }

            return Files;
        }

        /// <summary>
        /// 把隐式后缀转换为显式后缀。
        /// </summary>
        /// <param name="hideExtension">隐式后缀</param>
        /// <returns></returns>
        public static string getShowExtension(string hideExtension)
        {
            if (hideExtension == "") { return ""; }
            if (hideExtension == ".pv1") { return ".jpg"; }
            if (hideExtension == ".pv2") { return ".jpeg"; }
            if (hideExtension == ".pv3") { return ".png"; }
            if (hideExtension == ".pv4") { return ".bmp"; }
            if (hideExtension == ".pv5") { return ".gif"; }
            if (hideExtension == ".pv6") { return ".avi"; }
            if (hideExtension == ".pv7") { return ".mp4"; }
            if (hideExtension == ".pv8") { return ".webm"; }

            return null;
        }

        /// <summary>
        /// 把显式后缀转换为隐式后缀。
        /// </summary>
        /// <param name="showExtension">显式后缀</param>
        /// <returns></returns>
        public static string getHideExtension(string showExtension)
        {
            if (showExtension == "") { return ""; }
            if (showExtension == ".jpg") { return ".pv1"; }
            if (showExtension == ".jpeg") { return ".pv2"; }
            if (showExtension == ".png") { return ".pv3"; }
            if (showExtension == ".bmp") { return ".pv4"; }
            if (showExtension == ".gif") { return ".pv5"; }
            if (showExtension == ".avi") { return ".pv6"; }
            if (showExtension == ".mp4") { return ".pv7"; }
            if (showExtension == ".webm") { return ".pv8"; }

            return null;
        }

        /// <summary>
        /// 隐藏文件
        /// </summary>
        /// <param name="path">路径</param>
        public static void HideFiles(string path)
        {
            if (!Directory.Exists(path)) { return; }

            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo[] folders = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files) { HideFiles(path, file.Name); }
            foreach (DirectoryInfo folder in folders)
            {
                string newPath = path + "\\" + folder.Name;
                files = folder.GetFiles();
                foreach (FileInfo file in files) { HideFiles(newPath, file.Name); }
            }
        }
        /// <summary>
        /// 隐藏文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="name">名称</param>
        public static void HideFiles(string path, string name)
        {
            string extension = getExtension(name);
            int type = getFileType(extension);
            if (type == -1 || type == 1) { return; }

            FileInfo file = new FileInfo(path + "\\" + name);
            file.MoveTo(path + "\\" + name.Substring(0, name.Length - extension.Length) + getHideExtension(extension));
        }

        /// <summary>
        /// 显示文件
        /// </summary>
        /// <param name="path">路径</param>
        public static void ShowFiles(string path)
        {
            if (!Directory.Exists(path)) { return; }

            DirectoryInfo dir = new DirectoryInfo(path);
            DirectoryInfo[] folders = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files) { ShowFiles(path, file.Name); }
            foreach (DirectoryInfo folder in folders)
            {
                string newPath = path + "\\" + folder.Name;
                files = folder.GetFiles();
                foreach (FileInfo file in files) { ShowFiles(newPath, file.Name); }
            }
        }
        /// <summary>
        /// 显示文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="name">名称</param>
        public static void ShowFiles(string path, string name)
        {
            string extension = getExtension(name);
            if (!IsSupportHide(extension)) { return; }

            FileInfo file = new FileInfo(path + "\\" + name);
            file.MoveTo(path + "\\" + name.Substring(0, name.Length - extension.Length) + getShowExtension(extension));
        }

        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////
    }
}
