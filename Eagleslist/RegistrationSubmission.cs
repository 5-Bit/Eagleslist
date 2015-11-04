using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eagleslist
{
    public class RegistrationSubmission
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Email { get; private set; }

        public RegistrationSubmission(string username, string password, string email)
        {
            this.Username = username;
            this.Password = password;
            this.Email = email;
        }
    }
}
