using FirebaseAuthDemo.Models;
using FirebaseAuthDemo.Utils.Validation.NewFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Utils.Validation.Http_Request_Body_Schemas
{
    public class RecipeForm
    {
        #region Properties

        [Required]
        [StringLength(30, MinimumLength = 1)]
        public string Name
        { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int? Calories
        { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int? PrepTimeMinutes
        { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(30)]
        [CollectionConstraints(typeof(Ingredient))]
        public IEnumerable<Ingredient> Ingredients
        { get; set; }

        [Required]
        [StringEnumerableConstraints(StringMinLength = 1, StringMaxLength = 30, MinSize = 1, MaxSize = 30)]
        public IEnumerable<string> Steps
        { get; set; }

        [StringEnumerableConstraints(StringMinLength = 1, StringMaxLength = 30, MinSize = 1, MaxSize = 30)]
        public IEnumerable<string> Tags
        { get; set; }

        public string ImageReferencePath
        { get; set; }

        public string RootImagePath
        { get; set; }

        #endregion
    }
}
