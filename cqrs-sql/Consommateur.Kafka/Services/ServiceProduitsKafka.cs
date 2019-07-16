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
        private string bootstrapServers = "";
        private string schemaRegistryUrl = "";

        SchemaRegistryConfig schemaRegistryConfig = null;

        ConsumerConfig consumerConfig = null;

        public ServiceProduitsKafka(IConfiguration configuration, IServiceProduct serviceProduct)
        {
            _configuration = configuration;

            _serviceProduct = serviceProduct;

            InitLogger();

            ChargerVarEnv();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("server1.dbo.products démarré à : {Now}", DateTime.Now);

            stoppingToken.Register(() =>
                                   _logger.Information("server1.dbo.products a débuté son arrêt à : {Now}", DateTime.Now));

            var tacheConsommateur = Task.Run(() =>
            {
                using (var serdeProvider = new AvroSerdeProvider(new AvroSerdeProviderConfig { SchemaRegistryUrl = schemaRegistryUrl }))
                using (var consumer = new Consumer<GenericRecord, GenericRecord>(consumerConfig,
                    serdeProvider.GetDeserializerGenerator<GenericRecord>().Invoke(true),
                    serdeProvider.GetDeserializerGenerator<GenericRecord>().Invoke(false)))
                {
                    consumer.Subscribe(TOPIC);

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                var consumeResult = consumer.Consume(stoppingToken);

                                _logger.Information("Key: {Key}, Value: {Value}",
                                    consumeResult.Message.Key,
                                    consumeResult.Value);
                            }
                            catch (ConsumeException e)
                            {
                                _logger.Error("Consume error: {Reason}", e.Error.Reason);
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
        
        private void ChargerVarEnv()
        {
            bootstrapServers = Environment.GetEnvironmentVariable(ENV_KAFKA_BOOTSTRAPSERVERS);
            schemaRegistryUrl = Environment.GetEnvironmentVariable(ENV_KAFKA_SCHEMA_REGISTRY_URL);

            schemaRegistryConfig = new SchemaRegistryConfig
            {
                // Note: you can specify more than one schema registry url using the
                // schema.registry.url property for redundancy (comma separated list). 
                // The property name is not plural to follow the convention set by
                // the Java implementation.
                SchemaRegistryUrl = schemaRegistryUrl,
                // optional schema registry client properties:
                SchemaRegistryRequestTimeoutMs = 5000,
                SchemaRegistryMaxCachedSchemas = 10
            };

            consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = "avro-server1.dbo.products"
            };
        }
    }
}
