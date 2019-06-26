using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SCTV
{
	/// <summary>
	/// Summary description for splashScreen.
	/// </summary>
	public class splashScreen : System.Windows.Forms.Form
    {
        private IContainer components;
		private System.Windows.Forms.Label lblSCTV;
		private System.Windows.Forms.Label lblLoading;
		protected Rectangle WorkAreaRectangle;
		public string splashContent1="";
        private ProgressBar pbLoading;
        private Timer loadingTimer;
        public string splashContent2="";

        public string SplashMessage1
        {
            get { return lblSCTV.Text; }
            set { lblSCTV.Text = value; }
        }

        public string SplashMessage2
        {
            get { return lblLoading.Text; }
            set { lblLoading.Text = value; }
        }

		public splashScreen()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            loadingTimer.Enabled = true;
            loadingTimer.Start();
		}

        public splashScreen(Media MediaInfoToShow)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            loadingTimer.Enabled = true;
            loadingTimer.Start();
        }

        public splashScreen(string splashMessage1, string splashMessage2)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //this.ParentForm.Invalidate();

            SplashMessage1 = splashMessage1;
            SplashMessage2 = splashMessage2;

            if (splashMessage2 == "Loading")
                loadingTimer.Start();
            else
                pbLoading.Visible = false;
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.lblSCTV = new System.Windows.Forms.Label();
            this.lblLoading = new System.Windows.Forms.Label();
            this.pbLoading = new System.Windows.Forms.ProgressBar();
            this.loadingTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lblSCTV
            // 
            this.lblSCTV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSCTV.Font = new System.Drawing.Font("Castellar", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSCTV.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.lblSCTV.Location = new System.Drawing.Point(72, 80);
            this.lblSCTV.Name = "lblSCTV";
            this.lblSCTV.Size = new System.Drawing.Size(512, 168);
            this.lblSCTV.TabIndex = 0;
            this.lblSCTV.Text = "SCTV";
            this.lblSCTV.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLoading
            // 
            this.lblLoading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLoading.Font = new System.Drawing.Font("Perpetua", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoading.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblLoading.Location = new System.Drawing.Point(192, 256);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(240, 64);
            this.lblLoading.TabIndex = 1;
            this.lblLoading.Text = "Loading";
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbLoading
            // 
            this.pbLoading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbLoading.ForeColor = System.Drawing.Color.LimeGreen;
            this.pbLoading.Location = new System.Drawing.Point(364, 280);
            this.pbLoading.Name = "pbLoading";
            this.pbLoading.Size = new System.Drawing.Size(68, 23);
            this.pbLoading.TabIndex = 2;
            // 
            // loadingTimer
            // 
            this.loadingTimer.Enabled = true;
            this.loadingTimer.Interval = 2000;
            this.loadingTimer.Tick += new System.EventHandler(this.loadingTimer_Tick);
            // 
            // splashScreen
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(648, 424);
            this.Controls.Add(this.pbLoading);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.lblSCTV);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "splashScreen";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "splashScreen";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.splashScreen_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.splashScreen_Paint);
            this.VisibleChanged += new System.EventHandler(this.splashScreen_VisibleChanged);
            this.ResumeLayout(false);

		}
		#endregion

		private void splashScreen_VisibleChanged(object sender, System.EventArgs e)
		{
			if(this.Visible)
			{
                Cursor.Position = new Point(this.Width, Cursor.Position.Y);
                //Cursor.Hide();
                lblSCTV.Refresh();
                lblLoading.Refresh();
                this.Focus();
                this.BringToFront();
			}
			else
			{
				Cursor.Show();
			}
		}

		private void splashScreen_Load(object sender, System.EventArgs e)
		{
            //if(splashContent.Length>0)
            //{
            //    lblSCTV.Text = splashContent;
            //}

			WorkAreaRectangle = Screen.GetWorkingArea(WorkAreaRectangle);
			lblSCTV.Top = (WorkAreaRectangle.Height-175)/2;
			lblSCTV.Left = (WorkAreaRectangle.Width-lblSCTV.Width)/2;
			lblSCTV.Refresh();
			lblLoading.Top = (lblSCTV.Top + lblSCTV.Height + 10);
			lblLoading.Left = (WorkAreaRectangle.Width-lblLoading.Width)/2;
			lblLoading.Refresh();
		}

        private void loadingTimer_Tick(object sender, EventArgs e)
        {
            if (pbLoading.Value >= pbLoading.Maximum)
                pbLoading.Value = 0;

            pbLoading.Value += 10;
        }

        private void splashScreen_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush bgBrush = null;

            Rectangle rectTL = new Rectangle(0, 0, e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
            bgBrush = new LinearGradientBrush(rectTL, Color.AntiqueWhite, Color.SteelBlue, LinearGradientMode.ForwardDiagonal);
            fillRectangle(rectTL,e, bgBrush);

            Rectangle rectTR = new Rectangle(e.ClipRectangle.Width / 2, 0, e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
            bgBrush = new LinearGradientBrush(rectTR, Color.AntiqueWhite, Color.SteelBlue, LinearGradientMode.BackwardDiagonal);
            fillRectangle(rectTR, e, bgBrush);

            Rectangle rectBL = new Rectangle(0, e.ClipRectangle.Height / 2, e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
            bgBrush = new LinearGradientBrush(rectBL, Color.SteelBlue,  Color.AntiqueWhite,LinearGradientMode.BackwardDiagonal);
            fillRectangle(rectBL, e, bgBrush);

            Rectangle rectBR = new Rectangle(e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2, e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
            bgBrush = new LinearGradientBrush(rectBR, Color.SteelBlue,Color.AntiqueWhite,  LinearGradientMode.ForwardDiagonal);
            fillRectangle(rectBR, e, bgBrush);
        }

        private void fillRectangle(Rectangle rectToDraw, PaintEventArgs e, LinearGradientBrush bgBrush)
        {
            if (rectToDraw.Width > 0 && rectToDraw.Height > 0)
            {
                using (bgBrush)
                    e.Graphics.FillRectangle(bgBrush, rectToDraw);
            }
        }
	}
}
