using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Consommateur.Kafka.Modeles
{
    public class product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int idProduit { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public double? weight { get; set; }
    }
}
