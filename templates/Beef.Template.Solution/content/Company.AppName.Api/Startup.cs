﻿using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Beef.AspNetCore.WebApi;
using Beef.Caching.Policy;
using Beef.Diagnostics;
using Beef.Entities;
using Beef.Validation;
using Beef.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Company.AppName.Business;
using Company.AppName.Business.Data;

namespace Company.AppName.Api
{
    /// <summary>
    /// Represents the <b>startup</b> class.
    /// </summary>
    public class Startup
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="config">The <see cref="IConfiguration"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public Startup(IConfiguration config, ILoggerFactory loggerFactory)
        {
            // Use JSON property names in validation; default the page size and determine whether unhandled exception details are to be included in the response.
            ValidationArgs.DefaultUseJsonNames = true;
            PagingArgs.DefaultTake = config.GetValue<int>("BeefDefaultPageSize");
            WebApiExceptionHandlerMiddleware.IncludeUnhandledExceptionInResponse = config.GetValue<bool>("BeefIncludeExceptionInInternalServerError");

            // Configure the logger.
            _logger = loggerFactory.CreateLogger("Logging");
            Logger.RegisterGlobal((largs) => WebApiStartup.BindLogger(_logger, largs));

            // Configure the cache policies.
            CachePolicyManager.SetFromCachePolicyConfig(config.GetSection("BeefCaching").Get<CachePolicyConfig>());
            CachePolicyManager.StartFlushTimer(CachePolicyManager.TenMinutes, CachePolicyManager.FiveMinutes);

#if (implement_database || implement_entityframework)
            // Register the database.
            Database.Register(() => new Database(WebApiStartup.GetConnectionString(config, "Database")));

#endif
#if (implement_cosmos)
            // Register the DocumentDb/CosmosDb client.
            CosmosDb.Register(() =>
            {
                var cs = config.GetSection("CosmosDb");
                return new CosmosDb(new Microsoft.Azure.Cosmos.CosmosClient(cs.GetValue<string>("EndPoint"), cs.GetValue<string>("AuthKey")), cs.GetValue<string>("Database"));
            });

#endif
            // Register the ReferenceData provider.
            Beef.RefData.ReferenceDataManager.Register(new ReferenceDataProvider());
        }

        /// <summary>
        /// The configure services method called by the runtime; use this method to add services to the container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddHealthChecks();
            services.AddHttpClient();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Company.AppName API", Version = "v1" });

                var xmlName = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlFile = Path.Combine(AppContext.BaseDirectory, xmlName);
                if (File.Exists(xmlFile))
                    c.IncludeXmlComments(xmlFile);
            });
        }

        /// <summary>
        /// The configure method called by the runtime; use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="clientFactory">The <see cref="IHttpClientFactory"/>.</param>
        public void Configure(IApplicationBuilder app, IHttpClientFactory clientFactory)
        {
            // Register the ServiceAgent HttpClientCreate (for cross-domain calls) so it uses the factory.
            WebApiServiceAgentManager.RegisterHttpClientCreate((rd) =>
            {
                var hc = clientFactory.CreateClient(rd.BaseAddress.AbsoluteUri);
                hc.BaseAddress = rd.BaseAddress;
                return hc;
            });

            // Add exception handling to the pipeline.
            app.UseWebApiExceptionHandler();

            // Add Swagger as a JSON endpoint and to serve the swagger-ui to the pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Company.AppName"));

            // Add health checks page to the pipeline.
            app.UseHealthChecks("/health");

            // Add execution context set up to the pipeline.
            app.UseExecutionContext();

            // Add mvc to the pipeline to support the api's themselves.
            app.UseMvc();
        }
    }
}