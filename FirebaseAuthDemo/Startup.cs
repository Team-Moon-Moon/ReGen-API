using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAuthDemo.Services;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
            var projectId = Configuration.GetSection("Firebase").GetValue<string>("ProjectId");

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            InitializeDatabaseService(services);
            InitializeAuth(services);

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


            services.AddSingleton<IValidationService, ValidationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();       
        }

        public FirebaseClient CreateFirebaseDBClient()
        {
            var databaseUrl = Configuration.GetSection("Firebase")
                                           .GetSection("Database")
                                           .GetValue<string>("Url");

            var secret = Configuration.GetSection("Firebase")
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

        public void InitializeDatabaseService(IServiceCollection services)
        {
            var dbClient = CreateFirebaseDBClient();

            services.AddSingleton(dbClient);

            services.AddSingleton<IDatabaseService, FirebaseDatabaseService>();
        }

        public void InitializeAuth(IServiceCollection services)
        {
            var path = Configuration.GetSection("Firebase").GetValue<string>("CredentialFilePath");

            // Initialize the default app
            var defaultApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(path),
            });
            Console.WriteLine(defaultApp.Name); // "[DEFAULT]"

            // Retrieve services by passing the defaultApp variable...
            var defaultAuth = FirebaseAuth.GetAuth(defaultApp);

            // ... or use the equivalent shorthand notation
            //defaultAuth = FirebaseAuth.DefaultInstance;

            services.AddSingleton(defaultAuth);

            services.AddSingleton<IAuthService, FirebaseAuthService>();
        }
    }
}
