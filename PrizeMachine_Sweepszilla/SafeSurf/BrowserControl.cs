using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace SCTV
{
    public partial class BrowserControl : UserControl
    {
        bool addressBoxActive = false;
        ArrayList foundBlockedTerms = new ArrayList();
        MainForm mf;
        bool scriptErrorsSuppressed = true;
        bool showAddress = true;

        public bool ScriptErrorSuppressed
        {
            set { scriptErrorsSuppressed = value; }
        }

        public bool ShowAddressBar
        {
            set 
            {
                pnlAddress.Visible = value;
                showAddress = value; 
            }

            get { return showAddress; }
        }

        public BrowserControl()
        {
            InitializeComponent();
            _browser = new ExtendedWebBrowser();
            _browser.Dock = DockStyle.Fill;
            _browser.DownloadComplete += new EventHandler(_browser_DownloadComplete);
            _browser.Navigated += new WebBrowserNavigatedEventHandler(_browser_Navigated);
            _browser.StartNewWindow += new EventHandler<BrowserExtendedNavigatingEventArgs>(_browser_StartNewWindow);
            _browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(_browser_DocumentCompleted);
            _browser.ScriptErrorsSuppressed = scriptErrorsSuppressed;
            this.containerPanel.Controls.Add(_browser);

            // Make the magenta color transparent on the go button
            Bitmap bmp = (Bitmap)goButton.Image;
            bmp.MakeTransparent(Color.Magenta);

            _browser.BringToFront();
            _browser.Visible = false;
            
        }

        void _browser_DownloadComplete(object sender, EventArgs e)
        {
            // Check whether the document is available (it should be)
            if (this.WebBrowser.Document != null)
            {
                // Subscribe to the Error event
                this.WebBrowser.Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);

                //subscripe to the unload event
                this.WebBrowser.Document.Window.Unload += new HtmlElementEventHandler(Window_Unload);
//                this.WebBrowser.Document.Window.Document.Forms[0].InvokeMember;//????

                UpdateAddressBox();
            }
        }

        void Window_Unload(object sender, HtmlElementEventArgs e)
        {
            //this.WebBrowser.DocumentText
        }

        void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            // We got a script error, record it
            ScriptErrorManager.Instance.RegisterScriptError(e.Url, e.Description, e.LineNumber);
 
            // Let the browser know we handled this error.
            e.Handled = true;
        }

        void _browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            UpdateAddressBox();

            displayPage();
        }

        // Updates the addres box with the actual URL of the document
        private void UpdateAddressBox()
        {
            //only update addressBox if it's not active (no one is typing in it)
            if (!addressBoxActive)
            {
                string urlString = "";

                if(this.WebBrowser.Document != null)
                    urlString = this.WebBrowser.Document.Url.ToString();
                else if(this.WebBrowser.axIWebBrowser2 != null)
                    urlString = this.WebBrowser.axIWebBrowser2.LocationURL.ToString();

                if (!urlString.Equals(this.addressTextBox.Text, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.addressTextBox.Text = urlString;
                }
            }
        }

        private void displayPage()
        {
            if (mf == null)
                mf = GetMainFormFromControl(this);
            //check content of document before displaying
            if (mf.LoggedIn || checkContents())
            {
                //display page
                _browser.Visible = true;
            }
            else
            {
                //hide page
                _browser.Visible = false;

                BrowserControl bc = WindowManager.BrowserControlFromBrowser(_browser);
                // If we got null, return
                if (bc == null) return;

                // The Tag of the BrowserControl should point to the TabPage
                TabPage page = bc.Tag as TabPage;
                // If not, return
                if (page == null) return;

                page.Text = "Page Blocked";
                page.ToolTipText = "Page Blocked";

                //display block
                lblLoading.Text = "Page Blocked";

                ArrayList content = new ArrayList();

                //log blocked terms
                foreach (string term in foundBlockedTerms)
                {
                    content.Add(term.Trim());
                }

                mf.log(MainForm.foundBlockedTermsPath, Encryption.Encrypt((string[])content.ToArray(typeof(string))));

                //refresh loaded found blocked terms
                mf.loadFoundBlockedTerms(MainForm.foundBlockedTermsPath);
            }
        }

        /// <summary>
        /// Check the contents of the page against blocked terms list
        /// </summary>
        /// <returns></returns>
        private bool checkContents()
        {
            if (mf.adminLock)
                return false;
            else if(!mf.MonitorActivity)
                return true;

            string pageContents = _browser.DocumentText;
            bool displayPage = true;

            foreach (string term in MainForm.blockedTerms)
            {
                if (pageContents.Contains(" " + term + " "))
                {
                    foundBlockedTerms.Add(term);
                    displayPage = false;
                }
            }

            if (displayPage && mf.checkForms)
            {
                //check for forms
                displayPage = checkFormContents(_browser.Document);
            }
            return displayPage;
        }

        /// <summary>
        /// check for a form that is asking too many personal questions
        /// </summary>
        /// <param name="currentDoc"></param>
        /// <returns></returns>
        private bool checkFormContents(HtmlDocument currentDoc)
        {
            //try to block all forms but a simple search or login form
            bool validForm = false;
            string name = "";
            int ruleMatchThreshold = 5;
            int rulesMatched = 0;

            if (currentDoc != null)
            {
                foreach (HtmlElement formElement in currentDoc.Forms)
                {
                    string innerText = formElement.InnerText;
                    rulesMatched = 0;

                    //check for first name
                    if (innerText.ToLower().Contains("first name"))
                        rulesMatched++;

                    //check for last name
                    if (innerText.ToLower().Contains("last name"))
                        rulesMatched++;

                    //check for address
                    if (innerText.ToLower().Contains(" address"))
                        rulesMatched++;

                    //check for city
                    if (innerText.ToLower().Contains(" city"))
                        rulesMatched++;

                    //check for state
                    if (innerText.ToLower().Contains(" state"))
                        rulesMatched++;

                    //check for age
                    if (innerText.ToLower().Contains(" age "))
                        rulesMatched++;

                    //check for birthdate
                    if (innerText.ToLower().Contains("birthdate"))
                        rulesMatched++;

                    //check for interests
                    if (innerText.ToLower().Contains("interests"))
                        rulesMatched++;

                    //check for email address
                    if (innerText.ToLower().Contains(" email"))
                        rulesMatched++;

                    //check for password
                    if (innerText.ToLower().Contains(" password"))
                        rulesMatched++;

                    //check for username
                    if (innerText.ToLower().Contains("user name"))
                        rulesMatched++;

                    //check for gender
                    if (innerText.ToLower().Contains(" gender"))
                        rulesMatched++;

                    //check for postal code
                    if (innerText.ToLower().Contains("postal code"))
                        rulesMatched++;

                    if (rulesMatched >= ruleMatchThreshold)
                        break;
                }
                if (rulesMatched < ruleMatchThreshold)
                    validForm = true;
            }

            return validForm;
        }

        void _browser_StartNewWindow(object sender, BrowserExtendedNavigatingEventArgs e)
        {
            // Here we do the pop-up blocker work

            // Note that in Windows 2000 or lower this event will fire, but the
            // event arguments will not contain any useful information
            // for blocking pop-ups.

            // There are 4 filter levels.
            // None: Allow all pop-ups
            // Low: Allow pop-ups from secure sites
            // Medium: Block most pop-ups
            // High: Block all pop-ups (Use Ctrl to override)

            // We need the instance of the main form, because this holds the instance
            // to the WindowManager.
            if (mf == null)
                mf = GetMainFormFromControl(this);

            if (mf == null)
                return;

            // Allow a popup when there is no information available or when the Ctrl key is pressed
            bool allowPopup = (e.NavigationContext == UrlContext.None) || ((e.NavigationContext & UrlContext.OverrideKey) == UrlContext.OverrideKey);

            if (!allowPopup)
            {
                // Give None, Low & Medium still a chance.
                //switch (SettingsHelper.Current.FilterLevel)
                //{
                //    case PopupBlockerFilterLevel.None:
                //        allowPopup = true;
                //        break;
                //    case PopupBlockerFilterLevel.Low:
                //        // See if this is a secure site
                //        if (this.WebBrowser.EncryptionLevel != WebBrowserEncryptionLevel.Insecure)
                //            allowPopup = true;
                //        else
                //            // Not a secure site, handle this like the medium filter
                //            goto case PopupBlockerFilterLevel.Medium;
                //        break;
                //    case PopupBlockerFilterLevel.Medium:
                //        // This is the most dificult one.
                //        // Only when the user first inited and the new window is user inited
                //        if ((e.NavigationContext & UrlContext.UserFirstInited) == UrlContext.UserFirstInited && (e.NavigationContext & UrlContext.UserInited) == UrlContext.UserInited)
                //            allowPopup = true;
                //        break;
                //}
            }

            if (allowPopup)
            {
                // Check wheter it's a HTML dialog box. If so, allow the popup but do not open a new tab
                if (!((e.NavigationContext & UrlContext.HtmlDialog) == UrlContext.HtmlDialog))
                {
                    ExtendedWebBrowser ewb = mf.WindowManager.New(false);
                    // The (in)famous application object
                    e.AutomationObject = ewb.Application;
                }
            }
            else
                // Here you could notify the user that the pop-up was blocked
                e.Cancel = true;

        }

        void _browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            UpdateAddressBox();
        }

        private ExtendedWebBrowser _browser;

        // Allows other code to obtain a reference to the extended web browser component
        public ExtendedWebBrowser WebBrowser
        {
            get { return _browser; }
        }

        // Used for the go button
        private void goButton_Click(object sender, EventArgs e)
        {
            Navigate();
        }

        /// <summary>
        /// Navigate to the typed address
        /// </summary>
        private void Navigate()
        {
            if (blockedSite(this.addressTextBox.Text))
            {
                //hide page
                _browser.Visible = false;

                BrowserControl bc = WindowManager.BrowserControlFromBrowser(_browser);
                // If we got null, return
                if (bc == null) return;

                // The Tag of the BrowserControl should point to the TabPage
                TabPage page = bc.Tag as TabPage;
                // If not, return
                if (page == null) return;

                page.Text = "Site Blocked";
                page.ToolTipText = "Site Blocked";

                //display block
                lblLoading.Text = "Site Blocked";

                string[] content = new string[]{ this.addressTextBox.Text };

                if (mf == null)
                    mf = GetMainFormFromControl(this);

                //log blocked site
                mf.log(MainForm.foundBlockedSitesPath, Encryption.Encrypt(content));

                //refresh loaded found blocked sites
                mf.loadFoundBlockedSites(MainForm.foundBlockedSitesPath);
            }
            else
            {
                lblLoading.Text = "Loading...";

                this.WebBrowser.Navigate(this.addressTextBox.Text);
            }

            //reset loggedInTime since there is activity
            if(mf == null)
                mf = GetMainFormFromControl(this);
            mf.loggedInTime = 0;
        }

        // Used for obtaining the MainForm from a control
        public static MainForm GetMainFormFromControl(Control control)
        {
            while (control != null)
            {
                if (control is MainForm)
                    break;
                control = control.Parent;
            }
            return control as MainForm;
        }

        // Used for catching the Enter key in the textbox
        private void addressTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            addressBoxActive = true;

            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                Navigate();
            }
        }

        private void addressTextBox_Leave(object sender, EventArgs e)
        {
            addressBoxActive = false;
        }

        /// <summary>
        /// check current address against blockedSites list
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        private bool blockedSite(string site)
        {
            if (mf.adminLock)
                return true;
            else if (!mf.MonitorActivity)
                return false;

            bool isBlocked = false;

            foreach (string blockedSite in MainForm.blockedSites)
            {
                if (site.ToLower().Contains(blockedSite.ToLower()))
                {
                    isBlocked = true;
                    break;
                }
            }

            return isBlocked;
        }
    }
}
