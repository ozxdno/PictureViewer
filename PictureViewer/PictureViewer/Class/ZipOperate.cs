using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Drawing;
using System.Drawing.Drawing2D;
using ICSharpCode.SharpZipLib.Zip;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Checksums;

namespace PictureViewer.Class
{
    class ZipOperate
    {
        ////////////////////////////////////////////////////// public attribute /////////////////////////////////////////////////////

        public static List<string> PassWords = new List<string>() {""};
        public static string PassWord;

        ////////////////////////////////////////////////////// private attribute /////////////////////////////////////////////////////
        
        private static ZipArchive zip;
        private static string zip2;

        private static List<string> knownPassWordFile = new List<string>();
        private static List<string> knownPassWord = new List<string>();

        ////////////////////////////////////////////////////// public method /////////////////////////////////////////////////////

        /// <summary>
        /// 添加备用密码
        /// </summary>
        /// <param name="pw">密码字符串</param>
        public static void A_PassWord(string pw)
        {
            // 去除头部
            if (pw[0] == '+') { pw = pw.Substring(1); }

            // 分割并加入密码列表
            List<string> newpws = pw.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            PassWords.InsertRange(0, newpws);
        }
        /// <summary>
        /// 删除备用密码
        /// </summary>
        /// <param name="pw">密码字符串</param>
        public static void D_PassWord(string pw)
        {
            // 去除头部
            pw = pw.Substring(1);

            // 分割并去除
            string[] newpws = pw.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < newpws.Length; i++)
            {
                int index = FileOperate.Search(PassWords, newpws[i]);
                if (index == -1) { continue; }
                PassWords.RemoveAt(index);
            }
        }
        



        /// <summary>
        /// 读取 ZIP 文件中的所有支持文件
        /// </summary>
        /// <param name="fullpath"></param>
        public static void ReadZip(string fullpath)
        {
            zip = System.IO.Compression.ZipFile.OpenRead(fullpath);
            Form_Main.config.SubFiles = new List<string>();

            foreach (ZipArchiveEntry e in zip.Entries)
            {
                int type = FileOperate.getFileType(FileOperate.getExtension(e.Name));
                if (type == -1 || type == 0 || type == 1 || type == 5 || type == 4)
                { continue; }

                Form_Main.config.SubFiles.Add(e.Name);
            }
        }
        /// <summary>
        /// 读取 ZIP 压缩包中的指定图片文件加载至缓存中，返回加载是否成功（无密码）
        /// </summary>
        public static bool LoadPicture()
        {
            string name = Form_Main.config.SubFiles[Form_Main.config.SubIndex];

            foreach (ZipArchiveEntry e in zip.Entries)
            {
                if (e.Name != name) { continue; }
                Form_Main.config.SourPicture = Image.FromStream(e.Open()); return true;
            }

            return false;
        }
        /// <summary>
        /// 读取 ZIP 压缩包中的指定 GIF 文件加载至缓存中，返回加载是否成功（无密码）
        /// </summary>
        /// <returns></returns>
        public static bool LoadGif()
        {
            string name = Form_Main.config.SubFiles[Form_Main.config.SubIndex];

            foreach (ZipArchiveEntry e in zip.Entries)
            {
                if (e.Name != name) { continue; }
                Form_Main.config.SourPicture = Image.FromStream(e.Open()); return true;
            }

            return false;
        }
        /// <summary>
        /// 读取 ZIP 压缩包中的指定视频文件加载至缓存中，返回加载是否成功（无密码）。
        /// 不支持Vedio读取功能，永远返回 FALSE。
        /// </summary>
        /// <returns></returns>
        public static bool LoadVideo()
        {
            return false;
        }




