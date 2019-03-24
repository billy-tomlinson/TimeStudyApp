using System.Collections.Generic;
using System.Linq;
using TimeStudy.Model;
using TimeStudy.Services;
using TimeStudy.ViewModels;
using TimeStudyApp.Model;

namespace TimeStudyApp.Services.StateMachine
{
    public class ElementRunningState : BaseState
    {
        TimeStudyUnsequencedViewModel viewModel;
        ApplicationState stateservice = new ApplicationState();

        public ElementRunningState(TimeStudyUnsequencedViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override void AddElementWithoutLapTimeToList()
        {
            var element = viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);
            if (element.IsForeignElement)
            {
                viewModel.SelectedForeignElements.Add(element);
                viewModel.ForeignElementCount = viewModel.SelectedForeignElements.Count;

                var current = viewModel.Get_Running_LapTime();
                current.Status = Model.RunningStatus.Paused;
                current.TotalElapsedTimeDouble = viewModel.RealTimeTicks;
                current.TotalElapsedTime = viewModel.RealTimeTicks.ToString("0.000");

                viewModel.LapTimeRepo.SaveItem(current);

                viewModel.TimeWhenForiegnButtonClicked = viewModel.RealTimeTicks;

                current = Utilities.SetUpCurrentLapTime(viewModel.CycleCount,
                    element.Name, element.IsForeignElement, RunningStatus.Running);

                viewModel.CurrentApplicationState.CurrentState = Model.Status.InterruptElementRunning;
                stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
            }
            else 
            {
                var current = viewModel.Get_Running_LapTime();
                if(current == null)
                {

                    current = Utilities.SetUpCurrentLapTime(viewModel.CycleCount,
                        element.Name, element.IsForeignElement, RunningStatus.Running);

                    Utilities.CurrentRunningElementId = viewModel.LapTimeRepo.SaveItem(current);

                    viewModel.CurrentElementWithoutLapTimeName = current.Element;
                    viewModel.CurrentSequence = null;
                    viewModel.CurrentCycle = viewModel.CycleCount;

                    viewModel.IsForeignEnabled = true;
                }
            }
        }

        public override void ElementSelectedEvent()
        {
            var current = viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);
            if (current.IsForeignElement)
            {
                viewModel.IsForeignEnabled = false;

                var lastLap = viewModel.Get_Last_LapTime().Rating;
                if(lastLap == null) 
                {
                    viewModel.CurrentApplicationState.CurrentState = Status.InterruptElementRunning;
                    stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
                }
                else 
                {
                    viewModel.CurrentApplicationState.CurrentState = Status.OccassionalElementRunning;
                    stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
                }
            }
            else
            {
                viewModel.IsForeignEnabled = true;
                viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
                stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
            }
        }

        public override void RatingSelectedEvent()
        {
            viewModel.AllForiegnLapTimes = new List<LapTime>();
            viewModel.TimeWhenForiegnButtonClicked = 0;

            viewModel.LapTimes = viewModel.Get_All_LapTimes_Not_Running();

            viewModel.ForeignElementCount = 0;

            viewModel.Opacity = 0.2;
            viewModel.RatingsVisible = false;
            viewModel.ActivitiesVisible = true;
            viewModel.IsPageEnabled = false;

            viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void ShowForeignElements()
        {
            viewModel.Opacity = 0.2;
            viewModel.RatingsVisible = false;
            viewModel.ActivitiesVisible = true;

            viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void ShowNonForeignElements()
        {
            viewModel.LapTimerEvent();
            viewModel.IsCancelEnabled = false;
            viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void CloseActivitiesViewEvent()
        {
            viewModel.IsForeignEnabled = true;
        }

    }
}
