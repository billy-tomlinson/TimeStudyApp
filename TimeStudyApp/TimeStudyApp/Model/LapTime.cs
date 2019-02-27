﻿using System.Collections.Generic;
using Xamarin.Forms;

namespace TimeStudy.Model
{
    public class LapTime
    {
        public string TotalElapsedTime { get; set; }

        public string Element { get; set; }

        public string IndividualLapTime { get; set; }

        public int Count { get; set; }

        public int Cycle { get; set; }

        public int Sequence { get; set; }

        public int Rating { get; set; }

        public Color ElementColour { get; set; }

        public List<Activity> ForeignElements { get; set; }
    }
}
