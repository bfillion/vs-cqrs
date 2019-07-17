using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Consommateur.Kafka.Modeles;
using Consommateur.Kafka.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace Consommateur.Kafka
{
    public class Startup
    {
        //Constantes
        string MONGO_NOM_BANQUE = "MONGO_NOM_BANQUE";
        string MONGO_NOM_COLLECTION = "MONGO_NOM_COLLECTION";
        string MONGO_CHAINE_CONNEXION = "MONGO_CHAINE_CONNEXION";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //IOC Container
        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var builder = new ContainerBuilder();

            //Ajout des services au conteneur d'injection de dépendances.
            services.AddSingleton<IMagasinConfigBanque>(new MagasinConfigBanque()
            {
                ChaineConnexion = Configuration.GetSection(MONGO_CHAINE_CONNEXION).Value,
                NomBanque = Configuration.GetSection(MONGO_NOM_BANQUE).Value,
                NomCollection = Configuration.GetSection(MONGO_NOM_COLLECTION).Value
            });

            services.AddTransient(typeof(IServiceProduct), typeof(ServiceProduct));

            services.AddSingleton<IHostedService, ServiceProduitsKafka>();
            
            builder.Populate(services);
            this.ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .Enrich.WithProperty("Application", "Consommateur.Kafka")
                .CreateLogger();

            app.UseMvc();
        }
    }
}
