using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Utils.Custom_Exceptions
{
    public class NoResultsFoundException : Exception
    {
        public NoResultsFoundException()
        {
        }

        public NoResultsFoundException(string message) : base(message)
        {
        }

        public NoResultsFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
