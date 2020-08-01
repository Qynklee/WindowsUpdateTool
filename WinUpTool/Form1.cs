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
        public SystemTweak ST = new SystemTweak();
        public Form1()
        {
            if (ST.IsRunAsAdmin())
            {
                InitializeComponent();
                DialogResult abouttool = MessageBox.Show("Tool chỉ block update Windows không tắt services Windows Update :))))");
                ReloadStatusBlocking();
            }
            else
            {
                DialogResult abouttool = MessageBox.Show("Please rerun app as Admin !");
                exitToolStripMenuItem.PerformClick();
            }
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

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        public void ReloadStatusBlocking()
        {
            if (ST.NowIsBlocking() == 0)
            {
                label_Status.Text = "Update is Running!";
            }
            else
            {
                label_Status.Text = "Update is Blocked!";
            }
        }
    }
}
