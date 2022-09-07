using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posts.Domain.Exceptions
{
    public class InvalidUserIdException : Exception
    {
        public InvalidUserIdException(string message) : base(message)
        {

        }
    }
}
