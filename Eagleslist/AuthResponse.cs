using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eagleslist
{
    public class AuthResponse
    {
        public string Error { get; private set; }
        public int UserID { get; private set; }
        public string SessionID { get; private set; }

        public AuthResponse(string error, int userID, string sessionID)
        {
            this.Error = error;
            this.UserID = userID;
            this.SessionID = sessionID;
        }
    }
}
