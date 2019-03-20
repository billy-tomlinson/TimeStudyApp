using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TimeStudy.Model;
using TimeStudy.Services;
using TimeStudyApp.Model;
using TimeStudyApp.Services.StateMachine;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class TimeStudyUnsequencedViewModel : BaseViewModel
    {

        public Command StartTimer { get; set; }
        public Command LapTimer { get; set; }
        public Command StopTimer { get; set; }
        public Command ClearLaps { get; set; }
        public Command RatingSelected { get; set; }
        public Command ItemClickedCommand { get; set; }
        public Command RunningItemClickedCommand { get; set; }
        public Command ShowForeignElements { get; set; }
        public Command ShowNonForeignElements { get; set; }
        public Command CloseForeignElements { get; set; }
        public Command ResumePased { get; set; }
        public Command ElementSelected { get; set; }
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


        public BaseState ApplicationState { get; set; }
        public StateFactory ApplicationStateFactory { get; set; }

        public State CurrentApplicationState { get; set; }
        public ApplicationState StateService { get; set; }

        public List<Activity> SelectedForeignElements;

        public List<LapTime> AllForiegnLapTimes = new List<LapTime>();

        public int ActivitiesCount;
        public int ActivitiesCounter;
        public int CycleCount;

        public TimeStudyUnsequencedViewModel()
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
            ShowNonForeignElements = new Command(ShowNonForeignElementsEvent);
            CloseForeignElements = new Command(CloseForeignElementsEvent);
            ResumePased = new Command(ResumePausedEvent);
            ElementSelected = new Command(ElementsSelectedEvent);
            ItemClickedCommand = new Command(ShowForeignElementsEvent);
            CloseActivitiesView = new Command(CloseActivitiesViewEvent);

            ApplicationStateFactory = new StateFactory(this);

            StateService = new ApplicationState();
            var state = StateService.GetApplicationState();

            if (state != null)
                CurrentApplicationState = new State()
                {
                    Id = state.Id,
                    CurrentState = Status.NoElementRunning
                };
            else
            {
                CurrentApplicationState = new State()
                {
                    CurrentState = Status.NoElementRunning
                };
            }
            StateService.SaveApplicationState(CurrentApplicationState);

            LapTimes = new ObservableCollection<LapTime>();

            SelectedForeignElements = new List<Activity>();

            Activities = Get_All_ValueAdded_Rated_Enabled_Activities_WithChildren();

            CollectionOfElements = Get_All_Enabled_Activities();

            GroupElementsForActivitiesView();

            IsPageVisible = IsStudyValid();

            LapTime = 0;
            CurrentTicks = 0;
            StopWatchTime = "0.000";
            IsRunning = false;
            IsStartEnabled = true;
            IsLapEnabled = true;
            IsStopEnabled = false;
            IsClearEnabled = false;
            IsForeignEnabled = false;
            ActivitiesCount = Activities.Count;
            CycleCount = 1;

            LapButtonText = "   Start   ";
        }

        public void GroupElementsForActivitiesView()
        {
            IEnumerable<Activity> obsCollection = CollectionOfElements;

            var list1 = new List<Activity>(obsCollection);

            foreach (var activity in list1)
            {
                activity.Colour = Color.FromHex(activity.ItemColour);
            };

            CollectionOfElements = ConvertListToObservable(list1);

            GroupActivities = Utilities.BuildGroupOfActivities(CollectionOfElements);
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
            ActivitiesVisible = false;
            lapTimerEventClicked = false;
        }

        public void ResumePausedEvent()
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

            RunTimer();
            if (!HasBeenStopped)
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

            CurrentApplicationState.CurrentState = Status.NoElementRunning;
            StateService.SaveApplicationState(CurrentApplicationState);
        }

        public Custom.CustomButton RatingButton;

        void RatingSelectedEvent(object sender)
        {
            RatingButton = sender as Custom.CustomButton;
            var current = Get_Running_Unrated_LapTime(); //Get_Running_LapTime();
            if(current != null) 
            {
                current.Rating = RatingButton.Rating;

                Utilities.LastRatedLapTimeId = current.Id;

                LapTimeRepo.SaveItem(current);
            }

            ApplicationState = ApplicationStateFactory.GetCurrentState();
            ApplicationState.RatingSelectedEvent();
        }

        void ElementsSelectedEvent(object sender)
        {

            var value = (int)sender;

            Utilities.CurrentSelectedElementId = value;

            ApplicationState = ApplicationStateFactory.GetCurrentState();
            ApplicationState.ElementSelectedEvent();

            lapTimerEventClicked = false;

            Opacity = 1;

            var current = CollectionOfElements.FirstOrDefault(x => x.Id == Utilities.CurrentSelectedElementId);
            ApplicationState = ApplicationStateFactory.GetCurrentState();
            ApplicationState.AddElementWithoutLapTimeToList();

            ActivitiesVisible = false;
            RatingsVisible = false;
            Opacity = 1.0;

            IsRunning = true;
        }

        void ShowForeignElementsEvent()
        {
            CollectionOfElements = Get_All_Foreign_Enabled_Activities_WithChildren();
            GroupElementsForActivitiesView();

            ApplicationState = ApplicationStateFactory.GetCurrentState();
            ApplicationState.ShowForeignElements();
        }


        void ShowNonForeignElementsEvent()
        {
            CollectionOfElements = Get_All_NonForeign_Enabled_Activities_WithChildren();
            GroupElementsForActivitiesView();

            ApplicationState = ApplicationStateFactory.GetCurrentState();
            ApplicationState.ShowNonForeignElements(); ;
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

            var pausedLap = Get_Paused_LapTime();
            if (pausedLap == null)
            {
                if (ActivitiesVisible) return;

                SetUpButtonsAndTimeVariables();

                ForceRoundingToLapTime(true);

                SetUpCurrentLapTime();

                CheckIfRatedStudy();

            }
            else
            {
                var current = Get_Running_LapTime();
                if (current.IsForeignElement && !cancelActivitiesView)
                {
                    SetUpCurrentLapTime();
                    ShowForeignElementsEvent();
                    return;
                }

                CompleteCurrentForeignLapAndReinsatePausedLapToCurrentRunning();
            }

            cancelActivitiesView = false;

        }

        public void ProcessForeignElementWithRating(bool rated, string name, bool isForeign, int? rating = null)
        {
            AddForeignLapTimetoListAsCompleted(rating);

            var currentForeign = new LapTime
            {
                Cycle = CycleCount,
                Element = name,
                TotalElapsedTime = "Running",
                IsForeignElement = isForeign,
                StudyId = Utilities.StudyId
            };

            TimeWhenForiegnButtonClicked = RealTimeTicks;

            CurrentElementWithoutLapTimeName = currentForeign.Element;
            CurrentSequence = null;
            CurrentCycle = CycleCount;

            LapTimes = Get_All_LapTimes_Not_Running();
        }

        public void CompleteCurrentForeignLapAndReinsatePausedLapToCurrentRunning()
        {
            AddForeignLapTimetoListAsCompleted();

            ReInstatePausedLapTimeToCurrentRunning();
        }

        private void ReInstatePausedLapTimeToCurrentRunning()
        {
            LapTime current;

            current = Get_Paused_LapTime();

            current.TotalElapsedTime = "Running";

            LapTimeRepo.SaveItem(current);

            CurrentCycle = CycleCount;

            CurrentElementWithoutLapTimeName = current.Element;
            CurrentSequence = null;
            CurrentCycle = CycleCount;

            LapTimes = Get_All_LapTimes_Not_Running();
        }

        private void AddForeignLapTimetoListAsCompleted(int? rating = null)
        {

            ForceRoundingToLapTime();

            SetUpCurrentLapTime();

            var current = Get_Current_LapTime(Utilities.LastRatedLapTimeId);
            current.Rating = rating;

            LapTimeRepo.SaveItem(current);

            AllForiegnLapTimes.Add(current);
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

        private void AddElementWithoutLapTimeToList(Activity element)
        {
            LapButtonText = "  Lap ";

            LapTime pausedLapTime = Get_Paused_LapTime();

            if (pausedLapTime == null)
            {
                if (element.IsForeignElement)
                {
                    pausedLapTime = Get_Running_LapTime();
                    pausedLapTime.IndividualLapTimeFormatted = "Paused";
                    pausedLapTime.TotalElapsedTimeDouble = RealTimeTicks;
                    pausedLapTime.TotalElapsedTime = RealTimeTicks.ToString("0.000");

                    LapTimeRepo.SaveItem(pausedLapTime);

                    TimeWhenForiegnButtonClicked = RealTimeTicks;

                    var currentForeignLap = new LapTime
                    {
                        Cycle = CycleCount,
                        Element = element.Name,
                        TotalElapsedTime = "Running",
                        IsForeignElement = element.IsForeignElement,
                        StudyId = Utilities.StudyId
                    };

                    LapTimeRepo.SaveItem(currentForeignLap);

                }
            }
            else
            {
                var currentForeignLap = new LapTime
                {
                    Cycle = CycleCount,
                    Element = element.Name,
                    TotalElapsedTime = "Running",
                    IsForeignElement = element.IsForeignElement,
                    StudyId = Utilities.StudyId
                };

                LapTimeRepo.SaveItem(currentForeignLap);

                Opacity = 0.2;
                RatingsVisible = true;
            }

            CurrentElementWithoutLapTimeName = element.Name;
            CurrentSequence = null;
            CurrentCycle = CycleCount;

            LapTimes = Get_All_LapTimes_Not_Running();
        }

        public void SetUpCurrentLapTime()
        {
            string lapTimeTimeFormatted = LapTime.ToString("0.000");

            var currentLapTime = Get_Running_LapTime();
            if(currentLapTime == null)
                currentLapTime = Get_Current_LapTime(Utilities.LastRatedLapTimeId);

            currentLapTime.IndividualLapTime = LapTime;
            currentLapTime.IndividualLapTimeFormatted = lapTimeTimeFormatted;
            currentLapTime.TotalElapsedTimeDouble = RealTimeTicks;
            currentLapTime.TotalElapsedTime = RealTimeTicks.ToString("0.000");
            currentLapTime.ElementColour = Color.Gray;
            currentLapTime.ForeignElements = SelectedForeignElements;
            LapTimeRepo.SaveItem(currentLapTime);
        }

        public void AddCurrentWithoutLapTimeToList()
        {
            if (ActivitiesCounter == 0) ActivitiesCounter = 1;

            var element = Activities.FirstOrDefault(x => x.Sequence == ActivitiesCounter);

            if (ActivitiesCounter <= ActivitiesCount)
            {
                var currentWithoutLapTime = new LapTime
                {
                    Cycle = CycleCount,
                    Element = element.Name,
                    TotalElapsedTime = "Running",
                    ElementColour = Color.Silver,
                    StudyId = Utilities.StudyId
                };

                LapTimeRepo.SaveItem(currentWithoutLapTime);
            }

            CurrentElementWithoutLapTimeName = element.Name;
            CurrentSequence = element.Sequence;
            CurrentCycle = CycleCount;

            LapTimes = Get_All_LapTimes_Not_Running();
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
            LapTime = RealTimeTicks - TimeWhenLapButtonClicked;

            double foriegnLapTimesTotal = 0;
            foreach (var item in AllForiegnLapTimes)
            {
                foriegnLapTimesTotal = foriegnLapTimesTotal + item.IndividualLapTime;
            }

            LapTime = LapTime - foriegnLapTimesTotal; 

            double randomToForceRounding;

            Random r = new Random();
            int rInt = r.Next(0, 9);
            if (rInt > 0)
            {
                randomToForceRounding = (double)rInt / 10000;
                LapTime = LapTime + randomToForceRounding;
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

        static bool isForeignEnabled;
        public bool IsForeignEnabled
        {
            get => isForeignEnabled;
            set
            {
                isForeignEnabled = value;
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

        static ObservableCollection<Activity> collectionOfElements;
        public ObservableCollection<Activity> CollectionOfElements
        {
            get => collectionOfElements;
            set
            {
                collectionOfElements = value;
                OnPropertyChanged();
            }
        }

        static ObservableCollection<LapTime> lapTimes;
        public ObservableCollection<LapTime> LapTimes
        {
            get
            {
                return lapTimes;
            }
            set
            {
                lapTimes = value;
                OnPropertyChanged();
            }
        }
    }
}
