using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;
using SCTVObjects;

namespace SCTV.AdvancedMediaFilter
{
    public partial class AdvancedMediaFilter : UserControl
    {
        string selectedGenres = "";
        string selectedMediaTypes = "";
        string selectedStartsWith = "";
        MediaHandler myMedia = new MediaHandler();

        public delegate void UpdateFilters(string SelectedGenres, string SelectedMediaTypes, string SelectedStartsWith);
        public event UpdateFilters FiltersUpdated;

        //public string SelectedGenres
        //{
        //    get { return selectedGenres; }
        //    set { selectedGenres = value; }
        //}

        //public string SelectedMediaTypes
        //{
        //    get { return selectedMediaTypes; }
        //    set { selectedMediaTypes = value; }
        //}

        //public string SelectedStartsWith
        //{
        //    get { return selectedStartsWith; }
        //    set { selectedStartsWith = value; }
        //}

        public AdvancedMediaFilter()
        {
            InitializeComponent();

            initializeAdvancedFilters();
        }

        private void initializeAdvancedFilters()
        {
            //get mediatypes
            string correctCase = "";

            ArrayList mediaTypes = new ArrayList();

            try
            {
                //foreach (DataRowView drv in myMedia.GetAllMediaTypes())
                foreach (string type in myMedia.GetAllMediaTypes())
                {
                    string[] typeArray = type.ToString().Split('/');

                    for (int x = 0; x < typeArray.Length; x++)
                    {
                        correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(typeArray[x].Trim());
                        if (!mediaTypes.Contains(correctCase) && correctCase.Trim().Length > 0)//keep out duplicates
                            mediaTypes.Add(correctCase);
                    }


                    //correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(type.Trim());
                    //if (!mediaTypes.Contains(correctCase))//keep out duplicates
                    //    mediaTypes.Add(correctCase);
                }

                mediaTypes.Add("Online");
                mediaTypes.Sort();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            //set media types
            foreach (string tempType in mediaTypes)
                mediaTypeToolStripMenuItem.DropDownItems.Add(tempType, null, mediaTypeToolStripMenuItem_Click);

            setAdvancedGenreFilters(mediaTypes);
        }

        private void updateMediaFilters()
        {
            foreach (ToolStripItem filter in toolStripDynamicFilter.Items)
            {
                if (filter.Text != "Filters" && filter.Text.Length > 0)
                {
                    switch (filter.Tag.ToString())
                    {
                        case "Genre":
                            if (!selectedGenres.Contains(filter.Text))
                            {
                                if (selectedGenres.Length > 0)
                                    selectedGenres = selectedGenres + "|";

                                selectedGenres += filter.Text;
                            }
                            break;
                        case "Media Type":
                            if (!selectedMediaTypes.Contains(filter.Text))
                            {
                                if (selectedMediaTypes.Length > 0)
                                    selectedMediaTypes = selectedMediaTypes + "|";

                                selectedMediaTypes += filter.Text;
                            }
                            break;
                        case "Alpha/Numeric":
                            selectedStartsWith = filter.Text;
                            break;
                    }
                }
            }

            FiltersUpdated(selectedGenres, selectedMediaTypes, selectedStartsWith);
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int x = 1; x < toolStripDynamicFilter.Items.Count; x++)
                toolStripDynamicFilter.Items.RemoveAt(x);
        }

        /// <summary>
        /// Handle clicks on the media types dropdown list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mediaTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownButton newItem = new ToolStripDropDownButton();
            newItem.Text = ((ToolStripDropDownItem)sender).Text;
            newItem.Tag = ((ToolStripDropDownItem)sender).OwnerItem.Text;
            newItem.DropDownItems.Add("Delete", null, deleteItem_click);

            toolStripDynamicFilter.Items.Add(newItem);

            updateMediaFilters();
        }

        /// <summary>
        /// Handle clicks on the genre dropdown list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void genreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownButton newItem = new ToolStripDropDownButton();
            newItem.Text = ((ToolStripDropDownItem)sender).Text;
            newItem.Tag = ((ToolStripDropDownItem)sender).OwnerItem.Text;
            newItem.DropDownItems.Add("Delete", null, deleteItem_click);

            toolStripDynamicFilter.Items.Add(newItem);

            updateMediaFilters();
        }

        /// <summary>
        /// Handle clicks on the alpha/numeric dropdown list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void alphaNumericToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownButton newItem = new ToolStripDropDownButton();
            newItem.Text = ((ToolStripDropDownItem)sender).Text;
            newItem.Tag = ((ToolStripDropDownItem)sender).OwnerItem.Text;
            newItem.DropDownItems.Add("Delete", null, deleteItem_click);

            toolStripDynamicFilter.Items.Add(newItem);

            updateMediaFilters();
        }

        private void setAdvancedGenreFilters(ArrayList mediaTypes)
        {
            ArrayList allGenres = new ArrayList();

            foreach (string type in mediaTypes)
                allGenres = setAdvancedGenreFilters(type, false);

            allGenres.Sort();

            //set genre
            foreach (string tempCat in allGenres)
                genreToolStripMenuItem1.DropDownItems.Add(tempCat, null, genreToolStripMenuItem_Click);
        }

        private ArrayList setAdvancedGenreFilters(string mediaType, bool addToToolStripMenu)
        {
            //get genre
            string correctCase = "";

            //display categories
            ArrayList categories = new ArrayList();

            foreach (DataRowView drv in myMedia.GetAllCategories(mediaType))
            {
                string[] catArray;
                string categoryString = drv["category"].ToString();

                if (categoryString.Contains("/"))
                    catArray = categoryString.Split('/');
                else
                    catArray = categoryString.Split('|');

                for (int x = 0; x < catArray.Length; x++)
                {
                    correctCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(catArray[x].Trim());
                    if (!categories.Contains(correctCase) && correctCase.Length > 0)//keep out duplicates
                        categories.Add(correctCase);
                }
            }

            //add internet before sort so it will be sorted
            if (mediaType.ToLower() == "online")
                categories.Add("Internet");

            categories.Sort();//alphabetize

            //add utility pages to categories
            if (mediaType.ToLower() != "online")
            {
                if (!categories.Contains("New"))
                    categories.Insert(0, "New");

                if (!categories.Contains("Recent"))
                    categories.Insert(1, "Recent");

                if (!categories.Contains("Star Rating"))
                    categories.Insert(2, "Star Rating");

                if (!categories.Contains("Misc"))
                    categories.Insert(3, "Misc");

                if (!categories.Contains("Popular"))
                    categories.Insert(4, "Popular");

                if (!categories.Contains("Not-So Popular"))
                    categories.Insert(5, "Not-So Popular");

                if (!categories.Contains("All"))
                    categories.Insert(6, "All");
            }
            else if (mediaType.ToLower() == "online")
            {
                if (!categories.Contains("Home"))
                    categories.Insert(0, "Home");
            }

            if (!categories.Contains("Playlist"))
                categories.Insert(1, "Playlist");


            if (addToToolStripMenu)
            {
                //set genre
                foreach (string tempCat in categories)
                    genreToolStripMenuItem1.DropDownItems.Add(tempCat, null, genreToolStripMenuItem_Click);
            }

            return categories;
        }

        private void deleteItem_click(object sender, EventArgs e)
        {
            toolStripDynamicFilter.Items.Remove(((ToolStripMenuItem)sender).OwnerItem);

            updateMediaFilters();
        }
    }
}
