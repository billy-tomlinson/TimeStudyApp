namespace TimeStudy.Model
{
    public class MultipleActivities
    {
        public MultipleActivities()
        {
            ActivityOne = new Activity();
            ActivityTwo = new Activity();
            ActivityThree = new Activity();
        }

        public Activity ActivityOne { get; set; }
        public Activity ActivityTwo { get; set; }
        public Activity ActivityThree { get; set; }
    }
}
