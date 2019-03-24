using TimeStudy.Model;

namespace TimeStudyApp.Model
{
    public class State : BaseEntity
    {
        public Status CurrentState { get; set; }
        public int ElementRunning { get; set; }
        public int ElementPaused { get; set; }
    }

    public enum Status
    {
        NoElementRunning = 0,
        ElementRunning = 1,
        InterruptElementRunning = 2,
        OccassionalElementRunning = 3
    }
}
