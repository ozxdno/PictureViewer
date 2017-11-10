namespace PictureViewer
{
    partial class Form_Find
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Find));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.source1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.source2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.partToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.likeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.turnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.export2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.degreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pixesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Location = new System.Drawing.Point(197, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(320, 320);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.DoubleClickedToSwitch);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.source1ToolStripMenuItem,
            this.source2ToolStripMenuItem,
            this.modeToolStripMenuItem,
            this.toolStripMenuItem1,
            this.startToolStripMenuItem,
            this.restartToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.export2ToolStripMenuItem,
            this.degreeToolStripMenuItem,
            this.pixesToolStripMenuItem,
            this.switchToolStripMenuItem,
            this.openToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(124, 274);
            // 
            // source1ToolStripMenuItem
            // 
            this.source1ToolStripMenuItem.Name = "source1ToolStripMenuItem";
            this.source1ToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.source1ToolStripMenuItem.Text = "Source1";
            this.source1ToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Source1);
            // 
            // source2ToolStripMenuItem
            // 
            this.source2ToolStripMenuItem.Name = "source2ToolStripMenuItem";
            this.source2ToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.source2ToolStripMenuItem.Text = "Source2";
            this.source2ToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Source2);
            // 
            // modeToolStripMenuItem
            // 
            this.modeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullToolStripMenuItem,
            this.partToolStripMenuItem,
            this.sameToolStripMenuItem,
            this.likeToolStripMenuItem,
            this.turnToolStripMenuItem});
            this.modeToolStripMenuItem.Name = "modeToolStripMenuItem";
            this.modeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.modeToolStripMenuItem.Text = "Mode";
            // 
            // fullToolStripMenuItem
            // 
            this.fullToolStripMenuItem.Name = "fullToolStripMenuItem";
            this.fullToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.fullToolStripMenuItem.Text = "Full";
            this.fullToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Mode_Full);
            // 
            // partToolStripMenuItem
            // 
            this.partToolStripMenuItem.Name = "partToolStripMenuItem";
            this.partToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.partToolStripMenuItem.Text = "Part";
            this.partToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Mode_Part);
            // 
            // sameToolStripMenuItem
            // 
            this.sameToolStripMenuItem.Name = "sameToolStripMenuItem";
            this.sameToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.sameToolStripMenuItem.Text = "Same";
            this.sameToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Mode_Same);
            // 
            // likeToolStripMenuItem
            // 
            this.likeToolStripMenuItem.Name = "likeToolStripMenuItem";
            this.likeToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.likeToolStripMenuItem.Text = "Like";
            this.likeToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Mode_Like);
            // 
            // turnToolStripMenuItem
            // 
            this.turnToolStripMenuItem.Name = "turnToolStripMenuItem";
            this.turnToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.turnToolStripMenuItem.Text = "Turn";
            this.turnToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Mode_Turn);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(120, 6);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Start);
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.restartToolStripMenuItem.Text = "Restart";
            this.restartToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Restart);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Remove);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Export);
            // 
            // export2ToolStripMenuItem
            // 
            this.export2ToolStripMenuItem.Name = "export2ToolStripMenuItem";
            this.export2ToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.export2ToolStripMenuItem.Text = "Export2";
            this.export2ToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Export2);
            // 
            // degreeToolStripMenuItem
            // 
            this.degreeToolStripMenuItem.Name = "degreeToolStripMenuItem";
            this.degreeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.degreeToolStripMenuItem.Text = "Degree";
            this.degreeToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Degree);
            // 
            // pixesToolStripMenuItem
            // 
            this.pixesToolStripMenuItem.Name = "pixesToolStripMenuItem";
            this.pixesToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.pixesToolStripMenuItem.Text = "Pixes";
            this.pixesToolStripMenuItem.Visible = false;
            this.pixesToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Pixes);
            // 
            // switchToolStripMenuItem
            // 
            this.switchToolStripMenuItem.Name = "switchToolStripMenuItem";
            this.switchToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.switchToolStripMenuItem.Text = "Switch";
            this.switchToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Switch);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.RightMenu_Open);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox2.Location = new System.Drawing.Point(12, 1);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(178, 151);
            this.pictureBox2.TabIndex = 15;
            this.pictureBox2.TabStop = false;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("宋体", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(200, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 96);
            this.label4.TabIndex = 16;
            this.label4.Text = "<";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label4.Visible = false;
            this.label4.Click += new System.EventHandler(this.Previous);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("宋体", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(480, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 96);
            this.label5.TabIndex = 17;
            this.label5.Text = ">";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label5.Visible = false;
            this.label5.Click += new System.EventHandler(this.Next);
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(11, 186);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(180, 124);
            this.listBox1.TabIndex = 18;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.ListIndexChanged);
            // 
            // toolTip2
            // 
            this.toolTip2.ShowAlways = true;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.NavajoWhite;
            this.textBox1.Location = new System.Drawing.Point(11, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(180, 153);
            this.textBox1.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.DarkOrange;
            this.label1.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(197, 274);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 46);
            this.label1.TabIndex = 21;
            this.label1.Text = "Open";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Visible = false;
            this.label1.Click += new System.EventHandler(this.RightMenu_Open);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.DarkOrange;
            this.label2.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(361, 274);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 46);
            this.label2.TabIndex = 22;
            this.label2.Text = "Export";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Visible = false;
            this.label2.Click += new System.EventHandler(this.RightMenu_Export);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(11, 160);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(180, 20);
            this.comboBox1.TabIndex = 23;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboIndexChanged);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(67, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 24);
            this.label3.TabIndex = 24;
            this.label3.Text = "↓";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.Visible = false;
            this.label3.Click += new System.EventHandler(this.NextSour);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(67, 1);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 24);
            this.label6.TabIndex = 25;
            this.label6.Text = "↑";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label6.Visible = false;
            this.label6.Click += new System.EventHandler(this.PreviousSour);
            // 
            // Form_Find
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 326);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form_Find";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form_Find";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form_Close);
            this.Load += new System.EventHandler(this.Form_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem export2ToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem pixesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem degreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem switchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fullToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem partToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem likeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem turnToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem source1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem source2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
    }
}