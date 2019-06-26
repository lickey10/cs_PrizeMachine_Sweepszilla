using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCTV
{
    class RefreshUtilities
    {
        System.Windows.Forms.Timer goToURLTimer = new System.Windows.Forms.Timer();

        public event EventHandler ClickComplete;

        public RefreshUtilities()
        {
            goToURLTimer.Enabled = true;
            goToURLTimer.Tick += Timer_Tick; ;
            goToURLTimer.Interval = 1000;//one second
            goToURLTimer.Stop();
        }

        //private double randomSeconds()
        //{
        //    Random rnd = new Random();
        //    double seconds = rnd.Next(7, 15);

        //    double percentage = rnd.Next(1, 100);

        //    if (percentage > 80)
        //        seconds = seconds + (((rnd.Next(0, 12))));
        //    else if (percentage > 60)
        //        seconds = seconds + ((rnd.Next(0, 10)));
        //    else if (percentage > 30)
        //        seconds = seconds + (((rnd.Next(0, 10))));

        //    return seconds;
        //}

        private double randomSeconds(int seconds,int plusMinus)
        {
            Random rnd = new Random();
            int secondsLow = seconds - plusMinus;
            int secondsHigh = seconds + plusMinus;

            if (secondsLow < 0)
                secondsLow = 0;

            double rndSeconds = rnd.Next(secondsLow, secondsHigh);

            double percentage = rnd.Next(1, 100);

            if (percentage > 80)
                rndSeconds = rndSeconds + (((rnd.Next(1, 15))));
            else if (percentage > 60)
                rndSeconds = rndSeconds + ((rnd.Next(1, 10)));
            else if (percentage > 30)
                rndSeconds = rndSeconds + (((rnd.Next(1, 5))));

            return rndSeconds;
        }

        /// <summary>
        /// Stop all timers and null  the tags
        /// </summary>
        public void Cancel()
        {
            goToURLTimer.Stop();
            goToURLTimer.Tag = null;
        }

        public void GoToURL(string URL, System.Windows.Forms.TextBox txtDisplay, ExtendedWebBrowser browser)
        {
            GoToURL(URL, 12, txtDisplay, browser);
        }

        public void GoToURL(string URL, int refreshSeconds, System.Windows.Forms.TextBox txtDisplay, ExtendedWebBrowser browser)
        {
            GoToURL(URL, refreshSeconds, (int)(refreshSeconds/4), txtDisplay, browser);
        }

        public void GoToURL(string URL, int refreshSeconds, int plusMinusRefreshSeconds, System.Windows.Forms.TextBox txtDisplay, ExtendedWebBrowser browser)
        {
            try
            {
                if (goToURLTimer.Tag == null)
                {
                    goToURLTimer.Stop();

                    //this is how long before the link is clicked
                    TimerInfo timerInfo = new TimerInfo();
                    timerInfo.StartTime = DateTime.Now;
                    timerInfo.UrlToGoTo = URL;
                    timerInfo.Duration = TimeSpan.FromSeconds(randomSeconds(refreshSeconds, plusMinusRefreshSeconds));
                    timerInfo.TxtDisplay = txtDisplay;
                    timerInfo.Browser = browser;

                    goToURLTimer.Tag = timerInfo;
                    goToURLTimer.Tick += Timer_Tick;
                    goToURLTimer.Start();
                }
            }
            catch (Exception ex)
            {
                //Tools.WriteToFile(ex);
                throw;
                //Application.Restart();
            }
        }

        public void ClickElement(System.Windows.Forms.HtmlElement ElementToClick, System.Windows.Forms.TextBox txtDisplay)
        {
            try
            {
                if (goToURLTimer.Tag == null)
                {
                    goToURLTimer.Stop();

                    //this is how long before the link is clicked
                    
                    TimerInfo timerInfo = new TimerInfo();
                    timerInfo.StartTime = DateTime.Now;
                    timerInfo.ElementToClick = ElementToClick;
                    timerInfo.Duration = TimeSpan.FromSeconds(randomSeconds(11,4));
                    timerInfo.TxtDisplay = txtDisplay;

                    goToURLTimer.Tag = timerInfo;
                    goToURLTimer.Tick += Timer_Tick;
                    goToURLTimer.Start();

                    //keepRunning_tour_Timer.Stop();
                    //keepRunning_tour_Timer.Tag = null;

                }
            }
            catch (Exception ex)
            {
                //Tools.WriteToFile(ex);
                throw;
                //Application.Restart();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (((System.Windows.Forms.Timer)sender).Tag is TimerInfo)
                {
                    TimerInfo timerInfo = (TimerInfo)((System.Windows.Forms.Timer)sender).Tag;
                    TimeSpan elapsedTime = DateTime.Now - timerInfo.StartTime;
                    //int elapsedMilliseconds = ((int)((double)elapsedTime.Seconds) * 1000);

                    if (elapsedTime < timerInfo.Duration)
                    {
                        if (timerInfo.TxtDisplay != null && timerInfo.TxtDisplay is System.Windows.Forms.TextBox)
                            timerInfo.TxtDisplay.Text = (timerInfo.Duration.Seconds - elapsedTime.Seconds).ToString() + " seconds";
                    }
                    else //timer is expired
                    {
                        ((System.Windows.Forms.Timer)sender).Tag = null;
                        ((System.Windows.Forms.Timer)sender).Stop();

                        if (timerInfo.TxtDisplay != null && timerInfo.TxtDisplay is System.Windows.Forms.TextBox)
                            timerInfo.TxtDisplay.Text = "0 seconds";

                        if (timerInfo.UrlToGoTo.Trim().Length > 0 && timerInfo.Browser != null && timerInfo.Browser is ExtendedWebBrowser)
                        {
                            if (!timerInfo.Browser.IsBusy)
                                timerInfo.Browser.Stop();

                            timerInfo.Browser.Url = new Uri(timerInfo.UrlToGoTo);
                        }
                        else if(timerInfo.ElementToClick != null)//click a button or link
                        {
                            timerInfo.ElementToClick.InvokeMember("Click");

                            EventHandler handler = ClickComplete;
                            if (handler != null)
                            {
                                handler(timerInfo.ElementToClick, e);
                            }
                            
                            timerInfo.ElementToClick = null;
                        }
                    }
                }
                else
                {
                    ((System.Windows.Forms.Timer)sender).Tag = null;
                    ((System.Windows.Forms.Timer)sender).Stop();
                }
            }
            catch (Exception ex)
            {
                string found = "";
                //Tools.WriteToFile(ex);
                //throw;
                //Application.Restart();
            }
        }
    }
}
