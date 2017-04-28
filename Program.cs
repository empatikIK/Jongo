using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Jongo
{
    public class Student
    {
        [BsonId]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Number { get; set; }
        public bool Burs { get; set; }

        public Student Mentor { get; set; }
        public Student[] Friends { get; set; }
    }

    public class IdGenerator
    {
        [BsonId]
        public int Id { get; set; }
        public int LastId { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var r = new JongoDBRepository();

            //r.CreateCustomer(new Customer { Id = "Müşteri 1", Name = "ÖSYM" });
            //r.CreateCustomer(new Customer { Id = "Müşteri 2", Name = "ÖSYM" });
            //r.CreateCustomer(new Customer { Id = "Müşteri 3", Name = "ÖSYM" });

            r.Insert("Müşteri 1", "Haberler", "2", "<div>Haber 1</div>");
            r.Insert("Müşteri 1", "Ziyaretçiler", "2", "[IP:125.121.1,Browser:Chrome}");
            r.Insert("Müşteri 2", "Haberler", "2", "HAber 2");

            r.Insert("Müşteri X", "Haberler", "2", "HAber 2");

            return;

            var client = new MongoClient();
            var db = client.GetDatabase("local");


            var student = new Student
            {
                Id = GetNextId(),
                Name = "Ayşe",
                Surname = "Yılmaz",
                Number = 123,
                Burs = true,
                Mentor = new Jongo.Student { Id = 2, Name = "Ahmet" },
                Friends = new[] { new Student { Name = "Ali" }, new Student { Name = "Adem" } }
            };

            var collection = db.GetCollection<Student>("Students");

            collection.InsertOne(student);
            var d  = collection.Find(c => c.Id == 8).FirstOrDefault();
            var ayseler = collection.Find(c => c.Name == "Ayşe" && c.Mentor != null).ToList();
            collection.FindOneAndDelete(c => c.Id == 8);
            collection.DeleteOne(c => c.Id == 9);
            //collection.ReplaceOne(c => c.Id == 15, new Student { Id = 10, Name = "New 10" });
            
            var item = collection.Find(c => c.Id == 10).FirstOrDefault();
            item.Name = "MErhaba";
            item.Surname = "sMErhaba";
            collection.ReplaceOne(c => c.Id == 10, item);

            var updates = new List<UpdateDefinition<Student>>();

            collection.UpdateOne(c => c.Id == 11, Builders<Student>.Update.Set(c => c.Name, "New 11w"));
        }

        public static int GetNextId()
        {
            var client = new MongoClient();
            var db = client.GetDatabase("local");
            var c = db.GetCollection<IdGenerator>("IDS3");

            var item = c.Find(k => true).FirstOrDefault();

            if (item == null)
            {
                c.InsertOne(new IdGenerator { LastId = 1 });

                return 1;
            }
            c.ReplaceOne(k => true, new IdGenerator { LastId = item.LastId + 1 });

            return item.LastId + 1;
        }
    }
}