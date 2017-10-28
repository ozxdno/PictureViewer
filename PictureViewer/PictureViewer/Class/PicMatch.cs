using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace PictureViewer.Class
{
    class PicMatch
    {
        ///////////////////////////////////////////////////// public attribute ///////////////////////////////////////////////

        /// <summary>
        /// 标准图片
        /// </summary>
        public Bitmap Picture { set { sour = value; } }
        /// <summary>
        /// 匹配模式
        /// </summary>
        public MODE Mode;
        /// <summary>
        /// 停止匹配
        /// </summary>
        public bool Stop = false;

        /// <summary>
        /// 待比较的文件
        /// </summary>
        public List<string> CmpFiles = new List<string>();
        /// <summary>
        /// 匹配结果（文件名）
        /// </summary>
        public List<string> OutFiles = new List<string>();
        /// <summary>
        /// 匹配结果（序号）
        /// </summary>
        public List<int> OutIndexes = new List<int>();

        /// <summary>
        /// 正在比较的文件序号（下一个待比较图片）
        /// </summary>
        public int IndexCmp = 0;

        /// <summary>
        /// 相似程度（0-100，100完全相同，0完全不同）
        /// </summary>
        public int Degree = 80;
        /// <summary>
        /// 比较多少个像素
        /// </summary>
        public int Pixes = 100;

        /// <summary>
        /// 所比较的行
        /// </summary>
        public int Row = -1;
        /// <summary>
        /// 所比较的列
        /// </summary>
        public int Col = -1;
        
        /// <summary>
        /// 查找模式
        /// </summary>
        public enum MODE : ushort
        {
            /// <summary>
            /// 默认：FULL + SAME + NO_TURN
            /// </summary>
            DEFAULT = 0x0000,
            /// <summary>
            /// 大小必须相同（没有经过缩放）
            /// </summary>
            FULL = 0x0001,
            /// <summary>
            /// 大小可以不同（经过缩放）
            /// </summary>
            PART = 0x0002,
            /// <summary>
            /// 完全相同（两幅图内容完全一样）
            /// </summary>
            SAME = 0x0004,
            /// <summary>
            /// 部分相似（源图为要寻找图的一部分）
            /// </summary>
            LIKE = 0x0008,
            /// <summary>
            /// 经过了旋转和翻转
            /// </summary>
            TURN = 0x0010,

            FULL_SAME_NOTURN = FULL + SAME,
            FULL_SAME_TURN = FULL + SAME + TURN,
            PART_SAME_NOTURN = PART + SAME,
            PART_SAME_TURN = PART + SAME + TURN,
            FULL_LIKE_NOTURN = FULL + LIKE,
            FULL_LIKE_TURN = FULL + LIKE + TURN,
            PART_LIKE_NOTURN = PART + LIKE,
            PART_LIKE_TURN = PART + LIKE + TURN
        }

        ///////////////////////////////////////////////////// private attribute ///////////////////////////////////////////////

        /// <summary>
        /// 线程锁，读取数据时保证互斥
        /// </summary>
        private object Lock;

        /// <summary>
        /// 源图，未经任何修改
        /// </summary>
        private Bitmap sour;
        /// <summary>
        /// 待匹配图，未经任何修改
        /// </summary>
        private Bitmap dest;

        /// <summary>
        /// 源图高
        /// </summary>
        private int sourh;
        /// <summary>
        /// 源图宽
        /// </summary>
        private int sourw;
        /// <summary>
        /// 待匹配图高
        /// </summary>
        private int desth;
        /// <summary>
        /// 待匹配图宽
        /// </summary>
        private int destw;

        /// <summary>
        /// 源图所比较行的灰度值
        /// </summary>
        private int[] GraysR;
        /// <summary>
        /// 源图所比较列的灰度值
        /// </summary>
        private int[] GraysC;

        /// <summary>
        /// 待匹配图的灰度值
        /// </summary>
        private int[] GraysD;
        
        /// <summary>
        /// 源图区域信息
        /// </summary>
        private BLOCK BlockS;
        /// <summary>
        /// 待匹配图的区域信息
        /// </summary>
        private BLOCK BlockD;
        /// <summary>
        /// 区域
        /// </summary>
        private struct BLOCK
        {
            /// <summary>
            /// 块状区域类型
            /// 1 连续型；
            /// 2 起伏型；
            /// 3 上升型；
            /// 4 下降型；
            /// </summary>
            public int Type;
            /// <summary>
            /// 允许的最小数量
            /// </summary>
            public int Min;
            /// <summary>
            /// 源图与待匹配图的匹配结果
            /// </summary>
            public List<int> Match;

            /// <summary>
            /// 鉴别参数，不同类型意义不同。
            /// 1 波动误差
            /// </summary>
            public int Param1;

            /// <summary>
            /// 区域索引
            /// </summary>
            public List<int> Index;
            /// <summary>
            /// 区域开始索引
            /// </summary>
            public List<int> Begin;
            /// <summary>
            /// 区域结束索引
            /// </summary>
            public List<int> End;
            /// <summary>
            /// 区域长度
            /// </summary>
            public List<int> Length;
            /// <summary>
            /// 区域颜色
            /// </summary>
            public List<int> Gray;
        }

        /// <summary>
        /// 在比较过程中，认为两个像素点相等的允许误差
        /// </summary>
        private int permiterr = 10;
        
        ///////////////////////////////////////////////////// public method ///////////////////////////////////////////////

        /// <summary>
        /// 开始寻找匹配图片，以下几点必须确保：
        /// 0 匹配过程中不能修改参数（Stop除外），只能读取结果；
        /// 1 SourPicture 必须已经填充，否则没法比较；
        /// 2 CmpFiles 中的每一项都必须存在；
        /// </summary>
        public void Start()
        {
            if (IndexCmp < 0) { IndexCmp = 0; }
            if (Degree < 0) { Degree = 0; }
            if (Degree > 100) { Degree = 100; }
            if (Pixes < 0) { return; }
            if (Row < 0) { Row = -1; }
            if (Row >= sour.Height) { Row = -1; }
            if (Col < 0) { Col = -1; }
            if (Col >= sour.Width) { Col = -1; }
            if (GraysR == null || GraysR.Length == 0) { SetGraysR(); }
            if (GraysC == null || GraysC.Length == 0) { SetGraysC(); }
            if (GraysR.Length == 0) { return; }
            if (GraysC.Length == 0) { return; }

            BlockS.Match = new List<int>();
            BlockS.Index = new List<int>();
            BlockS.Begin = new List<int>();
            BlockS.End = new List<int>();
            BlockS.Length = new List<int>();
            BlockS.Gray = new List<int>();
            BlockD.Match = new List<int>();
            BlockD.Index = new List<int>();
            BlockD.Begin = new List<int>();
            BlockD.End = new List<int>();
            BlockD.Length = new List<int>();
            BlockD.Gray = new List<int>();
            Lock = new object();
            Stop = false;
            sourh = sour.Height;
            sourw = sour.Width;

            for (; !Stop && IndexCmp < CmpFiles.Count; IndexCmp++)
            {
                dest = (Bitmap)Image.FromFile(CmpFiles[IndexCmp]);
                desth = dest.Height;
                destw = dest.Width;

                bool correctRow = Row != -1 && cmpRow();
                bool correctCol = Col != -1 && cmpCol();

                if (correctRow && correctCol)
                {
                    lock (Lock) { OutFiles.Add(CmpFiles[IndexCmp]); OutIndexes.Add(IndexCmp); }
                }

                dest.Dispose();
            }
        }

        /// <summary>
        /// 当 Picture 存在时，自动填充 Row。
        /// </summary>
        public void SetRow()
        {

        }
        /// <summary>
        /// 当 Picture 存在时，自动填充 Col。
        /// </summary>
        public void SetCol()
        {

        }
        /// <summary>
        /// 当 Picture 存在时，自动填充 GraysR 和 GraysC。
        /// </summary>
        public void SetGrays()
        {
            SetGraysC(); SetGraysR();
        }
        /// <summary>
        /// 当 Picture 存在时，自动填充 GraysR。
        /// </summary>
        public void SetGraysR()
        {
            if (GraysR == null) { GraysR = new int[0]; }
            if (sour == null) { return; }
            if (Row < 0) { return; }
            if (Row >= sour.Height) { return; }

            GraysR = new int[sour.Width];
            for (int i = 0; i < sour.Width; i++) { GraysR[i] = getGray(sour.GetPixel(i, Row)); }
        }
        /// <summary>
        /// 当 Picture 存在时，自动填充 GraysC。
        /// </summary>
        public void SetGraysC()
        {
            if (GraysC == null) { GraysC = new int[0]; }
            if (sour == null) { return; }
            if (Col < 0) { return; }
            if (Col >= sour.Width) { return; }

            GraysC = new int[sour.Height];
            for (int i = 0; i < sour.Height; i++) { GraysC[i] = getGray(sour.GetPixel(Col, i)); }
        }

        ///////////////////////////////////////////////////// private method ///////////////////////////////////////////////

        private bool cmpRow()
        {
            #region FULL SAME NO_TURN

            if (Mode == MODE.FULL_SAME_NOTURN)
            {
                if (sourh != desth || sourw != destw) { return false; }

                int cmpixes = sourw > Pixes ? Pixes : sourw;
                double pace = (double)sourw / cmpixes;
                int permitcnterr = cmpixes * (100 - Degree) / 100;
                int permiterr = 10;
                int cnterr = 0;

                for (double p = 0; p < sourw; p += pace)
                {
                    int sgray = GraysR[(int)p];
                    int dgray = getGray(dest.GetPixel((int)p, Row));

                    int ierr = sgray - dgray;
                    if (ierr < 0) { ierr = -ierr; }
                    if (ierr > permiterr) { ++cnterr; }
                    if (cnterr > permitcnterr) { break; }
                }

                return cnterr <= permitcnterr;
            }

            #endregion

            #region FULL SAME TURN

            if (Mode == MODE.FULL_SAME_TURN)
            {
                
            }

            #endregion

            #region FULL LIKE NO_TURN

            if (Mode == MODE.FULL_LIKE_NOTURN)
            {

            }

            #endregion

            #region FULL LIKE TURN

            if (Mode == MODE.FULL_LIKE_TURN)
            {

            }

            #endregion

            #region PART SAME NO_TURN

            if (Mode == MODE.PART_SAME_NOTURN)
            {
                
            }

            #endregion

            #region PART SAME TURN

            if (Mode == MODE.PART_SAME_TURN)
            {
               
            }

            #endregion

            #region PATR LIKE NO_TURN

            if (Mode == MODE.PART_LIKE_NOTURN)
            {
                getBlockSR1();

                for (int row = 0; row < desth; row++){ if (cmpRR3(row)) { return true; } }
                return false;
            }

            #endregion

            #region PART LIKE TURN

            if (Mode == MODE.PART_LIKE_TURN)
            {
                
            }

            #endregion

            return false;
        }
        private bool cmpCol()
        {
            #region FULL SAME NO_TURN

            if (Mode == MODE.FULL_SAME_NOTURN)
            {
                if (sourh != desth || sourw != destw) { return false; }

                int cmpixes = sourw > Pixes ? Pixes : sourw;
                double pace = (double)sourw / cmpixes;
                int permitcnterr = cmpixes * (100 - Degree) / 100;
                int permiterr = 10;
                int cnterr = 0;

                for (double p = 0; p < sourw; p += pace)
                {
                    int sgray = GraysR[(int)p];
                    int dgray = getGray(dest.GetPixel((int)p, Row));

                    int ierr = sgray - dgray;
                    if (ierr < 0) { ierr = -ierr; }
                    if (ierr > permiterr) { ++cnterr; }
                    if (cnterr > permitcnterr) { break; }
                }

                return cnterr <= permitcnterr;
            }

            #endregion

            #region FULL SAME TURN

            if (Mode == MODE.FULL_SAME_TURN)
            {

            }

            #endregion

            #region FULL LIKE NO_TURN

            if (Mode == MODE.FULL_LIKE_NOTURN)
            {

            }

            #endregion

            #region FULL LIKE TURN

            if (Mode == MODE.FULL_LIKE_TURN)
            {

            }

            #endregion

            #region PART SAME NO_TURN

            if (Mode == MODE.PART_SAME_NOTURN)
            {

            }

            #endregion

            #region PART SAME TURN

            if (Mode == MODE.PART_SAME_TURN)
            {

            }

            #endregion

            #region PATR LIKE NO_TURN

            if (Mode == MODE.PART_LIKE_NOTURN)
            {
                getBlockSC1();

                for (int col = 0; col < destw; col++) { if (cmpCC3(col)) { return true; } }
                return false;
            }

            #endregion

            #region PART LIKE TURN

            if (Mode == MODE.PART_LIKE_TURN)
            {

            }

            #endregion

            return false;
        }

        private bool cmpRR(int row, int bg, int ed, int permitcnterr, double space, double dpace)
        {
            int cnterr = 0;

            for (double s = 0, d = bg; s < sourw; s += space, d += dpace)
            {
                int sgray = GraysR[(int)s];
                int dgray = getGray(dest.GetPixel((int)d, row));

                int ierr = sgray - dgray;
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }
        private bool cmpCC(int col, int bg, int ed, int permitcnterr, double space, double dpace)
        {
            int cnterr = 0;

            for (double s = 0, d = bg; s < sourh; s += space, d += dpace)
            {
                int sgray = GraysC[(int)s];
                int dgray = getGray(dest.GetPixel(col, (int)d));

                int ierr = sgray - dgray;
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }
        private bool cmpRC(int col, int bg, int ed, int permitcnterr, double space, double dpace)
        {
            int cnterr = 0;

            for (double s = 0, d = bg; s < sourw; s += space, d += dpace)
            {
                int sgray = GraysR[(int)s];
                int dgray = getGray(dest.GetPixel(col, (int)d));

                int ierr = sgray - dgray;
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }
        private bool cmpCR(int row, int bg, int ed, int permitcnterr, double space, double dpace)
        {
            int cnterr = 0;

            for (double s = 0, d = bg; s < sourh; s += space, d += dpace)
            {
                int sgray = GraysC[(int)s];
                int dgray = getGray(dest.GetPixel((int)d, row));

                int ierr = sgray - dgray;
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }

        private bool cmpRR2(int row, int bg, int ed, int permitcnterr, double space, double dpace)
        {
            int cnterr = 0; getGraysDR(row);

            for (double s = 0, d = bg; s < sourw; s += space, d += dpace)
            {
                int sgray = GraysR[(int)s];
                int dgray = GraysD[(int)d];

                int ierr = sgray - dgray;
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }
        private bool cmpCC2(int col, int bg, int ed, int permitcnterr, double space, double dpace)
        {
            int cnterr = 0; getGraysDC(col);

            for (double s = 0, d = bg; s < sourh; s += space, d += dpace)
            {
                int sgray = GraysC[(int)s];
                int dgray = GraysD[(int)d];

                int ierr = sgray - dgray;
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }
        private bool cmpRC2(int col, int bg, int ed, int permitcnterr, double space, double dpace)
        {
            int cnterr = 0; getGraysDC(col);

            for (double s = 0, d = bg; s < sourw; s += space, d += dpace)
            {
                int sgray = GraysR[(int)s];
                int dgray = GraysD[(int)d];

                int ierr = sgray - dgray;
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }
        private bool cmpCR2(int row, int bg, int ed, int permitcnterr, double space, double dpace)
        {
            int cnterr = 0; getGraysDR(row);

            for (double s = 0, d = bg; s < sourh; s += space, d += dpace)
            {
                int sgray = GraysC[(int)s];
                int dgray = GraysD[(int)d];

                int ierr = sgray - dgray;
                if (ierr < 0) { ierr = -ierr; }
                if (ierr > permiterr) { ++cnterr; }
                if (cnterr > permitcnterr) { break; }
            }

            return cnterr <= permitcnterr;
        }

        private bool cmpRR3(int row)
        {
            if (BlockS.Index.Count == 0) { return true; }

            getGraysDR(row);
            getBlockDR1();
            return cmpBlock();
        }
        private bool cmpCC3(int col)
        {
            if (BlockS.Index.Count == 0) { return true; }

            getGraysDC(col);
            getBlockDC1();
            return cmpBlock();
        }
        private bool cmpBlock()
        {
            if (BlockD.Index.Count < BlockS.Index.Count) { return false; }
            if (BlockS.Index.Count == 0) { return true; }

            BlockS.Match.Clear();
            BlockD.Match.Clear();
            
            for (int i = 0; i < BlockD.Index.Count; i++)
            {
                // 首个相同颜色块
                int cerr = BlockS.Gray[0] - BlockD.Gray[i];
                if (cerr < 0) { cerr = -cerr; }
                if (cerr > 1) { continue; }
                BlockS.Match.Add(0);
                BlockD.Match.Add(i);
                int index = 1;

                int errlen = BlockS.Length[0] - BlockD.Length[i];
                if (errlen > 5) { errlen = 1; }
                else if (errlen < -5) { errlen = 2; }
                else if (errlen < 3 && errlen > -3) { errlen = 3; }
                else { errlen = 0; }

                // 颜色块颜色、长度相同
                for (int j = i + 1; j < BlockD.Index.Count; j++)
                {
                    for (; index < BlockS.Index.Count;)
                    {
                        // 颜色
                        cerr = BlockS.Gray[index] - BlockD.Gray[j];
                        if (cerr < 0) { cerr = -cerr; }
                        if (cerr > 1) { break; }

                        // 长度
                        int errlen2 = BlockS.Length[index] - BlockD.Length[j];
                        if (errlen == 1 && errlen2 < 0) { break; }
                        if (errlen == 2 && errlen2 > 0) { break; }
                        if (errlen == 3 && (
                            errlen2 > 3 ||
                            errlen2 < -3
                            ))
                        { break; }

                        // 更新
                        BlockS.Match.Add(index);
                        BlockD.Match.Add(j); index++;
                        if (index == BlockS.Index.Count) { return true; }
                    }
                }

                BlockS.Match.Clear();
                BlockD.Match.Clear();
            }

            return false;
        }

        private int getGray(Color c)
        {
            return (c.R + c.G + c.B) / 3;
            //return (int)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B);
        }
        private int getGray(int R, int G, int B)
        {
            return (R + B + G) / 3;
            //return (int)(0.3 * R + 0.59 * G + 0.11 * B);
        }

        private void getGraysDR(int row)
        {
            GraysD = new int[destw];

            for (int i = 0; i < destw; i++)
            {
                GraysD[i] = getGray(dest.GetPixel(i, row));
            }
        }
        private void getGraysDC(int col)
        {
            GraysD = new int[desth];

            for (int i = 0; i < desth; i++)
            {
                GraysD[i] = getGray(dest.GetPixel(col, i));
            }
        }

        private void getBlockSR1()
        {
            BlockS.Index.Clear();
            BlockS.Begin.Clear();
            BlockS.End.Clear();
            BlockS.Length.Clear();
            BlockS.Gray.Clear();

            BlockS.Type = 1;
            BlockS.Min = 10;
            BlockS.Param1 = 5;

            int cnt = 0; int average = GraysR[0]; int sum = 0;
            int rate = sourw / Pixes;
            if (rate < BlockS.Min) { rate = BlockS.Min; }

            for (int i = 0; i < sourw; i++)
            {
                int err = GraysR[i] - average; if (err < 0) { err = -err; }
                if (err <= BlockS.Param1) { cnt++; sum += GraysR[i]; average = sum / cnt; continue; }
                if (cnt < rate) { cnt = 1; average = GraysR[i]; sum = average; continue; }

                BlockS.Begin.Add(i - cnt);
                BlockS.End.Add(i - 1);
                BlockS.Index.Add((i - cnt + i - 1) / 2);
                BlockS.Length.Add(cnt);
                BlockS.Gray.Add(average);

                cnt = 1; average = GraysR[i]; sum = average;
            }

            if (cnt >= rate)
            {
                BlockS.Begin.Add(sourw - cnt);
                BlockS.End.Add(sourw - 1);
                BlockS.Index.Add((sourw - cnt + sourw - 1) / 2);
                BlockS.Length.Add(cnt);
                BlockS.Gray.Add(average);
            }
        }
        private void getBlockSC1()
        {
            BlockS.Index.Clear();
            BlockS.Begin.Clear();
            BlockS.End.Clear();
            BlockS.Length.Clear();
            BlockS.Gray.Clear();

            BlockS.Type = 1;
            BlockS.Min = 10;
            BlockS.Param1 = 5;

            int cnt = 0; int average = GraysC[0]; int sum = 0;
            int rate = sourw / Pixes;
            if (rate < BlockS.Min) { rate = BlockS.Min; }

            for (int i = 0; i < sourh; i++)
            {
                int err = GraysC[i] - average; if (err < 0) { err = -err; }
                if (err <= BlockS.Param1) { cnt++; sum += GraysC[i]; average = sum / cnt; continue; }
                if (cnt < rate) { cnt = 1; average = GraysC[i]; sum = average; continue; }

                BlockS.Begin.Add(i - cnt);
                BlockS.End.Add(i - 1);
                BlockS.Index.Add((i - cnt + i - 1) / 2);
                BlockS.Length.Add(cnt);
                BlockS.Gray.Add(average);

                cnt = 1; average = GraysC[i]; sum = average;
            }

            if (cnt >= rate)
            {
                BlockS.Begin.Add(sourh - cnt);
                BlockS.End.Add(sourh - 1);
                BlockS.Index.Add((sourh - cnt + sourh - 1) / 2);
                BlockS.Length.Add(cnt);
                BlockS.Gray.Add(average);
            }
        }
        private void getBlockDR1()
        {
            BlockD.Index.Clear();
            BlockD.Begin.Clear();
            BlockD.End.Clear();
            BlockD.Length.Clear();
            BlockD.Gray.Clear();

            BlockD.Type = 1;
            BlockD.Min = 1;
            BlockD.Param1 = 5;

            int cnt = 0; int average = GraysD[0]; int sum = 0;
            //int rate = sourw / Pixes;
            //if (rate < BlockD.Min) { rate = BlockD.Min; }
            int rate = 1;

            for (int i = 0; i < destw; i++)
            {
                int err = GraysD[i] - average; if (err < 0) { err = -err; }
                if (err <= BlockD.Param1) { cnt++; sum += GraysD[i]; average = sum / cnt; continue; }
                if (cnt < rate) { cnt = 1; average = GraysD[i]; sum = average; continue; }

                BlockD.Begin.Add(i - cnt);
                BlockD.End.Add(i - 1);
                BlockD.Index.Add((i - cnt + i - 1) / 2);
                BlockD.Length.Add(cnt);
                BlockD.Gray.Add(average);

                cnt = 1; average = GraysD[i]; sum = average;
            }

            if (cnt >= rate)
            {
                BlockD.Begin.Add(destw - cnt);
                BlockD.End.Add(destw - 1);
                BlockD.Index.Add((destw - cnt + destw - 1) / 2);
                BlockD.Length.Add(cnt);
                BlockD.Gray.Add(average);
            }
        }
        private void getBlockDC1()
        {
            BlockD.Index.Clear();
            BlockD.Begin.Clear();
            BlockD.End.Clear();
            BlockD.Length.Clear();
            BlockD.Gray.Clear();

            BlockD.Type = 1;
            BlockD.Min = 1;
            BlockD.Param1 = 5;

            int cnt = 0; int average = GraysD[0]; int sum = 0;
            //int rate = sourw / Pixes;
            //if (rate < BlockD.Min) { rate = BlockD.Min; }
            int rate = 1;

            for (int i = 0; i < desth; i++)
            {
                int err = GraysD[i] - average; if (err < 0) { err = -err; }
                if (err <= BlockD.Param1) { cnt++; sum += GraysD[i]; average = sum / cnt; continue; }
                if (cnt < rate) { cnt = 1; average = GraysD[i]; sum = average; continue; }

                BlockD.Begin.Add(i - cnt);
                BlockD.End.Add(i - 1);
                BlockD.Index.Add((i - cnt + i - 1) / 2);
                BlockD.Length.Add(cnt);
                BlockD.Gray.Add(average);

                cnt = 1; average = GraysD[i]; sum = average;
            }

            if (cnt >= rate)
            {
                BlockD.Begin.Add(desth - cnt);
                BlockD.End.Add(desth - 1);
                BlockD.Index.Add((desth - cnt + desth - 1) / 2);
                BlockD.Length.Add(cnt);
                BlockD.Gray.Add(average);
            }
        }
    }
}
