namespace SCTV
{
    partial class SlidingBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlCover = new System.Windows.Forms.Panel();
            this.inactivityTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // pnlCover
            // 
            this.pnlCover.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCover.Location = new System.Drawing.Point(0, 0);
            this.pnlCover.Name = "pnlCover";
            this.pnlCover.Size = new System.Drawing.Size(1119, 116);
            this.pnlCover.TabIndex = 0;
            this.pnlCover.Visible = false;
            this.pnlCover.MouseLeave += new System.EventHandler(this.pnlCover_MouseLeave);
            this.pnlCover.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlCover_MouseMove);
            // 
            // inactivityTimer
            // 
            this.inactivityTimer.Interval = 1000;
            this.inactivityTimer.Tick += new System.EventHandler(this.inactivityTimer_Tick);
            // 
            // SlidingBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlCover);
            this.Name = "SlidingBar";
            this.Size = new System.Drawing.Size(1119, 116);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlCover;
        private System.Windows.Forms.Timer inactivityTimer;
    }
}
