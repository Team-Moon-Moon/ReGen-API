using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Utils.Elasticsearch.Search_Templates
{
    [Obsolete]
    public class RecipeTemplate
    {
        public string Query { get; set; }
        public string Size { get; set; }
        public string Params { get; set; }
    }
}
