using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace PictureViewer.Class
{
    /// <summary>
    /// 对图片进行快速匹配
    /// </summary>
    class PicMatch
    {
        ///////////////////////////////////////////////////// public attribute ///////////////////////////////////////////////
        
        /// <summary>
        /// 结束循环
        /// </summary>
        public bool Abort;
        
        ///////////////////////////////////////////////////// private attribute ///////////////////////////////////////////////

        private Form_Find.MODE mode = Form_Find.MODE.FULL_SAME_NOTURN;
        private int degree = 0;
        private int pixes = 0;

        private bool[] compared;

        private Form_Find.PICTURE sour;
        private Form_Find.PICTURE dest;
        
        ///////////////////////////////////////////////////// public method ///////////////////////////////////////////////
        
        /// <summary>
        /// 初始化，当重新开始搜索时调用。
        /// 继续搜索则不应该调用。
        /// </summary>
        public void Initialize()
        {
            compared = new bool[Form_Find.config.Sour.Count];
            for (int i = 0; i < Form_Find.config.Sour.Count; i++) { compared[i] = false; }
        }

        /// <summary>
        /// 开始查找
        /// </summary>
        public void Start()
        {
            Form_Find.IsFinish = false;
            Abort = false;

            if (Form_Find.config.Method == 0) { Cmp0(); }
            if (Form_Find.config.Method == 1) { Cmp1(); }
            if (Form_Find.config.Method == 2) { Cmp2(); }

            if (!Abort && Form_Find.config.Dest != null) { Form_Find.IndexD = Form_Find.config.Dest.Count; }
            Form_Find.IsFinish = true;
        }

        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////

        private bool loadPicture(int index)
        {
            Form_Find.PICTURE pic = Form_Find.PictureFiles[index];
            if (pic.Loaded) { return true; }
            if (!File.Exists(pic.Full)) { return false; }

            Bitmap img = (Bitmap)Image.FromFile(pic.Full);
            pic.Loaded = true;
            pic.Height = img.Height;
            pic.Width = img.Width;
            pic.Row = pic.Height / 2;
            pic.Col = pic.Width / 2;
            pic.GraysR = new int[pic.Width];
            pic.GraysC = new int[pic.Height];
            for (int i = 0; i < pic.Width; i++) { pic.GraysR[i] = toGray(img.GetPixel(i, pic.Row)); }
            for (int i = 0; i < pic.Height; i++) { pic.GraysC[i] = toGray(img.GetPixel(pic.Col, i)); }

            img.Dispose(); Form_Find.PictureFiles[index] = pic;
            return true;
        }
        private int toGray(Color c)
        {
            return (c.R + c.G + c.B) / 3;
        }

        private void Cmp0()
        {
            if (Form_Find.config.Sour.Count != 1) { return; }
            Form_Find.config.Standard.Clear();
            Form_Find.config.Standard.Add(Form_Find.config.Sour[0]);
            if (Form_Find.Results.Count == 0) { Form_Find.Results.Add(new List<int>()); }
            
            for (; !Abort && Form_Find.IndexS < Form_Find.config.Sour.Count; Form_Find.IndexS++)
            {
                if (!loadPicture(Form_Find.config.Sour[Form_Find.IndexS])) { continue; }
                sour = Form_Find.PictureFiles[Form_Find.config.Sour[Form_Find.IndexS]];

                for (; !Abort && Form_Find.IndexD < Form_Find.config.Dest.Count; Form_Find.IndexD++)
                {
                    if (!loadPicture(Form_Find.config.Dest[Form_Find.IndexD])) { continue; }
                    dest = Form_Find.PictureFiles[Form_Find.config.Dest[Form_Find.IndexD]];
                    mode = Form_Find.config.Mode;
                    degree = Form_Find.config.Degree;
                    pixes = Form_Find.config.MinCmpPix;

                    if (!cmpRow()) { continue; }
                    if (!cmpCol()) { continue; }

                    Form_Find.Results[0].Add(Form_Find.config.Dest[Form_Find.IndexD]);
                }

                if (!Abort) { Form_Find.IndexD = 0; } else { break; }
            }
        }
        private void Cmp1()
        {
            for (; !Abort && Form_Find.IndexS < Form_Find.config.Sour.Count; Form_Find.IndexS++)
            {
                if (compared[Form_Find.IndexS]) { continue; }
                if (!loadPicture(Form_Find.config.Sour[Form_Find.IndexS])) { continue; }
                sour = Form_Find.PictureFiles[Form_Find.config.Sour[Form_Find.IndexS]];
                
                bool newResultItem = false;

                for (; !Abort && Form_Find.IndexD < Form_Find.config.Sour.Count; Form_Find.IndexD++)
                {
                    if (Form_Find.IndexD <= Form_Find.IndexS) { continue; }
                    if (compared[Form_Find.IndexD]) { continue; }
                    if (!loadPicture(Form_Find.config.Sour[Form_Find.IndexD])) { continue; }
                    dest = Form_Find.PictureFiles[Form_Find.config.Sour[Form_Find.IndexD]];
                    mode = Form_Find.config.Mode;
                    degree = Form_Find.config.Degree;
                    pixes = Form_Find.config.MinCmpPix;

                    if (!cmpRow()) { continue; }
                    if (!cmpCol()) { continue; }

                    if (!newResultItem)
                    {
                        newResultItem = true;
                        Form_Find.Results.Add(new List<int>());
                        Form_Find.config.Standard.Add(Form_Find.config.Sour[Form_Find.IndexS]);
                    }
                    Form_Find.Results[Form_Find.Results.Count - 1].Add(Form_Find.config.Sour[Form_Find.IndexD]);
                    compared[Form_Find.IndexD] = true;
                }
                
                if (!Abort) { Form_Find.IndexD = Form_Find.IndexS + 2; } else { break; }
            }
        }
        private void Cmp2()
        {
            for (; !Abort && Form_Find.IndexS < Form_Find.config.Sour.Count; Form_Find.IndexS++)
            {
                if (!loadPicture(Form_Find.config.Sour[Form_Find.IndexS])) { continue; }
                sour = Form_Find.PictureFiles[Form_Find.config.Sour[Form_Find.IndexS]];

                bool newResultItem = false;

                for (; !Abort && Form_Find.IndexD < Form_Find.config.Dest.Count; Form_Find.IndexD++)
                {
                    if (!loadPicture(Form_Find.config.Dest[Form_Find.IndexD])) { continue; }
                    dest = Form_Find.PictureFiles[Form_Find.config.Dest[Form_Find.IndexD]];
                    mode = Form_Find.config.Mode;
                    degree = Form_Find.config.Degree;
                    pixes = Form_Find.config.MinCmpPix;

                    if (!cmpRow()) { continue; }
                    if (!cmpCol()) { continue; }
                    
                    if (!newResultItem)
                    {
                        newResultItem = true;
                        Form_Find.Results.Add(new List<int>());
                        Form_Find.config.Standard.Add(Form_Find.config.Sour[Form_Find.IndexS]);
                    }
                    Form_Find.Results[Form_Find.Results.Count - 1].Add(Form_Find.config.Dest[Form_Find.IndexD]);
                }

                if (!Abort) { Form_Find.IndexD = 0; } else { break; }
            }
        }

        private bool cmpRow()
        {
            #region FULL SAME NO_TURN

            if (mode == Form_Find.MODE.FULL_SAME_NOTURN)
            {
                if (sour.Height != dest.Height || sour.Width != dest.Width) { return false; }

                int cmpixes = sour.Width < pixes ? sour.Width : pixes;
                double pace = (double)sour.Width / cmpixes;
                int permitcnterr = cmpixes * (100 - degree) / 100;
                int permiterr = 10;
                int cnterr = 0;

                for (double p = 0; p < sour.Width; p += pace)
                {
                    int sgray = sour.GraysR[(int)p];
                    int dgray = dest.GraysR[(int)p];

                    int ierr = sgray - dgray;
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
                bool isForm1 = sour.Height == dest.Height && sour.Width == dest.Width;
                bool isForm2 = sour.Height == dest.Width && sour.Width == dest.Height;
                if (!isForm1 && !isForm2) { return false; }
                
                #region 旋转 0 或 180 度

                if (isForm1)
                {
                    int cmpixes = sour.Width < pixes ? sour.Width : pixes;
                    double pace = (double)sour.Width / cmpixes;
                    int permitcnterr = cmpixes * (100 - degree) / 100;
                    int permiterr = 10;
                    int cnterr = 0;

                    // 0
                    for (double p = 0; p < sour.Width; p += pace)
                    {
                        int sgray = sour.GraysR[(int)p];
                        int dgray = dest.GraysR[(int)p];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }

                    if (cnterr <= permitcnterr) { return true; }
                    cnterr = 0;

                    // 180
                    for (double p = 0; p < sour.Width; p += pace)
                    {
                        int sgray = sour.GraysR[(int)p];
                        int dgray = dest.GraysR[sour.Width - 1 - (int)p];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }

                    if (cnterr <= permitcnterr) { return true; }
                }

                #endregion

                #region 旋转 90 或 270 度

                if (isForm2)
                {
                    int cmpixes = sour.Width < pixes ? sour.Width : pixes;
                    double pace = (double)sour.Width / cmpixes;
                    int permitcnterr = cmpixes * (100 - degree) / 100;
                    int permiterr = 10;
                    int cnterr = 0;

                    // 90
                    for (double p = 0; p < sour.Width; p += pace)
                    {
                        int sgray = sour.GraysR[(int)p];
                        int dgray = dest.GraysC[(int)p];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }

                    if (cnterr <= permitcnterr) { return true; }
                    cnterr = 0;

                    // 270
                    for (double p = 0; p < sour.Width; p += pace)
                    {
                        int sgray = sour.GraysR[(int)p];
                        int dgray = dest.GraysC[sour.Width - 1 - (int)p];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }

                    if (cnterr <= permitcnterr) { return true; }
                }

                #endregion

                return false;
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
                double rate = (double)sour.Height / dest.Height;
                double expw = sour.Width / rate;
                bool isSame = dest.Width - expw < 3 && expw - dest.Width < 3;
                if (!isSame) { return false; }

                int cmpixes = sour.Width < dest.Width ?
                    (sour.Width < pixes ? sour.Width : pixes) :
                    (dest.Width < pixes ? dest.Width : pixes);
                double space = (double)sour.Width / cmpixes;
                double dpace = (double)dest.Width / cmpixes;
                int permitcnterr = cmpixes * (100 - degree) / 100;
                int permiterr = 10;
                int cnterr = 0;

                for (double s = 0, d = 0; s < sour.Width && d < dest.Width; s += space, d += dpace) 
                {
                    int sgray = sour.GraysR[(int)s];
                    int dgray = dest.GraysR[(int)d];

                    int ierr = sgray - dgray;
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
                double rate1 = (double)sour.Height / dest.Height;
                double expw1 = sour.Width / rate1;
                double rate2 = (double)sour.Width / dest.Height;
                double expw2 = sour.Height / rate2;

                bool isForm1 = dest.Width - expw1 < 3 && expw1 - dest.Width < 3;
                bool isForm2 = dest.Width - expw2 < 3 && expw2 - dest.Width < 3;
                
                #region 旋转 0 或 180 度

                if (isForm1)
                {
                    int cmpixes = sour.Width < dest.Width ?
                        (sour.Width < pixes ? sour.Width : pixes) :
                        (dest.Width < pixes ? dest.Width : pixes);
                    double space = (double)sour.Width / cmpixes;
                    double dpace = (double)dest.Width / cmpixes;
                    int permitcnterr = cmpixes * (100 - degree) / 100;
                    int permiterr = 10;
                    int cnterr = 0;

                    // 0
                    for (double s = 0, d = 0; s < sour.Width && d < dest.Width; s += space, d += dpace)
                    {
                        int sgray = sour.GraysR[(int)s];
                        int dgray = dest.GraysR[(int)d];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }

                    if (cnterr <= permitcnterr) { return true; }
                    cnterr = 0;

                    // 180
                    for (double s = 0, d = 0; s < sour.Width && d < dest.Width; s += space, d += dpace)
                    {
                        int sgray = sour.GraysR[(int)s];
                        int dgray = dest.GraysR[dest.Width - 1 - (int)d];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }
                    if (cnterr <= permitcnterr) { return true; }
                }

                #endregion

                #region 旋转 90 或 270 度

                if (isForm2)
                {
                    int cmpixes = sour.Width < dest.Height ?
                        (sour.Width < pixes ? sour.Width : pixes) :
                        (dest.Height < pixes ? dest.Height : pixes);
                    double space = (double)sour.Width / cmpixes;
                    double dpace = (double)dest.Height / cmpixes;
                    int permitcnterr = cmpixes * (100 - degree) / 100;
                    int permiterr = 10;
                    int cnterr = 0;

                    // 90
                    for (double s = 0, d = 0; s < sour.Width && d < dest.Height; s += space, d += dpace)
                    {
                        int sgray = sour.GraysR[(int)s];
                        int dgray = dest.GraysC[(int)d];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }

                    if (cnterr <= permitcnterr) { return true; }
                    cnterr = 0;

                    // 270
                    for (double s = 0, d = 0; s < sour.Width && d < dest.Height; s += space, d += dpace)
                    {
                        int sgray = sour.GraysR[(int)s];
                        int dgray = dest.GraysC[dest.Height - 1 - (int)d];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }
                    if (cnterr <= permitcnterr) { return true; }
                }

                #endregion

                return false;
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

                int cmpixes = sour.Height < pixes ? sour.Height : pixes;
                double pace = (double)sour.Height / cmpixes;
                int permitcnterr = cmpixes * (100 - degree) / 100;
                int permiterr = 10;
                int cnterr = 0;

                for (double p = 0; p < sour.Height; p += pace)
                {
                    int sgray = sour.GraysC[(int)p];
                    int dgray = dest.GraysC[(int)p];

                    int ierr = sgray - dgray;
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
                bool isForm1 = sour.Height == dest.Height && sour.Width == dest.Width;
                bool isForm2 = sour.Height == dest.Width && sour.Width == dest.Height;
                if (!isForm1 && !isForm2) { return false; }

                #region 旋转 0 或 180 度

                if (isForm1)
                {
                    int cmpixes = sour.Height < pixes ? sour.Height : pixes;
                    double pace = (double)sour.Height / cmpixes;
                    int permitcnterr = cmpixes * (100 - degree) / 100;
                    int permiterr = 10;
                    int cnterr = 0;

                    // 0
                    for (double p = 0; p < sour.Height; p += pace)
                    {
                        int sgray = sour.GraysC[(int)p];
                        int dgray = dest.GraysC[(int)p];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }

                    if (cnterr <= permitcnterr) { return true; }
                    cnterr = 0;

                    // 180
                    for (double p = 0; p < sour.Height; p += pace)
                    {
                        int sgray = sour.GraysC[(int)p];
                        int dgray = dest.GraysC[sour.Height - 1 - (int)p];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }

                    return cnterr <= permitcnterr;
                }

                #endregion

                #region 旋转 90 或 270 度

                if (isForm2)
                {
                    int cmpixes = sour.Height < pixes ? sour.Height : pixes;
                    double pace = (double)sour.Height / cmpixes;
                    int permitcnterr = cmpixes * (100 - degree) / 100;
                    int permiterr = 10;
                    int cnterr = 0;

                    // 90
                    for (double p = 0; p < sour.Height; p += pace)
                    {
                        int sgray = sour.GraysC[(int)p];
                        int dgray = dest.GraysR[(int)p];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }

                    if (cnterr <= permitcnterr) { return true; }
                    cnterr = 0;

                    // 270
                    for (double p = 0; p < sour.Height; p += pace)
                    {
                        int sgray = sour.GraysC[(int)p];
                        int dgray = dest.GraysR[sour.Height - 1 - (int)p];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }

                    return cnterr <= permitcnterr;
                }

                #endregion

                return false;
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
                double rate = (double)sour.Height / dest.Height;
                double expw = sour.Width / rate;
                bool isSame = dest.Width - expw < 3 && expw - dest.Width < 3;
                if (!isSame) { return false; }

                int cmpixes = sour.Height < dest.Height ?
                    (sour.Height < pixes ? sour.Height : pixes) :
                    (dest.Height < pixes ? dest.Height : pixes);
                double space = (double)sour.Height / cmpixes;
                double dpace = (double)dest.Height / cmpixes;
                int permitcnterr = cmpixes * (100 - degree) / 100;
                int permiterr = 10;
                int cnterr = 0;

                for (double s = 0, d = 0; s < sour.Height && d < dest.Height; s += space, d += dpace)
                {
                    int sgray = sour.GraysC[(int)s];
                    int dgray = dest.GraysC[(int)d];

                    int ierr = sgray - dgray;
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
                double rate1 = (double)sour.Height / dest.Height;
                double expw1 = sour.Width / rate1;
                double rate2 = (double)sour.Width / dest.Height;
                double expw2 = sour.Height / rate2;

                bool isForm1 = dest.Width - expw1 < 3 && expw1 - dest.Width < 3;
                bool isForm2 = dest.Width - expw2 < 3 && expw2 - dest.Width < 3;

                #region 旋转 0 或 180 度

                if (isForm1)
                {
                    int cmpixes = sour.Height < dest.Height ?
                        (sour.Height < pixes ? sour.Height : pixes) :
                        (dest.Height < pixes ? dest.Height : pixes);
                    double space = (double)sour.Height / cmpixes;
                    double dpace = (double)dest.Height / cmpixes;
                    int permitcnterr = cmpixes * (100 - degree) / 100;
                    int permiterr = 10;
                    int cnterr = 0;

                    // 0
                    for (double s = 0, d = 0; s < sour.Height && d < dest.Height; s += space, d += dpace)
                    {
                        int sgray = sour.GraysC[(int)s];
                        int dgray = dest.GraysC[(int)d];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }

                    if (cnterr <= permitcnterr) { return true; }
                    cnterr = 0;

                    // 180
                    for (double s = 0, d = 0; s < sour.Height && d < dest.Height; s += space, d += dpace)
                    {
                        int sgray = sour.GraysC[(int)s];
                        int dgray = dest.GraysC[dest.Height - 1 - (int)d];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }
                    if (cnterr <= permitcnterr) { return true; }
                }

                #endregion

                #region 旋转 90 或 270 度

                if (isForm2)
                {
                    int cmpixes = sour.Height < dest.Width ?
                        (sour.Height < pixes ? sour.Height : pixes) :
                        (dest.Width < pixes ? dest.Width : pixes);
                    double space = (double)sour.Height / cmpixes;
                    double dpace = (double)dest.Width / cmpixes;
                    int permitcnterr = cmpixes * (100 - degree) / 100;
                    int permiterr = 10;
                    int cnterr = 0;

                    // 90
                    for (double s = 0, d = 0; s < sour.Height && d < dest.Width; s += space, d += dpace)
                    {
                        int sgray = sour.GraysC[(int)s];
                        int dgray = dest.GraysR[(int)d];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }

                    if (cnterr <= permitcnterr) { return true; }
                    cnterr = 0;

                    // 270
                    for (double s = 0, d = 0; s < sour.Height && d < dest.Width; s += space, d += dpace)
                    {
                        int sgray = sour.GraysC[(int)s];
                        int dgray = dest.GraysR[dest.Width - 1 - (int)d];

                        int ierr = sgray - dgray;
                        if (ierr < 0) { ierr = -ierr; }
                        if (ierr > permiterr) { ++cnterr; }
                        if (cnterr > permitcnterr) { break; }
                    }
                    if (cnterr <= permitcnterr) { return true; }
                }

                #endregion

                return false;
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
