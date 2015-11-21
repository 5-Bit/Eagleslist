
namespace Eagleslist
{
    public class CommentRequest
    {
        public string SessionID { get; set; }
        public Comment Comment { get; set; }

        public CommentRequest(string sessionId, Comment comment)
        {
            this.Comment = Comment;
            this.SessionID = sessionId;
        }
    }
}
