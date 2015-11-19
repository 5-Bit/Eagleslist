using System;

namespace Eagleslist
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Comment
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Content { get; set; }
        public int ParentListingID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
