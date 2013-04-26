using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAControls
{
    public enum DataLoadState
    {
        Unknown = 0,
        Initialized = 1,
        Loading = 2,
        Complete = 4,
        Success = 8 + 4,
        Error = 16 + 4
    }
}
