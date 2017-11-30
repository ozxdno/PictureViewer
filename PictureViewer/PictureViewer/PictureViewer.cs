using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Files.Manager.Open();
            Forms.Manager.Open();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Forms.Manager.MainViewer);

            Files.Manager.Close();
            Forms.Manager.Close();
        }
    }
}
