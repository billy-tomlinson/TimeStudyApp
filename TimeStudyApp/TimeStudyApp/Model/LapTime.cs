using System.Collections.Generic;
using Xamarin.Forms;

namespace TimeStudy.Model
{
    public class LapTime
    {
        public string LapIdentity => $"{Cycle}{Sequence}";

        public string ParentIdentity { get; set; }

        public bool IsForeignElement { get; set; }

        public string TotalElapsedTime { get; set; }

        public string Element { get; set; }

        public string IndividualLapTime { get; set; }

        public int Cycle { get; set; }

        public double Sequence { get; set; }

        public int Rating { get; set; }

        public Color ElementColour { get; set; }

        public List<Activity> ForeignElements { get; set; }
    }
}
