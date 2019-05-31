namespace TimeStudy.Model
{
    public class MultipleActivities
    {
        public MultipleActivities()
        {
            ActivityOne = new WorkElement();
            ActivityTwo = new WorkElement();
            ActivityThree = new WorkElement();
        }

        public WorkElement ActivityOne { get; set; }
        public WorkElement ActivityTwo { get; set; }
        public WorkElement ActivityThree { get; set; }
    }
}
