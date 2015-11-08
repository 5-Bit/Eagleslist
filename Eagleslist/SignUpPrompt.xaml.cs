using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Eagleslist
{
    /// <summary>
    /// Interaction logic for SignUpPrompt.xaml
    /// </summary>
    public partial class SignUpPrompt : Window
    {
        public SignUpPrompt()
        {
            InitializeComponent();
        }

        private void ResendVerificationEmailClicked(object sender, RoutedEventArgs e)
        {

        }

        private void StartUsingEagleslistClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void SignUpButtonClicked(object sender, RoutedEventArgs e)
        {
            AttemptRegistration();
        }

        private async void AttemptRegistration()
        {
            RegistrationSubmission submission = new RegistrationSubmission(
                handleField.Text, passwordField.Password, emailField.Text
            );

            User createdUser = await RequestManager.AttemptRegistration(submission);

            Console.WriteLine(createdUser);
        }

        private void CollapseSignInFields()
        {
            TopGrid.Visibility = Visibility.Collapsed;
            BottomGrid.Visibility = Visibility.Visible;
        }
    }
}
