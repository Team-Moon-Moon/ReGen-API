//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace FirebaseAuthDemo.Configurations.Elasticsearch
//{
//    //public class Auth
//    //{
//    //}

//    public class Auth2
//    {
//        public string Username { get; set; }
//        public string Password { get; set; }
//    }

//    public class ElasticsearchApi
//    {
//        public Auth2 Auth { get; set; }
//        public string Uri { get; set; }
//        public string Index { get; set; }
//    }
//}

//namespace FirebaseAuthDemo.Configurations
//{
//    public class LogLevel
//    {
//        public string Default { get; set; }
//    }

//    public class Logging
//    {
//        public LogLevel LogLevel { get; set; }
//    }

//    public class Database
//    {
//        public string Secret { get; set; }
//        public string Url { get; set; }
//    }

//    public class FirebaseApi
//    {
//        public string ProjectId { get; set; }
//        public string WebApiKey { get; set; }
//        public string ServiceAccount { get; set; }
//        public string CredentialFilePath { get; set; }
//        public Auth Auth { get; set; }
//        public Database Database { get; set; }
//    }

    

    

//    public class OpenAPI
//    {
//        public string Title { get; set; }
//        public string Version { get; set; }
//    }

//    public class OpenID
//    {
//        public string Authority { get; set; }
//        public string Issuer { get; set; }
//    }

//    public class ReGenConfiguration
//    {
//        public Logging Logging { get; set; }
//        public string AllowedHosts { get; set; }
//        public FirebaseApi FirebaseApi { get; set; }
//        public ElasticsearchApi ElasticsearchApi { get; set; }
//        public OpenAPI OpenAPI { get; set; }
//        public OpenID OpenID { get; set; }
//    }
//}
