using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SCTV
{
    [ToolboxItem(true)]
    public partial class ColorTabControl : System.Windows.Forms.TabControl
    {
        // member variables
        System.Drawing.Color _InactiveFGColor = System.Drawing.SystemColors.ControlText;
        System.Drawing.Color _InactiveBGColor = System.Drawing.SystemColors.Control;
        System.Drawing.Font _InactiveTabFontStyle = new Font("Verdana", 9, FontStyle.Bold);
        System.Drawing.Font _ActiveTabFontStyle = new Font("Verdana", 9, FontStyle.Bold);
        string _TabName = string.Empty;
        bool _InactiveColorOn = false;

        public ColorTabControl()
        {
            InitializeComponent();
            this.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.DrawItem += new DrawItemEventHandler(this.RepaintControls);
            this.ControlAdded += new ControlEventHandler(ColorTabControl_ControlAdded);
        }
        //----------------------------------------------------------------------
        void ColorTabControl_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control.GetType() == typeof(TabPage))
            {
                e.Control.Font = new Font(e.Control.Font, e.Control.Font.Style & ~FontStyle.Bold);
            }
        }
        //----------------------------------------------------------------------
        private void RepaintControls(object sender, DrawItemEventArgs e)
        {
            try
            {

                Font _Fnt;
                Brush _BackBrush;
                Brush _ForeBrush;
                Rectangle _Rec = e.Bounds;


                if (e.Index == this.SelectedIndex)
                {
                    // Remove the comment below if you want the font style of selected tab page as normal.
                    // _Fnt = new Font(e.Font, e.Font.Style & ~FontStyle.Bold);

                    // Remove the comment below if you want the font style of selected tab page as bold.
                    _Fnt = new Font(e.Font, e.Font.Style);

                    _BackBrush = new SolidBrush(this.SelectedTab.BackColor);
                    _ForeBrush = new SolidBrush(this.SelectedTab.ForeColor);
                    _Rec = new Rectangle(_Rec.X + (this.Padding.X / 2), _Rec.Y + this.Padding.Y, _Rec.Width + this.Padding.X, _Rec.Height - (this.Padding.Y * 2));
                }
                else
                {

                    // Remove the comment below if you want the font style of inactive tab page as normal.
                    _Fnt = new Font(e.Font, e.Font.Style & ~FontStyle.Bold);

                    // Remove the comment below if you want the font style of inactive tab page as bold.
                    //_Fnt = new Font(e.Font, e.Font.Style);


                    if (this._InactiveColorOn == true)
                    {
                        _BackBrush = new SolidBrush(this.InactiveBGColor);
                        _ForeBrush = new SolidBrush(this.InactiveFGColor);
                    }
                    else
                    {
                        _BackBrush = new SolidBrush(this.TabPages[e.Index].BackColor);
                        _ForeBrush = new SolidBrush(this.TabPages[e.Index].ForeColor);
                    }
                    _Rec = new Rectangle(_Rec.X + (this.Padding.X / 2), _Rec.Y + this.Padding.Y, _Rec.Width + this.Padding.X, _Rec.Height - this.Padding.Y);
                }

                _TabName = this.TabPages[e.Index].Text;
                StringFormat _SF = new StringFormat();
                _SF.Alignment = StringAlignment.Center;

                e.Graphics.FillRectangle(_BackBrush, _Rec);
                e.Graphics.DrawString(_TabName, _Fnt, _ForeBrush, _Rec, _SF);

                _SF.Dispose();
                _BackBrush.Dispose();
                _ForeBrush.Dispose();
                _Fnt.Dispose();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }

        //----------------------------------------------------------------------
        [DefaultValue(typeof(System.Drawing.Font))]
        public System.Drawing.Font InactiveTabFontStyle
        {
            get
            {
                return _InactiveTabFontStyle;
            }
            set
            {
                _InactiveTabFontStyle = value;
                this.OnFontChanged(new EventArgs());

            }
        }

        //----------------------------------------------------------------------
        [DefaultValue(typeof(System.Drawing.Font))]
        public System.Drawing.Font ActiveTabFontStyle
        {
            get
            {
                return _ActiveTabFontStyle;
            }
            set
            {
                _ActiveTabFontStyle = value;
                this.OnFontChanged(new EventArgs());

            }
        }
        //----------------------------------------------------------------------
        [DefaultValue(typeof(System.Drawing.SystemColors), "ControlText")]
        public System.Drawing.Color InactiveFGColor
        {
            get
            {
                return _InactiveFGColor;
            }
            set
            {
                _InactiveFGColor = value;
                this.OnForeColorChanged(new EventArgs());
            }
        }
        //----------------------------------------------------------------------
        [DefaultValue(typeof(System.Drawing.SystemColors), "Control")]
        public System.Drawing.Color InactiveBGColor
        {
            get
            {
                return _InactiveBGColor;
            }
            set
            {
                _InactiveBGColor = value;
                this.OnBackColorChanged(new EventArgs());
            }
        }
        //----------------------------------------------------------------------
        [DefaultValue(false)]
        public bool InactiveColorOn
        {
            get
            {
                return _InactiveColorOn;
            }
            set
            {
                _InactiveColorOn = value;
                this.OnForeColorChanged(new EventArgs());
                this.OnBackColorChanged(new EventArgs());
            }
        }
        //----------------------------------------------------------------------    
    }
}
