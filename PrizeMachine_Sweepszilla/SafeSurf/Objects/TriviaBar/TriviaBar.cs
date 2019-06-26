using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
//using Microsoft.Win32;
using SCTVObjects;

namespace SCTVObjects
{
    public partial class TriviaBar : Form
    {
        // Threading
        static TriviaBar ms_frmTriviaBar = null;
        static Thread ms_oThread = null;

        // Fade in and out.
        private double m_dblOpacityIncrement = .05;
        private double m_dblOpacityDecrement = .08;
        private const int TIMER_INTERVAL = 50;

        // Status and progress bar
        static string ms_sStatus;
        private double m_dblCompletionFraction = 0;
        private Rectangle m_rProgress;

        // Progress smoothing
        private double m_dblLastCompletionFraction = 0.0;
        private double m_dblPBIncrementPerTimerInterval = .015;

        // Self-calibration support
        private bool m_bFirstLaunch = false;
        private DateTime m_dtStart;
        private bool m_bDTSet = false;
        private int m_iIndex = 1;
        private int m_iActualTicks = 0;
        private ArrayList m_alPreviousCompletionFraction;
        private ArrayList m_alActualTimes = new ArrayList();
        private const string REG_KEY_INITIALIZATION = "Initialization";
        private const string REGVALUE_PB_MILISECOND_INCREMENT = "Increment";
        private const string REGVALUE_PB_PERCENTS = "Percents";

        private static Media mediaTriviaToShow;
        private string[] triviaTemp;
        private int triviaIndex = 0;
        private int triviaWaitTime = 300;
        private int triviaTimeCounter = 0;
        private ArrayList trivia = new ArrayList();

        public TriviaBar()
        {
            InitializeComponent();

            if (mediaTriviaToShow != null)
            {
                string tempTriviaString = "";

                if (mediaTriviaToShow.Trivia != null && mediaTriviaToShow.Trivia.Length > 0)
                {
                    tempTriviaString = mediaTriviaToShow.Trivia.Replace("|||", "|");

                    triviaTemp = tempTriviaString.Split('|');
                }

                foreach (string tmpString in triviaTemp)
                {
                    if (tmpString.Trim().Length > 0 && !trivia.Contains(tmpString.Trim()))
                        trivia.Add(tmpString.Trim());
                }

                if (mediaTriviaToShow.Goofs != null && mediaTriviaToShow.Goofs.Length > 0)
                {
                    tempTriviaString = mediaTriviaToShow.Goofs.Replace("|||", "|");

                    triviaTemp = tempTriviaString.Split('|');
                }

                foreach (string tmpString in triviaTemp)
                {
                    if (tmpString.Trim().Length > 0 && !trivia.Contains(tmpString.Trim()))
                        trivia.Add(tmpString.Trim());
                }
            }

            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Top = 0;

            timer1.Start();
        }

        #region public methods
        // A static method to create the thread and 
        // launch the SplashScreen.
        static public void ShowTriviaBar(Media media)
        {
            // Make sure it's only launched once.
            if (ms_frmTriviaBar != null)
                return;

            mediaTriviaToShow = media;

            ms_oThread = new Thread(new ThreadStart(TriviaBar.ShowForm));
            ms_oThread.IsBackground = true;
            ms_oThread.ApartmentState = ApartmentState.STA;
            ms_oThread.Start();
        }

        // A property returning the splash screen instance
        static public TriviaBar TriviaForm
        {
            get
            {
                return ms_frmTriviaBar;
            }
        }

        // A private entry point for the thread.
        static private void ShowForm()
        {
            ms_frmTriviaBar = new TriviaBar();
            Application.Run(ms_frmTriviaBar);
        }

        // A static method to close the SplashScreen
        static public void CloseForm()
        {
            if (ms_frmTriviaBar != null && ms_frmTriviaBar.IsDisposed == false)
            {
                // Make it start going away.
                ms_frmTriviaBar.m_dblOpacityIncrement = -ms_frmTriviaBar.m_dblOpacityDecrement;
            }
            ms_oThread = null;	// we don't need these any more.
            ms_frmTriviaBar = null;
        }

        // A static method to set the status and update the reference.
        static public void SetStatus(string newStatus)
        {
            SetStatus(newStatus, true);
        }

        // A static method to set the status and optionally update the reference.
        // This is useful if you are in a section of code that has a variable
        // set of status string updates.  In that case, don't set the reference.
        static public void SetStatus(string newStatus, bool setReference)
        {
            ms_sStatus = newStatus;
            if (ms_frmTriviaBar == null)
                return;
            //if (setReference)
            //    ms_frmTriviaBar.SetReferenceInternal();
        }

        // Static method called from the initializing application to 
        // give the splash screen reference points.  Not needed if
        // you are using a lot of status strings.
        static public void SetReferencePoint()
        {
            if (ms_frmTriviaBar == null)
                return;
            //ms_frmTriviaBar.SetReferenceInternal();

        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            triviaTimeCounter++;

            if (triviaTimeCounter > triviaWaitTime)
            {
                triviaTimeCounter = 0;
                triviaIndex++;

                if (triviaIndex > trivia.Count)
                    triviaIndex = 0;

                lblTrivia.Refresh();
            }
        }

        private void lblTrivia_Paint(object sender, PaintEventArgs e)
        {
            if (e.ClipRectangle.Width > 0 && e.ClipRectangle.Height > 0 && trivia.Count > 0)
            {
                //draw media info
                Rectangle rec = e.ClipRectangle;
                rec.Height = e.ClipRectangle.Height - 4;
                rec.Width = e.ClipRectangle.Width - 4;
                rec.X = e.ClipRectangle.X + 2;
                rec.Y = e.ClipRectangle.Y + 2;

                Brush myBrush = Brushes.Black;

                e.Graphics.DrawString(trivia[triviaIndex].ToString(), this.Font, myBrush, rec, StringFormat.GenericDefault);

                //Rectangle backLeft = new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width / 2, e.ClipRectangle.Height);

                //LinearGradientBrush bgBrush = new LinearGradientBrush(backLeft, Color.Transparent, Color.SlateGray, LinearGradientMode.Horizontal);

                //if (bgBrush != null)
                //{
                //    using (bgBrush)
                //        e.Graphics.FillRectangle(bgBrush, backLeft);
                //}

                //Rectangle backRight = new Rectangle(e.ClipRectangle.X + e.ClipRectangle.Width / 2, e.ClipRectangle.Y, e.ClipRectangle.Width / 2, e.ClipRectangle.Height);

                //bgBrush = new LinearGradientBrush(backRight, Color.Transparent, Color.SlateGray, LinearGradientMode.Horizontal);

                //if (bgBrush != null)
                //{
                //    using (bgBrush)
                //        e.Graphics.FillRectangle(bgBrush, backRight);
                //}
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            triviaIndex++;
            lblTrivia.Refresh();
        }

        private void previousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            triviaIndex--;
            lblTrivia.Refresh();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void goToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Start();
            lblTrivia.Refresh();
        }
    }
}