using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCTVObjects
{
    public partial class internetConnectionDown : Form
    {
        public internetConnectionDown()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(chbDontTellAgain.Checked)
                this.DialogResult = DialogResult.Ignore;
            else
                this.DialogResult = DialogResult.Retry;

            this.Close();
        }
    }
}