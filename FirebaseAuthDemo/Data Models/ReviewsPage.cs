using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Models
{
    public class ReviewsPage
    {
        [Required]
        public IEnumerable<Review> Reviews
        { get; set; }

        [Required]
        public string NextKey
        { get; set; }
    }
}
