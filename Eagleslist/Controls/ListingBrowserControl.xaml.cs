using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

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

        private void VisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                GetNewListings();
            }
        }

        private void ListingsViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView selectedList = sender as ListView;

            if (selectedList.SelectedIndex < listings.Count && selectedList.SelectedIndex >= 0)
            {
                CurrentListing.SetListing(listings[selectedList.SelectedIndex]);
            }
            else
            {
                CurrentListing.SetListing(null);
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
    }
}
