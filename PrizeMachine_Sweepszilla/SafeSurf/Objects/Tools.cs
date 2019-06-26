using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SCTVObjects
{
    public class Tools
    {
        public static string errorFile = System.Windows.Forms.Application.StartupPath + "\\logs\\errors.txt";

        public static string logFile = System.Windows.Forms.Application.StartupPath + "\\logs\\log.txt";

        /// <summary>
        /// Write info to a file
        /// </summary>
        /// <param name="fileToWrite"></param>
        /// <param name="value"></param>
        public static void WriteToFile(string fileToWrite, string value)
        {
            try
            {
                //create the directory for the file
                if (!Directory.Exists(fileToWrite.Substring(0,fileToWrite.LastIndexOf("\\"))))
                    Directory.CreateDirectory(fileToWrite.Substring(0, fileToWrite.LastIndexOf("\\")));

                //append data to file
                using (StreamWriter file = new StreamWriter(fileToWrite, true))
                    file.WriteLine(DateTime.Now.ToShortDateString() +" "+ DateTime.Now.ToShortTimeString() +" : "+ value);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("sctv", "WriteToFile error: " + ex.Message);
            }
        }

        /// <summary>
        /// Write info to a file
        /// </summary>
        /// <param name="fileToWrite"></param>
        /// <param name="value"></param>
        public static void WriteToFile(string fileToWrite, Exception exception)
        {
            try
            {
                WriteToFile(fileToWrite, exception.Message + Environment.NewLine + exception.StackTrace);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("sctv", "WriteToFile error: " + ex.Message);
            }
        }

        /// <summary>
        /// Write info to a file
        /// </summary>
        /// <param name="fileToWrite"></param>
        /// <param name="value"></param>
        public static void WriteToFile(Exception exception)
        {
            WriteToFile(errorFile, exception);
        }
    }
}
