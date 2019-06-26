using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using SCTVObjects;

namespace SCTV
{
    public partial class SlidingBar : UserControl
    {
        private int inactivityTicker = 0;
        private int inactivityTime = 5;//time in seconds before no action is considered inactivity
        private Color backColor = Color.Transparent;
        private Color foreColor = Color.Transparent;
        private int slidingBarHeight = 50;

        public int inactivityTimerInterval
        {
            get { return inactivityTimer.Interval; }
            set { inactivityTimer.Interval = value; }
        }

        /// <summary>
        /// time in seconds before no action is considered inactivity
        /// </summary>
        public int InactivityTime
        {
            get { return inactivityTime; }
            set { inactivityTime = value; }
        }

        public Color BackColor
        {
            get { return BackColor; }
            set { backColor = value; }
        }

        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }

        public SlidingBar()
        {
            InitializeComponent();

            this.MouseLeave += new EventHandler(SlidingBar_MouseLeave);
            this.MouseMove += new MouseEventHandler(SlidingBar_MouseMove);

            this.Height = slidingBarHeight;

            inactivityTimer.Start();
        }

        void SlidingBar_MouseMove(object sender, MouseEventArgs e)
        {
            //show this
            //this.Height = 50;

            this.Visible = true;

            this.BackColor = backColor;

            inactivityTimer.Stop();

            inactivityTicker = 0;
        }

        void SlidingBar_MouseLeave(object sender, EventArgs e)
        {
            //start timer if mouse is off of mouse controls
            if (Cursor.Position.Y < this.Location.Y)
                inactivityTimer.Start();
        }

        void SlidingBar_Paint(object sender, PaintEventArgs e)
        {
            this.Refresh();
        }

        void SlidingBar_ControlAdded(object sender, ControlEventArgs e)
        {
            Control addedControl = (Control)sender;
            addedControl.Refresh();
        }

        private void inactivityTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                inactivityTicker++;

                if (inactivityTicker > inactivityTime)
                {
                    //int distanceToMove = this.Height + 2;
                    //int distanceMoved = 0;

                    inactivityTimer.Stop();

                    //hide this
                    //while (this.Height > 10)
                    //{
                    //    this.Height = this.Height - 1;
                    //    distanceMoved += 1;
                    //}

                    //this.Visible = false;
                    //this.BackColor = Color.Black;

                    //hide mouse on right side of screen
                    Cursor.Position = new Point(this.Width, Cursor.Position.Y);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile("inactivityTimer_Tick", ex.Message);
            }
        }

        private void pnlCover_MouseLeave(object sender, EventArgs e)
        {
            //start timer if mouse is off of mouse controls
            if (Cursor.Position.Y < this.Location.Y)
                inactivityTimer.Start();
        }

        private void pnlCover_MouseMove(object sender, MouseEventArgs e)
        {
            //show this
            this.Height = 50;

            this.Visible = true;

            this.BackColor = backColor;

            inactivityTimer.Stop();

            inactivityTicker = 0;
        }

        
    }
}
