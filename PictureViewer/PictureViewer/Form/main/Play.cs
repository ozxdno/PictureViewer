using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.MainForm
{
    class Play
    {
        /// <summary>
        /// 是否继续播放
        /// </summary>
        public bool IsPlaying
        {
            set;
            get;
        }

        /// <summary>
        /// 播放文件总集
        /// </summary>
        public List<Files.Index> FileIndexes
        {
            get;
            set;
        }
        /// <summary>
        /// 播放顺序
        /// </summary>
        public List<int> PlayIndexes
        {
            get;
            set;
        }
        /// <summary>
        /// 当前播放文件
        /// </summary>
        public int Index
        {
            set
            {
                if (value < 0) { value = 0; }
                if (value >= PlayIndexes.Count) { value = PlayIndexes.Count - 1; }
                Index = value;
            }
            get
            {
                if (Index >= PlayIndexes.Count) { return PlayIndexes.Count - 1; }
                if (Index < 0) { return 0; }
                return Index;
            }
        }

        /// <summary>
        /// 播放的开始时间
        /// </summary>
        public long Begin
        {
            set;
            get;
        }
        /// <summary>
        /// 播放的结束时间
        /// </summary>
        public long End
        {
            get { return Begin + Lasting; }
        }
        /// <summary>
        /// 播放的持续时间
        /// </summary>
        public long Lasting
        {
            set;
            get;
        }


        public bool Total
        {
            set;
            get;
        }
        public bool Root
        {
            set;
            get;
        }
        public bool Subroot
        {
            set;
            get;
        }

        public bool Picture
        {
            set;
            get;
        }
        public bool Gif
        {
            set;
            get;
        }
        public bool Music
        {
            set;
            get;
        }
        public bool Video
        {
            set;
            get;
        }

        public bool Forward
        {
            set;
            get;
        }
        public bool Backward
        {
            set;
            get;
        }
        public bool Rand
        {
            set;
            get;
        }

        public bool Single
        {
            set;
            get;
        }
        public bool Order
        {
            set;
            get;
        }
        public bool Circle
        {
            set;
            get;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public Play()
        {

        }

        public void SetForward()
        {
            if (PlayIndexes == null) { PlayIndexes = new List<int>(); }
            PlayIndexes.Clear();
            for (int i = 0; i < FileIndexes.Count; i++) { PlayIndexes.Add(i); }
        }
        public void SetBackward()
        {
            if (PlayIndexes == null) { PlayIndexes = new List<int>(); }
            PlayIndexes.Clear();
            for (int i = FileIndexes.Count - 1; i >= 0; i--) { PlayIndexes.Add(i); }
        }
        public void SetRand()
        {

        }
        public bool FinishedCurrent(long time)
        {
            return End < time;
        }
        public bool FinishedCurrent(AxWMPLib.AxWindowsMediaPlayer.State state)
        {
            return false;
        }
        public bool Finished()
        {
            if (Circle) { return false; }
            if (Single) { return true; }

            return Index == PlayIndexes[PlayIndexes.Count - 1];
        }
        public void SetIndex()
        {
            for (int i = 0; i < PlayIndexes.Count; i++)
            {
                if (Config.TreeIndex == FileIndexes[PlayIndexes[i]]) { Index = i; return; }
            }
        }
    }
}
