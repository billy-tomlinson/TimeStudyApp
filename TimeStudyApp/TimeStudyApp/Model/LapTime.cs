using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;
using TimeStudy.Services;
using TimeStudyApp.Model;
using Xamarin.Forms;

namespace TimeStudy.Model
{
    [Table("LapTime")]
    public class LapTime : BaseEntity
    {

        [ForeignKey(typeof(RatedTimeStudy))]
        public int StudyId { get; set; }

        public int Version { get; set; }

        [ForeignKey(typeof(WorkElement))]
        public int ActivityId { get; set; }

        public bool IsForeignElement { get; set; }

        public string TotalElapsedTime { get; set; }

        public string IndividualLapTime { get; set; }

        public double TotalElapsedTimeDouble { get; set; }

        public double IndividualLapTimeDouble { get; set; }

        public TimeSpan TotalElapsedTimeImperial { get; set; }

        public TimeSpan IndividualLapTimeImperial { get; set; }

        public double TimeWhenLapStarted { get; set; }

        public bool HasBeenPaused { get; set; }

        public string Element { get; set; }

        [Ignore]
        public double IndividualLapBMS 
        {
            get
            {
                return Rating != null && Rating != 0 ? IndividualLapTimeDouble * (int)Rating / 100 : IndividualLapTimeDouble;
            }
        }

        public int Cycle { get; set; }

        public int? Sequence { get; set; }

        public int? Rating { get; set; }

        public bool IsRated { get; set; }

        public RunningStatus Status { get; set; }

        public bool IsValueAdded { get; set; }

        [Ignore]
        public Color ElementColour { get; set; }

        [Ignore]
        public List<WorkElement> ForeignElements { get; set; }
    }
}
