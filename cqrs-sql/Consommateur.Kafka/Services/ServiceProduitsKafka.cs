using Confluent.SchemaRegistry;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avro.Generic;
using Confluent.Kafka.Serialization;
using server1.dbo.products;
using Consommateur.Kafka.Modeles;

namespace Consommateur.Kafka.Services
{
    public class ServiceProduitsKafka : BackgroundService
    {
        //constantes
        const string ENV_KAFKA_BOOTSTRAPSERVERS = "KAFKA_BOOTSTRAPSERVERS";
        const string ENV_KAFKA_SCHEMA_REGISTRY_URL = "KAFKA_SCHEMA_REGISTRY_URL";
        const string TOPIC = "server1.dbo.products";

        //variables
        private readonly IConfiguration _configuration = null;
        private readonly IServiceProduct _serviceProduct = null;

        private ILogger _logger = null;
        
        private readonly SchemaRegistryConfig schemaRegistryConfig = null;
        private readonly ConsumerConfig consumerConfig = null;

        public ServiceProduitsKafka(IConfiguration configuration, IServiceProduct serviceProduct)
        {
            _configuration = configuration;

            _serviceProduct = serviceProduct;

            InitLogger();

            schemaRegistryConfig = new SchemaRegistryConfig
            {
                SchemaRegistryUrl = _configuration.GetSection(ENV_KAFKA_SCHEMA_REGISTRY_URL).Value,
                SchemaRegistryRequestTimeoutMs = 5000,
                SchemaRegistryMaxCachedSchemas = 10
            };

            consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _configuration.GetSection(ENV_KAFKA_BOOTSTRAPSERVERS).Value,
                GroupId = "avro-server1.dbo.products"
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("server1.dbo.products démarré à : {Now}", DateTime.Now);

            stoppingToken.Register(() =>
                                   _logger.Information("server1.dbo.products a débuté son arrêt à : {Now}", DateTime.Now));

            var tacheConsommateur = Task.Run(() =>
            {
                using (var serdeProvider = new AvroSerdeProvider(new AvroSerdeProviderConfig { SchemaRegistryUrl = schemaRegistryConfig.SchemaRegistryUrl }))
                using (var consumer = new Consumer<GenericRecord, Envelope>(consumerConfig,
                    serdeProvider.GetDeserializerGenerator<GenericRecord>().Invoke(true),
                    serdeProvider.GetDeserializerGenerator<Envelope>().Invoke(false)))
                {
                    consumer.Subscribe(TOPIC);

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                var consumeResult = consumer.Consume(stoppingToken);

                                if (consumeResult.Value.before == null && consumeResult.Value.after != null)
                                {
                                    //Ajout
                                    product produit = new product()
                                    {
                                        idProduit = consumeResult.Value.after.id,
                                        name = consumeResult.Value.after.name,
                                        description = consumeResult.Value.after.description,
                                        weight = consumeResult.Value.after.weight
                                    };

                                    _serviceProduct.Create(produit);
                                }

                                if (consumeResult.Value.before != null && consumeResult.Value.after != null)
                                {
                                    //MAJ
                                    product produit = new product()
                                    {
                                        idProduit = consumeResult.Value.after.id,
                                        name = consumeResult.Value.after.name,
                                        description = consumeResult.Value.after.description,
                                        weight = consumeResult.Value.after.weight
                                    };

                                    _serviceProduct.Update(produit.idProduit, produit);
                                }

                                if (consumeResult.Value.before != null && consumeResult.Value.after == null)
                                {
                                    //Suppression
                                    _serviceProduct.Remove(consumeResult.Value.before.id);
                                }

                                _logger.Information("Key: {Key}, Value: {Value}",
                                    consumeResult.Message.Key,
                                    consumeResult.Value);
                            }
                            catch (ConsumeException e)
                            {
                                _logger.Error("Consommation erreur: {Reason}", e.Error.Reason);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // commit final offsets and leave the group.
                        consumer.Close();
                    }
                }
            });

            //_logger.Information("service server1.dbo.products arrêté à : {Now}", DateTime.Now);
        }

        private void InitLogger()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
