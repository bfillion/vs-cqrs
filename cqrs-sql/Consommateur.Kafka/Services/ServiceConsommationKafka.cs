using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Consommateur.Kafka.Services
{
    public class ServiceConsommationKafka : BackgroundService
    {
        //variables
        private readonly IConfiguration _configuration = null;

        private ILogger _logger = null;

        public ServiceConsommationKafka(IConfiguration configuration)
        {
            _configuration = configuration;

            InitLogger();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("LogReader server start at : {Now}", DateTime.Now);

            stoppingToken.Register(() =>
                                   _logger.Information("LogReader begining to stop at : {Now}", DateTime.Now));

            //using (var consumer = new Consumer<Ignore, string>(_config, null, new StringDeserializer(Encoding.UTF8)))
            //{
            //    consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(_topics[0], Partition, 0) });

            //    // Kafka errors.
            //    consumer.OnError += (_, error)
            //        => _logger.Error("Kafka error : {error}", error);

            //    // Consumer errors.
            //    consumer.OnConsumeError += (_, error)
            //        => _logger.Error("Consumer error : {error}", error);

            //    consumer.OnStatistics += (_, json)
            //        => _logger.Information("Stats : {json}", json);

            //    while (!stoppingToken.IsCancellationRequested)
            //    {
            //        Message<Ignore, string> msg;
            //        if (consumer.Consume(out msg, TimeSpan.FromSeconds(1)))
            //        {
            //            _logger.Information("Topic : {Topic} Partition : {Partition} Offset : {Offset} Value : {Value}",
            //                            msg.Topic, msg.Partition, msg.Offset, msg.Value);

            //            try
            //            {
            //                ProcessMessage(msg.Value);
            //            }
            //            catch (Exception erreur)
            //            {
            //                _logger.Error("Error in LogReader service.  Message : {Message}, Stack : {StackTrace}",
            //                          erreur.Message, erreur.StackTrace);
            //            }
            //        }

            //        await Task.Delay(VerificationDelay, stoppingToken);
            //    }
            //}

            _logger.Information("LogReader service stop at : {Now}", DateTime.Now);
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
