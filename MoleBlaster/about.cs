using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoleBlaster
{
    public partial class about : Form
    {
        public about()
        {
            InitializeComponent();
            label1.Text = "Moleblaster version " + Globals.version + "\r\n Written by W. Ryan Parker\r\n";
            linkLabel1.Text = "http://github.com/wrparker/moleblaster";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }
    }
}
