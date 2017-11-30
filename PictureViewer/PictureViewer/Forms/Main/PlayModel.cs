using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Forms
{
    /// <summary>
    /// 播放器模型
    /// </summary>
    class PlayModel
    {
        /// <summary>
        /// 当前正在播放，并且是否需要继续播放。
        /// </summary>
        public bool IsPlaying
        {
            get;
            set;
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        public long Begin
        {
            set;
            get;
        }
        /// <summary>
        /// 结束时间
        /// </summary>
        public long End
        {
            set;
            get;
        }
        /// <summary>
        /// 持续时间
        /// </summary>
        public long Lasting
        {
            set;
            get;
        }

        /// <summary>
        /// 对播放列表的索引号
        /// </summary>
        public int Index
        {
            set;
            get;
        }
        /// <summary>
        /// 有序的播放列表
        /// </summary>
        public List<Files.TreeIndex> SortList
        {
            get;
            set;
        }
        /// <summary>
        /// 随机的播放列表
        /// </summary>
        public List<Files.TreeIndex> RandList
        {
            get;
            set;
        }

        /// <summary>
        /// 到达了列表开头的文件
        /// </summary>
        public bool IsListBegin
        {
            get
            {
                if (SortList.Count == 0) { return true; }
                return Manager.MainViewer.TreeIndex == SortList[0];
            }
        }
        /// <summary>
        /// 到达了列表结束的文件
        /// </summary>
        public bool IsListEnd
        {
            get
            {
                if (SortList.Count == 0) { return true; }
                return Manager.MainViewer.TreeIndex == SortList[SortList.Count - 1];
            }
        }
        /// <summary>
        /// 到达了循环结束的文件
        /// </summary>
        public bool IsCircleEnd
        {
            get
            {
                if (SortList.Count == 0) { return true; }
                if (Forward) { return IsListEnd; }
                if (Backward) { return IsListBegin; }
                if (Rand) { return Manager.MainViewer.TreeIndex == RandList[SortList.Count - 1]; }
                return true;
            }
        }

        /// <summary>
        /// 播放全部目录
        /// </summary>
        public bool Total
        {
            get;
            set;
        }
        /// <summary>
        /// 播放当前根目录
        /// </summary>
        public bool Root
        {
            get;
            set;
        }
        /// <summary>
        /// 播放当前子目录
        /// </summary>
        public bool Subroot
        {
            get;
            set;
        }
        /// <summary>
        /// 前进播放
        /// </summary>
        public bool Forward
        {
            get;
            set;
        }
        /// <summary>
        /// 倒退播放
        /// </summary>
        public bool Backward
        {
            get;
            set;
        }
        /// <summary>
        /// 随机播放
        /// </summary>
        public bool Rand
        {
            get;
            set;
        }
        /// <summary>
        /// 播放图片文件
        /// </summary>
        public bool Picture
        {
            get;
            set;
        }
        /// <summary>
        /// 播放 GIF 文件
        /// </summary>
        public bool Gif
        {
            get;
            set;
        }
        /// <summary>
        /// 播放音频文件
        /// </summary>
        public bool Music
        {
            get;
            set;
        }
        /// <summary>
        /// 播放视频文件
        /// </summary>
        public bool Video
        {
            get;
            set;
        }
        /// <summary>
        /// 单曲播放（自动循环）
        /// </summary>
        public bool Single
        {
            get;
            set;
        }
        /// <summary>
        /// 顺序播放
        /// </summary>
        public bool Order
        {
            get;
            set;
        }
        /// <summary>
        /// 循环播放
        /// </summary>
        public bool Circle
        {
            get;
            set;
        }

        /// <summary>
        /// 创建播放器模型
        /// </summary>
        public PlayModel()
        {
            IsPlaying = false;
            Begin = 0;
            End = 0;
            Lasting = 0;
            Index = -1;
            SortList = new List<Files.TreeIndex>();
            RandList = new List<Files.TreeIndex>();

            Total = false;
            Root = false;
            Subroot = false;
            Forward = false;
            Backward = false;
            Rand = false;
            Picture = false;
            Gif = false;
            Music = false;
            Video = false;
            Single = false;
            Order = false;
            Circle = false;
        }

        /// <summary>
        /// 设置顺序播放列表
        /// </summary>
        public void SetSortList()
        {
            SortList.Clear();

            if (Total)
            {
                for (int i = 0; i < Files.Manager.Index.IndexPairs.Count; i++)
                {
                    for (int j = 0; j < Files.Manager.Index.IndexPairs[i].Count; j++)
                    {
                        for (int k = 0; k < Files.Manager.Index.IndexPairs[i][j].Count; k++)
                        {
                            Files.TreeIndex tree = new Files.TreeIndex(i, j, k);
                            Files.FileType type = tree.Model.Type;

                            if (type == Files.FileType.Picture && Picture) { SortList.Add(tree); }
                            if (type == Files.FileType.Gif && Gif) { SortList.Add(tree); }
                            if (type == Files.FileType.Music && Music) { SortList.Add(tree); }
                            if (type == Files.FileType.Video && Video) { SortList.Add(tree); }
                        }
                    }
                }
            }

            if (Root)
            {
                if (Manager.MainViewer.TreeIndex.Folder >= 0 && Manager.MainViewer.TreeIndex.Folder < Files.Manager.Index.IndexPairs.Count)
                {
                    int i = Manager.MainViewer.TreeIndex.Folder;
                    for (int j = 0; j < Files.Manager.Index.IndexPairs[i].Count; j++)
                    {
                        for (int k = 0; k < Files.Manager.Index.IndexPairs[i][j].Count; k++)
                        {
                            Files.TreeIndex tree = new Files.TreeIndex(i, j, k);
                            Files.FileType type = tree.Model.Type;

                            if (type == Files.FileType.Picture && Picture) { SortList.Add(tree); }
                            if (type == Files.FileType.Gif && Gif) { SortList.Add(tree); }
                            if (type == Files.FileType.Music && Music) { SortList.Add(tree); }
                            if (type == Files.FileType.Video && Video) { SortList.Add(tree); }
                        }
                    }
                }
            }

            if (Subroot)
            {
                if (Manager.MainViewer.TreeIndex.Exist && Manager.MainViewer.TreeIndex.Model.InFolder)
                {
                    int i = Manager.MainViewer.TreeIndex.Folder;
                    int j = Manager.MainViewer.TreeIndex.File;
                    for (int k = 0; k < Files.Manager.Index.IndexPairs[i][j].Count; k++)
                    {
                        Files.TreeIndex tree = new Files.TreeIndex(i, j, k);
                        Files.FileType type = tree.Model.Type;

                        if (type == Files.FileType.Picture && Picture) { SortList.Add(tree); }
                        if (type == Files.FileType.Gif && Gif) { SortList.Add(tree); }
                        if (type == Files.FileType.Music && Music) { SortList.Add(tree); }
                        if (type == Files.FileType.Video && Video) { SortList.Add(tree); }
                    }
                }
            }
        }
        /// <summary>
        /// 设置随机播放列表（必须先设置顺序播放列表）
        /// </summary>
        public void SetRandList()
        {
            RandList = SortList.OrderBy(x => Guid.NewGuid()).ToList();
        }
        /// <summary>
        /// 把当前界面显示的文件设置为当前播放文件
        /// </summary>
        public void SetCurrent()
        {
            for (int i = 0; i < SortList.Count; i++)
            {
                if ( Rand && Manager.MainViewer.TreeIndex == RandList[i]) { Index = i; return; }
                if (!Rand && Manager.MainViewer.TreeIndex == SortList[i]) { Index = i; return; }
            }
        }
        /// <summary>
        /// 上一个
        /// </summary>
        /// <param name="treeIndex">树形索引号</param>
        /// <param name="decreaseIndex">索引号减一</param>
        /// <returns></returns>
        public bool GetPrevious(ref Files.TreeIndex treeIndex, bool decreaseIndex = true)
        {
            if (SortList.Count == 0) { treeIndex = Manager.MainViewer.TreeIndex; return false; }

            int index = Index - 1;
            bool exist = index >= 0;
            if (!exist) { index = SortList.Count - 1; }
            treeIndex = Rand ? RandList[index] : SortList[index];

            if (decreaseIndex) { Index = index; }
            if (Single || Circle) { return true; }
            return exist;
        }
        /// <summary>
        /// 下一个
        /// </summary>
        /// <param name="treeIndex">树形索引号</param>
        /// <param name="increaseIndex">索引号加一</param>
        /// <returns></returns>
        public bool GetNext(ref Files.TreeIndex treeIndex, bool increaseIndex = true)
        {
            if (SortList.Count == 0) { treeIndex = Manager.MainViewer.TreeIndex; return false; }
            
            int index = Index + 1;
            bool exist = index < SortList.Count;
            if (!exist) { index = 0; }
            treeIndex = Rand ? RandList[index] : SortList[index];

            if (Single || Circle) { return true; }
            return exist;
        }
        /// <summary>
        /// 这一个
        /// </summary>
        /// <param name="treeIndex"></param>
        /// <returns></returns>
        public bool GetCurrent(ref Files.TreeIndex treeIndex)
        {
            if (Index < 0 || Index >= SortList.Count) { return false; }
            treeIndex = Rand ? RandList[Index] : SortList[Index];
            return true;
        }
    }
}
