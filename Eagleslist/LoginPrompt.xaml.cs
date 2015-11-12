using System;
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
            ProgressBar.Visibility = Visibility.Visible;
            AttemptLogin(handleField.Text, passwordField.Password);
        }

        private async void AttemptLogin(string handle, string password)
        {
            LoginRequest request = new LoginRequest(handle, password);
            User user = await RequestManager.AttemptLogin(request);
            ProgressBar.Visibility = Visibility.Collapsed;

            if (user != null)
            {
                if (user.AuthError == null || user.AuthError.Length == 0)
                {
                    MainWindow mainWindow = (MainWindow)Owner;
                    mainWindow.CurrentUser = user;

                    this.DialogResult = true;
                    Close();
                } else
                {
                    Console.WriteLine("Login failed");
                }
            }
            else
            {
                Console.WriteLine("Login failed");
            }
        }

        private void InputFieldChanged(object sender, RoutedEventArgs e)
        {
            SignInButton.IsEnabled = handleField.Text.Length > 0 && passwordField.Password.Length > 0;
        }
    }
}
