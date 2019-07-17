using System;
using System.Collections.Generic;
using Consommateur.Kafka.Modeles;
using MongoDB.Driver;

namespace Consommateur.Kafka.Services
{
    public class ServiceProduct : IServiceProduct
    {
        private readonly IMongoCollection<product> _produits;

        public ServiceProduct(IMagasinConfigBanque config)
        {
            var client = new MongoClient(config.ChaineConnexion);
            var database = client.GetDatabase(config.NomBanque);

            _produits = database.GetCollection<product>(config.NomCollection);
        }

        public List<product> Get() =>
            _produits.Find(product => true).ToList();

        public product Get(int id) =>
            _produits.Find<product>(product => product.idProduit == id).FirstOrDefault();

        public product Create(product produit)
        {
            _produits.InsertOne(produit);
            return produit;
        }

        public void Update(int id, product produitIn) =>
            _produits.ReplaceOne(product => product.idProduit == id, produitIn);

        public void Remove(product produitIn) =>
            _produits.DeleteOne(product => product.Id == produitIn.Id);

        public void Remove(int id) =>
            _produits.DeleteOne(product => product.idProduit == id);

    }
}
