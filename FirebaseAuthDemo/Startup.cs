using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAuthDemo.Configurations;
using FirebaseAuthDemo.Configurations.Elasticsearch;
using FirebaseAuthDemo.Filters;
using FirebaseAuthDemo.Models;
using FirebaseAuthDemo.Services;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace FirebaseAuthDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var projectId = Configuration.GetSection("FirebaseApi").GetValue<string>("ProjectId");

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddJsonOptions(o => o.SerializerSettings.ContractResolver = new DefaultContractResolver())
                    .AddMvcOptions(o => o.AllowEmptyInputInBodyModelBinding = true);

            InitializeDatabaseService(services);
            InitializeAuthService(services);
            InitializeSearchService(services);
            InitializeRecipeService(services);
            InitializeReviewsService(services);
            InitializeValidationService(services);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = $"https://securetoken.google.com/{projectId}";
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = $"https://securetoken.google.com/{projectId}",
                            ValidateAudience = true,
                            ValidAudience = projectId,
                            ValidateLifetime = true
                        };
                    });



            services.AddScoped<HttpResponseExceptionFilter>();
            services.AddScoped<ValidationFilter>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = Configuration.GetSection("OpenAPI").GetValue<string>("Title"), Version = Configuration.GetSection("OpenAPI").GetValue<string>("Version") });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "FirebaseAuthDemo.xml");
                c.IncludeXmlComments(filePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReGen API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();       
        }

        public FirebaseClient CreateFirebaseDBClient()
        {
            var databaseUrl = Configuration.GetSection("FirebaseApi")
                                           .GetSection("Database")
                                           .GetValue<string>("Url");

            var secret = Configuration.GetSection("FirebaseApi")
                                      .GetSection("Database")
                                      .GetValue<string>("Secret");

            FirebaseClient client =
                new FirebaseClient(
                    databaseUrl,
                    new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(secret),
                    }
                );

            return client;
        }

        private void InitializeDatabaseService(IServiceCollection services)
        {
            var dbClient = CreateFirebaseDBClient();

            services.AddSingleton(dbClient);

            services.AddSingleton<IDatabaseService, FirebaseDatabaseService>();
        }

        private void InitializeAuthService(IServiceCollection services)
        {
            var path = Configuration.GetSection("FirebaseApi").GetValue<string>("CredentialFilePath");

            JToken jAppSettings = JToken.Parse(
                File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json"))
            );
            

            string mapping = jAppSettings["GoogleCredentials"].ToString();


            // Initialize the default app
            var defaultApp = FirebaseApp.Create(new AppOptions()
            {
                //Credential = GoogleCredential.FromFile(path)
                Credential = GoogleCredential.FromJson(mapping)
            });
            Console.WriteLine(defaultApp.Name); // "[DEFAULT]"

            // Retrieve services by passing the defaultApp variable...
            var defaultAuth = FirebaseAuth.GetAuth(defaultApp);

            // ... or use the equivalent shorthand notation
            //defaultAuth = FirebaseAuth.DefaultInstance;

            services.AddSingleton(defaultAuth);

            services.AddSingleton<IAuthService, FirebaseAuthService>();
        }

        /// <summary>
        /// Creates a configured high-level client instance to use the Elasticsearch API.
        /// </summary>
        /// <param name="config">A strongly-typed configuration object.</param>
        /// <returns></returns>
        private ElasticClient CreateElasticClient(ElasticsearchConfiguration config)
        {
            var elasticConnectionSettings =
                new ConnectionSettings(new Uri(config.Uri))
                    .BasicAuthentication(config.Auth.Username, config.Auth.Password)
                    .DefaultMappingFor<RecipeLite>(s => s.IndexName(config.Index));
            elasticConnectionSettings.DefaultIndex(config.Index);
            return new ElasticClient(elasticConnectionSettings);
        }

        /// <summary>
        /// Adds a new Elasticsearch service to the DI container. Service configurations are read from a config file.
        /// </summary>
        /// <param name="services">The service provider.</param>
        private void InitializeSearchService(IServiceCollection services)
        {
            // Elasticsearch service initialization
            var elasticConfig = new ElasticsearchConfiguration();
            Configuration.GetSection("ElasticsearchApi").Bind(elasticConfig);
            //elasticConfig.Auth = auth;
            //elasticConfig.Uri = uri;

            services.AddSingleton(elasticConfig);

            var elasticClient = CreateElasticClient(elasticConfig);
            services.AddSingleton(elasticClient);

            services.AddScoped<ISearchService, ElasticsearchService>();
        }

        private void InitializeValidationService(IServiceCollection services)
        {
            services.AddSingleton<IValidationService, ValidationService>();
        }

        private void InitializeReviewsService(IServiceCollection services)
        {
            services.AddScoped<IReviewService, ReviewService>();
        }

        private void InitializeRecipeService(IServiceCollection services)
        {
            services.AddScoped<IRecipeService, RecipeService>();
        }
    }
}
