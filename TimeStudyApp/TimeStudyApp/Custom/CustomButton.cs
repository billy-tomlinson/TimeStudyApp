using System.ComponentModel;
using Xamarin.Forms;

namespace TimeStudy.Custom
{
    [DesignTimeVisible(true)]
    public class CustomButton: Button  
    {  
         public static readonly BindableProperty ActivityIdProperty =  
           BindableProperty.Create(  
               nameof(ActivityId),  
               typeof(int),  
               typeof(CustomButton),  
               0);  
  
        public int ActivityId  
        {  
            get { return (int)GetValue(ActivityIdProperty); }  
            set { SetValue(ActivityIdProperty, value); }  
        } 

        public static readonly BindableProperty RatingProperty =  
           BindableProperty.Create(
               nameof(Rating),  
               typeof(int),  
               typeof(CustomButton),  
               0);  
  
        public int Rating  
        {  
            get { return (int)GetValue(RatingProperty); }  
            set { SetValue(RatingProperty, value); }  
        }  
    }  
}
