using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.CertGen.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Microsoft.CertGen.Startup))]
namespace Microsoft.CertGen
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddLogging();

            builder.Services.AddSingleton<IConfiguration>(config);

            builder.Services.AddTransient<ICertificateGenerator, CertificateGenerator>();

            builder.Services.AddTransient(Provider => {
                var client = new CertificateClient(new Uri(config["KeyVaultUri"]), new DefaultAzureCredential(false));

                return client;
            });

            //builder.Services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CertGen", Version = "v1" });
            //});

        }
    }
}