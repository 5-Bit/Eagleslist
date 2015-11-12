using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System;
using Humanizer;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;

namespace Eagleslist
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Listing> listings 
            = new ObservableCollection<Listing>();

        private List<Canvas> primaryPanels = new List<Canvas>();
        private List<Canvas> secondaryPanels = new List<Canvas>();
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

        private User currentProfileUser
        {
            set
            {
                ProfileUsername.Text = value.Handle;
                ProfileEmail.Text = value.Email;
                ProfileBio.Text = value.Bio;
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

        private void HideAllContainersExcept(Canvas container)
        {
            if (primaryPanels.Contains(container))
            {
                primaryPanels.ForEach(delegate (Canvas canvas)
                {
                    int index = primaryPanels.IndexOf(canvas);
                    var button = sideBarButtonContainer.Children[index] as Button;

                    if (canvas == container)
                    {
                        (button.Content as DockPanel).Children[0].Visibility = Visibility.Visible;
                        container.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (button != searchButton)
                        {
                            (button.Content as DockPanel).Children[0].Visibility = Visibility.Hidden;
                        }

                        canvas.Visibility = Visibility.Collapsed;
                    }
                });
            }
            else
            {
                primaryPanels.ForEach(delegate (Canvas canvas)
                {
                    int index = primaryPanels.IndexOf(canvas);
                    var button = sideBarButtonContainer.Children[index] as Button;
                    if (button != searchButton)
                    {
                        (button.Content as DockPanel).Children[0].Visibility = Visibility.Hidden;
                    }

                    canvas.Visibility = Visibility.Collapsed;
                });
            }

            secondaryPanels.ForEach(delegate (Canvas canvas)
            {
                if (canvas == container)
                {
                    canvas.Visibility = Visibility.Visible;
                }
                else
                {
                    canvas.Visibility = Visibility.Collapsed;
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
            HideAllContainersExcept(listingsContainer);
            GetNewListings();
        }

        private void ProfileButtonClicked()
        {
            currentProfileUser = CurrentUser;
            HideAllContainersExcept(profileContainer);
        }

        private void MessagesButtonClicked()
        {
            HideAllContainersExcept(messagesContainer);
        }


        private async void GetFakeSearchListings()
        {
            RequestManager manager = new RequestManager();
            List<Listing> newListings = await manager.GetListings();
            List<Listing> tmp = new List<Listing>();
            tmp.Add(newListings[0]);

            listings = new ObservableCollection<Listing>(tmp);
            listingsView.ItemsSource = listings;

            if (listings.Count > 0)
            {
                listingsView.SelectedIndex = 0;
            }
        }

        private async void GetNewListings()
        {
            RequestManager manager = new RequestManager();
            List<Listing> newListings = await manager.GetListings();

            listings = new ObservableCollection<Listing>(newListings);
            listingsView.ItemsSource = listings;

            if (listings.Count > 0)
            {
                listingsView.SelectedIndex = 0;
            }
        }

        private void ListingsViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView selectedList = sender as ListView;

            if (selectedList.SelectedIndex < listings.Count && selectedList.SelectedIndex >= 0)
            {
                Listing selectedListing = listings[selectedList.SelectedIndex];

                listingTitleLabel.Content = selectedListing.Title;
                listingContentTextBlock.Text = selectedListing.Content;
                listingAskingPrice.Content = selectedListing.Price;
                listingConditionLabel.Content = selectedListing.Condition;
                listingTimePostedLabel.Content = HumanizeDateString(selectedListing.CreateDate);

                SetCurrentListingImage(selectedListing);
            }
        }

        private string HumanizeDateString(string input)
        {
            DateTime date = DateTime.Parse(input, null, System.Globalization.DateTimeStyles.RoundtripKind);

            if (date == null)
            {
                return null;
            }

            return date.Humanize();
        }

        private async void SetCurrentListingImage(Listing listing)
        {
            BitmapImage image = null;

            Uri result = null;
            bool success = Uri.TryCreate(listing.ImageURL, UriKind.Absolute, out result) 
                && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);

            if (success)
            {
                image = await RequestManager.GetBitmapFromURI(result);
            }
            else
            {
                image = new BitmapImage(new Uri("pack://application:,,,/images/missing.png"));
            }

            listingImageView.Source = image;
        }

        private void CoursesButtonClicked(object sender, RoutedEventArgs e)
        {
            HideAllContainersExcept(coursesContainer);
        }

        private void MessagesButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        private void SearchSubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            GetFakeSearchListings();
            HideAllContainersExcept(listingsContainer);
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

        private static void ChooseImages()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Eagleslist - Select Images";
            dialog.Multiselect = true;
            dialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";

            bool? x = dialog.ShowDialog();

            if (x.HasValue && x.Value)
            {
                foreach (String file in dialog.FileNames)
                {

                }
            }
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
