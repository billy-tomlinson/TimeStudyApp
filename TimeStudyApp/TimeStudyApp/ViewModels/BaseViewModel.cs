using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TimeStudy.Model;
using TimeStudy.Services;
using TimeStudyApp.Model;
using Xamarin.Forms;

namespace TimeStudy.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Command CloseView { get; set; }
        public Command Override { get; set; }

        private readonly string conn;

        public BaseViewModel(string conn = null)
        {
            this.conn = conn;
            SubmitDetails = new Command(SubmitDetailsAndNavigate);
            CloseView = new Command(CloseValidationView);
            EnsureTableCreation();
            InvalidText = "Please create a new study or select an existing one.";
            IsPageVisible = (Utilities.StudyId > 0);
        }


        public Command SubmitDetails { get; set; }


        public IBaseRepository<StudyHistoryVersion> StudyHistoryVersionRepo => new BaseRepository<StudyHistoryVersion>(conn);

        public IBaseRepository<LapTime> LapTimeRepo => new BaseRepository<LapTime>(conn);

        public IBaseRepository<LapTimeHistoric> LapTimeHistoricRepo => new BaseRepository<LapTimeHistoric>(conn);

        public IBaseRepository<State> StateRepo => new BaseRepository<State>(conn);

        public IBaseRepository<WorkElement> ActivityRepo => new BaseRepository<WorkElement>(conn);

        public IBaseRepository<WorkElementName> ActivityNameRepo => new BaseRepository<WorkElementName>(conn);

        public IBaseRepository<RatedTimeStudy> TimeStudyRepo => new BaseRepository<RatedTimeStudy>(conn);

        static ObservableCollection<WorkElement> activities;
        public ObservableCollection<WorkElement> Activities
        {
            get => activities;
            set
            {
                activities = value;
                OnPropertyChanged();
            }
        }

        static bool hasElements;
        public bool HasElements
        {
            get => hasElements;
            set
            {
                hasElements = value;
                OnPropertyChanged();
            }
        }

        static string currentTime;
        public string CurrentTime
        {
            get => currentTime;
            set
            {
                currentTime = value;
                OnPropertyChanged();
            }
        }

        static string timeOfNextObservation;
        public string TimeOfNextObservation
        {
            get => timeOfNextObservation;
            set
            {
                timeOfNextObservation = value;
                OnPropertyChanged();
            }
        }

        static ObservableCollection<MultipleActivities> _groupActivities;
        public ObservableCollection<MultipleActivities> GroupActivities
        {
            get => _groupActivities;
            set
            {
                _groupActivities = value;
                OnPropertyChanged();
            }
        }

        bool isInvalid = false;
        public bool IsInvalid
        {
            get { return isInvalid; }
            set
            {
                isInvalid = value;
                OnPropertyChanged();
            }
        }

        bool finishStudyFromActivitiesClicked;
        public bool FinishStudyFromActivitiesClicked
        {
            get { return finishStudyFromActivitiesClicked; }
            set
            {
                finishStudyFromActivitiesClicked = value;
                OnPropertyChanged();
            }
        }


        double opacity = 1;
        public double Opacity
        {
            get { return opacity; }
            set
            {
                opacity = value;
                OnPropertyChanged();
            }
        }

        string invalidText;
        public string InvalidText
        {
            get { return invalidText; }
            set
            {
                invalidText = value;
                OnPropertyChanged();
            }
        }

        int studyNumber;
        public int StudyNumber
        {
            get { return Utilities.StudyId; }
            set
            {
                studyNumber = value;
                OnPropertyChanged();
            }
        }

        int closeColumnSpan = 2;
        public int CloseColumnSpan
        {
            get { return closeColumnSpan; }
            set
            {
                closeColumnSpan = value;
                OnPropertyChanged();
            }
        }

        int totalObservationsRequired;
        public int TotalObservationsRequired
        {
            get { return totalObservationsRequired; }
            set
            {
                totalObservationsRequired = value;
                OnPropertyChanged();
            }
        }


        int totalObservationsTaken;
        public int TotalObservationsTaken
        {
            get { return totalObservationsTaken; }
            set
            {
                totalObservationsTaken = value;
                OnPropertyChanged();
            }
        }

        string totalOperatorPercentage;
        public string TotalOperatorPercentage
        {
            get { return totalOperatorPercentage; }
            set
            {
                totalOperatorPercentage = value;
                OnPropertyChanged();
            }
        }

        bool isPageVisible = false;
        public bool IsPageVisible
        {
            get { return isPageVisible; }
            set
            {
                isPageVisible = value;
                IsPageUnavailableVisible = !value;
                OnPropertyChanged();
            }
        }

        bool isOverrideVisible = false;
        public bool IsOverrideVisible
        {
            get { return isOverrideVisible; }
            set
            {
                isOverrideVisible = value;
                //IsPageUnavailableVisible = !value;
                OnPropertyChanged();
            }
        }


        bool showOkCancel = false;
        public bool ShowOkCancel
        {
            get { return showOkCancel; }
            set
            {
                showOkCancel = value;
                OnPropertyChanged();
            }
        }

        bool showClose = false;
        public bool ShowClose
        {
            get { return showClose; }
            set
            {
                showClose = value;
                OnPropertyChanged();
            }
        }


        int isItemEnabled;
        public int IsItemEnabled
        {
            get { return isItemEnabled; }
            set
            {
                isItemEnabled = value;
                OnPropertyChanged();
            }
        }

        bool isPageUnavailableVisible = false;
        public bool IsPageUnavailableVisible
        {
            get { return isPageUnavailableVisible; }
            set
            {
                isPageUnavailableVisible = value;
                OnPropertyChanged();
            }
        }

        private string validationText;
        public string ValidationText
        {
            get => validationText;
            set
            {
                validationText = value;
                OnPropertyChanged();
            }
        }

        static string alarmStatus;
        public string AlarmStatus
        {
            get => alarmStatus;
            set
            {
                alarmStatus = value;
                OnPropertyChanged();
            }
        }

        static string intervalMinutes;
        public string IntervalMinutes
        {
            get => intervalMinutes;
            set
            {
                intervalMinutes = value;
                OnPropertyChanged();
            }
        }

        private bool busy = false;
        public bool IsBusy
        {
            get { return busy; }
            set
            {
                if (busy == value)
                    return;

                busy = value;
                OnPropertyChanged();
            }
        }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        static bool percentagesVisible;
        public bool PercentagesVisible
        {
            get => percentagesVisible;
            set
            {
                percentagesVisible = value;
                OnPropertyChanged();
            }
        }

        static bool isPageEnabled;
        public bool IsPageEnabled
        {
            get => isPageEnabled;
            set
            {
                isPageEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool StudyInProcess
        {
            get => Get_LapTimes_By_StudyId().Count > 0;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void SubmitDetailsAndNavigate() { }

        public ObservableCollection<WorkElement> Get_Rated_Enabled_Activities()
        {
            return new ObservableCollection<WorkElement>(ActivityRepo.GetAllWithChildren()
                                         .Where(x => x.IsEnabled && x.Rated && x.StudyId == Utilities.StudyId));
        }

        public ObservableCollection<WorkElement> Get_All_NonValueAdded_Enabled_Activities()
        {
            return new ObservableCollection<WorkElement>(ActivityRepo.GetAllWithChildren()
                                         .Where(x => !x.IsValueAdded && x.StudyId == Utilities.StudyId));
        }


        public ObservableCollection<WorkElement> Get_All_NonValueAdded_Enabled_Unrated_Activities()
        {
            return new ObservableCollection<WorkElement>(ActivityRepo.GetAllWithChildren()
                                         .Where(x => x.IsEnabled && !x.IsValueAdded && !x.Rated && x.StudyId == Utilities.StudyId));
        }


        public ObservableCollection<WorkElement> Get_All_NonValueAdded_Enabled_Rated_Activities()
        {
            return new ObservableCollection<WorkElement>(ActivityRepo.GetAllWithChildren()
                                         .Where(x => x.IsEnabled && !x.IsValueAdded && x.Rated && x.StudyId == Utilities.StudyId));
        }

        public ObservableCollection<WorkElement> Get_All_Enabled_Activities()
        {
            return new ObservableCollection<WorkElement>(ActivityRepo.GetAllWithChildren()
                                         .Where(x => x.IsEnabled && x.StudyId == Utilities.StudyId));
        }

        public List<LapTime> Get_LapTimes_By_StudyId()
        {
            return LapTimeRepo.GetItems()
                                .Where(x => x.StudyId == Utilities.StudyId && x.Version == Utilities.StudyVersion).ToList();
                               
        }

        public ObservableCollection<WorkElement> Get_All_Enabled_Activities_WithChildren()
        {
            return new ObservableCollection<WorkElement>(ActivityRepo.GetAllWithChildren()
                                        .Where(x => x.IsEnabled && x.StudyId == Utilities.StudyId));
        }

        public ObservableCollection<WorkElement> Get_All_ValueAdded_Rated_Enabled_Activities_WithChildren()
        {
            return new ObservableCollection<WorkElement>(ActivityRepo.GetAllWithChildren()
                .Where(x => x.IsValueAdded && x.Rated && x.StudyId == Utilities.StudyId));
        }

        public ObservableCollection<LapTime> Get_All_LapTimes_Not_Running()
        {
            var list = LapTimeRepo.GetItems().Where(x =>x.StudyId == Utilities.StudyId
                && x.Version == Utilities.StudyVersion);
            return new ObservableCollection<LapTime>(list
                .Where(x => x.Status == RunningStatus.Completed || x.Status == RunningStatus.Paused )
                .OrderByDescending(x => x.TotalElapsedTimeDouble));
        }

        public LapTime Get_Current_LapTime(int lapId)
        {
            return LapTimeRepo.GetAllWithChildren()
                .FirstOrDefault(x => x.Id == lapId
                && x.StudyId == Utilities.StudyId && x.Version == Utilities.StudyVersion);
        }

        public LapTime Get_Last_NonForeign_LapTime()
        {
            return LapTimeRepo.GetAllWithChildren()
                .OrderByDescending(x => x.Id)
                .FirstOrDefault(x => x.StudyId == Utilities.StudyId && x.Version == Utilities.StudyVersion && !x.IsForeignElement);
        }

        public LapTime Get_Current_Foreign_LapTime()
        {
            return LapTimeRepo.GetAllWithChildren()
                .OrderByDescending(x => x.Id)
                .FirstOrDefault(x => x.StudyId == Utilities.StudyId && x.Version == Utilities.StudyVersion);

        }

        public LapTime Get_Running_LapTime()
        {
            return LapTimeRepo.GetAllWithChildren()
                .FirstOrDefault(x => x.Status == RunningStatus.Running
                && x.StudyId == Utilities.StudyId && x.Version == Utilities.StudyVersion);
        }

        public LapTime Get_Running_LapTime_By_Id()
        {
            return LapTimeRepo.GetAllWithChildren()
                .FirstOrDefault(x => x.Id == Utilities.CurrentRunningElementId
                && x.StudyId == Utilities.StudyId && x.Version == Utilities.StudyVersion);
        }

        public LapTime Get_Running_Unrated_LapTime()
        {
            return LapTimeRepo.GetAllWithChildren()
                .FirstOrDefault(x => x.Status != RunningStatus.Running && x.Status != RunningStatus.Paused && x.Rating == null
                && x.StudyId == Utilities.StudyId && x.Version == Utilities.StudyVersion);
        }

        public LapTime Get_Last_Recorded_LapTime()
        {
            return LapTimeRepo.GetAllWithChildren()
                .OrderByDescending(x => x.Id)
                .FirstOrDefault(x => x.Status != RunningStatus.Running && x.Status != RunningStatus.Paused && x.Rating != null
                && x.StudyId == Utilities.StudyId && x.Version == Utilities.StudyVersion);
        }

        public LapTime Get_Paused_LapTime()
        {
            return LapTimeRepo.GetAllWithChildren()
                .FirstOrDefault(x => x.Status == RunningStatus.Paused
                && x.StudyId == Utilities.StudyId  && x.Version == Utilities.StudyVersion);
        }

        public int Get_Last_Study_Version()
        {
            var lastVersion = LapTimeRepo.GetItems().ToList();
            if (lastVersion.Count == 0)
                return 0;

            return LapTimeRepo.GetItems().Max(x => x.Version);
        }


        public int Copy_LapTimes_Into_LapTimesHistoric()
        {
            var lastVersion = LapTimeRepo.GetItems().ToList();
            if (lastVersion.Count == 0)
                return 0;

            return LapTimeRepo.GetItems().Max(x => x.Version);
        }

        public ObservableCollection<WorkElementName> Get_All_ActivityNames()
        {
            return new ObservableCollection<WorkElementName>(ActivityNameRepo.GetItems());

        }

        public ObservableCollection<WorkElement> ConvertListToObservable(List<WorkElement> list1)
        {
            return new ObservableCollection<WorkElement>(list1.OrderBy(x => x.Id).Where(x => x.IsEnabled));
        }


        public State GetApplicationState()
        {
            return StateRepo.GetItems().FirstOrDefault();
        }


        public int SaveApplicationState(State state)
        {
            return StateRepo.SaveItem(state);
        }

        public int SaveActivityDetails(WorkElement activity)
        {
            ActivityNameRepo.SaveItem(activity.ActivityName);
            var returnId = ActivityRepo.SaveItem(activity);
            ActivityRepo.UpdateWithChildren(activity);
            return returnId;
        }

        private void EnsureTableCreation()
        {
            StateRepo.CreateTable();
            LapTimeRepo.CreateTable();
            ActivityRepo.CreateTable();
            ActivityNameRepo.CreateTable();
            TimeStudyRepo.CreateTable();
            StudyHistoryVersionRepo.CreateTable();
            LapTimeHistoricRepo.CreateTable();
        }

        public void CloseValidationView()
        {
            Opacity = 1;
            IsInvalid = false;
            IsPageEnabled = true;
        }
    }
}
