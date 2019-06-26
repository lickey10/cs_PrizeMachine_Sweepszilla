using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SCTVObjects
{
    public enum MediaStateEnum
    {
        Cancel, Play, PlayAndRecord, Record
    }

    public partial class InsertedMedia : GlassWindow
    {
        DriveInfo drive;
        string mediaName = "";
        private MediaStateEnum mediaState;

        public InsertedMedia()
        {
            InitializeComponent(); 
        }

        public DriveInfo Drive
        {
            get { return drive; }
            set 
            { 
                drive = value;

                if(mediaName.Trim().Length == 0)
                    MediaName = drive.VolumeLabel;
            }
        }

        public string MediaName
        {
            get { return mediaName; }
            set 
            { 
                mediaName = value;

                //discName = discName.Replace("<", "");
                //discName = discName.Replace(">", "");
                //discName = discName.Replace("[", "");
                //discName = discName.Replace("]", "");
                mediaName = System.Text.RegularExpressions.Regex.Replace(mediaName, @"\W*", "");

                this.Text = mediaName;
                txtTitle.Text = mediaName;
            }
        }

        public MediaStateEnum MediaState
        {
            get{return mediaState;}
        }

        public bool SkipMenu
        {
            get { return chbSkipMenu.Checked; }
            set { chbSkipMenu.Checked = value; }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            mediaState = MediaStateEnum.Play;
            this.Close();
        }

        private void btnPlayAndRecord_Click(object sender, EventArgs e)
        {
            mediaState = MediaStateEnum.PlayAndRecord;
            this.Close();
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            //mediaState = MediaStateEnum.Record;
            mediaState = MediaStateEnum.Cancel;
            MessageBox.Show("Coming Soon!!");
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mediaState = MediaStateEnum.Cancel;
            this.Close();
        }
    }
}