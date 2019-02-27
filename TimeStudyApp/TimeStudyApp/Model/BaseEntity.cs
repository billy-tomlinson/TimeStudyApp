using System;
using SQLite;

namespace TimeStudy.Model
{
    public class BaseEntity : IDisposable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}


