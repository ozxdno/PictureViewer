using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PictureViewer.Method
{
    class ReadFile
    {
        public static void ReadFile_CFG()
        {
            if (PVproject.sr != null) { throw new Exception("file is reading !"); }

            string ExePath = PVproject.CFG.exe_path;
            string Name = "pv.pvini";

            string FullPath = ExePath + "\\" + Name;
            //if (!File.Exists(FullPath)) { throw new Exception("No config file !"); }
            if (!File.Exists(FullPath)) { PVproject.CFG.FirstTime = true; return; }

            PVproject.sr = new StreamReader(FullPath);
            string NewLine = "";
            string[] NewItem = new string[0];

            NewLine = PVproject.sr.ReadLine(); PVproject.CFG.o_path = NewLine.Substring(12);
            NewLine = PVproject.sr.ReadLine(); PVproject.CFG.e_path = NewLine.Substring(12);
            NewLine = PVproject.sr.ReadLine(); PVproject.CFG.database_path = NewLine.Substring(14);
            NewLine = PVproject.sr.ReadLine(); PVproject.CFG.database_name = NewLine.Substring(14);
            NewLine = PVproject.sr.ReadLine(); PVproject.CFG.LastFileID = long.Parse(NewLine.Substring(13));
            NewLine = PVproject.sr.ReadLine(); PVproject.CFG.viewIndex = int.Parse(NewLine.Substring(8));

            NewLine = PVproject.sr.ReadLine();
            NewItem = NewLine.Split(':');
            NewItem = NewItem[1].Split('|');
            PVproject.CFG.pic_process = NewItem[0];
            PVproject.CFG.gif_process = NewItem[1];
            PVproject.CFG.mov_process = NewItem[2];
            
            string title = "", list = "";
            List<string> Exps = new List<string>();
            while (!PVproject.sr.EndOfStream)
            {
                NewLine = PVproject.sr.ReadLine();

                if (NewLine.Length > 2)
                {
                    if (NewLine[0].Equals('>') || NewLine[1].Equals('>'))
                    {
                        title = NewLine.Substring(2); continue;
                    }
                }
                if (title.Equals("Tags"))
                {
                    NewItem = NewLine.Split(':');
                    if (NewItem.Length < 2) { continue; }
                    list = NewItem[0];

                    NewItem = NewItem[1].Split('|');
                    PVproject.TAG.LIST ilist = new PVproject.TAG.LIST();
                    ilist.tags = new List<string>();
                    ilist.name = list;
                    foreach (string itag in NewItem)
                    {
                        ilist.tags.Add(itag);
                        PVproject.tag.TagStrs.Add(list + ":" + itag);
                        PVproject.tag.TagExps.Add("");
                    }
                    PVproject.tag.list.Add(ilist);
                }
                if (title.Equals("Exps"))
                {
                    Exps.Add(NewLine);
                }
            }

            PVproject.sr.Close();
            PVproject.sr = null;

            string[] item = new string[0];
            foreach (string iexp in Exps)
            {
                item = iexp.Split('|');
                if (item.Length < 2) { continue; }

                for (int i = 0; i < PVproject.tag.TagStrs.Count; i++)
                {
                    if (!item[0].Equals(PVproject.tag.TagStrs[i])) { continue; }
                    PVproject.tag.TagExps[i] = item[1]; break;
                }
            }
        }
        public static void ReadFile_DB()
        {
            if (PVproject.sr != null) { throw new Exception("file is reading !"); }

            string fullpath = PVproject.CFG.database_path + "\\" + PVproject.CFG.database_name;
            if (!File.Exists(fullpath)) { throw new Exception("PVDB File Not Exist !"); }

            PVproject.DB = new List<PVproject.DATABASE>();

            PVproject.sr = new StreamReader(fullpath);
            string line = ""; string[] item = new string[0];
            while (!PVproject.sr.EndOfStream)
            {
                line = PVproject.sr.ReadLine();
                item = line.Split('|');

                // Name,type,extension,size,tags
                PVproject.DATABASE db = new PVproject.DATABASE();
                db.tags = new List<string>();
                db.name = item[0];
                db.type = item[1];
                db.extension = item[2];
                db.size = int.Parse(item[3]);
                for (int i = 4; i < item.Length; i++) { db.tags.Add(item[i]); }
                PVproject.DB.Add(db);
            }
            PVproject.sr.Close();
            PVproject.sr = null;
        }
        public static void ReadFile_IN()
        {
            if (!Directory.Exists(PVproject.CFG.i_path)) { throw new Exception("Path unavailable !"); }

            PVproject.IN = new List<PVproject.DATABASE>();

            DirectoryInfo dir = new DirectoryInfo(PVproject.CFG.i_path);
            FileInfo[] Files = dir.GetFiles();
            DirectoryInfo[] Folders = dir.GetDirectories();

            foreach (FileInfo ifile in Files)
            {
                PVproject.DATABASE db = new PVproject.DATABASE();
                
                string ex = ifile.Extension;
                db.extension = ex;
                db.size = ifile.Length >> 10;
                db.tags = new List<string>();
                db.name = ifile.Name.Substring(0, ifile.Name.Length - ex.Length);

                // PIC
                if (ex.Equals(".jpg") || ex.Equals(".jpeg") || ex.Equals(".png") || ex.Equals(".bmp"))
                {
                    db.type = "PIC";
                    PVproject.IN.Add(db);
                    db.tags.Add("FileType:pic");
                    continue;
                }

                // GIF
                if (ex.Equals(".gif"))
                {
                    db.type = "GIF";
                    PVproject.IN.Add(db);
                    db.tags.Add("FileType:gif");
                    continue;
                }

                // 其余全是 MOV
                db.type = "MOV";
                PVproject.IN.Add(db);
                db.tags.Add("FileType:mov");
            }
            foreach (DirectoryInfo idir in Folders)
            {
                FileInfo[] files = idir.GetFiles();
                if (files.Length == 0) { continue; }

                string extension = files[0].Extension;
                if (!extension.Equals(".jpg") && !extension.Equals(".jpeg") && !extension.Equals(".png") && !extension.Equals(".bmp")) { continue; }

                bool allsametype = true;
                foreach (FileInfo ifile in files) { if (!extension.Equals(ifile.Extension)) { allsametype = false; break; } }
                if (!allsametype) { continue; }

                PVproject.DATABASE db = new PVproject.DATABASE();
                db.extension = files[0].Extension;
                db.size = files.Length;
                db.tags = new List<string>();
                db.tags.Add("FileType:cmc");
                db.name = idir.Name;
                db.type = "CMC";
                PVproject.IN.Add(db);
            }
        }
    }
}
