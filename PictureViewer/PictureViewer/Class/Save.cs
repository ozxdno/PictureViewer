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
            public int Form_Main_ShapeWindowRate { set { Load.settings.Form_Main_ShapeWindowRate = value; } }
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

                //swCFG.WriteLine("FileType0=" + FileSupport.O_FileType(0));
                //swCFG.WriteLine("FileType1=" + FileSupport.O_FileType(1));
                //swCFG.WriteLine("FileType2=" + FileSupport.O_FileType(2));
                //swCFG.WriteLine("FileType3=" + FileSupport.O_FileType(3));
                //swCFG.WriteLine("FileType4=" + FileSupport.O_FileType(4));
                //swCFG.WriteLine("FileType5=" + FileSupport.O_FileType(5));
                //swCFG.WriteLine("");

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
                swCFG.WriteLine("");
            }
            catch { swCFG.Close(); return false; } swCFG.Close(); return true;
        }


        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////
    }
}
