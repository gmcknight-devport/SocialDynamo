using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Tests.Exceptions
{
    public class DuplicateUserContainerException : Exception
    {
        public DuplicateUserContainerException(string message) : base(message)
        {

        }
    }
}
