using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Utils.Validation.Http_Request_Body_Schemas
{
    public class ReviewForm
    {
        //[Required]
        //public string RecipeId
        //{ get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Content
        { get; set; }

        [Required]
        [Range(1,5)]
        public int Rating
        { get; set; }
    }
}
