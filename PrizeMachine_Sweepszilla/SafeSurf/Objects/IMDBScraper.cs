using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Web;
using System.Collections;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Configuration;

namespace SCTVObjects
{
    public class IMDBScraper
    {
        private string imdbTitleSearchURL = "http://www.imdb.com/find?s=all&q=";
        private string imdbNumSearchURL = "http://www.imdb.com/title/";
        private string _FileContent;
        private HTMLCodes htmlCodes;

        #region Properties and their fields
        private string _MovieTitle;
        private string _Director;
        private string _Genre;
        private string _ReleaseYear;
        private string _TagLine;
        private string _rating;
        private string _ratingDescription;
        private string _imdbNum;
        private string _coverImage;
        private string _description;
        private System.Drawing.Image _thumbnail;
        private string titleMatch = "";
        private string exactTitleMatch = "";
        private string popularTitleMatch = "";
        private string partialTitleMatch = "";
        private string directorMatch = "";
        private string releaseYearMatch = "";
        private string genreMatch = "";
        private string taglineMatch = "";
        private string descriptionMatch = "";
        private string ratingMatch = "";
        private string ratingDescriptionMatch = "";
        private string titleSearchURL = "";
        private string numberSearchURL = "";
        private string _goofs = "";
        private string _trivia = "";
        private string _shortDescription = "";
        private string _stars = "";
        private string season = "";
        private string episodeNum = "";
        private string seriesName = "";
        private string seriesIMDBNum = "";
        private string seriesImage = "";
        private string seriesDescription = "";

        /// <summary>
        /// The title of the movie
        /// </summary>
        public string MovieTitle
        {
            get { return _MovieTitle; }
        }

        /// <summary>
        /// The name of the director
        /// </summary>
        public string Director
        {
            get { return _Director; }
        }

        /// <summary>
        /// An array of all the genres that apply to the movie
        /// </summary>
        public string Genre
        {
            get { return _Genre; }
        }

        /// <summary>
        /// The date the movie was released
        /// </summary>
        public string ReleaseYear
        {
            get { return _ReleaseYear; }
        }

        /// <summary>
        /// A breaf intro about the movie
        /// </summary>
        public string TagLine
        {
            get { return _TagLine; }
        }

        /// <summary>
        /// A breaf description about the movie
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// A thumbnail of the poster
        /// </summary>
        public System.Drawing.Image Thumbnail
        {
            get { return _thumbnail; }
        }

        public string Rating
        {
            get { return _rating; }
        }

        public string RatingDescription
        {
            get { return _ratingDescription; }
        }

        public string IMDBNum
        {
            get { return _imdbNum; }
        }

        public string CoverImage
        {
            get { return _coverImage; }
        }

        public string Goofs
        {
            get { return _goofs; }
        }

        public string Trivia
        {
            get { return _trivia; }
        }

        public string ShortDescription
        {
            get { return _shortDescription; }
        }

        public string Stars
        {
            get { return _stars; }
        }

        public string Season
        {
            get { return season; }
        }

        public string EpisodeNum
        {
            get { return episodeNum; }
        }

        public string SeriesName
        {
            get { return seriesName; }
        }

        public string SeriesIMDBNum
        {
            get { return seriesIMDBNum; }
        }

        public string SeriesImage
        {
            get { return seriesImage; }
        }

        public string SeriesDescription
        {
            get { return seriesDescription; }
        }
        #endregion

        public IMDBScraper()
        {
            initVariables();
        }

        private void initVariables()
        {
            try
            {
                titleMatch = ConfigurationManager.AppSettings["IMDBInfo.TitleMatch"];
                exactTitleMatch = ConfigurationManager.AppSettings["IMDBInfo.ExactTitleMatch"];
                popularTitleMatch = ConfigurationManager.AppSettings["IMDBInfo.PopularTitleMatch"];
                partialTitleMatch = ConfigurationManager.AppSettings["IMDBInfo.PartialTitleMatch"];
                directorMatch = ConfigurationManager.AppSettings["IMDBInfo.DirectorMatch"];
                releaseYearMatch = ConfigurationManager.AppSettings["IMDBInfo.ReleaseYearMatch"];
                genreMatch = ConfigurationManager.AppSettings["IMDBInfo.GenreMatch"];
                taglineMatch = ConfigurationManager.AppSettings["IMDBInfo.TaglineMatch"];
                descriptionMatch = ConfigurationManager.AppSettings["IMDBInfo.DescriptionMatch"];
                ratingMatch = ConfigurationManager.AppSettings["IMDBInfo.RatingMatch"];
                ratingDescriptionMatch = ConfigurationManager.AppSettings["IMDBInfo.RatingDescriptionMatch"];
                titleSearchURL = ConfigurationManager.AppSettings["IMDBInfo.TitleSearchURL"];
                numberSearchURL = ConfigurationManager.AppSettings["IMDBInfo.NumberSearchURL"];

                if (titleSearchURL != null)
                    imdbTitleSearchURL = titleSearchURL;

                if (numberSearchURL != null)
                    imdbNumSearchURL = numberSearchURL;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        }

        /// <summary>
        /// Get IMDB info by the IMDB number
        /// </summary>
        /// <param name="imdbNum">imdb number</param>
        public Media getInfoByNumber(Media media)
        {
            try
            {
                //download the whole page, to be able to search it by regex
                string URL = imdbNumSearchURL + HttpUtility.UrlEncode(media.IMDBNum) + "/";
                StreamReader sr = new StreamReader(new WebClient().OpenRead(URL));
                _FileContent = sr.ReadToEnd();

                _imdbNum = media.IMDBNum;

                htmlCodes = new HTMLCodes();

                return getInfo(media, false, media.MediaType);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);

                return null;
            }
        }

