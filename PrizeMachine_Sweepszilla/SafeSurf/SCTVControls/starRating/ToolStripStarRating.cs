using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SCTV {

    [ToolStripItemDesignerAvailabilityAttribute(ToolStripItemDesignerAvailability.All)]
    public class ToolStripStarRating : ToolStripControlHost {

        public ToolStripStarRating()
            : base(new StarRating()) {
        }

        public StarRating InnerControl {
            get { return (StarRating)Control; }
        }

        public int Rating {
            get { return InnerControl.Rating; }
            set { InnerControl.Rating = value; }
        }

        public event EventHandler<RatingChangedEventArgs> RatingChanged;

        protected override void OnSubscribeControlEvents(Control c) {

            // Call the base so the base events are connected. 
            base.OnSubscribeControlEvents(c);

            // Cast the control to a rating control. 
            StarRating rater = (StarRating)c;

            // Add the event. 
            rater.RatingValueChanged += HandleRatingValueChanged;

        }

        protected override void OnUnsubscribeControlEvents(Control c) {
            // Call the base method so the basic events are unsubscribed. 
            base.OnUnsubscribeControlEvents(c);

            // Cast the control to a rating control. 
            StarRating rater = (StarRating)c;

            // Add the event. 
            rater.RatingValueChanged -= HandleRatingValueChanged;

        }

        private void HandleRatingValueChanged(object sender, RatingChangedEventArgs e) {
            if (RatingChanged != null) {
                RatingChanged(this, e);
            }
        }

    }

}
