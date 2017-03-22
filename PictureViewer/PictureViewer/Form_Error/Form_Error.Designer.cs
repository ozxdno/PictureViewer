namespace PictureViewer.Form_Error
{
    partial class Form_Error
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
            this.Send = new System.Windows.Forms.Button();
            this.About = new System.Windows.Forms.TextBox();
            this.Email = new System.Windows.Forms.TextBox();
            this.ErrorMSG = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Send
            // 
            this.Send.Location = new System.Drawing.Point(211, 90);
            this.Send.Name = "Send";
            this.Send.Size = new System.Drawing.Size(43, 21);
            this.Send.TabIndex = 8;
            this.Send.Text = "Send";
            this.Send.UseVisualStyleBackColor = true;
            this.Send.Click += new System.EventHandler(this.Send_Click);
            // 
            // About
            // 
            this.About.Location = new System.Drawing.Point(12, 133);
            this.About.Name = "About";
            this.About.Size = new System.Drawing.Size(242, 21);
            this.About.TabIndex = 7;
            this.About.Text = "About: ";
            // 
            // Email
            // 
            this.Email.Location = new System.Drawing.Point(12, 90);
            this.Email.Name = "Email";
            this.Email.Size = new System.Drawing.Size(193, 21);
            this.Email.TabIndex = 6;
            this.Email.Text = "Email: ";
            // 
            // ErrorMSG
            // 
            this.ErrorMSG.Location = new System.Drawing.Point(12, 22);
            this.ErrorMSG.Multiline = true;
            this.ErrorMSG.Name = "ErrorMSG";
            this.ErrorMSG.ReadOnly = true;
            this.ErrorMSG.Size = new System.Drawing.Size(242, 37);
            this.ErrorMSG.TabIndex = 5;
            this.ErrorMSG.Text = "Sorry: An error occured in our program, send the feedback to us.";
            // 
            // Form_Error
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 177);
            this.Controls.Add(this.Send);
            this.Controls.Add(this.About);
            this.Controls.Add(this.Email);
            this.Controls.Add(this.ErrorMSG);
            this.Name = "Form_Error";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Error";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Send;
        private System.Windows.Forms.TextBox About;
        private System.Windows.Forms.TextBox Email;
        private System.Windows.Forms.TextBox ErrorMSG;
    }
}