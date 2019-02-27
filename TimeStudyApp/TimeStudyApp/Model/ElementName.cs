using SQLite;

namespace TimeStudy.Model
{
    [Table("ElementName")]
    public class ElementName : BaseEntity
    {
        public string Name { get; set; }

        [Ignore]
        public bool Selected { get; set; }
    }

}
