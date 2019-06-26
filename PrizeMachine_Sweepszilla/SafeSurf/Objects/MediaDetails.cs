using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SCTVObjects
{
    public partial class MediaDetails : Form
    {
        string movieTitle = "";
        Media mediaToEdit = new Media();
        bool readOnly = false;
        MediaHandler myMedia = new MediaHandler();
        
        public Media MediaToEdit
        {
            set
            {
                mediaToEdit = value; 
            }

            get
            {
                return mediaToEdit;
            }
        }

        public bool ReadOnly
        {
            set
            {
                readOnly = value;
            }

            get
            {
                return readOnly;
            }
        }

        public bool AutoUpdate
        {
            set 
            { 
                if(value)
                    btnAutoUpdate_Click(btnAutoUpdate, null); 
            }
        }

        public bool ClearIMDBNum
        {
            set
            {
                if (value)
                    txtImdbNumResult.Text = "";
            }
        }

        public MediaDetails(Media media, DataView availableCategories, ArrayList availableMediaCategories)
        {
            InitializeComponent();
            
            this.Refresh();

            MediaToEdit = media;

            populateForm(media, availableCategories, availableMediaCategories);
        }

        public MediaDetails(Media media)
        {
            InitializeComponent();
            
            this.Refresh();

            MediaToEdit = media;

            populateForm(media, myMedia.GetAllCategories(media.MediaType), myMedia.GetAllMediaTypes());
        }

        /// <summary>
        /// populate search ui with given media
        /// </summary>
        /// <param name="value"></param>
        private void populateForm(Media media, DataView availableCategories, ArrayList availableMediaCategories)
        {
            try
            {
                ArrayList categories = new ArrayList();
                ArrayList mediaCategories = new ArrayList();

                txtTitleResult.Text = media.Title;
                txtDirectorResult.Text = media.Director;

                string[] catArray;
                if (media.category.Contains("/"))
                    catArray = media.category.Split('/');
                else
                    catArray = media.category.Split('|');

                string[] medCatArray;
                if (media.MediaType.Contains("/"))
                    medCatArray = media.MediaType.Split('/');
                else
                    medCatArray = media.MediaType.Split('|');

                foreach (DataRowView drv in availableCategories)
                {
                    string catString = drv["category"].ToString();

                    string[] tempCatArray;

                    if (catString.Contains("/"))
                        tempCatArray = catString.Split('/');
                    else
                        tempCatArray = catString.Split('|');

                    foreach (string cat in tempCatArray)
                    {
                        if (!categories.Contains(cat.Trim()) && cat.Trim().Length > 0)
                            categories.Add(cat.Trim());
                    }
                }

                //add current categories
                foreach (string category in catArray)
                {
                    if (category.Trim().Length > 0 && !lbCurrentGenres.Items.Contains(category))
                        lbCurrentGenres.Items.Add(category.Trim());

                    if (categories.Contains(category.Trim()))
                        categories.Remove(category.Trim());
                }

                categories.TrimToSize();

                //add available categories
                foreach (string category in categories)
                    if(!lbAvailableGenres.Items.Contains(category.Trim()))
                        lbAvailableGenres.Items.Add(category.Trim());

                foreach (string catString in availableMediaCategories)
                {
                    //check for both "/" and "|"
                    string[] tempCatArray;

                    if (catString.Contains("/"))
                        tempCatArray = catString.Split('/');
                    else
                        tempCatArray = catString.Split('|');

                    foreach (string cat in tempCatArray)
                    {
                        if (!mediaCategories.Contains(cat.Trim()) && cat.Trim().Length > 0)
                            mediaCategories.Add(cat.Trim());
                    }
                }

                //add current mediaCategories
                foreach (string category in medCatArray)
                {
                    if (category.Trim().Length > 0 && !lbSelectedCategories.Items.Contains(category))
                        lbSelectedCategories.Items.Add(category.Trim());

                    if (mediaCategories.Contains(category.Trim()))
                        mediaCategories.Remove(category.Trim());
                }

                mediaCategories.TrimToSize();

                //add available mediaCategories
                foreach (string category in mediaCategories)
                    if(!lbAvailableCategories.Items.Contains(category.Trim()))
                        lbAvailableCategories.Items.Add(category.Trim());

                txtReleaseYearResult.Text = media.ReleaseYear;
                txtTaglineResult.Text = media.Description;
                txtImdbNumResult.Text = media.IMDBNum;
                txtRatingResult.Text = media.Rating;
                txtRatingReasonResult.Text = media.RatingDescription;

                if(System.IO.File.Exists(media.coverImage))
                    thumbnailPictureBox.ImageLocation = media.coverImage;
                else
                    thumbnailPictureBox.ImageLocation = Application.StartupPath + "//images//media//coverimages//notavailable.jpg";

                //add filepaths
                string[] filePaths;

                if (media.filePath != null && media.filePath.Contains("|"))
                    filePaths = media.filePath.Split('|');
                else
                {
                    filePaths = new string[1];
                    filePaths[0] = media.filePath;
                }

                foreach(string filepath in filePaths)
                {
                    if(!lbFiles.Items.Contains(filepath))
                        lbFiles.Items.Add(filepath.Trim());
                }

                //add description
                txtDescription.Text = media.Description;
                txtShortDescription.Text = media.ShortDescription;
                txtCoverImage.Text = media.coverImage;
                txtGoofs.Text = media.Goofs;
                txtTrivia.Text = media.Trivia;
                txtPerformers.Text = media.Performers;

                //enable/disable controls
                txtDirectorResult.ReadOnly = readOnly;
                txtImdbNumResult.ReadOnly = readOnly;
                txtRatingReasonResult.ReadOnly = readOnly;
                txtRatingResult.ReadOnly = readOnly;
                txtReleaseYearResult.ReadOnly = readOnly;
                txtTaglineResult.ReadOnly = readOnly;
                txtTitleResult.ReadOnly = readOnly;
                txtPerformers.ReadOnly = readOnly;
                btnAddCategory.Enabled = !readOnly;
                btnAddGenre.Enabled = !readOnly;
                btnAddNewCategory.Enabled = !readOnly;
                btnAddNewGenre.Enabled = !readOnly;
                btnRemoveCategory.Enabled = !readOnly;
                btnRemoveGenre.Enabled = !readOnly;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string selectedCategories = "";
            string selectedMediaTypes = "";
            string tempFilePath = "";

            mediaToEdit.Title = txtTitleResult.Text;
            mediaToEdit.Director = txtDirectorResult.Text;

            foreach (string category in lbCurrentGenres.Items)
            {
                if (category.Trim().Length > 0)
                    selectedCategories += category.Trim() + " | ";
            }

            mediaToEdit.category = selectedCategories;
            mediaToEdit.ReleaseYear = txtReleaseYearResult.Text;
            mediaToEdit.TagLine = txtTaglineResult.Text;
            mediaToEdit.IMDBNum = txtImdbNumResult.Text;
            mediaToEdit.Rating = txtRatingResult.Text;
            mediaToEdit.RatingDescription = txtRatingReasonResult.Text;
            mediaToEdit.Description = txtDescription.Text;
            MediaToEdit.ShortDescription = txtShortDescription.Text;
            MediaToEdit.Performers = txtPerformers.Text;

            foreach (string category in lbSelectedCategories.Items)
            {
                if (selectedMediaTypes.Trim().Length > 0)
                    selectedMediaTypes += "|";

                if (category.Trim().Length > 0)
                    selectedMediaTypes += category.Trim();
            }

            foreach(string filePath in lbFiles.Items)
            {
                if(filePath.Trim().Length > 0)
                {
                    if (tempFilePath.Trim().Length > 0)
                        tempFilePath += "|";

                    tempFilePath += filePath;
                }
            }

            mediaToEdit.filePath = tempFilePath;

            mediaToEdit.MediaType = selectedMediaTypes;

            mediaToEdit.Goofs = txtGoofs.Text;
            mediaToEdit.Trivia = txtTrivia.Text;
            mediaToEdit.coverImage = txtCoverImage.Text;

            //update media file
            myMedia.UpdateMediaInfo(mediaToEdit);

            //update in memory dataset
            //MediaHandler.GetMedia();

            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddGenre_Click(object sender, EventArgs e)
        {
            moveListboxItems(lbAvailableGenres, lbCurrentGenres);
        }

        private void btnRemoveGenre_Click(object sender, EventArgs e)
        {
            moveListboxItems(lbCurrentGenres, lbAvailableGenres);
        }

        private void btnRemoveCategory_Click(object sender, EventArgs e)
        {
            moveListboxItems(lbSelectedCategories, lbAvailableCategories);
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            moveListboxItems(lbAvailableCategories, lbSelectedCategories);
        }

        /// <summary>
        /// Move selected item to next listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbAvailableCategories_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            moveListboxItems(lbAvailableCategories, lbSelectedCategories);
        }

        private void lbSelectedCategories_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            moveListboxItems(lbSelectedCategories, lbAvailableCategories);
        }

        /// <summary>
        /// Move selected item to next listbox
        /// </summary>
        /// <param name="lbSource"></param>
        /// <param name="lbDestination"></param>
        /// <param name="valueToMove"></param>
        private void moveListboxItems(ListBox lbSource, ListBox lbDestination)
        {
            try
            {
                ArrayList movedItems = new ArrayList();

                foreach (string selectedItem in lbSource.SelectedItems)
                {
                    lbDestination.Items.Add(selectedItem);

                    movedItems.Add(selectedItem);
                }

                foreach (string itemToRemove in movedItems)
                    lbSource.Items.Remove(itemToRemove);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void autoUpdate()
        {
            Cursor = Cursors.WaitCursor;
            Media foundMedia = null;

            if (txtTitleResult.Text.Trim().Length > 0)
            {
                string textToSearch = txtTitleResult.Text.Replace("_", " ");

                if (lbSelectedCategories.Items.Contains("Music") || lbSelectedCategories.Items.Contains("music"))
                {
                    MusicBrainzScraper musicBrainz = new MusicBrainzScraper();

                    foundMedia = musicBrainz.getInfoByTitleAndArtist(MediaToEdit);
                }
                else
                {
                    IMDBScraper imdb = new IMDBScraper();
                    
                    //search by title
                    foundMedia = imdb.getInfoByTitle(textToSearch, false, "");
                }

                if (foundMedia != null)
                {
                    string tempFilePath = "";

                    foreach(string filePath in lbFiles.Items)
                    {
                        if (tempFilePath.Trim().Length > 0)
                            tempFilePath += "|";

                        tempFilePath += filePath;
                    }

                    foundMedia.filePath = tempFilePath;

                    populateForm(foundMedia, myMedia.GetAllCategories(foundMedia.MediaType), myMedia.GetAllMediaTypes());
                }
                else
                    txtTitleResult.Text = "No Results";
            }
            else
            {
                MessageBox.Show("You must enter something to search for");
            }

            Cursor = Cursors.Default;
        }

        private void lbAvailableGenres_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            moveListboxItems(lbAvailableGenres, lbCurrentGenres);
        }

        private void lbCurrentGenres_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            moveListboxItems(lbCurrentGenres, lbAvailableGenres);
        }

        private void btnAddNewCategory_Click(object sender, EventArgs e)
        {
            AddNewItem addItem = new AddNewItem();

            DialogResult result = addItem.ShowDialog(this);

            if (result == DialogResult.Yes)
                lbSelectedCategories.Items.Add(addItem.ItemToAdd);
        }

        private void btnAddNewGenre_Click(object sender, EventArgs e)
        {
            AddNewItem addItem = new AddNewItem();

            DialogResult result = addItem.ShowDialog(this);

            if(result == DialogResult.Yes)
                lbCurrentGenres.Items.Add(addItem.ItemToAdd);
        }

        private void btnAutoUpdate_Click(object sender, EventArgs e)
        {
            autoUpdate();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtCoverImage.Text = "";
            thumbnailPictureBox.ImageLocation = Application.StartupPath +"//images//media//coverimages//notavailable.jpg";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Cover Image";
            fdlg.InitialDirectory = Application.StartupPath +"//images//media//coverimages//";
            fdlg.Filter = "All files (*.*)|*.*|All files (*.*)|*.*";
            //fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;

            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                txtCoverImage.Text = fdlg.FileName;
                thumbnailPictureBox.ImageLocation = fdlg.FileName;
            }
        }

        private void btnClearInfo_Click(object sender, EventArgs e)
        {
            //clear all form info
            lbSelectedCategories.Items.Clear();
            lbCurrentGenres.Items.Clear();
            txtPerformers.Text = "";
            txtRatingResult.Text = "";
            txtRatingReasonResult.Text = "";
            txtDirectorResult.Text = "";
            txtReleaseYearResult.Text = "";
            txtImdbNumResult.Text = "";
            txtCoverImage.Text = "";
            txtTaglineResult.Text = "";
            txtGoofs.Text = "";
            txtTrivia.Text = "";

            thumbnailPictureBox.ImageLocation = Application.StartupPath + "//images//media//coverimages//notavailable.jpg";
        }

        private void btnFileUp_Click(object sender, EventArgs e)
        {
            ArrayList itemArray = new ArrayList();
            int itemCounter = 0;
            int itemIndex = lbFiles.SelectedIndex - 1;

            if (itemIndex < 0)
                itemIndex = 0;

            foreach (string item in lbFiles.Items)
            {
                if(itemCounter == itemIndex)//this is the new home of our selected item
                {
                    itemArray.Add(lbFiles.SelectedItem);
                }
                else if(itemCounter == lbFiles.SelectedIndex)
                {
                    itemArray.Add(lbFiles.Items[itemIndex]);
                }
                else
                    itemArray.Add(item);

                itemCounter++;
            }

            lbFiles.Items.Clear();

            foreach(string item in itemArray)
                lbFiles.Items.Add(item);

            lbFiles.SelectedIndex = itemIndex;
        }

        private void btnFileDown_Click(object sender, EventArgs e)
        {
            ArrayList itemArray = new ArrayList();
            int itemCounter = 0;
            int itemIndex = lbFiles.SelectedIndex + 1;

            if (itemIndex >= lbFiles.Items.Count)
                itemIndex = lbFiles.Items.Count - 1;

            foreach (string item in lbFiles.Items)
            {
                if (itemCounter == lbFiles.SelectedIndex)
                {
                    itemArray.Add(lbFiles.Items[itemIndex]);
                }
                else if (itemCounter == itemIndex)//this is the old home of our selected item
                {
                    itemArray.Add(lbFiles.SelectedItem);
                }
                else
                    itemArray.Add(item);

                itemCounter++;
            }

            lbFiles.Items.Clear();

            foreach (string item in itemArray)
                lbFiles.Items.Add(item);

            lbFiles.SelectedIndex = itemIndex;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lbFiles.Items.Count > 1)
                lbFiles.Items.Remove(lbFiles.SelectedItem);
            else
                MessageBox.Show("You must have at least one filepath");
        }
    }
}
