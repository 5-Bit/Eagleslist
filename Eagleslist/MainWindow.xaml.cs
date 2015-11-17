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
        private List<UserControl> primaryPanels = new List<UserControl>();
        private List<UserControl> secondaryPanels = new List<UserControl>();
        private LinkedList<object> navigationStack = new LinkedList<object>();

        internal User CurrentUser
        {
            get
            {
                return CredentialManager.getCurrentUser();
            }

            set
            {
                CredentialManager.setCurrentUser(value); // UI changes below dependent on this.
                accountComboBox.IsDropDownOpen = false;

                if (value != null)
                {
                    SetLoggedInUI();
                }
                else
                {
                    SetLoggedOutUI();
                }
            }
        }

        private bool userIsLoggedIn
        {
            get
            {
                return CurrentUser != null;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (userIsLoggedIn)
            {
                SetLoggedInUI();
            }
            else
            {
                SetLoggedOutUI();
            }

            primaryPanels.Add(searchContainer);
            primaryPanels.Add(composeContainer);
            primaryPanels.Add(listingsContainer);
            primaryPanels.Add(coursesContainer);

            secondaryPanels.Add(profileContainer);
            secondaryPanels.Add(messagesContainer);

            HideAllContainersExcept(searchContainer);

        }

        private void SetLoggedInUI()
        {
            accountOverlayButton.Content = CurrentUser.Handle;
            ToggleVisibleAccountComboBoxItems();
        }

        private void SetLoggedOutUI()
        {
            accountOverlayButton.Content = "Account";
            ToggleVisibleAccountComboBoxItems();
        }

        private void ToggleVisibleAccountComboBoxItems()
        {
            string tag = userIsLoggedIn ? "LoggedIn" : "LoggedOut";

            foreach (ComboBoxItem item in accountComboBox.Items)
            {
                if (item.Tag.Equals(tag))
                {
                    item.Visibility = Visibility.Visible;
                }
                else
                {
                    item.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void HideAllContainersExcept(UserControl container)
        {
            if (primaryPanels.Contains(container))
            {
                primaryPanels.ForEach(delegate (UserControl canvas)
                {
                    int index = primaryPanels.IndexOf(canvas);
                    var button = sideBarButtonContainer.Children[index] as Button;

                    if (canvas == container)
                    {
                        if (button != searchButton)
                        {
                            button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECEEF3"));
                        }

                        //(button.Content as DockPanel).Children[0].Visibility = Visibility.Visible;
                        container.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (button != searchButton)
                        {
                            //(button.Content as DockPanel).Children[0].Visibility = Visibility.Hidden;
                            button.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        }

                        canvas.Visibility = Visibility.Collapsed;
                    }
                });
            }
            else
            {
                primaryPanels.ForEach(delegate (UserControl control)
                {
                    int index = primaryPanels.IndexOf(control);
                    var button = sideBarButtonContainer.Children[index] as Button;
                    if (button != searchButton)
                    {
                        //(button.Content as DockPanel).Children[0].Visibility = Visibility.Hidden;
                    }

                    button.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    control.Visibility = Visibility.Collapsed;
                });
            }

            secondaryPanels.ForEach(delegate (UserControl control)
            {
                if (control == container)
                {
                    control.Visibility = Visibility.Visible;
                }
                else
                {
                    control.Visibility = Visibility.Collapsed;
                }
            });
        }

        private void ComposeButtonClicked(object sender, RoutedEventArgs e)
        {
            HideAllContainersExcept(composeContainer);
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            HideAllContainersExcept(searchContainer);
        }

        private void ListingsButtonClicked(object sender, RoutedEventArgs e)
        {
            listingsContainer.GetNewListings();
            HideAllContainersExcept(listingsContainer);
        }

        private void ProfileButtonClicked()
        {
            profileContainer.currentProfileUser = CurrentUser;
            HideAllContainersExcept(profileContainer);
        }

        private void MessagesButtonClicked()
        {
            HideAllContainersExcept(messagesContainer);
        }

        private void CoursesButtonClicked(object sender, RoutedEventArgs e)
        {
            HideAllContainersExcept(coursesContainer);
        }

        private void MessagesButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        private void accountDropDownClicked(object sender, RoutedEventArgs e)
        {
            accountComboBox.IsDropDownOpen = !accountComboBox.IsDropDownOpen;
        }

        private void accountComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (accountComboBox.SelectedIndex)
            {
                case 0:
                    ShowLoginDialog();
                    break;
                case 1:
                    ShowSignUpDialog();
                    break;
                case 2:
                    ProfileButtonClicked();
                    break;
                case 3:
                    MessagesButtonClicked();
                    break;
                case 4:
                    ShowSignOutDialog();
                    break;
                default:
                    break;
            }

            accountComboBox.IsDropDownOpen = false;
            accountComboBox.SelectedIndex = -1;
        }

        private void ShowSignOutDialog()
        {
            string text = "Are you sure you want to sign out?";
            string caption = "Eagleslist - Sign Out";

            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(text, caption, buttons, icon);

            if (result == MessageBoxResult.Yes)
            {
                if (CurrentUser.SessionID != null)
                {
                    string sessionID = String.Copy(CurrentUser.SessionID);
                    RequestManager.AttemptLogout(sessionID);
                }

                CurrentUser = null;
            }
        }

        private void ShowLoginDialog()
        {
            LoginPrompt prompt = new LoginPrompt();
            prompt.Owner = this;

            bool? _ = prompt.ShowDialog();
        }

        private void ShowSignUpDialog()
        {
            SignUpPrompt prompt = new SignUpPrompt();
            prompt.Owner = this;

            bool? _ = prompt.ShowDialog();
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

        private void NavigationBackButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        private void NavigationForwardButtonClicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
