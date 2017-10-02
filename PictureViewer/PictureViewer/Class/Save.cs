﻿using System;
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

                string RootPath = ""; if (FileOperate.RootFiles.Count != 0) { RootPath = FileOperate.RootFiles[0].Path; }
                for (int i = 1; i < FileOperate.RootFiles.Count; i++)
                { RootPath += "|" + FileOperate.RootFiles[i].Path; }
                swCFG.WriteLine("RootPath=" + RootPath);
                swCFG.WriteLine("");

                //swCFG.WriteLine("FileType0=" + FileSupport.O_FileType(0));
                //swCFG.WriteLine("FileType1=" + FileSupport.O_FileType(1));
                //swCFG.WriteLine("FileType2=" + FileSupport.O_FileType(2));
                //swCFG.WriteLine("FileType3=" + FileSupport.O_FileType(3));
                //swCFG.WriteLine("FileType4=" + FileSupport.O_FileType(4));
                //swCFG.WriteLine("FileType5=" + FileSupport.O_FileType(5));
                //swCFG.WriteLine("");
            }
            catch { swCFG.Close(); return false; } swCFG.Close(); return true;
        }


        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////
    }
}
