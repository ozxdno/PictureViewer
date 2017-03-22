using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PictureViewer.Method
{
    class OutputFile
    {
        public static void OutputFile_CFG()
        {
            if (PVproject.sw != null) { throw new Exception("File is Writing !"); }
            if (!Directory.Exists(PVproject.CFG.exe_path)) { throw new Exception("Path unavailable !"); }

            string fullpath = PVproject.CFG.exe_path + "\\pv.pvini";

            PVproject.sw = new StreamWriter(fullpath, false);

            PVproject.sw.WriteLine("output path:"+PVproject.CFG.o_path);
            PVproject.sw.WriteLine("export path:" + PVproject.CFG.e_path);
            PVproject.sw.WriteLine("database path:" + PVproject.CFG.database_path);
            PVproject.sw.WriteLine("database name:" + PVproject.CFG.database_name);
            PVproject.sw.WriteLine("last file id:" + PVproject.CFG.LastFileID.ToString());
            PVproject.sw.WriteLine("view at:" + PVproject.CFG.viewIndex.ToString());
            PVproject.sw.WriteLine("process:" +
                PVproject.CFG.pic_process + "|" +
                PVproject.CFG.gif_process + "|" +
                PVproject.CFG.mov_process);
            
            PVproject.sw.WriteLine(">>Tags");
            string NewLine = "";
            foreach (PVproject.TAG.LIST ilist in PVproject.tag.list)
            {
                NewLine = ilist.name + ":" + ilist.tags[0];
                for (int i=1; i<ilist.tags.Count; i++) { NewLine += "|" + ilist.tags[i]; }
                PVproject.sw.WriteLine(NewLine);
            }

            PVproject.sw.WriteLine(">>Exps");
            for (int i = 0; i < PVproject.tag.TagExps.Count; i++)
            {
                if (PVproject.tag.TagExps[i].Length == 0) { continue; }
                PVproject.sw.WriteLine(PVproject.tag.TagStrs[i] + "|" + PVproject.tag.TagExps[i]);
            }

            PVproject.sw.Close();
            PVproject.sw = null;
        }
        public static void OutputFile_DB()
        {
            if (PVproject.sw != null) { throw new Exception("File is Writing !"); }
            if (!Directory.Exists(PVproject.CFG.database_path)) { throw new Exception("Path unavailable !"); }
            
            string fullpath = PVproject.CFG.database_path + "\\" + PVproject.CFG.database_name;
            //File.Delete(fullpath);
            PVproject.sw = new StreamWriter(fullpath, false);

            string line = "";
            foreach(PVproject.DATABASE db in PVproject.DB)
            {
                line = "";

                // Name,type,extension,size,tags
                line += db.name;
                line += "|" + db.type;
                line += "|" + db.extension;
                line += "|" + db.size.ToString();
                foreach (string itag in db.tags) { line += "|" + itag; }

                PVproject.sw.WriteLine(line);
            }
            PVproject.sw.Close();
            PVproject.sw = null;
        }
    }
}
