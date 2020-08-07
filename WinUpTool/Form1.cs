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
            if (ST.IsRunAsAdmin() && ST.CheckBuildAndEditionWindows())
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
            MessageBox.Show("V1.1.0 - Create Tool for fun !");
        }

        private void meToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("I'm Qynklee, an idiot with laziness", "Konnichiwa");
        }


        public void ReloadStatusBlocking()
        {

            label_Status.Text = ST.NowIsBlocking();
            label_SvStt.Text = ST.CheckStatusServiceUpdate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ReloadStatusBlocking();

        }

        private void button_Enable_Click(object sender, EventArgs e)
        {
            ST.EnableUpdate();
            MessageBox.Show("Done!");
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
            MessageBox.Show("Service Update disable: You cant download app in Store!");
            MessageBox.Show("Block Update: You dont need update Windows anymore");
            MessageBox.Show("Pause Update until 2050: You will need update windows in 2050/1/1 :vvv");
            MessageBox.Show("Best Block Update: Good Choice if you want to forget all update :D");
            MessageBox.Show("Block Update does not same Turn off Service Update");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult YourChoice = MessageBox.Show("This option = Block Update + Disable Service Update + . Continue?", "Are you sure ?", MessageBoxButtons.YesNo);
            if (YourChoice == DialogResult.Yes)
            {
                ST.ChangeStartupTypeServiceUpdate();
                ST.BlockUpdate();
                MessageBox.Show("Stop + Block is done !");
            }
        }
    }

}
