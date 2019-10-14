using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Validation
{
    public class PutUserRating
    {
        [Required]
        public int Rating { get; set; }
    }
}
