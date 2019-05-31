using SQLite;

namespace TimeStudy.Model
{
    [Table("WorkElementName")]
    public class WorkElementName : BaseEntity
    {
        public string Name { get; set; }

        public bool IsMerge { get; set; } = false;

        [Ignore]
        public bool Selected { get; set; }
    }

}
