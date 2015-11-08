using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System;
using Humanizer;

namespace Eagleslist
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Listing> listings 
            = new ObservableCollection<Listing>();

        private List<Canvas> primaryPanels = new List<Canvas>();
        private List<Canvas> secondaryPanels = new List<Canvas>();

        internal User currentUser
        {
            get
            {
                return CredentialManager.getCurrentUser();
            }

            set
            {
                CredentialManager.setCurrentUser(value); // UI changes below dependent on this.

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
                return currentUser != null;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

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

            HideAllContainersExcept(searchContainer);
        }

        private void SetLoggedInUI()
        {
            Console.WriteLine("SetLoggedInUI");
            accountOverlayButton.Content = currentUser.Handle;
            ToggleVisibleAccountComboBoxItems();
        }

        private void SetLoggedOutUI()
        {
            Console.WriteLine("SetLoggedOutUI");
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
                    var index = primaryPanels.IndexOf(canvas);
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
                    canvas.Visibility = Visibility.Collapsed;
                });
            }

            secondaryPanels.ForEach(delegate (Canvas canvas)
            {
                canvas.Visibility = Visibility.Collapsed;
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
                    break;
                case 3:
                    break;
                case 4:
                    currentUser = null;
                    break;
                default:
                    break;
            }

            accountComboBox.IsDropDownOpen = false;
            accountComboBox.SelectedIndex = -1;
        }

        private void ShowLoginDialog()
        {
            LoginPrompt prompt = new LoginPrompt();
            prompt.Owner = this;

            bool? x = prompt.ShowDialog();
        }

        private void ShowSignUpDialog()
        {
            SignUpPrompt prompt = new SignUpPrompt();
            prompt.Owner = this;

            bool? x = prompt.ShowDialog();
        }
    }
}
