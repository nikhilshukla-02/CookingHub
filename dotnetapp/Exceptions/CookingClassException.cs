using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace dotnetapp.Exceptions
{
    public class CookingClassException : Exception
    {
        public CookingClassException(string message) : base (message)
        {
            
        }

    }
}