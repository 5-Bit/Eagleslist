﻿using Humanizer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for ListingBrowserControl.xaml
    /// </summary>
    public partial class ListingBrowserControl : UserControl
    {
        private ObservableCollection<Listing> listings = new ObservableCollection<Listing>();

        public ListingBrowserControl()
        {
            InitializeComponent();
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

        internal async void GetNewListings()
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
    }
}