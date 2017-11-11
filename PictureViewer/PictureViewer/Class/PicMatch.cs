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
        private int permitcnterr = 0;
        private int permiterr = 10;
        private int cnterr = 0;

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
            pixes = Form_Find.config.MinCmpPix;
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
            pic.GraysR = new int[pixes];
            pic.GraysC = new int[pixes];
            
            while(true)
            {
                double pace = (double)pic.Width / pixes;
                int half = pixes / 2;
                int cnt = 0;
                for (double i = 0; cnt < half; i += pace, cnt++)
                {
                    pic.GraysR[cnt] = toGray(img.GetPixel((int)i, pic.Row));
                    pic.GraysR[pixes - 1 - cnt] = toGray(img.GetPixel(pic.Width - 1 - (int)i, pic.Row));
                }
                break;
            }
            while (true)
            {
                double pace = (double)pic.Height / pixes;
                int half = pixes / 2;
                int cnt = 0;
                for (double i = 0; cnt < half; i += pace, cnt++)
                {
                    pic.GraysC[cnt] = toGray(img.GetPixel(pic.Col, (int)i));
                    pic.GraysC[pixes - 1 - cnt] = toGray(img.GetPixel(pic.Col, pic.Height - 1 - (int)i));
                }
                break;
            }

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
                    permitcnterr = pixes * (100 - degree) / 100;

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
                    permitcnterr = pixes * (100 - degree) / 100;

                    if (!cmpRow()) { continue; }
                    if (!cmpCol()) { continue; }

                    if (!newResultItem)
                    {
                        newResultItem = true;
                        Form_Find.Results.Add(new List<int>());
                        Form_Find.Results[Form_Find.Results.Count - 1].Add(Form_Find.config.Sour[Form_Find.IndexS]);
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
                    permitcnterr = pixes * (100 - degree) / 100;

                    if (!cmpRow()) { continue; }
                    if (!cmpCol()) { continue; }
                    
                    if (!newResultItem)
                    {
                        newResultItem = true;
                        Form_Find.Results.Add(new List<int>());
                        Form_Find.config.Standard.Add(Form_Find.config.Sour[Form_Find.IndexS]);
                        Form_Find.Results[Form_Find.Results.Count - 1].Add(Form_Find.config.Sour[Form_Find.IndexS]);
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
                return cmpRow000();
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
                    if (cmpRow000()) { return true; }
                    if (cmpRow180()) { return true; }
                }

                #endregion

                #region 旋转 90 或 270 度

                if (isForm2)
                {
                    if (cmpRow090()) { return true; }
                    if (cmpRow270()) { return true; }
                }

                #endregion

                return false;
            }

            #endregion

            #region FULL LIKE NO_TURN

            if (mode == Form_Find.MODE.FULL_LIKE_NOTURN)
            {
                return cmpRow000();
            }

            #endregion

            #region FULL LIKE TURN

            if (mode == Form_Find.MODE.FULL_LIKE_TURN)
            {
                if (cmpRow000()) { return true; }
                if (cmpRow090()) { return true; }
                if (cmpRow180()) { return true; }
                if (cmpRow270()) { return true; }

                return false;
            }

            #endregion

            #region PART SAME NO_TURN

            if (mode == Form_Find.MODE.PART_SAME_NOTURN)
            {
                double rate = (double)sour.Height / dest.Height;
                double expw = sour.Width / rate;
                bool isSame = dest.Width - expw < 3 && expw - dest.Width < 3;
                if (!isSame) { return false; }

                return cmpRow000();
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
                    if (cmpRow000()) { return true; }
                    if (cmpRow180()) { return true; }
                }

                #endregion

                #region 旋转 90 或 270 度

                if (isForm2)
                {
                    if (cmpRow090()) { return true; }
                    if (cmpRow270()) { return true; }
                }

                #endregion

                return false;
            }

            #endregion

            #region PATR LIKE NO_TURN

            if (mode == Form_Find.MODE.PART_LIKE_NOTURN)
            {
                return cmpRow000();
            }

            #endregion

            #region PART LIKE TURN

            if (mode == Form_Find.MODE.PART_LIKE_TURN)
            {
                if (cmpRow000()) { return true; }
                if (cmpRow090()) { return true; }
                if (cmpRow180()) { return true; }
                if (cmpRow270()) { return true; }

                return false;
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
                return cmpCol000();
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
                    if (cmpCol000()) { return true; }
                    if (cmpCol180()) { return true; }
                }

                #endregion

                #region 旋转 90 或 270 度

                if (isForm2)
                {
                    if (cmpCol090()) { return true; }
                    if (cmpCol270()) { return true; }
                }

                #endregion

                return false;
            }

            #endregion

            #region FULL LIKE NO_TURN

            if (mode == Form_Find.MODE.FULL_LIKE_NOTURN)
            {
                return cmpCol000();
            }

            #endregion

            #region FULL LIKE TURN

            if (mode == Form_Find.MODE.FULL_LIKE_TURN)
            {
                if (cmpCol000()) { return true; }
                if (cmpCol090()) { return true; }
                if (cmpCol180()) { return true; }
                if (cmpCol270()) { return true; }
                return false;
            }

            #endregion

            #region PART SAME NO_TURN

            if (mode == Form_Find.MODE.PART_SAME_NOTURN)
            {
                double rate = (double)sour.Height / dest.Height;
                double expw = sour.Width / rate;
                bool isSame = dest.Width - expw < 3 && expw - dest.Width < 3;
                if (!isSame) { return false; }
                return cmpCol000();
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
                    if (cmpCol000()) { return true; }
                    if (cmpCol180()) { return true; }
                }

                #endregion

                #region 旋转 90 或 270 度

                if (isForm2)
                {
                    if (cmpCol090()) { return true; }
                    if (cmpCol270()) { return true; }
                }

                #endregion

                return false;
            }

            #endregion

            #region PATR LIKE NO_TURN

            if (mode == Form_Find.MODE.PART_LIKE_NOTURN)
            {
                return cmpCol000();
            }

            #endregion

            #region PART LIKE TURN

            if (mode == Form_Find.MODE.PART_LIKE_TURN)
            {
                if (cmpCol000()) { return true; }
                if (cmpCol090()) { return true; }
                if (cmpCol180()) { return true; }
                if (cmpCol270()) { return true; }
                return false;
            }

            #endregion

            return false;
        }

        private bool cmpRow000()
        {
            cnterr = 0;

            for (int i = 0; i < pixes; i++)
            {
                int ierr = sour.GraysR[i] - dest.GraysR[i];
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }
        private bool cmpRow090()
        {
            cnterr = 0;

            for (int i = 0; i < pixes; i++)
            {
                int ierr = sour.GraysR[i] - dest.GraysC[i];
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }
        private bool cmpRow180()
        {
            cnterr = 0;

            for (int i = 0; i < pixes; i++)
            {
                int ierr = sour.GraysR[i] - dest.GraysR[pixes - 1 - i];
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }
        private bool cmpRow270()
        {
            cnterr = 0;

            for (int i = 0; i < pixes; i++)
            {
                int ierr = sour.GraysR[i] - dest.GraysC[pixes - 1 - i];
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }

        private bool cmpCol000()
        {
            cnterr = 0;

            for (int i = 0; i < pixes; i++)
            {
                int ierr = sour.GraysC[i] - dest.GraysC[i];
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }
        private bool cmpCol090()
        {
            cnterr = 0;

            for (int i = 0; i < pixes; i++)
            {
                int ierr = sour.GraysC[i] - dest.GraysR[i];
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }
        private bool cmpCol180()
        {
            cnterr = 0;

            for (int i = 0; i < pixes; i++)
            {
                int ierr = sour.GraysC[i] - dest.GraysC[pixes - 1 - i];
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }
        private bool cmpCol270()
        {
            cnterr = 0;

            for (int i = 0; i < pixes; i++)
            {
                int ierr = sour.GraysC[i] - dest.GraysR[pixes - 1 - i];
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }
    }
}
