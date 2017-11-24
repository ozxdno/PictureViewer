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
        /// 初始化，包含读入文件（LoadFile）、设置默认值（SetDefault）两个过程。
        /// </summary>
        public Load_pvini()
        {
            LoadFile();
            SetDefault();
        }

        /// <summary>
        /// 读入文件
        /// </summary>
        /// <returns></returns>
        public void LoadFile()
        {
            Config.RootPathes.Clear();

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

                    switch (item)
                    {
                        case "ExportFolder": Config.ExportPath = Load_vars.ToDirectory(vars); break;
                        case "FolderIndex": MainForm.Config.TreeIndex.Folder = Load_vars.ToInt(vars); break;
                        case "FileIndex": MainForm.Config.TreeIndex.File = Load_vars.ToInt(vars); break;
                        case "SubIndex": MainForm.Config.TreeIndex.Sub = Load_vars.ToInt(vars); break;
                        case "RootPath": Config.RootPathes.AddRange(Load_vars.ToStringList(vars)); break;
                        case "SupportPicture": Config.Supports.AddRange(Load_vars.ToStringList(vars)); break;
                        //case "SupportGif": ToStringEx(Item[1], ref SupportGif); break;
                        //case "SupportMusic": ToStringEx(Item[1], ref SupportMusic); break;
                        //case "SupportVideo": ToStringEx(Item[1], ref SupportVideo); break;
                        //case "SupportZip": ToStringEx(Item[1], ref SupportZip); break;
                        //case "Form_Main_Hide": Found.Form_Main_Hide = ToBool(Item[1], ref settings.Form_Main_Hide); break;
                        //case "Form_Main_Hide_L": Found.Form_Main_Hide_L = ToBool(Item[1], ref settings.Form_Main_Hide_L); break;
                        //case "Form_Main_Hide_R": Found.Form_Main_Hide_R = ToBool(Item[1], ref settings.Form_Main_Hide_R); break;
                        //case "Form_Main_Hide_U": Found.Form_Main_Hide_U = ToBool(Item[1], ref settings.Form_Main_Hide_U); break;
                        //case "Form_Main_Hide_D": Found.Form_Main_Hide_D = ToBool(Item[1], ref settings.Form_Main_Hide_D); break;
                        //case "Form_Main_Find_Full": Found.Form_Main_Find_Full = ToBool(Item[1], ref settings.Form_Main_Find_Full); break;
                        //case "Form_Main_Find_Part": Found.Form_Main_Find_Part = ToBool(Item[1], ref settings.Form_Main_Find_Part); break;
                        //case "Form_Main_Find_Same": Found.Form_Main_Find_Same = ToBool(Item[1], ref settings.Form_Main_Find_Same); break;
                        //case "Form_Main_Find_Like": Found.Form_Main_Find_Like = ToBool(Item[1], ref settings.Form_Main_Find_Like); break;
                        //case "Form_Main_Find_Turn": Found.Form_Main_Find_Turn = ToBool(Item[1], ref settings.Form_Main_Find_Turn); break;
                        //case "Form_Find_Degree": Found.Form_Find_Degree = ToInt(Item[1], ref settings.Form_Find_Degree); break;
                        //case "Form_Find_Pixes": Found.Form_Find_Pixes = ToInt(Item[1], ref settings.Form_Find_Pixes); break;
                        //case "Form_Main_UseSmallWindowOpen": Found.Form_Main_UseSmallWindowOpen = ToBool(Item[1], ref settings.Form_Main_UseSmallWindowOpen); break;
                        //case "Form_Main_Height": Found.Form_Main_Height = ToInt(Item[1], ref settings.Form_Main_Height); break;
                        //case "Form_Main_Width": Found.Form_Main_Width = ToInt(Item[1], ref settings.Form_Main_Width); break;
                        //case "Form_Main_Location_X": Found.Form_Main_Location_X = ToInt(Item[1], ref settings.Form_Main_Location_X); break;
                        //case "Form_Main_Location_Y": Found.Form_Main_Location_Y = ToInt(Item[1], ref settings.Form_Main_Location_Y); break;
                        //case "Form_Main_UseBoard": Found.Form_Main_UseBoard = ToBool(Item[1], ref settings.Form_Main_UseBoard); break;
                        //case "Form_Main_ShapeWindow": Found.Form_Main_ShapeWindow = ToBool(Item[1], ref settings.Form_Main_ShapeWindow); break;
                        //case "Form_Main_ShapeWindowRate": Found.Form_Main_ShapeWindowRate = ToDouble(Item[1], ref settings.Form_Main_ShapeWindowRate); break;
                        //case "Form_Main_Lock": Found.Form_Main_Lock = ToBool(Item[1], ref settings.Form_Main_Lock); break;
                        //case "Form_Main_MaxWindowSize": Found.Form_Main_MaxWindowSize = ToInt(Item[1], ref settings.Form_Main_MaxWindowSize); break;
                        //case "Form_Main_MinWindowSize": Found.Form_Main_MinWindowSize = ToInt(Item[1], ref settings.Form_Main_MinWindowSize); break;
                        //case "Form_Main_Tip": Found.Form_Main_Tip = ToBool(Item[1], ref settings.Form_Main_Tip); break;
                        //case "Form_Main_Play_Forward": Found.Form_Main_Play_Forward = ToBool(Item[1], ref settings.Form_Main_Play_Forward); break;
                        //case "Form_Main_Play_Backward": Found.Form_Main_Play_Backward = ToBool(Item[1], ref settings.Form_Main_Play_Backward); break;
                        //case "Form_Main_Play_TotalRoots": Found.Form_Main_Play_TotalRoots = ToBool(Item[1], ref settings.Form_Main_Play_TotalRoots); break;
                        //case "Form_Main_Play_Root": Found.Form_Main_Play_Root = ToBool(Item[1], ref settings.Form_Main_Play_Root); break;
                        //case "Form_Main_Play_Subroot": Found.Form_Main_Play_Subroot = ToBool(Item[1], ref settings.Form_Main_Play_Subroot); break;
                        //case "Form_Main_Play_Picture": Found.Form_Main_Play_Picture = ToBool(Item[1], ref settings.Form_Main_Play_Picture); break;
                        //case "Form_Main_Play_Gif": Found.Form_Main_Play_Gif = ToBool(Item[1], ref settings.Form_Main_Play_Gif); break;
                        //case "Form_Main_Play_Music": Found.Form_Main_Play_Music = ToBool(Item[1], ref settings.Form_Main_Play_Music); break;
                        //case "Form_Main_Play_Video": Found.Form_Main_Play_Video = ToBool(Item[1], ref settings.Form_Main_Play_Video); break;
                        //case "Form_Main_Play_Single": Found.Form_Main_Play_Single = ToBool(Item[1], ref settings.Form_Main_Play_Single); break;
                        //case "Form_Main_Play_Order": Found.Form_Main_Play_Order = ToBool(Item[1], ref settings.Form_Main_Play_Order); break;
                        //case "Form_Main_Play_Circle": Found.Form_Main_Play_Circle = ToBool(Item[1], ref settings.Form_Main_Play_Circle); break;
                        //case "Form_Main_Play_Rand": Found.Form_Main_Play_Rand = ToBool(Item[1], ref settings.Form_Main_Play_Rand); break;
                        //case "Form_Main_Play_ShowTime": Found.Form_Main_Play_ShowTime = ToInt(Item[1], ref settings.Form_Main_Play_ShowTime); break;
                        //case "FastKey_Find_Esc": Found.FastKey_Find_Esc = ToInt(Item[1], ref settings.FastKey_Find_Esc); break;
                        //case "FastKey_Find_Export": Found.FastKey_Find_Export = ToInt(Item[1], ref settings.FastKey_Find_Export); break;
                        //case "FastKey_Find_L": Found.FastKey_Find_L = ToInt(Item[1], ref settings.FastKey_Find_L); break;
                        //case "FastKey_Find_R": Found.FastKey_Find_R = ToInt(Item[1], ref settings.FastKey_Find_R); break;
                        //case "FastKey_Find_U": Found.FastKey_Find_U = ToInt(Item[1], ref settings.FastKey_Find_U); break;
                        //case "FastKey_Find_D": Found.FastKey_Find_D = ToInt(Item[1], ref settings.FastKey_Find_D); break;
                        //case "FastKey_Main_D": Found.FastKey_Main_D = ToInt(Item[1], ref settings.FastKey_Main_D); break;
                        //case "FastKey_Main_L": Found.FastKey_Main_L = ToInt(Item[1], ref settings.FastKey_Main_L); break;
                        //case "FastKey_Main_PageD": Found.FastKey_Main_PageD = ToInt(Item[1], ref settings.FastKey_Main_PageD); break;
                        //case "FastKey_Main_PageU": Found.FastKey_Main_PageU = ToInt(Item[1], ref settings.FastKey_Main_PageU); break;
                        //case "FastKey_Main_R": Found.FastKey_Main_R = ToInt(Item[1], ref settings.FastKey_Main_R); break;
                        //case "FastKey_Main_U": Found.FastKey_Main_U = ToInt(Item[1], ref settings.FastKey_Main_U); break;
                        //case "FastKey_Main_Board": Found.FastKey_Main_Board = ToInt(Item[1], ref settings.FastKey_Main_Board); break;
                        //case "FastKey_Main_Enter": Found.FastKey_Main_Enter = ToInt(Item[1], ref settings.FastKey_Main_Enter); break;
                        //case "FastKey_Main_Esc": Found.FastKey_Main_Esc = ToInt(Item[1], ref settings.FastKey_Main_Esc); break;
                        //case "FastKey_Main_Export": Found.FastKey_Main_Export = ToInt(Item[1], ref settings.FastKey_Main_Export); break;
                        //case "FastKey_Main_OpenComic": Found.FastKey_Main_OpenComic = ToInt(Item[1], ref settings.FastKey_Main_OpenComic); break;
                        //case "FastKey_Main_OpenCurrent": Found.FastKey_Main_OpenCurrent = ToInt(Item[1], ref settings.FastKey_Main_OpenCurrent); break;
                        //case "FastKey_Main_OpenExport": Found.FastKey_Main_OpenExport = ToInt(Item[1], ref settings.FastKey_Main_OpenExport); break;
                        //case "FastKey_Main_OpenRoot": Found.FastKey_Main_OpenRoot = ToInt(Item[1], ref settings.FastKey_Main_OpenRoot); break;
                        //case "FastKey_Main_Password": Found.FastKey_Main_Password = ToInt(Item[1], ref settings.FastKey_Main_Password); break;
                        //case "FastKey_Main_Rotate": Found.FastKey_Main_Rotate = ToInt(Item[1], ref settings.FastKey_Main_Rotate); break;
                        //case "FastKey_Search_Esc": Found.FastKey_Search_Esc = ToInt(Item[1], ref settings.FastKey_Search_Esc); break;
                        //case "FastKey_Search_Enter": Found.FastKey_Search_Enter = ToInt(Item[1], ref settings.FastKey_Search_Enter); break;
                        //case "FastKey_Input_Enter": Found.FastKey_Input_Enter = ToInt(Item[1], ref settings.FastKey_Input_Enter); break;
                        //case "FastKey_Image_Esc": Found.FastKey_Image_Esc = ToInt(Item[1], ref settings.FastKey_Image_Esc); break;
                        //case "FastKey_Image_Enter": Found.FastKey_Image_Enter = ToInt(Item[1], ref settings.FastKey_Image_Enter); break;
                        //case "FastKey_Image_Rotate": Found.FastKey_Image_Rotate = ToInt(Item[1], ref settings.FastKey_Image_Rotate); break;
                        //case "FastKey_Image_FlipX": Found.FastKey_Image_FlipX = ToInt(Item[1], ref settings.FastKey_Image_FlipX); break;
                        //case "FastKey_Main_FlipX": Found.FastKey_Main_FlipX = ToInt(Item[1], ref settings.FastKey_Main_FlipX); break;
                        //case "FastKey_Image_FlipY": Found.FastKey_Image_FlipY = ToInt(Item[1], ref settings.FastKey_Image_FlipY); break;
                        //case "FastKey_Main_FlipY": Found.FastKey_Main_FlipY = ToInt(Item[1], ref settings.FastKey_Main_FlipY); break;
                        default: break;
                    }
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// 读入信息有误时，自动更新为默认值。
        /// </summary>
        public void SetDefault()
        {

        }
    }
}
