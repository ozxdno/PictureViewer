namespace PictureViewer.Form_CMC
{
    partial class Form_CMC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_CMC));
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panel_pic = new System.Windows.Forms.Panel();
            this.SetTo = new System.Windows.Forms.TextBox();
            this.SetToLabel = new System.Windows.Forms.Label();
            this.Previous = new System.Windows.Forms.Button();
            this.Next = new System.Windows.Forms.Button();
            this.GoTo = new System.Windows.Forms.TextBox();
            this.total = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panel_pic.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(781, 536);
            this.pictureBox.TabIndex = 17;
            this.pictureBox.TabStop = false;
            this.pictureBox.DoubleClick += new System.EventHandler(this.pictureBox_DoubleClick);
            // 
            // panel_pic
            // 
            this.panel_pic.AutoScroll = true;
            this.panel_pic.Controls.Add(this.pictureBox);
            this.panel_pic.Location = new System.Drawing.Point(1, 1);
            this.panel_pic.Name = "panel_pic";
            this.panel_pic.Size = new System.Drawing.Size(781, 539);
            this.panel_pic.TabIndex = 18;
            // 
            // SetTo
            // 
            this.SetTo.Location = new System.Drawing.Point(53, 545);
            this.SetTo.Name = "SetTo";
            this.SetTo.Size = new System.Drawing.Size(61, 21);
            this.SetTo.TabIndex = 18;
            this.SetTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetTo_KeyDown);
            // 
            // SetToLabel
            // 
            this.SetToLabel.AutoSize = true;
            this.SetToLabel.Location = new System.Drawing.Point(12, 549);
            this.SetToLabel.Name = "SetToLabel";
            this.SetToLabel.Size = new System.Drawing.Size(35, 12);
            this.SetToLabel.TabIndex = 19;
            this.SetToLabel.Text = "Set :";
            // 
            // Previous
            // 
            this.Previous.Location = new System.Drawing.Point(229, 544);
            this.Previous.Name = "Previous";
            this.Previous.Size = new System.Drawing.Size(67, 23);
            this.Previous.TabIndex = 12;
            this.Previous.Text = "Previous";
            this.Previous.UseVisualStyleBackColor = true;
            this.Previous.Click += new System.EventHandler(this.Previous_Click);
            // 
            // Next
            // 
            this.Next.Location = new System.Drawing.Point(524, 544);
            this.Next.Name = "Next";
            this.Next.Size = new System.Drawing.Size(67, 23);
            this.Next.TabIndex = 13;
            this.Next.Text = "Next";
            this.Next.UseVisualStyleBackColor = true;
            this.Next.Click += new System.EventHandler(this.Next_Click);
            // 
            // GoTo
            // 
            this.GoTo.Location = new System.Drawing.Point(302, 546);
            this.GoTo.Name = "GoTo";
            this.GoTo.Size = new System.Drawing.Size(52, 21);
            this.GoTo.TabIndex = 15;
            this.GoTo.TextChanged += new System.EventHandler(this.GoTo_TextChanged);
            this.GoTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GoTo_KeyDown);
            // 
            // total
            // 
            this.total.Location = new System.Drawing.Point(360, 546);
            this.total.Name = "total";
            this.total.ReadOnly = true;
            this.total.Size = new System.Drawing.Size(158, 21);
            this.total.TabIndex = 16;
            this.total.Text = "Total: 0 / Size: 0";
            // 
            // Form_CMC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 569);
            this.Controls.Add(this.SetToLabel);
            this.Controls.Add(this.SetTo);
            this.Controls.Add(this.panel_pic);
            this.Controls.Add(this.total);
            this.Controls.Add(this.GoTo);
            this.Controls.Add(this.Next);
            this.Controls.Add(this.Previous);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Form_CMC";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CMC Viewer";
            this.ResizeEnd += new System.EventHandler(this.Form_CMC_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.Form_CMC_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_CMC_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panel_pic.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Panel panel_pic;
        private System.Windows.Forms.TextBox SetTo;
        private System.Windows.Forms.Label SetToLabel;
        private System.Windows.Forms.Button Previous;
        private System.Windows.Forms.Button Next;
        private System.Windows.Forms.TextBox GoTo;
        private System.Windows.Forms.TextBox total;
    }
}