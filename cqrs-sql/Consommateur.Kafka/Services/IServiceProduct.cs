using System.Collections.Generic;
using Consommateur.Kafka.Modeles;

namespace Consommateur.Kafka.Services
{
    public interface IServiceProduct
    {
        product Create(product produit);
        List<product> Get();
        product Get(int id);
        void Remove(product produitIn);
        void Remove(int id);
        void Update(int id, product produitIn);
    }
}