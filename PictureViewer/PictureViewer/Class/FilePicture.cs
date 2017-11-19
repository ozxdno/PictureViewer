using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PictureViewer.Class
{
    class FilePicture
    {
        public static int getIndex(string path, string name)
        {
            for (int i = 0; i < Form_Find.PictureFiles.Count; i++)
            {
                if (path == Form_Find.PictureFiles[i].Path && name == Form_Find.PictureFiles[i].Name) { return i; }
            }

            return -1;
        }
        public static int getIndex(string full)
        {
            for (int i = 0; i < Form_Find.PictureFiles.Count; i++)
            {
                if (full == Form_Find.PictureFiles[i].Full) { return i; }
            }

            return -1;
        }
        public static int getIndex(int folder, int file, int sub = -1)
        {
            int index = -1;
            for (int i = 0; i < Form_Find.PictureFiles.Count; i++)
            {
                for (int j = 0; j < Form_Find.PictureFiles[i].FolderIndexes.Count; j++)
                {
                    if (Form_Find.PictureFiles[i].FolderIndexes[j] != folder) { continue; }
                    if (Form_Find.PictureFiles[i].FileIndexes[j] != file) { continue; }

                    if (Form_Find.PictureFiles[i].SubIndexes[j] == -1) { index = i; }
                    if (Form_Find.PictureFiles[i].SubIndexes[j] == 0) { index = i; }
                    if (Form_Find.PictureFiles[i].SubIndexes[j] == sub) { return i; }
                }
            }
            return index;
        }

        public static void rotate(int index, int angle)
        {
            if (index < 0 || index >= Form_Find.PictureFiles.Count) { return; }
            Form_Find.PICTURE p = Form_Find.PictureFiles[index];

            try { FileInfo f = new FileInfo(p.Full); p.Time = f.LastWriteTime.ToFileTime(); p.Length = f.Length; }
            catch { Form_Find.PictureFiles.RemoveAt(index); }
            if (!p.Loaded) { return; }

            int h = p.Height;
            int w = p.Width;
            int r = p.Row;
            int c = p.Col;
            int[] gr = new int[p.GraysR.Length];
            int[] gc = new int[p.GraysC.Length];

            #region 090

            if (angle == 090)
            {
                p.Height = w;
                p.Width = h;
                p.Row = c;
                p.Col = r;

                for (int i = 0; i < p.GraysR.Length; i++)
                {
                    gc[p.GraysR.Length - 1 - i] = p.GraysR[i];
                }
                for (int i = 0; i < p.GraysC.Length; i++)
                {
                    gr[i] = p.GraysC[i];
                }

                p.GraysC = gc;
                p.GraysR = gr;
            }

            #endregion

            #region 180

            if (angle == 180)
            {
                for (int i = 0; i < p.GraysR.Length; i++)
                {
                    gr[p.GraysR.Length - 1 - i] = p.GraysR[i];
                }
                for (int i = 0; i < p.GraysC.Length; i++)
                {
                    gc[p.GraysC.Length - 1 - i] = p.GraysC[i];
                }

                p.GraysR = gr;
                p.GraysC = gc;
            }

            #endregion

            #region 270

            if (angle == 270)
            {
                p.Height = w;
                p.Width = h;
                p.Row = c;
                p.Col = r;

                for (int i = 0; i < p.GraysR.Length; i++)
                {
                    gc[i] = p.GraysR[i];
                }
                for (int i = 0; i < p.GraysC.Length; i++)
                {
                    gr[p.GraysC.Length - 1 - i] = p.GraysC[i];
                }

                p.GraysR = gr;
                p.GraysC = gc;
            }

            #endregion

            Form_Find.PictureFiles[index] = p;
        }
        public static void flipx(int index)
        {
            if (index < 0 || index >= Form_Find.PictureFiles.Count) { return; }
            Form_Find.PICTURE p = Form_Find.PictureFiles[index];

            try { FileInfo f = new FileInfo(p.Full); p.Time = f.LastWriteTime.ToFileTime(); p.Length = f.Length; }
            catch { Form_Find.PictureFiles.RemoveAt(index); }
            if (!p.Loaded) { return; }
            
            int[] gr = new int[p.GraysR.Length];
            for (int i = 0; i < p.GraysR.Length; i++)
            {
                gr[p.GraysR.Length - 1 - i] = p.GraysR[i];
            }
            p.GraysR = gr;

            Form_Find.PictureFiles[index] = p;
        }
        public static void flipy(int index)
        {
            if (index < 0 || index >= Form_Find.PictureFiles.Count) { return; }
            Form_Find.PICTURE p = Form_Find.PictureFiles[index];

            try { FileInfo f = new FileInfo(p.Full); p.Time = f.LastWriteTime.ToFileTime(); p.Length = f.Length; }
            catch { Form_Find.PictureFiles.RemoveAt(index); }
            if (!p.Loaded) { return; }

            int[] gc = new int[p.GraysC.Length];
            for (int i = 0; i < p.GraysC.Length; i++)
            {
                gc[p.GraysC.Length - 1 - i] = p.GraysC[i];
            }
            p.GraysC = gc;

            Form_Find.PictureFiles[index] = p;
        }
        public static void rename(int index, string dest)
        {
            if (index < 0 || index >= Form_Find.PictureFiles.Count) { return; }
            Form_Find.PICTURE p = Form_Find.PictureFiles[index];
            p.Name = dest;
            p.Full = p.Path + "\\" + p.Name;

            try { FileInfo f = new FileInfo(p.Full); p.Time = f.LastWriteTime.ToFileTime(); p.Length = f.Length; }
            catch { Form_Find.PictureFiles.RemoveAt(index); }

            Form_Find.PictureFiles[index] = p;
        }
        public static void moveto(int index, string dest)
        {
            if (index < 0 || index >= Form_Find.PictureFiles.Count) { return; }
            Form_Find.PICTURE p = Form_Find.PictureFiles[index];
            p.Path = FileOperate.getPath(dest);
            p.Name = FileOperate.getName(dest);
            p.Full = dest;

            try { FileInfo f = new FileInfo(p.Full); p.Time = f.LastWriteTime.ToFileTime(); p.Length = f.Length; }
            catch { Form_Find.PictureFiles.RemoveAt(index); }

            Form_Find.PictureFiles[index] = p;
        }
        public static void delete(int index)
        {
            if (index < 0 || index >= Form_Find.PictureFiles.Count) { return; }
            Form_Find.PictureFiles.RemoveAt(index);
        }
    }
}
