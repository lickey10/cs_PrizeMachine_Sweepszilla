namespace SCTVObjects
{
    partial class InsertedMedia
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnPlayAndRecord = new System.Windows.Forms.Button();
            this.gbDVD = new System.Windows.Forms.GroupBox();
            this.chbSkipMenu = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.gbTitle = new System.Windows.Forms.GroupBox();
            this.gbDVD.SuspendLayout();
            this.gbTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(6, 19);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(75, 23);
            this.btnPlay.TabIndex = 0;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnPlayAndRecord
            // 
            this.btnPlayAndRecord.Location = new System.Drawing.Point(121, 19);
            this.btnPlayAndRecord.Name = "btnPlayAndRecord";
            this.btnPlayAndRecord.Size = new System.Drawing.Size(121, 23);
            this.btnPlayAndRecord.TabIndex = 1;
            this.btnPlayAndRecord.Text = "Play and Record";
            this.btnPlayAndRecord.UseVisualStyleBackColor = true;
            this.btnPlayAndRecord.Click += new System.EventHandler(this.btnPlayAndRecord_Click);
            // 
            // gbDVD
            // 
            this.gbDVD.Controls.Add(this.chbSkipMenu);
            this.gbDVD.Controls.Add(this.btnPlay);
            this.gbDVD.Controls.Add(this.btnPlayAndRecord);
            this.gbDVD.Location = new System.Drawing.Point(12, 59);
            this.gbDVD.Name = "gbDVD";
            this.gbDVD.Size = new System.Drawing.Size(248, 71);
            this.gbDVD.TabIndex = 3;
            this.gbDVD.TabStop = false;
            this.gbDVD.Text = "DVD";
            // 
            // chbSkipMenu
            // 
            this.chbSkipMenu.AutoSize = true;
            this.chbSkipMenu.Checked = true;
            this.chbSkipMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSkipMenu.Location = new System.Drawing.Point(86, 48);
            this.chbSkipMenu.Name = "chbSkipMenu";
            this.chbSkipMenu.Size = new System.Drawing.Size(77, 17);
            this.chbSkipMenu.TabIndex = 5;
            this.chbSkipMenu.Text = "Skip Menu";
            this.chbSkipMenu.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(98, 136);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(6, 19);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(236, 20);
            this.txtTitle.TabIndex = 5;
            // 
            // gbTitle
            // 
            this.gbTitle.Controls.Add(this.txtTitle);
            this.gbTitle.Location = new System.Drawing.Point(12, 0);
            this.gbTitle.Name = "gbTitle";
            this.gbTitle.Size = new System.Drawing.Size(248, 53);
            this.gbTitle.TabIndex = 6;
            this.gbTitle.TabStop = false;
            this.gbTitle.Text = "Title";
            // 
            // InsertedMedia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 171);
            this.Controls.Add(this.gbTitle);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbDVD);
            this.Name = "InsertedMedia";
            this.Text = "DVD";
            this.gbDVD.ResumeLayout(false);
            this.gbDVD.PerformLayout();
            this.gbTitle.ResumeLayout(false);
            this.gbTitle.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnPlayAndRecord;
        private System.Windows.Forms.GroupBox gbDVD;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chbSkipMenu;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.GroupBox gbTitle;
    }
}