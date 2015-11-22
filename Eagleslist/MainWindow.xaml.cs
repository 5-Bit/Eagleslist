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
        private readonly List<UserControl> _primaryPanels = new List<UserControl>();
        private readonly List<UserControl> _secondaryPanels = new List<UserControl>();
        internal List<Button> PrimaryNavigationControls = new List<Button>();

        private LinkedList<object> _navigationStack = new LinkedList<object>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            topBar.ContainingWindow = this;
            sideBarButtonContainer.ContainingWindow = this;

            composeContainer.LoginTrigger = () => topBar.ShowLoginDialog();
            listingsContainer.LoginTrigger = () => topBar.ShowLoginDialog();

            ReloadLoginStateUi();

            _primaryPanels.Add(searchContainer);
            _primaryPanels.Add(composeContainer);
            _primaryPanels.Add(listingsContainer);
            _primaryPanels.Add(coursesContainer);

            _secondaryPanels.Add(profileContainer);
            _secondaryPanels.Add(messagesContainer);
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

        internal void ContainerDisplayPanelAtIndex(int index)
        {
            if (index < _primaryPanels.Count)
            {
                for (var iterator = 0; iterator < _primaryPanels.Count; iterator++)
                {
                    _primaryPanels[iterator].Visibility = index == iterator 
                        ? Visibility.Visible : Visibility.Collapsed;

                    if (iterator == 0) continue;
                    var button = PrimaryNavigationControls[iterator];

                    button.Background = iterator == index 
                        ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECEEF3")) 
                        : new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }

                foreach (var control in _secondaryPanels)
                {
                    control.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                for (var iterator = 0; iterator < _primaryPanels.Count; iterator++)
                {
                    if (iterator != 0)
                    {
                        var button = PrimaryNavigationControls[iterator];
                        button.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    }

                    _primaryPanels[iterator].Visibility = Visibility.Collapsed;
                }

                for (var iterator = 0; iterator < _secondaryPanels.Count; iterator++)
                {
                    _secondaryPanels[iterator].Visibility = index - _primaryPanels.Count == iterator ?
                        Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        internal void ShowSecondaryPanelAtIndex(int index)
        {
            if (index == 0)
            {
                profileContainer.CurrentProfileUser = CredentialManager.GetCurrentUser();
            }

            ContainerDisplayPanelAtIndex(_primaryPanels.Count + index);
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
