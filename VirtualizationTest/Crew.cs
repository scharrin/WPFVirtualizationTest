using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VirtualizationTest
{
    public class Crew
    {
        //private BitmapImage img;
        public string id { get; set; }
        public string name { get; set; }
        public string jobTitle { get; set; }
        public string department { get; set; }
        public int major { get; set; }
        public int minor { get; set; }
        public int latitude { get; set; }
        public int longitude { get; set; }
        public int status { get; set; }
        //public ImageSource img { get; set; }
    }
}
