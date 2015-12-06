using System.Collections.Generic;

namespace Eagleslist
{
    public class GoogleBookResponse
    {
        public string kind { get; set; }
        public int totalItems { get; set; }
        public List<GoogleBook> items { get; set; }
    }
}
