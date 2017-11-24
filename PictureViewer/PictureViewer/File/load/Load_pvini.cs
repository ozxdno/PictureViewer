using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PictureViewer.Files
{
    class Load_pvini
    {
        /// <summary>
        /// 用单独的线程加载文件
        /// </summary>
        public static void thread()
        {
            while (Config.Loading != null && Config.Loading.ThreadState == System.Threading.ThreadState.Running) ;

            Config.Loading = new System.Threading.Thread(load);
            Config.Loading.Start();
        }

        /// <summary>
        /// 读入文件
        /// </summary>
        /// <returns></returns>
        public static void load()
        {
            try
            {
                StreamReader sr = new StreamReader(Config.ConstFiles.pviniFull);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    int indexEq = line.IndexOf('='); if (indexEq == -1) { continue; }
                    string item = line.Substring(0, indexEq);
                    string vars = line.Substring(indexEq + 1);
                    if (item.Length == 0 || vars.Length == 0) { continue; }

                    #region 文本

                    switch (item)
                    {
                        case "ExportFolder": Config.ExportPath = Load_vars.ToDirectory(vars); break;
                        case "FolderIndex": MainForm.Config.TreeIndex.Folder = Load_vars.ToInt(vars); break;
                        case "FileIndex": MainForm.Config.TreeIndex.File = Load_vars.ToInt(vars); break;
                        case "SubIndex": MainForm.Config.TreeIndex.Sub = Load_vars.ToInt(vars); break;
                        case "RootPath": Config.RootPathes.AddRange(Load_vars.ToStringList(vars)); break;
                        case "SupportPicture": Load_vars.ToSupport(Support.TYPE.PICTURE, vars); break;
                        case "SupportGif": Load_vars.ToSupport(Support.TYPE.GIF, vars); break;
                        case "SupportMusic": Load_vars.ToSupport(Support.TYPE.MUSIC, vars); break;
                        case "SupportVideo": Load_vars.ToSupport(Support.TYPE.VIDEO, vars); break;
                        case "Form_Main_HideL": MainForm.Config.HideL = Load_vars.ToBool(vars); break;
                        case "Form_Main_HideR": MainForm.Config.HideR = Load_vars.ToBool(vars); break;
                        case "Form_Main_HideU": MainForm.Config.HideU = Load_vars.ToBool(vars); break;
                        case "Form_Main_HideD": MainForm.Config.HideD = Load_vars.ToBool(vars); break;
                        case "Form_Main_Find_Full": FindForm.Config.Find_Full = Load_vars.ToBool(vars); break;
                        case "Form_Main_Find_Part": FindForm.Config.Find_Part = Load_vars.ToBool(vars); break;
                        case "Form_Main_Find_Same": FindForm.Config.Find_Same = Load_vars.ToBool(vars); break;
                        case "Form_Main_Find_Like": FindForm.Config.Find_Like = Load_vars.ToBool(vars); break;
                        case "Form_Main_Find_Turn": FindForm.Config.Find_Turn = Load_vars.ToBool(vars); break;
                        case "Form_Find_Degree": FindForm.Config.Degree = Load_vars.ToInt(vars); break;
                        case "Form_Find_Pixes": FindForm.Config.Pixes = Load_vars.ToInt(vars); break;
                        case "Form_Main_UseSmallWindowOpen": MainForm.Config.SmallWindow = Load_vars.ToBool(vars); break;
                        case "Form_Main_Height": MainForm.Config.Height = Load_vars.ToInt(vars); break;
                        case "Form_Main_Width": MainForm.Config.Width = Load_vars.ToInt(vars); break;
                        case "Form_Main_Location_X": MainForm.Config.LocationX = Load_vars.ToInt(vars); break;
                        case "Form_Main_Location_Y": MainForm.Config.LocationY = Load_vars.ToInt(vars); break;
                        case "Form_Main_UseBoard": MainForm.Config.UseBoard = Load_vars.ToBool(vars); break;
                        case "Form_Main_ShapeWindow": MainForm.Config.ShapeWindow = Load_vars.ToBool(vars); break;
                        case "Form_Main_ShapeWindowRate": MainForm.Config.ShapeWindowRate = Load_vars.ToDouble(vars); break;
                        case "Form_Main_Lock": MainForm.Config.LockOpreate = Load_vars.ToBool(vars); break;
                        case "Form_Main_MaxWindowSize": MainForm.Config.MaxRate = Load_vars.ToDouble(vars); break;
                        case "Form_Main_MinWindowSize": MainForm.Config.MinRate = Load_vars.ToDouble(vars); break;
                        case "Form_Main_Tip": MainForm.Config.Tip = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_Forward": MainForm.Config.Play.Forward = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_Backward": MainForm.Config.Play.Backward = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_TotalRoots": MainForm.Config.Play.Total = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_Root": MainForm.Config.Play.Root = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_Subroot": MainForm.Config.Play.Subroot = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_Picture": MainForm.Config.Play.Picture = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_Gif": MainForm.Config.Play.Gif = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_Music": MainForm.Config.Play.Music = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_Video": MainForm.Config.Play.Video = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_Single": MainForm.Config.Play.Single = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_Order": MainForm.Config.Play.Order = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_Circle": MainForm.Config.Play.Circle = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_Rand": MainForm.Config.Play.Rand = Load_vars.ToBool(vars); break;
                        case "Form_Main_Play_ShowTime": MainForm.Config.Play.Lasting = Load_vars.ToInt(vars); ; break;
                        case "FastKey_Find_Esc": FindForm.Config.FastKey_Esc = Load_vars.ToInt(vars); break;
                        case "FastKey_Find_Export": FindForm.Config.FastKey_Export = Load_vars.ToInt(vars); break;
                        case "FastKey_Find_L": FindForm.Config.FastKey_L = Load_vars.ToInt(vars); break;
                        case "FastKey_Find_R": FindForm.Config.FastKey_R = Load_vars.ToInt(vars); break;
                        case "FastKey_Find_U": FindForm.Config.FastKey_U = Load_vars.ToInt(vars); break;
                        case "FastKey_Find_D": FindForm.Config.FastKey_D = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_D": MainForm.Config.FastKey_D = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_L": MainForm.Config.FastKey_L = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_PageD": MainForm.Config.FastKey_PageD = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_PageU": MainForm.Config.FastKey_PageU = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_R": MainForm.Config.FastKey_R = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_U": MainForm.Config.FastKey_U = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_Board": MainForm.Config.FastKey_Board = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_Enter": MainForm.Config.FastKey_Enter = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_Esc": MainForm.Config.FastKey_ESC = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_Export": MainForm.Config.FastKey_Export = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_OpenComic": MainForm.Config.FastKey_OpenComic = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_OpenCurrent": MainForm.Config.FastKey_OpenCurrent = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_OpenExport": MainForm.Config.FastKey_OpenExport = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_OpenRoot": MainForm.Config.FastKey_OpenRoot = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_Password": MainForm.Config.FastKey_Password = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_Rotate": MainForm.Config.FastKey_Rotate = Load_vars.ToInt(vars); break;
                        case "FastKey_Search_Esc": SearchForm.Config.FastKey_Esc = Load_vars.ToInt(vars); break;
                        case "FastKey_Search_Enter": SearchForm.Config.FastKey_Enter = Load_vars.ToInt(vars); break;
                        case "FastKey_Input_Enter": InputForm.Config.FastKey_Enter = Load_vars.ToInt(vars); break;
                        case "FastKey_Image_Esc": ImageForm.Config.FastKey_Esc = Load_vars.ToInt(vars); break;
                        case "FastKey_Image_Enter": ImageForm.Config.FastKey_Enter = Load_vars.ToInt(vars); break;
                        case "FastKey_Image_Rotate": ImageForm.Config.FastKey_Rotate = Load_vars.ToInt(vars); break;
                        case "FastKey_Image_FlipX": ImageForm.Config.FastKey_FlipX = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_FlipX": MainForm.Config.FastKey_FlipX = Load_vars.ToInt(vars); break;
                        case "FastKey_Image_FlipY": ImageForm.Config.FastKey_FlipY = Load_vars.ToInt(vars); break;
                        case "FastKey_Main_FlipY": MainForm.Config.FastKey_FlipY = Load_vars.ToInt(vars); break;
                        case "Form_Main_ShapeControlRate": MainForm.Config.ShapeControlRate = Load_vars.ToDouble(vars); break;
                        default: break;
                    }

                    #endregion
                }
                sr.Close();
            }
            catch
            {

            }

            for (int i = 0; i < Config.RootPathes.Count; i++) { Config.Trees.Add(new List<List<int>>()); }
            Load_files.load();
        }
    }
}
