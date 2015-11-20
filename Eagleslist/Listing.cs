
using System;

namespace Eagleslist
{
    public class Listing
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Handle { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string ImageURL { get; set; }
        public string ISBN { get; set; }
        public string Price { get; set; }
        public string Condition { get; set; }
        public DateTime CreateDate { get; set; }

        public int ListingID { get; set; }
        public int UserID { get; set; }
    }
}
