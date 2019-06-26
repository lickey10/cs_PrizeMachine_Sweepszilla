using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;
using SCTVObjects;

namespace SCTVObjects
{
    public partial class SplashScreenNew : Form
    {
        // Threading
        static SplashScreenNew ms_frmSplash = null;
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

        private static Media mediaToShow;
        private static string splashMessage = "";

        public SplashScreenNew()
        {
            InitializeComponent();

            if (mediaToShow == null)
            {
                Media sctvStartup = new Media();
                sctvStartup.Title = "SCTV";
                sctvStartup.coverImage = "images/tv.jpg";
                sctvStartup.Description = "Home Theater/Home Automation/Entertainment system";

                mediaToShow = sctvStartup;
            }

            positionPictureBox();

            this.Refresh();

            this.Opacity = .00;
            timer1.Interval = TIMER_INTERVAL;
            timer1.Start();
        }

        private void positionPictureBox()
        {
            //center pbMedia vertically
            pbMedia.Top = Screen.PrimaryScreen.Bounds.Height / 2 - pbMedia.Bounds.Height / 2;

            lblLoading.Top = pbMedia.Top - 40 - lblLoading.Height;

            pbLoading.Top = pbMedia.Top + pbMedia.Height + 40;
        }

        #region public methods
        // A static method to create the thread and 
        // launch the SplashScreen.
        static public void ShowSplashScreen(Media MediaToShow)
        {
            // Make sure it's only launched once.
            if (ms_frmSplash != null)
                return;

            mediaToShow = MediaToShow;

            ms_oThread = new Thread(new ThreadStart(SplashScreenNew.ShowForm));
            ms_oThread.IsBackground = true;
            ms_oThread.ApartmentState = ApartmentState.STA;
            ms_oThread.Start();
        }

        // A static method to create the thread and 
        // launch the SplashScreen.
        static public void ShowSplashScreen(string Message)
        {
            // Make sure it's only launched once.
            if (ms_frmSplash != null)
                return;

            splashMessage = Message;

            ms_oThread = new Thread(new ThreadStart(SplashScreenNew.ShowForm));
            ms_oThread.IsBackground = true;
            ms_oThread.ApartmentState = ApartmentState.STA;
            ms_oThread.Start();
        }

        // A property returning the splash screen instance
        static public SplashScreenNew SplashForm
        {
            get
            {
                return ms_frmSplash;
            }
        }

        // A private entry point for the thread.
        static private void ShowForm()
        {
            ms_frmSplash = new SplashScreenNew();
            Application.Run(ms_frmSplash);
        }

        // A static method to close the SplashScreen
        static public void CloseForm()
        {
            try
            {
                if (ms_frmSplash != null && ms_frmSplash.IsDisposed == false)
                {
                    // Make it start going away.
                    ms_frmSplash.m_dblOpacityIncrement = -ms_frmSplash.m_dblOpacityDecrement;
                }
                ms_oThread = null;  // we don't need these any more.
                ms_frmSplash = null;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "CloseForm error: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
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
            if (ms_frmSplash == null)
                return;
            //if (setReference)
            //    ms_frmSplash.SetReferenceInternal();
        }

        // Static method called from the initializing application to 
        // give the splash screen reference points.  Not needed if
        // you are using a lot of status strings.
        static public void SetReferencePoint()
        {
            if (ms_frmSplash == null)
                return;
            //ms_frmSplash.SetReferenceInternal();

        }

        #endregion

        // Utility function to return elapsed Milliseconds since the 
        // SplashScreen was launched.
        private double ElapsedMilliSeconds()
        {
            TimeSpan ts = DateTime.Now - m_dtStart;
            return ts.TotalMilliseconds;
        }

        // Tick Event handler for the Timer control.  Handle fade in and fade out.  Also
        // handle the smoothed progress bar.
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            //lblStatus.Text = ms_sStatus;

            if (m_dblOpacityIncrement > 0)
            {
                m_iActualTicks++;
                if (this.Opacity < 1)
                    this.Opacity += m_dblOpacityIncrement;
            }
            else
            {
                if (this.Opacity > 0)
                    this.Opacity += m_dblOpacityIncrement;
                else
                {
                    //StoreIncrements();
                    this.Close();
                    Debug.WriteLine("Called this.Close()");
                }
            }
            if (m_bFirstLaunch == false && m_dblLastCompletionFraction < m_dblCompletionFraction)
            {
                m_dblLastCompletionFraction += m_dblPBIncrementPerTimerInterval;
                //int width = (int)Math.Floor(pnlStatus.ClientRectangle.Width * m_dblLastCompletionFraction);
                //int height = pnlStatus.ClientRectangle.Height;
                //int x = pnlStatus.ClientRectangle.X;
                //int y = pnlStatus.ClientRectangle.Y;
                //if (width > 0 && height > 0)
                //{
                //    m_rProgress = new Rectangle(x, y, width, height);
                    //pnlStatus.Invalidate(m_rProgress);
                    //int iSecondsLeft = 1 + (int)(TIMER_INTERVAL * ((1.0 - m_dblLastCompletionFraction) / m_dblPBIncrementPerTimerInterval)) / 1000;
                    //if (iSecondsLeft == 1)
                    //    lblTimeRemaining.Text = string.Format("1 second remaining");
                    //else
                    //    lblTimeRemaining.Text = string.Format("{0} seconds remaining", iSecondsLeft);

                //}
            }

            if (pbLoading.Value >= pbLoading.Maximum)
                pbLoading.Value = 0;

            pbLoading.Value += 1;
        }

