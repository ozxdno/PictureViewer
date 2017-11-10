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
            public bool Form_Main_Tip;
            public bool Form_Main_Play_Forward;
            public bool Form_Main_Play_Backward;
            public bool Form_Main_Play_TotalRoots;
            public bool Form_Main_Play_Root;
            public bool Form_Main_Play_Subroot;
            public bool Form_Main_Play_Picture;
            public bool Form_Main_Play_Gif;
            public bool Form_Main_Play_Music;
            public bool Form_Main_Play_Video;
            public bool Form_Main_Play_Single;
            public bool Form_Main_Play_Order;
            public bool Form_Main_Play_Circle;
            public bool Form_Main_Play_Rand;
            public int Form_Main_Play_ShowTime;

            public int Form_Find_Degree;
            public int Form_Find_Pixes;

            public int FastKey_Main_U;
            public int FastKey_Main_D;
            public int FastKey_Main_L;
            public int FastKey_Main_R;
            public int FastKey_Main_PageU;
            public int FastKey_Main_PageD;
            public int FastKey_Main_Enter;
            public int FastKey_Main_Export;
            public int FastKey_Main_OpenExport;
            public int FastKey_Main_OpenRoot;
            public int FastKey_Main_OpenComic;
            public int FastKey_Main_OpenCurrent;
            public int FastKey_Main_Password;
            public int FastKey_Main_Esc;
            public int FastKey_Main_Board;
            public int FastKey_Main_Rotate;
            public int FastKey_Find_Esc;
            public int FastKey_Find_U;
            public int FastKey_Find_D;
            public int FastKey_Find_L;
            public int FastKey_Find_R;
            public int FastKey_Find_Export;
            public int FastKey_Search_Esc;
            public int FastKey_Search_Enter;
            public int FastKey_Input_Enter;
            public int FastKey_Image_Esc;
            public int FastKey_Image_Enter;
            public int FastKey_Image_Rotate;
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
            public bool Form_Main_Tip;
            public bool Form_Main_Play_Forward;
            public bool Form_Main_Play_Backward;
            public bool Form_Main_Play_TotalRoots;
            public bool Form_Main_Play_Root;
            public bool Form_Main_Play_Subroot;
            public bool Form_Main_Play_Picture;
            public bool Form_Main_Play_Gif;
            public bool Form_Main_Play_Music;
            public bool Form_Main_Play_Video;
            public bool Form_Main_Play_Single;
            public bool Form_Main_Play_Order;
            public bool Form_Main_Play_Circle;
            public bool Form_Main_Play_Rand;
            public bool Form_Main_Play_ShowTime;

            public bool Form_Find_Degree;
            public bool Form_Find_Pixes;

            public bool FastKey_Main_U;
            public bool FastKey_Main_D;
            public bool FastKey_Main_L;
            public bool FastKey_Main_R;
            public bool FastKey_Main_PageU;
            public bool FastKey_Main_PageD;
            public bool FastKey_Main_Enter;
            public bool FastKey_Main_Export;
            public bool FastKey_Main_OpenExport;
            public bool FastKey_Main_OpenRoot;
            public bool FastKey_Main_OpenComic;
            public bool FastKey_Main_OpenCurrent;
            public bool FastKey_Main_Password;
            public bool FastKey_Main_Esc;
            public bool FastKey_Main_Board;
            public bool FastKey_Main_Rotate;
            public bool FastKey_Find_Esc;
            public bool FastKey_Find_U;
            public bool FastKey_Find_D;
            public bool FastKey_Find_L;
            public bool FastKey_Find_R;
            public bool FastKey_Find_Export;
            public bool FastKey_Search_Esc;
            public bool FastKey_Search_Enter;
            public bool FastKey_Input_Enter;
            public bool FastKey_Image_Esc;
            public bool FastKey_Image_Enter;
            public bool FastKey_Image_Rotate;
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
            List<string> RootPath = new List<string>();

            List<string> SupportPicture = new List<string>();
            List<string> SupportGif = new List<string>();
            List<string> SupportMusic = new List<string>();
            List<string> SupportVideo = new List<string>();
            List<string> SupportZip = new List<string>();
            
            #endregion

            #region 读取配置文件

            string fullpath = Form_Main.config.ConfigPath + "\\" + Form_Main.config.ConfigName;
            bool existCFG = File.Exists(fullpath);
            if (existCFG) { srCFG = new StreamReader(fullpath); }

            string Line;
            string[] Item = new string[2];

            while (existCFG && !srCFG.EndOfStream)
            {
                Line = srCFG.ReadLine(); if (Line == "") { continue; }
                int cut = Line.IndexOf('=');
                Item[0] = cut == -1 ? Line : Line.Substring(0, cut);
                Item[1] = cut == -1 ? "" : Line.Substring(cut + 1);

                #region 文本

                switch (Item[0])
                {
                    case "ExportFolder": ToDirectory(Item[1], ref Form_Main.config.ExportFolder); break;
                    case "FolderIndex": ToInt(Item[1], ref Form_Main.config.FolderIndex); break;
                    case "FileIndex": ToInt(Item[1], ref Form_Main.config.FileIndex); break;
                    case "SubIndex": ToInt(Item[1], ref Form_Main.config.SubIndex); break;
                    case "RootPath": ToStringEx(Item[1], ref RootPath); break;
                    case "SupportPicture": ToStringEx(Item[1], ref SupportPicture); break;
                    case "SupportGif": ToStringEx(Item[1], ref SupportGif); break;
                    case "SupportMusic": ToStringEx(Item[1], ref SupportMusic); break;
                    case "SupportVideo": ToStringEx(Item[1], ref SupportVideo); break;
                    case "SupportZip": ToStringEx(Item[1], ref SupportZip); break;
                    case "Form_Main_Hide": Found.Form_Main_Hide = ToBool(Item[1], ref settings.Form_Main_Hide); break;
                    case "Form_Main_Hide_L": Found.Form_Main_Hide_L = ToBool(Item[1], ref settings.Form_Main_Hide_L); break;
                    case "Form_Main_Hide_R": Found.Form_Main_Hide_R = ToBool(Item[1], ref settings.Form_Main_Hide_R); break;
                    case "Form_Main_Hide_U": Found.Form_Main_Hide_U = ToBool(Item[1], ref settings.Form_Main_Hide_U); break;
                    case "Form_Main_Hide_D": Found.Form_Main_Hide_D = ToBool(Item[1], ref settings.Form_Main_Hide_D); break;
                    case "Form_Main_Find_Full": Found.Form_Main_Find_Full = ToBool(Item[1], ref settings.Form_Main_Find_Full); break;
                    case "Form_Main_Find_Part": Found.Form_Main_Find_Part = ToBool(Item[1], ref settings.Form_Main_Find_Part); break;
                    case "Form_Main_Find_Same": Found.Form_Main_Find_Same = ToBool(Item[1], ref settings.Form_Main_Find_Same); break;
                    case "Form_Main_Find_Like": Found.Form_Main_Find_Like = ToBool(Item[1], ref settings.Form_Main_Find_Like); break;
                    case "Form_Main_Find_Turn": Found.Form_Main_Find_Turn = ToBool(Item[1], ref settings.Form_Main_Find_Turn); break;
                    case "Form_Find_Degree": Found.Form_Find_Degree = ToInt(Item[1], ref settings.Form_Find_Degree); break;
                    case "Form_Find_Pixes": Found.Form_Find_Pixes = ToInt(Item[1], ref settings.Form_Find_Pixes); break;
                    case "Form_Main_UseSmallWindowOpen": Found.Form_Main_UseSmallWindowOpen = ToBool(Item[1], ref settings.Form_Main_UseSmallWindowOpen); break;
                    case "Form_Main_Height": Found.Form_Main_Height = ToInt(Item[1], ref settings.Form_Main_Height); break;
                    case "Form_Main_Width": Found.Form_Main_Width = ToInt(Item[1], ref settings.Form_Main_Width); break;
                    case "Form_Main_Location_X": Found.Form_Main_Location_X = ToInt(Item[1], ref settings.Form_Main_Location_X); break;
                    case "Form_Main_Location_Y": Found.Form_Main_Location_Y = ToInt(Item[1], ref settings.Form_Main_Location_Y); break;
                    case "Form_Main_UseBoard": Found.Form_Main_UseBoard = ToBool(Item[1], ref settings.Form_Main_UseBoard); break;
                    case "Form_Main_ShapeWindow": Found.Form_Main_ShapeWindow = ToBool(Item[1], ref settings.Form_Main_ShapeWindow); break;
                    case "Form_Main_ShapeWindowRate": Found.Form_Main_ShapeWindowRate = ToInt(Item[1], ref settings.Form_Main_ShapeWindowRate); break;
                    case "Form_Main_Lock": Found.Form_Main_Lock = ToBool(Item[1], ref settings.Form_Main_Lock); break;
                    case "Form_Main_MaxWindowSize": Found.Form_Main_MaxWindowSize = ToInt(Item[1], ref settings.Form_Main_MaxWindowSize); break;
                    case "Form_Main_MinWindowSize": Found.Form_Main_MinWindowSize = ToInt(Item[1], ref settings.Form_Main_MinWindowSize); break;
                    case "Form_Main_Tip": Found.Form_Main_Tip = ToBool(Item[1], ref settings.Form_Main_Tip); break;
                    case "Form_Main_Play_Forward": Found.Form_Main_Play_Forward = ToBool(Item[1], ref settings.Form_Main_Play_Forward); break;
                    case "Form_Main_Play_Backward": Found.Form_Main_Play_Backward = ToBool(Item[1], ref settings.Form_Main_Play_Backward); break;
                    case "Form_Main_Play_TotalRoots": Found.Form_Main_Play_TotalRoots = ToBool(Item[1], ref settings.Form_Main_Play_TotalRoots); break;
                    case "Form_Main_Play_Root": Found.Form_Main_Play_Root = ToBool(Item[1], ref settings.Form_Main_Play_Root); break;
                    case "Form_Main_Play_Subroot": Found.Form_Main_Play_Subroot = ToBool(Item[1], ref settings.Form_Main_Play_Subroot); break;
                    case "Form_Main_Play_Picture": Found.Form_Main_Play_Picture = ToBool(Item[1], ref settings.Form_Main_Play_Picture); break;
                    case "Form_Main_Play_Gif": Found.Form_Main_Play_Gif = ToBool(Item[1], ref settings.Form_Main_Play_Gif); break;
                    case "Form_Main_Play_Music": Found.Form_Main_Play_Music = ToBool(Item[1], ref settings.Form_Main_Play_Music); break;
                    case "Form_Main_Play_Video": Found.Form_Main_Play_Video = ToBool(Item[1], ref settings.Form_Main_Play_Video); break;
                    case "Form_Main_Play_Single": Found.Form_Main_Play_Single = ToBool(Item[1], ref settings.Form_Main_Play_Single); break;
                    case "Form_Main_Play_Order": Found.Form_Main_Play_Order = ToBool(Item[1], ref settings.Form_Main_Play_Order); break;
                    case "Form_Main_Play_Circle": Found.Form_Main_Play_Circle = ToBool(Item[1], ref settings.Form_Main_Play_Circle); break;
                    case "Form_Main_Play_Rand": Found.Form_Main_Play_Rand = ToBool(Item[1], ref settings.Form_Main_Play_Rand); break;
                    case "Form_Main_Play_ShowTime": Found.Form_Main_Play_ShowTime = ToInt(Item[1], ref settings.Form_Main_Play_ShowTime); break;
                    case "FastKey_Find_Esc": Found.FastKey_Find_Esc = ToInt(Item[1], ref settings.FastKey_Find_Esc); break;
                    case "FastKey_Find_Export": Found.FastKey_Find_Export = ToInt(Item[1], ref settings.FastKey_Find_Export); break;
                    case "FastKey_Find_L": Found.FastKey_Find_L = ToInt(Item[1], ref settings.FastKey_Find_L); break;
                    case "FastKey_Find_R": Found.FastKey_Find_R = ToInt(Item[1], ref settings.FastKey_Find_R); break;
                    case "FastKey_Find_U": Found.FastKey_Find_U = ToInt(Item[1], ref settings.FastKey_Find_U); break;
                    case "FastKey_Find_D": Found.FastKey_Find_D = ToInt(Item[1], ref settings.FastKey_Find_D); break;
                    case "FastKey_Main_D": Found.FastKey_Main_D = ToInt(Item[1], ref settings.FastKey_Main_D); break;
                    case "FastKey_Main_L": Found.FastKey_Main_L = ToInt(Item[1], ref settings.FastKey_Main_L); break;
                    case "FastKey_Main_PageD": Found.FastKey_Main_PageD = ToInt(Item[1], ref settings.FastKey_Main_PageD); break;
                    case "FastKey_Main_PageU": Found.FastKey_Main_PageU = ToInt(Item[1], ref settings.FastKey_Main_PageU); break;
                    case "FastKey_Main_R": Found.FastKey_Main_R = ToInt(Item[1], ref settings.FastKey_Main_R); break;
                    case "FastKey_Main_U": Found.FastKey_Main_U = ToInt(Item[1], ref settings.FastKey_Main_U); break;
                    case "FastKey_Main_Board": Found.FastKey_Main_Board = ToInt(Item[1], ref settings.FastKey_Main_Board); break;
                    case "FastKey_Main_Enter": Found.FastKey_Main_Enter = ToInt(Item[1], ref settings.FastKey_Main_Enter); break;
                    case "FastKey_Main_Esc": Found.FastKey_Main_Esc = ToInt(Item[1], ref settings.FastKey_Main_Esc); break;
                    case "FastKey_Main_Export": Found.FastKey_Main_Export = ToInt(Item[1], ref settings.FastKey_Main_Export); break;
                    case "FastKey_Main_OpenComic": Found.FastKey_Main_OpenComic = ToInt(Item[1], ref settings.FastKey_Main_OpenComic); break;
                    case "FastKey_Main_OpenCurrent": Found.FastKey_Main_OpenCurrent = ToInt(Item[1], ref settings.FastKey_Main_OpenCurrent); break;
                    case "FastKey_Main_OpenExport": Found.FastKey_Main_OpenExport = ToInt(Item[1], ref settings.FastKey_Main_OpenExport); break;
                    case "FastKey_Main_OpenRoot": Found.FastKey_Main_OpenRoot = ToInt(Item[1], ref settings.FastKey_Main_OpenRoot); break;
                    case "FastKey_Main_Password": Found.FastKey_Main_Password = ToInt(Item[1], ref settings.FastKey_Main_Password); break;
                    case "FastKey_Main_Rotate": Found.FastKey_Main_Rotate = ToInt(Item[1], ref settings.FastKey_Main_Rotate); break;
                    case "FastKey_Search_Esc": Found.FastKey_Search_Esc = ToInt(Item[1], ref settings.FastKey_Search_Esc); break;
                    case "FastKey_Search_Enter": Found.FastKey_Search_Enter = ToInt(Item[1], ref settings.FastKey_Search_Enter); break;
                    case "FastKey_Input_Enter": Found.FastKey_Input_Enter = ToInt(Item[1], ref settings.FastKey_Input_Enter); break;
                    case "FastKey_Image_Esc": Found.FastKey_Image_Esc = ToInt(Item[1], ref settings.FastKey_Image_Esc); break;
                    case "FastKey_Image_Enter": Found.FastKey_Image_Enter = ToInt(Item[1], ref settings.FastKey_Image_Enter); break;
                    case "FastKey_Image_Rotate": Found.FastKey_Image_Rotate = ToInt(Item[1], ref settings.FastKey_Image_Rotate); break;
                    default: break;
                }

                #endregion
            }

            #endregion

            #region 额外支持的文件

            for (int i = 0; i < SupportPicture.Count; i++)
            {
                string extension = SupportPicture[i];
                if (extension.Length == 0) { continue; }
                if (extension[0] != '.') { extension = "." + extension; }

                FileSupport.ExtraExtensions.Add(extension);
                FileSupport.ExtraTypes.Add(2);
                FileSupport.ExtraIsMusic.Add(false);
                FileSupport.ExtraIsVideo.Add(false);
            }
            for (int i = 0; i < SupportGif.Count; i++)
            {
                string extension = SupportGif[i];
                if (extension.Length == 0) { continue; }
                if (extension[0] != '.') { extension = "." + extension; }

                FileSupport.ExtraExtensions.Add(extension);
                FileSupport.ExtraTypes.Add(3);
                FileSupport.ExtraIsMusic.Add(false);
                FileSupport.ExtraIsVideo.Add(false);
            }
            for (int i = 0; i < SupportMusic.Count; i++)
            {
                string extension = SupportMusic[i];
                if (extension.Length == 0) { continue; }
                if (extension[0] != '.') { extension = "." + extension; }

                FileSupport.ExtraExtensions.Add(extension);
                FileSupport.ExtraTypes.Add(4);
                FileSupport.ExtraIsMusic.Add(true);
                FileSupport.ExtraIsVideo.Add(false);
            }
            for (int i = 0; i < SupportVideo.Count; i++)
            {
                string extension = SupportVideo[i];
                if (extension.Length == 0) { continue; }
                if (extension[0] != '.') { extension = "." + extension; }

                FileSupport.ExtraExtensions.Add(extension);
                FileSupport.ExtraTypes.Add(4);
                FileSupport.ExtraIsMusic.Add(false);
                FileSupport.ExtraIsVideo.Add(true);
            }
            for (int i = 0; i < SupportZip.Count; i++)
            {
                string extension = SupportZip[i];
                if (extension.Length == 0) { continue; }
                if (extension[0] != '.') { extension = "." + extension; }

                FileSupport.ExtraExtensions.Add(extension);
                FileSupport.ExtraTypes.Add(5);
                FileSupport.ExtraIsMusic.Add(false);
                FileSupport.ExtraIsVideo.Add(false);
            }

            FileSupport.Initialize();

            #endregion

            #region 根目录

            for (int i = 0; i < RootPath.Count; i++)
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

            #endregion

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
            int side = Math.Min(sh * 10 / 100, sw * 10 / 100);

            if (!Found.Form_Main_UseSmallWindowOpen) { settings.Form_Main_UseSmallWindowOpen = false; }
            if (!Found.Form_Main_Height) { settings.Form_Main_Height = sh * 80 / 100; }
            if (!Found.Form_Main_Width) { settings.Form_Main_Width = sw * 80 / 100; }
            if (settings.Form_Main_UseSmallWindowOpen)
            { settings.Form_Main_Height = settings.Form_Main_Width = side; }

            if (!Found.Form_Main_UseBoard) { settings.Form_Main_UseBoard = false; }
            if (!Found.Form_Main_ShapeWindow) { settings.Form_Main_ShapeWindow = true; }
            if (!Found.Form_Main_ShapeWindowRate) { settings.Form_Main_ShapeWindowRate = 80; }
            if (!Found.Form_Main_Lock) { settings.Form_Main_Lock = false; }

            if (!Found.Form_Main_Location_X) { settings.Form_Main_Location_X = sw * 25 / 100; }
            if (!Found.Form_Main_Location_Y) { settings.Form_Main_Location_Y = sh * 25 / 100; }

            if (!Found.Form_Main_MaxWindowSize) { settings.Form_Main_MaxWindowSize = 90; }
            if (!Found.Form_Main_MinWindowSize) { settings.Form_Main_MinWindowSize = 10; }

            if (!Found.Form_Main_Tip) { settings.Form_Main_Tip = !settings.Form_Main_UseBoard; }

            bool existOrder = settings.Form_Main_Play_Forward || settings.Form_Main_Play_Backward || settings.Form_Main_Play_Rand;
            if (!Found.Form_Main_Play_Forward) { settings.Form_Main_Play_Forward = existOrder ? false : true; }
            if (!Found.Form_Main_Play_Backward) { settings.Form_Main_Play_Backward = false; }
            if (!Found.Form_Main_Play_Rand) { settings.Form_Main_Play_Rand = false; }
            
            bool existPlayFolder = Found.Form_Main_Play_TotalRoots || Found.Form_Main_Play_Root || Found.Form_Main_Play_Subroot;
            if (!Found.Form_Main_Play_TotalRoots) { settings.Form_Main_Play_TotalRoots = false; }
            if (!Found.Form_Main_Play_Root) { settings.Form_Main_Play_Root = existPlayFolder ? false : true; }
            if (!Found.Form_Main_Play_Subroot) { settings.Form_Main_Play_Subroot = false; }
            
            bool existPlayType = settings.Form_Main_Play_Picture || settings.Form_Main_Play_Gif || settings.Form_Main_Play_Music || settings.Form_Main_Play_Video;
            if (!Found.Form_Main_Play_Picture) { settings.Form_Main_Play_Picture = existPlayType ? false : true; }
            if (!Found.Form_Main_Play_Gif) { settings.Form_Main_Play_Gif = false; }
            if (!Found.Form_Main_Play_Music) { settings.Form_Main_Play_Music = false; }
            if (!Found.Form_Main_Play_Video) { settings.Form_Main_Play_Video = false; }
            
            bool existPlayMode = settings.Form_Main_Play_Single || settings.Form_Main_Play_Order || settings.Form_Main_Play_Circle;
            if (!Found.Form_Main_Play_Single) { settings.Form_Main_Play_Single = false; }
            if (!Found.Form_Main_Play_Order) { settings.Form_Main_Play_Order = false; }
            if (!Found.Form_Main_Play_Circle) { settings.Form_Main_Play_Circle = existPlayType ? false : true; }
            
            if (!Found.Form_Main_Play_ShowTime) { settings.Form_Main_Play_ShowTime = 50; }

            if (!Found.FastKey_Main_Board) { settings.FastKey_Main_Board = (int)ConsoleKey.A; }
            if (!Found.FastKey_Main_D) { settings.FastKey_Main_D = (int)ConsoleKey.DownArrow; }
            if (!Found.FastKey_Main_Enter) { settings.FastKey_Main_Enter = (int)ConsoleKey.Enter; }
            if (!Found.FastKey_Main_Esc) { settings.FastKey_Main_Esc = (int)ConsoleKey.Escape; }
            if (!Found.FastKey_Main_Export) { settings.FastKey_Main_Export = (int)ConsoleKey.Delete; }
            if (!Found.FastKey_Main_L) { settings.FastKey_Main_L = (int)ConsoleKey.LeftArrow; }
            if (!Found.FastKey_Main_OpenComic) { settings.FastKey_Main_OpenComic = (int)ConsoleKey.D4; }
            if (!Found.FastKey_Main_OpenCurrent) { settings.FastKey_Main_OpenCurrent = (int)ConsoleKey.D3; }
            if (!Found.FastKey_Main_OpenExport) { settings.FastKey_Main_OpenExport = (int)ConsoleKey.D1; }
            if (!Found.FastKey_Main_OpenRoot) { settings.FastKey_Main_OpenRoot = (int)ConsoleKey.D2; }
            if (!Found.FastKey_Main_PageD) { settings.FastKey_Main_PageD = (int)ConsoleKey.PageDown; }
            if (!Found.FastKey_Main_PageU) { settings.FastKey_Main_PageU = (int)ConsoleKey.PageUp; }
            if (!Found.FastKey_Main_Password) { settings.FastKey_Main_Password = (int)ConsoleKey.P; }
            if (!Found.FastKey_Main_R) { settings.FastKey_Main_R = (int)ConsoleKey.RightArrow; }
            if (!Found.FastKey_Main_U) { settings.FastKey_Main_U = (int)ConsoleKey.UpArrow; }
            if (!Found.FastKey_Main_Rotate) { settings.FastKey_Main_Rotate = (int)ConsoleKey.R; }

            if (!Found.FastKey_Find_Esc) { settings.FastKey_Find_Esc = (int)ConsoleKey.Escape; }
            if (!Found.FastKey_Find_Export) { settings.FastKey_Find_Export = (int)ConsoleKey.Delete; }
            if (!Found.FastKey_Find_U) { settings.FastKey_Find_U = (int)ConsoleKey.UpArrow; }
            if (!Found.FastKey_Find_D) { settings.FastKey_Find_D = (int)ConsoleKey.DownArrow; }
            if (!Found.FastKey_Find_L) { settings.FastKey_Find_L = (int)ConsoleKey.LeftArrow; }
            if (!Found.FastKey_Find_R) { settings.FastKey_Find_R = (int)ConsoleKey.RightArrow; }

            if (!Found.FastKey_Search_Esc) { settings.FastKey_Search_Esc = (int)ConsoleKey.Escape; }
            if (!Found.FastKey_Search_Enter) { settings.FastKey_Search_Enter = (int)ConsoleKey.Enter; }

            if (!Found.FastKey_Input_Enter) { settings.FastKey_Input_Enter = (int)ConsoleKey.Enter; }

            if (!Found.FastKey_Image_Esc) { settings.FastKey_Image_Esc = (int)ConsoleKey.Escape; }
            if (!Found.FastKey_Image_Enter) { settings.FastKey_Image_Enter = (int)ConsoleKey.Enter; }
            if (!Found.FastKey_Image_Rotate) { settings.FastKey_Image_Rotate = (int)ConsoleKey.R; }

            #endregion

            #region 有历史信息，但需要修改

            settings.Form_Find_Pixes = settings.Form_Find_Pixes / 2 * 2;

            #endregion
        }

        private static bool ToDirectory(string str, ref string ret)
        {
            bool exist = Directory.Exists(str);
            ret = exist ? str : "";
            return exist;
        }
        private static bool ToFile(string str, ref string ret)
        {
            bool exist = File.Exists(str);
            ret = exist ? str : "";
            return exist;
        }
        private static bool ToBool(string str, ref bool ret)
        {
            try { ret = int.Parse(str) != 0; return true; } catch { return false; }
        }
        private static bool ToInt(string str, ref int ret)
        {
            try { ret = int.Parse(str); return true; } catch { return false; }
        }
        private static bool ToDouble(string str, ref double ret)
        {
            try { ret = double.Parse(str); return true; } catch { return false; }
        }
        private static bool ToString(string str, ref List<string> ret)
        {
            try { ret = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList(); return true; }
            catch { return false; }
        }
        private static bool ToString(string str, ref string[] ret)
        {
            try { ret = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries); return true; }
            catch { return false; }
        }
        private static bool ToStringEx(string str, ref List<string> ret)
        {
            List<string> temp = new List<string>();
            try {
                temp = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (ret == null) { ret = new List<string>(); }
                ret.InsertRange(ret.Count, temp);
                return true;
            }
            catch { return false; }
        }
        private static bool ToStringEx(string str, ref string[] ret)
        {
            string[] temp = new string[0];
            try {
                temp = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (ret == null) { ret = new string[0]; }
                List<string> temp2 = ret.ToList();
                temp2.InsertRange(temp.Length, temp.ToList());
                ret = temp2.ToArray();
                return true;
            }
            catch { return false; }
        }
    }
}
