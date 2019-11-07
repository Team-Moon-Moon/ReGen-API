using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Utils.Validation
{
    public class GetReviewsParameters
    {
        public string Start
        { get; set; }

        [Range(1, 100)]
        public int Size
        { get; set; } = 5;
    }
}
