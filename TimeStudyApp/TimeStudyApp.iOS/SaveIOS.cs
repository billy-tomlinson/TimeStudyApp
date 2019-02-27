using System;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(SaveIOS))]

class SaveIOS: ISave
    {
        //Method to save document as a file and view the saved document
        public async Task<string> SaveSpreadSheet(string filename, string contentType, MemoryStream stream)
        {
            //Get the root path in iOS device.
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //string filePath = Path.Combine("/Users/billytomlinson", filename);
            string filePath = Path.Combine(path, filename);

            //Create a file and write the stream into it.
            FileStream fileStream = File.Open(filePath, FileMode.Create);
            stream.Position = 0;
            stream.CopyTo(fileStream);
            fileStream.Flush();
            fileStream.Close();

            return path;

            //Invoke the saved document for viewing
            //UIViewController currentController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            //while (currentController.PresentedViewController != null)
            //    currentController = currentController.PresentedViewController;
            //UIView currentView = currentController.View;

            //QLPreviewController qlPreview = new QLPreviewController();
            //QLPreviewItem item = new QLPreviewItemBundle(filename, filePath);
            //qlPreview.DataSource = new PreviewControllerDS(item);

            //currentController.PresentViewController(qlPreview, true, null);

        }
    }
