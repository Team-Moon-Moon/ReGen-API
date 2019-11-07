using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Configurations.Elasticsearch
{
    public class ElasticsearchConfiguration
    {
        public Auth Auth { get; set; }
        public string Uri { get; set; }
        public string Index { get; set; }
    }

    public class Auth
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
