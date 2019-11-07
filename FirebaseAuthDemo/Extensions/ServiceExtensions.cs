using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Extensions
{
    //public static class ServiceExtensions
    //{
    //    private FirebaseClient CreateFirebaseDBClient()
    //    {
    //        var databaseUrl = Configuration.GetSection("FirebaseApi")
    //                                       .GetSection("Database")
    //                                       .GetValue<string>("Url");

    //        var secret = Configuration.GetSection("FirebaseApi")
    //                                  .GetSection("Database")
    //                                  .GetValue<string>("Secret");

    //        FirebaseClient client =
    //            new FirebaseClient(
    //                databaseUrl,
    //                new FirebaseOptions
    //                {
    //                    AuthTokenAsyncFactory = () => Task.FromResult(secret),
    //                }
    //            );

    //        return client;
    //    }

    //    private void InitializeDatabaseService(IServiceCollection services)
    //    {
    //        var dbClient = CreateFirebaseDBClient();

    //        services.AddSingleton(dbClient);

    //        services.AddSingleton<IDatabaseService, FirebaseDatabaseService>();
    //    }

    //    private void InitializeAuthService(IServiceCollection services)
    //    {
    //        var path = Configuration.GetSection("FirebaseApi").GetValue<string>("CredentialFilePath");

    //        // Initialize the default app
    //        var defaultApp = FirebaseApp.Create(new AppOptions()
    //        {
    //            Credential = GoogleCredential.FromFile(path),
    //        });
    //        Console.WriteLine(defaultApp.Name); // "[DEFAULT]"

    //        // Retrieve services by passing the defaultApp variable...
    //        var defaultAuth = FirebaseAuth.GetAuth(defaultApp);

    //        // ... or use the equivalent shorthand notation
    //        //defaultAuth = FirebaseAuth.DefaultInstance;

    //        services.AddSingleton(defaultAuth);

    //        services.AddSingleton<IAuthService, FirebaseAuthService>();
    //    }

    //    /// <summary>
    //    /// Creates a configured high-level client instance to use the Elasticsearch API.
    //    /// </summary>
    //    /// <param name="config">A strongly-typed configuration object.</param>
    //    /// <returns></returns>
    //    private ElasticClient CreateElasticClient(ElasticsearchConfiguration config)
    //    {
    //        var elasticConnectionSettings =
    //            new ConnectionSettings(new Uri(config.Uri))
    //                .BasicAuthentication(config.Auth.Username, config.Auth.Password)
    //                .DefaultMappingFor<RecipeLite>(s => s.IndexName(config.Index));
    //        elasticConnectionSettings.DefaultIndex(config.Index);
    //        return new ElasticClient(elasticConnectionSettings);
    //    }

    //    /// <summary>
    //    /// Adds a new Elasticsearch service to the DI container. Service configurations are read from a config file.
    //    /// </summary>
    //    /// <param name="services">The service provider.</param>
    //    private void InitializeSearchService(IServiceCollection services)
    //    {
    //        // Elasticsearch service initialization
    //        var elasticConfig = new ElasticsearchConfiguration();
    //        Configuration.GetSection("ElasticsearchApi").Bind(elasticConfig);
    //        //elasticConfig.Auth = auth;
    //        //elasticConfig.Uri = uri;

    //        services.AddSingleton(elasticConfig);

    //        var elasticClient = CreateElasticClient(elasticConfig);
    //        services.AddSingleton(elasticClient);

    //        services.AddScoped<ISearchService, ElasticsearchService>();
    //    }

    //    private void InitializeValidationService(IServiceCollection services)
    //    {
    //        services.AddSingleton<IValidationService, ValidationService>();
    //    }

    //    private void InitializeReviewsService(IServiceCollection services)
    //    {
    //        services.AddScoped<IReviewsService, ReviewsService>();
    //    }
    //}
}
