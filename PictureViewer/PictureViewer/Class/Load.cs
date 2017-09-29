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

        ///////////////////////////////////////////////////// private attribute ///////////////////////////////////////////////

        

        ///////////////////////////////////////////////////// public method ///////////////////////////////////////////////

        /// <summary>
        /// 读入配置文件，利用配置文件初始化变量，返回是否读取成功。
        /// </summary>
        /// <returns></returns>
        public static bool Load_CFG()
        {
            // 初始化变量
            Form_Main.config.ExportFolder = "";
            Form_Main.config.FolderIndex = -1;
            Form_Main.config.FileIndex = -1;
            Form_Main.config.SubIndex = -1;

            FileOperate.RootFiles = new List<FileOperate.ROOT>();

            // 找到配置文件
            string fullpath = Form_Main.config.ConfigPath + "\\" + Form_Main.config.ConfigName;
            if (!File.Exists(fullpath)) { return false; }

            // 读入文件内容
            try
            {
                string Line; string[] Item, RootPath = new string[0]; srCFG = new StreamReader(fullpath);

                while (!srCFG.EndOfStream)
                {
                    Line = srCFG.ReadLine(); if (Line == "") { continue; }
                    Item = Line.Split('=');

                    switch(Item[0])
                    {
                        case "ExportFolder": Form_Main.config.ExportFolder = Item.Length == 1 ? "" : Item[1]; break;
                        case "FolderIndex": Form_Main.config.FolderIndex = int.Parse(Item[1]); break;
                        case "FileIndex": Form_Main.config.FileIndex = int.Parse(Item[1]); break;
                        case "SubIndex": Form_Main.config.SubIndex = int.Parse(Item[1]); break;
                        case "RootPath": RootPath = Item.Length == 1 ? new string[0] : Item[1].Split('|'); break;
                        default: break;
                    }
                }

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
            }
            catch { srCFG.Close(); return false; } srCFG.Close(); return true;
        }

        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////
    }
}