        // Paint the portion of the panel invalidated during the tick event.
        private void pnlStatus_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (m_bFirstLaunch == false && e.ClipRectangle.Width > 0 && m_iActualTicks > 1)
            {
                if (m_rProgress.Width > 0 && m_rProgress.Height > 0)
                {
                    LinearGradientBrush brBackground = new LinearGradientBrush(m_rProgress, Color.FromArgb(58, 96, 151), Color.FromArgb(181, 237, 254), LinearGradientMode.Horizontal);
                    e.Graphics.FillRectangle(brBackground, m_rProgress);
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void maximizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pbMedia_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (mediaToShow != null)
                {
                    //check to see if item is the last item in the listview and add more if necessary and available
                    int textHeight = 18;// (int)(e.Graphics.MeasureString("WWW", lvFF.Font).Height + 4);
                    Brush myBrush = Brushes.Black;
                    Rectangle rectBottom = new Rectangle();
                    Rectangle rectBackground = new Rectangle();
                    Rectangle coverImageRect = new Rectangle(e.ClipRectangle.X + 10, e.ClipRectangle.Y + textHeight, 87, 140);
                    LinearGradientBrush bgBrush = null;
                    string description = "";

                    string coverImagePath = "";

                    coverImagePath = mediaToShow.coverImage;

                    if (!System.IO.File.Exists(coverImagePath))
                        coverImagePath = Application.StartupPath + "\\images\\media\\coverImages\\notavailable.jpg";

                    System.Drawing.Image coverImage = System.Drawing.Image.FromFile(coverImagePath);

                    if (coverImageRect.Width > 0 && coverImageRect.Height > 0 && e.ClipRectangle.Width > 0 && e.ClipRectangle.Height > 0)
                    {
                        rectBackground = new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height - (coverImageRect.X + coverImageRect.Height - 60));
                        rectBottom = new Rectangle(e.ClipRectangle.X, rectBackground.Location.Y + rectBackground.Height - 2, e.ClipRectangle.Width, e.ClipRectangle.Height - rectBackground.Height + 2);

                        bgBrush = new LinearGradientBrush(rectBackground, Color.FromArgb(60, 191, 255), Color.SlateGray, LinearGradientMode.Vertical);

                        if (bgBrush != null)
                            using (bgBrush)
                                e.Graphics.FillRectangle(bgBrush, rectBackground);

                        bgBrush = new LinearGradientBrush(rectBottom, Color.SlateGray, Color.FromArgb(60, 191, 255), LinearGradientMode.Vertical);

                        if (bgBrush != null)
                            using (bgBrush)
                                e.Graphics.FillRectangle(bgBrush, rectBottom);
                    }

                    if (e.ClipRectangle.Width > 0 && e.ClipRectangle.Height > 0)
                    {
                        //draw description
                        Rectangle rec = e.ClipRectangle;
                        rec.Height = e.ClipRectangle.Height - textHeight;
                        rec.Width = e.ClipRectangle.Width - 97;
                        rec.X = rec.X + 100;
                        rec.Y = rec.Y + textHeight;

                        description = mediaToShow.Title;

                        if (mediaToShow.Rating != null && mediaToShow.Rating.Trim().Length > 0)
                            description += " (" + mediaToShow.Rating + ")";

                        description += Environment.NewLine + mediaToShow.Description;

                        e.Graphics.DrawString(description, this.Font, myBrush, rec, StringFormat.GenericDefault);
                    }

                    //System.Drawing.Image tmpImage = new Bitmap(coverImage, coverImage.Width, coverImage.Height);

                    //draw black outline
                    try
                    {
                        using (Graphics g = Graphics.FromImage(coverImage))
                            g.DrawRectangle(Pens.Black, 0, 0, coverImage.Width - 1, coverImage.Height - 1);
                    }
                    catch (Exception ex)
                    {
                        Tools.WriteToFile(ex);
                    }

                    //e.Graphics.DrawRectangle(Pens.Black, 0, 0, tmpImage.Width - 1, tmpImage.Height - 1);

                    //using (Graphics g = e.Graphics)
                    //    g.DrawRectangle(Pens.Black, 0, 0, tmpImage.Width - 1, tmpImage.Height - 1);

                    //draw cover image
                    e.Graphics.DrawImage(coverImage, coverImageRect);

                    //draw mirror image
                    if (rectBottom != null)
                    {
                        System.Drawing.Imaging.ColorMatrix matrix = new System.Drawing.Imaging.ColorMatrix();
                        matrix.Matrix33 = 0.3f; //opacity 0 = completely transparent, 1 = completely opaque

                        System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();
                        attributes.SetColorMatrix(matrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);

                        Rectangle coverImageMirrorRectDest = new Rectangle(coverImageRect.X, coverImageRect.Y + coverImageRect.Height - 2, coverImageRect.Width, 25);
                        Rectangle coverImageMirrorRectSrc = new Rectangle(coverImageRect.X, coverImageRect.Height - coverImageMirrorRectDest.Height, coverImageRect.Width, coverImageMirrorRectDest.Height);
                        System.Drawing.Image coverImageMirror = System.Drawing.Image.FromFile(coverImagePath);
                        coverImageMirror.RotateFlip(RotateFlipType.RotateNoneFlipY);

                        e.Graphics.DrawImage(coverImageMirror, coverImageMirrorRectDest, 0, 0, coverImageMirror.Width, coverImageMirror.Height, GraphicsUnit.Pixel, attributes);
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        
         }

        private void lblLoading_Paint(object sender, PaintEventArgs e)
        {
            SCTVObjects.Fonts fonts = new Fonts();

            SolidBrush b = new SolidBrush(Color.Black);

            e.Graphics.DrawString("Loading", fonts.CustomFont("CoffeeTin Initials", 32), b, 0, 0, StringFormat.GenericTypographic);

            b.Dispose();
        }   
    }
}