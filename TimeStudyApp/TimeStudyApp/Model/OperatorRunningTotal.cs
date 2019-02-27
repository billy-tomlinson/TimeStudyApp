namespace TimeStudy.Model
{
    public class OperatorRunningTotal
    {
        public int OperatorId { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public int NumberOfObservations { get; set; }
        public double Percentage { get; set; }
        public int ObservationsRequired { get; set; }
        public string PercentageFormatted { get; set; }
        public bool IsRated { get; set; }
    }
}
