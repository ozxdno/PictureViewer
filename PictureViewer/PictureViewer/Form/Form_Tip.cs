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

            int sh = Screen.PrimaryScreen.Bounds.Height;
            int sw = Screen.PrimaryScreen.Bounds.Width;

            int bgx = MousePosition.X + 20;
            int edx = bgx + this.Width;
            int bgy = MousePosition.Y + 20;
            int edy = bgy + this.Height;

            if (edx > sw) { edx = sw - 10; bgx = edx - this.Width; }
            if (edy > sh) { edy = MousePosition.Y - 20; bgy = edy - this.Height; }

            this.Location = new Point(bgx, bgy);
            this.Show();
        }
        public void hide()
        {
            this.Hide();
        }

        /// <summary>
        /// 按下键的键值
        /// </summary>
        public int KeyValue = -1;
        /// <summary>
        /// 键的状态：
        /// 0 - 按下
        /// 2 - 抬起
        /// </summary>
        public int KeyState = 0;

        private void Form_Tip_KeyDown(object sender, KeyEventArgs e)
        {
            KeyValue = e.KeyValue;
            KeyState = 0;
        }

        private void Form_Tip_KeyUp(object sender, KeyEventArgs e)
        {
            KeyValue = e.KeyValue;
            KeyState = 2;
        }
    }
}
