namespace SCTVControls.Volume
{
    partial class Volume
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Volume));
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.btnMute = new System.Windows.Forms.Button();
            this.btnVolumeUp = new System.Windows.Forms.Button();
            this.btnVolumeDown = new System.Windows.Forms.Button();
            this.tbVolume = new SCTVControls.Volume.MACTrackBar();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // btnMute
            // 
            this.btnMute.FlatAppearance.BorderSize = 0;
            this.btnMute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMute.Image = global::SCTV.Properties.Resources.unMuted_button;
            this.btnMute.Location = new System.Drawing.Point(32, 0);
            this.btnMute.Name = "btnMute";
            this.btnMute.Size = new System.Drawing.Size(26, 23);
            this.btnMute.TabIndex = 14;
            this.btnMute.UseVisualStyleBackColor = true;
            this.btnMute.Click += new System.EventHandler(this.btnMute_Click);
            // 
            // btnVolumeUp
            // 
            this.btnVolumeUp.FlatAppearance.BorderSize = 0;
            this.btnVolumeUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolumeUp.Image = global::SCTV.Properties.Resources.up_button;
            this.btnVolumeUp.Location = new System.Drawing.Point(64, 0);
            this.btnVolumeUp.Name = "btnVolumeUp";
            this.btnVolumeUp.Size = new System.Drawing.Size(30, 23);
            this.btnVolumeUp.TabIndex = 13;
            this.btnVolumeUp.UseVisualStyleBackColor = true;
            this.btnVolumeUp.Click += new System.EventHandler(this.btnVolumeUp_Click);
            // 
            // btnVolumeDown
            // 
            this.btnVolumeDown.FlatAppearance.BorderSize = 0;
            this.btnVolumeDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolumeDown.Image = ((System.Drawing.Image)(resources.GetObject("btnVolumeDown.Image")));
            this.btnVolumeDown.Location = new System.Drawing.Point(3, 0);
            this.btnVolumeDown.Name = "btnVolumeDown";
            this.btnVolumeDown.Size = new System.Drawing.Size(26, 23);
            this.btnVolumeDown.TabIndex = 12;
            this.btnVolumeDown.UseVisualStyleBackColor = true;
            this.btnVolumeDown.Click += new System.EventHandler(this.btnVolumeDown_Click);
            // 
            // tbVolume
            // 
            this.tbVolume.BackColor = System.Drawing.Color.Transparent;
            this.tbVolume.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.tbVolume.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbVolume.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(125)))), ((int)(((byte)(123)))));
            this.tbVolume.IndentHeight = 6;
            this.tbVolume.Location = new System.Drawing.Point(95, 0);
            this.tbVolume.Maximum = 100;
            this.tbVolume.Minimum = 0;
            this.tbVolume.Name = "tbVolume";
            this.tbVolume.Size = new System.Drawing.Size(117, 47);
            this.tbVolume.TabIndex = 16;
            this.tbVolume.TickColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(146)))), ((int)(((byte)(148)))));
            this.tbVolume.TickHeight = 4;
            this.tbVolume.TrackerColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(130)))), ((int)(((byte)(198)))));
            this.tbVolume.TrackerSize = new System.Drawing.Size(16, 16);
            this.tbVolume.TrackLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(93)))), ((int)(((byte)(90)))));
            this.tbVolume.TrackLineHeight = 3;
            this.tbVolume.Value = 0;
            this.tbVolume.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbVolume_MouseUp);
            // 
            // Volume
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tbVolume);
            this.Controls.Add(this.btnMute);
            this.Controls.Add(this.btnVolumeUp);
            this.Controls.Add(this.btnVolumeDown);
            this.Name = "Volume";
            this.Size = new System.Drawing.Size(213, 23);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMute;
        private System.Windows.Forms.Button btnVolumeUp;
        private System.Windows.Forms.Button btnVolumeDown;
        private System.Windows.Forms.Timer timer;
        private MACTrackBar tbVolume;
    }
}
