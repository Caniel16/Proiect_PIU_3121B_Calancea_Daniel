using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media_file
{
    [Flags]
    public enum Stare
    {
        None = 0,
        Ascultat = 1,
        Favorit = 2,
        DeAscultat = 4,
        Arhivat = 8
    }
}
