using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCTV
{
    class browserAttributes
    {
        public System.Windows.Forms.Timer RefreshTimer;
        public bool WatchingVideo = false;
        public bool FoundCategory = false;
        public bool FoundVideo = false;
        public System.Windows.Forms.Timer KeepRunningTimer;
        ArrayList videosList;
    }
}
