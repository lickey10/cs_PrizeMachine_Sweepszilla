using System;
using System.Data;
using System.Text;
using System.Xml;
using System.IO;
using Microsoft.Xml.XQuery;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Configuration;

namespace SCTVObjects
{
	/// <summary>
	/// Summary description for mediaHandler.
	/// </summary>
	public class MediaHandler
	{
		#region "Delegates"
		public delegate void mediaListChanged();
		public event mediaListChanged OnListChanged;
		#endregion

		#region variables
		public static DataSet dsMedia = new DataSet();
        public static DataSet dsGames = new DataSet();
        public static DataSet dsGameCategories = new DataSet();
        public static DataSet dsGameRatings = new DataSet();
        public static DataSet dsGamesToCategories = new DataSet();
        public DataSet dsImages = new DataSet();
		public DataSet dsFileTypes = new DataSet();
        public static DataSet dsMediaTypes = new DataSet();
		public string currentCategory="";
        private string currentMediaType = "";
        private string currentStartsWith = "";
		XmlDocument xmlFileTypes = new XmlDocument();
		XmlDocument xmlLocations = new XmlDocument();
		ArrayList currentMediaExtensions = new ArrayList();
        ArrayList locations = new ArrayList();
        static string mediaXMLFilePath = ""; //Application.StartupPath +"\\config\\media.xml";
        static string gamesXMLFilePath = ""; //Application.StartupPath +"\\config\\games.xml";
        static string mediaXMLFileDefaultPath = ""; //"config/media_default.xml";
        static string playlistXMLFilePath = ""; //Application.StartupPath + "\\config\\playlist.xml";
        static string playlistXMLDefaultPath = ""; //Application.StartupPath + "\\config\\playlist_default.xml";
        static string imagesXMLPath = ""; //Application.StartupPath + "\\config\\images.xml";
        static string imagesXMLDefaultPath = ""; //Application.StartupPath + "\\config\\images_default.xml";
        static string locationXMLFilePath = ""; //Application.StartupPath + "\\config\\locations.xml";
        static string fileTypeFilePath = ""; //Application.StartupPath + "\\config\\fileTypes.xml";
        static string mediaTypesPath = ""; //Application.StartupPath + "\\config\\onlineMedia.xml";
        static string settingsFilePath = "";
        static string defaultPathToSaveTo = ""; //Application.StartupPath + "\\DVD\\";
        static string defaultMediaType = "Movies";
        XmlDocument xmlSettings = null;
        bool ignoreIMDBErrors = false;
        public DataSet dsPlaylist = new DataSet();
        bool updatedLocations = false;

        public ArrayList Locations
        {
            get
            {
                return locations;
            }
        }

        public bool UpdatedLocations
        {
            get
            {
                return updatedLocations;
            }
        }

        public XmlDocument XMLSettings
        {
            get
            {
                try
                {
                    if (xmlSettings == null)
                    {
                        xmlSettings = new XmlDocument();
                        xmlSettings.Load(SettingsFilePath);
                    }
                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(Tools.errorFile, "XMLSettings: " + ex.Message);
                }

                return xmlSettings;
            }
        }

        public string MediaXMLFilePath
        {
            get
            {
                XmlNode MediaXMLFilePathNode = XMLSettings.SelectSingleNode("/settings/MediaXMLFilePath");

                mediaXMLFilePath = MediaXMLFilePathNode.InnerText;

                if (mediaXMLFilePath.Trim().Length == 0)
                    mediaXMLFilePath = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.MediaXMLFilePath"]);

                return mediaXMLFilePath;
            }

            set
            {
                mediaXMLFilePath = value;

                XmlNode baseNode;
                //XmlDocument xmlSettings = new XmlDocument();

                if (File.Exists(SettingsFilePath))
                    XMLSettings.Load(SettingsFilePath);
                else
                {
                    //add base node to tempDoc
                    baseNode = XMLSettings.CreateNode(XmlNodeType.Element, "settings", "");

                    XMLSettings.AppendChild(baseNode);
                }

                XmlNode xmlSettingNodeToUpdate = XMLSettings.SelectSingleNode("/settings/MediaXMLFilePath");
                xmlSettingNodeToUpdate.InnerText = value;
                XMLSettings.Save(SettingsFilePath);

            }
        }

        public string GamesXMLFilePath
        {
            get
            {
                XmlNode GamesXMLFilePathNode = XMLSettings.SelectSingleNode("/settings/GamesXMLFilePath");

                gamesXMLFilePath = GamesXMLFilePathNode.InnerText;

                if (gamesXMLFilePath.Trim().Length == 0)
                    gamesXMLFilePath = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.GamesXMLFilePath"]);

                return gamesXMLFilePath;
            }

            set
            {
                gamesXMLFilePath = value;

                XmlNode baseNode;
                //XmlDocument xmlSettings = new XmlDocument();

                if (File.Exists(SettingsFilePath))
                    XMLSettings.Load(SettingsFilePath);
                else
                {
                    //add base node to tempDoc
                    baseNode = XMLSettings.CreateNode(XmlNodeType.Element, "settings", "");

                    XMLSettings.AppendChild(baseNode);
                }

                XmlNode xmlSettingNodeToUpdate = XMLSettings.SelectSingleNode("/settings/GamesXMLFilePath");
                xmlSettingNodeToUpdate.InnerText = value;
                XMLSettings.Save(SettingsFilePath);
            }
        }

        public string SettingsFilePath
        {
            get
            {
                if (settingsFilePath.Trim().Length == 0)
                    settingsFilePath = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.SettingsFilePath"]);

                return settingsFilePath;
            }
        }

        public string GameXMLFilePath
        {
            get
            {
                XmlNode GameXMLFilePathNode = XMLSettings.SelectSingleNode("/settings/GameXMLFilePath");

                gamesXMLFilePath = GameXMLFilePathNode.InnerText;

                if (gamesXMLFilePath.Trim().Length == 0)
                    gamesXMLFilePath = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.GameXMLFilePath"]);

                return gamesXMLFilePath;
            }

            set
            {
                gamesXMLFilePath = value;

                if (File.Exists(SettingsFilePath))
                    XMLSettings.Load(SettingsFilePath);
                else
                {
                    XmlNode baseNode;

                    //add base node to tempDoc
                    baseNode = XMLSettings.CreateNode(XmlNodeType.Element, "settings", "");

                    XMLSettings.AppendChild(baseNode);
                }

                XmlNode xmlSettingNodeToUpdate = XMLSettings.SelectSingleNode("/settings/GameXMLFilePath");
                xmlSettingNodeToUpdate.InnerText = value;
                XMLSettings.Save(SettingsFilePath);

            }
        }

        public string MediaXMLFileDefaultPath
        {
            get
            {
                XmlNode mediaXMLFileDefaultPathNode = XMLSettings.SelectSingleNode("/settings/MediaXMLFileDefaultPath");

                mediaXMLFileDefaultPath = mediaXMLFileDefaultPathNode.InnerText;

                if (mediaXMLFileDefaultPath.Trim().Length == 0)
                    mediaXMLFileDefaultPath = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.MediaXMLFileDefaultPath"]);

                return mediaXMLFileDefaultPath;
            }

            set
            {
                mediaXMLFileDefaultPath = value;

                if (File.Exists(SettingsFilePath))
                    XMLSettings.Load(SettingsFilePath);
                else
                {
                    XmlNode baseNode;

                    //add base node to tempDoc
                    baseNode = XMLSettings.CreateNode(XmlNodeType.Element, "settings", "");

                    XMLSettings.AppendChild(baseNode);
                }

                XmlNode xmlSettingNodeToUpdate = XMLSettings.SelectSingleNode("/settings/MediaXMLFileDefaultPath");
                xmlSettingNodeToUpdate.InnerText = value;
                XMLSettings.Save(SettingsFilePath);
            }
        }

        public string PlaylistXMLFilePath
        {
            get
            {
                XmlNode playlistXMLFilePathNode = XMLSettings.SelectSingleNode("/settings/PlaylistXMLFilePath");

                playlistXMLFilePath = playlistXMLFilePathNode.InnerText;

                if (playlistXMLFilePath.Trim().Length == 0)
                    playlistXMLFilePath = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.PlaylistXMLFilePath"]);

                return playlistXMLFilePath;
            }

            set
            {
                playlistXMLFilePath = value;

                if (File.Exists(SettingsFilePath))
                    XMLSettings.Load(SettingsFilePath);
                else
                {
                    XmlNode baseNode;

                    //add base node to tempDoc
                    baseNode = XMLSettings.CreateNode(XmlNodeType.Element, "settings", "");

                    XMLSettings.AppendChild(baseNode);
                }

                XmlNode xmlSettingNodeToUpdate = XMLSettings.SelectSingleNode("/settings/PlaylistXMLFilePath");
                xmlSettingNodeToUpdate.InnerText = value;
                XMLSettings.Save(SettingsFilePath);
            }
        }

        public string PlaylistXMLDefaultPath
        {
            get
            {
                XmlNode playlistXMLDefaultPathNode = XMLSettings.SelectSingleNode("/settings/PlaylistXMLDefaultPath");

                playlistXMLDefaultPath = playlistXMLDefaultPathNode.InnerText;

                if (playlistXMLDefaultPath.Trim().Length == 0)
                    playlistXMLDefaultPath = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.PlaylistXMLDefaultPath"]);

                return playlistXMLDefaultPath;
            }

            set
            {
                playlistXMLDefaultPath = value;

                if (File.Exists(SettingsFilePath))
                    XMLSettings.Load(SettingsFilePath);
                else
                {
                    XmlNode baseNode;

                    //add base node to tempDoc
                    baseNode = XMLSettings.CreateNode(XmlNodeType.Element, "settings", "");

                    XMLSettings.AppendChild(baseNode);
                }

                XmlNode xmlSettingNodeToUpdate = XMLSettings.SelectSingleNode("/settings/PlaylistXMLDefaultPath");
                xmlSettingNodeToUpdate.InnerText = value;
                XMLSettings.Save(SettingsFilePath);
            }
        }

        public string ImagesXMLPath
        {
            get
            {
                XmlNode imagesXMLPathNode = XMLSettings.SelectSingleNode("/settings/ImagesXMLPath");

                imagesXMLPath = imagesXMLPathNode.InnerText;

                if (imagesXMLPath.Trim().Length == 0)
                    imagesXMLPath = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.ImagesXMLPath"]);

                return imagesXMLPath;
            }

            set
            {
                imagesXMLPath = value;

                if (File.Exists(SettingsFilePath))
                    XMLSettings.Load(SettingsFilePath);
                else
                {
                    XmlNode baseNode;

                    //add base node to tempDoc
                    baseNode = XMLSettings.CreateNode(XmlNodeType.Element, "settings", "");

                    XMLSettings.AppendChild(baseNode);
                }

                XmlNode xmlSettingNodeToUpdate = XMLSettings.SelectSingleNode("/settings/ImagesXMLPath");
                xmlSettingNodeToUpdate.InnerText = value;
                XMLSettings.Save(SettingsFilePath);
            }
        }

        public string ImagesXMLDefaultPath
        {
            get
            {
                XmlNode imagesXMLDefaultPathNode = XMLSettings.SelectSingleNode("/settings/ImagesXMLDefaultPath");

                imagesXMLDefaultPath = imagesXMLDefaultPathNode.InnerText;

                if (imagesXMLDefaultPath.Trim().Length == 0)
                    imagesXMLDefaultPath = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.ImagesXMLDefaultPath"]);

                return imagesXMLDefaultPath;
            }

            set
            {
                imagesXMLDefaultPath = value;

                if (File.Exists(SettingsFilePath))
                    XMLSettings.Load(SettingsFilePath);
                else
                {
                    XmlNode baseNode;

                    //add base node to tempDoc
                    baseNode = XMLSettings.CreateNode(XmlNodeType.Element, "settings", "");

                    XMLSettings.AppendChild(baseNode);
                }

                XmlNode xmlSettingNodeToUpdate = XMLSettings.SelectSingleNode("/settings/ImagesXMLDefaultPath");
                xmlSettingNodeToUpdate.InnerText = value;
                XMLSettings.Save(SettingsFilePath);
            }
        }

        public string LocationXMLFilePath
        {
            get
            {
                XmlNode locationXMLFilePathNode = XMLSettings.SelectSingleNode("/settings/LocationXMLFilePath");

                if(locationXMLFilePathNode != null)
                    locationXMLFilePath = locationXMLFilePathNode.InnerText;

                if (locationXMLFilePath.Trim().Length == 0)
                    locationXMLFilePath = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.LocationXMLFilePath"]);

                return locationXMLFilePath;
            }

            set
            {
                locationXMLFilePath = value;

                if (File.Exists(SettingsFilePath))
                    XMLSettings.Load(SettingsFilePath);
                else
                {
                    XmlNode baseNode;

                    //add base node to tempDoc
                    baseNode = XMLSettings.CreateNode(XmlNodeType.Element, "settings", "");

                    XMLSettings.AppendChild(baseNode);
                }

                XmlNode xmlSettingNodeToUpdate = XMLSettings.SelectSingleNode("/settings/LocationXMLFilePath");
                xmlSettingNodeToUpdate.InnerText = value;
                XMLSettings.Save(SettingsFilePath);
            }
        }

        public string FileTypeFilePath
        {
            get
            {
                XmlNode fileTypeFilePathNode = XMLSettings.SelectSingleNode("/settings/FileTypeFilePath");

                if(fileTypeFilePathNode != null)
                    fileTypeFilePath = fileTypeFilePathNode.InnerText;

                if (fileTypeFilePath.Trim().Length == 0)
                    fileTypeFilePath = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.FileTypeFilePath"]);

                return fileTypeFilePath;
            }

            set
            {
                fileTypeFilePath = value;

                if (File.Exists(SettingsFilePath))
                    XMLSettings.Load(SettingsFilePath);
                else
                {
                    XmlNode baseNode;

                    //add base node to tempDoc
                    baseNode = XMLSettings.CreateNode(XmlNodeType.Element, "settings", "");

                    XMLSettings.AppendChild(baseNode);
                }

                XmlNode xmlSettingNodeToUpdate = XMLSettings.SelectSingleNode("/settings/FileTypeFilePath");
                xmlSettingNodeToUpdate.InnerText = value;
                XMLSettings.Save(SettingsFilePath);
            }
        }

        public string MediaTypesPath
        {
            get
            {
                XmlNode mediaTypesPathNode = XMLSettings.SelectSingleNode("/settings/MediaTypesPath");

                mediaTypesPath = mediaTypesPathNode.InnerText;

                if (mediaTypesPath.Trim().Length == 0)
                    mediaTypesPath = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.MediaTypesPath"]);

                return mediaTypesPath;
            }

            set
            {
                mediaTypesPath = value;

                if (File.Exists(SettingsFilePath))
                    XMLSettings.Load(SettingsFilePath);
                else
                {
                    XmlNode baseNode;

                    //add base node to tempDoc
                    baseNode = XMLSettings.CreateNode(XmlNodeType.Element, "settings", "");

                    XMLSettings.AppendChild(baseNode);
                }

                XmlNode xmlSettingNodeToUpdate = XMLSettings.SelectSingleNode("/settings/MediaTypesPath");
                xmlSettingNodeToUpdate.InnerText = value;
                XMLSettings.Save(SettingsFilePath);
            }
        }

        public string DefaultPathToSaveTo
        {
            get
            {
                XmlNode defaultPathToSaveToNode = XMLSettings.SelectSingleNode("/settings/DefaultPathToSaveTo");

                if(defaultPathToSaveToNode != null)
                    defaultPathToSaveTo = defaultPathToSaveToNode.InnerText;

                if (defaultPathToSaveTo.Trim().Length == 0)
                    defaultPathToSaveTo = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.DefaultPathToSaveTo"]);

                return defaultPathToSaveTo;
            }

            set
            {
                defaultPathToSaveTo = value;

                if (File.Exists(SettingsFilePath))
                    XMLSettings.Load(SettingsFilePath);
                else
                {
                    XmlNode baseNode;

                    //add base node to tempDoc
                    baseNode = XMLSettings.CreateNode(XmlNodeType.Element, "settings", "");

                    XMLSettings.AppendChild(baseNode);
                }

                XmlNode xmlSettingNodeToUpdate = XMLSettings.SelectSingleNode("/settings/DefaultPathToSaveTo");
                xmlSettingNodeToUpdate.InnerText = value;
                XMLSettings.Save(SettingsFilePath);
            }
        }

        #endregion

        #region public mediaHandler()
        public MediaHandler()
		{
            XmlNode baseNode;

            if (FileTypeFilePath != null && File.Exists(FileTypeFilePath))
                xmlFileTypes.Load(FileTypeFilePath);
            else
            {
                //add base node to tempDoc
                baseNode = xmlFileTypes.CreateNode(XmlNodeType.Element, "fileTypes", "");

                xmlFileTypes.AppendChild(baseNode);
            }

            if (File.Exists(LocationXMLFilePath))
                xmlLocations.Load(LocationXMLFilePath);
            else
            {
                //add base node to tempDoc
                baseNode = xmlLocations.CreateNode(XmlNodeType.Element, "locations", "");

                xmlLocations.AppendChild(baseNode);
            }

            settingsFilePath = replaceConstants(ConfigurationManager.AppSettings["MediaHandler.SettingsFilePath"]);

            xmlSettings = new XmlDocument();

            if(SettingsFilePath != null && SettingsFilePath.Trim().Length > 0)
                xmlSettings.Load(SettingsFilePath);

            findLocations();
		}
		#endregion

        private static string replaceConstants(string stringToMakeReplacements)
        {
            string returnString = "";

            if(stringToMakeReplacements != null && Application.StartupPath != null)
                returnString = stringToMakeReplacements.Replace("|Application.StartupPath|", Application.StartupPath);

            return returnString;
        }

        /// <summary>
        /// fills dsMedia with media records from mediaFilePath
        /// </summary>
        /// <returns></returns>
        public DataSet GetMedia()
        {
            try
            {
                GetGames();
                GetGameCategories();
                GetGameRatings();
                GetGamesToCategories();

                dsMedia = new DataSet();

                //DataTable dt = new DataTable();
                //dt.ReadXmlSchema(@"C:\utilities\programming\projects\SCTV2\SCTV2\config\media.xsd");
                //DataColumn dcDate = new DataColumn("DataAdded", System.Type.GetType("System.DateTime"));
                //dt.Columns.Add(dcDate);
                //dsMedia.Tables.Add(dt);

                try
                {
                    if (!File.Exists(MediaXMLFilePath))
                    {
                        XmlDocument tempDoc = new XmlDocument();

                        //add base node to tempDoc
                        XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "mediaFiles", "");

                        tempDoc.AppendChild(baseNode);

                        tempDoc.Save(MediaXMLFilePath);
                    }

                    if (File.Exists(MediaXMLFilePath))
                    {
                        try
                        {
                            XmlDocument xDoc = new XmlDocument();
                            xDoc.Load(MediaXMLFilePath);
                        }
                        catch (Exception)
                        {
                            XmlDocument tempDoc = new XmlDocument();

                            //add base node to tempDoc
                            XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "mediaFiles", "");

                            tempDoc.AppendChild(baseNode);

                            tempDoc.Save(MediaXMLFilePath);
                        }

                        dsMedia.ReadXml(new StringReader(xmlQuery(MediaXMLFilePath, "getMedia.xqu").InnerXml.ToString()));
                    }
                   
                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(Tools.errorFile, "dsMedia.ReadXml " + ex.Message);
                }

                if (dsMedia.Tables.Count < 1)
                {
                    if (File.Exists(MediaXMLFileDefaultPath))
                        dsMedia.ReadXml(new StringReader(xmlQuery(MediaXMLFileDefaultPath, "getMedia.xqu").InnerXml.ToString()));

                    //DataTable dtEmptyTable = new DataTable();
                    //dsMedia.Tables.Add(dtEmptyTable);
                }

