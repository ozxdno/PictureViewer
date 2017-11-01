﻿namespace PictureViewer
{
    partial class Form_Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.updataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.titleToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gotoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shapeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bigPicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.filePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.partToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.likeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.turnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.openExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openComicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updataToolStripMenuItem,
            this.infoToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.inputToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.gotoToolStripMenuItem,
            this.previousToolStripMenuItem,
            this.nextToolStripMenuItem,
            this.shapeToolStripMenuItem,
            this.lockToolStripMenuItem,
            this.tipToolStripMenuItem,
            this.bigPicToolStripMenuItem,
            this.hideToolStripMenuItem,
            this.toolStripMenuItem2,
            this.filePathToolStripMenuItem,
            this.playToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.findToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.toolStripMenuItem1,
            this.openExportToolStripMenuItem,
            this.openRootToolStripMenuItem,
            this.openFileToolStripMenuItem,
            this.openComicToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 522);
            // 
            // updataToolStripMenuItem
            // 
            this.updataToolStripMenuItem.Name = "updataToolStripMenuItem";
            this.updataToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.updataToolStripMenuItem.Text = "Refresh";
            this.updataToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Refresh);
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.titleToolStripMenuItem1,
            this.textToolStripMenuItem});
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.infoToolStripMenuItem.Text = "Info";
            this.infoToolStripMenuItem.Visible = false;
            // 
            // titleToolStripMenuItem1
            // 
            this.titleToolStripMenuItem1.Name = "titleToolStripMenuItem1";
            this.titleToolStripMenuItem1.Size = new System.Drawing.Size(100, 22);
            this.titleToolStripMenuItem1.Text = "Title";
            this.titleToolStripMenuItem1.ToolTipText = "File Belongs To (Folder)";
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            this.textToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.textToolStripMenuItem.Text = "Text";
            this.textToolStripMenuItem.ToolTipText = "File Name";
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Rename);
            // 
            // inputToolStripMenuItem
            // 
            this.inputToolStripMenuItem.Name = "inputToolStripMenuItem";
            this.inputToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.inputToolStripMenuItem.Text = "Input";
            this.inputToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Input);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pathToolStripMenuItem,
            this.exportFolderToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Export);
            // 
            // pathToolStripMenuItem
            // 
            this.pathToolStripMenuItem.Name = "pathToolStripMenuItem";
            this.pathToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.pathToolStripMenuItem.Text = "Path";
            this.pathToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Export_Path);
            // 
            // exportFolderToolStripMenuItem
            // 
            this.exportFolderToolStripMenuItem.Name = "exportFolderToolStripMenuItem";
            this.exportFolderToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exportFolderToolStripMenuItem.Text = "Export Folder";
            this.exportFolderToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Export_ExportFolder);
            // 
            // gotoToolStripMenuItem
            // 
            this.gotoToolStripMenuItem.Name = "gotoToolStripMenuItem";
            this.gotoToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.gotoToolStripMenuItem.Text = "Goto";
            this.gotoToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Goto);
            // 
            // previousToolStripMenuItem
            // 
            this.previousToolStripMenuItem.Name = "previousToolStripMenuItem";
            this.previousToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.previousToolStripMenuItem.Text = "Previous";
            this.previousToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Previous);
            // 
            // nextToolStripMenuItem
            // 
            this.nextToolStripMenuItem.Name = "nextToolStripMenuItem";
            this.nextToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.nextToolStripMenuItem.Text = "Next";
            this.nextToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Next);
            // 
            // shapeToolStripMenuItem
            // 
            this.shapeToolStripMenuItem.Name = "shapeToolStripMenuItem";
            this.shapeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.shapeToolStripMenuItem.Text = "Shape";
            this.shapeToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Shape);
            // 
            // lockToolStripMenuItem
            // 
            this.lockToolStripMenuItem.CheckOnClick = true;
            this.lockToolStripMenuItem.Name = "lockToolStripMenuItem";
            this.lockToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.lockToolStripMenuItem.Text = "Lock";
            // 
            // tipToolStripMenuItem
            // 
            this.tipToolStripMenuItem.CheckOnClick = true;
            this.tipToolStripMenuItem.Name = "tipToolStripMenuItem";
            this.tipToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.tipToolStripMenuItem.Text = "Tip";
            // 
            // bigPicToolStripMenuItem
            // 
            this.bigPicToolStripMenuItem.Name = "bigPicToolStripMenuItem";
            this.bigPicToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.bigPicToolStripMenuItem.Text = "Big Picture";
            this.bigPicToolStripMenuItem.Visible = false;
            this.bigPicToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_BigPicture);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideUToolStripMenuItem,
            this.hideDToolStripMenuItem,
            this.hideLToolStripMenuItem,
            this.hideRToolStripMenuItem});
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            this.hideToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hideToolStripMenuItem.Text = "Hide";
            this.hideToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Hide);
            // 
            // hideUToolStripMenuItem
            // 
            this.hideUToolStripMenuItem.CheckOnClick = true;
            this.hideUToolStripMenuItem.Name = "hideUToolStripMenuItem";
            this.hideUToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.hideUToolStripMenuItem.Text = "Hide U";
            // 
            // hideDToolStripMenuItem
            // 
            this.hideDToolStripMenuItem.CheckOnClick = true;
            this.hideDToolStripMenuItem.Name = "hideDToolStripMenuItem";
            this.hideDToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.hideDToolStripMenuItem.Text = "Hide D";
            // 
            // hideLToolStripMenuItem
            // 
            this.hideLToolStripMenuItem.CheckOnClick = true;
            this.hideLToolStripMenuItem.Name = "hideLToolStripMenuItem";
            this.hideLToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.hideLToolStripMenuItem.Text = "Hide L";
            // 
            // hideRToolStripMenuItem
            // 
            this.hideRToolStripMenuItem.CheckOnClick = true;
            this.hideRToolStripMenuItem.Name = "hideRToolStripMenuItem";
            this.hideRToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.hideRToolStripMenuItem.Text = "Hide R";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 6);
            // 
            // filePathToolStripMenuItem
            // 
            this.filePathToolStripMenuItem.Name = "filePathToolStripMenuItem";
            this.filePathToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.filePathToolStripMenuItem.Text = "File Path";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Delete);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullToolStripMenuItem,
            this.partToolStripMenuItem,
            this.sameToolStripMenuItem,
            this.likeToolStripMenuItem,
            this.turnToolStripMenuItem});
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.findToolStripMenuItem.Text = "Find";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Find);
            // 
            // fullToolStripMenuItem
            // 
            this.fullToolStripMenuItem.Name = "fullToolStripMenuItem";
            this.fullToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.fullToolStripMenuItem.Text = "Full";
            this.fullToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Find_Full);
            // 
            // partToolStripMenuItem
            // 
            this.partToolStripMenuItem.Checked = true;
            this.partToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.partToolStripMenuItem.Name = "partToolStripMenuItem";
            this.partToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.partToolStripMenuItem.Text = "Part";
            this.partToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Find_Part);
            // 
            // sameToolStripMenuItem
            // 
            this.sameToolStripMenuItem.Checked = true;
            this.sameToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sameToolStripMenuItem.Name = "sameToolStripMenuItem";
            this.sameToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.sameToolStripMenuItem.Text = "Same";
            this.sameToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Find_Same);
            // 
            // likeToolStripMenuItem
            // 
            this.likeToolStripMenuItem.Name = "likeToolStripMenuItem";
            this.likeToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.likeToolStripMenuItem.Text = "Like";
            this.likeToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Find_Like);
            // 
            // turnToolStripMenuItem
            // 
            this.turnToolStripMenuItem.Checked = true;
            this.turnToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.turnToolStripMenuItem.Name = "turnToolStripMenuItem";
            this.turnToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.turnToolStripMenuItem.Text = "Turn";
            this.turnToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Find_Turn);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.searchToolStripMenuItem.Text = "Search";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Search);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // openExportToolStripMenuItem
            // 
            this.openExportToolStripMenuItem.Name = "openExportToolStripMenuItem";
            this.openExportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openExportToolStripMenuItem.Text = "Open Export";
            this.openExportToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_OpenExport);
            // 
            // openRootToolStripMenuItem
            // 
            this.openRootToolStripMenuItem.Name = "openRootToolStripMenuItem";
            this.openRootToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openRootToolStripMenuItem.Text = "Open Root";
            this.openRootToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_OpenRoot);
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openFileToolStripMenuItem.Text = "Open File";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_OpenCurrent);
            // 
            // openComicToolStripMenuItem
            // 
            this.openComicToolStripMenuItem.Name = "openComicToolStripMenuItem";
            this.openComicToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openComicToolStripMenuItem.Text = "Open Comic";
            this.openComicToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_OpenComic);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 69);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(203, 52);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.Form_Main_DoubleClick);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(-1, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "<";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.Page_L);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(221, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = ">";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Click += new System.EventHandler(this.Page_R);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(82, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "↑";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.Click += new System.EventHandler(this.Page_U);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(82, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "↓";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label4.Click += new System.EventHandler(this.Page_D);
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.ContextMenuStrip = this.contextMenuStrip1;
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(1, 1);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(203, 51);
            this.axWindowsMediaPlayer1.TabIndex = 4;
            this.axWindowsMediaPlayer1.Visible = false;
            this.axWindowsMediaPlayer1.MouseDownEvent += new AxWMPLib._WMPOCXEvents_MouseDownEventHandler(this.WMP_MouseDown);
            this.axWindowsMediaPlayer1.MouseUpEvent += new AxWMPLib._WMPOCXEvents_MouseUpEventHandler(this.WMP_MouseUp);
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.CheckOnClick = true;
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.playToolStripMenuItem.Text = "Play";
            // 
            // Form_Main
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(232, 136);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.axWindowsMediaPlayer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Form_Main";
            this.Text = "Form1";
            this.Activated += new System.EventHandler(this.Form_Main_Activated);
            this.Deactivate += new System.EventHandler(this.Form_Main_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_Closed);
            this.Load += new System.EventHandler(this.Form_Loaded);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form_DragEntre);
            this.DoubleClick += new System.EventHandler(this.Form_Main_DoubleClick);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.ToolStripMenuItem updataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gotoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filePathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRootToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openComicToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideUToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bigPicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem partToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem likeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem turnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shapeToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem previousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem titleToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
    }
}

