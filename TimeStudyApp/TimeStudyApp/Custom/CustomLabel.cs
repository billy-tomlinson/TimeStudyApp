using Xamarin.Forms;

namespace TimeStudy.Custom
{
    public class CustomLabel : Label
    {
        public static readonly BindableProperty ActivityIdProperty =  
           BindableProperty.Create(  
               nameof(ActivityId),  
               typeof(int),  
               typeof(CustomLabel),  
               0);  
  
        public int ActivityId  
        {  
            get { return (int)GetValue(ActivityIdProperty); }  
            set { SetValue(ActivityIdProperty, value); }  
        } 


        public static readonly BindableProperty OperatorIdProperty =  
           BindableProperty.Create(  
               nameof(OperatorId),  
               typeof(int),  
               typeof(CustomLabel),  
               0);  
  
        public int OperatorId  
        {  
            get { return (int)GetValue(OperatorIdProperty); }  
            set { SetValue(OperatorIdProperty, value); }  
        } 

        public static readonly BindableProperty RatingProperty =  
           BindableProperty.Create(
               nameof(Rating),  
               typeof(int),  
                typeof(CustomLabel),  
               0);  
  
        public int Rating  
        {  
            get { return (int)GetValue(RatingProperty); }  
            set { SetValue(RatingProperty, value); }  
        } 
    }
}
