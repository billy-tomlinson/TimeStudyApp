﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TimeStudy.Model;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class TimeStudyViewModel : BaseViewModel
    {

        public Command StartTimer { get; set; }
        public Command LapTimer { get; set; }
        public Command StopTimer { get; set; }
        public Command ClearLaps { get; set; }
        public Command RatingSelected { get; set; }
        public Command ItemClickedCommand { get; set; }
        public Command RunningItemClickedCommand { get; set; }
        public Command ShowForeignElements { get; set; }
        public Command CloseForeignElements { get; set; }
        public Command ForeignElementSelected { get; set; }
        public Command CloseActivitiesView { get; set; }

        private bool IsRunning;
        private bool cancelActivitiesView;
        private bool HasBeenStopped;
        private bool lapTimerEventClicked;
        public double TimeWhenLapButtonClicked { get; set; }
        public double TimeWhenForiegnButtonClicked { get; set; }
        public double TimeWhenStopButtonClicked { get; set; }
        public double LapTime { get; set; }
        public double CurrentTicks { get; set; }
        public double LastSuccesstulLapTime { get; set; }
        public TimeSpan StartTime { get; set; }

        ObservableCollection<Activity> AllForeignElements;

        List<Activity> SelectedForeignElements;

        List<LapTime> AllForiegnLapTimes = new List<LapTime>();

        static int ActivitiesCount;
        static int ActivitiesCounter;
        static int CycleCount;

        LapTime CurrentLapTime;
        LapTime CurrentWithoutLapTime;
        Activity CurrentForeignElement;

        public TimeStudyViewModel()
        {
            ConstructorSetUp();
        }

        private void ConstructorSetUp()
        {

            StartTimer = new Command(StartTimerEvent);
            StopTimer = new Command(StopTimerEvent);
            LapTimer = new Command(LapTimerEvent);
            ClearLaps = new Command(ClearLapsEvent);
            Override = new Command(OverrideEvent);
            RatingSelected = new Command(RatingSelectedEvent);
            ShowForeignElements = new Command(ShowForeignElementsEvent);
            CloseForeignElements = new Command(CloseForeignElementsEvent);
            ForeignElementSelected = new Command(ForeignElementSelectedEvent);
            ItemClickedCommand = new Command(ShowForeignElementsEvent);
            CloseActivitiesView = new Command(CloseActivitiesViewEvent);

            LapTimes = new ObservableCollection<LapTime>();
            LapTimesList = new List<LapTime>();
            SelectedForeignElements = new List<Activity>();

            Activities = Get_All_ValueAdded_Rated_Enabled_Activities_WithChildren();

            AllForeignElements = Get_All_NonValueAdded_Enabled_Activities();

            IEnumerable<Activity> obsCollection = AllForeignElements;

            var list1 = new List<Activity>(obsCollection);

            foreach (var activity in list1)
            {
                activity.Colour = Color.FromHex(activity.ItemColour);
            };

            AllForeignElements = ConvertListToObservable(list1);

            GroupActivities = Utilities.BuildGroupOfActivities(AllForeignElements);

            IsPageVisible = IsStudyValid();

            LapTime = 0;
            CurrentTicks = 0;
            StopWatchTime = "0.000";
            IsRunning = false;
            IsStartEnabled = true;
            IsLapEnabled = false;
            IsStopEnabled = false;
            IsClearEnabled = false;
            ActivitiesCount = Activities.Count;
            CycleCount = 1;

            LapButtonText = "   Lap   ";
        }

        private bool IsStudyValid()
        {

            if (Utilities.StudyId == 0 || Utilities.IsCompleted)
                return false;

            if (Activities.Count == 0)
            {
                InvalidText = $"Please add at least one element to study {Utilities.StudyId.ToString()}";
                return false;
            }

            return true;
        }

        public void CloseActivitiesViewEvent()
        {
            cancelActivitiesView = true;
            Opacity = 1;
            LapTimerEvent();
            ActivitiesVisible = false;
            lapTimerEventClicked = false;
        }

        public void StartTimerEvent()
        {
            IsRunning = true;
            IsStartEnabled = false;
            IsLapEnabled = true;
            IsStopEnabled = true;
            IsClearEnabled = false;
            StartTime = DateTime.Now.TimeOfDay;
            StartTimeFormatted = StartTime.ToString(@"c");
            CurrentElement = Activities.FirstOrDefault(x => x.Sequence == ActivitiesCounter + 1);
            CurrentElementName = CurrentElement?.Name;

            RunTimer();
            if(!HasBeenStopped)
                AddCurrentWithoutLapTimeToList();

            HasBeenStopped = false;
        }

        public void StopTimerEvent()
        {
            IsRunning = false;
            IsStartEnabled = true;
            IsLapEnabled = false;
            IsStopEnabled = false;
            IsClearEnabled = true;
            IsStartEnabled = true;
            HasBeenStopped = true;

            TimeWhenStopButtonClicked = RealTimeTicks;
        }

        public void ClearLapsEvent()
        {
            ValidationText = "Are you sure you want to clear and reset the stop watch?";
            ShowOkCancel = true;
            IsOverrideVisible = false;
            ShowClose = false;
            Opacity = 0.2;
            CloseColumnSpan = 1;
            IsInvalid = true;
        }

        void OverrideEvent(object sender)
        {
            ConstructorSetUp();
            ShowOkCancel = false;
            IsInvalid = false;
            IsOverrideVisible = false;
            Opacity = 1.0;
            IsRunning = false;
            cancelActivitiesView = false;
            HasBeenStopped = false;
            lapTimerEventClicked = false;
            TimeWhenLapButtonClicked = 0;
            TimeWhenForiegnButtonClicked = 0;
            TimeWhenStopButtonClicked = 0;
            LapTime = 0;
            CurrentTicks = 0;
            LastSuccesstulLapTime = 0;
            ActivitiesCounter = 0;
            CurrentCycle = 0;
            CurrentSequence = null;
            CurrentElementWithoutLapTimeName = null;
            CurrentWithoutLapTime = null;
        }

        void RatingSelectedEvent(object sender)
        {
            if (CurrentLapTime == null)
                SetUpCurrentLapTime();

            var button = sender as Custom.CustomButton;
            CurrentLapTime.Rating = button.Rating;

            if(!CurrentLapTime.IsForeignElement)
            {
                AllForiegnLapTimes = new List<LapTime>();
                TimeWhenForiegnButtonClicked = 0;
            }

            if (!CurrentWithoutLapTime.IsForeignElement)
            {
                LapTimesList.Add(CurrentLapTime);

                LapTimes = new ObservableCollection<LapTime>(LapTimesList
                    .OrderByDescending(x => x.Cycle));
                   //.ThenByDescending(x => x.Sequence));
                    
                if (ActivitiesCounter == ActivitiesCount)
                {
                    CurrentElementName = Activities.FirstOrDefault(x => x.Sequence == 1).Name;
                }
                else
                    CurrentElementName = Activities.FirstOrDefault(x => x.Sequence == ActivitiesCounter + 1).Name;

                AddCurrentWithoutLapTimeToList();

                ForeignElementCount = 0;

                Opacity = 1;
                RatingsVisible = false;
            }
            else
            {
                ProcessForeignElementWithRating(CurrentForeignElement.Rated, CurrentForeignElement.Name, button.Rating);

                if(LapButtonText != "   Lap   ")
                {
                    ActivitiesVisible = true;
                    RatingsVisible = false;
                    Opacity = 0.2;
                }
                else 
                {
                    CompleteCurrentForeignLapAndReinsatePausedLapToCurrentRunning();
                    ActivitiesVisible = false;
                    RatingsVisible = false;
                    Opacity = 1.0;
                }
            }
        }

        void ForeignElementSelectedEvent(object sender)
        {
            lapTimerEventClicked = false;

            Opacity = 1;

            var value = (int)sender;

            CurrentForeignElement = AllForeignElements.Where(x => x.Id == value).FirstOrDefault();
            SelectedForeignElements.Add(CurrentForeignElement);
            ForeignElementCount = SelectedForeignElements.Count;
            AddForeignElementWithoutLapTimeToList(CurrentForeignElement);


            ActivitiesVisible = false;
            RatingsVisible = false;
            Opacity = 1.0;
        }

        void ShowForeignElementsEvent()
        {
            if (CurrentWithoutLapTime == null) return;

            if(CurrentWithoutLapTime.IsForeignElement && CurrentWithoutLapTime.IsRated && CurrentWithoutLapTime.Rating == null)
            {
                RatingsVisible = true;
                Opacity = 0.2;
            }
            else if (CurrentWithoutLapTime.IsForeignElement && !CurrentWithoutLapTime.IsRated && CurrentWithoutLapTime.Rating == null)
            {
                CompleteCurrentForeignLapAndReinsatePausedLapToCurrentRunning();
                RatingsVisible = false;
                Opacity = 0.2;

                if(!lapTimerEventClicked)
                {
                    ActivitiesVisible = true;
                    Opacity = 0.2;
                }
                else 
                {
                    ActivitiesVisible = false;
                    Opacity = 1.0;}
                }
            else
            {

                Opacity = 0.2;
                ActivitiesVisible = true;
            }
        }

        void CloseForeignElementsEvent(object sender)
        {
            ForeignElementsVisible = false;
            Opacity = 1.0;
        }

        public void LapTimerEvent()
        {
            lapTimerEventClicked = true;
            LapButtonText = "   Lap   ";
            if (PausedLapTime == null)
            {
                if (ActivitiesVisible) return;

                SetUpButtonsAndTimeVariables();

                ForceRoundingToLapTime(true);

                Activity element;

                if (ActivitiesCounter == 0)
                    ActivitiesCounter = 1;
                else
                    ActivitiesCounter = ActivitiesCounter + 1;

                if (ActivitiesCounter > ActivitiesCount)
                {
                    ActivitiesCounter = 1;
                    CycleCount = CycleCount + 1;
                }

                element = Activities.FirstOrDefault(x => x.Sequence == ActivitiesCounter);

                LapTimesList.Remove(CurrentWithoutLapTime);

                SetUpCurrentLapTime();

                SelectedForeignElements = new List<Activity>();

                TimeWhenLapButtonClicked = RealTimeTicks;

                if(!CurrentLapTime.IsForeignElement)
                    LastSuccesstulLapTime = TimeWhenLapButtonClicked;

                CheckIfRatedStudy();
            }
            else
            {
                if (CurrentWithoutLapTime.IsForeignElement && !cancelActivitiesView)
                {
                    SetUpCurrentLapTime();
                    ShowForeignElementsEvent();
                    return;
                }

                CompleteCurrentForeignLapAndReinsatePausedLapToCurrentRunning();
            }

            cancelActivitiesView = false;

        }

        private void ProcessForeignElementWithRating(bool rated, string name, int? rating = null)
        {
            AddForeignLapTimetoListAsCompleted(rating);

            var currentForeign = new LapTime
            {
                Cycle = CycleCount,
                Element = name,
                TotalElapsedTime = "Running",
                IsForeignElement = true,
                IsRated = rated,
                StudyId = Utilities.StudyId
            };

            CurrentWithoutLapTime = currentForeign;

            LapTimesList.Add(CurrentWithoutLapTime);

            TimeWhenForiegnButtonClicked = RealTimeTicks;

            CurrentElementWithoutLapTimeName = currentForeign.Element;
            CurrentSequence = null;
            CurrentCycle = CycleCount;

            RemoveDuplicate();

            LapTimes = new ObservableCollection<LapTime>(LapTimesList
                .Where(x => x.TotalElapsedTime != "Running")
                .OrderByDescending(x => x.TotalElapsedTime));
        }

        private void CompleteCurrentForeignLapAndReinsatePausedLapToCurrentRunning()
        {
            AddForeignLapTimetoListAsCompleted();

            ReInstatePausedLapTimeToCurrentRunning();
        }

        private void ReInstatePausedLapTimeToCurrentRunning()
        {
            if (lapTimerEventClicked)
            {
                LapTimesList.Remove(PausedLapTime);

                CurrentWithoutLapTime = PausedLapTime;
                CurrentWithoutLapTime.TotalElapsedTime = "Running";

                LapTimesList.Add(CurrentWithoutLapTime);

                PausedLapTime = null;
            }

            CurrentElementWithoutLapTimeName = CurrentWithoutLapTime.Element;
            //CurrentSequence = CurrentWithoutLapTime.Sequence != null ? CurrentWithoutLapTime.Sequence : null;
            CurrentCycle = CycleCount;

            RemoveDuplicate();

            LapTimes = new ObservableCollection<LapTime>(LapTimesList
                .Where(x => x.TotalElapsedTime != "Running")
                .OrderByDescending(x => x.TotalElapsedTime));
                
        }

        private void AddForeignLapTimetoListAsCompleted(int? rating = null)
        {
            LapTimesList.Remove(CurrentWithoutLapTime);

            ForceRoundingToLapTime();

            SetUpCurrentLapTime();

            CurrentLapTime.Rating = rating;

            //if (CurrentLapTime.IsForeignElement)
                //CurrentLapTime.ElementColour = Color.Orange;

            LapTimesList.Add(CurrentLapTime);

            AllForiegnLapTimes.Add(CurrentLapTime);
        }

        private void RemoveDuplicate()
        {
            //** HACK
            var lap = LapTimesList.Find(x => x.IsForeignElement && x.IsRated
                && x.IndividualLapTimeFormatted != string.Empty && x.TotalElapsedTime != string.Empty
                && x.Rating == null);

            if (lap != null)
                LapTimesList.Remove(lap);
        }

        public void RunTimer()
        {

            TimeSpan TotalTime;
            TimeSpan TimeElement = new TimeSpan();
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 100), () =>
            {
                if (!IsRunning) return false;

                TotalTime = TotalTime + TimeElement.Add(new TimeSpan(0, 0, 0, 1));

                var timeElaspedSinceStart = DateTime.Now.TimeOfDay - StartTime;

                var realTicks = timeElaspedSinceStart.Ticks / 1000000;

                RealTimeTicks = TimeWhenStopButtonClicked + (double)realTicks / 600;

                StopWatchTime = RealTimeTicks.ToString("0.000");

                return IsRunning;
            });
        }

        private void AddForeignElementWithoutLapTimeToList(Activity foreign)
        {
            LapButtonText = "  Resume ";

            if (PausedLapTime == null)
            {
                LapTimesList.Remove(CurrentWithoutLapTime);
                PausedLapTime = CurrentWithoutLapTime;
                PausedLapTime.IndividualLapTimeFormatted = "Paused";
                PausedLapTime.TotalElapsedTimeDouble = RealTimeTicks;
                PausedLapTime.TotalElapsedTime =  RealTimeTicks.ToString("0.000");
                LapTimesList.Add(PausedLapTime);

                TimeWhenForiegnButtonClicked = RealTimeTicks;

                var currentForeignLap = new LapTime
                {
                    Cycle = CycleCount,
                    Element = foreign.Name,
                    TotalElapsedTime = "Running",
                    IsForeignElement = foreign.IsForeignElement,
                    IsRated = foreign.Rated,
                    StudyId = Utilities.StudyId
                };

                CurrentWithoutLapTime = currentForeignLap;

                LapTimesList.Add(CurrentWithoutLapTime);
            }
            else
            {
                var currentForeignLap = new LapTime
                {
                    Cycle = CycleCount,
                    Element = foreign.Name,
                    TotalElapsedTime = "Running",
                    IsForeignElement = foreign.IsForeignElement,
                    IsRated = foreign.Rated,
                    StudyId = Utilities.StudyId
                };

                CurrentWithoutLapTime = currentForeignLap;
                if (CurrentWithoutLapTime.IsRated)
                {

                    Opacity = 0.2;
                    RatingsVisible = true;
                }
            }

            CurrentElementWithoutLapTimeName = foreign.Name;
            CurrentSequence = null;
            CurrentCycle = CycleCount;

            RemoveDuplicate();

            LapTimes = new ObservableCollection<LapTime>(LapTimesList
                .Where(x => x.TotalElapsedTime != "Running")
                .OrderByDescending(x => x.TotalElapsedTime));
        }

        private void SetUpCurrentLapTime()
        {
            string lapTimeTimeFormatted = LapTime.ToString("0.000");

            CurrentLapTime = CurrentWithoutLapTime;
            CurrentLapTime.IndividualLapTime = LapTime;
            CurrentLapTime.IndividualLapTimeFormatted = lapTimeTimeFormatted;
            CurrentLapTime.TotalElapsedTimeDouble = RealTimeTicks;
            CurrentLapTime.TotalElapsedTime = RealTimeTicks.ToString("0.000");
            CurrentLapTime.ElementColour = Color.Gray;
            CurrentLapTime.ForeignElements = SelectedForeignElements;
        }

        private void AddCurrentWithoutLapTimeToList()
        {
            if (ActivitiesCounter == 0) ActivitiesCounter = 1;

            var element = Activities.FirstOrDefault(x => x.Sequence == ActivitiesCounter);

            if (ActivitiesCounter <= ActivitiesCount)
            {
                CurrentWithoutLapTime = new LapTime
                {
                    Cycle = CycleCount,
                    Element = element.Name,
                    TotalElapsedTime = "Running",
                    //Sequence = element.Sequence,
                    ElementColour = Color.Silver,
                    IsRated = element.Rated
                };
            }

            CurrentElementWithoutLapTimeName = element.Name;
            CurrentSequence = element.Sequence;
            CurrentCycle = CycleCount;

            LapTimesList.Add(CurrentWithoutLapTime);

            RemoveDuplicate();

            LapTimes = new ObservableCollection<LapTime>(LapTimesList
                .Where(x => x.TotalElapsedTime != "Running")
                .OrderByDescending(x => x.TotalElapsedTime));
        }

        private void SetUpButtonsAndTimeVariables()
        {
            IsStartEnabled = false;
            IsLapEnabled = true;
            IsStopEnabled = true;
            IsClearEnabled = false;

            var timeElaspedSinceStart = DateTime.Now.TimeOfDay - StartTime;

            var timeElaspedSinceStartDecimal = timeElaspedSinceStart.Ticks / 1000000;
            RealTimeTicks = TimeWhenStopButtonClicked + (double)timeElaspedSinceStartDecimal / 600;
            CurrentTimeFormatted = timeElaspedSinceStart.ToString();
            CurrentTimeFormattedDecimal = RealTimeTicks.ToString("0.000");
        }

        private void CheckIfRatedStudy()
        {
            if (!Utilities.RatedStudy)
            {
                LapTimesList.Add(CurrentLapTime);

                AddCurrentWithoutLapTimeToList();

                ForeignElementCount = 0;
            }
            else
            {
                Opacity = 0.2;
                RatingsVisible = true;
            }
        }

        private void ForceRoundingToLapTime(bool isLapTime = false)
        {
            if(!CurrentWithoutLapTime.IsForeignElement)
                LapTime = RealTimeTicks - TimeWhenLapButtonClicked;
            else
            {
                if(TimeWhenForiegnButtonClicked != 0)
                    LapTime = RealTimeTicks - TimeWhenForiegnButtonClicked;
                else
                    LapTime = RealTimeTicks - PausedLapTime.TotalElapsedTimeDouble;
            }


            if (isLapTime && !CurrentWithoutLapTime.IsForeignElement)
            {
                double foriegnLapTimesTotal = 0;
                foreach (var item in AllForiegnLapTimes)
                {
                    foriegnLapTimesTotal = foriegnLapTimesTotal + item.IndividualLapTime;
                }

                LapTime = LapTime - foriegnLapTimesTotal; //- LastSuccesstulLapTime;
            }

            double randomToForceRounding;

            Random r = new Random();
            int rInt = r.Next(0, 9);
            if (rInt > 0)
            {
                randomToForceRounding = (double)rInt / 10000;
                LapTime = LapTime + randomToForceRounding;
            }
        }

        static LapTime lapTimeSelected;
        public LapTime LapTimeSelected
        {
            get => lapTimeSelected;
            set
            {
                lapTimeSelected = value;
                OnPropertyChanged();
            }
        }

        static LapTime pausedLapTime;
        public LapTime PausedLapTime
        {
            get => pausedLapTime;
            set
            {
                pausedLapTime = value;
                OnPropertyChanged();
            }
        }

        static ObservableCollection<Activity> foreignElements;
        public ObservableCollection<Activity> ForeignElements
        {
            get => foreignElements;
            set
            {
                foreignElements = value;
                OnPropertyChanged();
            }
        }

        static string stopWatchTime = "0.000";
        public string StopWatchTime
        {
            get => stopWatchTime;
            set
            {
                stopWatchTime = value;
                OnPropertyChanged();
            }
        }

        static string startTimeFormatted;
        public string StartTimeFormatted
        {
            get => startTimeFormatted;
            set
            {
                startTimeFormatted = value;
                OnPropertyChanged();
            }
        }
        static string currentTimeFormatted;
        public string CurrentTimeFormatted
        {
            get => currentTimeFormatted;
            set
            {
                currentTimeFormatted = value;
                OnPropertyChanged();
            }
        }

        static string currentTimeFormattedDecimal;
        public string CurrentTimeFormattedDecimal
        {
            get => currentTimeFormattedDecimal;
            set
            {
                currentTimeFormattedDecimal = value;
                OnPropertyChanged();
            }
        }

        static double realTimeTicks;
        public double RealTimeTicks
        {
            get => realTimeTicks;
            set
            {
                realTimeTicks = value;
                OnPropertyChanged();
            }
        }

        static bool isStartEnabled;
        public bool IsStartEnabled
        {
            get => isStartEnabled;
            set
            {
                isStartEnabled = value;
                OnPropertyChanged();
            }
        }

        static bool isStopEnabled;
        public bool IsStopEnabled
        {
            get => isStopEnabled;
            set
            {
                isStopEnabled = value;
                OnPropertyChanged();
            }
        }

        static bool isLapEnabled;
        public bool IsLapEnabled
        {
            get => isLapEnabled;
            set
            {
                isLapEnabled = value;
                OnPropertyChanged();
            }
        }

        static bool isClearEnabled;
        public bool IsClearEnabled
        {
            get => isClearEnabled;
            set
            {
                isClearEnabled = value;
                OnPropertyChanged();
            }
        }

        static bool ratingsVisible;
        public bool RatingsVisible
        {
            get => ratingsVisible;
            set
            {
                ratingsVisible = value;
                OnPropertyChanged();
            }
        }

        static bool activitiesVisible;
        public bool ActivitiesVisible
        {
            get => activitiesVisible;
            set
            {
                activitiesVisible = value;
                OnPropertyChanged();
            }
        }

        static bool foreignElementsVisible;
        public bool ForeignElementsVisible
        {
            get => foreignElementsVisible;
            set
            {
                foreignElementsVisible = value;
                OnPropertyChanged();
            }
        }

        static Activity currentElement;
        public Activity CurrentElement
        {
            get => currentElement;
            set
            {
                currentElement = value;
                OnPropertyChanged();
            }
        }

        static string currentElementName;
        public string CurrentElementName
        {
            get => currentElementName;
            set
            {
                currentElementName = value;
                OnPropertyChanged();
            }
        }

        static string lapButtonText;
        public string LapButtonText
        {
            get => lapButtonText;
            set
            {
                lapButtonText = value;
                OnPropertyChanged();
            }
        }

        static string currentElementWithoutLapTimeName;
        public string CurrentElementWithoutLapTimeName
        {
            get => currentElementWithoutLapTimeName;
            set
            {
                currentElementWithoutLapTimeName = value;
                OnPropertyChanged();
            }
        }

        static int foreignElementCount;
        public int ForeignElementCount
        {
            get => foreignElementCount;
            set
            {
                foreignElementCount = value;
                OnPropertyChanged();
            }
        }

        static int? currentSequence;
        public int? CurrentSequence
        {
            get => currentSequence;
            set
            {
                currentSequence = value;
                OnPropertyChanged();
            }
        }

        static int currentCycle;
        public int CurrentCycle
        {
            get => currentCycle;
            set
            {
                currentCycle = value;
                OnPropertyChanged();
            }
        }

        static List<LapTime> LapTimesList = new List<LapTime>();

        static ObservableCollection<LapTime> lapTimes;
        public ObservableCollection<LapTime> LapTimes
        {
            get
            {
                return new ObservableCollection<LapTime>(LapTimesList
                .Where(x => x.TotalElapsedTime != "Running")
                .OrderByDescending(x => x.TotalElapsedTime));
            }
            set
            {
                lapTimes = value;
                OnPropertyChanged();
            }
        }
    }
}
