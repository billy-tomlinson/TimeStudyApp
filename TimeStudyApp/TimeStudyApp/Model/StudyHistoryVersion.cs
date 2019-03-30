using SQLite;
using SQLiteNetExtensions.Attributes;
using TimeStudy.Model;

namespace TimeStudyApp.Model
{
    [Table("StudyHistoryVersion")]
    public class StudyHistoryVersion : BaseEntity
    {
        [ForeignKey(typeof(ActivitySampleStudy))]
        public int StudyId { get; set; }
    }
}
