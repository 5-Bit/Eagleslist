using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eagleslist
{
    public interface Navigatable<T, U> where T : new()
    {
        T LoadFromNavigation(U load);
    }
}
