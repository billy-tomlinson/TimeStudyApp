using LocalDataAccess.iOS;
using SQLite;
using System;
using System.IO;
[assembly: Xamarin.Forms.Dependency(typeof(IOSSQLite))]
namespace LocalDataAccess.iOS
{
    public class IOSSQLite
    {
        public SQLiteConnection GetConnection()
        {
            var dbName = "ActivitySample.db";
            string personalFolder =
              System.Environment.
              GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryFolder =
              Path.Combine(personalFolder, "..", "Library");
            var path = Path.Combine(libraryFolder, dbName);
            return new SQLiteConnection(path);
        }
    }
}