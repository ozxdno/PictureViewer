using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PictureViewer
{
    public partial class Form_Tip : Form
    {
        public Form_Tip()
        {
            InitializeComponent();
        }

        public void show(string tipMsg)
        {
            if (tipMsg == null) { tipMsg = ""; }

            Graphics g = this.label1.CreateGraphics();
            SizeF sz = g.MeasureString(tipMsg, this.label1.Font);

            this.label1.Location = new Point(1, 1);
            this.label1.Text = tipMsg;
            this.Height = this.label1.Height + 2;
            //this.Width = (int)sz.Width + 3;
            this.Width = this.label1.Width + 12;
            this.Location = new Point(MousePosition.X + 20, MousePosition.Y + 20);
            
            this.Show();
        }
        public void hide()
        {
            this.Hide();
        }

        public int KeyValue = -1;
        private void Form_Tip_KeyDown(object sender, KeyEventArgs e)
        {
            KeyValue = e.KeyValue;
        }
    }
}
