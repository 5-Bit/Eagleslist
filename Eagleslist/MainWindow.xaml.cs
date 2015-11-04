using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System;
using Humanizer;
using System.Windows.Media;

namespace Eagleslist
{
    public partial class MainWindow : Window
    {
        ObservableCollection<Listing> listings = new ObservableCollection<Listing>();
        List<Canvas> primaryPanels = new List<Canvas>();

        public MainWindow()
        {
            InitializeComponent();

            primaryPanels.Add(composeContainer);
            primaryPanels.Add(searchContainer);
            primaryPanels.Add(listingsContainer);
            primaryPanels.Add(coursesContainer);

            HideAllContainersExcept(composeContainer);
        }

        private void HideAllContainersExcept(Canvas container)
        {
            foreach (Canvas canvas in primaryPanels)
            {
                // 218
                var index = primaryPanels.IndexOf(canvas);
                var button = sideBarButtonContainer.Children[index] as Button;

                if (canvas == container)
                {
                    (button.Content as DockPanel).Children[0].Visibility = Visibility.Visible;
                    //button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DADADA"));
                    container.Visibility = Visibility.Visible;
                }
                else
                {
                    (button.Content as DockPanel).Children[0].Visibility = Visibility.Hidden;
                    //button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECECEC"));
                    canvas.Visibility = Visibility.Collapsed;
                }
            }
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

        private void ProfileButtonClicked(object sender, RoutedEventArgs e)
        {
            LoginPrompt prompt = new LoginPrompt();
            prompt.Owner = this;

            bool? success = prompt.ShowDialog();

            Console.WriteLine(success.HasValue);
            Console.WriteLine(success.Value);

            if (success.HasValue && success.Value)
            {
                //profileLabel.Content = "0x7FFFFFFF";

                //var bitmap = new BitmapImage(new Uri("pack://application:,,,/images/mick.png"));
                //profileImage.Source = bitmap;
            }
        }

        private void SearchSubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            GetFakeSearchListings();
            HideAllContainersExcept(listingsContainer);
        }
    }
}
