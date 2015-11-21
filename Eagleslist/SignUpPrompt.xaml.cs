using System;
using System.Collections.Generic;
using System.Windows;

namespace Eagleslist
{
    public partial class SignUpPrompt
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
            ProgressBar.Visibility = Visibility.Visible;
            AttemptRegistration();
        }

        private async void AttemptRegistration()
        {
            var submission = new RegistrationSubmission(
                handleField.Text, passwordField.Password, emailField.Text
            );

            var user = await RequestManager.AttemptRegistration(submission);
            ProgressBar.Visibility = Visibility.Collapsed;

            if (!string.IsNullOrEmpty(user?.AuthError)) return;

            CredentialManager.SetCurrentUser(user, false);
            var window = (MainWindow)Owner;
            window.ReloadLoginStateUi();
            CollapseSignInFields();
        }

        private void CollapseSignInFields()
        {
            TopGrid.Visibility = Visibility.Collapsed;
            BottomGrid.Visibility = Visibility.Visible;
        }

        private void UserInputChanged(object sender, RoutedEventArgs e)
        {
            var errors = Validate();

            if (errors.Count == 0)
            {
                InputErrorTextBox.Height = 0;
                InputErrorTextBox.Text = null;
                SignUpButton.IsEnabled = true;
            }
            else
            {
                SignUpButton.IsEnabled = false;
                InputErrorTextBox.Height = double.NaN;
                InputErrorTextBox.Text = string.Join<string>("\n\n", errors);
            }
        }

        private List<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(emailField.Text.Trim()))
            {
                errors.Add("Email field can not be empty");
            }
            else
            {
                //Regex pattern = new Regex("^[a-zA-Z]+[0-9]*@(eagle.)?fgcu.edu$");
                //bool isEmailValid = emailField.Text != null && pattern.IsMatch(emailField.Text);
                var isEmailValid = emailField.Text.EndsWith("@eagle.fgcu.edu")
                    || emailField.Text.EndsWith("@fgcu.edu");
                if (!isEmailValid)
                {
                    errors.Add("Email must be a valid FGCU email address");
                }
            }

            if (string.IsNullOrEmpty(handleField.Text.Trim()))
            {
                errors.Add("Username field can not be empty");
            }

            if (string.IsNullOrEmpty(passwordField.Password.Trim())
                || string.IsNullOrEmpty(confirmPasswordField.Password.Trim()))
            {
                errors.Add("Password fields can not be empty");
            }
            else
            {
                if (!passwordField.Password.Equals(confirmPasswordField.Password))
                {
                    errors.Add("Passwords do not match");
                }
            }

            return errors;
        }
    }
}
