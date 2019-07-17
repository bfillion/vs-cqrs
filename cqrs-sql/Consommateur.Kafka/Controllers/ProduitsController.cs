using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consommateur.Kafka.Modeles;
using Consommateur.Kafka.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Consommateur.Kafka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProduitsController : ControllerBase
    {
        private readonly IServiceProduct _serviceProduct = null;

        public ProduitsController(IServiceProduct serviceProduct)
        {
            _serviceProduct = serviceProduct;
        }

        // GET: api/Produits
        [HttpGet]
        public IEnumerable<product> Get()
        {
            List<product> products = _serviceProduct.Get();

            return products;
        }
    }
}
