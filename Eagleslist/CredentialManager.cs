using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eagleslist
{
    public class CredentialManager
    {
        public static User getCurrentUser()
        {
            return new User(0, "session", "", "msmaccallum5438@eagle.fgcu.edu", "0x7fffffff", "bio", true, false);
        }

        public static void setCurrentUser(User user)
        {

        }
    }
}
