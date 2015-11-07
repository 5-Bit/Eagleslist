using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Net.Http;
using System;
using Newtonsoft.Json;

namespace Eagleslist
{
    public partial class LoginPrompt : Window
    {
        public LoginPrompt()
        {
            InitializeComponent();
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    this.DialogResult = true;
        //    Close();
        //}

        private void SignInClicked(object sender, RoutedEventArgs e)
        {
            AttemptLogin(handleField.Text, passwordField.Password);
        }

        private async void AttemptLogin(string handle, string password)
        {
            LoginRequest request = new LoginRequest(handle, password);
            User user = await RequestManager.AttemptLogin(request);
            Console.WriteLine("New user: " + user);

            if (user.AuthError == null || user.AuthError.Length == 0)
            {
                this.DialogResult = true;
                Close();

                MainWindow mainWindow = (MainWindow)Owner;
                mainWindow.currentUser = user;
            }

            Console.WriteLine(user.Handle);
            Console.WriteLine(user.ID);
            Console.WriteLine(user.SessionID);
            Console.WriteLine(user.Email);
        }

        private async void AttemptRegistration()
        {
            //Console.WriteLine("AttemptRegistration");

            //RequestManager manager = new RequestManager();
            //HttpResponseMessage response = await manager.AttemptRegistration(
            //    new RegistrationSubmission(registrationUsernameField.Text, registrationEmailField.Text, registrationPasswordField.Password)
            //);

            //Console.WriteLine(response.StatusCode);
            //if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            //{
            //    return;
            //}

            //string responseString = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseString);
            //AuthResponse deserialized = JsonConvert.DeserializeObject<AuthResponse>(responseString);

            //if (deserialized.Error != null)
            //{
            //    Console.WriteLine(deserialized.Error);
            //    return;
            //}

            //Console.WriteLine(deserialized.SessionID);
            //Console.WriteLine(deserialized.UserID);
        }
    }
}
