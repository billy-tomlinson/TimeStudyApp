using System.Collections.Generic;
using System.Drawing;
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
            if (Utilities.IsForeignElement)
            {
                viewModel.SelectedForeignElements.Add(element);
                viewModel.ForeignElementCount = viewModel.SelectedForeignElements.Count;

                var current = viewModel.Get_Running_LapTime();
                current.Status = Model.RunningStatus.Paused;
                current.TotalElapsedTimeDouble = Utilities.TimeWhenLapOrForiegnButtonClicked;
                current.TotalElapsedTime = Utilities.TimeWhenLapOrForiegnButtonClicked.ToString("0.000");
                current.HasBeenPaused = true;
                viewModel.LapTimeRepo.SaveItem(current);

                current = Utilities.SetUpCurrentLapTime(viewModel.CycleCount,
                    element.Name, RunningStatus.Running, element.Rated, viewModel.LapTime);

                viewModel.CurrentApplicationState.CurrentState = Model.Status.InterruptElementRunning;
                stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
            }
            else 
            {
                var current = viewModel.Get_Running_LapTime();
                if(current == null)
                {

                    current = Utilities.SetUpCurrentLapTime(viewModel.CycleCount,
                        element.Name,  RunningStatus.Running, element.Rated, viewModel.LapTime);

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
            if (Utilities.IsForeignElement)
            {
                viewModel.IsForeignEnabled = false;

                var lastLap = viewModel.Get_Last_NonForeign_LapTime().Rating;
                if(lastLap == null) 
                {
                    if(current.Rated)
                        viewModel.CurrentApplicationState.CurrentState = Status.InterruptElementRunning;
                    else
                        viewModel.CurrentApplicationState.CurrentState = Status.UnratedInterruptElementRunning;
                }
                else 
                {
                    if (current.Rated)
                        viewModel.CurrentApplicationState.CurrentState = Status.OccassionalElementRunning;
                    else
                        viewModel.CurrentApplicationState.CurrentState = Status.UnratedOccassionalElementRunning;
                }
            }
            else
            {
                if (current.Rated)
                    viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
                else
                    viewModel.CurrentApplicationState.CurrentState = Status.UnratedOccassionalElementRunning;

                viewModel.IsForeignEnabled = true;
            }

            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void RatingSelectedEvent()
        {
            viewModel.AllForiegnLapTimes = new List<LapTime>();

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

        public override void ShowStandardElements()
        {
            viewModel.CollectionOfElements = viewModel.Get_All_Enabled_Activities_WithChildren();
            viewModel.GroupElementsForActivitiesView();

            viewModel.LapTimerEvent();
            viewModel.IsCancelEnabled = true;
            viewModel.CurrentApplicationState.CurrentState = Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }

        public override void CloseActivitiesViewEvent()
        {
            viewModel.IsForeignEnabled = true;
        }
    }
}
