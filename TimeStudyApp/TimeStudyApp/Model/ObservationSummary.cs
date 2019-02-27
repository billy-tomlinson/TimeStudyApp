using System;
namespace TimeStudy.Model
{
    public class ObservationSummary
    {
        public string ActivityName { get; set; }
        public int NumberOfObservations { get; set; }
        public double TotalTime { get; set; }
        public double Percentage { get; set; }
        public string OperatorName { get; set; }
        public int TotalRating { get; set; }
    }
}
