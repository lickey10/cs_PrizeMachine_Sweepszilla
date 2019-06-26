using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
//using SCTVDeviceVolumeMonitor;

namespace SCTVObjects
{
    public class DVDHandler
    {
        //public static DeviceVolumeMonitor deviceMonitor;

        public static void VolumeInserted(int aMask)
        {
            // -------------------------
            // A volume was inserted
            // -------------------------
            //MessageBox.Show("Volume inserted in " + deviceMonitor.MaskToLogicalPaths(aMask));
            //lbEvents.Items.Add("Volume inserted in " + fNative.MaskToLogicalPaths(aMask));

            bool skipDVDMenu = false;
            string discName;
            DriveInfo driveInfo;
            InsertedMedia insertedMedia = new InsertedMedia();
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["Media.SkipDVDMenu"], out skipDVDMenu);
            insertedMedia.SkipMenu = skipDVDMenu;
            insertedMedia.ShowDialog();
            //string driveLetter = deviceMonitor.MaskToLogicalPaths(aMask);

            //driveInfo = new DriveInfo(driveLetter);
            //discName = driveInfo.VolumeLabel;

            //discName = discName.Replace("<", "");
            //discName = discName.Replace(">", "");
            //discName = discName.Replace("[", "");
            //discName = discName.Replace("]", "");
            //discName = System.Text.RegularExpressions.Regex.Replace(discName, @"\W*", "");

            //switch (insertedMedia.MediaState)
            //{
            //    case MediaStateEnum.Play:
            //        playRemoveableMedia(driveLetter, insertedMedia.SkipMenu);
            //        break;
            //    case MediaStateEnum.PlayAndRecord:
            //        playRemoveableMedia(driveLetter, defaultPathToSaveTo + "\\" + discName + ".CEL", insertedMedia.SkipMenu);
            //        break;
            //    case MediaStateEnum.Record:
            //        recordRemoveableMedia(driveLetter, defaultPathToSaveTo + "\\" + discName + ".CEL", insertedMedia.SkipMenu);
            //        break;
            //}
        }

        public static void VolumeRemoved(int aMask)
        {
            // --------------------
            // A volume was removed
            // --------------------
            //MessageBox.Show("Volume removed from " + deviceMonitor.MaskToLogicalPaths(aMask));
            //lbEvents.Items.Add("Volume removed from " + fNative.MaskToLogicalPaths(aMask));
        }

        //private void playRemoveableMedia(string driveLetter, bool skipMenu)
        //{
        //    playRemoveableMedia(driveLetter, "", skipMenu);
        //}

        //private void playRemoveableMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        //{
        //    //make sure mediaplayer exists
        //    FormCollection openForms = Application.OpenForms;

        //    if (openForms["liquidMediaPlayer"] == null)
        //    {
        //        liquidMediaPlayer mediaPlayer = new liquidMediaPlayer();
        //        mediaPlayer.PlayRemoveableMedia(driveLetter, fileToRecordTo, skipMenu);

        //        mediaPlayer.ShowDialog();
        //    }
        //    else
        //        openForms["liquidMediaPlayer"].ShowDialog();


        //}

        //private void recordRemoveableMedia(string driveLetter, string fileToRecordTo, bool skipMenu)
        //{
        //    //make sure mediaplayer exists
        //    FormCollection openForms = Application.OpenForms;

        //    if (openForms["liquidMediaPlayer"] == null)
        //    {
        //        liquidMediaPlayer mediaPlayer = new liquidMediaPlayer();
        //        mediaPlayer.RecordRemoveableMedia(driveLetter, fileToRecordTo, skipMenu);

        //        mediaPlayer.ShowDialog();
        //    }
        //    else
        //        openForms["liquidMediaPlayer"].ShowDialog();
        //}
    }
}
