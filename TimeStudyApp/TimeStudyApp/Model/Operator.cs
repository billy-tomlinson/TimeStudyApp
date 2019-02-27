using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;
using TimeStudy.Services;
using Xamarin.Forms;

namespace TimeStudy.Model
{
    [Table("Operator")]    
    public class Operator : BaseEntity
    {
        public Operator()
        {
            StudyId = Utilities.StudyId;
        }

        [ForeignKey(typeof(ActivitySampleStudy))]
        public int StudyId { get; set; }

        public string Name { get; set; }

        public DateTime Date => DateTime.Now;

        public bool IsEnabled { get; set; } = true;

        public string Observed { get; set; }

        public bool Isvisible { get; set; }

        public double Opacity { get; set; } = 1;

        [OneToMany]
        public List<Observation> Observations { get; set; }

        public string ObservedColour { get; set; } = Utilities.ValidColour;

        [Ignore]
        public Color ConvertedColour => Color.FromHex(ObservedColour);

        public string DeleteIcon { get; set; } = Utilities.DeleteImage;

        public string SettingsIcon { get; set; } = String.Empty;
    }
}
