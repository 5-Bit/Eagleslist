
namespace Eagleslist
{
    public class CommentRequest
    {
        public string SessionID { get; set; }

        public CommentRequest(string sessionId)
        {
            this.SessionID = sessionId;
        }
    }
}
