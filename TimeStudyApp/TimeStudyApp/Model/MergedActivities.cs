using SQLite;
using SQLiteNetExtensions.Attributes;

namespace TimeStudy.Model
{
    [Table("MergedActivities")] 
    public class MergedActivities : BaseEntity
    {
        [ForeignKey(typeof(WorkElement))]
        public int ActivityId { get; set; }

        [ForeignKey(typeof(WorkElement))]
        public int MergedActivityId { get; set; }
    }
}
