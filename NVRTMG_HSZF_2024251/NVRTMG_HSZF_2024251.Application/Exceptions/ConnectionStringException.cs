using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVRTMG_HSZF_2024251.Application.Exceptions
{
    public class ConnectionStringException : Exception
    {
        public ConnectionStringException(string message) : base(message) { } 
    }
}
