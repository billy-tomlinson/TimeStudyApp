using SQLite;

namespace TimeStudy.Model
{
    [Table("ActivityName")]
    public class ActivityName : BaseEntity
    {
        public string Name { get; set; }

        public bool IsMerge { get; set; } = false;

        [Ignore]
        public bool Selected { get; set; }
    }

}
