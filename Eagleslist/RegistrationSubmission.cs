
namespace Eagleslist
{
    public class RegistrationSubmission
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public RegistrationSubmission(string Username, string Password, string Email) 
        {
            this.Username = Username;
            this.Password = Password;
            this.Email = Email;
        }
    }
}
