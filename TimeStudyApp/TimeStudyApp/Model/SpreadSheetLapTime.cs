using System;
namespace TimeStudyApp.Model
{
    public class SpreadSheetLapTime
    {

        public int StudyId { get; set; }

        public string Element { get; set; }

        public string TotalElapsedTime { get; set; }

        public string IndividualLapTimeFormatted { get; set; }

        public bool IsForeignElement { get; set; }

        public int? Rating { get; set; }

    }
}
