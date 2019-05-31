using System;
using SQLite;
using SQLiteNetExtensions.Attributes;
using TimeStudy.Model;

namespace TimeStudyApp.Model
{
    [Table("TimeStudyHistoryVersion")]
    public class TimeStudyHistoryVersion : BaseEntity
    {
        [ForeignKey(typeof(RatedTimeStudy))]
        public int StudyId { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }

        public DateTime TimeStudyStarted { get; set; }

        public DateTime TimeStudyFinished { get; set; }

        [Ignore]
        public string DateTimeFormatted
        {
            get { return $"{Date.ToString("dd/MM/yyyy")} : {Time.ToString(@"hh\:mm")}"; }
        }
    }
}
