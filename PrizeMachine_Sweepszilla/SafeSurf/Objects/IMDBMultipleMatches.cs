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
    public partial class MultipleMatches : Form
    {
        ArrayList multipleMatches = new ArrayList();
        Media mediaResult = null;

        public ArrayList Media
        {
            get
            {
                return multipleMatches;
            }

            set
            {
                multipleMatches = value;
            }
        }

        public Media MediaResult
        {
            get
            {
                return mediaResult;
            }
        }

        public MultipleMatches()
        {
            InitializeComponent();
        }

        public MultipleMatches(ArrayList Media)
        {
            InitializeComponent();

            multipleMatches = Media;

            populateListView();
        }

        private void populateListView()
        {
            if (multipleMatches.Count > 0)
            {
                foreach (Media media in multipleMatches)
                {
                    ListViewItem newItem = new ListViewItem();
                    newItem.Text = media.Title + " " + media.ReleaseYear + " " + media.MediaType;
                    newItem.Tag = media;
                    newItem.ToolTipText = media.IMDBNum;

                    lvMatches.Items.Add(newItem);
                }
            }
            else
            {
                ListViewItem newItem = new ListViewItem();
                newItem.Text = "No Results";
                newItem.ToolTipText = "No Results";

                lvMatches.Items.Add(newItem);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            selectMedia();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            selectMedia();
        }

        private void selectMedia()
        {
            Cursor = Cursors.WaitCursor;
            
            if (lvMatches.SelectedItems.Count > 0)
            {
                if (lvMatches.SelectedItems[0].Tag == null)//no results
                    this.Close();

                Media media = (Media)lvMatches.SelectedItems[0].Tag;

                //search for the item selected
                IMDBScraper scraper = new IMDBScraper();
                mediaResult = scraper.getInfoByNumber(media);
            }

            if (mediaResult != null)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            Cursor = Cursors.Default;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}