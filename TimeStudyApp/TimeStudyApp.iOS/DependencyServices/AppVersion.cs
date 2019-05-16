using Foundation;
using TimeStudyApp.iOS.DependencyServices;
using TimeStudyApp.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppVersion))]
namespace TimeStudyApp.iOS.DependencyServices
{
    public class AppVersion : IAppVersion
    {
        public string GetVersion()
        {
            return NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
        }
        public string GetBuild()
        {
            return NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
        }
    }
}
