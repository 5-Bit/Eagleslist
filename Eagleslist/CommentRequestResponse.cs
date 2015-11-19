using System.Collections.Generic;

namespace Eagleslist
{
    public class CommentRequestResponse
    {
        public string Error { get; set; }
        public List<Comment> Comments { get; set; } 
    }
}
