using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SCTV
{
  public partial class AboutForm : Form
  {
    public AboutForm()
    {
      InitializeComponent();
    }

    private void okButton_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}