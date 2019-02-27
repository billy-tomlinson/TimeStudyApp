namespace TimeStudy.Model
{
    public class MultipleElements
    {
        public MultipleElements()
        {
            ElementOne = new WorkElement();
            ElementTwo = new WorkElement();
            ElementThree = new WorkElement();
        }

        public WorkElement ElementOne { get; set; }
        public WorkElement ElementTwo { get; set; }
        public WorkElement ElementThree { get; set; }
    }
}
