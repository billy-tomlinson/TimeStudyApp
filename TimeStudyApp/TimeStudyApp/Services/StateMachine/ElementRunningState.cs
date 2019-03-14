using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TimeStudy.Model;
using TimeStudy.Services;
using TimeStudy.ViewModels;

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
                //if (viewModel.CurrentSelectedElement.IsForeignElement)
                //{
                    viewModel.SelectedForeignElements.Add(element);
                    viewModel.ForeignElementCount = viewModel.SelectedForeignElements.Count;
                //}

                var current = viewModel.Get_Running_LapTime();
                //viewModel.LapTimesList.Remove(viewModel.CurrentWithoutLapTime);
                //viewModel.PausedLapTime = viewModel.CurrentWithoutLapTime;
                current.IndividualLapTimeFormatted = "Paused";
                current.TotalElapsedTimeDouble = viewModel.RealTimeTicks;
                current.TotalElapsedTime = viewModel.RealTimeTicks.ToString("0.000");

                //viewModel.LapTimesList.Add(viewModel.PausedLapTime);
                viewModel.LapTimeRepo.SaveItem(current);

                viewModel.TimeWhenForiegnButtonClicked = viewModel.RealTimeTicks;

                var currentForeignLap = new LapTime
                {
                    Cycle = viewModel.CycleCount,
                    Element = element.Name,
                    TotalElapsedTime = "Running",
                    IsForeignElement = element.IsForeignElement,
                    StudyId = Utilities.StudyId
                };

                //viewModel.CurrentWithoutLapTime = currentForeignLap;

                //viewModel.LapTimesList.Add(viewModel.CurrentWithoutLapTime);
                //var id = viewModel.LapTimeRepo.SaveItem(viewModel.CurrentWithoutLapTime);
                //viewModel.CurrentWithoutLapTime = viewModel.Get_Running_LapTime(id);

                viewModel.CurrentApplicationState.CurrentState = Model.Status.ForeignElementRunning;
                stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
            }
            else 
            {
                var current = viewModel.Get_Running_LapTime();
                if(current == null)
                {
                    current = new LapTime
                    {
                        Cycle = viewModel.CycleCount,
                        Element = element.Name,
                        TotalElapsedTime = "Running",
                        IsForeignElement = element.IsForeignElement,
                        StudyId = Utilities.StudyId
                    };

                    viewModel.LapTimeRepo.SaveItem(current);

                    viewModel.CurrentElementWithoutLapTimeName = current.Element;
                    viewModel.CurrentSequence = null;
                    viewModel.CurrentCycle = viewModel.CycleCount;
                }
            }
        }

        public override void ElementSelectedEvent()
        {
         //viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == id);

            var current = viewModel.CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);
            if (current.IsForeignElement)
            {
                viewModel.IsForeignEnabled = false;
                viewModel.CurrentApplicationState.CurrentState = Model.Status.ForeignElementRunning;
                stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
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

            //var current = viewModel.Get_Running_LapTime();
            //viewModel.LapTimesList.Add(viewModel.CurrentLapTime);
            //viewModel.CurrentLapTime = viewModel.Get_Running_LapTime(viewModel.CurrentWithoutLapTime.Id);
            //viewModel.LapTimeRepo.SaveItem(current);
            //viewModel.LapTimes = new ObservableCollection<LapTime>(viewModel.LapTimesList
            //.OrderByDescending(x => x.Cycle)
            //.ThenByDescending(x => x.Sequence));

            viewModel.LapTimes = viewModel.Get_All_LapTimes_Not_Running();

            if (viewModel.ActivitiesCounter == viewModel.ActivitiesCount)
            {
                viewModel.CurrentElementName = viewModel.Activities.FirstOrDefault(x => x.Sequence == 1).Name;
            }
            else
                viewModel.CurrentElementName = viewModel.Activities.FirstOrDefault(x => x.Sequence == viewModel.ActivitiesCounter + 1).Name;

            //viewModel.AddCurrentWithoutLapTimeToList();

            viewModel.ForeignElementCount = 0;

            viewModel.Opacity = 1;
            viewModel.RatingsVisible = false;
            viewModel.ActivitiesVisible = true;

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

            viewModel.CurrentApplicationState.CurrentState = Model.Status.ElementRunning;
            stateservice.SaveApplicationState(viewModel.CurrentApplicationState);
        }
    }
}
