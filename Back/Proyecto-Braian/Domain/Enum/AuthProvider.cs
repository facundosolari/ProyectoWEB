using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    [Flags]
    public enum AuthProvider
    {
        None = 0,
        Local = 1,
        Google = 2
    }
}
