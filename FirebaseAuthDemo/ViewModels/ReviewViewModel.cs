using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.ViewModels
{
    public class ReviewViewModel
    {
        [Required]
        public string Author
        { get; set; }

        [Required]
        public string Content
        { get; set; }

        [Required]
        public DateTime Timestamp
        { get; set; }

        [Required]
        public int Rating
        { get; set; }
    }
}
