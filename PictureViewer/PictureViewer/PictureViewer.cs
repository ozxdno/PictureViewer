using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PictureViewer.Class;

namespace PictureViewer
{
    static class PictureViewer
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Files.Config.Initialize();
            MainForm.Config.Initialize();
            FindForm.Config.Initialize();
            ImageForm.Config.Initialize();
            SearchForm.Config.Initialize();
            InputForm.Config.Initialize();
            Strings.Language.Initialize();

            //Files.Load_pvdata.thread();
            Files.Load_pvini.thread();

            Files.Config.SetDefault();
            MainForm.Config.SetDefault();
            FindForm.Config.SetDefault();
            ImageForm.Config.SetDefault();
            SearchForm.Config.SetDefault();
            InputForm.Config.SetDefault();
            Strings.Language.SetDefault();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form_Main());
        }
    }
}
