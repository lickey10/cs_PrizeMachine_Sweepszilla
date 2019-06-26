using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterForm
{
    public partial class Form1 : Form
    {
        SCTV.MainForm safeSurf = null;
        Timer waitTimer = new Timer();
        string newUrl = "";

        public Form1()
        {
            InitializeComponent();
            
            waitTimer.Interval = 1000;
            waitTimer.Tick += WaitTimer_Tick;
            waitTimer.Enabled = true;

            safeSurf = new SCTV.MainForm();
            safeSurf.FormClosed += SafeSurf_FormClosed;
            safeSurf.FormClosing += SafeSurf_FormClosing;

            safeSurf.Show();
        }

        private void SafeSurf_FormClosing(object sender, FormClosingEventArgs e)
        {
            newUrl = safeSurf.URL.ToString();
        }

        private void WaitTimer_Tick(object sender, EventArgs e)
        {
            waitTimer.Stop();

            if (safeSurf != null)
            {
                safeSurf.Dispose();
                safeSurf = null;
            }

            safeSurf = new SCTV.MainForm();

            if(newUrl.Length > 0)
                safeSurf.URL = new Uri(newUrl);

            newUrl = "";
        }

        private void SafeSurf_FormClosed(object sender, FormClosedEventArgs e)
        {
            //wait a few seconds and restart the form
            waitTimer.Stop();
            waitTimer.Interval = 3000;
            waitTimer.Start();
        }
    }
}
