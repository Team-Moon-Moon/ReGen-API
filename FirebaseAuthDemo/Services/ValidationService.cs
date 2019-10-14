using FirebaseAuthDemo.Validation;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Services
{
    /// <summary>
    /// Validates REST API request bodies using NJsonSchema.
    /// Documentation: https://github.com/RicoSuter/NJsonSchema
    /// </summary>
    public class ValidationService : IValidationService
    {
        public void ValidateRatingPutRequest(JObject data)
        {
            var schema =
                JsonSchema.FromType<PutUserRating>();
            //var schemaData = schema.ToJson();
            var errors = schema.Validate(data.ToString());

            //foreach (var error in errors)
            //    Console.WriteLine(error.Path + ": " + error.Kind);

            //schema = await JsonSchema.FromJsonAsync(schemaData);

            if (errors.Count > 0)
            {
                var e = new FormatException("Badly formatted JSON body");
                e.Data.Add("ValidationErrors", errors);
                throw e;
            }

            return;
        }
    }
}
