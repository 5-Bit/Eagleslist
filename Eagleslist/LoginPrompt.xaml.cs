using System.Windows;

namespace Eagleslist
{
    public partial class LoginPrompt : Window
    {
        public LoginPrompt()
        {
            InitializeComponent();
        }

        private void SignInClicked(object sender, RoutedEventArgs e)
        {
            AttemptLogin(handleField.Text, passwordField.Password);
        }

        private async void AttemptLogin(string handle, string password)
        {
            LoginRequest request = new LoginRequest(handle, password);
            User user = await RequestManager.AttemptLogin(request);

            if (user.AuthError == null || user.AuthError.Length == 0)
            {
                this.DialogResult = true;
                Close();

                MainWindow mainWindow = (MainWindow)Owner;
                mainWindow.currentUser = user;
            }
        }
    }
}
