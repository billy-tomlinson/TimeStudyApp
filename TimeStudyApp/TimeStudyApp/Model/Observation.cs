using System;
using SQLiteNetExtensions.Attributes;

namespace TimeStudy.Model
{
    public class Observation : BaseEntity
    {
        [ForeignKey(typeof(Operator))]
        public int OperatorId { get; set; }

        [ForeignKey(typeof(Activity))]
        public int ActivityId { get; set; }

        public int AliasActivityId { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public int Rating { get; set; }

        public string ActivityName { get; set; }

        public int ObservationNumber { get; set; }

        [ForeignKey(typeof(ActivitySampleStudy))]
        public int StudyId { get; set; }
    }
}
