using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Models
{
    public class Ingredient
    {
        #region Properties

        public string IngredientName
        { get; set; }

        public string IngredientAmount
        { get; set; }

        #endregion

        public override string ToString()
        {
            return $"{IngredientAmount} {IngredientName}";
        }
    }
}
