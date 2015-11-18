using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media;

namespace Eagleslist
{
    public partial class MainWindow : Window
    {
        private List<UserControl> PrimaryPanels = new List<UserControl>();
        private List<UserControl> SecondaryPanels = new List<UserControl>();
        internal List<Button> PrimaryNavigationControls = new List<Button>();

        private LinkedList<object> navigationStack = new LinkedList<object>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            topBar.ContainingWindow = this;
            sideBarButtonContainer.ContainingWindow = this;

            composeContainer.ContainingWindow = this;
            composeContainer.LoginTrigger = () => topBar.ShowLoginDialog();

            if (CredentialManager.UserIsLoggedIn)
            {
                topBar.SetLoggedInUI();
            }
            else
            {
                topBar.SetLoggedOutUI();
            }

            PrimaryPanels.Add(searchContainer);
            PrimaryPanels.Add(composeContainer);
            PrimaryPanels.Add(listingsContainer);
            PrimaryPanels.Add(coursesContainer);

            SecondaryPanels.Add(profileContainer);
            SecondaryPanels.Add(messagesContainer);
        }

        internal void ContainerDisplayPanelAtIndex(int index)
        {
            if (index < PrimaryPanels.Count)
            {
                for (int iterator = 0; iterator < PrimaryPanels.Count; iterator++)
                {
                    PrimaryPanels[iterator].Visibility = index == iterator 
                        ? Visibility.Visible : Visibility.Collapsed;

                    if (iterator != 0)
                    {
                        Button button = PrimaryNavigationControls[iterator];

                        if (iterator == index)
                        {
                            button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECEEF3"));
                        }
                        else
                        {
                            button.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        }
                    }
                }

                for (int iterator = 0; iterator < SecondaryPanels.Count; iterator++)
                {
                    SecondaryPanels[iterator].Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                for (int iterator = 1; iterator < PrimaryPanels.Count; iterator++)
                {
                    Button button = PrimaryNavigationControls[iterator];
                    button.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }

                for (int iterator = 0; iterator < SecondaryPanels.Count; iterator++)
                {
                    if (index - PrimaryPanels.Count == iterator)
                    {
                        SecondaryPanels[iterator].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        SecondaryPanels[iterator].Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        internal void ShowSecondaryPanelAtIndex(int index)
        {
            if (index == 0)
            {
                profileContainer.currentProfileUser = CredentialManager.GetCurrentUser();
            }

            ContainerDisplayPanelAtIndex(PrimaryPanels.Count + index);
        }

        private static string GravatarURLFromUser(User user, int size)
        {
            string root = "http://www.gravatar.com/avatar";
            string hash = GravatarMD5StringFromString(user.Email);

            return string.Format("{0}/{1}?s={2}", root, hash, size);
        }

        private static string GravatarMD5StringFromString(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, input.Trim().ToLower());
                return VerifyMd5Hash(md5Hash, input, hash) ? hash : null;
            }
        }

        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            string hashOfInput = GetMd5Hash(md5Hash, input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}
