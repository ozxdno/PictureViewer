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
    public partial class Form_Input : Form
    {
        ////////////////////////////////////////////////////// public attribute //////////////////////////////////////////

        public string Input;

        ////////////////////////////////////////////////////// private attribute //////////////////////////////////////////

        ////////////////////////////////////////////////////// public method //////////////////////////////////////////

        public Form_Input(string input = "")
        {
            InitializeComponent();
            this.textBox1.Text = input;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13) { Input = this.textBox1.Text; this.Close(); }
        }

        ////////////////////////////////////////////////////// private method //////////////////////////////////////////
    }
}
