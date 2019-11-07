using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Models
{
    public class Recipe
    {
        #region Properties

        [Required]
        public string Key
        { get; set; }

        [Required]
        public string AuthorId
        { get; set; }

        [Required]
        public string Name
        { get; set; }

        [Required]
        public int? Calories
        { get; set; }

        [Required]
        public int? PrepTimeMinutes
        { get; set; }

        [Required]
        public IEnumerable<Ingredient> Ingredients
        { get; set; }

        [Required]
        public IEnumerable<string> Steps
        { get; set; }

        [Obsolete]
        public int StarRating
        { get; set; }

        [Obsolete]
        public string[] Reviews
        { get; set; }

        public IEnumerable<string> Tags
        { get; set; }

        public string ImageReferencePath
        { get; set; }

        //public Sprite ImageSprite;

        public string RootImagePath
        { get; set; }

        #endregion

        ///// <summary>
        ///// Creates a new instance of Recipe
        ///// </summary>
        ///// <param name="name">The name of the recipe</param>
        ///// <param name="imagePath">Image itemPath url for the database</param>
        ///// <param name="calories">The amount of calories</param>
        ///// <param name="prepTimeMinutes">The prep time in minutes</param>
        ///// <param name="tags">The tags for this recipe</param>
        ///// <param name="ingredients">The ingredients for this recipe</param>
        ///// <param name="steps">The steps for this recipe</param>
        ///// <param name="reviews">The reviews for this recipe</param>
        ///// <param name="starRating">The rating for this recipe</param>
        //public Recipe(string name, string imagePath, int calories, int prepTimeMinutes, List<string> tags, List<Ingredient> ingredients, List<string> steps, List<string> reviews = null, int starRating = 5, string key = "")
        //{

        //    Name = name;
        //    ImageReferencePath = imagePath;
        //    Calories = calories;
        //    PrepTimeMinutes = prepTimeMinutes;
        //    Tags = tags.ToArray();
        //    Ingredients = ingredients.ToArray();
        //    Steps = steps.ToArray();
        //    StarRating = starRating;

        //    if (reviews != null)
        //        Reviews = reviews.ToArray();

        //    Key = key;


        //}

        
        public override string ToString()
        {
            String recipeString = "";

            recipeString += "Name: " + Name;
            recipeString += "\nCalories: " + Calories;
            recipeString += "\nPrep Time: " + PrepTimeMinutes + " minutes";
            recipeString += "\n\nIngredients: ";

            foreach (var ingredient in Ingredients)
                recipeString += "\n" + ingredient;

            recipeString += "\n\nSteps: ";

            foreach (var step in Steps)
                recipeString += "\n\n" + step;

            recipeString += "\n\n" + RootImagePath;

            return recipeString;
        }

    }
}
