using FirebaseAuthDemo.Models;
using FirebaseAuthDemo.Utils.Validation.Http_Request_Body_Schemas;
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
        public void ValidateUserRatingForm(JObject data)
        {
            var schema = JsonSchema.FromType<UserRatingForm>();

            var errors = schema.Validate(data.ToString());
            
            if (errors.Count > 0)
            {
                var e = new FormatException("Badly formatted JSON body");
                e.Data.Add("ValidationErrors", errors);
                throw e;
            }

            return;
        }

        public void ValidateRecipeForm(JObject recipe)
        {
            var schema = JsonSchema.FromType<Recipe>();

            var errors = schema.Validate(recipe.ToString());

            if (errors.Count > 0)
            {
                var e = new FormatException("Badly formatted JSON body");
                e.Data.Add("ValidationErrors", errors);
                throw e;
            }

            return;
        }

        public void ValidateReviewForm(JObject review)
        {
            var schema = JsonSchema.FromType<ReviewForm>();

            var errors = schema.Validate(review.ToString());

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
