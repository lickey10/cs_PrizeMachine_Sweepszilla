using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCTVObjects
{
    partial class ControlHolder : Form
    {
        Rectangle displayedRect;
        
        public delegate void FormClose();
        public event FormClose closeForm;

        public delegate void FormMinimize();
        public event FormMinimize formMinimizing;

        public delegate void FormMaximize();
        public event FormMaximize formMaximizing;

        public Rectangle DisplayRect
        {
            set { displayedRect = value; }
        }

        public bool DisplayMinimize
        {
            set { btnMinimize.Visible = value; }
        }

        public bool DisplayMaximize
        {
            set { btnMaximize.Visible = value; }
        }

        public bool DisplayClose
        {
            set { btnClose.Visible = value; }
        }

        public ControlHolder()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            closeForm();

            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            formMinimizing();

            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            formMaximizing();

            this.WindowState = FormWindowState.Maximized;
        }

        private void ControlHolder_Load(object sender, EventArgs e)
        {
            

        }
    }
}