                if (dsMedia.Tables[0] != null)
                {
                    try
                    {
                        dsMedia = validateMedia(dsMedia);
                        
                        if (dsMedia.Tables[0].Columns.Contains("SortBy"))
                            dsMedia.Tables[0].DefaultView.Sort = "SortBy";//alphabetize                        
                    }
                    catch (Exception ex)
                    {
                        Tools.WriteToFile(Tools.errorFile, "sort " + ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Tools.WriteToFile(Tools.errorFile, e.ToString());
            }
            return dsMedia;
        }

        /// <summary>
        /// fills dsGames with game records from gamesFilePath
        /// </summary>
        /// <returns></returns>
        public DataSet GetGames()
        {
            try
            {
                dsGames = new DataSet();

                //DataTable dt = new DataTable();
                //dt.ReadXmlSchema(@"C:\utilities\programming\projects\SCTV2\SCTV2\config\media.xsd");
                //DataColumn dcDate = new DataColumn("DataAdded", System.Type.GetType("System.DateTime"));
                //dt.Columns.Add(dcDate);
                //dsMedia.Tables.Add(dt);

                try
                {
                    if (!File.Exists(GamesXMLFilePath))
                    {
                        XmlDocument tempDoc = new XmlDocument();

                        //add base node to tempDoc
                        XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "games", "");

                        tempDoc.AppendChild(baseNode);

                        tempDoc.Save(GamesXMLFilePath);
                    }

                    if (File.Exists(GamesXMLFilePath))
                    {
                        try
                        {
                            XmlDocument xDoc = new XmlDocument();
                            xDoc.Load(GamesXMLFilePath);
                        }
                        catch (Exception ex)
                        {
                            Tools.WriteToFile(Tools.errorFile, "GetGames - xDoc.Load " + ex.Message);

                            XmlDocument tempDoc = new XmlDocument();

                            //add base node to tempDoc
                            XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "games", "");

                            tempDoc.AppendChild(baseNode);

                            tempDoc.Save(GamesXMLFilePath);
                        }

                        dsGames.ReadXml(new StringReader(xmlQuery(GamesXMLFilePath, "getGames.xqu").InnerXml.ToString()));
                    }

                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(Tools.errorFile, "GetGames " + ex.Message);
                }

                if (dsGames.Tables[0] != null)
                {
                    try
                    {
                        dsGames = validateGames(dsGames);

                        if (dsGames.Tables[0].Columns.Contains("SortBy"))
                            dsGames.Tables[0].DefaultView.Sort = "SortBy";//alphabetize   
                        else if (dsGames.Tables[0].Columns.Contains("title"))
                            dsGames.Tables[0].DefaultView.Sort = "title";//alphabetize                 
                    }
                    catch (Exception ex)
                    {
                        Tools.WriteToFile(Tools.errorFile, "GetGames sort " + ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Tools.WriteToFile(Tools.errorFile, e.ToString());
            }
            return dsGames;
        }

        /// <summary>
        /// fills dsGames with game records from gamesFilePath
        /// </summary>
        /// <returns></returns>
        public DataSet GetGameCategories()
        {
            try
            {
                dsGameCategories = new DataSet();

                //DataTable dt = new DataTable();
                //dt.ReadXmlSchema(@"C:\utilities\programming\projects\SCTV2\SCTV2\config\media.xsd");
                //DataColumn dcDate = new DataColumn("DataAdded", System.Type.GetType("System.DateTime"));
                //dt.Columns.Add(dcDate);
                //dsMedia.Tables.Add(dt);

                try
                {
                    if (!File.Exists(GamesXMLFilePath.ToLower().Replace("games.xml","gameCategories.xml")))
                    {
                        XmlDocument tempDoc = new XmlDocument();

                        //add base node to tempDoc
                        XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "GamesToCategories", "");

                        tempDoc.AppendChild(baseNode);

                        tempDoc.Save(GamesXMLFilePath.ToLower().Replace("games.xml", "gameCategories.xml"));
                    }

                    if (File.Exists(GamesXMLFilePath.ToLower().Replace("games.xml", "gameCategories.xml")))
                    {
                        try
                        {
                            XmlDocument xDoc = new XmlDocument();
                            xDoc.Load(GamesXMLFilePath.ToLower().Replace("games.xml", "gameCategories.xml"));
                        }
                        catch (Exception ex)
                        {
                            Tools.WriteToFile(Tools.errorFile, "GetGameCategories - xDoc.Load " + ex.Message);

                            XmlDocument tempDoc = new XmlDocument();

                            //add base node to tempDoc
                            XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "GamesToCategories", "");

                            tempDoc.AppendChild(baseNode);

                            tempDoc.Save(GamesXMLFilePath.ToLower().Replace("games.xml", "gameCategories.xml"));
                        }

                        dsGameCategories.ReadXml(new StringReader(xmlQuery(GamesXMLFilePath.ToLower().Replace("games.xml", "gameCategories.xml"), "getGameCategories.xqu").InnerXml.ToString()));
                    }

                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(Tools.errorFile, "GetGameCategories " + ex.Message);
                }

                if (dsGameCategories.Tables[0] != null)
                {
                    try
                    {
                        //dsGameCategories = validateGames(dsGameCategories);

                        if (dsGameCategories.Tables[0].Columns.Contains("SortBy"))
                            dsGameCategories.Tables[0].DefaultView.Sort = "SortBy";//alphabetize   
                        else if (dsGameCategories.Tables[0].Columns.Contains("name"))
                            dsGameCategories.Tables[0].DefaultView.Sort = "name";//alphabetize
                    }
                    catch (Exception ex)
                    {
                        Tools.WriteToFile(Tools.errorFile, "GetGameCategories sort and filter " + ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Tools.WriteToFile(Tools.errorFile, e.ToString());
            }

            DataSet dsTempGames = new DataSet();
            DataTable dtTempGames = dsGameCategories.Tables[0].DefaultView.Table.Copy();
            dsTempGames.Tables.Add(dtTempGames);

            dsGameCategories = dsTempGames.Copy();

            return dsGameCategories;
        }

        /// <summary>
        /// fills dsGames with game records from gamesFilePath
        /// </summary>
        /// <returns></returns>
        public DataSet GetGameRatings()
        {
            try
            {
                dsGameRatings = new DataSet();

                //DataTable dt = new DataTable();
                //dt.ReadXmlSchema(@"C:\utilities\programming\projects\SCTV2\SCTV2\config\media.xsd");
                //DataColumn dcDate = new DataColumn("DataAdded", System.Type.GetType("System.DateTime"));
                //dt.Columns.Add(dcDate);
                //dsMedia.Tables.Add(dt);

                try
                {
                    if (!File.Exists(GamesXMLFilePath.ToLower().Replace("games.xml", "gamesRatings.xml")))
                    {
                        XmlDocument tempDoc = new XmlDocument();

                        //add base node to tempDoc
                        XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "GamesRatings", "");

                        tempDoc.AppendChild(baseNode);

                        tempDoc.Save(GamesXMLFilePath.ToLower().Replace("games.xml", "gamesRatings.xml"));
                    }

                    if (File.Exists(GamesXMLFilePath.ToLower().Replace("games.xml", "gamesRatings.xml")))
                    {
                        try
                        {
                            XmlDocument xDoc = new XmlDocument();
                            xDoc.Load(GamesXMLFilePath.ToLower().Replace("games.xml", "gamesRatings.xml"));
                        }
                        catch (Exception ex)
                        {
                            Tools.WriteToFile(Tools.errorFile, "GetGameRatings - xDoc.Load " + ex.Message);

                            XmlDocument tempDoc = new XmlDocument();

                            //add base node to tempDoc
                            XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "GamesRatings", "");

                            tempDoc.AppendChild(baseNode);

                            tempDoc.Save(GamesXMLFilePath.ToLower().Replace("games.xml", "gamesRatings.xml"));
                        }

                        dsGameRatings.ReadXml(new StringReader(xmlQuery(GamesXMLFilePath.ToLower().Replace("games.xml", "gamesRatings.xml"), "getGameRatings.xqu").InnerXml.ToString()));
                    }

                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(Tools.errorFile, "GetGameRatings " + ex.Message);
                }

                if (dsGameRatings.Tables[0] != null)
                {
                    try
                    {
                        dsGameRatings = validateGames(dsGames);

                        if (dsGameRatings.Tables[0].Columns.Contains("SortBy"))
                            dsGameRatings.Tables[0].DefaultView.Sort = "SortBy";//alphabetize                        
                    }
                    catch (Exception ex)
                    {
                        Tools.WriteToFile(Tools.errorFile, "GetGameRatings sort " + ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Tools.WriteToFile(Tools.errorFile, e.ToString());
            }
            return dsGameRatings;
        }

        /// <summary>
        /// fills dsGames with game records from gamesFilePath
        /// </summary>
        /// <returns></returns>
        public DataSet GetGamesToCategories()
        {
            try
            {
                dsGamesToCategories = new DataSet();

                //DataTable dt = new DataTable();
                //dt.ReadXmlSchema(@"C:\utilities\programming\projects\SCTV2\SCTV2\config\media.xsd");
                //DataColumn dcDate = new DataColumn("DataAdded", System.Type.GetType("System.DateTime"));
                //dt.Columns.Add(dcDate);
                //dsMedia.Tables.Add(dt);

                try
                {
                    if (!File.Exists(GamesXMLFilePath.ToLower().Replace("games.xml", "gamesToCategories.xml")))
                    {
                        XmlDocument tempDoc = new XmlDocument();

                        //add base node to tempDoc
                        XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "GamesToCategories", "");

                        tempDoc.AppendChild(baseNode);

                        tempDoc.Save(GamesXMLFilePath.ToLower().Replace("games.xml", "gamesToCategories.xml"));
                    }

                    if (File.Exists(GamesXMLFilePath.ToLower().Replace("games.xml", "gamesToCategories.xml")))
                    {
                        try
                        {
                            XmlDocument xDoc = new XmlDocument();
                            xDoc.Load(GamesXMLFilePath.ToLower().Replace("games.xml", "gamesToCategories.xml"));
                        }
                        catch (Exception ex)
                        {
                            Tools.WriteToFile(Tools.errorFile, "GetGamesToCategories - xDoc.Load " + ex.Message);

                            XmlDocument tempDoc = new XmlDocument();

                            //add base node to tempDoc
                            XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "GamesToCategories", "");

                            tempDoc.AppendChild(baseNode);

                            tempDoc.Save(GamesXMLFilePath.ToLower().Replace("games.xml", "gamesToCategories.xml"));
                        }

                        dsGamesToCategories.ReadXml(new StringReader(xmlQuery(GamesXMLFilePath.ToLower().Replace("games.xml", "gamesToCategories.xml"), "getGames.xqu").InnerXml.ToString()));
                    }

                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(Tools.errorFile, "GetGamesToCategories " + ex.Message);
                }

                if (dsGamesToCategories != null && dsGamesToCategories.Tables.Count > 0)
                {
                    try
                    {
                        dsGamesToCategories = validateGames(dsGamesToCategories);

                        if (dsGamesToCategories.Tables[0].Columns.Contains("SortBy"))
                            dsGamesToCategories.Tables[0].DefaultView.Sort = "SortBy";//alphabetize                        
                    }
                    catch (Exception ex)
                    {
                        Tools.WriteToFile(Tools.errorFile, "GetGamesToCategories sort " + ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Tools.WriteToFile(Tools.errorFile, e.ToString());
            }
            return dsGamesToCategories;
        }
        
        /// <summary>
        /// fills dsMediaTypes with media records from mediaTypesPath
        /// </summary>
        /// <returns></returns>
        public DataSet GetMediaTypes()
        {
            try
            {
                dsMediaTypes = new DataSet();

                //DataTable dt = new DataTable();
                //dt.ReadXmlSchema(@"C:\utilities\programming\projects\SCTV2\SCTV2\config\media.xsd");
                //DataColumn dcDate = new DataColumn("DataAdded", System.Type.GetType("System.DateTime"));
                //dt.Columns.Add(dcDate);
                //dsMedia.Tables.Add(dt);

                try
                {
                    if (File.Exists(MediaTypesPath))
                        dsMediaTypes.ReadXml(new StringReader(xmlQuery(MediaTypesPath, "getMediaTypes.xqu").InnerXml.ToString()));

                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(Tools.errorFile, "dsMediaTypes.ReadXml " + ex.Message);
                }

                if (dsMediaTypes.Tables.Count < 1)
                {
                    DataTable dtEmptyTable = new DataTable();
                    dsMediaTypes.Tables.Add(dtEmptyTable);
                }
                else if (dsMediaTypes.Tables[0] != null)
                {
                    try
                    {
                        if (dsMediaTypes.Tables[0].Columns.Contains("name"))
                            dsMediaTypes.Tables[0].DefaultView.Sort = "name";//alphabetize                        
                    }
                    catch (Exception ex)
                    {
                        Tools.WriteToFile(Tools.errorFile, "sort " + ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Tools.WriteToFile(Tools.errorFile, e.ToString());
            }
            return dsMediaTypes;
        }

        /// <summary>
        /// find the sequel to the given file
        /// </summary>
        /// <param name="currentFile">The file to find the sequel to</param>
        /// <returns></returns>
        public Media GetSequel(Media currentFile, bool continousPlay)
        {
            Media sequel = null;

            try
            {
                int intFileNameNums = 0;

                if (dsMedia.Tables.Count < 1)//check for the existance of tables in dsMedia
                    GetMedia();

                if (dsMedia.Tables[0].Rows.Count > 1)
                {
                    if (currentFile.filename == null)
                    {
                        string[] filePath = currentFile.filePath.Split('\\');
                        currentFile.filename = filePath[filePath.Length - 1];
                    }

                    //get any numbers at the end of the filepath
                    Match m = new Regex(@"\d+$", RegexOptions.Compiled).Match(currentFile.filename.Trim().Substring(0, currentFile.filename.LastIndexOf(".")));
                    string fileNameNums = m.Value;

                    if (fileNameNums.Length > 0 && int.TryParse(fileNameNums, out intFileNameNums))
                    {
                        string fileNameBase = currentFile.filename.Trim().Substring(0, m.Index);
                        int tempFileNameNums = 0;

                        //try to find sequel using filePath
                        dsMedia.Tables[0].DefaultView.RowFilter = "filePath LIKE '*" + fileNameBase.Replace("'", "''") + "*'";

                        dsMedia.Tables[0].DefaultView.Sort = "filePath";

                        foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)//iterate through results and look for one with a number greater than fileNameNums
                        {
                            //Match foundMatch = new Regex(@"^[\+\-]?\d*\.?[Ee]?[\+\-]?\d*$", RegexOptions.Compiled).Match(dr["filePath"].ToString().Trim());
                            Match foundMatch = new Regex(@"\d+$", RegexOptions.Compiled).Match(dr["filePath"].ToString().Trim().Substring(0, dr["filePath"].ToString().Trim().LastIndexOf(".")));

                            if (foundMatch.Value.Length > 0)
                            {
                                if (int.TryParse(foundMatch.Value, out tempFileNameNums))
                                {
                                    if (tempFileNameNums > intFileNameNums)
                                    {
                                        sequel = CreateMedia(dr);

                                        break;
                                    }
                                }
                            }
                        }


                    }

                    //if not found try and find sequel using title
                    if (sequel == null)
                    {
                        string titleBase = currentFile.Title.Trim();

                        if (titleBase.IndexOf(" ") > 0)
                            titleBase = titleBase.Substring(0, titleBase.LastIndexOf(" "));
                        else if (titleBase.IndexOf("_") > 0)
                            titleBase = titleBase.Substring(0, titleBase.LastIndexOf("_"));

                        titleBase = titleBase.Replace("'", "''");

                        //try to find sequel using title
                        dsMedia.Tables[0].DefaultView.RowFilter = "title LIKE '*" + titleBase.Replace("'", "''") + "*'";

                        dsMedia.Tables[0].DefaultView.Sort = "title";

                        foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)//iterate through results and look for one with a similar title
                        {
                            if (dr["title"].ToString() != currentFile.Title)
                            {
                                sequel = CreateMedia(dr);

                                break;
                            }
                        }
                    }

                    //still not found - play another title in the exact category
                    if (sequel == null && continousPlay)
                    {
                        dsMedia.Tables[0].DefaultView.RowFilter = "";

                        //get category media
                        DataView dvCategoryMedia = GetCategoryMedia(currentFile.category, currentStartsWith, currentMediaType);

                        if (dvCategoryMedia.Count > 0)
                        {
                            //get random number to choose an index with
                            Random rnd = new Random();
                            int newIndex = rnd.Next(0, dvCategoryMedia.Count - 1);

                            DataRowView drv = dvCategoryMedia[newIndex];

                            sequel = CreateMedia(drv);

                            if (sequel.filePath == currentFile.filePath)
                                sequel = null;
                        }
                    }

                    //still not found - play another title in one of the categories
                    if (sequel == null && continousPlay)
                    {
                        foreach (string category in currentFile.category.Split('|'))
                        {
                            dsMedia.Tables[0].DefaultView.RowFilter = "";

                            //get category media
                            DataView dvCategoryMedia = GetCategoryMedia(category, currentStartsWith, currentMediaType);

                            if (dvCategoryMedia.Count > 1)
                            {
                                //this was causing a loop
                                //while (sequel == null)
                                //{
                                //    //get random number to choose an index with
                                //    Random rnd = new Random();
                                //    int newIndex = rnd.Next(0, dvCategoryMedia.Count - 1);

                                //    DataRowView drv = dvCategoryMedia[newIndex];

                                //    sequel = CreateMedia(drv);

                                //    if (sequel.filePath == currentFile.filePath)
                                //        sequel = null;
                                //}

                                break;
                            }
                        }
                    }

                    //still not found - play another title at random
                    if (sequel == null && continousPlay)
                    {
                        dsMedia.Tables[0].DefaultView.RowFilter = "";

                        int tryCounter = 0;

                        while (sequel == null)
                        {
                            tryCounter++; 

                            //get random number to choose an index with
                            Random rnd = new Random();
                            int newIndex = rnd.Next(0, dsMedia.Tables[0].DefaultView.Count - 1);

                            DataRowView drv = dsMedia.Tables[0].DefaultView[newIndex];

                            sequel = CreateMedia(drv);

                            if (sequel.filePath == currentFile.filePath)
                                sequel = null;

                            if(tryCounter > 4)
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "GetSequel "+ ex.Message + Environment.NewLine + ex.StackTrace);
            }
            
            return sequel;
        }

        /// <summary>
        /// validate columns against media.xsd
        /// </summary>
        /// <param name="dsMedia"></param>
        private DataSet validateMedia(DataSet dsMedia)
        {
            try
            {
                //create table if it doesn't exist
                if (dsMedia.Tables.Count < 1)
                {
                    DataTable dtEmptyTable = new DataTable();
                    dsMedia.Tables.Add(dtEmptyTable);
                }

                FileInfo fi = null;

                DataColumn dc;

                //start checking columns
                //TODO: need to read these values from the xsd
                if (!dsMedia.Tables[0].Columns.Contains("ID"))
                {
                    dc = new DataColumn();
                    //dc.DefaultValue = "";
                    dc.ColumnName = "ID";
                    dc.AutoIncrement = true;

                    //reset id's
                    //dsMedia.Tables[0].Columns["ID"].AutoIncrement = true;
                    //dc.AutoIncrementSeed = 0;
                    dc.AutoIncrementStep = 1;
                    dc.AutoIncrementSeed = 1;

                    dsMedia.Tables[0].Columns.Add(dc);

                    dc.SetOrdinal(0);
                }

                if (!dsMedia.Tables[0].Columns.Contains("title"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "title";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("previousTitle"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "previousTitle";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("performers"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "performers";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("ratingDescription"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "ratingDescription";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("description"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "description";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("stars"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "stars";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("category"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "category";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("Director"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "Director";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("ReleaseYear"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "ReleaseYear";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("timesPlayed"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "0";
                    dc.ColumnName = "timesPlayed";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("filePath"))
                {
                    fi = new FileInfo(dsMedia.Tables[0].Columns["filePath"].ToString());

                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "filePath";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("grammar"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "grammar";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("coverImage"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "coverImage";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("IMDBNum"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "IMDBNum";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("SortBy"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "SortBy";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("LastPlayed"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "LastPlayed";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("MediaType"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = defaultMediaType;
                    dc.ColumnName = "MediaType";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("DateAdded"))
                {
                    DateTime dateAdded = DateTime.Now.AddYears(-1);

                    if (fi != null)
                        dateAdded = fi.CreationTime;

                    dc = new DataColumn();
                    dc.DefaultValue = dateAdded.ToShortDateString();
                    dc.ColumnName = "DateAdded";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("LastPlayPosition"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "LastPlayPosition";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("Goofs"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "Goofs";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("Trivia"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "Trivia";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("ShortDescription"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "ShortDescription";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("Season"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "Season";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("EpisodeNum"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "EpisodeNum";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("SeriesName"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "SeriesName";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("SeriesIMDBNum"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "SeriesIMDBNum";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("SeriesImage"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "SeriesImage";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("SeriesDescription"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "SeriesDescription";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("Artist"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "Artist";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("Album"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "Album";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("Lyrics"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "Lyrics";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("Length"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "Length";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("RecordingID"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "RecordingID";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("ArtistID"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "ArtistID";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("ReleaseID"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "ReleaseID";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                if (!dsMedia.Tables[0].Columns.Contains("rating"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "rating";

                    dsMedia.Tables[0].Columns.Add(dc);
                }

                //validate ID for all records
                //dsMedia.Tables[0].DefaultView.RowFilter = "len(ID) = 0";
                //foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                //    dr["ID"] = dr.Row.Table.Rows.IndexOf(dr.Row);
                foreach(DataRow dr in dsMedia.Tables[0].Rows)
                    dr["ID"] = dr.Table.Rows.IndexOf(dr);

                //validate mediaType for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(MediaType) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["MediaType"] = defaultMediaType;

                //validate timesPlayed for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(timesPlayed) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["timesPlayed"] = "0";

                //validate DateAdded for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(DateAdded) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["DateAdded"] = DateTime.Now.ToShortDateString();

                //validate sortBy
                dsMedia.Tables[0].DefaultView.RowFilter = "len(SortBy) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["SortBy"] = formatSortBy(dr["title"].ToString());

                //validate goofs for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(Goofs) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["Goofs"] = "";

                //validate trivia for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(Trivia) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["Trivia"] = "";

                //validate shortDescription for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(ShortDescription) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["ShortDescription"] = "";

                //validate season for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(Season) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["Season"] = "";

                //validate episode for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(EpisodeNum) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["EpisodeNum"] = "";

                //validate seriesName for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(SeriesName) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["SeriesName"] = "";

                //validate seriesIMDBNum for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(SeriesIMDBNum) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["SeriesIMDBNum"] = "";

                //validate seriesImage for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(SeriesImage) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["SeriesImage"] = "";

                //validate seriesDescription for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(SeriesDescription) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["SeriesDescription"] = "";

                //validate Artist for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(Artist) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["Artist"] = "";

                //validate Album for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(Album) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["Album"] = "";

                //validate Lyrics for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(Lyrics) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["Lyrics"] = "";

                //validate Length for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(Length) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["Length"] = "";

                //validate RecordingID for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(RecordingID) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["RecordingID"] = "";

                //validate ArtistID for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(ArtistID) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["ArtistID"] = "";

                //validate ReleaseID for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(ReleaseID) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["ReleaseID"] = "";

                //validate Rating for all records
                dsMedia.Tables[0].DefaultView.RowFilter = "len(rating) = 0";
                foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                    dr["rating"] = "";

                

                //save xml
                dsMedia.WriteXml(MediaXMLFilePath);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "validateMedia: "+ ex.ToString());
            }

            return dsMedia;
        }

        private DataSet validateGames(DataSet dsGames)
        {
            try
            {
                //create table if it doesn't exist
                if (dsGames.Tables.Count < 1)
                {
                    DataTable dtEmptyTable = new DataTable();
                    dsGames.Tables.Add(dtEmptyTable);
                }

                FileInfo fi = null;

                DataColumn dc;

                //start checking columns
                //TODO: need to read these values from the xsd
                if (!dsGames.Tables[0].Columns.Contains("ID"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "ID";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("title"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "title";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("previousTitle"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "previousTitle";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("ratingDescription"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "ratingDescription";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("description"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "description";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("stars"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "stars";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("category"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "category";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("ReleaseYear"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "ReleaseYear";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("timesPlayed"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "0";
                    dc.ColumnName = "timesPlayed";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("filePath"))
                {
                    fi = new FileInfo(Application.StartupPath +"\\games\\"+ dsGames.Tables[0].Columns["fileName"].ToString());

                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "filePath";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("grammar"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "grammar";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("coverImage"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "coverImage";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("SortBy"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "SortBy";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("LastPlayed"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "LastPlayed";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("MediaType"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = defaultMediaType;
                    dc.ColumnName = "MediaType";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("DateAdded"))
                {
                    DateTime dateAdded = DateTime.Now.AddYears(-1);

                    if (fi != null)
                        dateAdded = fi.CreationTime;

                    dc = new DataColumn();
                    dc.DefaultValue = dateAdded.ToShortDateString();
                    dc.ColumnName = "DateAdded";

                    dsGames.Tables[0].Columns.Add(dc);
                }

                if (!dsGames.Tables[0].Columns.Contains("ShortDescription"))
                {
                    dc = new DataColumn();
                    dc.DefaultValue = "";
                    dc.ColumnName = "ShortDescription";

                    dsGames.Tables[0].Columns.Add(dc);
                }
                
                //validate ID for all records
                //dsMedia.Tables[0].DefaultView.RowFilter = "len(ID) = 0";
                //foreach (DataRowView dr in dsMedia.Tables[0].DefaultView)
                //    dr["ID"] = dr.Row.;

                //validate timesPlayed for all records
                dsGames.Tables[0].DefaultView.RowFilter = "len(timesPlayed) = 0";
                foreach (DataRowView dr in dsGames.Tables[0].DefaultView)
                    dr["timesPlayed"] = "0";

                //validate DateAdded for all records
                dsGames.Tables[0].DefaultView.RowFilter = "len(DateAdded) = 0";
                foreach (DataRowView dr in dsGames.Tables[0].DefaultView)
                    dr["DateAdded"] = DateTime.Now.ToShortDateString();

                //validate sortBy
                dsGames.Tables[0].DefaultView.RowFilter = "len(SortBy) = 0";
                foreach (DataRowView dr in dsGames.Tables[0].DefaultView)
                    dr["SortBy"] = formatSortBy(dr["title"].ToString());

                //validate shortDescription for all records
                dsGames.Tables[0].DefaultView.RowFilter = "len(ShortDescription) = 0";
                foreach (DataRowView dr in dsGames.Tables[0].DefaultView)
                    dr["ShortDescription"] = "";
                
                //save xml
                dsGames.WriteXml(GamesXMLFilePath);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "validateGames: " + ex.ToString());
            }

            return dsGames;
        }

        /// <summary>
        /// add locations to arraylist
        /// </summary>
        private void findLocations()
        {
            bool addedLocation = false;
            int locationCount = locations.Count;

            locations.Clear();

            foreach (XmlNode location in xmlLocations["locations"])
                if (location.InnerText.ToString().Trim().Length > 0 && !locations.Contains(location.InnerText.ToString().ToLower()) && !locations.Contains(location.InnerText.ToString()))
                {
                    locations.Add(location.InnerText.ToString().Trim());
                    addedLocation = true;
                }

            //add DefaultPathToSaveTo to locations 
            //TODO: get this value from config file
            //if (locations.Count == 0 && !locations.Contains(DefaultPathToSaveTo.ToLower()))
            if(!addedLocation)
                locations.Add(DefaultPathToSaveTo.ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newMediaType"></param>
        /// <returns></returns>
		public DataView changeFileTypes(string newFileType)
		{
			while(dsMedia.Tables.Count < 1)//check for the existance of tables in dsMedia
			{
				GetMedia();
			}

			while(dsFileTypes.Tables.Count < 1)//check for the existance of tables in dsMediaTypes
			{
				getFileTypes(newFileType);
			}

			string strLIKE = null;
			int rowCount = 0;

			foreach(DataRow dr in dsFileTypes.Tables[0].Rows) //get rows that belong to this media type
			{
				Tools.WriteToFile(Tools.errorFile,"mediaType "+ dr[0].ToString());
				rowCount++;
				strLIKE += "[filePath] LIKE '%."+ dr[0].ToString().Replace("'", "''") + "'";
				currentMediaExtensions.Add(dr[0].ToString());
				if(rowCount < dr.Table.Rows.Count)//this is not the last row add an "OR" to the end of the string
					strLIKE += " OR ";
			}
			dsMedia.Tables[0].DefaultView.RowFilter = strLIKE;
//			updateCategories(dsMedia.Tables[0].DefaultView);
//			return dsMedia;
			return dsMedia.Tables[0].DefaultView;
        }

        public DataView GetTVShowMedia(string seriesIMDBNum)
        {
            DataView foundData = new DataView();
            DataTable tempDT;
            DataTable dtTVShows = null;// = new DataTable();

            //GetMedia();

            if (dsMedia.Tables.Count > 0)
            {
                dsMedia.Tables[0].DefaultView.RowFilter = "seriesIMDBNum = '" + seriesIMDBNum + "'";
            }

            dsMedia.Tables[0].DefaultView.Sort = "title";
            foundData = dsMedia.Tables[0].DefaultView;

            return foundData;
        }

        public DataView GetAlbumsMedia(string artistID)
        {
            DataView foundData = new DataView();
            DataTable tempDT;
            DataTable dtTVShows = null;

            //GetMedia();

            if (dsMedia.Tables.Count > 0)
            {
                dsMedia.Tables[0].DefaultView.RowFilter = "artistID = '" + artistID + "'";
            }

            //get distinct albums
            ArrayList foundAlbumIDs = new ArrayList();
            DataTable dtAlbums = dsMedia.Tables[0].Clone();

            foreach (DataRow dr in dsMedia.Tables[0].DefaultView.ToTable().Rows)
            {
                if (!foundAlbumIDs.Contains(dr["Album"].ToString()))
                {
                    DataRow drAlbum = dtAlbums.NewRow();
                    drAlbum.ItemArray = dr.ItemArray;
                    drAlbum["RecordingID"] = "";
                    //drAlbum["ReleaseID"] = drAlbum["ReleaseID"];
                    //drAlbum["ArtistID"] = drAlbum["ArtistID"];
                    drAlbum["title"] = drAlbum["Album"];
                    drAlbum["filePath"] = "";
                    //drShow["coverImage"] = drShow["coverImage"].ToString();
                    //drAlbum["description"] = drAlbum["Description"];
                    dtAlbums.Rows.Add(drAlbum);

                    foundAlbumIDs.Add(dr["Album"].ToString());
                }
            }

            dtAlbums.DefaultView.Sort = "album";
            foundData = dtAlbums.DefaultView;

            return foundData;
        }

        public DataView GetSongsMedia(string releaseID)
        {
            DataView foundData = new DataView();
            DataTable tempDT;
            DataTable dtTVShows = null;

            //GetMedia();

            if (dsMedia.Tables.Count > 0)
            {
                dsMedia.Tables[0].DefaultView.RowFilter = "releaseID = '" + releaseID + "'";
            }
            
            dsMedia.Tables[0].DefaultView.Sort = "title";
            foundData = dsMedia.Tables[0].DefaultView;

            return foundData;
        }

        public DataView GetCategoryMedia(string category, string startsWith, string mediaType)
        {
            return GetCategoryMedia(category, startsWith, mediaType, true);
        }

        /// <summary>
        /// Gets media that is in all of the given categories(genres)
        /// </summary>
        /// <param name="category"></param>
        /// <param name="startsWith"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public DataView GetCategoryMedia(string category, string startsWith, string mediaType, bool filterOutResults)
        {
            DataView foundData = new DataView();
            DataTable tempDT;

            try
            {
                currentCategory = category;
                currentMediaType = mediaType;
                currentStartsWith = startsWith;

                string[] categories;
                string[] starters;

                //if (category.Contains("|"))
                //{
                    categories = category.Split('|');

                    category = "";

                    foreach (string cat in categories)
                    {
                        if (category.Length > 0)
                        {
                            if (filterOutResults)
                                category += " and ";
                            else
                                category += " or ";

                            category += "category LIKE '*" + cat.Replace("'", "''") + "*'";
                        }
                        else
                            category += "category LIKE '*" + cat.Replace("'", "''") + "*'";
                    }
                //}

                if (currentCategory.ToLower() == "playlist")
                {
                    if (dsPlaylist.Tables.Count < 1)
                        GetPlaylist();

                    //if(startsWith.ToLower() == "all")
                    //    dsPlaylist.Tables[0].DefaultView.RowFilter = "";
                    //else
                    dsPlaylist.Tables[0].DefaultView.RowFilter = "";

                    foundData = dsPlaylist.Tables[0].DefaultView;
                }
                else
                {
                    DataTable sortableDt;
                    DataColumn sortableTimesPlayed;
                    int intTimesPlayed = 0;

                    if (dsMedia.Tables.Count < 1)//check for the existance of tables in dsMedia
                        GetMedia();

                    switch (mediaType.ToLower())
                    {
                        case "pictures":
                            switch (category.ToLower().Replace("category like '*","").Replace("*'",""))
                            {
                                case "all":
                                    if (startsWith.ToLower() != "all")
                                        dsMedia.Tables[0].DefaultView.RowFilter = "SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";
                                    else
                                        dsMedia.Tables[0].DefaultView.RowFilter = "MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";

                                    dsMedia.Tables[0].DefaultView.Sort = "SortBy";
                                    foundData = dsMedia.Tables[0].DefaultView;
                                    break;
                                case "misc":
                                    if (startsWith.ToLower() != "all")
                                        dsMedia.Tables[0].DefaultView.RowFilter = "len(category) = 0 and SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";
                                    else
                                        dsMedia.Tables[0].DefaultView.RowFilter = "len(category) = 0 and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";

                                    dsMedia.Tables[0].DefaultView.Sort = "SortBy";
                                    foundData = dsMedia.Tables[0].DefaultView;
                                    break;
                                case "new":
                                    sortableDt = dsMedia.Tables[0].Copy();

                                    if (startsWith.ToLower() != "all")
                                        sortableDt.DefaultView.RowFilter = "SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*' and timesPlayed = '0'";
                                    else
                                        sortableDt.DefaultView.RowFilter = "MediaType LIKE '*" + mediaType.Replace("'", "''") + "*' and timesPlayed = '0'";

                                    DataColumn sortableDate = new DataColumn("sortableDate", System.Type.GetType("System.DateTime"));
                                    sortableDt.DefaultView.Table.Columns.Add(sortableDate);

                                    foreach (DataRow dr in sortableDt.DefaultView.Table.Rows)
                                    {
                                        DateTime dateAdded = new DateTime();

                                        if (DateTime.TryParse(dr["DateAdded"].ToString(), out dateAdded))
                                            dr["sortableDate"] = dateAdded;
                                        else
                                            dr["sortableDate"] = DateTime.Now;
                                    }

                                    sortableDt.DefaultView.Sort = "sortableDate desc, SortBy";

                                    sortableDt.Columns.Remove("sortableDate");

                                    foundData = getTopDataViewRows(sortableDt.DefaultView, 50);
                                    break;
                                case "not-so popular":
                                    sortableDt = dsMedia.Tables[0].Copy();

                                    if (startsWith.ToLower() != "all")
                                        sortableDt.DefaultView.RowFilter = "SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";
                                    else
                                        sortableDt.DefaultView.RowFilter = "MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";

                                    sortableTimesPlayed = new DataColumn("sortableTimesPlayed", System.Type.GetType("System.Int32"));
                                    sortableDt.DefaultView.Table.Columns.Add(sortableTimesPlayed);

                                    foreach (DataRow dr in sortableDt.DefaultView.Table.Rows)
                                    {
                                        intTimesPlayed = 0;

                                        if (int.TryParse(dr["sortableTimesPlayed"].ToString(), out intTimesPlayed))
                                            dr["sortableTimesPlayed"] = intTimesPlayed;
                                        else
                                            dr["sortableTimesPlayed"] = 0;
                                    }

                                    sortableDt.DefaultView.Sort = "sortableTimesPlayed";

                                    sortableDt.Columns.Remove("sortableTimesPlayed");

                                    foundData = getTopDataViewRows(sortableDt.DefaultView, 50);
                                    break;
                                case "popular":
                                    intTimesPlayed = 0;

                                    sortableDt = dsMedia.Tables[0].Copy();

                                    if (startsWith.ToLower() != "all")
                                        sortableDt.DefaultView.RowFilter = "SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";
                                    else
                                        sortableDt.DefaultView.RowFilter = "MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";

                                    sortableTimesPlayed = new DataColumn("sortableTimesPlayed", System.Type.GetType("System.Int32"));
                                    sortableDt.DefaultView.Table.Columns.Add(sortableTimesPlayed);

                                    foreach (DataRow dr in sortableDt.DefaultView.Table.Rows)
                                    {
                                        intTimesPlayed = 0;

                                        if (int.TryParse(dr["sortableTimesPlayed"].ToString(), out intTimesPlayed))
                                            dr["sortableTimesPlayed"] = intTimesPlayed;
                                        else
                                            dr["sortableTimesPlayed"] = 0;
                                    }

                                    sortableDt.DefaultView.Sort = "sortableTimesPlayed desc";

                                    sortableDt.Columns.Remove("sortableTimesPlayed");

                                    foundData = getTopDataViewRows(sortableDt.DefaultView, 50);
                                    break;
                                default:
                                    if (startsWith.ToLower() != "all")
                                        dsMedia.Tables[0].DefaultView.RowFilter = category + " and SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";
                                        //dsMedia.Tables[0].DefaultView.RowFilter = "category LIKE '*" + category + "*' and SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType + "*'";
                                    else
                                        dsMedia.Tables[0].DefaultView.RowFilter = category + " and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";
                                        //dsMedia.Tables[0].DefaultView.RowFilter = "category LIKE '*" + category + "*' and MediaType LIKE '*" + mediaType + "*'";

                                    dsMedia.Tables[0].DefaultView.Sort = "SortBy";
                                    foundData = dsMedia.Tables[0].DefaultView;
                                    break;
                            }
                            break;
                        case "online":
                            //filter for category and return
                            if (startsWith.ToLower() == "all")
                                dsMediaTypes.Tables[0].DefaultView.RowFilter = category + " and mediaCategory LIKE '*" + mediaType.Replace("'", "''") + "*'";
                                //dsMediaTypes.Tables[0].DefaultView.RowFilter = "category LIKE '*" + category + "*' and mediaCategory LIKE '*" + mediaType + "*'";
                            else
                                dsMediaTypes.Tables[0].DefaultView.RowFilter = category + " and mediaCategory LIKE '*" + mediaType.Replace("'", "''") + "*' and name LIKE '" + startsWith + "*'";
                                //dsMediaTypes.Tables[0].DefaultView.RowFilter = "category LIKE '*" + category + "*' and mediaCategory LIKE '*" + mediaType + "*' and name LIKE '" + startsWith + "*'";

                            dsMediaTypes.Tables[0].DefaultView.Sort = "name";
                            foundData = dsMediaTypes.Tables[0].DefaultView;
                            break;
                        case "games":
                            if (dsGames == null || dsGames.Tables.Count == 0)
                                dsGames = GetGames();

                            dsGameCategories.Tables[0].DefaultView.RowFilter = "name = '" + category.ToLower().Replace("category like '*", "").Replace("*'", "").Trim() + "'";
                            string categoryID = dsGameCategories.Tables[0].DefaultView[0]["categoryID"].ToString();

                            switch (category.ToLower().Replace("category like '*", "").Replace("*'", "").Trim())
                            {
                                case "all":
                                    if (startsWith.ToLower() != "all")
                                        dsGames.Tables[0].DefaultView.RowFilter = "name LIKE '" + startsWith + "*'";
                                    else
                                        dsGames.Tables[0].DefaultView.RowFilter = "";

                                    dsGames.Tables[0].DefaultView.Sort = "name";
                                    foundData = dsGames.Tables[0].DefaultView;
                                    break;
                                case "misc":
                                    if (startsWith.ToLower() != "all")
                                        dsGames.Tables[0].DefaultView.RowFilter = "len(category) = 0 and name LIKE '" + startsWith + "*'";
                                    else
                                        dsGames.Tables[0].DefaultView.RowFilter = "len(category) = 0";

                                    if (mediaType.ToLower() == "tv")
                                        dsGames.Tables[0].DefaultView.RowFilter += " or len(SeriesIMDBNum) = 0";

                                    dsGames.Tables[0].DefaultView.Sort = "name";
                                    foundData = dsGames.Tables[0].DefaultView;
                                    break;
                                case "new":
                                    sortableDt = dsGames.Tables[0].DefaultView.Table.Copy();

                                    if (startsWith.ToLower() != "all")
                                        sortableDt.DefaultView.RowFilter = "name LIKE '" + startsWith + "*' and timesPlayed = '0'";
                                    else
                                        sortableDt.DefaultView.RowFilter = "timesPlayed = '0'";

                                    DataColumn sortableDate = new DataColumn("sortableDate", System.Type.GetType("System.DateTime"));
                                    sortableDt.DefaultView.Table.Columns.Add(sortableDate);

                                    foreach (DataRow dr in sortableDt.DefaultView.Table.Rows)
                                    {
                                        DateTime dateAdded = new DateTime();

                                        if (DateTime.TryParse(dr["DateAdded"].ToString(), out dateAdded))
                                            dr["sortableDate"] = dateAdded;
                                        else
                                            dr["sortableDate"] = DateTime.Now.AddYears(-1);
                                    }

                                    sortableDt.DefaultView.Sort = "sortableDate desc, name";

                                    sortableDt.Columns.Remove("sortableDate");

                                    foundData = getTopDataViewRows(sortableDt.DefaultView, 50);
                                    break;
                                case "not-so popular":
                                    sortableDt = dsGames.Tables[0].Copy();

                                    if (startsWith.ToLower() != "all")
                                        sortableDt.DefaultView.RowFilter = "name LIKE '" + startsWith + "*'";
                                    else
                                        sortableDt.DefaultView.RowFilter = "";

                                    sortableTimesPlayed = new DataColumn("sortableTimesPlayed", System.Type.GetType("System.Int32"));
                                    sortableDt.DefaultView.Table.Columns.Add(sortableTimesPlayed);

                                    foreach (DataRow dr in sortableDt.DefaultView.Table.Rows)
                                    {
                                        intTimesPlayed = 0;

                                        if (int.TryParse(dr["sortableTimesPlayed"].ToString(), out intTimesPlayed))
                                            dr["sortableTimesPlayed"] = intTimesPlayed;
                                        else
                                            dr["sortableTimesPlayed"] = 0;
                                    }

                                    sortableDt.DefaultView.Sort = "sortableTimesPlayed";

                                    sortableDt.Columns.Remove("sortableTimesPlayed");

                                    foundData = getTopDataViewRows(sortableDt.DefaultView, 50);
                                    break;
                                case "popular":
                                    sortableDt = dsGames.Tables[0].Copy();

                                    if (startsWith.ToLower() != "all")
                                        sortableDt.DefaultView.RowFilter = "name LIKE '" + startsWith + "*'";
                                    else
                                        sortableDt.DefaultView.RowFilter = "";

                                    sortableTimesPlayed = new DataColumn("sortableTimesPlayed", System.Type.GetType("System.Int32"));
                                    sortableDt.DefaultView.Table.Columns.Add(sortableTimesPlayed);

                                    foreach (DataRow dr in sortableDt.DefaultView.Table.Rows)
                                    {
                                        intTimesPlayed = 0;

                                        if (int.TryParse(dr["sortableTimesPlayed"].ToString(), out intTimesPlayed))
                                            dr["sortableTimesPlayed"] = intTimesPlayed;
                                        else
                                            dr["sortableTimesPlayed"] = 0;
                                    }

                                    sortableDt.DefaultView.Sort = "sortableTimesPlayed desc";

                                    sortableDt.Columns.Remove("sortableTimesPlayed");

                                    foundData = getTopDataViewRows(sortableDt.DefaultView, 50);
                                    break;
                                case "recent":
                                    sortableDt = dsGames.Tables[0].DefaultView.Table.Copy();

                                    if (startsWith.ToLower() != "all")
                                        sortableDt.DefaultView.RowFilter = "name LIKE '" + startsWith + "*' and len(LastPlayed) > 0";
                                    else
                                        sortableDt.DefaultView.RowFilter = "len(LastPlayed) > 0";

                                    if (!sortableDt.DefaultView.Table.Columns.Contains("sortableDate"))
                                    {
                                        sortableDate = new DataColumn("sortableDate", System.Type.GetType("System.DateTime"));
                                        sortableDt.DefaultView.Table.Columns.Add(sortableDate);
                                    }

                                    foreach (DataRow dr in sortableDt.DefaultView.Table.Rows)
                                    {
                                        DateTime dtLastPlayed = new DateTime();

                                        if (DateTime.TryParse(dr["LastPlayed"].ToString().Split('|')[0], out dtLastPlayed))
                                            dr["sortableDate"] = dtLastPlayed;
                                    }

                                    sortableDt.DefaultView.Sort = "sortableDate desc, name";

                                    sortableDt.Columns.Remove("sortableDate");

                                    foundData = getTopDataViewRows(sortableDt.DefaultView, 50);
                                    break;
                                case "star rating":
                                    int starRating = 0;

                                    if (!dsGames.Tables[0].DefaultView.Table.Columns.Contains("sortableStars"))
                                    {
                                        sortableTimesPlayed = new DataColumn("sortableStars", System.Type.GetType("System.Int32"));
                                        dsGames.Tables[0].DefaultView.Table.Columns.Add(sortableTimesPlayed);
                                    }

                                    if (startsWith.ToLower() != "all")
                                        dsGames.Tables[0].DefaultView.RowFilter = "name LIKE '" + startsWith + "*' and len(stars) > 0";
                                    else
                                        dsGames.Tables[0].DefaultView.RowFilter = "len(stars) > 0";

                                    foreach (DataRow dr in dsGames.Tables[0].DefaultView.Table.Rows)
                                    {
                                        starRating = 0;

                                        int.TryParse(dr["stars"].ToString(), out starRating);

                                        dr["sortableStars"] = starRating;
                                    }

                                    dsGames.Tables[0].DefaultView.Sort = "stars desc, name";

                                    dsGames.Tables[0].DefaultView.Table.Columns.Remove("sortableStars");

                                    foundData = getTopDataViewRows(dsGames.Tables[0].DefaultView, 50);
                                    break;
                                default:
                                    if (startsWith.ToLower() != "all")
                                        dsGames.Tables[0].DefaultView.RowFilter = category.ToLower().Replace(category.ToLower().Replace("category like '*", "").Replace("*'", ""), categoryID) + " and name LIKE '" + startsWith + "*' and categoryID NOT = '8' and display = 'True'";
                                    else
                                        dsGames.Tables[0].DefaultView.RowFilter = category.ToLower().Replace(category.ToLower().Replace("category like '*", "").Replace("*'", ""),categoryID);

                                    dsGames.Tables[0].DefaultView.RowFilter = dsGames.Tables[0].DefaultView.RowFilter.Replace("*", "");
                                    dsGames.Tables[0].DefaultView.RowFilter = dsGames.Tables[0].DefaultView.RowFilter.Replace("category like '", "categoryID = '");

                                    dsGames.Tables[0].DefaultView.Sort = "title";

                                    foundData = dsGames.Tables[0].DefaultView;

                                    break;
                            }

                            break;
                        case "music":
                            //returns a unique dataview of artists

                            //if (startsWith.ToLower() != "all")
                            //    dsMedia.Tables[0].DefaultView.RowFilter = "SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType + "*'";
                            //else

                            DataTable dtArtists = null;

                            if (dsMedia.Tables.Count > 0)
                            {
                                //works just doesn't take into account genre and alphabet
                                //dsMedia.Tables[0].DefaultView.RowFilter = "len(SeriesIMDBNum) > 0 and MediaType LIKE '*" + mediaType + "*'";
                                string tempCategory = category;

                                tempCategory = tempCategory.Replace("category LIKE '*New*'", "");

                                if (tempCategory.ToLower().Replace("category like '*", "").Replace("*'", "").Replace("and ", "").Trim() == "misc")
                                {
                                    dtArtists = dsMedia.Tables[0].Copy();
                                    dtArtists.DefaultView.RowFilter = "MediaType LIKE '*" + mediaType.Replace("'", "''") + "*' and len(ArtistID) = 0";
                                }
                                else
                                {
                                    if (tempCategory.Trim().Length > 0)
                                        tempCategory += " and";

                                    if (startsWith.ToLower() != "all")
                                        dsMedia.Tables[0].DefaultView.RowFilter = tempCategory + " Title LIKE '" + startsWith + "*' and len(ArtistID) > 0 and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";
                                    else
                                        dsMedia.Tables[0].DefaultView.RowFilter = tempCategory + " len(ArtistID) > 0 and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";

                                    //get distinct artists
                                    ArrayList foundArtistIDs = new ArrayList();
                                    dtArtists = dsMedia.Tables[0].Clone();

                                    foreach (DataRow dr in dsMedia.Tables[0].DefaultView.ToTable().Rows)
                                    {
                                        if (!foundArtistIDs.Contains(dr["ArtistID"].ToString()))
                                        {
                                            DataRow drArtist = dtArtists.NewRow();
                                            drArtist.ItemArray = dr.ItemArray;
                                            drArtist["RecordingID"] = "";
                                            drArtist["ReleaseID"] = "";
                                            drArtist["ArtistID"] = dr["ArtistID"];
                                            drArtist["title"] = drArtist["Artist"];
                                            drArtist["filePath"] = "";
                                            //drShow["coverImage"] = drShow["coverImage"].ToString();
                                            drArtist["description"] = drArtist["Description"].ToString();
                                            dtArtists.Rows.Add(drArtist);

                                            foundArtistIDs.Add(dr["ArtistID"].ToString());
                                        }
                                    }
                                }
                            }

                            dtArtists.DefaultView.Sort = "Artist";
                            foundData = dtArtists.DefaultView;
                            break;
                        case "tv":
                            //if (startsWith.ToLower() != "all")
                            //    dsMedia.Tables[0].DefaultView.RowFilter = "SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType + "*'";
                            //else

                            DataTable dtTVShows = null;

                            if (dsMedia.Tables.Count > 0)
                            {
                                string tempCategory = category;
                                
                                tempCategory = tempCategory.Replace("category LIKE '*New*'", "");

                                if (tempCategory.ToLower().Replace("category like '*", "").Replace("*'", "").Replace("and ","").Trim() == "misc")
                                {
                                    dtTVShows = dsMedia.Tables[0].Copy();
                                    dtTVShows.DefaultView.RowFilter = "MediaType LIKE '*" + mediaType.Replace("'", "''") + "*' and len(SeriesIMDBNum) = 0";
                                }
                                else
                                {
                                    if (tempCategory.Trim().Length > 0)
                                        tempCategory += " and";

                                    if (startsWith.ToLower() != "all")
                                        dsMedia.Tables[0].DefaultView.RowFilter = tempCategory + " SeriesName LIKE '" + startsWith + "*' and len(SeriesIMDBNum) > 0 and MediaType LIKE '*" + mediaType + "*'";
                                    else
                                        dsMedia.Tables[0].DefaultView.RowFilter = tempCategory + " len(SeriesIMDBNum) > 0 and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";

                                    //get distinct tv shows
                                    ArrayList foundShowsIMDBNum = new ArrayList();
                                    dtTVShows = dsMedia.Tables[0].Clone();

                                    foreach (DataRow dr in dsMedia.Tables[0].DefaultView.ToTable().Rows)
                                    {
                                        if (!foundShowsIMDBNum.Contains(dr["SeriesIMDBNum"].ToString()))
                                        {
                                            DataRow drShow = dtTVShows.NewRow();
                                            drShow.ItemArray = dr.ItemArray;
                                            drShow["IMDBNum"] = "";
                                            drShow["title"] = drShow["SeriesName"].ToString();
                                            drShow["filePath"] = "";
                                            drShow["coverImage"] = drShow["SeriesImage"].ToString();
                                            drShow["description"] = drShow["SeriesDescription"].ToString();
                                            dtTVShows.Rows.Add(drShow);

                                            foundShowsIMDBNum.Add(dr["SeriesIMDBNum"].ToString());
                                        }
                                    }
                                }
                            }

                            dtTVShows.DefaultView.Sort = "SeriesName";
                            foundData = dtTVShows.DefaultView;
                            break;
                        default: //movies
                            //dsMedia.Tables[0].DefaultView.ToTable().WriteXml(System.Windows.Forms.Application.StartupPath + @"\config\foundMedia.xml");

                            switch (category.ToLower().Replace("category like '*", "").Replace("*'", ""))
                            {
                                case "all":
                                    if (startsWith.ToLower() != "all")
                                        dsMedia.Tables[0].DefaultView.RowFilter = "SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";
                                    else
                                        dsMedia.Tables[0].DefaultView.RowFilter = "MediaType LIKE '*" + mediaType + "*'";

                                    dsMedia.Tables[0].DefaultView.Sort = "SortBy";
                                    foundData = dsMedia.Tables[0].DefaultView;
                                    break;
                                case "misc":
                                    if (startsWith.ToLower() != "all")
                                        dsMedia.Tables[0].DefaultView.RowFilter = "len(category) = 0 and SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";
                                    else
                                        dsMedia.Tables[0].DefaultView.RowFilter = "len(category) = 0 and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";

                                    if (mediaType.ToLower() == "tv")
                                        dsMedia.Tables[0].DefaultView.RowFilter += " or (MediaType LIKE '*" + mediaType.Replace("'", "''") + "*' and len(SeriesIMDBNum) = 0)";

                                    dsMedia.Tables[0].DefaultView.Sort = "SortBy";
                                    foundData = dsMedia.Tables[0].DefaultView;
                                    break;
                                case "new":
                                    sortableDt = dsMedia.Tables[0].DefaultView.Table.Copy();

                                    if (startsWith.ToLower() != "all")
                                        sortableDt.DefaultView.RowFilter = "SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*' and timesPlayed = '0'";
                                    else
                                        sortableDt.DefaultView.RowFilter = "MediaType LIKE '*" + mediaType.Replace("'", "''") + "*' and timesPlayed = '0'";

                                    DataColumn sortableDate = new DataColumn("sortableDate", System.Type.GetType("System.DateTime"));
                                    sortableDt.DefaultView.Table.Columns.Add(sortableDate);

                                    foreach (DataRow dr in sortableDt.DefaultView.Table.Rows)
                                    {
                                        DateTime dateAdded = new DateTime();

                                        if (DateTime.TryParse(dr["DateAdded"].ToString(), out dateAdded))
                                            dr["sortableDate"] = dateAdded;
                                        else
                                            dr["sortableDate"] = DateTime.Now.AddYears(-1);
                                    }

                                    sortableDt.DefaultView.Sort = "sortableDate desc, SortBy";

                                    sortableDt.Columns.Remove("sortableDate");

                                    foundData = getTopDataViewRows(sortableDt.DefaultView, 50);
                                    break;
                                case "not-so popular":
                                    sortableDt = dsMedia.Tables[0].Copy();

                                    if (startsWith.ToLower() != "all")
                                        sortableDt.DefaultView.RowFilter = "SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";
                                    else
                                        sortableDt.DefaultView.RowFilter = "MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";

                                    sortableTimesPlayed = new DataColumn("sortableTimesPlayed", System.Type.GetType("System.Int32"));
                                    sortableDt.DefaultView.Table.Columns.Add(sortableTimesPlayed);

                                    foreach (DataRow dr in sortableDt.DefaultView.Table.Rows)
                                    {
                                        intTimesPlayed = 0;

                                        if (int.TryParse(dr["sortableTimesPlayed"].ToString(), out intTimesPlayed))
                                            dr["sortableTimesPlayed"] = intTimesPlayed;
                                        else
                                            dr["sortableTimesPlayed"] = 0;
                                    }

                                    sortableDt.DefaultView.Sort = "sortableTimesPlayed";

                                    sortableDt.Columns.Remove("sortableTimesPlayed");

                                    foundData = getTopDataViewRows(sortableDt.DefaultView, 50);
                                    break;
                                case "popular":
                                    sortableDt = dsMedia.Tables[0].Copy();

                                    if (startsWith.ToLower() != "all")
                                        sortableDt.DefaultView.RowFilter = "SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";
                                    else
                                        sortableDt.DefaultView.RowFilter = "MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";

                                    sortableTimesPlayed = new DataColumn("sortableTimesPlayed", System.Type.GetType("System.Int32"));
                                    sortableDt.DefaultView.Table.Columns.Add(sortableTimesPlayed);

                                    foreach (DataRow dr in sortableDt.DefaultView.Table.Rows)
                                    {
                                        intTimesPlayed = 0;

                                        if (int.TryParse(dr["sortableTimesPlayed"].ToString(), out intTimesPlayed))
                                            dr["sortableTimesPlayed"] = intTimesPlayed;
                                        else
                                            dr["sortableTimesPlayed"] = 0;
                                    }

                                    sortableDt.DefaultView.Sort = "sortableTimesPlayed desc";

                                    sortableDt.Columns.Remove("sortableTimesPlayed");

                                    foundData = getTopDataViewRows(sortableDt.DefaultView, 50);
                                    break;
                                case "recent":
                                    sortableDt = dsMedia.Tables[0].DefaultView.Table.Copy();

                                    if (startsWith.ToLower() != "all")
                                        sortableDt.DefaultView.RowFilter = "SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*' and len(LastPlayed) > 0";
                                    else
                                        sortableDt.DefaultView.RowFilter = "MediaType LIKE '*" + mediaType.Replace("'", "''") + "*' and len(LastPlayed) > 0";

                                    if (!sortableDt.DefaultView.Table.Columns.Contains("sortableDate"))
                                    {
                                        sortableDate = new DataColumn("sortableDate", System.Type.GetType("System.DateTime"));
                                        sortableDt.DefaultView.Table.Columns.Add(sortableDate);
                                    }

                                    foreach (DataRow dr in sortableDt.DefaultView.Table.Rows)
                                    {
                                        DateTime dtLastPlayed = new DateTime();

                                        if (DateTime.TryParse(dr["LastPlayed"].ToString().Split('|')[0], out dtLastPlayed))
                                            dr["sortableDate"] = dtLastPlayed;
                                    }

                                    sortableDt.DefaultView.Sort = "sortableDate desc, SortBy";

                                    sortableDt.Columns.Remove("sortableDate");

                                    foundData = getTopDataViewRows(sortableDt.DefaultView, 50);
                                    break;
                                case "star rating":
                                    int starRating = 0;

                                    if (!dsMedia.Tables[0].DefaultView.Table.Columns.Contains("sortableStars"))
                                    {
                                        sortableTimesPlayed = new DataColumn("sortableStars", System.Type.GetType("System.Int32"));
                                        dsMedia.Tables[0].DefaultView.Table.Columns.Add(sortableTimesPlayed);
                                    }

                                    if (startsWith.ToLower() != "all")
                                        dsMedia.Tables[0].DefaultView.RowFilter = "SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*' and len(stars) > 0";
                                    else
                                        dsMedia.Tables[0].DefaultView.RowFilter = "MediaType LIKE '*" + mediaType.Replace("'", "''") + "*' and len(stars) > 0";

                                    foreach (DataRow dr in dsMedia.Tables[0].DefaultView.Table.Rows)
                                    {
                                        starRating = 0;

                                        int.TryParse(dr["stars"].ToString(), out starRating);

                                        dr["sortableStars"] = starRating;
                                    }

                                    dsMedia.Tables[0].DefaultView.Sort = "stars desc, SortBy";

                                    dsMedia.Tables[0].DefaultView.Table.Columns.Remove("sortableStars");

                                    foundData = getTopDataViewRows(dsMedia.Tables[0].DefaultView, 50);
                                    break;
                                case "justin downloads":
                                    sortableDt = dsMedia.Tables[0].Copy();

                                    sortableDt.DefaultView.RowFilter = "Category LIKE '*" + category.Replace("'", "''") + "*'";

                                    sortableDt.DefaultView.Sort = "SortBy, Title";

                                    foundData = sortableDt.DefaultView;

                                    tempDT = null;
                                    break;
                                default:
                                    if (startsWith.ToLower() != "all")
                                        dsMedia.Tables[0].DefaultView.RowFilter = category + " and SortBy LIKE '" + startsWith + "*' and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";
                                    else
                                        dsMedia.Tables[0].DefaultView.RowFilter = category + " and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";

                                    dsMedia.Tables[0].DefaultView.Sort = "SortBy";

                                    foundData = dsMedia.Tables[0].DefaultView;

                                    break;
                            }

                            //filter out duplicate entries for movies for justindownloads
                            if (mediaType.ToLower() == "movies")
                            {
                                tempDT = dsMedia.Tables[0].Clone();
                                tempDT.TableName = "tempDT";
                                string currentTitle = "";

                                //foreach (DataRow dr in foundDat.Tables[0].DefaultView)
                                foreach (DataRowView dr in foundData)
                                {
                                    try
                                    {
                                        if (dr["filePath"].ToString().ToLower().Contains("justindownloads"))
                                        {
                                            //get title from filepath
                                            currentTitle = dr["filePath"].ToString();
                                            currentTitle = currentTitle.Substring(0, currentTitle.IndexOf("_"));
                                            currentTitle = currentTitle.Substring(currentTitle.LastIndexOf("\\") + 1);
                                            currentTitle = FormatNameString(currentTitle);

                                            dr["SortBy"] = formatSortBy(currentTitle);

                                            //get unique shows to display in library for justindownloads

                                            if (tempDT.Rows.Count == 0 || tempDT.Select("filePath like '%" + currentTitle.Replace("'", "''") + "_%'").Length == 0)
                                            {
                                                tempDT.ImportRow(dr.Row);

                                                //tempDT.WriteXml(@"C:\utilities\programming\projects\SCTV2\SCTV2\bin\Debug\logs\tempDT.xml");
                                            }
                                        }
                                        else
                                            tempDT.ImportRow(dr.Row);
                                    }
                                    catch (Exception ex)
                                    {
                                        Tools.WriteToFile(Tools.errorFile, "GetCategoryMedia Justin " + ex.ToString());
                                    }
                                }

                                foundData = tempDT.DefaultView;
                            }

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "GetCategoryMedia " + ex.ToString());
            }

            return foundData;
        }
        
        /// <summary>
        /// Gets the available categories for the given mediaType
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
		public DataView GetAllCategories(string mediaType)
		{
			try
			{
                if (mediaType.ToLower() == "online")
                {
                    if (dsMediaTypes.Tables.Count < 1)//check for the existance of tables in dsMediaTypes
                        GetMediaTypes();

                    if (dsMediaTypes.Tables.Count > 0)
                    {
                        dsMediaTypes.Tables[0].DefaultView.RowFilter = "len(category) > 0 and mediaCategory LIKE '*" + mediaType.Replace("'", "''") + "*'";

                        //get distinct categories
                        ArrayList foundCategories = new ArrayList();
                        foreach (DataRow dr in dsMediaTypes.Tables[0].DefaultView.Table.Rows)
                        {
                            if (!foundCategories.Contains(dr["category"].ToString()))
                            {
                                if (dr["category"].ToString().Trim().Length > 0)
                                    foundCategories.Add(dr["category"].ToString());
                            }
                            //else //remove duplicate rows
                            //    dsMedia.Tables[0].DefaultView.Table.Rows.Remove(dr);
                        }
                    }
                    else
                    {
                        if (dsMediaTypes.Tables.Count == 0)
                        {
                            //create empty dataTable
                            DataTable dtEmptyTable = new DataTable();
                            dsMediaTypes.Tables.Add(dtEmptyTable);
                        }
                    }

                    return dsMediaTypes.Tables[0].DefaultView;
                }
                else if (mediaType.ToLower() == "games")
                {
                    DataTable tempGameCategories = dsGameCategories.Tables[0].Copy();

                    tempGameCategories.Columns["name"].ColumnName = "category";

                    tempGameCategories.DefaultView.RowFilter = "category NOT = 'Adult'";

                    //filter out categories that don't have any games
                    //iterate dsGames and get categoryID's
                    //use the categoryID's to filter gameCategories that are not in the list
                    ArrayList categoryIDs = new ArrayList();
                    string categoryIDString = "";

                    foreach (DataRow dr in dsGames.Tables[0].Rows)
                    {
                        if (!categoryIDs.Contains(dr["categoryID"].ToString()))
                            categoryIDs.Add(dr["categoryID"].ToString());
                    }

                    foreach (string id in categoryIDs)
                    {
                        if (categoryIDString.Length > 0)
                            categoryIDString += ",";

                        categoryIDString += id;
                    }

                    tempGameCategories.DefaultView.RowFilter += " and categoryID IN (" + categoryIDString + ")";

                    return tempGameCategories.DefaultView;
                }
                else
                {
                    if (dsMedia.Tables.Count < 1)//check for the existance of tables in dsMedia
                        GetMedia();

                    if (dsMedia.Tables.Count > 0)
                    {
                        dsMedia.Tables[0].DefaultView.RowFilter = "len(category) > 0 and MediaType LIKE '*" + mediaType.Replace("'", "''") + "*'";

                        //get distinct categories
                        ArrayList foundCategories = new ArrayList();
                        foreach (DataRow dr in dsMedia.Tables[0].DefaultView.Table.Rows)
                        {
                            if (!foundCategories.Contains(dr["category"].ToString()))
                            {
                                if (dr["category"].ToString().Trim().Length > 0)
                                    foundCategories.Add(dr["category"].ToString());
                            }
                            //else //remove duplicate rows - throws an error
                            //    dsMedia.Tables[0].DefaultView.Table.Rows.Remove(dr);
                        }
                    }
                    else
                    {
                        if (dsMedia.Tables.Count == 0)
                        {
                            //create empty dataTable
                            DataTable dtEmptyTable = new DataTable();
                            dsMedia.Tables.Add(dtEmptyTable);
                        }
                    }
                }
			}
			catch(Exception ex)
			{
				Tools.WriteToFile(Tools.errorFile,ex.ToString());
			}

            return dsMedia.Tables[0].DefaultView;
		}

        /// <summary>
        /// Gets the available mediaTypes from dsMedia
        /// </summary>
        /// <returns></returns>
        public ArrayList GetAllMediaTypes()
        {
            ArrayList foundTypes = new ArrayList();

            try
            {
                if (dsMedia.Tables.Count < 1)//check for the existance of tables in dsMedia
                    GetMedia();
                
                if (dsMedia.Tables.Count > 0 && dsMedia.Tables[0].Columns.Contains("MediaType"))
                {
                    dsMedia.Tables[0].DefaultView.RowFilter = "len(MediaType) > 0";
                    
                    //get distinct types
                    foundTypes = new ArrayList();
                    foreach (DataRow dr in dsMedia.Tables[0].DefaultView.Table.Rows)
                    {
                        if (!foundTypes.Contains(dr["MediaType"].ToString().Replace("|", "").Trim()))
                            foundTypes.Add(dr["MediaType"].ToString().Replace("|", "").Trim());
                    }
                }
                else
                {
                    if (dsMedia.Tables.Count == 0)
                    {
                        //create empty dataTable
                        DataTable dtEmptyTable = new DataTable();
                        dsMedia.Tables.Add(dtEmptyTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex.ToString());
            }

            //return dsMedia.Tables[0].DefaultView;
            return foundTypes;
        }

        /// <summary>
        /// Get media that has newly been added
        /// </summary>
        /// <returns></returns>
        public DataView GetNewMedia()
        {
            try
            {
                if (dsMedia.Tables.Count < 1)//check for the existance of tables in dsMedia
                    GetMedia();

                if (dsMedia.Tables.Count > 0)
                    dsMedia.Tables[0].DefaultView.RowFilter = "dateAdded > "+ DateTime.Now.AddDays(-1);
                else
                {
                    //create empty dataTable
                    DataTable dtEmptyTable = new DataTable();
                    dsMedia.Tables.Add(dtEmptyTable);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "GetNewMedia "+ ex.ToString());
            }

            return dsMedia.Tables[0].DefaultView;
        }

        /// <summary>
        /// returns a dataview that only contains the top n rows from the source dataview
        /// </summary>
        /// <param name="dvSource"></param>
        /// <param name="numRows"></param>
        /// <returns></returns>
        private DataView getTopDataViewRows(DataView dvSource, int numRows)
        {
            DataTable dtSource = dvSource.Table.Clone();

            for (int i = 0; i < numRows; i++)
            {
                if (i >= dvSource.Count)
                {
                    break;
                }
                dtSource.ImportRow(dvSource[i].Row);
            }
            
            //return new DataView(dtSource, dvSource.RowFilter, dvSource.Sort, dvSource.RowStateFilter); 

            return dtSource.DefaultView;
        }
		
		public DataView SearchByFilePath(string filePath)
		{
			try
			{
				if(dsMedia.Tables.Count < 1)
				{
					GetMedia();
				}
				dsMedia.Tables[0].DefaultView.RowFilter = "filePath LIKE '*"+ filePath.Replace("'", "''") + "*'";
				return dsMedia.Tables[0].DefaultView;
			}
			catch(Exception ex)
			{
				Tools.WriteToFile(Tools.errorFile,"searchByFilePath ERROR "+ ex.ToString());
				return null;
			}
		}

		public DataView SearchByTitle(string title)
		{
			try
			{
				if(dsMedia.Tables.Count < 1)
				{
					GetMedia();
				}
				dsMedia.Tables[0].DefaultView.RowFilter = "title LIKE '%"+ title.Replace("'", "''") + "%'";
				return dsMedia.Tables[0].DefaultView;
			}
			catch(Exception ex)
			{
				Tools.WriteToFile(Tools.errorFile, "SearchByTitle ERROR " + ex.ToString());
				return null;
			}
		}

		/// <summary>
		/// Get file types from fileTypes.xml
		/// </summary>
		/// <param name="mediaType"></param>
		public void getFileTypes(string fileType)
		{
			dsFileTypes.Clear();
			dsFileTypes.ReadXml(new StringReader(xmlQuery("fileTypes.xml","(document(\"theFile\")//fileTypes//"+ fileType +"//type)").InnerXml.ToString()));
		}

		/// <summary>
		/// will query an xml document given the document name and query or a query file name
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		private static XmlDocument xmlQuery(string filePath,string query)
		{
			XmlDocument xmlResults = new XmlDocument();
			try 
			{ 
				// create the collection
				XQueryNavigatorCollection col = new XQueryNavigatorCollection();                        
				// add the file to the collection
				// the file will be referenced in the query statement by its alias
				col.AddNavigator(filePath,"theFile"); 
				//				if(fileName == "Config\\media.xml")
				//				{
				//					col.AddNavigator("Config\\fileTypes.xml", "fileTypes");
				//				}
				// copy the query out from the file
				String q = String.Empty;
				if ( query.IndexOf(".xqu") > 0)//this is a query file 
				{                
					StreamReader sr = new StreamReader(Application.StartupPath +"\\XQu\\"+ query);
					q = sr.ReadToEnd();
					sr.Close();
				}
				else
				{
					q = query;
				}
//				Tools.WriteToFile(Tools.errorFile,q.ToString());
				// compile the query
				XQueryExpression expr = new XQueryExpression(q);

                if(filePath.ToLower().Contains("media.xml"))
                    xmlResults.LoadXml("<mediaFiles>" + expr.Execute(col).ToXml() + "</mediaFiles>");
                else if (filePath.ToLower().Contains("games.xml"))
                    xmlResults.LoadXml("<games>" + expr.Execute(col).ToXml() + "</games>");
                else if (filePath.ToLower().Contains("gamecategories.xml"))
                    xmlResults.LoadXml("<GameCategories>" + expr.Execute(col).ToXml() + "</GameCategories>");
                else if (filePath.ToLower().Contains("gamesratings.xml"))
                    xmlResults.LoadXml("<GamesRatings>" + expr.Execute(col).ToXml() + "</GamesRatings>");
                else if (filePath.ToLower().Contains("gamestocategories.xml"))
                    xmlResults.LoadXml("<GamesToCategories>" + expr.Execute(col).ToXml() + "</GamesToCategories>");
                else
                    xmlResults.LoadXml("<mediaFiles>" + expr.Execute(col).ToXml() + "</mediaFiles>");

                //				foreach(XmlNode node in xmlResults)
                //				{
                //					foreach(XmlElement elem in node)
                //					{
                //						lblMessage.Text += elem["Name"].InnerText.ToString();
                //					}
                //				}
            }
			catch ( Exception e ) 
			{
//				lblMessage.Text = e.ToString();
				Tools.WriteToFile(Tools.errorFile,"File: "+ filePath +"     "+ e.ToString());
			}
//			Tools.WriteToFile(Tools.errorFile,"Results: "+ expr.Execute(col).ToXml().ToString());
			return xmlResults;
		}

        /// <summary>
        /// finds new files in the folders specified in config/locations.xml and ads their information to config/media.xml
        /// </summary>
        public void lookForMedia(bool bestMatch)
        {
            try
            {
                XmlDocument tempDoc = new XmlDocument();
                DataSet ds = new DataSet();

                if (File.Exists(MediaXMLFilePath))
                {
                    tempDoc.Load(MediaXMLFilePath);
                    ds = GetMedia();
                }
                else
                {
                    //add base node to tempDoc
                    XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "mediaFiles", "");

                    tempDoc.AppendChild(baseNode);
                }

                if (ds.Tables.Count > 0 && ds.Tables[0].Columns.Contains("filePath"))//there is media
                {
                    DataColumn[] pk = new DataColumn[1];
                    pk[0] = ds.Tables[0].Columns["filePath"];
                    ds.Tables[0].PrimaryKey = pk;
                }

                if(locations.Count == 0)
                    System.Windows.Forms.MessageBox.Show("There are no valid media sources to read from");
                else
                {
                    foreach(string location in locations)
                    {
                        string newLocation = location;

                        if (newLocation.Contains(","))
                            newLocation = newLocation.Split(',')[0];

                        if(Directory.Exists(newLocation))
                        {
                            tempDoc = lookForMedia(bestMatch, location, tempDoc, ds);
                        }
                        else
                            System.Windows.Forms.MessageBox.Show("Invalid media source "+ newLocation);

                        //tempDoc.Save(mediaFilePath);
                    }
                }
                
                dsMedia = GetMedia();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, ex.ToString());
            }
        }

        private XmlDocument lookForMedia(bool bestMatch, string location, XmlDocument tempDoc, DataSet ds)
        {
            //string dLocation = location.InnerText.ToString().Trim();
            string dLocation = location;
            string mediaType = "";
            string category = "";

            if (location.Contains(","))
            {
                dLocation = location.Split(',')[0];
                mediaType = location.Split(',')[1];
            }
            
            try
            {
                DirectoryInfo fl = new DirectoryInfo(dLocation);

                foreach (FileSystemInfo entry in fl.GetFileSystemInfos())
                {
                    if (entry.Extension.Length > 0)//this is a file
                    {
                        //check extension and make sure it's a type we recognize
                        if (compareExtension(entry.Extension))
                        {
                            string foundMediaType = "";

                            if (entry.FullName.ToLower().Contains("justindownloads"))
                                foundMediaType = "TV";
                            else
                            {
                                foundMediaType = getMediaType(entry.Extension);

                                if (mediaType.ToLower() == "tv" && foundMediaType.ToLower() == "movies")
                                    foundMediaType = mediaType; 

                                if (foundMediaType.Length == 0)
                                    foundMediaType = mediaType;
                            }

                            //string dvdMovieName = "";
                            DirectoryInfo dvdFolder = null;

                            if(entry.Extension.ToLower().Contains("vob"))//these are dvd files - the folder needs to be played all at once
                            {
                                if (fl.Name.ToLower() == "video_ts")
                                {
                                    //dvdMovieName = fl.Parent.Parent.Name;
                                    dvdFolder = fl.Parent;
                                }
                                else
                                    dvdFolder = fl;
                                    //dvdMovieName = fl.Parent.Name;
                            }

                            if (dLocation.LastIndexOf("\\") != dLocation.Length - 1)
                                dLocation += "\\";

                            if (ds.Tables.Count == 0 || ds.Tables[0].DefaultView.Table.Select("filePath LIKE '*" + dLocation + entry.Name.Trim().Replace("'","''") + "*'").Length == 0)//ds.Tables[0].Rows.Find(dLocation + "\\" + entry.Name.ToString().Trim()) == null)//this file name doesn't exist in the media file
                            {
                                if (foundMediaType.ToLower() == "images")
                                {
                                    tempDoc = addImageFile(tempDoc, entry, bestMatch, foundMediaType);
                                }
                                else if(dvdFolder != null)
                                {
                                    tempDoc = addDVDFolder(tempDoc, dvdFolder, entry, bestMatch, foundMediaType);
                                }
                                else if (foundMediaType.ToLower() == "music")
                                {
                                    tempDoc = addMusicFile(tempDoc, entry, fl, bestMatch, foundMediaType);
                                }
                                else
                                {
                                    //right now everything is being added as video
                                    tempDoc = addVideoFile(tempDoc, entry, bestMatch, foundMediaType);
                                }

                                //string foundMediaType = getMediaType(entry.Extension);

                                //switch (foundMediaType.ToLower())
                                //{
                                //    case "movies"://this is all video - right now they are classified as "Movies"
                                //        tempDoc = addVideoFile(tempDoc, entry, bestMatch, foundMediaType);
                                //        break;
                                //    case "music":
                                //        tempDoc = addVideoFile(tempDoc, entry, bestMatch, foundMediaType);
                                //        break;
                                //    case "images":
                                //        //tempDoc = addImageFile(tempDoc, entry, bestMatch);
                                //        break;
                                //}
                            }
                            else if (ds.Tables[0].DefaultView.Table.Select("(len(IMDBNum) = 0 or len(category) = 0) and filePath LIKE '*" + dLocation + entry.Name.Trim().Replace("'", "''") + "*'").Length > 0 && (foundMediaType.ToLower() == "movies" || foundMediaType.ToLower() == "tv"))
                            {
                                try
                                {
                                    //try to get imdb info for this one
                                    IMDBScraper imdb = new IMDBScraper();
                                    Media newMedia = imdb.getInfoByTitle(entry.Name.ToString().Trim(), bestMatch, foundMediaType);

                                    ds.Tables[0].DefaultView.RowFilter = "(len(IMDBNum) = 0 or len(category) = 0) and filePath LIKE '*" + dLocation + entry.Name.Trim().Replace("'", "''") + "*'";

                                    Media currMedia = CreateMedia(ds.Tables[0].DefaultView[0]);

                                    newMedia.ID = currMedia.ID;

                                    if (newMedia.category == null || newMedia.category.Length == 0)
                                        newMedia.category = currMedia.category;

                                    if (newMedia.coverImage == null || newMedia.coverImage.Length == 0)
                                        newMedia.coverImage = currMedia.coverImage;

                                    newMedia.DateAdded = currMedia.DateAdded;

                                    if (newMedia.Description == null || newMedia.Description.Length == 0)
                                        newMedia.Description = currMedia.Description;

                                    if (newMedia.Director == null || newMedia.Director.Length == 0)
                                        newMedia.Director = currMedia.Director;

                                    if (newMedia.filename == null || newMedia.filename.Length == 0)
                                        newMedia.filename = currMedia.filename;

                                    if (newMedia.filePath == null || newMedia.filePath.Length == 0)
                                        newMedia.filePath = currMedia.filePath;

                                    newMedia.grammar = currMedia.grammar;
                                    newMedia.LastPlayed = currMedia.LastPlayed;
                                    newMedia.LastPlayPosition = currMedia.LastPlayPosition;

                                    if (newMedia.MediaType == null || newMedia.MediaType.Length == 0)
                                        newMedia.MediaType = currMedia.MediaType;

                                    newMedia.Message = currMedia.Message;

                                    if (newMedia.Performers == null || newMedia.Performers.Length == 0)
                                        newMedia.Performers = currMedia.Performers;

                                    newMedia.PreviousTitle = currMedia.PreviousTitle;
                                    if (newMedia.Rating == null || newMedia.Rating.Length == 0)
                                        newMedia.Rating = currMedia.Rating;

                                    if (newMedia.RatingDescription == null || newMedia.RatingDescription.Length == 0)
                                        newMedia.RatingDescription = currMedia.RatingDescription;

                                    if (newMedia.ReleaseYear == null || newMedia.ReleaseYear.Length == 0)
                                        newMedia.ReleaseYear = currMedia.ReleaseYear;

                                    newMedia.SortBy = currMedia.SortBy;
                                    newMedia.Stars = currMedia.Stars;

                                    if (newMedia.TagLine == null || newMedia.TagLine.Length == 0)
                                        newMedia.TagLine = currMedia.TagLine;

                                    newMedia.TimesPlayed = currMedia.TimesPlayed;

                                    if (newMedia.Title == null || newMedia.Title.Length == 0)
                                        newMedia.Title = currMedia.Title;

                                    if (newMedia.Goofs == null || newMedia.Goofs.Length == 0)
                                        newMedia.Goofs = currMedia.Goofs;

                                    if (newMedia.Trivia == null || newMedia.Trivia.Length == 0)
                                        newMedia.Trivia = currMedia.Trivia;

                                    if (newMedia.ShortDescription == null || newMedia.ShortDescription.Length == 0)
                                        newMedia.ShortDescription = currMedia.ShortDescription;

                                    if (newMedia.Season == null || newMedia.Season.Length == 0)
                                        newMedia.Season = currMedia.Season;

                                    if (newMedia.EpisodeNum == null || newMedia.EpisodeNum.Length == 0)
                                        newMedia.EpisodeNum = currMedia.EpisodeNum;

                                    if (newMedia.SeriesName == null || newMedia.SeriesName.Length == 0)
                                        newMedia.SeriesName = currMedia.SeriesName;

                                    if (newMedia.SeriesIMDBNum == null || newMedia.SeriesIMDBNum.Length == 0)
                                        newMedia.SeriesIMDBNum = currMedia.SeriesIMDBNum;

                                    if (newMedia.SeriesImage == null || newMedia.SeriesImage.Length == 0)
                                        newMedia.SeriesImage = currMedia.SeriesImage;

                                    if (newMedia.SeriesDescription == null || newMedia.SeriesDescription.Length == 0)
                                        newMedia.SeriesDescription = currMedia.SeriesDescription;

                                    if (newMedia.Artist == null || newMedia.Artist.Length == 0)
                                        newMedia.Artist = currMedia.Artist;

                                    if (newMedia.Album == null || newMedia.Album.Length == 0)
                                        newMedia.Album = currMedia.Album;

                                    if (newMedia.Lyrics == null || newMedia.Lyrics.Length == 0)
                                        newMedia.Lyrics = currMedia.Lyrics;

                                    if (newMedia.Length == null || newMedia.Length.Length == 0)
                                        newMedia.Length = currMedia.Length;

                                    if (newMedia.RecordingID == null || newMedia.RecordingID.Length == 0)
                                        newMedia.RecordingID = currMedia.RecordingID;

                                    if (newMedia.ArtistID == null || newMedia.ArtistID.Length == 0)
                                        newMedia.ArtistID = currMedia.ArtistID;

                                    if (newMedia.ReleaseID == null || newMedia.ReleaseID.Length == 0)
                                        newMedia.ReleaseID = currMedia.ReleaseID;

                                    UpdateMediaInfo(newMedia);

                                    tempDoc.Load(MediaXMLFilePath);
                                }
                                catch (Exception ex)
                                {
                                    Tools.WriteToFile(ex);
                                    Tools.WriteToFile(Tools.errorFile, "lookForMedia1 location " + location +"\\"+ entry.Name);
                                }                                
                            }
                        }
                    }
                }

                //look through children folders
                foreach (DirectoryInfo dir in fl.GetDirectories())
                {
                    if(mediaType.Trim().Length > 0)
                        tempDoc = lookForMedia(bestMatch, dir.FullName +","+ mediaType, tempDoc, ds);
                    else
                        tempDoc = lookForMedia(bestMatch, dir.FullName, tempDoc, ds);
                }
            }
            catch (Exception e)
            {
                Tools.WriteToFile(Tools.errorFile, "lookForMedia2 " + e.Message + Environment.NewLine + e.StackTrace);
                Tools.WriteToFile(Tools.errorFile, "lookForMedia2 location " + location);
            }

            return tempDoc;
        }

        public int CleanMedia()
        {
            int numDeletedRecords = 0;
            ArrayList rowsToDelete = new ArrayList();

            //make sure we don't have any media without a filepath
            //find the given media
            dsMedia.Tables[0].DefaultView.RowFilter = "len(filePath) = 0";
            DataView dv = dsMedia.Tables[0].DefaultView;

            try
            {
                rowsToDelete = new ArrayList();

                foreach (DataRowView drMediaToDelete in dv)
                {
                    //drMediaToDelete.Delete();
                    rowsToDelete.Add(drMediaToDelete);
                    numDeletedRecords++;
                }

                foreach (DataRow dr in rowsToDelete)
                    dsMedia.Tables[0].Rows.Remove(dr);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "cleanMedia error: " + ex.Message);
            }

            try
            {
                rowsToDelete = new ArrayList();

                //iterate all media records and check for valid filepaths
                foreach (DataRow dr in dsMedia.Tables[0].Rows)
                {
                    if (!File.Exists(dr["filePath"].ToString()))
                    {
                        //dr.Delete();
                        rowsToDelete.Add(dr);
                        numDeletedRecords++;
                    }
                }

                foreach(DataRow dr in rowsToDelete)
                    dsMedia.Tables[0].Rows.Remove(dr);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "cleanMedia error: " + ex.Message);
            }

            //reset id's
            dsMedia.Tables[0].Columns["ID"].AutoIncrement = true;
            dsMedia.Tables[0].Columns["ID"].AutoIncrementSeed = 0;
            dsMedia.Tables[0].Columns["ID"].AutoIncrementStep = -1;
            dsMedia.Tables[0].Columns["ID"].AutoIncrementSeed = -1;

            //save xml
            dsMedia.WriteXml(MediaXMLFilePath);
            return numDeletedRecords;
        }

        /// <summary>
        /// Add video file info to given xmlDocument
        /// </summary>
        /// <param name="tempDoc"></param>
        /// <param name="fileToAdd"></param>
        /// <param name="bestMatch"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private XmlDocument addVideoFile(XmlDocument videoDoc, FileSystemInfo fileToAdd, bool bestMatch, string foundMediaType)
        {
            IMDBScraper imdb = null;
            Media newMedia = null;
            string fileName = "";
            string formattedFileName = "";
            fileName = System.Text.RegularExpressions.Regex.Replace(fileToAdd.Name, fileToAdd.Extension, "");
            
            if (fileToAdd.FullName.ToLower().Contains("justindownloads"))
                fileName = fileName.Split('_')[0];

            formattedFileName = FormatNameString(fileName);

            if (!isDuplicate(videoDoc, fileToAdd.FullName, "mediaFiles/media/filePath"))
            {
                try
                {
                    if (foundMediaType.ToLower() == "movies" || foundMediaType.ToLower() == "tv")
                    {
                        newMedia = getTVSeriesInfoFromFileName(fileName, null);

                        if(newMedia.MediaType == null || newMedia.MediaType.Trim().Length == 0)
                            newMedia.MediaType = foundMediaType;

                        //get imdb info
                        imdb = new IMDBScraper();
                        newMedia = imdb.getInfoByTitle(formattedFileName, bestMatch, newMedia);

                        //set mediaType to tv if it's a justin download and it wasn't found on imdb
                        if ((newMedia.IMDBNum != null && newMedia.IMDBNum.Trim().Length == 0) && (newMedia.filePath != null && newMedia.filePath.ToLower().Contains("justindownloads")))
                            newMedia.MediaType = "TV";

                        if (newMedia.MediaType != null)
                            foundMediaType = newMedia.MediaType;
                        else
                            newMedia.MediaType = foundMediaType;

                        if(foundMediaType.ToLower() == "tv")
                            newMedia = getEpisodeInfo(newMedia);
                            

                        //imdb.getPhoto(newMedia);
                    }
                }
                catch (Exception ex)
                {
                    imdb = null;

                    if (!ignoreIMDBErrors && !bestMatch)
                    {
                        internetConnectionDown connectionDown = new internetConnectionDown();
                        DialogResult result = connectionDown.ShowDialog();

                        if (result == DialogResult.Retry)
                            ignoreIMDBErrors = false;
                        else if (result == DialogResult.Ignore)
                            ignoreIMDBErrors = true;
                        else
                            ignoreIMDBErrors = false;
                    }

                    Tools.WriteToFile(Tools.errorFile, "lookForMedia error: " + ex.Message);
                }

                XmlNode newNode = videoDoc.CreateNode(XmlNodeType.Element, "media", "");
                XmlNode IDNode = videoDoc.CreateNode(XmlNodeType.Element, "ID", "");
                XmlNode previousTitleNode = videoDoc.CreateNode(XmlNodeType.Element, "previousTitle", "");
                previousTitleNode.InnerText = formattedFileName;
                XmlNode nameNode = videoDoc.CreateNode(XmlNodeType.Element, "title", "");
                XmlNode performersNode = videoDoc.CreateNode(XmlNodeType.Element, "performers", "");
                XmlNode ratingNode = videoDoc.CreateNode(XmlNodeType.Element, "rating", "");
                XmlNode ratingDescriptionNode = videoDoc.CreateNode(XmlNodeType.Element, "ratingDescription", "");
                XmlNode descriptionNode = videoDoc.CreateNode(XmlNodeType.Element, "description", "");
                XmlNode starsNode = videoDoc.CreateNode(XmlNodeType.Element, "stars", "");
                XmlNode categoryNode = videoDoc.CreateNode(XmlNodeType.Element, "category", "");
                XmlNode timesPlayedNode = videoDoc.CreateNode(XmlNodeType.Element, "timesPlayed", "");
                timesPlayedNode.InnerText = "0";
                XmlNode filePathNode = videoDoc.CreateNode(XmlNodeType.Element, "filePath", "");
                filePathNode.InnerText = fileToAdd.FullName;
                XmlNode grammarNode = videoDoc.CreateNode(XmlNodeType.Element, "grammar", "");
                XmlNode coverImageNode = videoDoc.CreateNode(XmlNodeType.Element, "coverImage", "");
                XmlNode imdbNumNode = videoDoc.CreateNode(XmlNodeType.Element, "IMDBNum", "");
                XmlNode directorNode = videoDoc.CreateNode(XmlNodeType.Element, "Director", "");
                XmlNode releaseYearNode = videoDoc.CreateNode(XmlNodeType.Element, "ReleaseYear", "");
                XmlNode dateAddedNode = videoDoc.CreateNode(XmlNodeType.Element, "DateAdded", "");
                XmlNode sortByNode = videoDoc.CreateNode(XmlNodeType.Element, "SortBy", "");
                XmlNode lastPlayedNode = videoDoc.CreateNode(XmlNodeType.Element, "LastPlayed", "");
                XmlNode mediaTypeNode = videoDoc.CreateNode(XmlNodeType.Element, "MediaType", "");
                XmlNode taglineNode = videoDoc.CreateNode(XmlNodeType.Element, "Tagline", "");
                XmlNode lastPlayPositionNode = videoDoc.CreateNode(XmlNodeType.Element, "LastPlayPosition", "");
                XmlNode goofsNode = videoDoc.CreateNode(XmlNodeType.Element, "Goofs", "");
                XmlNode triviaNode = videoDoc.CreateNode(XmlNodeType.Element, "Trivia", "");
                XmlNode shortDescriptionNode = videoDoc.CreateNode(XmlNodeType.Element, "ShortDescription", "");
                XmlNode seasonNode = videoDoc.CreateNode(XmlNodeType.Element, "Season", "");
                XmlNode episodeNumNode = videoDoc.CreateNode(XmlNodeType.Element, "EpisodeNum", "");
                XmlNode seriesNameNode = videoDoc.CreateNode(XmlNodeType.Element, "SeriesName", "");
                XmlNode seriesIMDBNumNode = videoDoc.CreateNode(XmlNodeType.Element, "SeriesIMDBNum", "");
                XmlNode seriesImageNode = videoDoc.CreateNode(XmlNodeType.Element, "SeriesImage", "");
                XmlNode seriesDescriptionNode = videoDoc.CreateNode(XmlNodeType.Element, "SeriesDescription", "");

                dateAddedNode.InnerText = fileToAdd.CreationTime.ToShortDateString();

                if (imdb != null)
                {
                    //if (imdb.MovieTitle.Length > 0 && !imdb.MovieTitle.Contains("IMDb Search") && !imdb.MovieTitle.Contains("Find - IMDb"))
                    if (newMedia.Title.Length > 0 && !newMedia.Title.Contains("IMDb Search") && !newMedia.Title.Contains("Find - IMDb"))
                    {
                        //clean up title
                        //string tempTitle = imdb.MovieTitle;
                        //tempTitle = tempTitle.Replace("&#x22;", "\"");
                        //tempTitle = tempTitle.Replace("&#x26;", "&");
                        //tempTitle = tempTitle.Replace("&#x27;", "'");

                        nameNode.InnerText = newMedia.Title;
                    }
                    else
                        nameNode.InnerText = formattedFileName;

                    ratingNode.InnerText = newMedia.Rating;
                    ratingDescriptionNode.InnerText = newMedia.RatingDescription;
                    taglineNode.InnerText = newMedia.TagLine;
                    descriptionNode.InnerText = newMedia.Description;
                    categoryNode.InnerText = newMedia.category;

                    if (newMedia.Title.Length > 0 && !newMedia.Title.Contains("IMDb Search") && !newMedia.Title.Contains("Find - IMDb"))
                        grammarNode.InnerText = newMedia.Title;
                    else
                        grammarNode.InnerText = formattedFileName;

                    coverImageNode.InnerText = newMedia.coverImage;
                    imdbNumNode.InnerText = newMedia.IMDBNum;
                    directorNode.InnerText = newMedia.Director;
                    releaseYearNode.InnerText = newMedia.ReleaseYear;
                    goofsNode.InnerText = newMedia.Goofs;
                    triviaNode.InnerText = newMedia.Trivia;
                    shortDescriptionNode.InnerText = newMedia.ShortDescription;
                    seasonNode.InnerText = newMedia.Season;
                    episodeNumNode.InnerText = newMedia.EpisodeNum;
                    seriesNameNode.InnerText = newMedia.SeriesName;
                    seriesIMDBNumNode.InnerText = newMedia.SeriesIMDBNum;
                    seriesImageNode.InnerText = newMedia.SeriesImage;
                    seriesDescriptionNode.InnerText = newMedia.SeriesDescription;
                }
                else
                {
                    if (foundMediaType.ToLower() == "pictures")
                        coverImageNode.InnerText = filePathNode.InnerText;

                    nameNode.InnerText = formattedFileName;
                    grammarNode.InnerText = formattedFileName;
                }

                sortByNode.InnerText = formatSortBy(nameNode.InnerText);
                mediaTypeNode.InnerText = foundMediaType;

                if (filePathNode.InnerText.ToLower().Contains("justindownloads"))
                {
                    if (categoryNode.InnerText.Length > 0)
                        categoryNode.InnerText += "|";

                    categoryNode.InnerText += "Justin Downloads";
                }

                //if(dsMedia != null && dsMedia.Tables.Count > 0)

                //IDNode.InnerText = dsMedia.Tables[0].Rows.Count.ToString();
                //else
                //    IDNode.InnerText = "1";

                IDNode.InnerText = videoDoc.SelectNodes("/mediaFiles/media").Count.ToString();

                newNode.AppendChild(IDNode);
                newNode.AppendChild(nameNode);
                newNode.AppendChild(previousTitleNode);
                newNode.AppendChild(performersNode);
                newNode.AppendChild(ratingNode);
                newNode.AppendChild(ratingDescriptionNode);
                newNode.AppendChild(taglineNode);
                newNode.AppendChild(descriptionNode);
                newNode.AppendChild(starsNode);
                newNode.AppendChild(categoryNode);
                newNode.AppendChild(directorNode);
                newNode.AppendChild(releaseYearNode);
                newNode.AppendChild(dateAddedNode);

                //string[] categories = imdb.Genre.Split('/');

                //for (int x = 0; x < categories.Length; x++)
                //{
                //    XmlNode categoryNode = tempDoc.CreateNode(XmlNodeType.Element, "category", "");
                //    categoryNode.InnerText = categories[x].Trim();
                //    newNode.AppendChild(categoryNode);
                //    break;
                //}

                newNode.AppendChild(timesPlayedNode);
                newNode.AppendChild(filePathNode);
                newNode.AppendChild(grammarNode);
                newNode.AppendChild(coverImageNode);
                newNode.AppendChild(imdbNumNode);
                newNode.AppendChild(sortByNode);
                newNode.AppendChild(lastPlayedNode);
                newNode.AppendChild(mediaTypeNode);
                newNode.AppendChild(lastPlayPositionNode);
                newNode.AppendChild(goofsNode); 
                newNode.AppendChild(triviaNode);
                newNode.AppendChild(shortDescriptionNode);
                newNode.AppendChild(seasonNode);
                newNode.AppendChild(episodeNumNode);
                newNode.AppendChild(seriesNameNode);
                newNode.AppendChild(seriesIMDBNumNode);
                newNode.AppendChild(seriesImageNode);
                newNode.AppendChild(seriesDescriptionNode);
                videoDoc["mediaFiles"].AppendChild(newNode);

                videoDoc.Save(MediaXMLFilePath);
            }

            return videoDoc;
        }

        /// <summary>
        /// Add video file info to given xmlDocument
        /// </summary>
        /// <param name="tempDoc"></param>
        /// <param name="fileToAdd"></param>
        /// <param name="bestMatch"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private XmlDocument addMusicFile(XmlDocument videoDoc, FileSystemInfo fileToAdd, DirectoryInfo locationsDirectory, bool bestMatch, string foundMediaType)
        {
            MusicBrainzScraper musicBrainz = null;
            Media newMedia = null;
            string fileName = "";
            string formattedFileName = "";
            fileName = System.Text.RegularExpressions.Regex.Replace(fileToAdd.Name, fileToAdd.Extension, "");

            formattedFileName = FormatNameString(fileName);

            if (!isDuplicate(videoDoc, fileToAdd.FullName, "mediaFiles/media/filePath"))
            {
                try
                {
                    //trys to fill in artist and title
                    newMedia = getMusicInfoFromFileSystemInfo(fileToAdd, locationsDirectory.FullName, null);

                    //sets mediaType to Music
                    newMedia.MediaType = foundMediaType;

                    newMedia.filename = fileToAdd.FullName;

                    //get MusicBrainz info
                    musicBrainz = new MusicBrainzScraper();
                    newMedia = musicBrainz.getInfoByTitleAndArtist(newMedia);
                        
                    if (newMedia.MediaType != null)
                        foundMediaType = newMedia.MediaType;
                    else
                        newMedia.MediaType = foundMediaType;
                }
                catch (Exception ex)
                {
                    musicBrainz = null;
                    
                    Tools.WriteToFile(Tools.errorFile, "addMusicFile error: " + ex.Message + Environment.NewLine + ex.StackTrace);
                }

                XmlNode newNode = videoDoc.CreateNode(XmlNodeType.Element, "media", "");
                XmlNode IDNode = videoDoc.CreateNode(XmlNodeType.Element, "ID", "");
                XmlNode previousTitleNode = videoDoc.CreateNode(XmlNodeType.Element, "previousTitle", "");
                previousTitleNode.InnerText = formattedFileName;
                XmlNode nameNode = videoDoc.CreateNode(XmlNodeType.Element, "title", "");
                XmlNode performersNode = videoDoc.CreateNode(XmlNodeType.Element, "performers", "");
                XmlNode ratingNode = videoDoc.CreateNode(XmlNodeType.Element, "rating", "");
                XmlNode ratingDescriptionNode = videoDoc.CreateNode(XmlNodeType.Element, "ratingDescription", "");
                XmlNode descriptionNode = videoDoc.CreateNode(XmlNodeType.Element, "description", "");
                XmlNode starsNode = videoDoc.CreateNode(XmlNodeType.Element, "stars", "");
                XmlNode categoryNode = videoDoc.CreateNode(XmlNodeType.Element, "category", "");
                XmlNode timesPlayedNode = videoDoc.CreateNode(XmlNodeType.Element, "timesPlayed", "");
                timesPlayedNode.InnerText = "0";
                XmlNode filePathNode = videoDoc.CreateNode(XmlNodeType.Element, "filePath", "");
                filePathNode.InnerText = fileToAdd.FullName;
                XmlNode grammarNode = videoDoc.CreateNode(XmlNodeType.Element, "grammar", "");
                XmlNode coverImageNode = videoDoc.CreateNode(XmlNodeType.Element, "coverImage", "");
                XmlNode imdbNumNode = videoDoc.CreateNode(XmlNodeType.Element, "IMDBNum", "");
                XmlNode directorNode = videoDoc.CreateNode(XmlNodeType.Element, "Director", "");
                XmlNode releaseYearNode = videoDoc.CreateNode(XmlNodeType.Element, "ReleaseYear", "");
                XmlNode dateAddedNode = videoDoc.CreateNode(XmlNodeType.Element, "DateAdded", "");
                XmlNode sortByNode = videoDoc.CreateNode(XmlNodeType.Element, "SortBy", "");
                XmlNode lastPlayedNode = videoDoc.CreateNode(XmlNodeType.Element, "LastPlayed", "");
                XmlNode mediaTypeNode = videoDoc.CreateNode(XmlNodeType.Element, "MediaType", "");
                XmlNode taglineNode = videoDoc.CreateNode(XmlNodeType.Element, "Tagline", "");
                XmlNode lastPlayPositionNode = videoDoc.CreateNode(XmlNodeType.Element, "LastPlayPosition", "");
                XmlNode goofsNode = videoDoc.CreateNode(XmlNodeType.Element, "Goofs", "");
                XmlNode triviaNode = videoDoc.CreateNode(XmlNodeType.Element, "Trivia", "");
                XmlNode shortDescriptionNode = videoDoc.CreateNode(XmlNodeType.Element, "ShortDescription", "");
                XmlNode seasonNode = videoDoc.CreateNode(XmlNodeType.Element, "Season", "");
                XmlNode episodeNumNode = videoDoc.CreateNode(XmlNodeType.Element, "EpisodeNum", "");
                XmlNode seriesNameNode = videoDoc.CreateNode(XmlNodeType.Element, "SeriesName", "");
                XmlNode seriesIMDBNumNode = videoDoc.CreateNode(XmlNodeType.Element, "SeriesIMDBNum", "");
                XmlNode seriesImageNode = videoDoc.CreateNode(XmlNodeType.Element, "SeriesImage", "");
                XmlNode seriesDescriptionNode = videoDoc.CreateNode(XmlNodeType.Element, "SeriesDescription", "");
                XmlNode artistNode = videoDoc.CreateNode(XmlNodeType.Element, "Artist", "");
                XmlNode albumNode = videoDoc.CreateNode(XmlNodeType.Element, "Album", "");
                XmlNode lyricsNode = videoDoc.CreateNode(XmlNodeType.Element, "Lyrics", "");
                XmlNode lengthNode = videoDoc.CreateNode(XmlNodeType.Element, "Length", "");
                XmlNode recordingIDNode = videoDoc.CreateNode(XmlNodeType.Element, "RecordingID", "");
                XmlNode artistIDNode = videoDoc.CreateNode(XmlNodeType.Element, "ArtistID", "");
                XmlNode releaseIDNode = videoDoc.CreateNode(XmlNodeType.Element, "ReleaseID", "");

                dateAddedNode.InnerText = fileToAdd.CreationTime.ToShortDateString();

                if (musicBrainz != null)
                {
                    //if (imdb.MovieTitle.Length > 0 && !imdb.MovieTitle.Contains("IMDb Search") && !imdb.MovieTitle.Contains("Find - IMDb"))
                    if (newMedia.Title != null && newMedia.Title.Length > 0)// && !newMedia.Title.Contains("IMDb Search") && !newMedia.Title.Contains("Find - IMDb"))
                    {
                        nameNode.InnerText = newMedia.Title;
                    }
                    else
                        nameNode.InnerText = formattedFileName;

                    ratingNode.InnerText = newMedia.Rating;
                    ratingDescriptionNode.InnerText = newMedia.RatingDescription;
                    taglineNode.InnerText = newMedia.TagLine;
                    descriptionNode.InnerText = newMedia.Description;
                    categoryNode.InnerText = newMedia.category;
                    grammarNode.InnerText = nameNode.InnerText;
                    coverImageNode.InnerText = newMedia.coverImage;
                    imdbNumNode.InnerText = newMedia.IMDBNum;
                    directorNode.InnerText = newMedia.Director;
                    releaseYearNode.InnerText = newMedia.ReleaseYear;
                    goofsNode.InnerText = newMedia.Goofs;
                    triviaNode.InnerText = newMedia.Trivia;
                    shortDescriptionNode.InnerText = newMedia.ShortDescription;
                    seasonNode.InnerText = newMedia.Season;
                    episodeNumNode.InnerText = newMedia.EpisodeNum;
                    seriesNameNode.InnerText = newMedia.SeriesName;
                    seriesIMDBNumNode.InnerText = newMedia.SeriesIMDBNum;
                    seriesImageNode.InnerText = newMedia.SeriesImage;
                    seriesDescriptionNode.InnerText = newMedia.SeriesDescription;
                    artistNode.InnerText = newMedia.Artist;
                    albumNode.InnerText = newMedia.Album;
                    lyricsNode.InnerText = newMedia.Lyrics;
                    lengthNode.InnerText = newMedia.Length;
                    recordingIDNode.InnerText = newMedia.RecordingID;
                    artistIDNode.InnerText = newMedia.ArtistID;
                    releaseIDNode.InnerText = newMedia.ReleaseID;
                }

                //if (dsMedia != null && dsMedia.Tables.Count > 0)
                //    IDNode.InnerText = dsMedia.Tables[0].Rows.Count.ToString();
                //else
                //    IDNode.InnerText = "1";

                IDNode.InnerText = videoDoc.SelectNodes("/mediaFiles/media").Count.ToString();

                sortByNode.InnerText = formatSortBy(nameNode.InnerText);
                mediaTypeNode.InnerText = foundMediaType;

                newNode.AppendChild(IDNode);
                newNode.AppendChild(nameNode);
                newNode.AppendChild(previousTitleNode);
                newNode.AppendChild(performersNode);
                newNode.AppendChild(ratingNode);
                newNode.AppendChild(ratingDescriptionNode);
                newNode.AppendChild(taglineNode);
                newNode.AppendChild(descriptionNode);
                newNode.AppendChild(starsNode);
                newNode.AppendChild(categoryNode);
                newNode.AppendChild(directorNode);
                newNode.AppendChild(releaseYearNode);
                newNode.AppendChild(dateAddedNode);

                //string[] categories = imdb.Genre.Split('/');

                //for (int x = 0; x < categories.Length; x++)
                //{
                //    XmlNode categoryNode = tempDoc.CreateNode(XmlNodeType.Element, "category", "");
                //    categoryNode.InnerText = categories[x].Trim();
                //    newNode.AppendChild(categoryNode);
                //    break;
                //}

                newNode.AppendChild(timesPlayedNode);
                newNode.AppendChild(filePathNode);
                newNode.AppendChild(grammarNode);
                newNode.AppendChild(coverImageNode);
                newNode.AppendChild(imdbNumNode);
                newNode.AppendChild(sortByNode);
                newNode.AppendChild(lastPlayedNode);
                newNode.AppendChild(mediaTypeNode);
                newNode.AppendChild(lastPlayPositionNode);
                newNode.AppendChild(goofsNode);
                newNode.AppendChild(triviaNode);
                newNode.AppendChild(shortDescriptionNode);
                newNode.AppendChild(seasonNode);
                newNode.AppendChild(episodeNumNode);
                newNode.AppendChild(seriesNameNode);
                newNode.AppendChild(seriesIMDBNumNode);
                newNode.AppendChild(seriesImageNode);
                newNode.AppendChild(seriesDescriptionNode);
                newNode.AppendChild(artistNode);
                newNode.AppendChild(albumNode);
                newNode.AppendChild(lyricsNode);
                newNode.AppendChild(lengthNode);
                newNode.AppendChild(recordingIDNode);
                newNode.AppendChild(artistIDNode);
                newNode.AppendChild(releaseIDNode);
                videoDoc["mediaFiles"].AppendChild(newNode);

                videoDoc.Save(MediaXMLFilePath);
            }

            return videoDoc;
        }

        private XmlDocument addDVDFolder(XmlDocument videoDoc, DirectoryInfo folderToAdd, FileSystemInfo fileToAdd, bool bestMatch, string foundMediaType)
        {
            IMDBScraper imdb = null;
            Media newMedia = null;
            string folderName = "";
            string formattedFolderName = "";
            folderName = folderToAdd.Name;

            //if (fileToAdd.FullName.ToLower().Contains("justindownloads"))
            //    fileName = fileName.Split('_')[0];

            formattedFolderName = FormatNameString(folderName);

            XmlNode xmlDVDNode = videoDoc.SelectSingleNode("/mediaFiles/media[contains(filePath,'" + folderToAdd.FullName + "')]");

            //dsMedia.Tables[0].DefaultView.RowFilter = "len(category) = 0 and filepath LIKE '*" + folderToAdd + "*'";

            //dsMedia.Tables[0].DefaultView.Sort = "SortBy";
            //foundData = dsMedia.Tables[0].DefaultView;

            //if (!isDuplicate(videoDoc, fileToAdd.FullName, "mediaFiles/media/filePath"))
            if (xmlDVDNode == null)
            {
                try
                {
                    if (foundMediaType.ToLower() == "movies" || foundMediaType.ToLower() == "tv")
                    {
                        newMedia = getTVSeriesInfoFromFileName(folderName, null);

                        if (newMedia.MediaType == null || newMedia.MediaType.Trim().Length == 0)
                            newMedia.MediaType = foundMediaType;

                        //get imdb info
                        imdb = new IMDBScraper();
                        newMedia = imdb.getInfoByTitle(formattedFolderName, bestMatch, newMedia);

                        //set mediaType to tv if it's a justin download and it wasn't found on imdb
                        if ((newMedia.IMDBNum != null && newMedia.IMDBNum.Trim().Length == 0) && (newMedia.filePath != null && newMedia.filePath.ToLower().Contains("justindownloads")))
                            newMedia.MediaType = "TV";

                        if (newMedia.MediaType != null)
                            foundMediaType = newMedia.MediaType;
                        else
                            newMedia.MediaType = foundMediaType;

                        if (foundMediaType.ToLower() == "tv")
                            newMedia = getEpisodeInfo(newMedia);


                        //imdb.getPhoto(newMedia);
                    }
                }
                catch (Exception ex)
                {
                    imdb = null;

                    if (!ignoreIMDBErrors && !bestMatch)
                    {
                        internetConnectionDown connectionDown = new internetConnectionDown();
                        DialogResult result = connectionDown.ShowDialog();

                        if (result == DialogResult.Retry)
                            ignoreIMDBErrors = false;
                        else if (result == DialogResult.Ignore)
                            ignoreIMDBErrors = true;
                        else
                            ignoreIMDBErrors = false;
                    }

                    Tools.WriteToFile(Tools.errorFile, "addDVDFolder error: " + ex.Message);
                }

                XmlNode newNode = videoDoc.CreateNode(XmlNodeType.Element, "media", "");
                XmlNode IDNode = videoDoc.CreateNode(XmlNodeType.Element, "ID", "");
                XmlNode previousTitleNode = videoDoc.CreateNode(XmlNodeType.Element, "previousTitle", "");
                previousTitleNode.InnerText = formattedFolderName;
                XmlNode nameNode = videoDoc.CreateNode(XmlNodeType.Element, "title", "");
                XmlNode performersNode = videoDoc.CreateNode(XmlNodeType.Element, "performers", "");
                XmlNode ratingNode = videoDoc.CreateNode(XmlNodeType.Element, "rating", "");
                XmlNode ratingDescriptionNode = videoDoc.CreateNode(XmlNodeType.Element, "ratingDescription", "");
                XmlNode descriptionNode = videoDoc.CreateNode(XmlNodeType.Element, "description", "");
                XmlNode starsNode = videoDoc.CreateNode(XmlNodeType.Element, "stars", "");
                XmlNode categoryNode = videoDoc.CreateNode(XmlNodeType.Element, "category", "");
                XmlNode timesPlayedNode = videoDoc.CreateNode(XmlNodeType.Element, "timesPlayed", "");
                timesPlayedNode.InnerText = "0";
                XmlNode filePathNode = videoDoc.CreateNode(XmlNodeType.Element, "filePath", "");
                filePathNode.InnerText = fileToAdd.FullName;
                XmlNode grammarNode = videoDoc.CreateNode(XmlNodeType.Element, "grammar", "");
                XmlNode coverImageNode = videoDoc.CreateNode(XmlNodeType.Element, "coverImage", "");
                XmlNode imdbNumNode = videoDoc.CreateNode(XmlNodeType.Element, "IMDBNum", "");
                XmlNode directorNode = videoDoc.CreateNode(XmlNodeType.Element, "Director", "");
                XmlNode releaseYearNode = videoDoc.CreateNode(XmlNodeType.Element, "ReleaseYear", "");
                XmlNode dateAddedNode = videoDoc.CreateNode(XmlNodeType.Element, "DateAdded", "");
                XmlNode sortByNode = videoDoc.CreateNode(XmlNodeType.Element, "SortBy", "");
                XmlNode lastPlayedNode = videoDoc.CreateNode(XmlNodeType.Element, "LastPlayed", "");
                XmlNode mediaTypeNode = videoDoc.CreateNode(XmlNodeType.Element, "MediaType", "");
                XmlNode taglineNode = videoDoc.CreateNode(XmlNodeType.Element, "Tagline", "");
                XmlNode lastPlayPositionNode = videoDoc.CreateNode(XmlNodeType.Element, "LastPlayPosition", "");
                XmlNode goofsNode = videoDoc.CreateNode(XmlNodeType.Element, "Goofs", "");
                XmlNode triviaNode = videoDoc.CreateNode(XmlNodeType.Element, "Trivia", "");
                XmlNode shortDescriptionNode = videoDoc.CreateNode(XmlNodeType.Element, "ShortDescription", "");
                XmlNode seasonNode = videoDoc.CreateNode(XmlNodeType.Element, "Season", "");
                XmlNode episodeNumNode = videoDoc.CreateNode(XmlNodeType.Element, "EpisodeNum", "");
                XmlNode seriesNameNode = videoDoc.CreateNode(XmlNodeType.Element, "SeriesName", "");
                XmlNode seriesIMDBNumNode = videoDoc.CreateNode(XmlNodeType.Element, "SeriesIMDBNum", "");
                XmlNode seriesImageNode = videoDoc.CreateNode(XmlNodeType.Element, "SeriesImage", "");
                XmlNode seriesDescriptionNode = videoDoc.CreateNode(XmlNodeType.Element, "SeriesDescription", "");

                dateAddedNode.InnerText = folderToAdd.CreationTime.ToShortDateString();

                if (imdb != null)
                {
                    //if (imdb.MovieTitle.Length > 0 && !imdb.MovieTitle.Contains("IMDb Search") && !imdb.MovieTitle.Contains("Find - IMDb"))
                    if (newMedia.Title.Length > 0 && !newMedia.Title.Contains("IMDb Search") && !newMedia.Title.Contains("Find - IMDb"))
                    {
                        //clean up title
                        //string tempTitle = imdb.MovieTitle;
                        //tempTitle = tempTitle.Replace("&#x22;", "\"");
                        //tempTitle = tempTitle.Replace("&#x26;", "&");
                        //tempTitle = tempTitle.Replace("&#x27;", "'");

                        nameNode.InnerText = newMedia.Title;
                    }
                    else
                        nameNode.InnerText = formattedFolderName;

                    ratingNode.InnerText = newMedia.Rating;
                    ratingDescriptionNode.InnerText = newMedia.RatingDescription;
                    taglineNode.InnerText = newMedia.TagLine;
                    descriptionNode.InnerText = newMedia.Description;
                    categoryNode.InnerText = newMedia.category;

                    if (newMedia.Title.Length > 0 && !newMedia.Title.Contains("IMDb Search") && !newMedia.Title.Contains("Find - IMDb"))
                        grammarNode.InnerText = newMedia.Title;
                    else
                        grammarNode.InnerText = formattedFolderName;

                    coverImageNode.InnerText = newMedia.coverImage;
                    imdbNumNode.InnerText = newMedia.IMDBNum;
                    directorNode.InnerText = newMedia.Director;
                    releaseYearNode.InnerText = newMedia.ReleaseYear;
                    goofsNode.InnerText = newMedia.Goofs;
                    triviaNode.InnerText = newMedia.Trivia;
                    shortDescriptionNode.InnerText = newMedia.ShortDescription;
                    seasonNode.InnerText = newMedia.Season;
                    episodeNumNode.InnerText = newMedia.EpisodeNum;
                    seriesNameNode.InnerText = newMedia.SeriesName;
                    seriesIMDBNumNode.InnerText = newMedia.SeriesIMDBNum;
                    seriesImageNode.InnerText = newMedia.SeriesImage;
                    seriesDescriptionNode.InnerText = newMedia.SeriesDescription;
                }
                else
                {
                    if (foundMediaType.ToLower() == "pictures")
                        coverImageNode.InnerText = filePathNode.InnerText;

                    nameNode.InnerText = formattedFolderName;
                    grammarNode.InnerText = formattedFolderName;
                }

                sortByNode.InnerText = formatSortBy(nameNode.InnerText);
                mediaTypeNode.InnerText = foundMediaType;

                if (filePathNode.InnerText.ToLower().Contains("justindownloads"))
                {
                    if (categoryNode.InnerText.Length > 0)
                        categoryNode.InnerText += "|";

                    categoryNode.InnerText += "Justin Downloads";
                }

                //if (dsMedia != null && dsMedia.Tables.Count > 0)
                //    IDNode.InnerText = dsMedia.Tables[0].Rows.Count.ToString();
                //else
                //    IDNode.InnerText = "1";

                IDNode.InnerText = videoDoc.SelectNodes("/mediaFiles/media").Count.ToString();

                newNode.AppendChild(IDNode);
                newNode.AppendChild(nameNode);
                newNode.AppendChild(previousTitleNode);
                newNode.AppendChild(performersNode);
                newNode.AppendChild(ratingNode);
                newNode.AppendChild(ratingDescriptionNode);
                newNode.AppendChild(taglineNode);
                newNode.AppendChild(descriptionNode);
                newNode.AppendChild(starsNode);
                newNode.AppendChild(categoryNode);
                newNode.AppendChild(directorNode);
                newNode.AppendChild(releaseYearNode);
                newNode.AppendChild(dateAddedNode);

                //string[] categories = imdb.Genre.Split('/');

                //for (int x = 0; x < categories.Length; x++)
                //{
                //    XmlNode categoryNode = tempDoc.CreateNode(XmlNodeType.Element, "category", "");
                //    categoryNode.InnerText = categories[x].Trim();
                //    newNode.AppendChild(categoryNode);
                //    break;
                //}

                newNode.AppendChild(timesPlayedNode);
                newNode.AppendChild(filePathNode);
                newNode.AppendChild(grammarNode);
                newNode.AppendChild(coverImageNode);
                newNode.AppendChild(imdbNumNode);
                newNode.AppendChild(sortByNode);
                newNode.AppendChild(lastPlayedNode);
                newNode.AppendChild(mediaTypeNode);
                newNode.AppendChild(lastPlayPositionNode);
                newNode.AppendChild(goofsNode);
                newNode.AppendChild(triviaNode);
                newNode.AppendChild(shortDescriptionNode);
                newNode.AppendChild(seasonNode);
                newNode.AppendChild(episodeNumNode);
                newNode.AppendChild(seriesNameNode);
                newNode.AppendChild(seriesIMDBNumNode);
                newNode.AppendChild(seriesImageNode);
                newNode.AppendChild(seriesDescriptionNode);
                videoDoc["mediaFiles"].AppendChild(newNode);

                videoDoc.Save(MediaXMLFilePath);
            }
            else//add fileToAdd info to filepath
            {
                //make sure this exact record doesn't already exist
                XmlNode xmlExistingNode = videoDoc.SelectSingleNode("/mediaFiles/media[filePath='" + fileToAdd.FullName.Replace("'", "''") + "']");

                if(xmlExistingNode == null)
                {
                    (xmlDVDNode["filePath"]).FirstChild.Value += "|" + fileToAdd.FullName;

                    videoDoc.Save(MediaXMLFilePath);
                }
            }

            return videoDoc;
        }

        /// <summary>
        /// Add video file info to given xmlDocument
        /// </summary>
        /// <param name="tempDoc"></param>
        /// <param name="fileToAdd"></param>
        /// <param name="bestMatch"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private XmlDocument addGamesFile(XmlDocument gameDoc, FileSystemInfo fileToAdd, bool bestMatch, string foundMediaType)
        {
            IMDBScraper imdb = null;
            string fileName = "";
            string formattedFileName = "";
            fileName = System.Text.RegularExpressions.Regex.Replace(fileToAdd.Name, fileToAdd.Extension, "");

            if (fileToAdd.FullName.ToLower().Contains("justindownloads"))
                fileName = fileName.Split('_')[0];

            formattedFileName = FormatNameString(fileName);

            if (!isDuplicate(gameDoc, fileToAdd.FullName, "gameFiles/game/filePath"))
            {
                XmlNode newNode = gameDoc.CreateNode(XmlNodeType.Element, "game", "");
                XmlNode previousTitleNode = gameDoc.CreateNode(XmlNodeType.Element, "previousTitle", "");
                previousTitleNode.InnerText = formattedFileName;
                XmlNode nameNode = gameDoc.CreateNode(XmlNodeType.Element, "title", "");
                XmlNode ratingNode = gameDoc.CreateNode(XmlNodeType.Element, "rating", "");
                XmlNode ratingDescriptionNode = gameDoc.CreateNode(XmlNodeType.Element, "ratingDescription", "");
                XmlNode descriptionNode = gameDoc.CreateNode(XmlNodeType.Element, "fullDescription", "");
                XmlNode starsNode = gameDoc.CreateNode(XmlNodeType.Element, "stars", "");
                XmlNode categoryNode = gameDoc.CreateNode(XmlNodeType.Element, "genre", "");
                XmlNode timesPlayedNode = gameDoc.CreateNode(XmlNodeType.Element, "timesPlayed", "");
                timesPlayedNode.InnerText = "0";
                XmlNode filePathNode = gameDoc.CreateNode(XmlNodeType.Element, "filePath", "");
                filePathNode.InnerText = fileToAdd.FullName;
                XmlNode grammarNode = gameDoc.CreateNode(XmlNodeType.Element, "grammar", "");
                XmlNode coverImageNode = gameDoc.CreateNode(XmlNodeType.Element, "thumbnailImage", "");
                XmlNode releaseYearNode = gameDoc.CreateNode(XmlNodeType.Element, "ReleaseYear", "");
                XmlNode dateAddedNode = gameDoc.CreateNode(XmlNodeType.Element, "DateAdded", "");
                XmlNode sortByNode = gameDoc.CreateNode(XmlNodeType.Element, "SortBy", "");
                XmlNode lastPlayedNode = gameDoc.CreateNode(XmlNodeType.Element, "LastPlayed", "");
                XmlNode mediaTypeNode = gameDoc.CreateNode(XmlNodeType.Element, "MediaType", "");
                XmlNode triviaNode = gameDoc.CreateNode(XmlNodeType.Element, "Trivia", "");
                XmlNode shortDescriptionNode = gameDoc.CreateNode(XmlNodeType.Element, "ShortDescription", "");
                XmlNode seasonNode = gameDoc.CreateNode(XmlNodeType.Element, "Season", "");
                XmlNode episodeNumNode = gameDoc.CreateNode(XmlNodeType.Element, "EpisodeNum", "");
                XmlNode seriesNameNode = gameDoc.CreateNode(XmlNodeType.Element, "SeriesName", "");
                XmlNode seriesIMDBNumNode = gameDoc.CreateNode(XmlNodeType.Element, "SeriesIMDBNum", "");
                XmlNode seriesImageNode = gameDoc.CreateNode(XmlNodeType.Element, "SeriesImage", "");
                XmlNode seriesDescriptionNode = gameDoc.CreateNode(XmlNodeType.Element, "SeriesDescription", "");

                dateAddedNode.InnerText = fileToAdd.CreationTime.ToShortDateString();

                sortByNode.InnerText = formatSortBy(nameNode.InnerText);
                mediaTypeNode.InnerText = foundMediaType;

                if (filePathNode.InnerText.ToLower().Contains("justindownloads"))
                {
                    if (categoryNode.InnerText.Length > 0)
                        categoryNode.InnerText += "|";

                    categoryNode.InnerText += "Justin Downloads";
                }

                newNode.AppendChild(nameNode);
                newNode.AppendChild(previousTitleNode);
                newNode.AppendChild(ratingNode);
                newNode.AppendChild(ratingDescriptionNode);
                newNode.AppendChild(descriptionNode);
                newNode.AppendChild(starsNode);
                newNode.AppendChild(categoryNode);
                newNode.AppendChild(releaseYearNode);
                newNode.AppendChild(dateAddedNode);
                newNode.AppendChild(timesPlayedNode);
                newNode.AppendChild(filePathNode);
                newNode.AppendChild(grammarNode);
                newNode.AppendChild(coverImageNode);
                newNode.AppendChild(sortByNode);
                newNode.AppendChild(lastPlayedNode);
                newNode.AppendChild(mediaTypeNode);
                newNode.AppendChild(triviaNode);
                newNode.AppendChild(shortDescriptionNode);
                gameDoc["gameFiles"].AppendChild(newNode);

                gameDoc.Save(GameXMLFilePath);
            }

            return gameDoc;
        }
        
        /// <summary>
        /// Add audio file info to given xmlDocument
        /// </summary>
        /// <param name="tempDoc"></param>
        /// <param name="fileToAdd"></param>
        /// <param name="bestMatch"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private XmlDocument addAudioFile(XmlDocument audioDoc, FileSystemInfo fileToAdd, bool bestMatch, string foundMediaType)
        {
            IMDBScraper imdb = null;
            string fileName = "";
            string formattedFileName = "";
            fileName = System.Text.RegularExpressions.Regex.Replace(fileToAdd.Name, fileToAdd.Extension, "");
            formattedFileName = FormatNameString(fileName);
            //string foundMediaType = getMediaType(fileToAdd.Extension);

            //try
            //{
            //    //get imdb info
            //    imdb = new IMDBScraper();
            //    Media newMedia = imdb.getInfoByTitle(fileName, bestMatch);

            //    foundMediaType = newMedia.MediaType;
            //    //imdb.getPhoto(newMedia);
            //}
            //catch (Exception ex)
            //{
            //    imdb = null;

            //    if (!ignoreIMDBErrors)
            //    {
            //        internetConnectionDown connectionDown = new internetConnectionDown();
            //        DialogResult result = connectionDown.ShowDialog();

            //        if (result == DialogResult.Retry)
            //            ignoreIMDBErrors = false;
            //        else if (result == DialogResult.Ignore)
            //            ignoreIMDBErrors = true;
            //        else
            //            ignoreIMDBErrors = false;
            //    }

            //    //System.Windows.Forms.MessageBox.Show("Your internet connection is down.  I can't get any media information right now.  Try again later.");

            //    Tools.WriteToFile(Tools.errorFile, "lookForMedia error: " + ex.Message);
            //}

            XmlNode newNode = audioDoc.CreateNode(XmlNodeType.Element, "media", "");
            XmlNode previousTitleNode = audioDoc.CreateNode(XmlNodeType.Element, "previousTitle", "");
            previousTitleNode.InnerText = formattedFileName;
            XmlNode nameNode = audioDoc.CreateNode(XmlNodeType.Element, "title", "");
            XmlNode performersNode = audioDoc.CreateNode(XmlNodeType.Element, "performers", "");
            XmlNode ratingNode = audioDoc.CreateNode(XmlNodeType.Element, "rating", "");
            XmlNode ratingDescriptionNode = audioDoc.CreateNode(XmlNodeType.Element, "ratingDescription", "");
            XmlNode descriptionNode = audioDoc.CreateNode(XmlNodeType.Element, "description", "");
            XmlNode starsNode = audioDoc.CreateNode(XmlNodeType.Element, "stars", "");
            XmlNode categoryNode = audioDoc.CreateNode(XmlNodeType.Element, "category", "");
            XmlNode timesPlayedNode = audioDoc.CreateNode(XmlNodeType.Element, "timesPlayed", "");
            timesPlayedNode.InnerText = "0";
            XmlNode filePathNode = audioDoc.CreateNode(XmlNodeType.Element, "filePath", "");
            //filePathNode.InnerText = location.InnerText + "\\" + fileToAdd.Name;
            //filePathNode.InnerText = location + "\\" + fileToAdd.Name;
            filePathNode.InnerText = fileToAdd.FullName;
            XmlNode grammarNode = audioDoc.CreateNode(XmlNodeType.Element, "grammar", "");
            XmlNode coverImageNode = audioDoc.CreateNode(XmlNodeType.Element, "coverImage", "");
            XmlNode imdbNumNode = audioDoc.CreateNode(XmlNodeType.Element, "IMDBNum", "");
            XmlNode directorNode = audioDoc.CreateNode(XmlNodeType.Element, "Director", "");
            XmlNode releaseYearNode = audioDoc.CreateNode(XmlNodeType.Element, "ReleaseYear", "");
            XmlNode dateAddedNode = audioDoc.CreateNode(XmlNodeType.Element, "DateAdded", "");
            XmlNode sortByNode = audioDoc.CreateNode(XmlNodeType.Element, "SortBy", "");
            XmlNode lastPlayedNode = audioDoc.CreateNode(XmlNodeType.Element, "LastPlayed", "");
            XmlNode mediaTypeNode = audioDoc.CreateNode(XmlNodeType.Element, "MediaType", "");
            XmlNode taglineNode = audioDoc.CreateNode(XmlNodeType.Element, "Tagline", "");

            dateAddedNode.InnerText = fileToAdd.CreationTime.ToShortDateString();

            if (imdb != null)
            {
                if (imdb.MovieTitle.Length > 0 && !imdb.MovieTitle.Contains("IMDb Search"))
                {
                    //clean up title
                    //string tempTitle = imdb.MovieTitle;
                    //tempTitle = tempTitle.Replace("&#x22;", "\"");
                    //tempTitle = tempTitle.Replace("&#x26;", "&");
                    //tempTitle = tempTitle.Replace("&#x27;", "'");

                    nameNode.InnerText = imdb.MovieTitle;
                }
                else
                    nameNode.InnerText = formattedFileName;

                ratingNode.InnerText = imdb.Rating;
                ratingDescriptionNode.InnerText = imdb.RatingDescription;
                taglineNode.InnerText = imdb.TagLine;
                descriptionNode.InnerText = imdb.Description;
                categoryNode.InnerText = imdb.Genre;

                if (imdb.MovieTitle.Length > 0)
                    grammarNode.InnerText = imdb.MovieTitle;
                else
                    grammarNode.InnerText = formattedFileName;

                coverImageNode.InnerText = imdb.CoverImage;
                imdbNumNode.InnerText = imdb.IMDBNum;
                directorNode.InnerText = imdb.Director;
                releaseYearNode.InnerText = imdb.ReleaseYear;
            }
            else
            {
                nameNode.InnerText = formattedFileName;
                grammarNode.InnerText = formattedFileName;
            }

            sortByNode.InnerText = formatSortBy(nameNode.InnerText);
            mediaTypeNode.InnerText = foundMediaType;

            newNode.AppendChild(nameNode);
            newNode.AppendChild(previousTitleNode);
            newNode.AppendChild(performersNode);
            newNode.AppendChild(ratingNode);
            newNode.AppendChild(ratingDescriptionNode);
            newNode.AppendChild(taglineNode);
            newNode.AppendChild(descriptionNode);
            newNode.AppendChild(starsNode);
            newNode.AppendChild(categoryNode);
            newNode.AppendChild(directorNode);
            newNode.AppendChild(releaseYearNode);
            newNode.AppendChild(dateAddedNode);

            //string[] categories = imdb.Genre.Split('/');

            //for (int x = 0; x < categories.Length; x++)
            //{
            //    XmlNode categoryNode = tempDoc.CreateNode(XmlNodeType.Element, "category", "");
            //    categoryNode.InnerText = categories[x].Trim();
            //    newNode.AppendChild(categoryNode);
            //    break;
            //}

            newNode.AppendChild(timesPlayedNode);
            newNode.AppendChild(filePathNode);
            newNode.AppendChild(grammarNode);
            newNode.AppendChild(coverImageNode);
            newNode.AppendChild(imdbNumNode);
            newNode.AppendChild(sortByNode);
            newNode.AppendChild(lastPlayedNode);
            newNode.AppendChild(mediaTypeNode);
            audioDoc["mediaFiles"].AppendChild(newNode);

            return audioDoc;
        }

        /// <summary>
        /// Add image file info to given xmlDocument
        /// </summary>
        /// <param name="imageDoc"></param>
        /// <param name="fileToAdd"></param>
        /// <param name="bestMatch"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private XmlDocument addImageFile(XmlDocument imageDoc, FileSystemInfo fileToAdd, bool bestMatch, string foundMediaType)
        {
            string fileName = "";
            string formattedFileName = "";
            fileName = System.Text.RegularExpressions.Regex.Replace(fileToAdd.Name, fileToAdd.Extension, "");
            formattedFileName = FormatNameString(fileName);
            
            XmlNode newNode = imageDoc.CreateNode(XmlNodeType.Element, "image", "");
            XmlNode nameNode = imageDoc.CreateNode(XmlNodeType.Element, "title", "");
            XmlNode descriptionNode = imageDoc.CreateNode(XmlNodeType.Element, "description", "");
            XmlNode categoryNode = imageDoc.CreateNode(XmlNodeType.Element, "category", "");
            XmlNode timesViewedNode = imageDoc.CreateNode(XmlNodeType.Element, "timesViewed", "");
            timesViewedNode.InnerText = "0";
            XmlNode filePathNode = imageDoc.CreateNode(XmlNodeType.Element, "filePath", "");
            filePathNode.InnerText = fileToAdd.FullName;
            XmlNode grammarNode = imageDoc.CreateNode(XmlNodeType.Element, "grammar", "");
            XmlNode dateAddedNode = imageDoc.CreateNode(XmlNodeType.Element, "DateAdded", "");
            XmlNode sortByNode = imageDoc.CreateNode(XmlNodeType.Element, "SortBy", "");
            XmlNode mediaTypeNode = imageDoc.CreateNode(XmlNodeType.Element, "MediaType", "");
            XmlNode coverImageNode = imageDoc.CreateNode(XmlNodeType.Element, "coverImage", "");

            coverImageNode.InnerText = fileToAdd.FullName;
            dateAddedNode.InnerText = fileToAdd.CreationTime.ToShortDateString();
            nameNode.InnerText = formattedFileName;
            grammarNode.InnerText = formattedFileName;
            sortByNode.InnerText = formatSortBy(nameNode.InnerText);
            mediaTypeNode.InnerText = foundMediaType;

            newNode.AppendChild(nameNode);
            newNode.AppendChild(descriptionNode);
            newNode.AppendChild(categoryNode);
            newNode.AppendChild(dateAddedNode);

            //string[] categories = imdb.Genre.Split('/');

            //for (int x = 0; x < categories.Length; x++)
            //{
            //    XmlNode categoryNode = tempDoc.CreateNode(XmlNodeType.Element, "category", "");
            //    categoryNode.InnerText = categories[x].Trim();
            //    newNode.AppendChild(categoryNode);
            //    break;
            //}

            newNode.AppendChild(coverImageNode);
            newNode.AppendChild(timesViewedNode);
            newNode.AppendChild(filePathNode);
            newNode.AppendChild(grammarNode);
            newNode.AppendChild(sortByNode);
            newNode.AppendChild(mediaTypeNode);
            imageDoc["images"].AppendChild(newNode);

            return imageDoc;
        }

        public XmlDocument AddMediaFile(FileSystemInfo fileToAdd, bool bestMatch)
        {
            XmlDocument tempDoc = new XmlDocument();

            try
            {
                if (compareExtension(fileToAdd.Extension))
                {
                    string foundMediaType = "";

                    if (File.Exists(MediaXMLFilePath))
                    {
                        tempDoc.Load(MediaXMLFilePath);
                    }
                    else
                    {
                        //add base node to tempDoc
                        XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "mediaFiles", "");

                        tempDoc.AppendChild(baseNode);
                    }

                    if (fileToAdd.FullName.ToLower().Contains("justindownloads"))
                        foundMediaType = "TV";
                    else
                        foundMediaType = getMediaType(fileToAdd.Extension);

                    DirectoryInfo dvdFolder = null;

                    if (fileToAdd.Extension.ToLower() == "vob")//these are dvd files - the folder needs to be played all at once
                    {
                        DirectoryInfo fl = new DirectoryInfo(fileToAdd.FullName);

                        if (fl.Parent.Name.ToLower() == "video_ts")
                        {
                            dvdFolder = fl.Parent.Parent;
                        }
                        else
                            dvdFolder = fl.Parent;
                    }

                    if (foundMediaType.ToLower() == "images")
                    {
                        tempDoc = addImageFile(tempDoc, fileToAdd, bestMatch, foundMediaType);
                    }
                    else if (dvdFolder != null)
                    {
                        tempDoc = addDVDFolder(tempDoc, dvdFolder, fileToAdd, bestMatch, foundMediaType);
                    }
                    else
                    {
                        //right now everything is being added as video
                        tempDoc = addVideoFile(tempDoc, fileToAdd, bestMatch, foundMediaType);
                    }

                    dsMedia = GetMedia();
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return tempDoc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xDoc">the document the value will be added to</param>
        /// <param name="valueToAdd">value to add to the named node</param>
        /// <param name="nodePath">The name and path of the node that the value will get added to</param>
        /// <returns></returns>
        private bool isDuplicate(XmlDocument xDoc, string valueToAdd, string nodePath)
        {
            bool foundDuplicate = false;

            foreach(XmlNode node in xDoc.SelectNodes(nodePath))
            {
                if (node.InnerText.ToLower() == valueToAdd.ToLower())
                {
                    foundDuplicate = true;
                    break;
                }
            }

            return foundDuplicate;
        }

        #region depricated code
        /// <summary>
        /// Look for files and update speech files to match
        /// </summary>
        /// <param name="location"></param>
        /// <param name="checkSubFolders"></param>
        /// <param name="tempDoc"></param>
        //public void FindFiles(string location,bool checkSubFolders,XmlDocument tempDoc)
        //{
        //    try
        //    {
        //        bool foundNewFiles = false;
        //        string fileName = "";
        //        string category = "";
        //        string formattedFileName = "";
        //        DirectoryInfo fl = new DirectoryInfo(location);
				
        //        if(tempDoc == null)
        //        {
        //            tempDoc = new XmlDocument();
        //            tempDoc.Load(mediaFilePath);
        //        }

        //        XmlDocument xMediaSpeech = new XmlDocument();
        //        xMediaSpeech.Load(System.Windows.Forms.Application.StartupPath + "\\speech\\vocab\\xmlAll.xml");
        //        //FileInfo fiMedia = new FileInfo(Application.StartupPath + "\\speech\\vocab\\xmlMedia.xml");

        //        //if (fiMedia.Exists && xMediaSpeech != null)
        //        //    xMediaSpeech.Load(Application.StartupPath + "\\speech\\vocab\\xmlMedia.xml");
        //        //else
        //        //    xMediaSpeech.LoadXml(generateMediaSpeechFile().InnerXml);

        //        DataSet ds = new DataSet();
        //        ds.ReadXml(mediaFilePath);

        //        if (ds.Tables.Count > 0)//there is media
        //        {
        //            DataColumn[] pk = new DataColumn[1];
        //            pk[0] = ds.Tables[0].Columns["filePath"];
        //            ds.Tables[0].PrimaryKey = pk;
        //        }
						
        //        foreach (FileSystemInfo entry in fl.GetFileSystemInfos())
        //        {
        //            if(entry.Extension.Length > 0)//this is a file
        //            {
        //                //check extension and make sure it's a type we recognize
        //                if(compareExtension(entry.Extension))
        //                {
        //                    //add to media file
        //                    if (ds.Tables.Count == 0 || (ds.Tables[0].Rows.Find(location + "\\" + entry.Name.ToString().Trim()) == null && ds.Tables[0].Rows.Find(location + "\\" + entry.Name.ToString().Trim().ToUpper()) == null))//this file name doesn't exist in the media file
        //                    {
        //                        fileName = System.Text.RegularExpressions.Regex.Replace(entry.Name,entry.Extension,"");
        //                        formattedFileName = formatNameString(fileName);
        //                        foundNewFiles = true;

        //                        //get imdb info
        //                        IMDBScraper imdb = new IMDBScraper();
        //                        Media newMedia = imdb.getInfoByTitle(formattedFileName, true);
        //                        imdb.getPhoto(newMedia);
                                
        //                        XmlNode newNode = tempDoc.CreateNode(XmlNodeType.Element,"media","");

        //                        XmlNode previousTitleNode = tempDoc.CreateNode(XmlNodeType.Element, "previousTitle", "");
        //                        previousTitleNode.InnerText = formattedFileName;
        //                        XmlNode nameNode = tempDoc.CreateNode(XmlNodeType.Element,"title","");
        //                        if (imdb.MovieTitle.Length > 0)
        //                            nameNode.InnerText = imdb.MovieTitle;
        //                        else
        //                            nameNode.InnerText = formattedFileName;
        //                        XmlNode performersNode = tempDoc.CreateNode(XmlNodeType.Element,"performers","");
        //                        XmlNode ratingNode = tempDoc.CreateNode(XmlNodeType.Element,"rating","");
        //                        ratingNode.InnerText = imdb.Rating;
        //                        XmlNode ratingDescriptionNode = tempDoc.CreateNode(XmlNodeType.Element, "ratingDescription", "");
        //                        ratingDescriptionNode.InnerText = imdb.RatingDescription;
        //                        XmlNode descriptionNode = tempDoc.CreateNode(XmlNodeType.Element,"description","");
        //                        descriptionNode.InnerText = imdb.TagLine;
        //                        XmlNode starsNode = tempDoc.CreateNode(XmlNodeType.Element,"stars","");
        //                        XmlNode categoryNode = tempDoc.CreateNode(XmlNodeType.Element,"category","");
        //                        if (imdb.Genre.Length > 0)
        //                            ratingNode.InnerText = imdb.Genre;
        //                        else
        //                        {
        //                            category = formatNameString(getCategory(location));
        //                            categoryNode.InnerText = category;
        //                        }
        //                        XmlNode timesPlayedNode = tempDoc.CreateNode(XmlNodeType.Element,"timesPlayed","");
        //                        XmlNode filePathNode = tempDoc.CreateNode(XmlNodeType.Element,"filePath","");
        //                        filePathNode.InnerText = location +"\\"+ entry.Name;
        //                        XmlNode grammarNode = tempDoc.CreateNode(XmlNodeType.Element, "grammar", "");
        //                        if (imdb.MovieTitle.Length > 0)
        //                            grammarNode.InnerText = imdb.MovieTitle;
        //                        else
        //                            grammarNode.InnerText = formattedFileName;
        //                        XmlNode coverImageNode = tempDoc.CreateNode(XmlNodeType.Element, "coverImage", "");
        //                        coverImageNode.InnerText = formattedFileName;
        //                        XmlNode imdbNumNode = tempDoc.CreateNode(XmlNodeType.Element, "IMDBNum", "");
        //                        imdbNumNode.InnerText = imdb.IMDBNum;
        //                        XmlNode dateAddedNode = tempDoc.CreateNode(XmlNodeType.Element, "DateAdded", "");
        //                        dateAddedNode.InnerText = entry.CreationTime.ToShortDateString();
        //                        XmlNode releaseYearNode = tempDoc.CreateNode(XmlNodeType.Element, "ReleaseYear", "");
        //                        releaseYearNode.InnerText = "";
        //                        XmlNode sortByNode = tempDoc.CreateNode(XmlNodeType.Element, "SortBy", "");
        //                        sortByNode.InnerText = nameNode.InnerText;
        //                        XmlNode lastPlayedNode = tempDoc.CreateNode(XmlNodeType.Element, "LastPlayed", "");
        //                        lastPlayedNode.InnerText = "";
        //                        XmlNode mediaTypeNode = tempDoc.CreateNode(XmlNodeType.Element, "MediaType", "");
        //                        mediaTypeNode.InnerText = "";

                                
					
        //                        newNode.AppendChild(nameNode);
        //                        newNode.AppendChild(previousTitleNode);
        //                        newNode.AppendChild(performersNode);
        //                        newNode.AppendChild(ratingNode);
        //                        newNode.AppendChild(ratingDescriptionNode);
        //                        newNode.AppendChild(descriptionNode);
        //                        newNode.AppendChild(starsNode);
        //                        newNode.AppendChild(categoryNode);
        //                        newNode.AppendChild(timesPlayedNode);
        //                        newNode.AppendChild(filePathNode);
        //                        newNode.AppendChild(grammarNode);
        //                        newNode.AppendChild(coverImageNode);
        //                        newNode.AppendChild(imdbNumNode);
        //                        newNode.AppendChild(dateAddedNode);
        //                        newNode.AppendChild(releaseYearNode);
        //                        newNode.AppendChild(sortByNode);
        //                        newNode.AppendChild(lastPlayedNode);
        //                        newNode.AppendChild(mediaTypeNode);
        //                        tempDoc["mediaFiles"].AppendChild(newNode);
        //                    }

        //                    if (formattedFileName.Trim().Length > 0 || category.Trim().Length > 0)
        //                    {
        //                        //add to media speech file
        //                        //insertMediaXml(formattedFileName, ref xMediaSpeech);
        //                        //insertMediaXml(category, ref xMediaSpeech);
        //                    }
        //                }
        //            }
        //            else if(checkSubFolders) //this is a directory and they want to check subfolders
        //            {
        //                FindFiles(location +"\\"+ entry.Name,true,tempDoc);
        //            }
        //        }

        //        if(foundNewFiles) //only save the media file if there was new files added
        //        {
        //            if (ds.Tables.Count > 0)//there is media
        //            {
        //                DataColumn[] pk = new DataColumn[1];
        //                pk[0] = ds.Tables[0].Columns["filePath"];
        //                ds.Tables[0].PrimaryKey = pk;
        //            }

        //            //Tools.WriteToFile(Tools.errorFile,"++++++++++++ found files");
        //            tempDoc.Save(mediaFilePath);
        //            xMediaSpeech.Save("speech/vocab/xmlAll.xml");
        //            //myMedia = null; //remove and recreate myMedia with the new files
        //            //myMedia = new mediaHandler();

        //            lookForMedia(true);
        //        }
        //        //else
        //            //Tools.WriteToFile(Tools.errorFile,"-----------  didn't find any files");
        //    }
        //    catch(Exception e)
        //    {
        //        Tools.WriteToFile(Tools.errorFile, e.ToString());
        //    }
        //}
        #endregion

        /// <summary>
        /// Find the places in the string that should have spaces
        /// </summary>
        /// <param name="stringToSpace"></param>
        /// <returns></returns>
        public static string FormatNameString(string stringToSpace)
        {
            try
            {
                if (stringToSpace != null && stringToSpace.Length > 0)
                {
                    int offSet = 0;
                    int convertedChar = 0;
                    int previousChar = 0;
                    ArrayList foundSpaces = new ArrayList();
                    string charSpace = " ";
                    
                    //replace underscores with space
                    stringToSpace = stringToSpace.Replace("_", " ");

                    //remove extension - this can remove too much
                    if (stringToSpace.LastIndexOf(".") > 0 && stringToSpace.LastIndexOf(".") == stringToSpace.Length - 4)
                        stringToSpace = stringToSpace.Substring(0, stringToSpace.LastIndexOf("."));

                    //remove any special characters
                    stringToSpace = System.Text.RegularExpressions.Regex.Replace(stringToSpace, @"[^a-zA-Z0-9\s]", "");

                    //find change from lower case to upper case
                    for (int x = 0; x < stringToSpace.Length; x++)
                    {
                        convertedChar = Convert.ToInt32(stringToSpace[x]);

                        if (x > 1)
                            previousChar = Convert.ToInt32(stringToSpace[x - 1]);

                        if (x > 1 && (convertedChar >= 65 && convertedChar <= 90) && (convertedChar < 65 || stringToSpace[x - 1] != charSpace[0]) && (previousChar > 0 && (previousChar < 65 || previousChar > 90)))
                        {
                            foundSpaces.Add(x);
                        }
                    }

                    for (int x = 0; x < foundSpaces.Count; x++)
                    {
                        //don't add space if the next letter is capitalized
                        if ((x == foundSpaces.Count - 1) || ((int)foundSpaces[x] != (((int)foundSpaces[x + 1]) - 1)))
                        {
                            stringToSpace = stringToSpace.Insert(int.Parse(foundSpaces[x].ToString()) + offSet, " ");
                            offSet++;
                        }
                    }

                    //capitalize first letter
                    //stringToSpace = stringToSpace.Substring(0, 1).ToUpper() + stringToSpace.Substring(1);

                    //remove double spaces
                    string prevString = "";
                    while (stringToSpace != prevString)
                    {
                        prevString = stringToSpace;
                        stringToSpace = stringToSpace.Replace("  ", " ").Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            stringToSpace = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stringToSpace);// capitalize each word

            return stringToSpace;

            //if ((convertedChar >= 65) && (convertedChar <= 90))
            //{
            //    uLetters++;
            //}

            //if ((convertedPw >= 97) && (convertedPw <= 122))
            //{
            //    lLetters++;
            //}

            //if ((convertedPw >= 48) && (convertedPw <= 57))
            //{
            //    numbers++;
            //}

            //if ((convertedPw >= 33) && (convertedPw <= 47))
            //{
            //    symbols++;
            //}

            //if ((convertedPw >= 58) && (convertedPw <= 64))
            //{
            //    symbols++;
            //}

            //if ((convertedPw >= 91) && (convertedPw <= 96))
            //{
            //    symbols++;
            //}

            //if ((convertedPw >= 123) && (convertedPw <= 126))
            //{
            //    symbols++;
            //}
            //}
            // #### >END< PROCESSING EACH CHARACTER ####

            //lblDetails.Text += "Upper Case Letters: " + uLetters.ToString() + "\n";
            //lblDetails.Text += "Lower Case Letters: " + lLetters.ToString() + "\n";
            //lblDetails.Text += "Numbers: " + numbers.ToString() + "\n";
            //lblDetails.Text += "Symbols: " + symbols.ToString() + "\n";
        }

        private string findCategory(string location, string entryName)
        {
            string currentCategory = "";// System.Text.RegularExpressions.Regex.Replace(location, "Z:\\Video\\", "");
            
            if (currentCategory != entryName)//if the file is in a subfolder make the name of the subfolder the category name
            {
                currentCategory = System.Text.RegularExpressions.Regex.Replace(currentCategory, entryName, "");
                return currentCategory;
            }
            else
                return null;
        }

        private bool compareExtension(string extension)
        {
            //video
            foreach (XmlNode node in xmlFileTypes.GetElementsByTagName("video").Item(0))
            {
                if ("." + node.InnerText.ToLower() == extension.ToLower())//this extension is in the list of file types
                    return true;
            }

            //audio
            foreach (XmlNode node in xmlFileTypes.GetElementsByTagName("audio").Item(0))
            {
                if ("." + node.InnerText.ToLower() == extension.ToLower())//this extension is in the list of file types
                    return true;
            }

            //images
            foreach (XmlNode node in xmlFileTypes.GetElementsByTagName("images").Item(0))
            {
                if ("." + node.InnerText.ToLower() == extension.ToLower())//this extension is in the list of file types
                    return true;
            }

            //games
            foreach (XmlNode node in xmlFileTypes.GetElementsByTagName("games").Item(0))
            {
                if ("." + node.InnerText.ToLower() == extension.ToLower())//this extension is in the list of file types
                    return true;
            }

            //documents
            foreach (XmlNode node in xmlFileTypes.GetElementsByTagName("documents").Item(0))
            {
                if ("." + node.InnerText.ToLower() == extension.ToLower())//this extension is in the list of file types
                    return true;
            }

            return false;
        }

        /// <summary>
        /// returns the media type when given the extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private string getMediaType(string extension)
        {
            string foundMediaType = "";

            if (xmlFileTypes.ChildNodes != null)
            {
                foreach (XmlNode node in xmlFileTypes.ChildNodes)
                {
                    foundMediaType = getMediaType(extension, node);

                    if (foundMediaType.Length > 0)
                        break;
                }

                return foundMediaType;
            }
            else
                return "";
        }

        /// <summary>
        /// returns the media type when given the extension and startingNode
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="startingNode"></param>
        /// <returns></returns>
        private string getMediaType(string extension, XmlNode startingNode)
        {
            string foundMediaType = "";

            foreach (XmlNode node in startingNode.ChildNodes)
            {
                if (node.ChildNodes != null && node.ChildNodes.Count > 0)
                    foundMediaType = getMediaType(extension, node);

                if (foundMediaType.Length == 0)
                {
                    if ("." + node.InnerText.ToLower() == extension.ToLower())//this extension is in the list of media types
                    {
                        foundMediaType = node.ParentNode.ParentNode.Name;

                        break;
                    }
                }
                else
                    break;
            }

            switch (foundMediaType)
            {
                case "video":
                case "Movies":
                    foundMediaType = "Movies";
                    break;
                case "audio":
                case "Music":
                    foundMediaType = "Music";
                    break;
                case "images":
                case "Pictures":
                    foundMediaType = "Pictures";
                    break;
                default:
                    foundMediaType = "";
                    break;
            }

            return foundMediaType;
        }

        private Media getMusicInfoFromFileSystemInfo(FileSystemInfo file, string locationsFolderPath, Media newMedia)
        {
            string artist = "";
            string songTitle = "";
            string tempSongTitle = "";

            if (newMedia == null)
                newMedia = new Media();

            try
            {
                //gets the folder name for the artist
                artist = file.FullName.Replace(file.Name, "");
                
                //remove any location paths from artist
                foreach (string location in locations)
                {
                    artist = artist.Replace(location, "");
                }

                if (artist.Contains("\\"))
                {
                    artist = artist.Substring(0, artist.IndexOf("\\"));
                }

                artist = artist.Replace("\\", "").Trim();

                newMedia.Artist = FormatNameString(artist);
                
                songTitle = System.Text.RegularExpressions.Regex.Replace(file.Name, file.Extension, "");
                songTitle = FormatNameString(songTitle);

                if(artist.Length > 0)
                    tempSongTitle = songTitle.Replace(artist, "").Trim();

                if (tempSongTitle.Length > 0)
                    songTitle = tempSongTitle;

                newMedia.Title = songTitle;
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "getMusicInfoFromFileSystemInfo " + ex.ToString());
            }
            
            return newMedia;
        }

        private Media getTVSeriesInfoFromFileName(string fileName, Media newMedia)
        {
            string season = "";
            string episode = "";
            string splitString = "";
            string tempStringClean = "";
            string tempTitle = "";

            if (newMedia == null)
                newMedia = new Media();

            try
            {
                string tempSeasonAndEpisode = getSeasonAndEpisode(fileName).Trim();
                string tempEpisode = "";
                string tempSeason = tempSeasonAndEpisode;

                if(tempSeasonAndEpisode.Length > 0 && tempSeasonAndEpisode.Contains("|"))
                {
                    tempSeason = tempSeasonAndEpisode.Split('|')[0];
                    tempEpisode = tempSeasonAndEpisode.Split('|')[1];
                }

                newMedia.Season = tempSeason;
                newMedia.EpisodeNum = tempEpisode;

                //newMedia.Season = getSeason(fileName);
                //newMedia.EpisodeNum = getEpisodeFromString(fileName);

                if (newMedia.Season.Length > 0)
                    tempTitle = fileName.Substring(0, fileName.IndexOf(newMedia.Season) - 1).Trim();

                if (newMedia.EpisodeNum.Length > 0 && tempTitle.Contains(newMedia.EpisodeNum))
                {
                    if (fileName.IndexOf(newMedia.EpisodeNum) > tempTitle.Length / 2)
                        tempTitle = tempTitle.Substring(0, tempTitle.IndexOf(newMedia.EpisodeNum) - 1).Trim();
                    else
                        tempTitle = tempTitle.Substring(tempTitle.IndexOf(newMedia.EpisodeNum)).Trim();
                }

                if(newMedia.Season.Trim().Length > 0 && newMedia.EpisodeNum.Trim().Length > 0)
                    newMedia.MediaType = "TV";
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "getTVSeriesInfoFromFileName " + ex.ToString());
            }
            
            newMedia.Title = FormatNameString(tempTitle);

            return newMedia;
        }

        private string getSeasonAndEpisode(string stringToSearch)
        {
            string returnString = "";
            string splitString = "";

            try
            {
                stringToSearch = stringToSearch.ToLower();

                if (stringToSearch.Contains("_s"))
                    splitString = "_s";
                else if (stringToSearch.Contains(" season"))
                    splitString = " season";
                else if (stringToSearch.Contains(" s"))
                    splitString = " s";

                int splitCounter = 0;

                //get season
                foreach (string tempString in stringToSearch.Split(new string[] { splitString }, StringSplitOptions.RemoveEmptyEntries))
                {
                    splitCounter++;

                    if (splitCounter > 1)
                    {
                        string tempStringClean = tempString.Replace("eason", "");

                        if (Char.IsDigit(tempStringClean.ToCharArray()[0]))//the first character is a digit
                        {
                            returnString = tempStringClean;

                            foreach (char character in tempStringClean.ToCharArray())
                            {
                                if (!Char.IsDigit(character))
                                {
                                    returnString = tempStringClean.Substring(0, tempStringClean.IndexOf(character.ToString()));
                                    string tempEpisode = getEpisodeFromString(tempStringClean.Substring(tempStringClean.IndexOf(character.ToString())));

                                    if (tempEpisode.Trim().Length > 0)
                                        returnString += "|" + tempEpisode;

                                    break;
                                }
                            }
                        }

                        if (returnString.Length > 0)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "getSeason " + ex.ToString());
            }

            return returnString;
        }

        /// <summary>
        /// Get episode information from imdb
        /// </summary>
        /// <param name="media"></param>
        private Media getEpisodeInfo(Media media)
        {
            //get imdb info
            IMDBScraper imdb = new IMDBScraper();
            media = imdb.GetEpisodeInfo(media);

            return media;
        }

        private string getEpisodeFromString(string stringToSearch)
        {
            string returnString = "";
            string splitString = "";

            try
            {
                stringToSearch = stringToSearch.ToLower();

                if (stringToSearch.Contains("_e"))
                    splitString = "_e";
                else if (stringToSearch.Contains(" episode"))
                    splitString = " episode";
                else if (stringToSearch.Contains("#"))
                    splitString = "#";
                else if (stringToSearch.Contains(" e"))
                    splitString = " e";
                else if (stringToSearch.Contains("e"))
                    splitString = "e";

                int splitCounter = 0;

                //get episode
                foreach (string tempString in stringToSearch.Split(new string[] { splitString }, StringSplitOptions.RemoveEmptyEntries))
                {
                    splitCounter++;

                    //if (splitCounter > 1)//skip first split
                    //{
                        string tempStringClean = tempString.Replace("pisode", "");

                        if (Char.IsDigit(tempStringClean.ToCharArray()[0]))//the first character is a digit
                        {
                            returnString = tempStringClean;

                            foreach (char character in tempStringClean.ToCharArray())
                            {
                                if (!Char.IsDigit(character))
                                {
                                    returnString = tempStringClean.Substring(0, tempStringClean.IndexOf(character.ToString()));
                                    break;
                                }
                            }
                        }

                        if (returnString.Length > 0)
                        {
                            break;
                        }
                    //}
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "getEpisodeFromString " + ex.ToString());
            }

            return returnString;
        }

        public void AddLocation(string location)
        {
            ArrayList newLocation = new ArrayList();
            newLocation.Add(location);

            AddLocations(newLocation, true);
        }

        public void AddLocations(ArrayList newLocations, bool removeMissingLocations)
        {
            if (xmlLocations == null)
            {
                xmlLocations.Load(LocationXMLFilePath);
                findLocations();
            }

            string locationCorrectCase = "";
            string fileType = "";
            string newLocation = "";
            updatedLocations = false;

            XmlNodeList locationNodes = xmlLocations.GetElementsByTagName("locations");

            try
            {
                foreach (string location in newLocations)
                {
                    newLocation = location;
                    fileType = "";

                    if (newLocation.Contains(","))
                    {
                        fileType = newLocation.Split(',')[1];
                        newLocation = newLocation.Split(',')[0];
                    }

                    locationCorrectCase = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(newLocation);//correct the casing

                    if (!locations.Contains(location) || (xmlLocations.SelectSingleNode("/locations/location[text()='" + locationCorrectCase + "']") == null && xmlLocations.SelectSingleNode("/locations/location[text()='" + locationCorrectCase + "," + fileType +"']") == null))//(newLocation.ToLower() == DefaultPathToSaveTo.ToLower() && locations.Count == 1))//keep out duplicates and the DefaultPathToSaveTo
                    {
                        updatedLocations = true;

                        XmlNode newNode = xmlLocations.CreateNode(XmlNodeType.Element, "location", "");
                        newNode.InnerText = locationCorrectCase;

                        if (fileType.Trim().Length > 0)
                        {
                            newNode.InnerText += "," + fileType;
                            //XmlAttribute tempAttribute = xmlLocations.CreateAttribute("MediaType");
                            //tempAttribute.Value = fileType;
                            //newNode.Attributes.Append(tempAttribute);
                        }
                        //					XmlNode subNode = xmlLocations.CreateNode(XmlNodeType.Element,"subFolders","");
                        //					subNode.InnerText=subFoldersCheckBox.Checked.ToString();
                        //					newNode.AppendChild(subNode);
                        //					newNode.Value = ;
                        locationNodes[0].AppendChild(newNode);
                        //locations.Add(location);
                    }
                }

                if (removeMissingLocations)//need to remove any locations that aren't in newLocations
                {
                    ArrayList currentLocations = new ArrayList();// (ArrayList)newLocations.Clone();
                    ArrayList locationsToRemove = new ArrayList();

                    foreach (string location in newLocations)
                        currentLocations.Add(location.ToLower());

                    findLocations();

                    foreach (string location in locations)
                    {
                        //locationCorrectCase = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(newLocation);//correct the casing

                        if (!currentLocations.Contains(location.ToLower()))//find the location not in newLocations
                        {
                            updatedLocations = true;

                            locationsToRemove.Add(location);

                            XmlNode xmlLocationNodeToRemove = xmlLocations.SelectSingleNode("/locations/location[text()='" + location + "']");

                            if (xmlLocationNodeToRemove != null)
                                xmlLocationNodeToRemove.ParentNode.RemoveChild(xmlLocationNodeToRemove);
                        }
                    }

                    foreach (string location in locationsToRemove)
                    {
                        locations.Remove(location);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            if (updatedLocations) //there were new locations added or removed
                xmlLocations.Save(LocationXMLFilePath);
        }

        /// <summary>
        /// update the info of the given Media
        /// </summary>
        /// <param name="updatedMedia"></param>
        public void UpdateMediaInfo(Media updatedMedia)
        {
            try
            {
                //if (dsMedia.Tables.Count < 1)//check for the existance of tables in dsMedia
                    //GetMedia();
                //else
                //{
                //    DataTable dtEmptyTable = new DataTable();
                //    dsMedia.Tables.Add(dtEmptyTable);
                //}

                //update all related files for justindownloads
                if (updatedMedia.filePath.ToLower().Contains("justindownloads"))
                {
                    Media relatedMedia;
                    //get title from filepath
                    string currentTitle = updatedMedia.filePath;
                    currentTitle = currentTitle.Substring(0, currentTitle.IndexOf("_"));
                    currentTitle = currentTitle.Substring(currentTitle.LastIndexOf("\\") + 1);

                    //get related justin files
                    DataTable sortableDt = MediaHandler.dsMedia.Tables[0].Copy();

                    sortableDt.DefaultView.RowFilter = "filepath LIKE '*justindownloads*' and filepath LIKE '*" + currentTitle.Replace("'", "''") + "*'";

                    sortableDt.DefaultView.Sort = "SortBy, Title";

                    foreach (DataRowView dr in sortableDt.DefaultView)
                    {
                        relatedMedia = MediaHandler.CreateMedia(dr);

                        relatedMedia.category = updatedMedia.category;
                        relatedMedia.coverImage = updatedMedia.coverImage;
                        relatedMedia.Description = updatedMedia.Description;
                        relatedMedia.Director = updatedMedia.Director;
                        relatedMedia.grammar = updatedMedia.grammar;
                        relatedMedia.IMDBNum = updatedMedia.IMDBNum;
                        relatedMedia.MediaType = updatedMedia.MediaType;
                        relatedMedia.Message = updatedMedia.Message;
                        relatedMedia.Performers = updatedMedia.Performers;
                        relatedMedia.Rating = updatedMedia.Rating;
                        relatedMedia.RatingDescription = updatedMedia.RatingDescription;
                        relatedMedia.ReleaseYear = updatedMedia.ReleaseYear;
                        relatedMedia.SortBy = updatedMedia.SortBy;
                        relatedMedia.Stars = updatedMedia.Stars;
                        relatedMedia.TagLine = updatedMedia.TagLine;
                        relatedMedia.ShortDescription = updatedMedia.ShortDescription;
                        relatedMedia.Goofs = updatedMedia.Goofs;
                        relatedMedia.Trivia = updatedMedia.Trivia;
                        relatedMedia.Season = updatedMedia.Season;
                        relatedMedia.EpisodeNum = updatedMedia.EpisodeNum;
                        relatedMedia.SeriesName = updatedMedia.SeriesName;
                        relatedMedia.SeriesIMDBNum = updatedMedia.SeriesIMDBNum;
                        relatedMedia.SeriesImage = updatedMedia.SeriesImage;
                        relatedMedia.SeriesDescription = updatedMedia.SeriesDescription;

                        //update media file
                        //find the given media
                        //dsMedia.Tables[0].DefaultView.RowFilter = "filePath LIKE '*" + relatedMedia.filePath.Replace("'", "''") + "*'";
                        dsMedia.Tables[0].DefaultView.RowFilter = "ID = '" + relatedMedia.ID + "'";
                        DataView dv = dsMedia.Tables[0].DefaultView;

                        //update media item
                        if (relatedMedia.category != null)
                            dv[0]["category"] = relatedMedia.category;
                        //else
                        //    dv[0]["category"] = "";

                        if (relatedMedia.coverImage != null)
                            dv[0]["coverImage"] = relatedMedia.coverImage;
                        //else
                        //    dv[0]["coverImage"] = "";

                        if (relatedMedia.Description != null)
                            dv[0]["description"] = relatedMedia.Description;
                        //else
                        //    dv[0]["description"] = "";

                        if (relatedMedia.filePath != null)
                            dv[0]["filePath"] = relatedMedia.filePath;
                        //else
                        //    dv[0]["filePath"] = "";

                        if (relatedMedia.grammar != null)
                            dv[0]["grammar"] = relatedMedia.grammar;
                        //else
                        //    dv[0]["grammar"] = "";

                        if (relatedMedia.IMDBNum != null)
                            dv[0]["IMDBNum"] = relatedMedia.IMDBNum;
                        //else
                        //    dv[0]["IMDBNum"] = "";

                        if (relatedMedia.Performers != null)
                            dv[0]["performers"] = relatedMedia.Performers;
                        //else
                        //    dv[0]["performers"] = "";

                        if (relatedMedia.PreviousTitle != null)
                            dv[0]["previousTitle"] = relatedMedia.PreviousTitle;
                        //else
                        //    dv[0]["previousTitle"] = "";

                        if (relatedMedia.Rating != null)
                            dv[0]["Rating"] = relatedMedia.Rating;
                        //else
                        //    dv[0]["Rating"] = "";

                        if (relatedMedia.RatingDescription != null)
                            dv[0]["RatingDescription"] = relatedMedia.RatingDescription;
                        //else
                        //    dv[0]["RatingDescription"] = "";

                        if (relatedMedia.Stars != null)
                            dv[0]["Stars"] = relatedMedia.Stars;
                        //else
                        //    dv[0]["Stars"] = "";

                        if (relatedMedia.TimesPlayed != null)
                            dv[0]["TimesPlayed"] = relatedMedia.TimesPlayed;
                        //else
                        //    dv[0]["TimesPlayed"] = "0";

                        if (relatedMedia.Title != null)
                        {
                            dv[0]["Title"] = relatedMedia.Title;
                            dv[0]["SortBy"] = formatSortBy(relatedMedia.Title);
                        }
                        //else
                        //    dv[0]["Title"] = "";

                        if (relatedMedia.Director != null)
                            dv[0]["Director"] = relatedMedia.Director;
                        //else
                        //    dv[0]["Director"] = "";

                        if (relatedMedia.ReleaseYear != null)
                            dv[0]["ReleaseYear"] = relatedMedia.ReleaseYear;
                        //else
                        //    dv[0]["ReleaseYear"] = "";

                        if (relatedMedia.DateAdded != null)
                            dv[0]["DateAdded"] = relatedMedia.DateAdded;
                        //else
                        //    dv[0]["DateAdded"] = "";

                        if (relatedMedia.MediaType != null)
                            dv[0]["MediaType"] = relatedMedia.MediaType;

                        //if (relatedMedia.LastPlayed != null && relatedMedia.LastPlayed.Replace("|", "").Trim().Length > 0)
                        //    dv[0]["LastPlayed"] = relatedMedia.LastPlayed +"|" + dv[0]["LastPlayed"];

                        if (relatedMedia.LastPlayPosition != null)
                            dv[0]["LastPlayPosition"] = relatedMedia.LastPlayPosition;

                        //save xml
                        dsMedia.WriteXml(MediaXMLFilePath);
                    }
                }
                else
                {
                    //string updatedFilePath = updatedMedia.filePath;
                    //if (updatedFilePath.Contains("|"))
                    //    updatedFilePath = updatedFilePath.Substring(0, updatedFilePath.IndexOf("|"));

                    //find the given media
                    //dsMedia.Tables[0].DefaultView.RowFilter = "filePath LIKE '*" + updatedFilePath.Replace("'","''") + "*'";
                    dsMedia.Tables[0].DefaultView.RowFilter = "ID = '" + updatedMedia.ID + "'";
                    DataView dv = dsMedia.Tables[0].DefaultView;

                    if (updatedMedia.ID != null)
                        dv[0]["ID"] = updatedMedia.ID;

                    //update media item
                    if (updatedMedia.category != null)
                        dv[0]["category"] = updatedMedia.category;
                    //else
                    //    dv[0]["category"] = "";

                    if (updatedMedia.coverImage != null)
                        dv[0]["coverImage"] = updatedMedia.coverImage;
                    //else
                    //    dv[0]["coverImage"] = "";

                    if (updatedMedia.Description != null)
                        dv[0]["description"] = updatedMedia.Description;
                    //else
                    //    dv[0]["description"] = "";

                    if (updatedMedia.filePath != null)
                        dv[0]["filePath"] = updatedMedia.filePath;
                    //else
                    //    dv[0]["filePath"] = "";

                    if (updatedMedia.grammar != null)
                        dv[0]["grammar"] = updatedMedia.grammar;
                    //else
                    //    dv[0]["grammar"] = "";

                    if (updatedMedia.IMDBNum != null)
                        dv[0]["IMDBNum"] = updatedMedia.IMDBNum;
                    //else
                    //    dv[0]["IMDBNum"] = "";

                    if (updatedMedia.Performers != null)
                        dv[0]["performers"] = updatedMedia.Performers;
                    //else
                    //    dv[0]["performers"] = "";

                    if (updatedMedia.PreviousTitle != null)
                        dv[0]["previousTitle"] = updatedMedia.PreviousTitle;
                    //else
                    //    dv[0]["previousTitle"] = "";

                    if (updatedMedia.Rating != null)
                        dv[0]["Rating"] = updatedMedia.Rating;
                    //else
                    //    dv[0]["Rating"] = "";

                    if (updatedMedia.RatingDescription != null)
                        dv[0]["RatingDescription"] = updatedMedia.RatingDescription;
                    //else
                    //    dv[0]["RatingDescription"] = "";

                    if (updatedMedia.Stars != null)
                        dv[0]["Stars"] = updatedMedia.Stars;
                    //else
                    //    dv[0]["Stars"] = "";

                    if (updatedMedia.TimesPlayed != null)
                        dv[0]["TimesPlayed"] = updatedMedia.TimesPlayed;
                    //else
                    //    dv[0]["TimesPlayed"] = "0";

                    if (updatedMedia.Title != null)
                    {
                        dv[0]["Title"] = updatedMedia.Title;
                        dv[0]["SortBy"] = formatSortBy(updatedMedia.Title);
                    }
                    //else
                    //    dv[0]["Title"] = "";

                    if (updatedMedia.Director != null)
                        dv[0]["Director"] = updatedMedia.Director;
                    //else
                    //    dv[0]["Director"] = "";

                    if (updatedMedia.ReleaseYear != null)
                        dv[0]["ReleaseYear"] = updatedMedia.ReleaseYear;
                    //else
                    //    dv[0]["ReleaseYear"] = "";

                    if (updatedMedia.DateAdded != null)
                        dv[0]["DateAdded"] = updatedMedia.DateAdded;
                    //else
                    //    dv[0]["DateAdded"] = "";

                    if (updatedMedia.MediaType != null)
                        dv[0]["MediaType"] = updatedMedia.MediaType;

                    //if (updatedMedia.LastPlayed != null && updatedMedia.LastPlayed.Replace("|", "").Trim().Length > 0)
                    //    dv[0]["LastPlayed"] = updatedMedia.LastPlayed + "|" + dv[0]["LastPlayed"];

                    if (updatedMedia.LastPlayPosition != null)
                        dv[0]["LastPlayPosition"] = updatedMedia.LastPlayPosition;

                    if(updatedMedia.Goofs != null)
                        dv[0]["Goofs"] = updatedMedia.Goofs;

                    if (updatedMedia.Trivia != null)
                        dv[0]["Trivia"] = updatedMedia.Trivia;

                    if (updatedMedia.ShortDescription != null)
                        dv[0]["ShortDescription"] = updatedMedia.ShortDescription;

                    if (updatedMedia.Season != null)
                        dv[0]["Season"] = updatedMedia.Season;

                    if (updatedMedia.EpisodeNum != null)
                        dv[0]["EpisodeNum"] = updatedMedia.EpisodeNum;

                    if (updatedMedia.SeriesName != null)
                        dv[0]["SeriesName"] = updatedMedia.SeriesName;

                    if (updatedMedia.SeriesIMDBNum != null)
                        dv[0]["SeriesIMDBNum"] = updatedMedia.SeriesIMDBNum;

                    if (updatedMedia.SeriesImage != null)
                        dv[0]["SeriesImage"] = updatedMedia.SeriesImage;

                    if (updatedMedia.SeriesDescription != null)
                        dv[0]["SeriesDescription"] = updatedMedia.SeriesDescription;

                    //save xml
                    dsMedia.WriteXml(MediaXMLFilePath);

                    GetMedia();
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "UpdateMediaInfo "+ ex.ToString());
            }
        }

        public void DeleteMedia(Media mediaToDelete)
        {
            try
            {
                string deleteFilePath = mediaToDelete.filePath;

                if (deleteFilePath.Contains("|"))
                    deleteFilePath = deleteFilePath.Substring(0, deleteFilePath.IndexOf("|"));

                //find the given media
                //dsMedia.Tables[0].DefaultView.RowFilter = "ID = '" + mediaToDelete.ID + "'";
                dsMedia.Tables[0].DefaultView.RowFilter = "filePath = '" + mediaToDelete.filePath + "'";
                DataView dv = dsMedia.Tables[0].DefaultView;
                
                dsMedia.Tables[0].Rows.Remove(dsMedia.Tables[0].DefaultView[0].Row);

                foreach (DataRow dr in dsMedia.Tables[0].Rows)
                    dr["ID"] = dr.Table.Rows.IndexOf(dr);
                
                //save xml
                dsMedia.WriteXml(MediaXMLFilePath);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "DeleteMedia " + ex.ToString() + Environment.NewLine + ex.StackTrace);
            }            
        }

        public Media MergeMedia(Media media1, Media media2)
        {
            Media mergedMedia = media1;

            if (!media2.coverImage.ToLower().Contains("notavailable.jpg") && File.Exists(media2.filePath) && (media2.coverImage.Trim().Length > media1.coverImage.Trim().Length))
            {
                
                mergedMedia = media2;
                DeleteMedia(media1);

                //find the given media
                dsMedia.Tables[0].DefaultView.RowFilter = "filepath = '" + media2.filePath + "'";
                mergedMedia.filePath = media2.filePath + "|" + media1.filePath;
                
            }
            else
            {
                DeleteMedia(media2);

                //find the given media
                dsMedia.Tables[0].DefaultView.RowFilter = "filepath = '" + media1.filePath + "'";
                mergedMedia.filePath = media1.filePath + "|" + media2.filePath;
            }

            foreach(DataRowView dr in dsMedia.Tables[0].DefaultView)
                dr["filePath"] = mergedMedia.filePath;

            //save xml
            dsMedia.WriteXml(MediaXMLFilePath);

            return mergedMedia;
        }

        /// <summary>
        /// clean the name that is used to sort by
        /// </summary>
        /// <param name="nameToClean"></param>
        /// <returns></returns>
        private static string formatSortBy(string nameToClean)
        {
            string cleanSortName = nameToClean;

            cleanSortName = cleanSortName.Replace("\"", "");

            if (cleanSortName.ToLower().StartsWith("the "))
                cleanSortName = cleanSortName.Substring(4);

            return cleanSortName;
        }

        #region Playlist
        /// <summary>
        /// Reads playlist from file
        /// </summary>
        /// <returns></returns>
        public DataSet GetPlaylist()
        {
            return GetPlaylist("", "","","");
        }

        /// <summary>
        /// Gets the playlist items for the given playlist
        /// </summary>
        /// <param name="playlistName"></param>
        /// <returns></returns>
        public DataSet GetPlaylist(string playlistName)
        {
            return GetPlaylist(playlistName, "", "", "");
        }

        public DataSet GetPlaylist(string playlistName, string category, string startsWith, string mediaType)
        {
            try
            {
                dsPlaylist = new DataSet();

                try
                {
                    if (File.Exists(PlaylistXMLFilePath))
                        dsPlaylist.ReadXml(new StringReader(xmlQuery(PlaylistXMLFilePath, "getPlaylist.xqu").InnerXml.ToString()));

                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(Tools.errorFile, "dsPlaylist.ReadXml " + ex.Message);
                }

                if (dsPlaylist.Tables.Count < 1)
                {
                    if (File.Exists(PlaylistXMLDefaultPath))
                        dsPlaylist.ReadXml(new StringReader(xmlQuery(PlaylistXMLDefaultPath, "getPlaylist.xqu").InnerXml.ToString()));
                    else
                        MessageBox.Show("Missing Default playlist files.  Reinstall SCTV");
                }
                else if (dsPlaylist.Tables[0] != null)
                {
                    try
                    {
                        //dsPlaylist = validateMedia(dsPlaylist);

                        //dsMediaTypes.Tables[0].DefaultView.RowFilter = "category LIKE '*" + category + "*' and mediaCategory LIKE '*" + mediaType + "*'";

                        if (dsPlaylist.Tables[0].Columns.Contains("playlistName"))
                            dsPlaylist.Tables[0].DefaultView.Sort = "playlistName, mediaSource";//alphabetize                        
                    }
                    catch (Exception ex)
                    {
                        Tools.WriteToFile(Tools.errorFile, "sort " + ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Tools.WriteToFile(Tools.errorFile, e.ToString());
            }
            return dsPlaylist;
        }

        /// <summary>
        /// Add media to current playlist
        /// </summary>
        /// <param name="media"></param>
        public DataSet AddToPlaylist(Media media)
        {
            try
            {
                XmlDocument tempDoc = new XmlDocument();

                if (File.Exists(PlaylistXMLFilePath))
                {
                    tempDoc.Load(PlaylistXMLFilePath);
                    //ds = GetMedia();
                }
                else
                {
                    //add base node to tempDoc
                    XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "playlistItems", "");

                    tempDoc.AppendChild(baseNode);
                }

                XmlNode playlistItemNode = tempDoc.CreateNode(XmlNodeType.Element, "playlistItem", "");
                XmlNode playlistNameNode = tempDoc.CreateNode(XmlNodeType.Element, "playlistName", "");
                playlistNameNode.InnerText = "test";
                XmlNode mediaSourceNode = tempDoc.CreateNode(XmlNodeType.Element, "mediaSource", "");
                mediaSourceNode.InnerText = media.filePath;

                playlistItemNode.AppendChild(playlistNameNode);
                playlistItemNode.AppendChild(mediaSourceNode);

                tempDoc["playlistItems"].AppendChild(playlistItemNode);

                tempDoc.Save(PlaylistXMLFilePath);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return GetPlaylist();

        }
        #endregion

        #region Images
        /// <summary>
        /// finds new files in the folders specified in config/locations.xml and ads their information to config/media.xml
        /// </summary>
        public void lookForImages(bool bestMatch)
        {
            try
            {
                XmlDocument tempDoc = new XmlDocument();
                DataSet ds = new DataSet();

                if (File.Exists(ImagesXMLPath))
                {
                    tempDoc.Load(ImagesXMLPath);
                    ds = GetImages();
                }
                else
                {
                    //add base node to tempDoc
                    XmlNode baseNode = tempDoc.CreateNode(XmlNodeType.Element, "images", "");

                    tempDoc.AppendChild(baseNode);
                }

                if (locations.Count == 0)
                    System.Windows.Forms.MessageBox.Show("There are no valid media sources to read from");
                else
                {
                    foreach (string location in locations)
                    {
                        if (Directory.Exists(location))
                        {
                            tempDoc = lookForImages(bestMatch, location, tempDoc, ds);
                        }
                        else
                            System.Windows.Forms.MessageBox.Show("Invalid media source " + location);

                        tempDoc.Save(ImagesXMLPath);
                    }
                }

                dsImages = GetImages();
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(Tools.errorFile, "lookForImages: "+ ex.ToString());
            }
        }

        private XmlDocument lookForImages(bool bestMatch, string location, XmlDocument tempDoc, DataSet ds)
        {
            string dLocation = location;
            string category = "";

            try
            {
                DirectoryInfo fl = new DirectoryInfo(dLocation);

                foreach (FileSystemInfo entry in fl.GetFileSystemInfos())
                {
                    if (entry.Extension.Length > 0)//this is a file
                    {
                        //check extension and make sure it's a type we recognize
                        if (compareExtension(entry.Extension))
                        {
                            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Find(dLocation + "\\" + entry.Name.ToString().Trim()) == null)//this file name doesn't exist in the media file
                            {
                                string foundMediaType = getMediaType(entry.Extension);

                                switch (foundMediaType.ToLower())
                                {
                                    case "pictures":
                                        tempDoc = addImageFile(tempDoc, entry, bestMatch, foundMediaType);
                                        break;
                                    default:
                                        //MessageBox.Show("Didn't find correct mediaType.  Looking for images");
                                        break;
                                }
                            }
                        }
                    }
                }

                //look through children folders
                foreach (DirectoryInfo dir in fl.GetDirectories())
                    tempDoc = lookForImages(bestMatch, dir.FullName, tempDoc, ds);
            }
            catch (Exception e)
            {
                Tools.WriteToFile(Tools.errorFile, "lookForImages2 " + e.Message);
            }

            return tempDoc;
        }

        /// <summary>
        /// fills dsMedia with media records from mediaFilePath
        /// </summary>
        /// <returns></returns>
        public DataSet GetImages()
        {
            try
            {
                dsImages = new DataSet();

                try
                {
                    if (File.Exists(ImagesXMLPath))
                        dsImages.ReadXml(new StringReader(xmlQuery(ImagesXMLPath, "getImages.xqu").InnerXml.ToString()));

                }
                catch (Exception ex)
                {
                    Tools.WriteToFile(Tools.errorFile, "dsImages.ReadXml " + ex.Message);
                }

                if (dsImages.Tables.Count < 1)
                {
                    if (File.Exists(ImagesXMLPath))
                        dsImages.ReadXml(new StringReader(xmlQuery(ImagesXMLDefaultPath, "getImages.xqu").InnerXml.ToString()));

                    //DataTable dtEmptyTable = new DataTable();
                    //dsMedia.Tables.Add(dtEmptyTable);
                }
                else if (dsImages.Tables[0] != null)
                {
                    try
                    {
                        //dsImages = validateMedia(dsImages);

                        if (dsImages.Tables[0].Columns.Contains("SortBy"))
                            dsImages.Tables[0].DefaultView.Sort = "SortBy";//alphabetize   

                        if (dsImages.Tables[0].Columns.Contains("filePath"))//there are images
                        {
                            DataColumn[] pk = new DataColumn[1];
                            pk[0] = dsImages.Tables[0].Columns["filePath"];
                            dsImages.Tables[0].PrimaryKey = pk;
                        }
                    }
                    catch (Exception ex)
                    {
                        Tools.WriteToFile(Tools.errorFile, "sort " + ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Tools.WriteToFile(Tools.errorFile, e.ToString());
            }
            return dsImages;
        }

        #endregion

        public static Media CreateMedia(DataRowView mediaRow)
        {
            Media retMedia = new Media();

            retMedia.ID = mediaRow["ID"].ToString();
            retMedia.category = mediaRow["category"].ToString();
            retMedia.coverImage = mediaRow["coverImage"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("dateAdded"))
                retMedia.DateAdded = mediaRow["dateAdded"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("description"))
                retMedia.Description = mediaRow["description"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("director"))
                retMedia.Director = mediaRow["director"].ToString();

            //retMedia.filename = mediaRow["fileName"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("filePath"))
                retMedia.filePath = mediaRow["filePath"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("filename"))
                retMedia.filePath = mediaRow["filename"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("grammar"))
                retMedia.grammar = mediaRow["grammar"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("IMDBNum"))
                retMedia.IMDBNum = mediaRow["IMDBNum"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("lastPlayed"))
                retMedia.LastPlayed = mediaRow["lastPlayed"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("MediaType"))
                retMedia.MediaType = mediaRow["MediaType"].ToString();

            //retMedia.Message = mediaRow["message"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("performers"))
                retMedia.Performers = mediaRow["performers"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("previousTitle"))
                retMedia.PreviousTitle = mediaRow["previousTitle"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("rating"))
                retMedia.Rating = mediaRow["rating"].ToString();
            else
                retMedia.Rating = "";

            if (mediaRow.DataView.Table.Columns.Contains("RatingDescription"))
                retMedia.RatingDescription = mediaRow["RatingDescription"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("ReleaseYear"))
                retMedia.ReleaseYear = mediaRow["ReleaseYear"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("sortBy"))
                retMedia.SortBy = mediaRow["sortBy"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("stars"))
                retMedia.Stars = mediaRow["stars"].ToString();

            if(mediaRow.DataView.Table.Columns.Contains("tagline"))
                retMedia.TagLine = mediaRow["tagline"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("timesPlayed"))
                retMedia.TimesPlayed = mediaRow["timesPlayed"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("title"))
                retMedia.Title = mediaRow["title"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("LastPlayPosition"))
                retMedia.LastPlayPosition = mediaRow["LastPlayPosition"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("Goofs"))
                retMedia.Goofs = mediaRow["Goofs"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("Trivia"))
                retMedia.Trivia = mediaRow["Trivia"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("ShortDescription"))
                retMedia.ShortDescription = mediaRow["ShortDescription"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("Season"))
                retMedia.Season = mediaRow["Season"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("EpisodeNum"))
                retMedia.EpisodeNum = mediaRow["EpisodeNum"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("SeriesName"))
                retMedia.SeriesName = mediaRow["SeriesName"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("SeriesIMDBNum"))
                retMedia.SeriesIMDBNum = mediaRow["SeriesIMDBNum"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("SeriesImage"))
                retMedia.SeriesImage = mediaRow["SeriesImage"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("SeriesDescription"))
                retMedia.SeriesDescription = mediaRow["SeriesDescription"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("Artist"))
                retMedia.Artist = mediaRow["Artist"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("Album"))
                retMedia.Album = mediaRow["Album"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("Lyrics"))
                retMedia.Lyrics = mediaRow["Lyrics"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("Length"))
                retMedia.Length = mediaRow["Length"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("RecordingID"))
                retMedia.RecordingID = mediaRow["RecordingID"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("ArtistID"))
                retMedia.ArtistID = mediaRow["ArtistID"].ToString();

            if (mediaRow.DataView.Table.Columns.Contains("ReleaseID"))
                retMedia.ReleaseID = mediaRow["ReleaseID"].ToString();

            return retMedia;
        }

        /// <summary>
        /// Add entry to onlineMedia file
        /// </summary>
        public void AddOnlineMedia(string name, string url, string mediaCategory, string category, string description, string coverImage)
        {
            //make sure dataset has entries
            if (dsMediaTypes == null)
                GetMediaTypes();

            //add new entry
            dsMediaTypes.Tables[0].Rows.Add(new object[] { name, mediaCategory, category, description, url, coverImage });

            //<name>Hulu</name>
            //<mediaCategory>Online</mediaCategory>
            //<category>TV</category>
            //<description>Online Movies and TV</description>
            //<url>www.hulu.com/browse/alphabetical/episode</url>
            //<coverImage>/images/OnlineMedia/Hulu.jpg</coverImage>

            //save
            dsMediaTypes.WriteXml(MediaTypesPath);
        }
    }

    /// <summary>
    /// The helper class to sort in ascending order by FileTime(LastVisited).
    /// </summary>
    //public class SortFileTimeAscendingHelper : IComparer
    //{
    //    [DllImport("Kernel32.dll")]
    //    static extern int CompareFileTime([In] ref DateTime lpFileTime1, [In] ref DateTime lpFileTime2);

    //    int IComparer.Compare(object a, object b)
    //    {
    //        DataRowView c1 = (DataRowView)a;
    //        DataRowView c2 = (DataRowView)b;

    //        return (CompareFileTime(ref DateTime.Parse(c1["DateAdded"].ToString()), ref DateTime.Parse(c2["DateAdded"].ToString())));
    //    }

    //    public static IComparer SortFileTimeAscending()
    //    {
    //        return (IComparer)new SortFileTimeAscendingHelper();
    //    }
    //} 
}
