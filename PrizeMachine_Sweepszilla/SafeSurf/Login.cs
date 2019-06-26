using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SCTV
{
    public partial class Login : Form
    {
        bool update = false;

        public bool Update
        {
            set 
            { 
                update = value;

                if (update)
                    btnLogin.Text = "Update";
            }
        }

        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!update)
            {
                string[] loginInfo = File.ReadAllLines(MainForm.loginInfoPath);

                if (loginInfo.Length != 2)
                {
                    this.DialogResult = DialogResult.None;
                    //TODO: log reason for admin lock
                }
                else if(!loginInfo[0].Contains("username:"))
                {
                    loginInfo = Encryption.Decrypt(loginInfo);
                }

                if (!loginInfo[0].Contains("username:"))
                {
                    this.DialogResult = DialogResult.None;
                    //TODO: log reason for admin lock
                }

                if (txtUserName.Text.Trim() == loginInfo[0].Replace("username:","") && txtPassword.Text.Trim() == loginInfo[1].Replace("password:",""))
                {
                    this.DialogResult = DialogResult.Yes;
                    lblError.Visible = false;
                    this.Close();
                }
                else
                {
                    //TODO: log invalid attempt   
                    this.DialogResult = DialogResult.No;
                    lblError.Visible = true;
                }
            }
            else
            {
                string[] loginInfo = {"username:"+ txtUserName.Text.Trim(), "password:"+ txtPassword.Text.Trim()};
                loginInfo = Encryption.Encrypt(loginInfo);
                File.WriteAllLines(MainForm.loginInfoPath, loginInfo);
                this.Close();
            }
        }

        private void Login_Activated(object sender, EventArgs e)
        {
            if(txtUserName.Text.Trim().Length == 0)
                txtUserName.Focus();
        }
    }
}