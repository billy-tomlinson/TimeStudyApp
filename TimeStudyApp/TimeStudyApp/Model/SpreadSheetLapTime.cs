using System;
namespace TimeStudyApp.Model
{
    public class SpreadSheetLapTime
    {

        public int StudyId { get; set; }

        public int ElementId { get; set; }

        public string Element { get; set; }

        public double TotalElapsedTime { get; set; }

        public double IndividualLapTime { get; set; }

        public bool IsForeignElement { get; set; }

        public int? Rating { get; set; }

        public double IndividualLapTimeNormalised { get; set; }

    }
}
