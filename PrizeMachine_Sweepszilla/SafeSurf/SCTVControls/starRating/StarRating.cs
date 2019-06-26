using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SCTV {
    
    public partial class StarRating : UserControl {

        public enum Layouts {
            Horizontal,
            Vertical
        }

        public event RatingValueChangedEventHandler RatingValueChanged;
        public delegate void RatingValueChangedEventHandler(object sender, RatingChangedEventArgs e);

        private Layouts _controlLayout = Layouts.Horizontal;
        public Layouts ControlLayout {
            get { return _controlLayout; }
            set {
                _controlLayout = value;
                OrientControl();
            }
        }

        private int _rating = 0;
        public int Rating {
            get { return _rating; }
            set {
                _rating = value;
                ShowRating();
            }
        }

        private BorderStyle _wrapperPanelBorderStyle = BorderStyle.None;
        public BorderStyle WrapperPanelBorderStyle {
            get { return this.BorderStyle; }
            set {
                _wrapperPanelBorderStyle = value;
                this.BorderStyle = value;
            }
        }

        public StarRating() {
            InitializeComponent();
        }

        private void StarRating_Load(object sender, EventArgs e) {
            OrientControl();
            ShowRating();
        }

        private void OrientControl() {

            switch (ControlLayout) {

                case Layouts.Vertical:
                    this.Size = new Size(22, 102);
                    pbNoRating.Location = new Point(2, 82);
                    pbStar1.Location = new Point(2, 66);
                    pbStar2.Location = new Point(2, 50);
                    pbStar3.Location = new Point(2, 34);
                    pbStar4.Location = new Point(2, 18);
                    pbStar5.Location = new Point(2, 2);
                    break;

                case Layouts.Horizontal:
                    this.Size = new Size(102, 22);
                    pbNoRating.Location = new Point(0, 2);
                    pbStar1.Location = new Point(17, 1);
                    pbStar2.Location = new Point(34, 1);
                    pbStar3.Location = new Point(51, 1);
                    pbStar4.Location = new Point(68, 1);
                    pbStar5.Location = new Point(85, 1);
                    break;

            }

        }

        private void ShowRating() {

            switch (this.Rating) {
                case 0:
                    pbStar1.Image = Properties.Resources.rating_star_disabled;
                    pbStar2.Image = Properties.Resources.rating_star_disabled;
                    pbStar3.Image = Properties.Resources.rating_star_disabled;
                    pbStar4.Image = Properties.Resources.rating_star_disabled;
                    pbStar5.Image = Properties.Resources.rating_star_disabled;

                    toolTips.SetToolTip(pbNoRating, "No Rating");
                    toolTips.SetToolTip(pbStar1, "Rate 1 Star");
                    toolTips.SetToolTip(pbStar2, "Rate 2 Stars");
                    toolTips.SetToolTip(pbStar3, "Rate 3 Stars");
                    toolTips.SetToolTip(pbStar4, "Rate 4 Stars");
                    toolTips.SetToolTip(pbStar5, "Rate 5 Stars");
                    break;

                case 1:
                    pbStar1.Image = Properties.Resources.rating_star_enabled;
                    pbStar2.Image = Properties.Resources.rating_star_disabled;
                    pbStar3.Image = Properties.Resources.rating_star_disabled;
                    pbStar4.Image = Properties.Resources.rating_star_disabled;
                    pbStar5.Image = Properties.Resources.rating_star_disabled;

                    toolTips.SetToolTip(pbNoRating, "Remove Rating");
                    toolTips.SetToolTip(pbStar1, "Rated 1 Star");
                    toolTips.SetToolTip(pbStar2, "Rate 2 Stars");
                    toolTips.SetToolTip(pbStar3, "Rate 3 Stars");
                    toolTips.SetToolTip(pbStar4, "Rate 4 Stars");
                    toolTips.SetToolTip(pbStar5, "Rate 5 Stars");
                    break;

                case 2:
                    pbStar1.Image = Properties.Resources.rating_star_enabled;
                    pbStar2.Image = Properties.Resources.rating_star_enabled;
                    pbStar3.Image = Properties.Resources.rating_star_disabled;
                    pbStar4.Image = Properties.Resources.rating_star_disabled;
                    pbStar5.Image = Properties.Resources.rating_star_disabled;

                    toolTips.SetToolTip(pbNoRating, "Remove Rating");
                    toolTips.SetToolTip(pbStar1, "Rate 1 Star");
                    toolTips.SetToolTip(pbStar2, "Rated 2 Stars");
                    toolTips.SetToolTip(pbStar3, "Rate 3 Stars");
                    toolTips.SetToolTip(pbStar4, "Rate 4 Stars");
                    toolTips.SetToolTip(pbStar5, "Rate 5 Stars");
                    break;

                case 3:
                    pbStar1.Image = Properties.Resources.rating_star_enabled;
                    pbStar2.Image = Properties.Resources.rating_star_enabled;
                    pbStar3.Image = Properties.Resources.rating_star_enabled;
                    pbStar4.Image = Properties.Resources.rating_star_disabled;
                    pbStar5.Image = Properties.Resources.rating_star_disabled;

                    toolTips.SetToolTip(pbNoRating, "Remove Rating");
                    toolTips.SetToolTip(pbStar1, "Rate 1 Star");
                    toolTips.SetToolTip(pbStar2, "Rate 2 Stars");
                    toolTips.SetToolTip(pbStar3, "Rated 3 Stars");
                    toolTips.SetToolTip(pbStar4, "Rate 4 Stars");
                    toolTips.SetToolTip(pbStar5, "Rate 5 Stars");
                    break;

                case 4:
                    pbStar1.Image = Properties.Resources.rating_star_enabled;
                    pbStar2.Image = Properties.Resources.rating_star_enabled;
                    pbStar3.Image = Properties.Resources.rating_star_enabled;
                    pbStar4.Image = Properties.Resources.rating_star_enabled;
                    pbStar5.Image = Properties.Resources.rating_star_disabled;

                    toolTips.SetToolTip(pbNoRating, "Remove Rating");
                    toolTips.SetToolTip(pbStar1, "Rate 1 Star");
                    toolTips.SetToolTip(pbStar2, "Rate 2 Stars");
                    toolTips.SetToolTip(pbStar3, "Rate 3 Stars");
                    toolTips.SetToolTip(pbStar4, "Rated 4 Stars");
                    toolTips.SetToolTip(pbStar5, "Rate 5 Stars");
                    break;

                case 5:
                    pbStar1.Image = Properties.Resources.rating_star_enabled;
                    pbStar2.Image = Properties.Resources.rating_star_enabled;
                    pbStar3.Image = Properties.Resources.rating_star_enabled;
                    pbStar4.Image = Properties.Resources.rating_star_enabled;
                    pbStar5.Image = Properties.Resources.rating_star_enabled;

                    toolTips.SetToolTip(pbNoRating, "Remove Rating");
                    toolTips.SetToolTip(pbStar1, "Rate 1 Star");
                    toolTips.SetToolTip(pbStar2, "Rate 2 Stars");
                    toolTips.SetToolTip(pbStar3, "Rate 3 Stars");
                    toolTips.SetToolTip(pbStar4, "Rate 4 Stars");
                    toolTips.SetToolTip(pbStar5, "Rated 5 Stars");
                    break;

            }

        }

        private void ShowRatingHover(int newRating) {

            switch (newRating) {
                case 0:
                    pbStar1.Image = Properties.Resources.rating_star_disabled;
                    pbStar2.Image = Properties.Resources.rating_star_disabled;
                    pbStar3.Image = Properties.Resources.rating_star_disabled;
                    pbStar4.Image = Properties.Resources.rating_star_disabled;
                    pbStar5.Image = Properties.Resources.rating_star_disabled;
                    break;

                case 1:
                    pbStar1.Image = Properties.Resources.rating_star_enabled;
                    pbStar2.Image = Properties.Resources.rating_star_disabled;
                    pbStar3.Image = Properties.Resources.rating_star_disabled;
                    pbStar4.Image = Properties.Resources.rating_star_disabled;
                    pbStar5.Image = Properties.Resources.rating_star_disabled;
                    break;

                case 2:
                    pbStar1.Image = Properties.Resources.rating_star_enabled;
                    pbStar2.Image = Properties.Resources.rating_star_enabled;
                    pbStar3.Image = Properties.Resources.rating_star_disabled;
                    pbStar4.Image = Properties.Resources.rating_star_disabled;
                    pbStar5.Image = Properties.Resources.rating_star_disabled;
                    break;

                case 3:
                    pbStar1.Image = Properties.Resources.rating_star_enabled;
                    pbStar2.Image = Properties.Resources.rating_star_enabled;
                    pbStar3.Image = Properties.Resources.rating_star_enabled;
                    pbStar4.Image = Properties.Resources.rating_star_disabled;
                    pbStar5.Image = Properties.Resources.rating_star_disabled;
                    break;

                case 4:
                    pbStar1.Image = Properties.Resources.rating_star_enabled;
                    pbStar2.Image = Properties.Resources.rating_star_enabled;
                    pbStar3.Image = Properties.Resources.rating_star_enabled;
                    pbStar4.Image = Properties.Resources.rating_star_enabled;
                    pbStar5.Image = Properties.Resources.rating_star_disabled;
                    break;

                case 5:
                    pbStar1.Image = Properties.Resources.rating_star_enabled;
                    pbStar2.Image = Properties.Resources.rating_star_enabled;
                    pbStar3.Image = Properties.Resources.rating_star_enabled;
                    pbStar4.Image = Properties.Resources.rating_star_enabled;
                    pbStar5.Image = Properties.Resources.rating_star_enabled;
                    break;

            }

        }

        #region Click Events

        private void pbNoRating_Click(object sender, System.EventArgs e) {
            int pre = Rating;
            Rating = 0;
            if (RatingValueChanged != null) {
                RatingValueChanged(this, new RatingChangedEventArgs(pre, Rating));
            }
        }

        private void pbStar1_Click(object sender, System.EventArgs e) {
            int pre = Rating;
            Rating = 1;
            if (RatingValueChanged != null) {
                RatingValueChanged(this, new RatingChangedEventArgs(pre, Rating));
            }
        }

        private void pbStar2_Click(object sender, System.EventArgs e) {
            int pre = Rating;
            Rating = 2;
            if (RatingValueChanged != null) {
                RatingValueChanged(this, new RatingChangedEventArgs(pre, Rating));
            }
        }

        private void pbStar3_Click(object sender, System.EventArgs e) {
            int pre = Rating;
            Rating = 3;
            if (RatingValueChanged != null) {
                RatingValueChanged(this, new RatingChangedEventArgs(pre, Rating));
            }
        }

        private void pbStar4_Click(object sender, System.EventArgs e) {
            int pre = Rating;
            Rating = 4;
            if (RatingValueChanged != null) {
                RatingValueChanged(this, new RatingChangedEventArgs(pre, Rating));
            }
        }

        private void pbStar5_Click(object sender, System.EventArgs e) {
            int pre = Rating;
            Rating = 5;
            if (RatingValueChanged != null) {
                RatingValueChanged(this, new RatingChangedEventArgs(pre, Rating));
            }
        }

        #endregion

        #region Mouse Hover Events

        private void pbNoRating_MouseHover(object sender, System.EventArgs e) {
            ShowRatingHover(0);
        }

        private void pbStar1_MouseHover(object sender, System.EventArgs e) {
            ShowRatingHover(1);
        }

        private void pbStar2_MouseHover(object sender, System.EventArgs e) {
            ShowRatingHover(2);
        }

        private void pbStar3_MouseHover(object sender, System.EventArgs e) {
            ShowRatingHover(3);
        }

        private void pbStar4_MouseHover(object sender, System.EventArgs e) {
            ShowRatingHover(4);
        }

        private void pbStar5_MouseHover(object sender, System.EventArgs e) {
            ShowRatingHover(5);
        }

        #endregion

        #region Mouse Leave Events
        
        private void pbNoRating_MouseLeave(object sender, System.EventArgs e) {
            ShowRating();
        }

        private void pbStar1_MouseLeave(object sender, System.EventArgs e) {
            ShowRating();
        }

        private void pbStar2_MouseLeave(object sender, System.EventArgs e) {
            ShowRating();
        }

        private void pbStar3_MouseLeave(object sender, System.EventArgs e) {
            ShowRating();
        }

        private void pbStar4_MouseLeave(object sender, System.EventArgs e) {
            ShowRating();
        }

        private void pbStar5_MouseLeave(object sender, System.EventArgs e) {
            ShowRating();
        }

        #endregion

    }

}
