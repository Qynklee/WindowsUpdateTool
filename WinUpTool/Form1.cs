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
                timer1.Start();
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
            MessageBox.Show("V1.0.0 - Create Tool for fun !");
        }

        private void meToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("I'm Qynklee, an idiot with laziness", "Konnichiwa");
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
            label_SvStt.Text = ST.CheckStatusServiceUpdate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ReloadStatusBlocking();

        }

        private void button_Enable_Click(object sender, EventArgs e)
        {
            ST.EnableUpdate();

        }

        private void button_Block_Click(object sender, EventArgs e)
        {
            if (ST.BlockUpdate())
            {
                MessageBox.Show("Block Update Successfully");
            }
            else
            {
                MessageBox.Show("ERRORRR :((((");
            }
        }

        private void button_Service_Click(object sender, EventArgs e)
        {
            ST.ChangeStartupTypeServiceUpdate();
            MessageBox.Show("Changed!");
        }

        private void guideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Service Update turn off: You can download app in Store!");
            MessageBox.Show("Block Update: You dont need update Windows anymore");
            MessageBox.Show("Block Update does not same Turn off Service Update");

        }
    }

}
