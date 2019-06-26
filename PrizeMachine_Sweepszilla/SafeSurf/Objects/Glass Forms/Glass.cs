using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCTVObjects
{
    partial class Glass : Form
    {
        ControlHolder ch;
        bool flag = false;
        int xOffset = 0;
        int yOffset = 0;
        Form parentForm = null;
        int titleBarSize = 0;
        string backgroundImagePath = "images\\glassforms\\blue_big.gif";
        string defaultBackgroundImagePath = "images\\glassforms\\blue_big.gif";
        private Rectangle activeArea;
        private double glassOpacity = 0.5;
        bool keepWindowVisible = true;
        bool displayMinimize = true;
        bool displayMaximize = true;
        bool displayClose = true;
        bool parentActivated = false;

        public delegate void FormClose();
        public event FormClose closeForm;

        public delegate void FormMinimize();
        public event FormMinimize formMinimizing;

        public delegate void FormMaximize();
        public event FormMaximize formMaximizing;

        public Form ParentForm
        {
            set { parentForm = value; }
        }

        public int TitleBarSize
        {
            set { titleBarSize = value; }
            get { return titleBarSize; }
        }

        public string BackgroundImagePath
        {
            set { backgroundImagePath = value; }
            get { return backgroundImagePath; }
        }

        public Rectangle ActiveArea
        {
            set { activeArea = value; }
        }

        public bool KeepWindowVisible
        {
            set { keepWindowVisible = value; }
            get { return keepWindowVisible; }
        }

        public double GlassOpacity
        {
            set 
            { 
                glassOpacity = value;

                this.Opacity = glassOpacity;
            }
            get { return glassOpacity; }
        }

        public bool DisplayMinimize
        {
            set { displayMinimize = value; }
        }

        public bool DisplayMaximize
        {
            set { displayMaximize = value; }
        }

        public bool DisplayClose
        {
            set { displayClose = value; }
        }

        public Glass()
        {
            InitializeComponent();
        }

        private void Glass_Load(object sender, EventArgs e)
        {
            try
            {
                if (System.IO.File.Exists(backgroundImagePath))
                    this.BackgroundImage = (Bitmap)Bitmap.FromFile(backgroundImagePath);
                else
                    this.BackgroundImage = (Bitmap)Bitmap.FromFile(defaultBackgroundImagePath);

                this.Opacity = glassOpacity;
                this.TransparencyKey = System.Drawing.SystemColors.Control;
                
                //ch = new ControlHolder();
                //ch.DisplayClose = this.displayClose;
                //ch.DisplayMaximize = this.displayMaximize;
                //ch.DisplayMinimize = this.displayMinimize;
                //ch.Parent = this.Parent;
                //ch.closeForm += new ControlHolder.FormClose(ch_closeForm);
                //ch.formMaximizing += new ControlHolder.FormMaximize(ch_FormMaximizing);
                //ch.formMinimizing += new ControlHolder.FormMinimize(ch_formMinimizing);

                this.DesktopBounds = new Rectangle(parentForm.DesktopBounds.X - 20, parentForm.DesktopBounds.Y - 20, parentForm.Width + 40, parentForm.Height + 40);

                if (activeArea == null || activeArea.Width == 0 || activeArea.Height == 0)
                    activeArea = parentForm.DesktopBounds;
                    //activeArea = new Rectangle(20, 20, this.Width - 40, this.Height - 40);

                //updateOtherFormPositions();

                //ch.Show();

                parentForm.FormClosing += new FormClosingEventHandler(parentForm_FormClosing);
                parentForm.Activated += new EventHandler(parentForm_Activated);
                
                if (keepWindowVisible)
                    keepWindowOnScreen();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        void parentForm_Activated(object sender, EventArgs e)
        {
            //setZOrder();
        }

        private void setZOrder()
        {
            if (!parentActivated)
            {
                //this.TopMost = true;
                //ch.TopMost = true;
                //parentForm.TopMost = true;

                this.BringToFront();
                parentForm.BringToFront();

                parentActivated = true;
                parentForm.Activate();
            }
            else
                parentActivated = false;
        }

        void parentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //ch.Close();

            this.Close();
        }

        void ch_closeForm()
        {
            this.closeForm();

            this.Close();
        }

        void ch_formMinimizing()
        {
            this.formMinimizing();

            this.WindowState = FormWindowState.Minimized;
        }

        void ch_FormMaximizing()
        {
            this.formMaximizing();

            this.WindowState = FormWindowState.Maximized;
        }

        private void updateOtherFormPositions()
        {
            //ch.DesktopBounds = this.DesktopBounds;
            
            //position the glasswindow
            parentForm.DesktopBounds = new Rectangle(this.DesktopLocation.X + activeArea.X, this.DesktopLocation.Y + activeArea.Y + titleBarSize, activeArea.Width, activeArea.Height - titleBarSize);
            parentForm.BringToFront();//keeps the form on top of the glass
        }

        private void Glass_MouseUp(object sender, MouseEventArgs e)
        {
            flag = false;

            keepWindowOnScreen();
        }

        private void Glass_MouseDown(object sender, MouseEventArgs e)
        {
            flag = true;

            xOffset = Cursor.Position.X - this.Location.X;
            yOffset = Cursor.Position.Y - this.Location.Y;
        }

        private void Glass_MouseMove(object sender, MouseEventArgs e)
        {
            //Check if Flag is True ??? if so then make form position same

            if (flag == true)
            {
                //position the glass
                this.Location = new Point(Cursor.Position.X - xOffset, Cursor.Position.Y - yOffset);

                updateOtherFormPositions();
            }
        }

        private void keepWindowOnScreen()
        {
            bool windowMoved = false;

            if (this.Left < 0)
            {
                this.Left = 0;

                windowMoved = true;
            }
            else if (this.Right > Screen.PrimaryScreen.Bounds.Width)
            {
                this.Left = Screen.PrimaryScreen.Bounds.Width - this.Width;

                windowMoved = true;
            }

            if (this.Top < 0)
            {
                this.Top = 0;

                windowMoved = true;
            }
            else if (this.Bottom > Screen.PrimaryScreen.Bounds.Height)
            {
                this.Top = Screen.PrimaryScreen.Bounds.Height - this.Height;

                windowMoved = true;
            }

            //if(windowMoved)
            //    updateOtherFormPositions();
        }
    }
}
