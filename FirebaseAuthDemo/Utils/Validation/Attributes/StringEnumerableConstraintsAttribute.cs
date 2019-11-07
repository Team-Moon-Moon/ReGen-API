using System;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FirebaseAuthDemo.Utils.Validation.NewFolder
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class StringEnumerableConstraintsAttribute : ValidationAttribute
    {
        public int StringMinLength
        { get; set; }

        public int StringMaxLength
        { get; set; }

        public int MinSize
        { get; set; }

        public int MaxSize
        { get; set; }

        public StringEnumerableConstraintsAttribute()
        {
            StringMinLength = 0;
            StringMaxLength = int.MaxValue;
            MinSize = 0;
            MaxSize = int.MaxValue;
        }

        public override bool IsValid(object value)
        {
            var obj = value as List<string>;

            if (obj?.Count < MinSize)
            {
                ErrorMessage = $"Field cannot have less than {MinSize} element(s).";
                return false;
            }

            if (obj?.Count > MaxSize)
            {
                ErrorMessage = $"Field cannot have more than {MaxSize} element(s).";
                return false;
            }

            if (obj != null)
            {
                foreach (var str in obj)
                {
                    if (string.IsNullOrWhiteSpace(str) || str.Length < StringMinLength || str.Length > StringMaxLength)
                    {
                        ErrorMessage = $"Field elements must be between {StringMinLength} and {StringMaxLength} characters.";
                        return false;
                    }
                } 
            }

            return true;
        }
    }
}
