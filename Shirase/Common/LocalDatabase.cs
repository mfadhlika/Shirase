using LiteDB;
using Shirase.Pushover.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Shirase.Common
{
    public class LocalDatabase
    {
        private readonly LiteDatabase _db = new(Path.Combine(ApplicationData.Current.LocalFolder.Path, "shirase.db"));

        public LocalDatabase() { }

        public void Insert<T>(string collection, T data)
        {
            var col = _db.GetCollection<T>(collection);

            col.Upsert(data);
        }

        public void InsertBulk<T>(string collection, List<T> data)
        {
            var col = _db.GetCollection<T>(collection);

            col.InsertBulk(data);
        }

        public IEnumerable<T> GetAll<T>(string collection)
        {
            var col = _db.GetCollection<T>(collection);

            return col.FindAll();
        }

        public ILiteQueryable<T> Query<T>(string collection)
        {
            var col = _db.GetCollection<T>(collection);

            return col.Query();
        }
    }
}
 