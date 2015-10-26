using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eagleslist
{
    public class Listing
    {
        public string Title { get; private set; }

        public Listing(string title)
        {
            this.Title = title;
        }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
