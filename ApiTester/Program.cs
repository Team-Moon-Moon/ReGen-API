using Firebase.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ApiTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(configuration.GetSection("Firebase").GetSection("ApiKey").Value));
            var auth = Task.Run(() =>
            {
                return authProvider.SignInWithEmailAndPasswordAsync("lol@lol.lol", "password");
            }).Result;

            var accessToken = auth.FirebaseToken;
        }
    }
}
