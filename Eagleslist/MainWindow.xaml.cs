using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media;

namespace Eagleslist
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //composeContainer.LoginTrigger = () => topBar.ShowLoginDialog();
            //listingsContainer.LoginTrigger = () => topBar.ShowLoginDialog();

            ReloadLoginStateUi();
        }

        internal void RenderNavigationObject(Navigatable obj)
        {
            var element = obj as UIElement;

            if (element == null)
            {
                return;
            }

            for (var index = 0; index < ContainerGrid.Children.Count; index++)
            {
                UIElement child = ContainerGrid.Children[index];

                if (Grid.GetRow(child) == 1 && Grid.GetColumn(child) == 1)
                {
                    ContainerGrid.Children.RemoveAt(index);
                    index--;
                }
            }

            Grid.SetRow(element, 1);
            Grid.SetColumn(element, 1);

            ContainerGrid.Children.Add(element);
        }

        internal void ReloadLoginStateUi()
        {
            if (CredentialManager.UserIsLoggedIn)
            {
                topBar.SetLoggedInUi();
            }
            else
            {
                topBar.SetLoggedOutUi();
            }
        }

        private static string GravatarUrlFromUser(User user, int size)
        {
            const string root = "http://www.gravatar.com/avatar";
            var hash = GravatarMd5StringFromString(user.Email);

            return $"{root}/{hash}?s={size}";
        }

        private static string GravatarMd5StringFromString(string input)
        {
            using (var md5Hash = MD5.Create())
            {
                var hash = GetMd5Hash(md5Hash, input.Trim().ToLower());
                return VerifyMd5Hash(md5Hash, input, hash) ? hash : null;
            }
        }

        private static string GetMd5Hash(HashAlgorithm md5Hash, string input)
        {
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();

            foreach (var b in data)
            {
                sBuilder.Append(b.ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private static bool VerifyMd5Hash(HashAlgorithm md5Hash, string input, string hash)
        {
            var hashOfInput = GetMd5Hash(md5Hash, input);
            var comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }

        internal void SearchButtonClicked(SearchButton button)
        {
            topBar.SearchButtonClicked(button);
        }
    }
}
