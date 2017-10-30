using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PictureViewer.Class
{
    class Load
    {
        ///////////////////////////////////////////////////// public attribute ///////////////////////////////////////////////

        /// <summary>
        /// 配置文件输入流
        /// </summary>
        public static StreamReader srCFG = null;

        /// <summary>
        /// 用户设置的历史记录
        /// </summary>
        public static SETTINGS settings;
        /// <summary>
        /// 用户设置的历史记录
        /// </summary>
        public struct SETTINGS
        {
            public bool Form_Main_Hide;
            public bool Form_Main_Hide_L;
            public bool Form_Main_Hide_R;
            public bool Form_Main_Hide_U;
            public bool Form_Main_Hide_D;
            public bool Form_Main_Find_Full;
            public bool Form_Main_Find_Part;
            public bool Form_Main_Find_Same;
            public bool Form_Main_Find_Like;
            public bool Form_Main_Find_Turn;
            public bool Form_Main_UseSmallWindowOpen;
            public int Form_Main_Height;
            public int Form_Main_Width;
            public int Form_Main_Location_X;
            public int Form_Main_Location_Y;
            public bool Form_Main_UseBoard;
            public bool Form_Main_ShapeWindow;
            public int Form_Main_ShapeWindowRate;
            public bool Form_Main_Lock;
            public int Form_Main_MaxWindowSize;
            public int Form_Main_MinWindowSize;

            public int Form_Find_Degree;
            public int Form_Find_Pixes;
        }

        ///////////////////////////////////////////////////// private attribute ///////////////////////////////////////////////

        private static FOUND Found;
        private struct FOUND
        {
            public bool Form_Main_Hide;
            public bool Form_Main_Hide_L;
            public bool Form_Main_Hide_R;
            public bool Form_Main_Hide_U;
            public bool Form_Main_Hide_D;
            public bool Form_Main_Find_Full;
            public bool Form_Main_Find_Part;
            public bool Form_Main_Find_Same;
            public bool Form_Main_Find_Like;
            public bool Form_Main_Find_Turn;
            public bool Form_Main_UseSmallWindowOpen;
            public bool Form_Main_Height;
            public bool Form_Main_Width;
            public bool Form_Main_Location_X;
            public bool Form_Main_Location_Y;
            public bool Form_Main_UseBoard;
            public bool Form_Main_ShapeWindow;
            public bool Form_Main_ShapeWindowRate;
            public bool Form_Main_Lock;
            public bool Form_Main_MaxWindowSize;
            public bool Form_Main_MinWindowSize;

            public bool Form_Find_Degree;
            public bool Form_Find_Pixes;
        }

        ///////////////////////////////////////////////////// public method ///////////////////////////////////////////////

        /// <summary>
        /// 读入配置文件，利用配置文件初始化变量，返回是否读取成功。
        /// </summary>
        /// <returns></returns>
        public static bool Load_CFG()
        {
            #region 初始化变量

            Form_Main.config.ExportFolder = "";
            Form_Main.config.FolderIndex = -1;
            Form_Main.config.FileIndex = -1;
            Form_Main.config.SubIndex = -1;

            FileOperate.RootFiles = new List<FileOperate.ROOT>();
            FileSupport.Initialize();

            #endregion

            #region 读取配置文件

            string fullpath = Form_Main.config.ConfigPath + "\\" + Form_Main.config.ConfigName;
            if (!File.Exists(fullpath)) { SetDefault(); return false; }
            
            try
            {
                string Line; string[] Item, RootPath = new string[0]; srCFG = new StreamReader(fullpath);
                
                while (!srCFG.EndOfStream)
                {
                    Line = srCFG.ReadLine(); if (Line == "") { continue; }
                    Item = Line.Split('=');

                    #region 文本

                    switch (Item[0])
                    {
                        case "ExportFolder": Form_Main.config.ExportFolder = Item.Length == 1 ? "" : Item[1]; break;
                        case "FolderIndex": Form_Main.config.FolderIndex = int.Parse(Item[1]); break;
                        case "FileIndex": Form_Main.config.FileIndex = int.Parse(Item[1]); break;
                        case "SubIndex": Form_Main.config.SubIndex = int.Parse(Item[1]); break;
                        case "RootPath": RootPath = Item.Length == 1 ? new string[0] : Item[1].Split('|'); break;
                        //case "FileType0": FileSupport.I_FileType(0, Item[1]); break;
                        //case "FileType1": FileSupport.I_FileType(1, Item[1]); break;
                        //case "FileType2": FileSupport.I_FileType(2, Item[1]); break;
                        //case "FileType3": FileSupport.I_FileType(3, Item[1]); break;
                        //case "FileType4": FileSupport.I_FileType(4, Item[1]); break;
                        //case "FileType5": FileSupport.I_FileType(5, Item[1]); break;
                        case "Form_Main_Hide":settings.Form_Main_Hide = int.Parse(Item[1]) != 0; Found.Form_Main_Hide = true; break;
                        case "Form_Main_Hide_L": settings.Form_Main_Hide_L = int.Parse(Item[1]) != 0; Found.Form_Main_Hide_L = true; break;
                        case "Form_Main_Hide_R": settings.Form_Main_Hide_R = int.Parse(Item[1]) != 0; Found.Form_Main_Hide_R = true; break;
                        case "Form_Main_Hide_U": settings.Form_Main_Hide_U = int.Parse(Item[1]) != 0; Found.Form_Main_Hide_U = true; break;
                        case "Form_Main_Hide_D": settings.Form_Main_Hide_D = int.Parse(Item[1]) != 0; Found.Form_Main_Hide_D = true; break;
                        case "Form_Main_Find_Full": settings.Form_Main_Find_Full = int.Parse(Item[1]) != 0; Found.Form_Main_Find_Full = true; break;
                        case "Form_Main_Find_Part": settings.Form_Main_Find_Part = int.Parse(Item[1]) != 0; Found.Form_Main_Find_Part = true; break;
                        case "Form_Main_Find_Same": settings.Form_Main_Find_Same = int.Parse(Item[1]) != 0; Found.Form_Main_Find_Same = true; break;
                        case "Form_Main_Find_Like": settings.Form_Main_Find_Like = int.Parse(Item[1]) != 0; Found.Form_Main_Find_Like = true; break;
                        case "Form_Main_Find_Turn": settings.Form_Main_Find_Turn = int.Parse(Item[1]) != 0; Found.Form_Main_Find_Turn = true; break;
                        case "Form_Find_Degree": settings.Form_Find_Degree = int.Parse(Item[1]); Found.Form_Find_Degree = true; break;
                        case "Form_Find_Pixes": settings.Form_Find_Pixes = int.Parse(Item[1]); Found.Form_Find_Pixes = true; break;
                        case "Form_Main_UseSmallWindowOpen": settings.Form_Main_UseSmallWindowOpen = int.Parse(Item[1]) != 0; Found.Form_Main_UseSmallWindowOpen = true; break;
                        case "Form_Main_Height": settings.Form_Main_Height = int.Parse(Item[1]); Found.Form_Main_Height = true; break;
                        case "Form_Main_Width": settings.Form_Main_Width = int.Parse(Item[1]); Found.Form_Main_Width = true; break;
                        case "Form_Main_Location_X": settings.Form_Main_Location_X = int.Parse(Item[1]); Found.Form_Main_Location_X = true; break;
                        case "Form_Main_Location_Y": settings.Form_Main_Location_Y = int.Parse(Item[1]); Found.Form_Main_Location_Y = true; break;
                        case "Form_Main_UseBoard": settings.Form_Main_UseBoard = int.Parse(Item[1]) != 0; Found.Form_Main_UseBoard = true; break;
                        case "Form_Main_ShapeWindow": settings.Form_Main_ShapeWindow = int.Parse(Item[1]) != 0; Found.Form_Main_ShapeWindow = true; break;
                        case "Form_Main_ShapeWindowRate":settings.Form_Main_ShapeWindowRate = int.Parse(Item[1]); Found.Form_Main_ShapeWindowRate = true; break;
                        case "Form_Main_Lock": settings.Form_Main_Lock = int.Parse(Item[1]) != 0; Found.Form_Main_Lock = true; break;
                        case "Form_Main_MaxWindowSize": settings.Form_Main_MaxWindowSize = int.Parse(Item[1]); Found.Form_Main_MaxWindowSize = true; break;
                        case "Form_Main_MinWindowSize": settings.Form_Main_MinWindowSize = int.Parse(Item[1]); Found.Form_Main_MinWindowSize = true; break;
                        default: break;
                    }

                    #endregion

                    #region 根目录

                    for (int i = 0; i < RootPath.Length; i++)
                    {
                        FileOperate.ROOT root = new FileOperate.ROOT();
                        root.Path = RootPath[i];
                        root.Name = new List<string>();
                        if (!Directory.Exists(root.Path)) { continue; }

                        DirectoryInfo dir = new DirectoryInfo(root.Path);
                        FileInfo[] files = dir.GetFiles();
                        DirectoryInfo[] folders = dir.GetDirectories();

                        foreach (DirectoryInfo folder in folders) { root.Name.Add(folder.Name); }
                        foreach (FileInfo file in files)
                        { if (FileOperate.IsSupport(FileOperate.getExtension(file.Name))) { root.Name.Add(file.Name); } }

                        FileOperate.RootFiles.Add(root);
                    }
                    RootPath = new string[0];

                    #endregion
                }

                #region 去除重复根目录

                for (int i = FileOperate.RootFiles.Count - 1; i >= 0; i--)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (FileOperate.RootFiles[i].Path != FileOperate.RootFiles[j].Path) { continue; }
                        FileOperate.RootFiles.RemoveAt(i); break;
                    }
                }

                #endregion
            }
            catch
            {
                SetDefault();
                srCFG.Close();
                //System.Windows.Forms.MessageBox.Show("文件（pv.ini）已损坏！", "提示");
                return false;
            }

            #endregion

            #region 设置默认值，关闭文件流

            SetDefault(); srCFG.Close();
            return true;

            #endregion
        }

        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////

        private static void SetDefault()
        {
            #region 没有历史信息，默认设置

            settings.Form_Main_Hide = settings.Form_Main_Hide_U && settings.Form_Main_Hide_D && settings.Form_Main_Hide_L && settings.Form_Main_Hide_R;

            if (!Found.Form_Main_Find_Full && !Found.Form_Main_Find_Part) { settings.Form_Main_Find_Part = false; }
            if (!Found.Form_Main_Find_Full && Found.Form_Main_Find_Part) { settings.Form_Main_Find_Full = !settings.Form_Main_Find_Part; }
            if (Found.Form_Main_Find_Full && !Found.Form_Main_Find_Part) { settings.Form_Main_Find_Part = !settings.Form_Main_Find_Full; }
            if (Found.Form_Main_Find_Full && Found.Form_Main_Find_Part) { settings.Form_Main_Find_Full = !settings.Form_Main_Find_Part; }

            if (!Found.Form_Main_Find_Same && !Found.Form_Main_Find_Like) { settings.Form_Main_Find_Like = false; }
            if (!Found.Form_Main_Find_Same && Found.Form_Main_Find_Like) { settings.Form_Main_Find_Same = !settings.Form_Main_Find_Like; }
            if (Found.Form_Main_Find_Same && !Found.Form_Main_Find_Like) { settings.Form_Main_Find_Like = !settings.Form_Main_Find_Same; }
            if (Found.Form_Main_Find_Same && Found.Form_Main_Find_Like) { settings.Form_Main_Find_Same = !settings.Form_Main_Find_Like; }

            if (!Found.Form_Main_Find_Turn) { settings.Form_Main_Find_Turn = true; }

            if (!Found.Form_Find_Degree) { settings.Form_Find_Degree = 80; }
            if (!Found.Form_Find_Pixes) { settings.Form_Find_Pixes = 100; }

            int sh = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            int sw = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

            if (!Found.Form_Main_UseSmallWindowOpen) { settings.Form_Main_UseSmallWindowOpen = false; }
            if (!Found.Form_Main_Height) { settings.Form_Main_Height = sh * 50 / 100; }
            if (!Found.Form_Main_Width) { settings.Form_Main_Width = sw * 50 / 100; }
            if (settings.Form_Main_UseSmallWindowOpen)
            { settings.Form_Main_Height = 150; settings.Form_Main_Width = 200; }

            if (!Found.Form_Main_UseBoard) { settings.Form_Main_UseBoard = false; }
            if (!Found.Form_Main_ShapeWindow) { settings.Form_Main_ShapeWindow = true; }
            if (!Found.Form_Main_ShapeWindowRate) { settings.Form_Main_ShapeWindowRate = 80; }
            if (!Found.Form_Main_Lock) { settings.Form_Main_Lock = false; }

            if (!Found.Form_Main_Location_X) { settings.Form_Main_Location_X = sw * 25 / 100; }
            if (!Found.Form_Main_Location_Y) { settings.Form_Main_Location_Y = sh * 25 / 100; }

            if (!Found.Form_Main_MaxWindowSize) { settings.Form_Main_MaxWindowSize = 90; }
            if (!Found.Form_Main_MinWindowSize) { settings.Form_Main_MinWindowSize = 10; }

            #endregion
        }
    }
}
