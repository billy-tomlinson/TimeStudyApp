using System;
using System.IO;
using CoreGraphics;
using Foundation;
using TimeStudy;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace TimeStudyApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            UINavigationBar.Appearance.TintColor = Color.White.ToUIColor();
            UINavigationBar.Appearance.BackIndicatorImage = new UIImage();
            UINavigationBar.Appearance.BackIndicatorTransitionMaskImage = new UIImage();


            UITabBar.Appearance.ShadowImage = new UIImage();
            UITabBar.Appearance.BackgroundImage = new UIImage();

            UITabBar.Appearance.SelectedImageTintColor = UIColor.Black;

            UITabBarItem.Appearance.SetTitleTextAttributes(
                new UITextAttributes()
                {
                    TextColor = UIColor.Black
                },
                UIControlState.Selected);
                
            string dbName = "TyeTimeStudy.db3";
            string folderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "..", "Library");
            //string folderPath = "/Users/billytomlinson";
            string dbPath = Path.Combine(folderPath, dbName);
            LoadApplication(new App(dbPath));

            return base.FinishedLaunching(app, options);
        }

        public static CGColor ToCGColor(Color color)
        {
            return new CGColor(CGColorSpace.CreateSrgb(), new nfloat[] { (float)color.R, (float)color.G, (float)color.B, (float)color.A });
        }
    }
}