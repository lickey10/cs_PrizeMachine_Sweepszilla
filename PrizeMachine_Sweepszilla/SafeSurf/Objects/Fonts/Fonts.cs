using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Text;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Drawing;

namespace SCTVObjects
{
    public class Fonts
    {
        public ArrayList GetFonts()
        {
            ArrayList fonts = new ArrayList();
            string[] names = this.GetType().Assembly.GetManifestResourceNames();


            foreach (string name in names)
            {
                if(name.ToLower().Contains(".ttf"))
                    fonts.Add(name);
            }

            return fonts;
        }

        public System.Drawing.Font CustomFont(string FontName)
        {
            return CustomFont(FontName, 18);
        }

        public System.Drawing.Font CustomFont(string FontName, int Size)
        {
            System.Drawing.Font customFont = null;

            try
            {
                customFont = new Font((FontFamily)getFontCollection(FontName).Families[0], Size, FontStyle.Regular);
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);

                customFont = SystemFonts.DefaultFont;
            }

            return customFont;
        }

        private PrivateFontCollection getFontCollection(string fontName)
        {
            PrivateFontCollection pfc = new PrivateFontCollection();

            try
            {
                Stream fontStream = this.GetType().Assembly.GetManifestResourceStream("SCTVObjects.Fonts.Fonts." + fontName + ".ttf");

                if (fontStream != null)
                {
                    byte[] fontdata = new byte[fontStream.Length];

                    fontStream.Read(fontdata, 0, (int)fontStream.Length);

                    fontStream.Close();

                    unsafe
                    {
                        fixed (byte* pFontData = fontdata)
                            pfc.AddMemoryFont((System.IntPtr)pFontData, fontdata.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.WriteToFile(ex);
            }

            return pfc;
        }
    }
}
