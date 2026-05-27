using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media_file
{

    [Flags]
    public enum Gen
    {
        None = 0,
        Fantezie = 1,
        ScientificFiction = 2,
        Mister = 4,
        Romance = 8,
        NonFictiune = 16
    }
}
