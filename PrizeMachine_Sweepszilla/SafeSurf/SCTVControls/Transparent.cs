using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SCTV
{
    public static class Transparent
    {
        public static void ToTransparent(this System.Windows.Forms.Button Button, System.Drawing.Color TransparentColor)
        {
            Bitmap bmp = ((Bitmap)Button.Image);
            //Bitmap bmp = new Bitmap(100, 100);
            //Button.Image = bmp;
            bmp.MakeTransparent(TransparentColor);
            int x = (Button.Width - bmp.Width) / 2;
            int y = (Button.Height - bmp.Height) / 2;
            Graphics gr = Button.CreateGraphics();
            gr.DrawImage(bmp, x, y);
        }
    }
}
