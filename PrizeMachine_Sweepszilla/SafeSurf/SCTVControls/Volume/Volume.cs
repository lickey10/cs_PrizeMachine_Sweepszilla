using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SCTV;
using SCTVObjects;

namespace SCTVControls.Volume
{
    public partial class Volume : UserControl
    {
        private Speakers speakers;
        int currentVolume = 0;

        public Volume()
        {
            InitializeComponent();

            speakers = new Speakers();

            tbVolume.Value = speakers.GetVolume();

            timer.Start();
        }

        //protected override void OnMouseWheel(MouseEventArgs e) 
        //{ 
        //    //int intValueBeforeScroll = this.Value; 
        //    base.OnMouseWheel(e); 
        //    //this.Value = intValueBeforeScroll; 
        //}

        //check volume and update UI
        private void timer_Tick(object sender, EventArgs e)
        {
            //set volume
            //tbVolume.Value = speakers.GetVolume();

            if (speakers.MuteStatus == 1)
            {
                this.btnMute.Image = global::SCTV.Properties.Resources.muted_button;
            }
            else
            {
                this.btnMute.Image = global::SCTV.Properties.Resources.unMuted_button;
            }
        }

        private void btnVolumeUp_Click(object sender, EventArgs e)
        {
            speakers.Volume("volume up");

            tbVolume.Value = speakers.GetVolume();
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            speakers.Volume("mute");

            if (speakers.MuteStatus == 1)
            {
                this.btnMute.Image = global::SCTV.Properties.Resources.muted_button;
            }
            else
            {
                this.btnMute.Image = global::SCTV.Properties.Resources.unMuted_button;
            }

            tbVolume.Value = 0;
        }

        private void btnVolumeDown_Click(object sender, EventArgs e)
        {
            speakers.Volume("volume down");

            tbVolume.Value = speakers.GetVolume();
        }

        private void tbVolume_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                double percentage = (double)e.X / (double)tbVolume.Width;

                speakers.Volume(percentage);

                currentVolume = (int)percentage;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "tbVolume_MouseUp: " + ex.Message);
            }
        }
    }
}
