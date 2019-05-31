using System;
using SQLite;
using SQLiteNetExtensions.Attributes;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.Model
{
    [Table("WorkElement")]
    public class WorkElement : BaseEntity
    {
        public WorkElement()
        {
            Colour = Utilities.UnClicked;
            StudyId = Utilities.StudyId;
            ActivityName = new WorkElementName();
        }

        [ForeignKey(typeof(WorkElementName))] 
        public int ActivityNameId { get; set; }

        [OneToOne]
        public WorkElementName ActivityName { get; set; }

        [ForeignKey(typeof(RatedTimeStudy))]
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

        public string ObservedColour { get; set; } = Utilities.ValidColour;

        [Ignore]
        public Color ConvertedColour => Color.FromHex(ObservedColour);

        public string DeleteIcon { get; set; } = Utilities.DeleteImage;

        public string SettingsIcon { get; set; } = Utilities.CommentsImage;

    }
}