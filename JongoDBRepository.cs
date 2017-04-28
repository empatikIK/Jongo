using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Security;

namespace Jongo
{
    public class Document
    {
        [BsonId]
        public string Id { get; set; }
        public string Content { get; set; }
    }

    public class Customer
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
    }

    public interface IJongoDBRepository
    {
        void Insert(string customerKey, string collection, string id, string content);
        void Update(string customerKey, string collection, string id, string content);
        void Delete(string customerKey, string collection, string id);
        string Get(string customerKey, string collection, string id);
    }

    public class JongoDBRepository : IJongoDBRepository
    {
        public void Insert(string customerKey, string collection, string id, string content)
        {
            var customer = this.GetCollectionCustomer().Find(c => c.Id == customerKey).FirstOrDefault();

            if (customer == null)
            {
                throw new SecurityException("Customer not found!");
            }

            if (this.Get(customerKey, collection, id) != null)
            {
                throw new Exception("Document already exist");
            }

            this.GetCollection().InsertOne(new Document
            {
                Id = this.GenerateId(customerKey, collection, id),
                Content = content
            });
        }

        public void Update(string customerKey, string collection, string id, string content)
        {
            try
            {
                var documentId = this.GenerateId(customerKey, collection, id);

                this.GetCollection().ReplaceOne(c => c.Id == documentId, new Document
                {
                    Id = documentId,
                    Content = content
                });
            }
            catch (Exception ex)
            {
                // Log exception

                throw new Exception("Bir hata oluştu");
            }
        }

        public string Get(string customerKey, string collection, string id)
        {
            var documentId = this.GenerateId(customerKey, collection, id);

            return this.GetCollection().Find(c => c.Id == documentId).FirstOrDefault()?.Content;
        }

        public void Delete(string customerKey, string collection, string id)
        {
            var documentId = this.GenerateId(customerKey, collection, id);

            this.GetCollection().DeleteOne(c => c.Id == documentId);
        }

        private string GenerateId(string customerKey, string collection, string id)
        {
            return customerKey + "_" + collection + "_" + id;
        }

        private IMongoCollection<Document> GetCollection()
        {
            var client = new MongoClient();
            var db = client.GetDatabase("local");
            var collection = db.GetCollection<Document>("Documents");

            return collection;
        }

        private IMongoCollection<Customer> GetCollectionCustomer()
        {
            var client = new MongoClient();
            var db = client.GetDatabase("local");
            var collection = db.GetCollection<Customer>("Customers");

            return collection;
        }

        public void CreateCustomer(Customer c)
        {
            var client = new MongoClient();
            var db = client.GetDatabase("local");
            var collection = db.GetCollection<Customer>("Customers");

            collection.InsertOne(c);
        }
    }
}
