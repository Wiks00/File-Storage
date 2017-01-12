using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public interface ILogAdapter
    {
        void Error(Exception error, string message);
        void Info(string message);
    }
}
