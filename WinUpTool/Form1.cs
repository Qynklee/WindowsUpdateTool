using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinUpTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult abouttool = MessageBox.Show("V1.0.0 - Create Tool for fun !");
        }

        private void meToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult abouttool = MessageBox.Show("I'm Qynklee, a idiot with laziness", "Konnichiwa");
        }
    }
}
