using System;
using System.Windows;
using System.Windows.Input;

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
                if (string.IsNullOrEmpty(user.AuthError))
                {
                    CredentialManager.SetCurrentUser(user, StaySignedInCheckbox.IsChecked ?? false);
                    DialogResult = true;
                }
                else
                {
                    Console.WriteLine(user.AuthError);
                    LoginFailed();
                }
            }
            else
            {
                Console.WriteLine("user is null");
                LoginFailed();
            }

            MainWindow window = (MainWindow)Owner;
            window.ReloadLoginStateUi();
        }

        private void LoginFailed()
        {
            InvalidLoginLabel.Visibility = Visibility.Visible;
            passwordField.Password = string.Empty;
            passwordField.Focus();
        }

        private void InputFieldChanged(object sender, RoutedEventArgs e)
        {
            SignInButton.IsEnabled = handleField.Text.Length > 0 && passwordField.Password.Length > 0;
        }

        private void FieldKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SignInButton.IsEnabled)
            {
                SignInClicked(null, null);
            }
        }
    }
}
