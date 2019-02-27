using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.Model
{
    [Table("Activity")]
    public class Activity : BaseEntity
    {
        public Activity()
        {
            Colour = Utilities.UnClicked;
            Activities = new List<Activity>();
            StudyId = Utilities.StudyId;
            ActivityName = new ActivityName();
        }

        [ForeignKey(typeof(ActivityName))] 
        public int ActivityNameId { get; set; }

        [OneToOne]
        public ActivityName ActivityName { get; set; }

        [ForeignKey(typeof(ActivitySampleStudy))]
        public int StudyId { get; set; }

        public string Comment { get; set; }

        public bool IsEnabled { get; set; }

        public bool Rated { get; set; }

        public int Sequence { get; set; }

        public bool IsValueAdded { get; set; } = true;

        public string ItemColour { get; set; } = Utilities.ValueAddedColour;

        public DateTime Date => DateTime.Now;

        public double Opacity { get; set; } = 1;

        [Ignore]
        public string Name => ActivityName.Name;

        [Ignore]
        public Color Colour { get; set; } 

        [Ignore]
        public bool Selected { get; set; }

        [ManyToMany(typeof(MergedActivities), "ActivityId", "MergedActivities",
        CascadeOperations = CascadeOperation.All)]
        public List<Activity> Activities { get; set; }

        [ManyToMany(typeof(MergedActivities), "MergedActivityId", "Activities",
        CascadeOperations = CascadeOperation.All, ReadOnly = true)]
        public List<Activity> MergedActivities { get; set; }

        public string ObservedColour { get; set; } = Utilities.ValidColour;

        [Ignore]
        public Color ConvertedColour => Color.FromHex(ObservedColour);

        public string DeleteIcon { get; set; } = Utilities.DeleteImage;

        public string SettingsIcon { get; set; } = Utilities.CommentsImage;

    }
}