using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

namespace SCTV
{
	/// <summary>
	/// Summary description for splashScreen.
	/// </summary>
	public class SplashScreen2 : System.Windows.Forms.Form
    {
        private IContainer components;
		protected Rectangle WorkAreaRectangle;
		public string splashContent1="";
        private ProgressBar pbLoading;
        private Timer loadingTimer;
        private PictureBox pictureBox1;
        public string splashContent2="";
        Media mediaInfoToShow;

        public SplashScreen2()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            loadingTimer.Enabled = true;
            loadingTimer.Start();
		}

        public SplashScreen2(Media MediaInfoToShow)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            loadingTimer.Enabled = true;
            loadingTimer.Start();
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
            this.pbLoading = new System.Windows.Forms.ProgressBar();
            this.loadingTimer = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pbLoading
            // 
            this.pbLoading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbLoading.ForeColor = System.Drawing.Color.LimeGreen;
            this.pbLoading.Location = new System.Drawing.Point(290, 361);
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
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBox1.Location = new System.Drawing.Point(78, 75);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(493, 259);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // SplashScreen2
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(648, 424);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pbLoading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SplashScreen2";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "splashScreen";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.splashScreen_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.splashScreen_Paint);
            this.VisibleChanged += new System.EventHandler(this.splashScreen_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void splashScreen_VisibleChanged(object sender, System.EventArgs e)
		{
			if(this.Visible)
			{
                Cursor.Position = new Point(this.Width, Cursor.Position.Y);
                //Cursor.Hide();
                //lblSCTV.Refresh();
                //lblLoading.Refresh();
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
            WorkAreaRectangle = Screen.GetWorkingArea(WorkAreaRectangle);
            //lblSCTV.Top = (WorkAreaRectangle.Height-175)/2;
            //lblSCTV.Left = (WorkAreaRectangle.Width-lblSCTV.Width)/2;
			
            this.Invalidate();
		}

        private void loadingTimer_Tick(object sender, EventArgs e)
        {
            if (pbLoading.Value >= pbLoading.Maximum)
                pbLoading.Value = 0;

            pbLoading.Value += 10;
        }

        private void splashScreen_Paint(object sender, PaintEventArgs e)
        {
            if (e.ClipRectangle.Width > 0 && e.ClipRectangle.Height > 0)
            {
                LinearGradientBrush bgBrush = null;

                Rectangle rectTL = new Rectangle(0, 0, e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
                bgBrush = new LinearGradientBrush(rectTL, Color.AntiqueWhite, Color.SteelBlue, LinearGradientMode.ForwardDiagonal);
                fillRectangle(rectTL, e, bgBrush);

                Rectangle rectTR = new Rectangle(e.ClipRectangle.Width / 2, 0, e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
                bgBrush = new LinearGradientBrush(rectTR, Color.AntiqueWhite, Color.SteelBlue, LinearGradientMode.BackwardDiagonal);
                fillRectangle(rectTR, e, bgBrush);

                Rectangle rectBL = new Rectangle(0, e.ClipRectangle.Height / 2, e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
                bgBrush = new LinearGradientBrush(rectBL, Color.SteelBlue, Color.AntiqueWhite, LinearGradientMode.BackwardDiagonal);
                fillRectangle(rectBL, e, bgBrush);

                Rectangle rectBR = new Rectangle(e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2, e.ClipRectangle.Width / 2, e.ClipRectangle.Height / 2);
                bgBrush = new LinearGradientBrush(rectBR, Color.SteelBlue, Color.AntiqueWhite, LinearGradientMode.ForwardDiagonal);
                fillRectangle(rectBR, e, bgBrush);
            }
        }

        private void fillRectangle(Rectangle rectToDraw, PaintEventArgs e, LinearGradientBrush bgBrush)
        {
            if (rectToDraw.Width > 0 && rectToDraw.Height > 0)
            {
                using (bgBrush)
                    e.Graphics.FillRectangle(bgBrush, rectToDraw);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (mediaInfoToShow != null)
            {
                //int textHeight = (int)(e.Graphics.MeasureString("WWW", lvFF.Font).Height + 4);
                Brush myBrush = Brushes.Black;
                Rectangle rectBottom;
                Rectangle rectBackground;
                Rectangle coverImageRect = new Rectangle(e.ClipRectangle.X + 10, e.ClipRectangle.Y, 87, 140);
                string coverImagePath = "";

                LinearGradientBrush bgBrush = null;

                rectBackground = new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height - 35);
                rectBottom = new Rectangle(e.ClipRectangle.X, rectBackground.Location.Y + rectBackground.Height - 2, e.ClipRectangle.Width, e.ClipRectangle.Height - rectBackground.Height);

                bgBrush = new LinearGradientBrush(rectBackground, Color.Transparent, Color.SlateGray, LinearGradientMode.Vertical);

                if (bgBrush != null)
                {
                    using (bgBrush)
                        e.Graphics.FillRectangle(bgBrush, rectBackground);
                }

                bgBrush = new LinearGradientBrush(rectBottom, Color.SlateGray, Color.Transparent, LinearGradientMode.Vertical);

                if (bgBrush != null)
                {
                    using (bgBrush)
                        e.Graphics.FillRectangle(bgBrush, rectBottom);
                }

                //if (e.Item.Tag is Media)
                //{
                //    coverImagePath = ((Media)e.Item.Tag).coverImage;
                //}
                //else if (e.Item.Tag is OnlineMediaType)
                //coverImagePath = ((OnlineMediaType)e.Item.Tag).CoverImage;

                coverImagePath = mediaInfoToShow.coverImage;

                if (!File.Exists(coverImagePath))
                    coverImagePath = Application.StartupPath + "\\images\\notavailable.jpg";

                System.Drawing.Image coverImage = System.Drawing.Image.FromFile(coverImagePath);

                //draw black outline
                using (Graphics g = Graphics.FromImage(coverImage))
                    g.DrawRectangle(Pens.Black, 0, 0, coverImage.Width - 1, coverImage.Height - 1);

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

                //e.Graphics.DrawString(mediaInfoToShow, this.Font, myBrush, rec, StringFormat.GenericDefault);
            }
            else
                pictureBox1.Visible = false;
        }
	}
}
