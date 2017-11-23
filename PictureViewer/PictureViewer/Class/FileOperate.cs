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
            return Application.StartupPath;
        }
        /// <summary>
        /// 对一个完整的文件进行分割，分别获取文件路径和文件名
        /// </summary>
        /// <param name="fullname">全路径</param>
        /// <param name="path">所在路径</param>
        /// <param name="name">文件名</param>
        public static void getPathName(string fullname, ref string path, ref string name)
        {
            int cut = fullname.LastIndexOf('\\');
            if (cut == -1) { path = ""; name = fullname; return; }

            path = fullname.Substring(0, cut);
            name = fullname.Substring(cut + 1);
            return;
        }
        /// <summary>
        /// 获取显示文本（适用于窗体）。
        /// </summary>
        /// <param name="text">用于显示的文本</param>
        /// <param name="nShow">允许显示长度</param>
        /// <param name="nLink">连接符的数量</param>
        /// <param name="getTail">保留尾部，为真时保留头部</param>
        /// <param name="link">连接符（当文本过长的省略符）</param>
        /// <returns></returns>
        public static string getShowString(string text, int nShow, int nLink, bool getTail = true, char link = '.')
        {
            if (text == null) { return text; }
            if (text.Length <= nShow) { return text; }

            string linkstr = "";
            for (int i = 0; i < nLink; i++) { linkstr += link; }
            if (getTail) { return linkstr + text.Substring(text.Length - nShow - nLink); }
            return text.Substring(text.Length - nShow - nLink) + linkstr;
        }

        /// <summary>
        /// 搜索根目录
        /// </summary>
        /// <param name="rootpath">需要寻找的根目录</param>
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
        /// 用类型搜索文件。Type （类型）值如下：
        /// 1 - Folder；
        /// 2 - PIC；
        /// 3 - GIF；
        /// 4 - MUSIC；
        /// 5 - VIDEO；
        /// 6 - ZIP；
        /// </summary>
        /// <param name="Types">类型集合</param>
        /// <param name="FolderIndexes">结果的根目录索引号</param>
        /// <param name="FileIndexes">结果的文件索引号</param>
        /// <param name="SubIndexes">结果的子文件索引号</param>
        public static void SearchFileByType(List<int> Types, ref List<int> FolderIndexes, ref List<int> FileIndexes, ref List<int> SubIndexes)
        {
            FolderIndexes = new List<int>();
            FileIndexes = new List<int>();
            SubIndexes = new List<int>();
            if (Types == null || Types.Count == 0) { return; }

            bool addfolder = false, addpic = false, addgif = false, addmusic = false, addvideo = false, addzip = false;
            for (int i = 0; i < Types.Count; i++)
            {
                if (Types[i] == 1) { addfolder = true; }
                if (Types[i] == 2) { addpic = true; }
                if (Types[i] == 3) { addgif = true; }
                if (Types[i] == 4) { addmusic = true; }
                if (Types[i] == 5) { addvideo = true; }
                if (Types[i] == 6) { addzip = true; }
            }

            for (int i = 0; i < RootFiles.Count; i++)
            {
                for (int j = 0; j < RootFiles[i].Name.Count; j++)
                {
                    string path = RootFiles[i].Path;
                    string name = RootFiles[i].Name[j];
                    string extension = getExtension(name);
                    int type = getFileType(extension);
                    bool isMusic = IsMusic(extension);
                    bool isVideo = IsVideo(extension);

                    bool found =
                        (addfolder && type == 1) ||
                        (addpic && type == 2) ||
                        (addgif && type == 3) ||
                        (addmusic && isMusic) ||
                        (addvideo && isVideo) ||
                        (addzip && type == 5);
                    if (found) { FolderIndexes.Add(i); FileIndexes.Add(j); SubIndexes.Add(-1); continue; }
                    if (!IsComic(type)) { continue; }

                    List<string> subfiles = null;
                    if (type == 1) { subfiles = getSubFiles(path + "\\" + name); }
                    if (type == 5) { subfiles = ZipOperate.getZipFileEX(path + "\\" + name); }
                    if (subfiles == null || subfiles.Count == 0) { continue; }

                    for (int k = 0; k < subfiles.Count; k++)
                    {
                        name = subfiles[k];
                        extension = getExtension(name);
                        type = getFileType(extension);
                        isMusic = IsMusic(extension);
                        isVideo = IsVideo(extension);

                        found =
                            (addpic && type == 2) ||
                            (addgif && type == 3) ||
                            (addmusic && isMusic) ||
                            (addvideo && isVideo);

                        if (found) { FolderIndexes.Add(i); FileIndexes.Add(j); SubIndexes.Add(k); }
                    }
                }
            }
        }

        /// <summary>
        /// 从完整的文件名中获取文件路径
        /// </summary>
        /// <param name="fullname">文件的完整文件名</param>
        /// <returns></returns>
        public static string getPath(string fullname)
        {
            if (fullname == null || fullname.Length == 0) { return ""; }
            int cut = fullname.LastIndexOf('\\'); if (cut == -1) { return ""; }
            return fullname.Substring(0, cut);
        }
        /// <summary>
        /// 从完整的文件名中获取文件名称
        /// </summary>
        /// <param name="fullname">文件的完整文件名</param>
        public static string getName(string fullname)
        {
            if (fullname == null || fullname.Length == 0) { return ""; }
            int cut = fullname.LastIndexOf('\\'); if (cut == -1) { return fullname; }
            return fullname.Substring(cut + 1);
        }
        /// <summary>
        /// 从文件名中获取不带后缀的文件名
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public static string getName2(string name)
        {
            if (name == null || name.Length == 0) { return ""; }
            int dot = name.LastIndexOf('.'); if (dot == -1) { return name; }
            return name.Substring(0, dot);
        }
        /// <summary>
        /// 获取文件后缀
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        public static string getExtension(string name)
        {
            if (name == null || name.Length == 0) { return "unknow"; }
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
            return FileSupport.getFileType(extension);

            // 以下为历史代码，已弃用。

            // show files
            if (extension == "") { return 1; }
            if (extension == ".jpg") { return 2; }
            if (extension == ".jpeg") { return 2; }
            if (extension == ".png") { return 2; }
            if (extension == ".bmp") { return 2; }
            if (extension == ".gif") { return 3; }
            if (extension == ".avi") { return 4; }
            if (extension == ".mp4") { return 4; }
            if (extension == ".webm") { return 0; } // 未被处理的文件
            if (extension == ".zip") { return 5; }
            if (extension == ".mp3") { return 4; }
            // hide files
            if (extension == ".pv1") { return 2; }
            if (extension == ".pv2") { return 2; }
            if (extension == ".pv3") { return 2; }
            if (extension == ".pv4") { return 2; }
            if (extension == ".pv5") { return 3; }
            if (extension == ".pv6") { return 4; }
            if (extension == ".pv7") { return 4; }
            if (extension == ".pv8") { return 0; }
            if (extension == ".pv9") { return 5; }
            if (extension == ".pv10") { return 4; }

            return -1;
        }

        /// <summary>
        /// 判断文件是否支持（不能判断文件夹）
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        public static bool IsSupport(string extension)
        {
            return FileSupport.IsSupport(extension);

            // 以下为历史代码，已弃用。

            // show file
            if (extension == ".jpg") { return true; }
            if (extension == ".jpeg") { return true; }
            if (extension == ".png") { return true; }
            if (extension == ".bmp") { return true; }
            if (extension == ".gif") { return true; }
            if (extension == ".avi") { return true; }
            if (extension == ".mp4") { return true; }
            if (extension == ".webm") { return true; }
            if (extension == ".zip") { return true; }
            if (extension == ".mp3") { return true; }
            // hide file
            if (extension == ".pv1") { return true; }
            if (extension == ".pv2") { return true; }
            if (extension == ".pv3") { return true; }
            if (extension == ".pv4") { return true; }
            if (extension == ".pv5") { return true; }
            if (extension == ".pv6") { return true; }
            if (extension == ".pv7") { return true; }
            if (extension == ".pv8") { return true; }
            if (extension == ".pv9") { return true; }
            if (extension == ".pv10") { return true; }
            return false;
        }

        /// <summary>
        /// 判断文件是否支持（不能判断文件夹）
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        public static bool IsSupportHide(string extension)
        {
            return FileSupport.IsSupportHide(extension);

            // 以下为历史代码，已弃用。

            if (extension == ".pv1") { return true; }
            if (extension == ".pv2") { return true; }
            if (extension == ".pv3") { return true; }
            if (extension == ".pv4") { return true; }
            if (extension == ".pv5") { return true; }
            if (extension == ".pv6") { return true; }
            if (extension == ".pv7") { return true; }
            if (extension == ".pv8") { return true; }
            if (extension == ".pv9") { return true; }
            if (extension == ".pv10") { return true; }
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
        /// 重新加载指定路径下的文件。
        /// </summary>
        /// <param name="fullpath">指定路径</param>
        /// <param name="jump">是否跳转到该路径</param>
        public static void Reload(string fullpath, bool jump = false)
        {
            // 覆盖路径是否已经存在，若不存在，新建路径。
            int index = Search(fullpath);
            if (index != -1) { RootFiles[index].Name.Clear(); }
            if (index == -1)
            {
                return; // 不允许重建
                index = RootFiles.Count; ROOT newRoot = new ROOT();
                newRoot.Path = fullpath;
                newRoot.Name = new List<string>();
                RootFiles.Add(newRoot);
            }

            // 重新加载该路径下所有文件
            ROOT cover = RootFiles[index]; if (!Directory.Exists(cover.Path)) { return; }
            DirectoryInfo dir = new DirectoryInfo(cover.Path);
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] folders = dir.GetDirectories();

            foreach (DirectoryInfo folder in folders) { cover.Name.Add(folder.Name); }
            foreach (FileInfo file in files)
            { if (FileOperate.IsSupport(FileOperate.getExtension(file.Name))) { cover.Name.Add(file.Name); } }

            RootFiles[index] = cover; if (!jump) { return; }

            Form_Main.config.FolderIndex = index;
            Form_Main.config.FileIndex = 0;
            Form_Main.config.SubIndex = 0;
        }
        /// <summary>
        /// 重新加载所有根目录下的文件（不会删除相同的根目录）
        /// </summary>
        public static void Reload()
        {
            for (int i = RootFiles.Count - 1; i >= 0; i--)
            {
                if (!Directory.Exists(RootFiles[i].Path)) { RootFiles.RemoveAt(i); }
            }

            for (int i = 0; i < RootFiles.Count; i++)
            {
                RootFiles[i].Name.Clear();
                if (!Directory.Exists(RootFiles[i].Path)) { continue; }

                DirectoryInfo dir = new DirectoryInfo(RootFiles[i].Path);
                FileInfo[] files = dir.GetFiles();
                DirectoryInfo[] folders = dir.GetDirectories();

                for (int j = 0; j < folders.Length; j++)
                {
                    RootFiles[i].Name.Add(folders[j].Name);
                }
                for (int j = 0; j < files.Length; j++)
                {
                    if (!IsSupport(files[j].Extension)) { continue; }
                    RootFiles[i].Name.Add(files[j].Name);
                }
            }
        }
        /// <summary>
        /// 重新加载指定根目录的文件
        /// </summary>
        /// <param name="index">根目录索引号</param>
        /// <param name="jump">是否跳转到该路径</param>
        public static void Reload(int index, bool jump = false)
        {
            if (index < 0 || index >= FileOperate.RootFiles.Count) { return; }
            RootFiles[index].Name.Clear();
            if (!Directory.Exists(RootFiles[index].Path)) { return; }

            ROOT cover = RootFiles[index];
            DirectoryInfo dir = new DirectoryInfo(cover.Path);
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] folders = dir.GetDirectories();

            foreach (DirectoryInfo folder in folders) { cover.Name.Add(folder.Name); }
            foreach (FileInfo file in files)
            { if (FileOperate.IsSupport(FileOperate.getExtension(file.Name))) { cover.Name.Add(file.Name); } }

            RootFiles[index] = cover; if (!jump) { return; }

            Form_Main.config.FolderIndex = index;
            Form_Main.config.FileIndex = 0;
            Form_Main.config.SubIndex = 0;
        }

        /// <summary>
        /// 输出单个文件，失败时返回 FALSE。
        /// </summary>
        /// <param name="fullname">文件的全称</param>
        /// <param name="reason">失败的原因</param>
        /// <returns></returns>
        public static bool Export(string fullname, ref string reason)
        {
            if (!File.Exists(fullname)) { reason = "源文件不存在！"; return false; }

            string exportpath = Form_Main.config.ExportFolder;
            if (!Directory.Exists(exportpath)) { exportpath = getExePath(); }
            
            string destpath = exportpath + "\\" + getName(fullname);
            if (File.Exists(destpath)) { reason = "目标文件已存在！"; return false; }

            try { File.Move(fullname, destpath); } catch { reason = "移动失败！"; return false; }
            reason = ""; return true;
        }
        /// <summary>
        /// 输出复数文件
        /// </summary>
        /// <param name="files">源文件的全称</param>
        /// <param name="indexes">移动失败文件索引号</param>
        /// <param name="reasons">移动失败的原因</param>
        public static void Export(List<string> files, ref List<int> indexes, ref List<string> reasons)
        {
            string reason = "";
            bool success;

            indexes = new List<int>();
            reasons = new List<string>();
            if (files == null || files.Count == 0) { return; }

            for (int i = 0; i < files.Count; i++)
            {
                success = Export(files[i], ref reason);
                if (!success) { indexes.Add(i); reasons.Add(reason); }
            }
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
                //int type = getFileType(file.Extension);
                //if (type == -1) { continue; }
                //if (type == 1) { continue; }
                //Files.Add(file.Name);

                if (IsSupport(file.Extension)) { Files.Add(file.Name); }
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
            return FileSupport.getShowExtension(hideExtension);

            // 以下为历史代码，已弃用。

            if (hideExtension == "") { return ""; }
            if (hideExtension == ".pv1") { return ".jpg"; }
            if (hideExtension == ".pv2") { return ".jpeg"; }
            if (hideExtension == ".pv3") { return ".png"; }
            if (hideExtension == ".pv4") { return ".bmp"; }
            if (hideExtension == ".pv5") { return ".gif"; }
            if (hideExtension == ".pv6") { return ".avi"; }
            if (hideExtension == ".pv7") { return ".mp4"; }
            if (hideExtension == ".pv8") { return ".webm"; }
            if (hideExtension == ".pv9") { return ".zip"; }
            if (hideExtension == ".pv10") { return ".mp3"; }
            //
            if (hideExtension == ".jpg") { return ".jpg"; }
            if (hideExtension == ".jpeg") { return ".jpeg"; }
            if (hideExtension == ".png") { return ".png"; }
            if (hideExtension == ".bmp") { return ".bmp"; }
            if (hideExtension == ".gif") { return ".gif"; }
            if (hideExtension == ".avi") { return ".avi"; }
            if (hideExtension == ".mp4") { return ".mp4"; }
            if (hideExtension == ".webm") { return ".webm"; }
            if (hideExtension == ".zip") { return ".zip"; }
            if (hideExtension == ".mp3") { return ".mp3"; }

            return null;
        }

        /// <summary>
        /// 把显式后缀转换为隐式后缀。
        /// </summary>
        /// <param name="showExtension">显式后缀</param>
        /// <returns></returns>
        public static string getHideExtension(string showExtension)
        {
            return FileSupport.getHideExtension(showExtension);

            // 以下为历史代码，已弃用。

            if (showExtension == "") { return ""; }
            if (showExtension == ".jpg") { return ".pv1"; }
            if (showExtension == ".jpeg") { return ".pv2"; }
            if (showExtension == ".png") { return ".pv3"; }
            if (showExtension == ".bmp") { return ".pv4"; }
            if (showExtension == ".gif") { return ".pv5"; }
            if (showExtension == ".avi") { return ".pv6"; }
            if (showExtension == ".mp4") { return ".pv7"; }
            if (showExtension == ".webm") { return ".pv8"; }
            if (showExtension == ".zip") { return ".pv9"; }
            if (showExtension == ".mp3") { return ".pv10"; }
            //
            if (showExtension == ".pv1") { return ".pv1"; }
            if (showExtension == ".pv2") { return ".pv2"; }
            if (showExtension == ".pv3") { return ".pv3"; }
            if (showExtension == ".pv4") { return ".pv4"; }
            if (showExtension == ".pv5") { return ".pv5"; }
            if (showExtension == ".pv6") { return ".pv6"; }
            if (showExtension == ".pv7") { return ".pv7"; }
            if (showExtension == ".pv8") { return ".pv8"; }
            if (showExtension == ".pv9") { return ".pv9"; }
            if (showExtension == ".pv10") { return ".pv10"; }

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

            // 已经存在，无法重命名回去。
            string newName = path + "\\" + name.Substring(0, name.Length - extension.Length) + getHideExtension(extension);
            if (File.Exists(newName)) { return; }

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

            // 已经存在，无法重命名回去。
            string newName = path + "\\" + name.Substring(0, name.Length - extension.Length) + getShowExtension(extension);
            if (File.Exists(newName)) { return; }

            FileInfo file = new FileInfo(path + "\\" + name);
            file.MoveTo(path + "\\" + name.Substring(0, name.Length - extension.Length) + getShowExtension(extension));
        }

        /// <summary>
        /// 指定序号的路径（根目录）是否存在
        /// </summary>
        /// <param name="FolderIndex">指定路径（根目录）</param>
        /// <returns></returns>
        public static bool ExistFolder(int FolderIndex)
        {
            return FolderIndex >= 0 && FolderIndex < RootFiles.Count;
        }
        /// <summary>
        /// 指定序号的文件是否存在
        /// </summary>
        /// <param name="FolderIndex">指定路径（根目录）</param>
        /// <param name="FileIndex">指定文件</param>
        /// <returns></returns>
        public static bool ExistFile(int FolderIndex, int FileIndex)
        {
            if (!ExistFolder(FolderIndex)) { return false; }
            return FileIndex >= 0 && FileIndex < RootFiles[FolderIndex].Name.Count;
        }
        /// <summary>
        /// 判断该后缀是否为漫画文件后缀（子文件夹）
        /// </summary>
        /// <param name="extension">后缀</param>
        /// <returns></returns>
        public static bool IsComic(string extension)
        {
            int type = FileSupport.getFileType(extension);
            return IsComic(type);
        }
        /// <summary>
        /// 判断该类型的文件是否为漫画文件（子文件夹）
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <returns></returns>
        public static bool IsComic(int type)
        {
            return type == 1 || type == 5;
        }
        /// <summary>
        /// 判断该后缀是否为音频文件后缀
        /// </summary>
        /// <param name="extension">后缀</param>
        /// <returns></returns>
        public static bool IsMusic(string extension)
        {
            return FileSupport.IsMusic(extension);
        }
        /// <summary>
        /// 判断该后缀是否为视频文件后缀
        /// </summary>
        /// <param name="extension">后缀</param>
        /// <returns></returns>
        public static bool IsVideo(string extension)
        {
            return FileSupport.IsVideo(extension);
        }
        /// <summary>
        /// 判断指定类型是不是图片文件
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsPicture(int type)
        {
            return type == 2;
        }
        /// <summary>
        /// 判断指定后缀是不是图片文件
        /// </summary>
        /// <param name="extension">后缀</param>
        /// <returns></returns>
        public static bool IsPicture(string extension)
        {
            return IsPicture(getFileType(extension));
        }
        /// <summary>
        /// 判断指定类型是不是 GIF 文件
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsGif(int type)
        {
            return type == 3;
        }
        /// <summary>
        /// 判断指定后缀是不是 GIF 文件
        /// </summary>
        /// <param name="extension">后缀</param>
        /// <returns></returns>
        public static bool IsGif(string extension)
        {
            return IsGif(getFileType(extension));
        }
        /// <summary>
        /// 判断指定类型是不是 ZIP 文件
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsZip(int type)
        {
            return type == 5;
        }
        /// <summary>
        /// 判断指定后缀是不是 ZIP 文件
        /// </summary>
        /// <param name="extension">后缀</param>
        /// <returns></returns>
        public static bool IsZip(string extension)
        {
            return IsZip(getFileType(extension));
        }
        /// <summary>
        /// 判断指定类型是不是文件夹
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsFolder(int type)
        {
            return type == 1;
        }
        /// <summary>
        /// 判断指定后缀是不是文件夹
        /// </summary>
        /// <param name="extension">后缀</param>
        /// <returns></returns>
        public static bool IsFolder(string extension)
        {
            return IsFolder(getFileType(extension));
        }
        /// <summary>
        /// 判断指定类型是不是流媒体
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsStream(int type)
        {
            return type == 4;
        }
        /// <summary>
        /// 判断指定后缀是不是流媒体
        /// </summary>
        /// <param name="extension">后缀</param>
        /// <returns></returns>
        public static bool IsStream(string extension)
        {
            return IsStream(getFileType(extension));
        }
        /// <summary>
        /// 判断指定类型是不是错误类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsError(int type)
        {
            return type < 0 || type > 5;
        }
        /// <summary>
        /// 判断指定类型是不是不支持类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsUnsupport(int type)
        {
            return type == 0;
        }

        /// <summary>
        /// 获取指定序号的根目录，超出范围会自动更正，不存在返回 NULL
        /// </summary>
        /// <param name="FolderIndex">指定根目录序号</param>
        /// <param name="Fit">是否自动调整索引号</param>
        /// <returns></returns>
        public static string getIndexPath(int FolderIndex, bool Fit = true)
        {
            if (RootFiles.Count == 0) { return null; }

            if (FolderIndex < 0) { if (!Fit) { return null; } FolderIndex = 0; }
            if (FolderIndex >= RootFiles.Count) { if (!Fit) { return null; } FolderIndex = RootFiles.Count - 1; }

            return RootFiles[FolderIndex].Path;
        }
        /// <summary>
        /// 获取指定序号文件的文件名，超出范围会自动更正，不存在返回 NULL
        /// </summary>
        /// <param name="FolderIndex">指定根目录序号</param>
        /// <param name="FileIndex">指定文件序号</param>
        /// <param name="Fit">是否自动调整索引号</param>
        /// <returns></returns>
        public static string getIndexName(int FolderIndex, int FileIndex, bool Fit = true)
        {
            if (RootFiles.Count == 0) { return null; }
            if (FolderIndex < 0) { if (!Fit) { return null; } FolderIndex = 0; }
            if (FolderIndex >= RootFiles.Count) { if (!Fit) { return null; } FolderIndex = RootFiles.Count - 1; }

            if (RootFiles[FolderIndex].Name.Count == 0) { return null; }
            if (FileIndex < 0) { if (!Fit) { return null; } FileIndex = 0; }
            if (FileIndex >= RootFiles[FolderIndex].Name.Count) { if (!Fit) { return null; } FileIndex = RootFiles[FolderIndex].Name.Count - 1; }

            return RootFiles[FolderIndex].Name[FileIndex];
        }

        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////
    }
}
