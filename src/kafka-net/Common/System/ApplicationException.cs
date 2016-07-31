#if NETCORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System
{

    public class ApplicationException
        : Exception
    {

        public ApplicationException()
            : this("")
        {
        }

        public ApplicationException(string message)
            : base(message)
        {
        }

    }

}
#endif