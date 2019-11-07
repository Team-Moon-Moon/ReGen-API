using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Models
{
    //[ElasticsearchType(RelationName = "recipes")]
    public class RecipeLite
    {
        [Text(Name = "name")]
        public string Name
        { get; set; }

        //[Nested]
        //[PropertyName("ingredients")]
        [Object]
        public IEnumerable<Ingredient> Ingredients
        { get; set; }

        [PropertyName("tags")]
        public IEnumerable<string> Tags
        { get; set; }
    }
}
