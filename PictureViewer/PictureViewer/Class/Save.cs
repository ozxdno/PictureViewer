using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PictureViewer.Class
{
    class Save
    {
        ///////////////////////////////////////////////////// public attribute ///////////////////////////////////////////////
    
        /// <summary>
        /// 配置文件输出流
        /// </summary>
        public static StreamWriter swCFG = null;
        /// <summary>
        /// 图片文件输出流
        /// </summary>
        public static StreamWriter swPIC = null;

        /// <summary>
        /// 保存用户设置信息
        /// </summary>
        public static SETTINGS settings;
        /// <summary>
        /// 保存用户设置信息
        /// </summary>
        public struct SETTINGS
        {
            public bool Form_Main_Hide { set { Load.settings.Form_Main_Hide = value; } }
            public bool Form_Main_Hide_L { set { Load.settings.Form_Main_Hide_L = value; } }
            public bool Form_Main_Hide_R { set { Load.settings.Form_Main_Hide_R = value; } }
            public bool Form_Main_Hide_U { set { Load.settings.Form_Main_Hide_U = value; } }
            public bool Form_Main_Hide_D { set { Load.settings.Form_Main_Hide_D = value; } }
            public bool Form_Main_Find_Full { set { Load.settings.Form_Main_Find_Full = value; } }
            public bool Form_Main_Find_Part { set { Load.settings.Form_Main_Find_Part = value; } }
            public bool Form_Main_Find_Same { set { Load.settings.Form_Main_Find_Same = value; } }
            public bool Form_Main_Find_Like { set { Load.settings.Form_Main_Find_Like = value; } }
            public bool Form_Main_Find_Turn { set { Load.settings.Form_Main_Find_Turn = value; } }
            public bool Form_Main_UseSmallWindowOpen { set { Load.settings.Form_Main_UseSmallWindowOpen = value; } }
            public int Form_Main_Height { set { Load.settings.Form_Main_Height = value; } }
            public int Form_Main_Width { set { Load.settings.Form_Main_Width = value; } }
            public int Form_Main_Location_X { set { Load.settings.Form_Main_Location_X = value; } }
            public int Form_Main_Location_Y { set { Load.settings.Form_Main_Location_Y = value; } }
            public bool Form_Main_Lock { set { Load.settings.Form_Main_Lock = value; } }

            public int Form_Find_Degree { set { Load.settings.Form_Find_Degree = value; } }
            public int Form_Find_Pixes { set { Load.settings.Form_Find_Pixes = value; } }

            public bool Form_Main_UseBoard { set { Load.settings.Form_Main_UseBoard = value; } }
            public bool Form_Main_ShapeWindow { set { Load.settings.Form_Main_ShapeWindow = value; } }
            public double Form_Main_ShapeWindowRate { set { Load.settings.Form_Main_ShapeWindowRate = value; } }
            public bool Form_Main_Tip { set { Load.settings.Form_Main_Tip = value; } }

            public bool Form_Main_Play_Forward { set { Load.settings.Form_Main_Play_Forward = value; } }
            public bool Form_Main_Play_Backward { set { Load.settings.Form_Main_Play_Backward = value; } }
            public bool Form_Main_Play_TotalRoots { set { Load.settings.Form_Main_Play_TotalRoots = value; } }
            public bool Form_Main_Play_Root { set { Load.settings.Form_Main_Play_Root = value; } }
            public bool Form_Main_Play_Subroot { set { Load.settings.Form_Main_Play_Subroot = value; } }
            public bool Form_Main_Play_Picture { set { Load.settings.Form_Main_Play_Picture = value; } }
            public bool Form_Main_Play_Gif { set { Load.settings.Form_Main_Play_Gif = value; } }
            public bool Form_Main_Play_Music { set { Load.settings.Form_Main_Play_Music = value; } }
            public bool Form_Main_Play_Video { set { Load.settings.Form_Main_Play_Video = value; } }
            public bool Form_Main_Play_Single { set { Load.settings.Form_Main_Play_Single = value; } }
            public bool Form_Main_Play_Order { set { Load.settings.Form_Main_Play_Order = value; } }
            public bool Form_Main_Play_Circle { set { Load.settings.Form_Main_Play_Circle = value; } }
            public bool Form_Main_Play_Rand { set { Load.settings.Form_Main_Play_Rand = value; } }
            public int Form_Main_Play_ShowTime { set { Load.settings.Form_Main_Play_ShowTime = value; } }
        }

        ///////////////////////////////////////////////////// private attribute ///////////////////////////////////////////////



        ///////////////////////////////////////////////////// public method ///////////////////////////////////////////////

        /// <summary>
        /// 保存 CFG 文件，返回是否保存成功。
        /// </summary>
        /// <returns></returns>
        public static bool Save_CFG()
        {
            try
            {
                string fullpath = Form_Main.config.ConfigPath + "\\" + Form_Main.config.ConfigName;
                swCFG = new StreamWriter(fullpath, false);

                swCFG.WriteLine("FolderIndex=" + Form_Main.config.FolderIndex.ToString());
                swCFG.WriteLine("FileIndex=" + Form_Main.config.FileIndex.ToString());
                swCFG.WriteLine("SubIndex=" + Form_Main.config.SubIndex.ToString());
                swCFG.WriteLine("");
                swCFG.WriteLine("ExportFolder=" + Form_Main.config.ExportFolder);
                swCFG.WriteLine("");

                //string RootPath = ""; if (FileOperate.RootFiles.Count != 0) { RootPath = FileOperate.RootFiles[0].Path; }
                //for (int i = 1; i < FileOperate.RootFiles.Count; i++)
                //{ RootPath += "|" + FileOperate.RootFiles[i].Path; }
                for (int i = 0; i < FileOperate.RootFiles.Count; i++)
                { swCFG.WriteLine("RootPath=" + FileOperate.RootFiles[i].Path); }
                swCFG.WriteLine("");

                for (int i = 0; i < FileSupport.ExtraExtensions.Count; i++)
                {
                    string extenison = FileSupport.ExtraExtensions[i];
                    int type = FileSupport.ExtraTypes[i];
                    bool isMusic = FileSupport.ExtraIsMusic[i];
                    bool isVideo = FileSupport.ExtraIsVideo[i];

                    if (type == 2) { swCFG.WriteLine("SupportPicture=" + extenison); }
                    if (type == 3) { swCFG.WriteLine("SupportGif=" + extenison); }
                    if (type == 4 && isMusic) { swCFG.WriteLine("SupportMusic=" + extenison); }
                    if (type == 4 && isVideo) { swCFG.WriteLine("SupportVideo=" + extenison); }
                    if (type == 5) { swCFG.WriteLine("SupportZip=" + extenison); }
                }
                swCFG.WriteLine("");

                swCFG.WriteLine("Form_Main_Hide=" + (Load.settings.Form_Main_Hide ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Hide_L=" + (Load.settings.Form_Main_Hide_L ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Hide_R=" + (Load.settings.Form_Main_Hide_R ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Hide_U=" + (Load.settings.Form_Main_Hide_U ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Hide_D=" + (Load.settings.Form_Main_Hide_D ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Find_Full=" + (Load.settings.Form_Main_Find_Full ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Find_Part=" + (Load.settings.Form_Main_Find_Part ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Find_Same=" + (Load.settings.Form_Main_Find_Same ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Find_Like=" + (Load.settings.Form_Main_Find_Like ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Find_Turn=" + (Load.settings.Form_Main_Find_Turn ? "1" : "0"));
                swCFG.WriteLine("");

                swCFG.WriteLine("Form_Find_Degree=" + Load.settings.Form_Find_Degree.ToString());
                swCFG.WriteLine("Form_Find_Pixes=" + Load.settings.Form_Find_Pixes.ToString());
                swCFG.WriteLine("");

                swCFG.WriteLine("Form_Main_UseSmallWindowOpen=" + (Load.settings.Form_Main_UseSmallWindowOpen ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Height=" + Load.settings.Form_Main_Height.ToString());
                swCFG.WriteLine("Form_Main_Width=" + Load.settings.Form_Main_Width.ToString());
                swCFG.WriteLine("Form_Main_Location_X=" + Load.settings.Form_Main_Location_X.ToString());
                swCFG.WriteLine("Form_Main_Location_Y=" + Load.settings.Form_Main_Location_Y.ToString());
                swCFG.WriteLine("Form_Main_Lock=" + (Load.settings.Form_Main_Lock ? "1" : "0"));
                swCFG.WriteLine("");

                swCFG.WriteLine("Form_Main_UseBoard=" + (Load.settings.Form_Main_UseBoard ? "1" : "0"));
                swCFG.WriteLine("Form_Main_ShapeWindow=" + (Load.settings.Form_Main_ShapeWindow ? "1" : "0"));
                swCFG.WriteLine("Form_Main_ShapeWindowRate=" + Load.settings.Form_Main_ShapeWindowRate.ToString());
                swCFG.WriteLine("Form_Main_MaxWindowSize=" + Load.settings.Form_Main_MaxWindowSize.ToString());
                swCFG.WriteLine("Form_Main_MinWindowSize=" + Load.settings.Form_Main_MinWindowSize.ToString());
                swCFG.WriteLine("Form_Main_Tip=" + (Load.settings.Form_Main_Tip ? "1" : "0"));
                swCFG.WriteLine("");

                swCFG.WriteLine("Form_Main_Play_Forward=" + (Load.settings.Form_Main_Play_Forward ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Play_Backward=" + (Load.settings.Form_Main_Play_Backward ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Play_TotalRoots=" + (Load.settings.Form_Main_Play_TotalRoots ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Play_Root=" + (Load.settings.Form_Main_Play_Root ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Play_Subroot=" + (Load.settings.Form_Main_Play_Subroot ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Play_Picture=" + (Load.settings.Form_Main_Play_Picture ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Play_Gif=" + (Load.settings.Form_Main_Play_Gif ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Play_Music=" + (Load.settings.Form_Main_Play_Music ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Play_Video=" + (Load.settings.Form_Main_Play_Video ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Play_Single=" + (Load.settings.Form_Main_Play_Single ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Play_Order=" + (Load.settings.Form_Main_Play_Order ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Play_Circle=" + (Load.settings.Form_Main_Play_Circle ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Play_Rand=" + (Load.settings.Form_Main_Play_Rand ? "1" : "0"));
                swCFG.WriteLine("Form_Main_Play_ShowTime=" + Load.settings.Form_Main_Play_ShowTime.ToString());
                swCFG.WriteLine("");

                swCFG.WriteLine("FastKey_Main_Esc=" + Load.settings.FastKey_Main_Esc.ToString());
                swCFG.WriteLine("FastKey_Main_Enter=" + Load.settings.FastKey_Main_Enter.ToString());
                swCFG.WriteLine("FastKey_Main_Board=" + Load.settings.FastKey_Main_Board.ToString());
                swCFG.WriteLine("FastKey_Main_U=" + Load.settings.FastKey_Main_U.ToString());
                swCFG.WriteLine("FastKey_Main_D=" + Load.settings.FastKey_Main_D.ToString());
                swCFG.WriteLine("FastKey_Main_L=" + Load.settings.FastKey_Main_L.ToString());
                swCFG.WriteLine("FastKey_Main_R=" + Load.settings.FastKey_Main_R.ToString());
                swCFG.WriteLine("FastKey_Main_PageU=" + Load.settings.FastKey_Main_PageU.ToString());
                swCFG.WriteLine("FastKey_Main_PageD=" + Load.settings.FastKey_Main_PageD.ToString());
                swCFG.WriteLine("FastKey_Main_Export=" + Load.settings.FastKey_Main_Export.ToString());
                swCFG.WriteLine("FastKey_Main_OpenExport=" + Load.settings.FastKey_Main_OpenExport.ToString());
                swCFG.WriteLine("FastKey_Main_OpenRoot=" + Load.settings.FastKey_Main_OpenRoot.ToString());
                swCFG.WriteLine("FastKey_Main_OpenComic=" + Load.settings.FastKey_Main_OpenComic.ToString());
                swCFG.WriteLine("FastKey_Main_OpenCurrent=" + Load.settings.FastKey_Main_OpenCurrent.ToString());
                swCFG.WriteLine("FastKey_Main_Password=" + Load.settings.FastKey_Main_Password.ToString());
                swCFG.WriteLine("FastKey_Main_Rotate=" + Load.settings.FastKey_Main_Rotate.ToString());
                swCFG.WriteLine("FastKey_Main_FlipX=" + Load.settings.FastKey_Main_FlipX.ToString());
                swCFG.WriteLine("FastKey_Main_FlipY=" + Load.settings.FastKey_Main_FlipY.ToString());
                swCFG.WriteLine("");

                swCFG.WriteLine("FastKey_Find_Esc=" + Load.settings.FastKey_Find_Esc.ToString());
                swCFG.WriteLine("FastKey_Find_U=" + Load.settings.FastKey_Find_U.ToString());
                swCFG.WriteLine("FastKey_Find_D=" + Load.settings.FastKey_Find_D.ToString());
                swCFG.WriteLine("FastKey_Find_L=" + Load.settings.FastKey_Find_L.ToString());
                swCFG.WriteLine("FastKey_Find_R=" + Load.settings.FastKey_Find_R.ToString());
                swCFG.WriteLine("FastKey_Find_Export=" + Load.settings.FastKey_Find_Export.ToString());
                swCFG.WriteLine("");

                swCFG.WriteLine("FastKey_Search_Esc=" + Load.settings.FastKey_Search_Esc.ToString());
                swCFG.WriteLine("FastKey_Search_Enter=" + Load.settings.FastKey_Search_Enter.ToString());
                swCFG.WriteLine("");

                swCFG.WriteLine("FastKey_Input_Enter=" + Load.settings.FastKey_Input_Enter.ToString());
                swCFG.WriteLine("");

                swCFG.WriteLine("FastKey_Image_Esc=" + Load.settings.FastKey_Image_Esc.ToString());
                swCFG.WriteLine("FastKey_Image_Enter=" + Load.settings.FastKey_Image_Enter.ToString());
                swCFG.WriteLine("FastKey_Image_Rotate=" + Load.settings.FastKey_Image_Rotate.ToString());
                swCFG.WriteLine("FastKey_Image_FlipX=" + Load.settings.FastKey_Image_FlipX.ToString());
                swCFG.WriteLine("FastKey_Image_FlipY=" + Load.settings.FastKey_Image_FlipY.ToString());
                swCFG.WriteLine("");
            }
            catch
            {  }

            try { swCFG.Close(); } catch { }

            return true;
        }

        /// <summary>
        /// 保存 PICTURES 文件
        /// </summary>
        /// <returns></returns>
        public static bool Save_PIC()
        {
            try
            {
                string fullpath = Form_Main.config.ConfigPath + "\\pvdata";
                swPIC = new StreamWriter(fullpath, false);

                for (int i = 0; i < Form_Find.PictureFiles.Count; i++)
                {
                    Form_Find.PICTURE p = Form_Find.PictureFiles[i];
                    if (!p.Loaded) { continue; }
                    if (!File.Exists(p.Full)) { continue; }

                    swPIC.WriteLine(getPictureString(p));
                }
            }
            catch
            {

            }

            try { swPIC.Close(); } catch { }
            return true;
        }

        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////

        private static string getPictureString(Form_Find.PICTURE p)
        {
            string pstr = "";

            pstr += p.Path + "|";
            pstr += p.Name + "|";
            pstr += p.Time.ToString() + "|";
            pstr += p.Height.ToString() + "|";
            pstr += p.Width.ToString() + "|";
            pstr += p.Length.ToString() + "|";
            pstr += p.Row.ToString() + "|";
            pstr += p.Col.ToString() + "|";

            for (int i = 0; i < p.GraysR.Length; i++) { pstr += p.GraysR[i].ToString() + "|"; }
            for (int i = 0; i < p.GraysC.Length; i++) { pstr += p.GraysC[i].ToString() + "|"; }

            return pstr;
        }
    }
}
