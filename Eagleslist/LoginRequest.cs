using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eagleslist
{
    public class LoginRequest
    {
        public string UserHandle { get; private set; }
        public string Password { get; private set; }

        public LoginRequest(string UserHandle, string Password)
        {
            this.UserHandle = UserHandle;
            this.Password = Password;
        }
    }
}
