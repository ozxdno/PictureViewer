using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Forms
{
    /// <summary>
    /// 静态类，窗体管理器的所有数据。
    /// </summary>
    class Manager
    {
        /// <summary>
        /// 主查看器
        /// </summary>
        public static Form_Main MainViewer
        {
            set;
            get;
        }
        /// <summary>
        /// 搜索器
        /// </summary>
        public static Form_Search Seacher
        {
            set;
            get;
        }
        /// <summary>
        /// 文件列表浏览器
        /// </summary>
        public static Form_List Lister
        {
            set;
            get;
        }
        /// <summary>
        /// 从查看器
        /// </summary>
        public static List<Form_View> SubViewers
        {
            get;
            set;
        }
        /// <summary>
        /// 属性查看器
        /// </summary>
        public static Form_Attribute Attributer
        {
            set;
            get;
        }
        
        /// <summary>
        /// 打开窗体管理器
        /// </summary>
        public static void Open()
        {
            MainViewer = new Form_Main();
            Seacher = new Form_Search();
            Lister = new Form_List();
            SubViewers = new List<Form_View>();
            Attributer = new Form_Attribute();

            Files.Load pvini = new Files.Load(Files.Manager.Pvini.Model);
            MainViewer.LoadConfig();
        }
        /// <summary>
        /// 关闭窗体管理器
        /// </summary>
        public static void Close()
        {
            //try { MainViewer.Visible = false; } catch { }
            //try { Seacher.Visible = false; } catch { }
            //try { Lister.Visible = false; } catch { }
            //try { Attributer.Visible = false; } catch { }

            //for (int i = 0; i < SubViewers.Count; i++)
            //{ try { SubViewers[i].Visible = false; } catch { } }
        }

        /// <summary>
        /// 请求释放正在操作的文件
        /// </summary>
        /// <returns></returns>
        public static void Asking()
        {
            MainViewer.Answered = false;
        }
        /// <summary>
        /// 请求已经被回答，文件已经被释放。
        /// </summary>
        /// <returns></returns>
        public static bool Answered()
        {
            return MainViewer.Answered;
        }
    }
}
