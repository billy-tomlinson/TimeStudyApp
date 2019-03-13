﻿using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.Model
{
    [Table("LapTime")]
    public class LapTime : BaseEntity
    {
        //public string LapIdentity => $"{Cycle}{Sequence}";

        //public string ParentIdentity { get; set; }

        [ForeignKey(typeof(ActivitySampleStudy))]
        public int StudyId { get; set; }

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

        [Ignore]
        public Color ElementColour { get; set; }

        [Ignore]
        public List<Activity> ForeignElements { get; set; }
    }
}
