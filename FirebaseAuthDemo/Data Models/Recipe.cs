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
        [Nest.Text(Name = "Key")]
        public string Key
        { get; set; }

        [Required]
        [Nest.Text(Name = "AuthorID")]
        public string AuthorID
        { get; set; }

        [Required]
        [Nest.Text(Name = "name")]
        public string Name
        { get; set; }

        [Required]
        public int? Calories
        { get; set; }

        [Required]
        public int? PrepTimeMinutes
        { get; set; }

        [Required]
        [Nest.PropertyName("ingredients")]
        public IEnumerable<Ingredient> Ingredients
        { get; set; }

        [Required]
        public IEnumerable<string> Steps
        { get; set; }

        //[Obsolete]
        //public int StarRating
        //{ get; set; }

        //[Obsolete]
        //public string[] Reviews
        //{ get; set; }

        [Nest.PropertyName("tags")]
        public IEnumerable<string> Tags
        { get; set; }

        public string ImageReferencePath
        { get; set; }

        //public Sprite ImageSprite;

        public string RootImagePath
        { get; set; }

        #endregion
        
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
