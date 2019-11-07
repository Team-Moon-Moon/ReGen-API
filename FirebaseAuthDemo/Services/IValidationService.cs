using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Services
{
    public interface IValidationService
    {
        void ValidateUserRatingForm(JObject data);

        void ValidateRecipeForm(JObject recipe);

        void ValidateReviewForm(JObject review);
    }
}