        /// <summary>
        /// 读取 ZIP 文件中的所有支持文件（有密码），返回错误类型：1-密码错误
        /// </summary>
        /// <param name="fullpath">ZIP 文件绝对路径</param>
        public static int ReadZipEX(string fullpath)
        {
            Form_Main.config.SubFiles = new List<string>();
            zip2 = fullpath;
            if (!TestPassWords()) { return 1; }

            FileStream sIn = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
            ICSharpCode.SharpZipLib.Zip.ZipInputStream zip = new ZipInputStream(sIn);
            zip.Password = PassWord;
            
            ICSharpCode.SharpZipLib.Zip.ZipEntry e = zip.GetNextEntry();
            while (e != null)
            {
                int type = FileOperate.getFileType(FileOperate.getExtension(e.Name));

                if (type == 2) { Form_Main.config.SubFiles.Add(e.Name); }
                if (type == 3) { Form_Main.config.SubFiles.Add(e.Name); }

                e = zip.GetNextEntry();
            }

            return 0;
        }
        /// <summary>
        /// 读取 ZIP 文件中的压缩文件列表。
        /// </summary>
        /// <param name="fullpath">ZIP 文件绝对路径</param>
        /// <returns></returns>
        public static List<string> getZipFileEX(string fullpath)
        {
            List<string> files = new List<string>();
            zip2 = fullpath;
            if (!TestPassWords()) { return files; }

            FileStream sIn = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
            ICSharpCode.SharpZipLib.Zip.ZipInputStream zip = new ZipInputStream(sIn);
            zip.Password = PassWord;

            ICSharpCode.SharpZipLib.Zip.ZipEntry e = zip.GetNextEntry();
            while (e != null)
            {
                int type = FileOperate.getFileType(FileOperate.getExtension(e.Name));

                if (type == 2) { files.Add(e.Name); }
                if (type == 3) { files.Add(e.Name); }

                e = zip.GetNextEntry();
            }

            return files;
        }
        /// <summary>
        /// 读取 ZIP 压缩包中的指定图片文件加载至缓存中，返回加载是否成功（有密码）
        /// </summary>
        public static bool LoadPictureEX()
        {
            FileStream sIn = new FileStream(zip2, FileMode.Open, FileAccess.Read);
            ZipInputStream zip = new ZipInputStream(sIn);

            string name = Form_Main.config.SubFiles[Form_Main.config.SubIndex];
            zip.Password = PassWord;
            ICSharpCode.SharpZipLib.Zip.ZipEntry e = zip.GetNextEntry();

            while (e != null)
            {
                if (e.Name != name) { e = zip.GetNextEntry(); continue; }
                Form_Main.config.SourPicture = Image.FromStream(zip);
                return true;
            }

            return false;
        }
        /// <summary>
        /// 读取 ZIP 压缩包中的 GIF 文件加载至缓存中，返回加载是否成功（有密码）
        /// </summary>
        public static bool LoadGifEX()
        {
            FileStream sIn = new FileStream(zip2, FileMode.Open, FileAccess.Read);
            ZipInputStream zip = new ZipInputStream(sIn);

            string name = Form_Main.config.SubFiles[Form_Main.config.SubIndex];
            zip.Password = PassWord;
            ICSharpCode.SharpZipLib.Zip.ZipEntry e = zip.GetNextEntry();

            while (e != null)
            {
                if (e.Name != name) { e = zip.GetNextEntry(); continue; }
                Form_Main.config.SourPicture = Image.FromStream(zip);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 测试所列出的所有密码，把正确的结果保存到 PassWord 中，密码错误返回 FALSE。
        /// </summary>
        /// <param name="zipFilePath">ZIP 文件全路径</param>
        /// <returns></returns>
        private static bool TestPassWords(string zipFilePath = null)
        {
            if (zipFilePath == null) { zipFilePath = zip2; }

            // 若文件已经通过密码测试，则不再测试。
            for (int i = 0; i < knownPassWordFile.Count; i++)
            {
                if (knownPassWordFile[i] == zipFilePath) { PassWord = knownPassWord[i]; return true; }
            }

            // 在已有密码中没找到，则测试密码。
            foreach (string pw in PassWords)
            {
                try
                {
                    FileStream sIn = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read);
                    ICSharpCode.SharpZipLib.Zip.ZipInputStream zip = new ZipInputStream(sIn);

                    zip.Password = pw;
                    if (zip.Password.Length == 0) { zip.Password = null; }

                    ICSharpCode.SharpZipLib.Zip.ZipEntry e = zip.GetNextEntry();
                    //if (e == null) { return false; }
                    //e = zip.GetNextEntry();
                    while (e != null) { e = zip.GetNextEntry(); }

                    PassWord = pw; if (PassWord.Length == 0) { PassWord = null; }
                    knownPassWordFile.Add(zipFilePath);
                    knownPassWord.Add(PassWord);
                    return true;
                }
                catch { }
            }

            return false;
        }
        /// <summary>
        /// 加密算法，返回解码后密码（好像不用也行啊，不知道是什么东西）
        /// </summary>
        /// <param name="str">原密码</param>
        /// <returns></returns>
        private static string LockMethod_MD5(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            for (int i = 0; i < s.Length; i++)
            {
                pwd = pwd + s[i].ToString("X");
            }
            return pwd;
        }

        ////////////////////////////////////////////////////// private method /////////////////////////////////////////////////////
    }
}
