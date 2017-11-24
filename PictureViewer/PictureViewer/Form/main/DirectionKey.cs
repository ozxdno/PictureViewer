using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.MainForm
{
    class DirectionKey
    {
        public bool IsDownAny
        {
            get { return DownL || DownR || DownU || DownD; }
        }
        public bool IsDownOnlyL
        {
            get { return DownL && !DownR && !DownU && !DownD; }
        }

        public bool DownL
        {
            set;
            get;
        }
        public bool DownR
        {
            set;
            get;
        }
        public bool DownU
        {
            set;
            get;
        }
        public bool DownD
        {
            set;
            get;
        }
        
        public bool Down(int keyValue)
        {
            if (keyValue == Config.FastKey_L) { DownL = true; return true; }
            if (keyValue == Config.FastKey_R) { DownR = true; return true; }
            if (keyValue == Config.FastKey_U) { DownU = true; return true; }
            if (keyValue == Config.FastKey_D) { DownD = true; return true; }

            return false;
        }
        public bool Up(int keyValue)
        {
            if (keyValue == Config.FastKey_L) { DownL = false; return true; }
            if (keyValue == Config.FastKey_R) { DownR = false; return true; }
            if (keyValue == Config.FastKey_U) { DownU = false; return true; }
            if (keyValue == Config.FastKey_D) { DownD = false; return true; }

            return false;
        }
    }
}
