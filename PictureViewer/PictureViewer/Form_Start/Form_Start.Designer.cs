namespace PictureViewer
{
    partial class Form_Start
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Start));
            this.ADD = new System.Windows.Forms.Button();
            this.DEL = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.VIEW = new System.Windows.Forms.Button();
            this.TagExp = new System.Windows.Forms.TextBox();
            this.CFG = new System.Windows.Forms.Button();
            this.listbox_list = new System.Windows.Forms.ListBox();
            this.listbox_tagstr = new System.Windows.Forms.ListBox();
            this.listbox_tag = new System.Windows.Forms.ListBox();
            this.DispartLabel = new System.Windows.Forms.Label();
            this.Welcom = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // ADD
            // 
            this.ADD.Location = new System.Drawing.Point(84, 62);
            this.ADD.Name = "ADD";
            this.ADD.Size = new System.Drawing.Size(42, 20);
            this.ADD.TabIndex = 14;
            this.ADD.Text = "ADD";
            this.ADD.UseVisualStyleBackColor = true;
            this.ADD.Click += new System.EventHandler(this.ADD_Click);
            // 
            // DEL
            // 
            this.DEL.Location = new System.Drawing.Point(42, 62);
            this.DEL.Name = "DEL";
            this.DEL.Size = new System.Drawing.Size(42, 20);
            this.DEL.TabIndex = 15;
            this.DEL.Text = "DEL";
            this.DEL.UseVisualStyleBackColor = true;
            this.DEL.Click += new System.EventHandler(this.DEL_Click);
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(126, 62);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(42, 20);
            this.OK.TabIndex = 16;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // VIEW
            // 
            this.VIEW.Location = new System.Drawing.Point(168, 62);
            this.VIEW.Name = "VIEW";
            this.VIEW.Size = new System.Drawing.Size(42, 20);
            this.VIEW.TabIndex = 17;
            this.VIEW.Text = "VIEW";
            this.VIEW.UseVisualStyleBackColor = true;
            this.VIEW.Click += new System.EventHandler(this.VIEW_Click);
            // 
            // TagExp
            // 
            this.TagExp.AcceptsReturn = true;
            this.TagExp.Location = new System.Drawing.Point(0, 1);
            this.TagExp.Multiline = true;
            this.TagExp.Name = "TagExp";
            this.TagExp.Size = new System.Drawing.Size(210, 55);
            this.TagExp.TabIndex = 18;
            this.TagExp.TextChanged += new System.EventHandler(this.TagExp_TextChanged);
            // 
            // CFG
            // 
            this.CFG.Location = new System.Drawing.Point(0, 62);
            this.CFG.Name = "CFG";
            this.CFG.Size = new System.Drawing.Size(42, 20);
            this.CFG.TabIndex = 19;
            this.CFG.Text = "CFG";
            this.CFG.UseVisualStyleBackColor = true;
            this.CFG.Click += new System.EventHandler(this.CFG_Click);
            // 
            // listbox_list
            // 
            this.listbox_list.FormattingEnabled = true;
            this.listbox_list.ItemHeight = 12;
            this.listbox_list.Location = new System.Drawing.Point(8, 100);
            this.listbox_list.Name = "listbox_list";
            this.listbox_list.Size = new System.Drawing.Size(72, 460);
            this.listbox_list.TabIndex = 20;
            this.listbox_list.SelectedIndexChanged += new System.EventHandler(this.listbox_list_SelectedIndexChanged);
            // 
            // listbox_tagstr
            // 
            this.listbox_tagstr.FormattingEnabled = true;
            this.listbox_tagstr.ItemHeight = 12;
            this.listbox_tagstr.Location = new System.Drawing.Point(88, 268);
            this.listbox_tagstr.Name = "listbox_tagstr";
            this.listbox_tagstr.Size = new System.Drawing.Size(114, 292);
            this.listbox_tagstr.TabIndex = 21;
            this.listbox_tagstr.SelectedIndexChanged += new System.EventHandler(this.listbox_tagstr_SelectedIndexChanged);
            // 
            // listbox_tag
            // 
            this.listbox_tag.FormattingEnabled = true;
            this.listbox_tag.ItemHeight = 12;
            this.listbox_tag.Location = new System.Drawing.Point(88, 100);
            this.listbox_tag.Name = "listbox_tag";
            this.listbox_tag.Size = new System.Drawing.Size(114, 160);
            this.listbox_tag.TabIndex = 22;
            this.listbox_tag.SelectedIndexChanged += new System.EventHandler(this.listbox_tag_SelectedIndexChanged);
            // 
            // DispartLabel
            // 
            this.DispartLabel.AutoSize = true;
            this.DispartLabel.Location = new System.Drawing.Point(-2, 85);
            this.DispartLabel.Name = "DispartLabel";
            this.DispartLabel.Size = new System.Drawing.Size(221, 12);
            this.DispartLabel.TabIndex = 23;
            this.DispartLabel.Text = "------------------------------------";
            // 
            // Welcom
            // 
            this.Welcom.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Welcom.BackgroundImage")));
            this.Welcom.Location = new System.Drawing.Point(0, 100);
            this.Welcom.Name = "Welcom";
            this.Welcom.Size = new System.Drawing.Size(210, 460);
            this.Welcom.TabIndex = 24;
            this.Welcom.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Welcom_MouseClick);
            // 
            // Form_Start
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(211, 569);
            this.Controls.Add(this.Welcom);
            this.Controls.Add(this.DispartLabel);
            this.Controls.Add(this.listbox_tag);
            this.Controls.Add(this.listbox_tagstr);
            this.Controls.Add(this.listbox_list);
            this.Controls.Add(this.CFG);
            this.Controls.Add(this.TagExp);
            this.Controls.Add(this.VIEW);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.DEL);
            this.Controls.Add(this.ADD);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form_Start";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ozxdno";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormStart_FormClosed);
            this.Load += new System.EventHandler(this.Form_Start_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ADD;
        private System.Windows.Forms.Button DEL;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button VIEW;
        private System.Windows.Forms.TextBox TagExp;
        private System.Windows.Forms.Button CFG;
        private System.Windows.Forms.ListBox listbox_list;
        private System.Windows.Forms.ListBox listbox_tag;
        private System.Windows.Forms.Label DispartLabel;
        public System.Windows.Forms.ListBox listbox_tagstr;
        private System.Windows.Forms.Panel Welcom;
    }
}

