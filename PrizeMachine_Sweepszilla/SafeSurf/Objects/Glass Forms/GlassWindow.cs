using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System;

namespace SCTVObjects
{
    public partial class GlassWindow : Form
    {
        string title = "";
        Glass glass;
        int titleBarSize = 20;
        string backgroundImagePath = "";
        Rectangle activeArea;
        double glassOpacity = 0.5;
        bool keepWindowVisible = true;
        bool displayMinimize = true;
        bool displayMaximize = true;
        bool displayClose = true;
        
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string BackgroundImagePath
        {
            get { return backgroundImagePath; }
            set { backgroundImagePath = value; }
        }

        public int TitleBarSize
        {
            set { titleBarSize = value; }
            get { return titleBarSize; }
        }

        public Rectangle ActiveArea
        {
            set { activeArea = value; }
        }

        public double GlassOpacity
        {
            set { glassOpacity = value; }
            get { return glassOpacity; }
        }

        public bool KeepWindowVisible
        {
            set { keepWindowVisible = value; }
            get { return keepWindowVisible; }
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

        public GlassWindow()
        {
            InitializeComponent();
        }

        private void GlassWindow_Load(object sender, EventArgs e)
        {
            try
            {
                glass = new Glass();
                glass.ParentForm = this;
                glass.Parent = this.Parent;
                glass.TitleBarSize = titleBarSize;
                glass.ActiveArea = activeArea;
                glass.GlassOpacity = glassOpacity;
                glass.KeepWindowVisible = keepWindowVisible;
                glass.BackgroundImagePath = backgroundImagePath;
                glass.DisplayClose = displayClose;
                glass.DisplayMaximize = displayMaximize;
                glass.DisplayMinimize = displayMinimize;
                glass.closeForm += new Glass.FormClose(glass_closeForm);
                glass.formMaximizing += new Glass.FormMaximize(glass_formMaximizing);
                glass.formMinimizing += new Glass.FormMinimize(glass_formMinimizing);

                glass.Show();
                
                this.TransparencyKey = System.Drawing.SystemColors.Control;
                
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }            
        }

        void glass_formMinimizing()
        {
            this.WindowState = FormWindowState.Minimized;
        }

        void glass_formMaximizing()
        {
            this.WindowState = FormWindowState.Maximized;
        }

        void glass_closeForm()
        {
            this.Close();
        }

        private void GlassWindow_Activated(object sender, EventArgs e)
        {
            //glass.Focus();
        }
    }
}