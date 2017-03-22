namespace PictureViewer.Form_View
{
    partial class Form_View
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_View));
            this.Panel_S = new System.Windows.Forms.Panel();
            this.PasteTags = new System.Windows.Forms.Button();
            this.CopyTags = new System.Windows.Forms.Button();
            this.SearchB = new System.Windows.Forms.Button();
            this.SearchDB = new System.Windows.Forms.Button();
            this.DeleteB = new System.Windows.Forms.Button();
            this.OriginalS = new System.Windows.Forms.Button();
            this.NextS = new System.Windows.Forms.Button();
            this.totalS = new System.Windows.Forms.TextBox();
            this.gotoS = new System.Windows.Forms.TextBox();
            this.Replace = new System.Windows.Forms.Button();
            this.RemoveB = new System.Windows.Forms.Button();
            this.PreviousS = new System.Windows.Forms.Button();
            this.ToDataBase = new System.Windows.Forms.Button();
            this.RemoveS = new System.Windows.Forms.Button();
            this.pic_S = new System.Windows.Forms.PictureBox();
            this.Panel_B = new System.Windows.Forms.Panel();
            this.pic_B = new System.Windows.Forms.PictureBox();
            this.totalB = new System.Windows.Forms.TextBox();
            this.gotoB = new System.Windows.Forms.TextBox();
            this.OriginalB = new System.Windows.Forms.Button();
            this.NextB = new System.Windows.Forms.Button();
            this.PreviousB = new System.Windows.Forms.Button();
            this.ToDesk = new System.Windows.Forms.Button();
            this.ExchangeIndex = new System.Windows.Forms.TextBox();
            this.Export1 = new System.Windows.Forms.Button();
            this.Export2 = new System.Windows.Forms.Button();
            this.Panel_S.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_S)).BeginInit();
            this.Panel_B.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_B)).BeginInit();
            this.SuspendLayout();
            // 
            // Panel_S
            // 
            this.Panel_S.Controls.Add(this.PasteTags);
            this.Panel_S.Controls.Add(this.CopyTags);
            this.Panel_S.Controls.Add(this.SearchB);
            this.Panel_S.Controls.Add(this.SearchDB);
            this.Panel_S.Controls.Add(this.DeleteB);
            this.Panel_S.Controls.Add(this.OriginalS);
            this.Panel_S.Controls.Add(this.NextS);
            this.Panel_S.Controls.Add(this.totalS);
            this.Panel_S.Controls.Add(this.gotoS);
            this.Panel_S.Controls.Add(this.Replace);
            this.Panel_S.Controls.Add(this.RemoveB);
            this.Panel_S.Controls.Add(this.PreviousS);
            this.Panel_S.Controls.Add(this.ToDataBase);
            this.Panel_S.Controls.Add(this.RemoveS);
            this.Panel_S.Controls.Add(this.pic_S);
            this.Panel_S.Location = new System.Drawing.Point(1, 4);
            this.Panel_S.Name = "Panel_S";
            this.Panel_S.Size = new System.Drawing.Size(200, 567);
            this.Panel_S.TabIndex = 10;
            // 
            // PasteTags
            // 
            this.PasteTags.Location = new System.Drawing.Point(97, 406);
            this.PasteTags.Name = "PasteTags";
            this.PasteTags.Size = new System.Drawing.Size(68, 23);
            this.PasteTags.TabIndex = 16;
            this.PasteTags.Text = "vTags";
            this.PasteTags.UseVisualStyleBackColor = true;
            this.PasteTags.Click += new System.EventHandler(this.PasteTags_Click);
            // 
            // CopyTags
            // 
            this.CopyTags.Location = new System.Drawing.Point(28, 406);
            this.CopyTags.Name = "CopyTags";
            this.CopyTags.Size = new System.Drawing.Size(63, 23);
            this.CopyTags.TabIndex = 15;
            this.CopyTags.Text = "cTags";
            this.CopyTags.UseVisualStyleBackColor = true;
            this.CopyTags.Click += new System.EventHandler(this.CopyTags_Click);
            // 
            // SearchB
            // 
            this.SearchB.Location = new System.Drawing.Point(28, 522);
            this.SearchB.Name = "SearchB";
            this.SearchB.Size = new System.Drawing.Size(137, 23);
            this.SearchB.TabIndex = 14;
            this.SearchB.Text = "Search Big PIC";
            this.SearchB.UseVisualStyleBackColor = true;
            this.SearchB.Click += new System.EventHandler(this.SearchB_Click);
            // 
            // SearchDB
            // 
            this.SearchDB.Location = new System.Drawing.Point(28, 493);
            this.SearchDB.Name = "SearchDB";
            this.SearchDB.Size = new System.Drawing.Size(137, 23);
            this.SearchDB.TabIndex = 13;
            this.SearchDB.Text = "Search DataBase";
            this.SearchDB.UseVisualStyleBackColor = true;
            this.SearchDB.Click += new System.EventHandler(this.SearchDB_Click);
            // 
            // DeleteB
            // 
            this.DeleteB.Location = new System.Drawing.Point(28, 377);
            this.DeleteB.Name = "DeleteB";
            this.DeleteB.Size = new System.Drawing.Size(137, 23);
            this.DeleteB.TabIndex = 12;
            this.DeleteB.Text = "Delete Big PIC";
            this.DeleteB.UseVisualStyleBackColor = true;
            this.DeleteB.Click += new System.EventHandler(this.DeleteB_Click);
            // 
            // OriginalS
            // 
            this.OriginalS.Location = new System.Drawing.Point(134, 271);
            this.OriginalS.Name = "OriginalS";
            this.OriginalS.Size = new System.Drawing.Size(63, 23);
            this.OriginalS.TabIndex = 11;
            this.OriginalS.Text = "Original";
            this.OriginalS.UseVisualStyleBackColor = true;
            this.OriginalS.Click += new System.EventHandler(this.OriginalS_Click);
            // 
            // NextS
            // 
            this.NextS.Location = new System.Drawing.Point(68, 271);
            this.NextS.Name = "NextS";
            this.NextS.Size = new System.Drawing.Size(63, 23);
            this.NextS.TabIndex = 10;
            this.NextS.Text = "Next";
            this.NextS.UseVisualStyleBackColor = true;
            this.NextS.Click += new System.EventHandler(this.NextS_Click);
            // 
            // totalS
            // 
            this.totalS.Location = new System.Drawing.Point(47, 244);
            this.totalS.Name = "totalS";
            this.totalS.ReadOnly = true;
            this.totalS.Size = new System.Drawing.Size(150, 21);
            this.totalS.TabIndex = 9;
            this.totalS.Text = "Total: 0 / Size: 0";
            // 
            // gotoS
            // 
            this.gotoS.Location = new System.Drawing.Point(3, 244);
            this.gotoS.Name = "gotoS";
            this.gotoS.Size = new System.Drawing.Size(38, 21);
            this.gotoS.TabIndex = 2;
            this.gotoS.TextChanged += new System.EventHandler(this.gotoS_TextChanged);
            // 
            // Replace
            // 
            this.Replace.Location = new System.Drawing.Point(28, 464);
            this.Replace.Name = "Replace";
            this.Replace.Size = new System.Drawing.Size(137, 23);
            this.Replace.TabIndex = 8;
            this.Replace.Text = "Replace";
            this.Replace.UseVisualStyleBackColor = true;
            this.Replace.Click += new System.EventHandler(this.Replace_Click);
            // 
            // RemoveB
            // 
            this.RemoveB.Location = new System.Drawing.Point(28, 348);
            this.RemoveB.Name = "RemoveB";
            this.RemoveB.Size = new System.Drawing.Size(137, 23);
            this.RemoveB.TabIndex = 7;
            this.RemoveB.Text = "Remove Big PIC";
            this.RemoveB.UseVisualStyleBackColor = true;
            this.RemoveB.Click += new System.EventHandler(this.RemoveB_Click);
            // 
            // PreviousS
            // 
            this.PreviousS.Location = new System.Drawing.Point(3, 271);
            this.PreviousS.Name = "PreviousS";
            this.PreviousS.Size = new System.Drawing.Size(63, 23);
            this.PreviousS.TabIndex = 5;
            this.PreviousS.Text = "Previous";
            this.PreviousS.UseVisualStyleBackColor = true;
            this.PreviousS.Click += new System.EventHandler(this.PreviousS_Click);
            // 
            // ToDataBase
            // 
            this.ToDataBase.Location = new System.Drawing.Point(28, 435);
            this.ToDataBase.Name = "ToDataBase";
            this.ToDataBase.Size = new System.Drawing.Size(137, 23);
            this.ToDataBase.TabIndex = 4;
            this.ToDataBase.Text = "To DataBase";
            this.ToDataBase.UseVisualStyleBackColor = true;
            this.ToDataBase.Click += new System.EventHandler(this.ToDataBase_Click);
            // 
            // RemoveS
            // 
            this.RemoveS.Location = new System.Drawing.Point(28, 319);
            this.RemoveS.Name = "RemoveS";
            this.RemoveS.Size = new System.Drawing.Size(137, 23);
            this.RemoveS.TabIndex = 3;
            this.RemoveS.Text = "Remove Small PIC";
            this.RemoveS.UseVisualStyleBackColor = true;
            this.RemoveS.Click += new System.EventHandler(this.RemoveS_Click);
            // 
            // pic_S
            // 
            this.pic_S.Location = new System.Drawing.Point(3, 3);
            this.pic_S.MinimumSize = new System.Drawing.Size(194, 235);
            this.pic_S.Name = "pic_S";
            this.pic_S.Size = new System.Drawing.Size(194, 235);
            this.pic_S.TabIndex = 0;
            this.pic_S.TabStop = false;
            // 
            // Panel_B
            // 
            this.Panel_B.AutoScroll = true;
            this.Panel_B.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Panel_B.BackgroundImage")));
            this.Panel_B.Controls.Add(this.pic_B);
            this.Panel_B.Location = new System.Drawing.Point(207, 4);
            this.Panel_B.Name = "Panel_B";
            this.Panel_B.Size = new System.Drawing.Size(776, 537);
            this.Panel_B.TabIndex = 9;
            // 
            // pic_B
            // 
            this.pic_B.Location = new System.Drawing.Point(3, 0);
            this.pic_B.Name = "pic_B";
            this.pic_B.Size = new System.Drawing.Size(773, 534);
            this.pic_B.TabIndex = 1;
            this.pic_B.TabStop = false;
            this.pic_B.DoubleClick += new System.EventHandler(this.pic_B_DoubleClick);
            // 
            // totalB
            // 
            this.totalB.Location = new System.Drawing.Point(534, 545);
            this.totalB.Name = "totalB";
            this.totalB.ReadOnly = true;
            this.totalB.Size = new System.Drawing.Size(158, 21);
            this.totalB.TabIndex = 11;
            this.totalB.Text = "Total: 0 / Size: 0";
            this.totalB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.totalB_KeyDown);
            // 
            // gotoB
            // 
            this.gotoB.Location = new System.Drawing.Point(476, 545);
            this.gotoB.Name = "gotoB";
            this.gotoB.Size = new System.Drawing.Size(52, 21);
            this.gotoB.TabIndex = 10;
            this.gotoB.TextChanged += new System.EventHandler(this.gotoB_TextChanged);
            this.gotoB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gotoB_KeyDown);
            // 
            // OriginalB
            // 
            this.OriginalB.Location = new System.Drawing.Point(403, 544);
            this.OriginalB.Name = "OriginalB";
            this.OriginalB.Size = new System.Drawing.Size(67, 23);
            this.OriginalB.TabIndex = 5;
            this.OriginalB.Text = "Original";
            this.OriginalB.UseVisualStyleBackColor = true;
            this.OriginalB.Click += new System.EventHandler(this.OriginalB_Click);
            // 
            // NextB
            // 
            this.NextB.Location = new System.Drawing.Point(749, 544);
            this.NextB.Name = "NextB";
            this.NextB.Size = new System.Drawing.Size(67, 23);
            this.NextB.TabIndex = 4;
            this.NextB.Text = "Next";
            this.NextB.UseVisualStyleBackColor = true;
            this.NextB.Click += new System.EventHandler(this.NextB_Click);
            // 
            // PreviousB
            // 
            this.PreviousB.Location = new System.Drawing.Point(330, 544);
            this.PreviousB.Name = "PreviousB";
            this.PreviousB.Size = new System.Drawing.Size(67, 23);
            this.PreviousB.TabIndex = 3;
            this.PreviousB.Text = "Previous";
            this.PreviousB.UseVisualStyleBackColor = true;
            this.PreviousB.Click += new System.EventHandler(this.PreviousB_Click);
            // 
            // ToDesk
            // 
            this.ToDesk.Location = new System.Drawing.Point(236, 544);
            this.ToDesk.Name = "ToDesk";
            this.ToDesk.Size = new System.Drawing.Size(88, 23);
            this.ToDesk.TabIndex = 5;
            this.ToDesk.Text = "Export Desk";
            this.ToDesk.UseVisualStyleBackColor = true;
            this.ToDesk.Click += new System.EventHandler(this.ToDesk_Click);
            // 
            // ExchangeIndex
            // 
            this.ExchangeIndex.Location = new System.Drawing.Point(698, 545);
            this.ExchangeIndex.Name = "ExchangeIndex";
            this.ExchangeIndex.Size = new System.Drawing.Size(45, 21);
            this.ExchangeIndex.TabIndex = 12;
            this.ExchangeIndex.TextChanged += new System.EventHandler(this.ExchangeIndex_TextChanged);
            this.ExchangeIndex.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ExchangeIndex_KeyDown);
            // 
            // Export1
            // 
            this.Export1.Location = new System.Drawing.Point(822, 544);
            this.Export1.Name = "Export1";
            this.Export1.Size = new System.Drawing.Size(56, 23);
            this.Export1.TabIndex = 13;
            this.Export1.Text = "Export1";
            this.Export1.UseVisualStyleBackColor = true;
            this.Export1.Click += new System.EventHandler(this.Export1_Click);
            // 
            // Export2
            // 
            this.Export2.Location = new System.Drawing.Point(884, 544);
            this.Export2.Name = "Export2";
            this.Export2.Size = new System.Drawing.Size(56, 23);
            this.Export2.TabIndex = 14;
            this.Export2.Text = "Export2";
            this.Export2.UseVisualStyleBackColor = true;
            this.Export2.Click += new System.EventHandler(this.Export2_Click);
            // 
            // Form_View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 569);
            this.Controls.Add(this.Export2);
            this.Controls.Add(this.Export1);
            this.Controls.Add(this.ExchangeIndex);
            this.Controls.Add(this.ToDesk);
            this.Controls.Add(this.totalB);
            this.Controls.Add(this.gotoB);
            this.Controls.Add(this.Panel_S);
            this.Controls.Add(this.OriginalB);
            this.Controls.Add(this.Panel_B);
            this.Controls.Add(this.NextB);
            this.Controls.Add(this.PreviousB);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form_View";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Picture Viewer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_View_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_View_KeyDown);
            this.Panel_S.ResumeLayout(false);
            this.Panel_S.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic_S)).EndInit();
            this.Panel_B.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pic_B)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel Panel_S;
        private System.Windows.Forms.Button SearchB;
        private System.Windows.Forms.Button SearchDB;
        private System.Windows.Forms.Button DeleteB;
        private System.Windows.Forms.Button OriginalS;
        private System.Windows.Forms.Button NextS;
        private System.Windows.Forms.TextBox totalS;
        private System.Windows.Forms.TextBox gotoS;
        private System.Windows.Forms.Button Replace;
        private System.Windows.Forms.Button RemoveB;
        private System.Windows.Forms.Button PreviousS;
        private System.Windows.Forms.Button ToDataBase;
        private System.Windows.Forms.Button RemoveS;
        private System.Windows.Forms.PictureBox pic_S;
        private System.Windows.Forms.Panel Panel_B;
        private System.Windows.Forms.TextBox totalB;
        private System.Windows.Forms.TextBox gotoB;
        private System.Windows.Forms.Button OriginalB;
        private System.Windows.Forms.Button NextB;
        private System.Windows.Forms.Button PreviousB;
        private System.Windows.Forms.PictureBox pic_B;
        private System.Windows.Forms.Button ToDesk;
        private System.Windows.Forms.TextBox ExchangeIndex;
        private System.Windows.Forms.Button Export1;
        private System.Windows.Forms.Button Export2;
        private System.Windows.Forms.Button CopyTags;
        private System.Windows.Forms.Button PasteTags;
    }
}