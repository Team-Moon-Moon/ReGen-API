using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Models
{
    public class Review
    {
        [Required]
        public string ReviewId
        { get; set; }

        [Required]
        public string UserId
        { get; set; }

        [Required]
        [JsonIgnore]
        public string RecipeId
        { get; set; }

        [Required]
        public string Content
        { get; set; }

        [Required]
        public DateTime Timestamp
        { get; set; }

        [Required]
        [JsonIgnore]
        [Range(1, 5)]
        public int Rating
        { get; set; }

        public override string ToString()
        {
            return Content;
        }
    }
}
