using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PictureViewer.Form_Error
{
    public partial class Form_Error : Form
    {
        public Form_Error()
        {
            InitializeComponent();
        }

        private void Send_Click(object sender, EventArgs e)
        {
            PVproject.Username = this.Email.Text;
            PVproject.Password = this.About.Text; this.Close();
        }
    }
}
