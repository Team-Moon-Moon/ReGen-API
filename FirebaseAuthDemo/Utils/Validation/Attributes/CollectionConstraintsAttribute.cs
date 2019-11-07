using System;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Collections;

namespace FirebaseAuthDemo.Utils.Validation.NewFolder
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class CollectionConstraintsAttribute : ValidationAttribute
    {
        public int MinSize
        { get; set; }

        public int MaxSize
        { get; set; }

        private Type _type;

        public CollectionConstraintsAttribute(Type type)
        {
            _type = type;
            MinSize = 0;
            MaxSize = int.MaxValue;
        }

        public override bool IsValid(object value)
        {
            try
            {
                var list = value as IEnumerable;

                if (list != null)
                {
                    foreach (var s in list)
                    {
                        if (s is null)
                        {
                            ErrorMessage = "Field element(s) cannot be null.";
                            return false;
                        }
                        if (s.GetType() != _type)
                        {
                            ErrorMessage = $"Field element(s) must match type {_type.Name}";
                            return false;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
