using System;

namespace Eagleslist
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Comment
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public int ParentListingID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime EndDate { get; set; }

        public Comment(int id, int userId, string userName, string content, int parentListingId, DateTime createDate, DateTime endDate)
        {
            this.ID = id;
            this.UserID = userId;
            this.UserName = userName;
            this.Content = content;
            this.ParentListingID = parentListingId;
            this.CreateDate = createDate;
            this.EndDate = endDate;
        }
    }
}
