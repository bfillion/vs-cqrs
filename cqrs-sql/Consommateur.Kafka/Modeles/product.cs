using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Consommateur.Kafka.Modeles
{
    public class product
    {
        public product()
        {
        
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int id { get; set; }

        public int name { get; set; }

        public int description { get; set; }

        public int weight { get; set; }
    }
}
