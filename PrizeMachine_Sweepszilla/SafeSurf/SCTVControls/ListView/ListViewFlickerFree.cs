using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SCTV.ListView
{
    #region WM - Window Messages
    public enum WM
    {
        WM_NULL = 0x0000,
        WM_CREATE = 0x0001,
        WM_DESTROY = 0x0002,
        WM_MOVE = 0x0003,
        WM_SIZE = 0x0005,
        WM_ACTIVATE = 0x0006,
        WM_SETFOCUS = 0x0007,
        WM_KILLFOCUS = 0x0008,
        WM_ENABLE = 0x000A,
        WM_SETREDRAW = 0x000B,
        WM_SETTEXT = 0x000C,
        WM_GETTEXT = 0x000D,
        WM_GETTEXTLENGTH = 0x000E,
        WM_PAINT = 0x000F,
        WM_CLOSE = 0x0010,
        WM_QUERYENDSESSION = 0x0011,
        WM_QUIT = 0x0012,
        WM_QUERYOPEN = 0x0013,
        WM_ERASEBKGND = 0x0014,

    }
    #endregion

    #region RECT
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
    #endregion

    public class ListViewFF : System.Windows.Forms.ListView
    {
        bool updating;
        int itemnumber;
        int marqueeSpeed = 700;

        #region Imported User32.DLL functions
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern bool ValidateRect(IntPtr handle, ref RECT rect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        protected static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        #endregion

        protected const int WM_VSCROLL = 0x115;

        private ScrollCommand scrollCommand;
        private int scrollPositionOld;
        private Timer timer;

        private enum ScrollBar : int { Horizontal = 0x0, Vertical = 0x1 }
        public enum ScrollBarDirection : int { Horizontal = 0x0, Vertical = 0x1 }
        public enum ScrollCommand : int { Up = 0x0, Down = 0x1, EndScroll = 0x8 }

        //public void SetScroll(int x, int y)
        //{
        //    this.SetScroll(((int)ScrollBar.Horizontal), x);
        //    this.SetScroll(((int)ScrollBar.Vertical), y);
        //}

        //public void SetScrollX(int position)
        //{
        //    this.SetScroll(((int)ScrollBar.Horizontal), position);
        //}

        //public void SetScrollY(int position)
        //{
        //    this.SetScroll(((int)ScrollBar.Vertical), position);
        //}

        public ListViewFF()
        {

        }

        public void ScrollLeft()
        {
            this.timer = new Timer();
            this.MarqueeSpeed = 700;

            this.scrollPositionOld = int.MinValue;
            this.scrollCommand = ScrollCommand.Down;

          //  this.timer = new Timer() { Interval = this.MarqueeSpeed };
            this.timer.Tick += (sender, e) =>
            {
                int scrollPosition = ListViewFF.GetScrollPos((IntPtr)this.Handle, (int)ScrollBarDirection.Horizontal);
                if (scrollPosition == this.scrollPositionOld)
                {
                    if (this.scrollCommand == ScrollCommand.Down)
                    {
                        this.scrollCommand = ScrollCommand.Up;
                    }
                    else
                    {
                        this.scrollCommand = ScrollCommand.Down;
                    }
                }
                this.scrollPositionOld = scrollPosition;

                ListViewFF.SendMessage((IntPtr)this.Handle, ListViewFF.WM_VSCROLL, (IntPtr)this.scrollCommand, IntPtr.Zero);
                ListViewFF.SendMessage((IntPtr)this.Handle, ListViewFF.WM_VSCROLL, (IntPtr)ScrollCommand.EndScroll, IntPtr.Zero);
            };
            this.timer.Start();
        }

        public void ScrollRight()
        {
            this.timer = new Timer();
            this.MarqueeSpeed = 700;

            this.scrollPositionOld = int.MinValue;
            this.scrollCommand = ScrollCommand.Up;

            //  this.timer = new Timer() { Interval = this.MarqueeSpeed };
            this.timer.Tick += (sender, e) =>
            {
                int scrollPosition = ListViewFF.GetScrollPos((IntPtr)this.Handle, (int)ScrollBarDirection.Horizontal);
                if (scrollPosition == this.scrollPositionOld)
                {
                    //if (this.scrollCommand == ScrollCommand.Down)
                    //{
                    //    this.scrollCommand = ScrollCommand.Down;
                    //}
                    //else
                    //{
                        this.scrollCommand = ScrollCommand.Up;
                    //}
                }
                this.scrollPositionOld = scrollPosition;

                ListViewFF.SendMessage((IntPtr)this.Handle, ListViewFF.WM_VSCROLL, (IntPtr)this.scrollCommand, IntPtr.Zero);
                ListViewFF.SendMessage((IntPtr)this.Handle, ListViewFF.WM_VSCROLL, (IntPtr)ScrollCommand.EndScroll, IntPtr.Zero);
            };
            this.timer.Start();
        }

        public int MarqueeSpeed
        {
            get
            {
                //return marqueeSpeed;
                return 0;
            }
            set
            {
                if (value > 0)
                {
                    marqueeSpeed = value;
                    this.timer.Interval = value;
                }
                else
                {
                    if(this.timer != null)
                        this.timer.Stop();
                }
            }
        }

        //private void SetScroll(ScrollBar bar, int position)
        //{
        //    if (!this.IsDisposed)
        //    {
        //        ListViewFF.SetScrollPos((IntPtr)this.Handle, (int)bar, position, true);
        //        ListViewFF.PostMessage((IntPtr)this.Handle, ListViewFF.WM_VSCROLL, 4 + 0x10000 * position, 0);
        //    }
        //}

        /// <summary>
        /// When adding an item in a loop, use this to update the newly added item.
        /// </summary>
        /// <param name="iIndex">Index of the item just added</param>
        public void UpdateItem(int iIndex)
        {
            updating = true;
            itemnumber = iIndex;
            this.Update();
            updating = false;
        }

        protected override void WndProc(ref Message messg)
        {
            if (updating)
            {
                // We do not want to erase the background, turn this message into a null-message
                if ((int)WM.WM_ERASEBKGND == messg.Msg)
                    messg.Msg = (int)WM.WM_NULL;
                else if ((int)WM.WM_PAINT == messg.Msg)
                {
                    RECT vrect = this.GetWindowRECT();
                    // validate the entire window				
                    ValidateRect(this.Handle, ref vrect);

                    //Invalidate only the new item
                    Invalidate(this.Items[itemnumber].Bounds);
                }

            }
            base.WndProc(ref messg);
        }


        #region private helperfunctions

        // Get the listview's rectangle and return it as a RECT structure
        private RECT GetWindowRECT()
        {
            RECT rect = new RECT();
            rect.left = this.Left;
            rect.right = this.Right;
            rect.top = this.Top;
            rect.bottom = this.Bottom;
            return rect;
        }

        #endregion
    }
	
}
