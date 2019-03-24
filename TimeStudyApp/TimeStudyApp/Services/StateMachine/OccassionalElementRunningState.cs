using System;
using System.Linq;
using TimeStudy.Services;
using TimeStudy.ViewModels;
using TimeStudyApp.Model;

namespace TimeStudyApp.Services.StateMachine
{
    public class OccassionalElementRunningState : BaseState
    {

        TimeStudyUnsequencedViewModel viewModel;
        ApplicationState stateservice = new ApplicationState();

        public OccassionalElementRunningState(TimeStudyUnsequencedViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override void AddElementWithoutLapTimeToList()
        {
            var element = viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);
            //if (element.IsForeignElement)
            //{
            //    viewModel.SelectedForeignElements.Add(element);
            //    viewModel.ForeignElementCount = viewModel.SelectedForeignElements.Count;

            //    var current = viewModel.Get_Running_LapTime();
            //    current.Status = Model.RunningStatus.Paused;
            //    current.TotalElapsedTimeDouble = viewModel.RealTimeTicks;
            //    current.TotalElapsedTime = viewModel.RealTimeTicks.ToString("0.000");

            //    viewModel.LapTimeRepo.SaveItem(current);

            //    viewModel.TimeWhenForiegnButtonClicked = viewModel.RealTimeTicks;

            //    current = Utilities.SetUpCurrentLapTime(viewModel.CycleCount,
            //        element.Name, element.IsForeignElement, RunningStatus.Running);

            //    viewModel.CurrentApplicationState.CurrentState = Model.Status.InterruptElementRunning;
            //    stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
            //}
            //else
            //{
                var current = viewModel.Get_Running_LapTime();
                if (current == null)
                {

                    current = Utilities.SetUpCurrentLapTime(viewModel.CycleCount,
                        element.Name, element.IsForeignElement, RunningStatus.Running);

                    Utilities.CurrentRunningElementId = viewModel.LapTimeRepo.SaveItem(current);

                    viewModel.CurrentElementWithoutLapTimeName = current.Element;
                    viewModel.CurrentSequence = null;
                    viewModel.CurrentCycle = viewModel.CycleCount;

                    viewModel.IsForeignEnabled = false;
                }
            //}
        }

        public override void CloseActivitiesViewEvent()
        {
            throw new NotImplementedException();
        }

        public override void ElementSelectedEvent()
        {
            throw new NotImplementedException();
        }

        public override void RatingSelectedEvent()
        {
            throw new NotImplementedException();
        }

        public override void ShowForeignElements()
        {
            throw new NotImplementedException();
        }

        public override void ShowNonForeignElements()
        {
            viewModel.LapTimerEvent();
            viewModel.IsCancelEnabled = false;
            viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }
    }
}