        /// <summary>
        /// Get IMDB info by the IMDB number
        /// </summary>
        /// <param name="imdbNum">imdb number</param>
        public Media getInfoByNumber(string imdbNumber)
        {
            try
            {
                //download the whole page, to be able to search it by regex
                string URL = imdbNumSearchURL + HttpUtility.UrlEncode(imdbNumber) + "/";
                StreamReader sr = new StreamReader(new WebClient().OpenRead(URL));
                _FileContent = sr.ReadToEnd();

                //_imdbNum = media.IMDBNum;

                htmlCodes = new HTMLCodes();

                Media media = new Media();
                media.IMDBNum = imdbNumber;

                return getInfo(media, true, "");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);

                return null;
            }
        }

        /// <summary>
        /// Get IMDB info by the media title
        /// </summary>
        /// <param name="title">Media title</param>
        /// <param name="bestMatch">Whether to automatically choose first popular result match or let the user choose</param>
        public Media getInfoByTitle(string title, bool bestMatch, string mediaType)
        {
            try
            {
                title = SCTVObjects.MediaHandler.FormatNameString(title);

                if (title.IndexOf("(") > 0)
                    title = title.Substring(0, title.IndexOf("("));

                //title = title.Replace("_", " ");

                //download the whole page, to be able to search it by regex
                string URL = imdbTitleSearchURL + HttpUtility.UrlEncode(title);
                StreamReader sr = new StreamReader(new WebClient().OpenRead(URL));
                _FileContent = sr.ReadToEnd();

                htmlCodes = new HTMLCodes();

                Media media = new Media();
                media.Title = title;

                return getInfo(media, bestMatch, mediaType);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);

                Media media = new Media();
                media.Title = title;

                return media;
            }                    
        }

        /// <summary>
        /// Get IMDB info by the media title
        /// </summary>
        /// <param name="title">Media title</param>
        /// <param name="bestMatch">Whether to automatically choose first popular result match or let the user choose</param>
        public Media getInfoByTitle(string title, bool bestMatch, Media media)
        {
            try
            {
                title = SCTVObjects.MediaHandler.FormatNameString(title);

                if (title.IndexOf("(") > 0)
                    title = title.Substring(0, title.IndexOf("("));
                else if (media.Title.Length > 0)
                    title = media.Title;

                //title = title.Replace("_", " ");

                //download the whole page, to be able to search it by regex
                string URL = imdbTitleSearchURL + HttpUtility.UrlEncode(title);
                StreamReader sr = new StreamReader(new WebClient().OpenRead(URL));
                _FileContent = sr.ReadToEnd();

                htmlCodes = new HTMLCodes();

                //Media media = new Media();

                if(media.Title == null || media.Title.Length == 0)
                    media.Title = title;

                return getInfo(media, bestMatch, media.MediaType);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);

                //Media media = new Media();
                if (media.Title == null || media.Title.Length == 0)
                    media.Title = title;

                return media;
            }
        }

        public Media GetEpisodeInfo(Media media)
        {
            try
            {
                if (media.SeriesIMDBNum != null && media.SeriesIMDBNum.Length > 0 && media.Season != null && media.Season.Length > 0 && media.EpisodeNum != null && media.EpisodeNum.Length > 0)
                {
                    //download the whole page
                    //string URL = imdbTitleSearchURL + HttpUtility.UrlEncode(title);
                    string URL = "http://www.imdb.com/title/" + media.SeriesIMDBNum + "/episodes?season=" + media.Season;
                    StreamReader sr = new StreamReader(new WebClient().OpenRead(URL));
                    _FileContent = sr.ReadToEnd();

                    htmlCodes = new HTMLCodes();

                    media.PreviousTitle = media.Title;

                    string currentEpisodeInfo = findValue(_FileContent, "<meta itemprop=\"episodeNumber\" content=\"" + media.EpisodeNum + "\"/>", "<div class=\"clear\">");

                    if (currentEpisodeInfo.Trim().Length > 0)
                    {
                        media.Title = getEpisodeTitle(currentEpisodeInfo);
                        media.Description = getEpisodeDescription(currentEpisodeInfo);
                        media.ReleaseYear = getEpisodeAirDate(currentEpisodeInfo);
                        media.IMDBNum = getEpisodeIMDBNum(currentEpisodeInfo);

                        int episodeNum = -1;
                        int.TryParse(media.EpisodeNum, out episodeNum);

                        if (episodeNum > -1)
                        {
                            currentEpisodeInfo = _FileContent.Split(new string[] { "<img width=\"200\" height=\"112\"" }, StringSplitOptions.RemoveEmptyEntries)[episodeNum];
                            media.coverImage = getPhoto(currentEpisodeInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return media;
        }

        private string getEpisodeTitle(string stringToSearch)
        {
            string returnString = "";

            try
            {
                returnString = findValue(stringToSearch, "title=\"", "\"", false);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return returnString.Trim();
        }

        private string getEpisodeAirDate(string stringToSearch)
        {
            string returnString = "";

            try
            {
                returnString = findValue(stringToSearch, "<div class=\"airdate\">", "</div>", false);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return returnString.Trim();
        }

        private string getEpisodeDescription(string stringToSearch)
        {
            string returnString = "";

            try
            {
                returnString = findValue(stringToSearch, "itemprop=\"description\">", "</div>", false);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return returnString.Trim();
        }

        private string getEpisodeIMDBNum(string stringToSearch)
        {
            string returnString = "";

            try
            {
                returnString = findValue(stringToSearch, "<a href=\"/title/", "/?", false);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return returnString.Trim();
        }

        /// <summary>
        /// Downloads the poster thumbnail of the movie, and saves it in the Images folder
        /// </summary>
        /// <returns>The filename of the downloaded image</returns>
        /// <remarks></remarks>
        public string getPhoto(Media media)
        {
            string photoPath = "";

            try
            {
                //string MovieName = media.Title.Split(' ')[0];
                string tempString = "";
                int startString = 0;
                int endString = 0;
                string imageURL = "";

                tempString = findValue(_FileContent, "<div class=\"poster\">", ".jpg", true);
                tempString = findValue(tempString, "src=", ".jpg", true);

                //startString = _FileContent.IndexOf("<img src=\"http://ia.media-imdb.com/images/");

                //tempString = _FileContent.Substring(startString);

                //endString = tempString.IndexOf(".jpg\"") + 4;

                //tempString = tempString.Substring(0, endString);

                imageURL = tempString.Replace("src=", "");
                imageURL = imageURL.Replace("\"", "");

                tempString = tempString.Substring(tempString.LastIndexOf("/") + 1);

                //if (tempString.ToLower().Contains("poster"))//this is the photo we are looking for
                //{
                //    tempString = tempString.Substring(tempString.IndexOf("http://ia.media-imdb.com/images"));
                //    tempString = tempString.Substring(0, tempString.IndexOf(".jpg"));

                photoPath = tempString;
                //}            


                //find the img tag containing the poster in the page
                //string RegExPattern = "<img [^\\>]* " + photoPath +
                //    ". [^\\>]* src \\s* = \\s* [\\\"\\']? ( [^\\\"\\'\\s>]* )";

                //Regex R1 = new Regex(RegExPattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //MatchCollection matches = R1.Matches(_FileContent);

                ////find the link in the img tag and download the image and save it in the images folder
                //Regex R2 = new Regex("http.{0,}",
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //if (matches.Count > 0)
                //{
                //    Match posterImageUrl = R2.Match(matches[0].Value);
                _coverImage = System.Windows.Forms.Application.StartupPath + "\\images\\media\\coverImages\\" + photoPath;
                photoPath = System.Windows.Forms.Application.StartupPath + "\\images\\media\\coverImages\\" + photoPath;

                if (!File.Exists(photoPath) && imageURL.Trim().Length > 0)
                {
                    System.Drawing.Image posterImage = System.Drawing.Image.FromStream(new WebClient().OpenRead(imageURL));
                    //    string imageName = media.IMDBNum;

                    //    if (imageName.Trim().Length == 0)
                    //        imageName = media.filename;



                    SaveJpeg(photoPath, posterImage, 100);

                    _thumbnail = posterImage;
                }
                //}
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return photoPath;
        }

        public string getPhoto(string stringToSeach)
        {
            string photoPath = "";

            try
            {
                string tempString = "";
                string imageURL = "";

                tempString = findValue(stringToSeach, "class=\"zero-z-index\"", "\">", false);
                tempString = findValue(tempString, "src=\"", ".jpg", true);

                imageURL = tempString.Replace("src=\"", "");

                tempString = tempString.Substring(tempString.LastIndexOf("/") + 1);

                photoPath = tempString;

                _coverImage = System.Windows.Forms.Application.StartupPath + "\\images\\media\\coverImages\\" + photoPath;
                photoPath = System.Windows.Forms.Application.StartupPath + "\\images\\media\\coverImages\\" + photoPath;

                if (!File.Exists(photoPath))
                {
                    System.Drawing.Image posterImage = System.Drawing.Image.FromStream(new WebClient().OpenRead(imageURL));

                    SaveJpeg(photoPath, posterImage, 100);

                    _thumbnail = posterImage;
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return photoPath;
        }

        /// <summary>
        /// Retrieve fields from IMDB, run this sub before using the properites
        /// </summary>
        /// <remarks></remarks>
        public Media getInfo(Media media, bool bestMatch, string mediaType)
        {
            Media mediaResult = null;

            try
            {
                string matchContent = "";
                ArrayList searchResults = new ArrayList();

                //scrape the movie title
                _MovieTitle = getTitle(_FileContent, true);

                if (_MovieTitle == "IMDb Search" || _MovieTitle == "Find - IMDb")//there were multiple results
                {
                    if (bestMatch)
                    {
                        ArrayList matches = getMultipleMatchTitles(_FileContent);

                        if (matches.Count > 0)
                        {
                            string searchTitle = MediaHandler.FormatNameString(media.Title).ToLower();
                            searchTitle = searchTitle.Replace(" ", "");

                            foreach (Media foundMedia in matches)
                            {
                                //try to match the title with search string
                                string tempTitle = MediaHandler.FormatNameString(foundMedia.Title).ToLower();
                                tempTitle = tempTitle.Replace(" ", "");

                                //look for the entire title and media type
                                if (tempTitle.Contains(searchTitle) || searchTitle.Contains(tempTitle))
                                {
                                    if (foundMedia.Season == null || foundMedia.Season.Length == 0)
                                        foundMedia.Season = media.Season;

                                    if (foundMedia.EpisodeNum == null || foundMedia.EpisodeNum.Length == 0)
                                        foundMedia.EpisodeNum = media.EpisodeNum;

                                    if (mediaResult == null)
                                        mediaResult = foundMedia;
                                    else //check multiple match results for mediatype match to make sure we get the best match
                                    {
                                        if (mediaType != null && foundMedia.MediaType != null && foundMedia.MediaType.ToLower() == mediaType.ToLower())
                                            mediaResult = foundMedia;
                                    }

                                    if(mediaType != null && mediaResult.MediaType != null && mediaResult.MediaType.ToLower() == mediaType.ToLower())
                                        break;
                                }
                            }

                            //didn't find match - look for half the title
                            if (mediaResult == null)
                            {
                                foreach (Media foundMedia in matches)
                                {
                                    string tempTitle = MediaHandler.FormatNameString(foundMedia.Title).ToLower();
                                    tempTitle = tempTitle.Replace(" ", "");

                                    if (tempTitle.Contains(searchTitle.Substring(0, searchTitle.Length / 2)))
                                    {
                                        mediaResult = foundMedia;
                                        break;
                                    }
                                }
                            }

                            //pick the first one since we didn't find a match
                            if (mediaResult == null)
                                mediaResult = (Media)matches[0];

                            mediaResult = getInfoByNumber(mediaResult);
                        }
                        //else
                        //{
                        //    //didn't find match - look for half the title
                        //    string tempTitle = MediaHandler.FormatNameString(media.Title).ToLower();
                        //    string tempTitle2 = tempTitle;

                        //    if (tempTitle.IndexOf(" ") > 0)
                        //    {
                        //        tempTitle2 = tempTitle.Substring(0, tempTitle.IndexOf(" "));
                        //        tempTitle2 = tempTitle.Substring(0, tempTitle.IndexOf(" ") + tempTitle2.Length + 3);

                        //        string tempTitle3 = tempTitle.Replace(tempTitle2, "").Trim();

                        //        if(tempTitle3.IndexOf(" ") > 0)
                        //        {
                        //            tempTitle3 = tempTitle3.Substring(0, tempTitle3.IndexOf(" "));
                        //            tempTitle = tempTitle.Substring(0, tempTitle2.Length + tempTitle3.Length + 1);
                        //        }
                        //    }
 
                        //    mediaResult = getInfoByTitle(tempTitle, bestMatch);
                        //}
                    }
                    else
                    {
                        searchResults = getMultipleMatchTitles(_FileContent);

                        //display results
                        MultipleMatches multipleMatches = new MultipleMatches(searchResults);
                        multipleMatches.StartPosition = FormStartPosition.CenterScreen;
                        DialogResult result = multipleMatches.ShowDialog();
                        multipleMatches.BringToFront();

                        if (result == DialogResult.OK)
                            mediaResult = multipleMatches.MediaResult;
                    }

                    ////get exact matches
                    //string exactMatchPattern = @"Titles\ \(Exact\ Matches\).*?</p>";
                    ////string exactMatchPattern = @"title_exact.*?</td>";
                    //Regex R1 = new Regex(exactMatchPattern,
                    //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                    ////get popular titles
                    //string popularMatchPattern = @"Popular\ Titles.*?</p>";
                    //Regex R2 = new Regex(popularMatchPattern,
                    //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                    ////if(R2.Matches(_FileContent).Count > 0)
                    ////    matchContent = R2.Matches(_FileContent)[0].Value;

                    //if (R2.Matches(_FileContent).Count > 0)
                    //    searchResults.AddRange(getMultipleMatchTitles(R2.Matches(_FileContent)[0].Value,0));

                    ////if (R1.Matches(_FileContent).Count > 0)
                    ////    matchContent += R1.Matches(_FileContent)[0].Value;

                    //for (int x = 0; x < R1.Matches(_FileContent).Count; x++)
                    //    searchResults.AddRange(getMultipleMatchTitles(R1.Matches(_FileContent)[x].Value, x));




                    //if (matchContent.Trim().Length > 0)
                    //{
                    //    //display matches
                    //    mediaResult = displayMultipleMatches(matchContent, bestMatch);//the first one is the popular match
                    //}
                    //else //if (mediaResult == null)// || mediaResult.filePath == null || mediaResult.filePath.Length == 0)
                    //{
                    //    //find the results and their movie number and let user choose which is correct
                    //    string paragraphPattern = "<p>.*</p>";
                    //    R1 = new Regex(paragraphPattern,
                    //        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                    //    if (R1.Matches(_FileContent).Count > 0)
                    //    {
                    //        //display matches
                    //        mediaResult = displayMultipleMatches(R1.Matches(_FileContent)[0].Value, bestMatch);//the first one is the popular match
                    //    }
                    //    else
                    //        _MovieTitle = "No Results";
                    //}
                }
                else
                {
                    mediaResult = new Media();
                    mediaResult.Title = _MovieTitle.Trim();

                    //scrape the director
                    _Director = getDirectors(_FileContent);
                    mediaResult.Director = HttpUtility.UrlDecode(HttpUtility.UrlEncode(_Director).Replace("%0a", "")).Trim();

                    //scrape release year
                    _ReleaseYear = getReleaseYear(_FileContent);
                    mediaResult.ReleaseYear = _ReleaseYear;

                    //scrape movie genre
                    _Genre = getGenre(_FileContent);
                    mediaResult.category = _Genre;

                    //scrape movie tagline
                    _TagLine = getTagline(_FileContent);
                    mediaResult.TagLine = HttpUtility.UrlDecode(HttpUtility.UrlEncode(_TagLine).Replace("%0a", "")).Trim();

                    //scrape movie description
                    _description = getDescription(_FileContent);
                    mediaResult.Description = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlEncode(_description).Replace("%0a", "")).Trim();

                    //scrape rating
                    _rating = getRating(_FileContent);
                    mediaResult.Rating = _rating;

                    //scrape rating description
                    _ratingDescription = getRatingDescription(_FileContent);
                    mediaResult.RatingDescription = _ratingDescription;

                    //get mediaType from title
                    mediaResult.MediaType = getMediaType(getTitle(_FileContent,false));

                    if (_imdbNum != null && _imdbNum.Trim().Length > 0)
                        mediaResult.IMDBNum = _imdbNum;
                    else
                    {
                        mediaResult.IMDBNum = getIMDBNum(_FileContent);
                        _imdbNum = mediaResult.IMDBNum;
                    }

                    mediaResult.Performers = getStarPerformers(_FileContent);

                    mediaResult.coverImage = getPhoto(mediaResult);

                    mediaResult.Goofs = getGoofs(_FileContent);

                    mediaResult.Trivia = getTrivia(_FileContent);

                    mediaResult.ShortDescription = getShortDescription(_FileContent);

                    mediaResult.Season = media.Season;

                    mediaResult.EpisodeNum = media.EpisodeNum;

                    mediaResult.SeriesName = media.SeriesName;

                    mediaResult.SeriesIMDBNum = media.SeriesIMDBNum;

                    mediaResult.SeriesImage = media.SeriesImage;

                    mediaResult.SeriesDescription = media.SeriesDescription;

                    _goofs = mediaResult.Goofs;
                    _trivia = mediaResult.Trivia;
                    _shortDescription = mediaResult.ShortDescription;
                    _stars = mediaResult.Stars;
                }

                //if (mediaResult.MediaType == "TV" && mediaResult.SeriesName.Length > 0)//seriesName has already been set so we should be on the episode list page
                //{
                //    mediaResult.Season = getSeason(_FileContent);
                //    mediaResult.Episode = getEpisode(_FileContent);
                //}

                if (mediaResult != null && mediaResult.MediaType != null && mediaResult.MediaType == "TV")
                {
                    if (mediaResult.SeriesName == null || mediaResult.SeriesName.Length == 0)
                        mediaResult.SeriesName = mediaResult.Title;

                    if (mediaResult.SeriesIMDBNum == null || mediaResult.SeriesIMDBNum.Length == 0)
                        mediaResult.SeriesIMDBNum = mediaResult.IMDBNum;

                    if (mediaResult.SeriesImage == null || mediaResult.SeriesImage.Length == 0)
                        mediaResult.SeriesImage = mediaResult.coverImage;

                    if (mediaResult.Season == null || mediaResult.Season.Length == 0)
                        mediaResult.Season = media.Season;

                    if (mediaResult.SeriesDescription == null || mediaResult.SeriesDescription.Length == 0)
                        mediaResult.SeriesDescription = mediaResult.Description;
                }

                mediaResult.ID = media.ID;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            if (mediaResult == null)
                mediaResult = media;
            
            return mediaResult;
        }

        /// <summary>
        /// find and display multiple matches
        /// </summary>
        /// <param name="matchContent"></param>
        /// <returns></returns>
        private Media displayMultipleMatches(string matchContent, bool bestMatch)
        {
            try
            {
                //find the imdbNumber and titles and display
                ArrayList searchResults = new ArrayList();
                string foundImdbNum = "";
                Media foundMatch = null;
                Media selectedMedia = null;
                int matchIndex = 0;

                //string linkPattern = @"(<a.*?>.*?</a>)";
                string linkPattern = @"find-title-.*?/title_popular/images/b.gif.*?</td>";//popular title match
                Regex R1 = new Regex(linkPattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                if (R1.Matches(matchContent).Count == 0)
                {
                    linkPattern = @"find-title-.*?</td></tr></table>";

                    R1 = new Regex(linkPattern,
                        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                }

                if (R1.Matches(matchContent).Count == 0)
                {
                    linkPattern = @"find-title-.*?<br>&#160;";

                    R1 = new Regex(linkPattern,
                        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                }

                foreach (Match match in R1.Matches(matchContent))
                {
                    Media media = new Media();

                    media.IMDBNum = getIMDBNum(match.Groups[0].Value);

                    foundImdbNum = media.IMDBNum;

                    media.Title = getMultipleMatchTitle(matchContent, matchIndex);

                    if (media.Title.Trim().Length > 0)
                    {
                        searchResults.Add(media);

                        if (bestMatch)
                            break;
                    }

                    matchIndex++;
                }

                if (bestMatch)
                {
                    if (searchResults.Count > 0)
                    {
                        selectedMedia = (Media)searchResults[0];

                        //get info
                        if (foundImdbNum != null && foundImdbNum.Length > 0)
                            foundMatch = getInfoByNumber(selectedMedia);
                    }
                }
                else
                {
                    //display results
                    MultipleMatches multipleMatches = new MultipleMatches(searchResults);
                    multipleMatches.StartPosition = FormStartPosition.CenterScreen;
                    DialogResult result = multipleMatches.ShowDialog();

                    if (result == DialogResult.OK)
                        foundMatch = multipleMatches.MediaResult;
                }

                return foundMatch;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);

                return null;
            }
        }

        private string getMediaType(string stringToSearch)
        {
            string foundMediaType = "";

            try
            {
                if (stringToSearch.ToLower().Contains("tv series") || stringToSearch.ToLower().Contains("(tv)"))
                    foundMediaType = "TV";

                if (foundMediaType == "")
                    foundMediaType = "Movies";
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return foundMediaType;
        }

        private string getTitle(string content, bool cleanTheTitle)
        {
            string title = "";

            try
            {
                string titlePattern = "<title>.*</title>";

                if (titleMatch != null)
                    titlePattern = titleMatch;

                Regex R1 = new Regex(titlePattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                if (R1.Matches(content).Count > 0)
                {
                    title = R1.Matches(content)[0].Value;

                    //remove the beginning title tag <title>
                    title = title.Substring(7);

                    //remove the ending title tag </title>
                    title = title.Substring(0, title.Length - 8);
                }

                if(cleanTheTitle && title.IndexOf("(") > 0)
                    title = title.Substring(0, title.IndexOf("("));
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(title);
        }

        private string getMultipleMatchTitle(string content, int matchIndex)
        {
            string title = "";

            try
            {
                //first look for an exact title match
                string titlePattern = "find-title-" + (matchIndex + 1) + "/title_exact/images/b.gif.*?</td></tr></table>";
                Regex R1 = new Regex(titlePattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                if (R1.Matches(content).Count == 0)//now look for a popular match
                {
                    titlePattern = "title_popular/images/b.gif.*?<br>";
                    R1 = new Regex(titlePattern,
                        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                }

                if (R1.Matches(content).Count == 0)//try a second pattern for popular match
                {
                    titlePattern = "/find-title-" + (matchIndex + 1) + "/.*?</td></tr></table>";

                    R1 = new Regex(titlePattern,
                        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                }

                if (R1.Matches(content).Count > 0)
                {
                    title = R1.Matches(content)[0].Value;

                    //remove the beginning link tags
                    title = title.Substring(title.IndexOf(">") + 1);

                    //remove the ending tags
                    //title = title.Substring(0, title.Length - 4);
                    title = title.Substring(0, title.IndexOf("    "));

                    title = title.Replace("</a>", "");

                    title = htmlCodes.ToText(title).Trim();
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return title;
        }

        private ArrayList getMultipleMatchTitles(string content)
        {
            string title = "";
            ArrayList foundMatches = new ArrayList();

            try
            {
                string titlePattern = "";

                //first look for popular matches
                if (popularTitleMatch != null)
                    titlePattern = popularTitleMatch;
                else
                    titlePattern = "title_popular.*?<p";

                Regex R1 = new Regex(titlePattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                foreach (Match match in R1.Matches(content))
                {
                    title = match.Value.Substring(0,match.Value.IndexOf("</a>"));

                    if (title.Contains("     "))
                        title = title.Substring(0, title.IndexOf("     "));

                    Media tempMedia = new Media();
                    tempMedia.Title = htmlCodes.ToText(title).Trim();
                    tempMedia.Title = tempMedia.Title.Replace("<p", "");
                    tempMedia.IMDBNum = getIMDBNum(title);
                    tempMedia.ReleaseYear = getReleaseYear(content);

                    if (tempMedia.ReleaseYear.Trim().Length == 0 && match.Value.Contains("(") && match.Value.Contains(")"))
                        tempMedia.ReleaseYear = match.Value.Substring(match.Value.IndexOf("(") + 1, match.Value.IndexOf(")") - match.Value.IndexOf("(") - 1);

                    if ((tempMedia.MediaType == null || tempMedia.MediaType.Length == 0) && match.Value.Contains("(TV Series)"))
                        tempMedia.MediaType = "TV";
                    else
                        tempMedia.MediaType = "Movies";

                    foundMatches.Add(tempMedia);
                }

                //now look for exact title matches
                if(exactTitleMatch !=null)
                    titlePattern = exactTitleMatch;
                else
                    titlePattern = "title_exact.*?<p";

                R1 = new Regex(titlePattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                foreach (Match match in R1.Matches(content))
                {
                    title = match.Value;

                    if (title.Contains("      "))
                        title = title.Substring(0, title.IndexOf("      "));

                    Media tempMedia = new Media();
                    tempMedia.Title = htmlCodes.ToText(title).Trim();
                    tempMedia.IMDBNum = getIMDBNum(match.Value);
                    foundMatches.Add(tempMedia);
                }

                //now look for partial matches
                if (partialTitleMatch != null)
                    titlePattern = partialTitleMatch;
                else
                    titlePattern = "title_substring.*?<p";

                R1 = new Regex(titlePattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                foreach (Match match in R1.Matches(content))
                {
                    title = match.Value;

                    if (title.Contains("      "))
                        title = title.Substring(0, title.IndexOf("      "));

                    Media tempMedia = new Media();
                    tempMedia.Title = htmlCodes.ToText(title).Trim();
                    tempMedia.IMDBNum = getIMDBNum(match.Value);
                    foundMatches.Add(tempMedia);
                }

                //now look for aprox matches
                //if (partialTitleMatch != null)
                //    titlePattern = partialTitleMatch;
                //else
                    titlePattern = "title_approx.*?<div";

                R1 = new Regex(titlePattern,
                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                foreach (Match match in R1.Matches(content))
                {
                    title = match.Value;

                    if (title.Contains("      "))
                        title = title.Substring(0, title.IndexOf("      "));

                    Media tempMedia = new Media();
                    tempMedia.Title = htmlCodes.ToText(title).Trim();
                    tempMedia.IMDBNum = getIMDBNum(match.Value);
                    foundMatches.Add(tempMedia);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return foundMatches;
        }

        private string getIMDBNum(string content)
        {
            string foundImdbNum = "";

            try
            {
                //string imdbPattern = @"link=/title/tt.*?/";
                string imdbPattern = @"/title/tt.*?/";

                Match m2 = Regex.Match(content, imdbPattern,
                    RegexOptions.Singleline);

                if (m2.Success)
                {
                    string theLink = m2.Groups[0].Value;

                    //need to parse and get movie number
                    foundImdbNum = theLink.Replace("/title/", "");
                    foundImdbNum = foundImdbNum.Replace("/", "");
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            if (foundImdbNum.Contains("<") || foundImdbNum.Contains(">"))
                foundImdbNum = "";

            return htmlCodes.ToText(foundImdbNum);
        }

        private string getDirectors(string content)
        {
            string director = "";

            try
            {
                //string directorPattern = "Director[s]*:.* [^\\>]* \\s*> \\s* [^\\<]* ";

                //if (directorMatch != null)
                //    directorPattern = directorMatch;

                //Regex R1 = new Regex(directorPattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //if (R1.Matches(content).Count > 0)
                //{
                //    director = R1.Matches(content)[0].Value;
                //    director = (director.Split('>')[2]).Trim();
                //}

                director = findValue(content, "<h4 class=\"inline\">Director:</h4>", "</div>").Trim();

                if(director.Length == 0)
                    director = findValue(content, "<h4 class=\"inline\">Directors:</h4>", "</div>").Trim();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(director);
        }

        private string getStarPerformers(string content)
        {
            string stars = "";

            try
            {
                //string directorPattern = "Director[s]*:.* [^\\>]* \\s*> \\s* [^\\<]* ";

                //if (directorMatch != null)
                //    directorPattern = directorMatch;

                //Regex R1 = new Regex(directorPattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //if (R1.Matches(content).Count > 0)
                //{
                //    director = R1.Matches(content)[0].Value;
                //    director = (director.Split('>')[2]).Trim();
                //}

                stars = findValue(content, "Stars:", "<span class=\"ghost\">").Trim();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(stars);
        }

        private string getIMDBRating(string content)
        {
            string rating = "";

            try
            {
                rating = findValue(content, "<span class=\"rating-rating\">", "</span></span>");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(rating);
        }

        private string getReleaseYear(string content)
        {
            string releaseYear = "";

            try
            {
                //string releaseYearPattern = "Date:.* [^\\>]* \\s*> \\s* [^\\<]* ";

                //if (releaseYearMatch != null)
                //    releaseYearPattern = releaseYearMatch;

                //Regex R1 = new Regex(releaseYearPattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //if (R1.Matches(content).Count > 0)
                //{
                //    releaseYear = R1.Matches(content)[0].Value;
                //    R1 = new Regex("\\d{4,4}",
                //        RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //    releaseYear = R1.Matches(releaseYear)[0].Value.Trim();
                //}

                releaseYear = findValue(content, "<meta itemprop=\"datePublished\" content=\"", " />");
                releaseYear = releaseYear.Replace("<meta itemprop=\"datePublished\" content=\"", "");
                releaseYear = releaseYear.Replace(" />", "");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(releaseYear).Trim();
        }

        private string getGenre(string content)
        {
            string genre = "";
            string[] genreArray;

            try
            {
                genre = findValue(content, "<span class=\"itemprop\" itemprop=\"genre\">", "</div>", true);
                genre = genre.Replace("\n", "");

                if (genre.Contains("See more release dates"))
                    genre = genre.Substring(0, genre.IndexOf("<span class=\"ghost\">"));

                genre = htmlCodes.ToText(genre);

                if (genre.Contains(","))
                {
                    //string startString = "";
                    genreArray = genre.Split(',');
                    genre = "";

                    foreach (string value in genreArray)
                    {
                        if (genre.Length > 0)
                        {
                            genre += "|";
                            //startString = @"/Sections/Genres/";
                        }
                        //else
                        //    startString = "       >";

                        //genre += findValue(value, startString, "</a>");
                        
                        genre += value.Trim();
                    }
                }

                //string genrePattern = "Sections/Genres.*";
                //Regex R1 = new Regex(genrePattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //if (R1.Matches(content).Count > 0)
                //{
                //    genre = R1.Matches(content)[0].Value;

                //    MatchCollection genreResults = R1.Matches(content);
                //    string[] genreArray = genreResults[0].ToString().Split('|');

                //    for (int C = 0; C <= genreArray.Length - 1; C++)
                //    {
                //        string seperater = "";

                //        if ((C % 2 != 0) & (genreArray[C].Contains("more") == false))
                //        {
                //            if (genre != "" & genre != null)
                //                seperater = " / ";
                //            genre += seperater + genreArray[C].Substring(0, genreArray[C].Length - 3);
                //        }
                //    }
                //}

                //genre = genre.Replace("Genre:</h5> / ", "");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(genre);
        }

        private string getTagline(string content)
        {
            string tagline = "";

            try
            {
                //string taglinePattern = "Taglines:.*<span class=\"see - more inline\">";
                tagline = findValue(content, "Taglines:</h4>", "<", false);

                //string tagLine = tempTagline.Split(new char[] { '>', '<' })[2];
                tagline = tagline.Replace("\n","").Trim();

                //if (taglineMatch != null)
                //    taglinePattern = taglineMatch;

                //Regex R1 = new Regex(taglinePattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //if (R1.Matches(content).Count > 0)
                //{
                //    string tagLine = R1.Matches(content)[0].Value;
                //    tagLine = tagLine.Split(new char[] { '>', '<' })[2];
                //    tagline = tagLine.Trim();
                //}
                //else
                //{
                //    tagline = "";
                //}
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(tagline);
        }

        private string getShortDescription(string content)
        {
            string shortDescription = "";

            try
            {
                shortDescription = findValue(content, "<div class=\"summary_text\" itemprop=\"description\">", "</div>").Trim();
                shortDescription = shortDescription.Replace("See more  &raquo;", "");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(shortDescription);
        }

        private string getDescription(string content)
        {
            string description = "";
            
            try
            {
                description = findValue(content, "<h2>Storyline</h2>", "</p>");
                string tempDescription = findValue(description, "<p>", "<");

                if (tempDescription.Trim().Length == 0)
                    tempDescription = findValue(description, " ", "<");
                
                description = tempDescription.Trim();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(description);
        }

        private string getGoofs(string content)
        {
            string goofs = "";

            goofs = findValue(content, "<h4>Goofs</h4>", "<a").Trim();

            return htmlCodes.ToText(goofs);
        }

        //private string getGoofs(string imdbNumber)
        //{
        //    string goofs = "";

        //    try
        //    {
        //        //download the whole page, to be able to search it
        //        StreamReader sr = new StreamReader(new WebClient().OpenRead("http://www.imdb.com/title/"+ imdbNumber +"/goofs"));
        //        string goofContent = sr.ReadToEnd();


        //        goofs = findValue(goofContent, "<ul class=\"trivia\">", "<hr/>");

        //        goofs = htmlCodes.ToText(goofs);
        //    }
        //    catch (Exception ex)
        //    {
        //        Tools.WriteToFile(ex);

        //        Tools.WriteToFile(Tools.errorFile, "url to search: http://www.imdb.com/title/" + imdbNumber + "/goofs");
        //    }

        //    return goofs;
        //}

        private string getTrivia(string content)
        {
            string trivia = "";

            try
            {
                trivia = findValue(content, "<h4>Trivia</h4>", "</div>").Trim();

                ////download the whole page, to be able to search it
                //StreamReader sr = new StreamReader(new WebClient().OpenRead("http://www.imdb.com/title/" + imdbNumber + "/trivia"));
                //string triviaContent = sr.ReadToEnd();

                //trivia = findValue(triviaContent, " class=\"soda\">", "<!-- sid: t-channel : MIDDLE_CENTER -->");

                //trivia = htmlCodes.ToText(trivia);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "getTrivia() error: "+ ex.Message);
            }

            return htmlCodes.ToText(trivia);
        }

        private string getRating(string content)
        {
            string rating = "";

            try
            {
                //string ratingPattern = "<span itemprop=\"contentRating\">.*</span>";
                rating = findValue(content, "<meta itemprop=\"contentRating\" content=\"", "\"");

                if (rating.Trim().Length == 0)
                {
                    rating = findValue(content, "<span itemprop=\"contentRating\">Rated ", "</span>", false);

                    if (rating.IndexOf(" for") > 0)
                        rating = rating.Substring(0, rating.IndexOf(" for")).Trim();
                }

                //if (ratingMatch != null)
                //    ratingPattern = ratingMatch;

                //Regex R1 = new Regex(ratingPattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

                //if (R1.Matches(content).Count > 0)
                //{
                //    rating = R1.Matches(content)[0].Value;

                //    //remove all lines before rating
                //    //rating = rating.Substring(rating.IndexOf("\nRated") + 6).Trim();

                //    //remove all lines after rating
                //    rating = rating.Substring(0, rating.IndexOf(" for")).Trim();
                //}
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(rating);
        }

        private string getRatingDescription(string content)
        {
            string ratingDescription = "";

            try
            {
                //string ratingDescriptionPattern = "<span itemprop=\"contentRating\">.*</span>";
                ratingDescription = findValue(content, "<span itemprop=\"contentRating\">Rated ", "</span>", false);

                if(ratingDescription.IndexOf(" for") > 0)
                    ratingDescription = ratingDescription.Substring(ratingDescription.IndexOf(" for ") + 5);

                //if (ratingDescriptionMatch != null)
                //    ratingDescriptionPattern = ratingDescriptionMatch;

                //Regex R1 = new Regex(ratingDescriptionPattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

                //if (R1.Matches(content).Count > 0)
                //{
                //    ratingDescription = R1.Matches(content)[0].Value;

                //    //remove all lines before rating
                //    //ratingDescription = ratingDescription.Substring(ratingDescription.IndexOf("\nRated") + 6).Trim();

                //    //find reason
                //    ratingDescription = ratingDescription.Substring(ratingDescription.IndexOf(" for ") + 5);
                //    //ratingDescription = ratingDescription.Substring(0, ratingDescription.IndexOf("\n</div>")).Trim();
                //}
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(ratingDescription);
        }

        private string getEpisode(string content)
        {
            string stars = "";

            try
            {
                //string directorPattern = "Director[s]*:.* [^\\>]* \\s*> \\s* [^\\<]* ";

                //if (directorMatch != null)
                //    directorPattern = directorMatch;

                //Regex R1 = new Regex(directorPattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //if (R1.Matches(content).Count > 0)
                //{
                //    director = R1.Matches(content)[0].Value;
                //    director = (director.Split('>')[2]).Trim();
                //}

                stars = findValue(content, "Stars:", "</div>");
                stars = stars.Replace("  ", "");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(stars);
        }

        private string getSeason(string content)
        {
            string stars = "";

            try
            {
                //string directorPattern = "Director[s]*:.* [^\\>]* \\s*> \\s* [^\\<]* ";

                //if (directorMatch != null)
                //    directorPattern = directorMatch;

                //Regex R1 = new Regex(directorPattern,
                //    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

                //if (R1.Matches(content).Count > 0)
                //{
                //    director = R1.Matches(content)[0].Value;
                //    director = (director.Split('>')[2]).Trim();
                //}

                stars = findValue(content, "Stars:", "</div>");
                stars = stars.Replace("  ", "");
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return htmlCodes.ToText(stars);
        }

        /// <summary> 
        /// Saves an image as a jpeg image, with the given quality 
        /// </summary> 
        /// <param name="path">Path to which the image would be saved.</param> 
        // <param name="quality">An integer from 0 to 100, with 100 being the 
        /// highest quality</param> 
        public static void SaveJpeg(string path, System.Drawing.Image img, int quality) 
        {
            try
            {
                if (!File.Exists(path))
                {
                    if (quality < 0 || quality > 100)
                        throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

                    //Image newImage = new Image();
                    //newImage = img;

                    // Encoder parameter for image quality 
                    EncoderParameter qualityParam =
                        new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                    // Jpeg image codec 
                    ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

                    EncoderParameters encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = qualityParam;

                    img.Save(path, jpegCodec, encoderParams);
                    //img.Save(path);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }
        } 

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        private static ImageCodecInfo GetEncoderInfo(string mimeType) 
        {
            try
            {
                // Get image codecs for all image formats 
                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

                // Find the correct image codec 
                for (int i = 0; i < codecs.Length; i++)
                    if (codecs[i].MimeType == mimeType)
                        return codecs[i];
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return null; 
        }

        private string findValue(string stringToParse, string startPattern, string endPattern)
        {
            return findValue(stringToParse, startPattern, endPattern, false);
        }

        private string findValue(string stringToParse, string startPattern, string endPattern, bool returnSearchPatterns)
        {
            int start = 0;
            int end = 0;
            string foundValue = "";

            try
            {
                start = stringToParse.IndexOf(startPattern);

                if (start > -1)
                {
                    if (!returnSearchPatterns)
                        stringToParse = stringToParse.Substring(start + startPattern.Length);
                    else
                        stringToParse = stringToParse.Substring(start);

                    end = stringToParse.IndexOf(endPattern);

                    if (end > 0)
                    {
                        if (returnSearchPatterns)
                            foundValue = stringToParse.Substring(0, end + endPattern.Length);
                        else
                            foundValue = stringToParse.Substring(0, end);
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return foundValue;
        }
    }
}
