using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PictureViewer.Class
{
    /// <summary>
    /// 对图片进行快速匹配
    /// </summary>
    class PicMatch
    {
        ///////////////////////////////////////////////////// public attribute ///////////////////////////////////////////////
        
        public bool Abort;

        ///////////////////////////////////////////////////// private attribute ///////////////////////////////////////////////

        private Form_Find.MODE mode = Form_Find.MODE.FULL_SAME_NOTURN;
        private int begin = 0;
        private int end = 0;
        private int degree = 80;
        private int pixes = 100;

        private Form_Find.PICTURE sour;
        private Form_Find.PICTURE dest;

        ///////////////////////////////////////////////////// public method ///////////////////////////////////////////////

        public void Initialize(Form_Find.MODE mode, int begin, int end, Form_Find.PICTURE sour)
        {
            this.mode = mode;
            this.begin = begin;
            this.end = end;
            this.sour = sour;
        }
        public void Start()
        {
            if (begin < 0) { begin = 0; }
            if (end < 0) { end = 0; }
            if (begin >= Form_Find.PictureFiles.Count) { begin = Form_Find.PictureFiles.Count - 1; }
            if (end >= Form_Find.PictureFiles.Count) { end = Form_Find.PictureFiles.Count - 1; }

            this.mode = Form_Find.MODE.PART_SAME_NOTURN;

            for (; !Abort && begin <= end; begin++)
            {
                // 更新参数
                dest = Form_Find.PictureFiles[begin];

                // 比较
                if (cmpRow() && cmpCol()) {  }
            }
        }

        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////

        private bool cmpRow()
        {
            #region FULL SAME NO_TURN

            if (mode == Form_Find.MODE.FULL_SAME_NOTURN)
            {
                if (sour.Height != dest.Height || sour.Width != dest.Width) { return false; }

                double pace = sour.Width > pixes ? (double)sour.Width / pixes : 1;
                int permitcnterr = sour.Width > pixes ?
                    pixes * (100 - degree) / 100 :
                    sour.Width * (100 - degree) / 100;
                int permiterr = 10;
                int cnterr = 0;

                for (double p = 0; p < sour.Width; p += pace)
                {
                    int sgray = sour.GraysR[(int)p];
                    int dgray = dest.GraysR[(int)p];

                    int ierr = (sgray - dgray) / 3;
                    if (ierr < 0) { ierr = -ierr; }
                    if (ierr > permiterr) { ++cnterr; }
                    if (cnterr > permitcnterr) { break; }
                }

                return cnterr <= permitcnterr;
            }

            #endregion

            #region FULL SAME TURN

            if (mode == Form_Find.MODE.FULL_SAME_TURN)
            {
                
            }

            #endregion

            #region FULL LIKE NO_TURN

            if (mode == Form_Find.MODE.FULL_LIKE_NOTURN)
            {

            }

            #endregion

            #region FULL LIKE TURN

            if (mode == Form_Find.MODE.FULL_LIKE_TURN)
            {

            }

            #endregion

            #region PART SAME NO_TURN

            if (mode == Form_Find.MODE.PART_SAME_NOTURN)
            {
                double rate = sour.Height > dest.Height ?
                    (double)sour.Height / dest.Height :
                    (double)dest.Height / sour.Height;
                int errsize = sour.Height > dest.Height ?
                    (int)(sour.Width / rate) - dest.Width :
                    (int)(dest.Width / rate) - sour.Width;

                if (errsize < 0) { errsize = -errsize; }
                if (errsize > 1) { return false; }

                int cmpixes = sour.Width < dest.Width ?
                    (sour.Width < pixes ? sour.Width : pixes) :
                    (dest.Width < pixes ? dest.Width : pixes);
                double space = (double)sour.Width / cmpixes;
                double dpace = (double)dest.Width / cmpixes;
                int permitcnterr = cmpixes * (100 - degree) / 100;
                int permiterr = 10;
                int cnterr = 0;

                for (double s = 0, d = 0; s < sour.Width; s += space, d += dpace)
                {
                    int sgray = sour.GraysR[(int)s];
                    int dgray = dest.GraysR[(int)d];

                    int ierr = (sgray - dgray) / 3;
                    if (ierr < 0) { ierr = -ierr; }
                    if (ierr > permiterr) { ++cnterr; }
                    if (cnterr > permitcnterr) { break; }
                }

                return cnterr <= permitcnterr;
            }

            #endregion

            #region PART SAME TURN

            if (mode == Form_Find.MODE.PART_SAME_TURN)
            {
                
            }

            #endregion

            #region PATR LIKE NO_TURN

            if (mode == Form_Find.MODE.PART_LIKE_NOTURN)
            {

            }

            #endregion

            #region PART LIKE TURN

            if (mode == Form_Find.MODE.PART_LIKE_TURN)
            {

            }

            #endregion

            return false;
        }
        private bool cmpCol()
        {
            #region FULL SAME NO_TURN

            if (mode == Form_Find.MODE.FULL_SAME_NOTURN)
            {
                if (sour.Height != dest.Height || sour.Width != dest.Width) { return false; }

                double pace = sour.Width > pixes ? (double)sour.Width / pixes : 1;
                int permitcnterr = sour.Width > pixes ?
                    pixes * (100 - degree) / 100 :
                    sour.Width * (100 - degree) / 100;
                int permiterr = 10;
                int cnterr = 0;

                for (double p = 0; p < sour.Width; p += pace)
                {
                    int sgray = sour.GraysR[(int)p];
                    int dgray = dest.GraysR[(int)p];

                    int ierr = (sgray - dgray) / 3;
                    if (ierr < 0) { ierr = -ierr; }
                    if (ierr > permiterr) { ++cnterr; }
                    if (cnterr > permitcnterr) { break; }
                }

                return cnterr <= permitcnterr;
            }

            #endregion

            #region FULL SAME TURN

            if (mode == Form_Find.MODE.FULL_SAME_TURN)
            {

            }

            #endregion

            #region FULL LIKE NO_TURN

            if (mode == Form_Find.MODE.FULL_LIKE_NOTURN)
            {

            }

            #endregion

            #region FULL LIKE TURN

            if (mode == Form_Find.MODE.FULL_LIKE_TURN)
            {

            }

            #endregion

            #region PART SAME NO_TURN

            if (mode == Form_Find.MODE.PART_SAME_NOTURN)
            {
                double rate = sour.Height > dest.Height ?
                    (double)sour.Height / dest.Height :
                    (double)dest.Height / sour.Height;
                int errsize = sour.Height > dest.Height ?
                    (int)(sour.Width / rate) - dest.Width :
                    (int)(dest.Width / rate) - sour.Width;

                if (errsize < 0) { errsize = -errsize; }
                if (errsize > 1) { return false; }

                int cmpixes = sour.Height < dest.Height ?
                    (sour.Height < pixes ? sour.Height : pixes) :
                    (dest.Height < pixes ? dest.Height : pixes);
                double space = (double)sour.Height / cmpixes;
                double dpace = (double)dest.Height / cmpixes;
                int permitcnterr = cmpixes * (100 - degree) / 100;
                int permiterr = 10;
                int cnterr = 0;

                for (double s = 0, d = 0; s < sour.Height; s += space, d += dpace)
                {
                    int sgray = sour.GraysC[(int)s];
                    int dgray = dest.GraysC[(int)d];

                    int ierr = (sgray - dgray) / 3;
                    if (ierr < 0) { ierr = -ierr; }
                    if (ierr > permiterr) { ++cnterr; }
                    if (cnterr > permitcnterr) { break; }
                }

                return cnterr <= permitcnterr;
            }

            #endregion

            #region PART SAME TURN

            if (mode == Form_Find.MODE.PART_SAME_TURN)
            {

            }

            #endregion

            #region PATR LIKE NO_TURN

            if (mode == Form_Find.MODE.PART_LIKE_NOTURN)
            {

            }

            #endregion

            #region PART LIKE TURN

            if (mode == Form_Find.MODE.PART_LIKE_TURN)
            {

            }

            #endregion

            return false;
        }
    }
}
