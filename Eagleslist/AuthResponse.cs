
namespace Eagleslist
{
    public class AuthResponse
    {
        public string Error { get; private set; }
        public string SessionID { get; private set; }
        public int UserID { get; private set; }

        public AuthResponse(string Error, string SessionID, int UserID)
        {
            this.Error = Error;
            this.SessionID = SessionID;
            this.UserID = UserID;
        }
    }
}
