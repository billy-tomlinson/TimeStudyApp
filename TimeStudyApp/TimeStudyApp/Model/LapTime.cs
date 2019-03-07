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

        public double TotalElapsedTimeDouble { get; set; }

        public string Element { get; set; }

        public string IndividualLapTimeFormatted { get; set; }

        public double IndividualLapTime { get; set; }

        public int Cycle { get; set; }

        public int? Sequence { get; set; }

        public int? Rating { get; set; }

        public bool IsRated { get; set; }

        public Color ElementColour { get; set; }

        public List<Activity> ForeignElements { get; set; }
    }
}
