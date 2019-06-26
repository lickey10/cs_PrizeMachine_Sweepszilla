//
// Bill SerGio
// A simple quick and dirty C# .NET Interface to the VLC (VideoLan) Media Player
// For the best C# .NET Interface that is FREE go to:
// http://wiki.videolan.org/.Net_Interface_to_VLC
//

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;
using Microsoft.Win32;


namespace SCTVObjects
{
    public class VLC
    {
        public enum Error
        {
            Success = -0,
            NoMem = -1,
            Thread = -2,
            Timeout = -3,
            NoMod = -10,
            NoObj = -20,
            BadObj = -21,
            NoVar = -30,
            BadVar = -31,
            Exit = -255,
            Generic = -666
        };

        enum Mode
        {
            Insert = 0x01,
            Replace = 0x02,
            Append = 0x04,
            Go = 0x08,
            CheckInsert = 0x10
        };

        enum Pos
        {
            End = -666
        };

        [DllImport("libvlc")]
        static extern int VLC_Create();
        [DllImport("libvlc")]
        static extern Error VLC_Init(int iVLC, int Argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)]string[] Argv);
        [DllImport("libvlc")]
        static extern Error VLC_AddIntf(int iVLC, string Name, bool Block, bool Play);
        [DllImport("libvlc")]
        static extern Error VLC_Die(int iVLC);
        [DllImport("libvlc")]
        static extern string VLC_Error();
        [DllImport("libvlc")]
        static extern string VLC_Version();
        [DllImport("libvlc")]
        static extern Error VLC_CleanUp(int iVLC);
        [DllImport("libvlc")]
        static extern Error VLC_Destroy(int iVLC);
        [DllImport("libvlc")]
        static extern Error VLC_AddTarget(int iVLC, string Target, string[] Options, int OptionsCount, int Mode, int Pos);
        [DllImport("libvlc")]
        static extern Error VLC_Play(int iVLC);
        [DllImport("libvlc")]
        static extern Error VLC_Pause(int iVLC);
        [DllImport("libvlc")]
        static extern Error VLC_Stop(int iVLC);
        [DllImport("libvlc")]
        static extern bool VLC_IsPlaying(int iVLC);
        [DllImport("libvlc")]
        static extern float VLC_PositionGet(int iVLC);
        [DllImport("libvlc")]
        static extern float VLC_PositionSet(int iVLC, float Pos);
        [DllImport("libvlc")]
        static extern int VLC_TimeGet(int iVLC);
        [DllImport("libvlc")]
        static extern Error VLC_TimeSet(int iVLC, int Seconds, bool Relative);
        [DllImport("libvlc")]
        static extern int VLC_LengthGet(int iVLC);
        [DllImport("libvlc")]
        static extern float VLC_SpeedFaster(int iVLC);
        [DllImport("libvlc")]
        static extern float VLC_SpeedSlower(int iVLC);
        [DllImport("libvlc")]
        static extern int VLC_PlaylistIndex(int iVLC);
        [DllImport("libvlc")]
        static extern int VLC_PlaylistNumberOfItems(int iVLC);
        [DllImport("libvlc")]
        static extern Error VLC_PlaylistNext(int iVLC);
        [DllImport("libvlc")]
        static extern Error VLC_PlaylistPrev(int iVLC);
        [DllImport("libvlc")]
        static extern Error VLC_PlaylistClear(int iVLC);
        [DllImport("libvlc")]
        static extern int VLC_VolumeSet(int iVLC, int Volume);
        [DllImport("libvlc")]
        static extern int VLC_VolumeGet(int iVLC);
        [DllImport("libvlc")]
        static extern Error VLC_VolumeMute(int iVLC);
        [DllImport("libvlc")]
        static extern Error VLC_FullScreen(int iVLC);
        [DllImport("libvlc")]
        static extern int VLC_VariableGet(int iVLC, string Name, int Value);
        [DllImport("libvlc")]
        static extern int VLC_VariableSet(int iVLC, string Name, int Value);
        [DllImport("libvlc")]
        static extern void __config_PutPsz(int iVLC, String name, String value);

        private int iVLC = -1;
        private static string vlcInstallDirectory = "";
        private string mediaType = "";

        public int getVariable(string Name)
        {
            return VLC_VariableGet(iVLC, Name, -1);
        }

        public int setVariable(string Name, int Value)
        {
            return VLC_VariableSet(iVLC, Name, Value);
        }

        public bool playing
        {
            get { return VLC_IsPlaying(iVLC); }
        }

        public string GetError()
        {
            return VLC_Error();
        }

        public int GetLengthSecs()
        {
            return VLC_LengthGet(iVLC);
        }

        public int GetPositionSecs()
        {
            return VLC_TimeGet(iVLC);
        }

        public int GetTime()
        {
            return VLC_TimeGet(iVLC);
        }

        public float GetPosition()
        {
            return VLC_PositionGet(iVLC);
        }

        public void SetPositionSecs(int newPosition)
        {
            VLC_TimeSet(iVLC, newPosition, false);
        }

        public void SetVolume(int newVolume)
        {
            VLC_VolumeSet(iVLC, newVolume);
        }

        public VLC()
        {
            playMedia(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Substring(6));
        }

        public VLC(string newMediaType)
        {
            mediaType = newMediaType;
            playMedia(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Substring(6));
        }

        //public VLC(string vlcDirectory)
        //{
        //    playMedia(vlcDirectory);
        //}

        ~VLC()
        {
            try
            {
                VLC_CleanUp(iVLC);
                VLC_Destroy(iVLC);
            }
            catch { }
        }

        private void playMedia(string vlcDirectory)
        {
            vlcInstallDirectory = vlcDirectory;

            if (iVLC != -1)
                return;

            try
            {
                this.iVLC = VLC_Create();
                if (this.iVLC < 0)
                    throw new Exception("Failed to create VLC instance");

                //                EXIT STATUS
                //0 is success 
                //Non-zero means an error occurred, which is printed in stderr. 

                //The following error codes indicate the nature of the error: 
                //1 Error while parsing the arguments. 
                //2 Initializing the encoder failed(either vorbis or theora). 
                //3 Could not open/configure sound card. 
                //4 Xdamage extension not present. 
                //5 Shared memory extension not present. 
                //6 Xfixes extension not present. 
                //7 XInitThreads failed. 
                //8 No $DISPLAY environment variable and none specified as argument. 
                //9 Cannot connect to Xserver. 
                //10 Color depth is not 32, 24 or 16bpp. 
                //11 Improper window specification. 
                //12 Cannot attach shared memory to proccess. 
                //13 Cannot open file for writting. 
                //14 Cannot load the Jack library (dlopen/dlsym error on libjack.so). 
                //15 Cannot create new client. 
                //16 Cannot activate client. 
                //17 Port registration/connection failure. 


                //--sout-transcode-vb <integer>
                //                 Video bitrate Target bitrate of the transcoded video stream.



                //            --no-cursor 

                //Disable drawing of the cursor. 



                //Encoding Options: 

                //--on-the-fly-encoding 

                //Encode the audio-video data, while recording. 


                //-v_quality n 

                //A number from 0 to 63 for desired encoded video quality(default 63). 


                //-v_bitrate n 

                //A number from 45000 to 2000000 for desired encoded video bitrate(default 45000). 


                //-s_quality n 

                //Desired audio quality(-1 to 10). 

                //check for required folders
                if(!Directory.Exists(vlcInstallDirectory + @"\plugins"))
                    MessageBox.Show("Required folder \"plugins\" is missing");
                else if (!Directory.Exists(vlcInstallDirectory + @"\snapshots"))
                    MessageBox.Show("Required folder \"snapshots\" is missing");
                else
                {
                    if (mediaType.ToLower() == "music")
                    {
                        string[] initOptionsMusic = {vlcInstallDirectory,
                                           "--no-one-instance",
                                           "--drop-late-frames",
                                           "--disable-screensaver",
                                           "--overlay",
                                           "dummy",
                                           "--fast-mutex",
                                           "--win9x-cv-method=1",
                                           "--rtsp-caching=1200",
                                           "--plugin-path=" + vlcInstallDirectory + @"\plugins",
                                           "--snapshot-path=" + vlcInstallDirectory + @"\snapshots",
                                           "--snapshot-prefix=snap_",
                                           "--key-snapshot=S",
                                           "--snapshot-format=png",
                                           "--audio-visual=visualizer",
                                           "--effect-list=spectrometer",
                                           //"--sub-filter=marq",
                                           //"--marq-marquee=Chad Lickey",
                                           //"--marq-opacity=0",

                    //string[] initOptions = {vlcInstallDirectory,
                    //                       "--no-one-instance",
                    //                       "--drop-late-frames",
                    //                       "--disable-screensaver",
                    //                       "--overlay",
                    //                       "--rtsp-caching=1200",
                    //                       "--plugin-path=" + vlcInstallDirectory + @"\plugins",
                    //                       "--snapshot-path=" + vlcInstallDirectory + @"\snapshots",
                    //                       "--snapshot-prefix=snap_",
                    //                       "--key-snapshot=S",
                    //                       "--snapshot-format=png",
				};

                        Error err = VLC_Init(iVLC, initOptionsMusic.Length, initOptionsMusic);
                        if (err != Error.Success)
                        {
                            VLC_Destroy(iVLC);
                            this.iVLC = -1;
                        }
                    }
                    else
                    {
                        string[] initOptions = {vlcInstallDirectory,
                                           "--no-one-instance",
                                           "--drop-late-frames",
                                           "--disable-screensaver",
                                           "--overlay",
                                           "dummy",
                                           "--fast-mutex",
                                           "--win9x-cv-method=1",
                                           "--rtsp-caching=1200",
                                           "--plugin-path=" + vlcInstallDirectory + @"\plugins",
                                           "--snapshot-path=" + vlcInstallDirectory + @"\snapshots",
                                           "--snapshot-prefix=snap_",
                                           "--key-snapshot=S",
                                           "--snapshot-format=png",
                                           //"--sub-filter=marq",
                                           //"--marq-marquee=Chad Lickey",
                                           //"--marq-opacity=0",

                    //string[] initOptions = {vlcInstallDirectory,
                    //                       "--no-one-instance",
                    //                       "--drop-late-frames",
                    //                       "--disable-screensaver",
                    //                       "--overlay",
                    //                       "--rtsp-caching=1200",
                    //                       "--plugin-path=" + vlcInstallDirectory + @"\plugins",
                    //                       "--snapshot-path=" + vlcInstallDirectory + @"\snapshots",
                    //                       "--snapshot-prefix=snap_",
                    //                       "--key-snapshot=S",
                    //                       "--snapshot-format=png",
				};

                        Error err = VLC_Init(iVLC, initOptions.Length, initOptions);
                        if (err != Error.Success)
                        {
                            VLC_Destroy(iVLC);
                            this.iVLC = -1;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Could not find libvlc. ERROR:"+ ex.Message);
            }
        }

        public void SetOutputWindow(int outputWindow)
        {
            if (outputWindow > 0)
                VLC_VariableSet(iVLC, "drawable", outputWindow);
        }

        public Error addTarget(string Target)
        {
            return VLC_AddTarget(iVLC, Target, null, 0, (int)Mode.Append, (int)Pos.End);
        }

        public Error addTarget(string Target, string[] Options)
        {
            return VLC_AddTarget(iVLC, Target, Options, Options.Length, (int)Mode.Append, (int)Pos.End);
        }

        public Error play()
        {
            return VLC_Play(iVLC);
        }

        public Error pause()
        {
            return VLC_Pause(iVLC);
        }

        public Error stop()
        {
            return VLC_Stop(iVLC);
        }

        public Error next()
        {
            return VLC_PlaylistNext(iVLC);
        }

        public Error previous()
        {
            return VLC_PlaylistPrev(iVLC);
        }

        public Error playlistClear()
        {
            return VLC_PlaylistClear(iVLC);
        }

        public Error SetConfigVariable(String name, String value)
        {
            __config_PutPsz(iVLC, name, value);
            return Error.Success;
        }



    }
}
