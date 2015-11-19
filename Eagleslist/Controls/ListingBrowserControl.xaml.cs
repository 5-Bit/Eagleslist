using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for ListingBrowserControl.xaml
    /// </summary>
    public partial class ListingBrowserControl
    {
        private ObservableCollection<Listing> _listings = new ObservableCollection<Listing>();

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
            var selectedList = sender as ListView;

            if (selectedList != null && selectedList.SelectedIndex < _listings.Count && selectedList.SelectedIndex >= 0)
            {
                CurrentListing.SetListing(_listings[selectedList.SelectedIndex]);
            }
            else
            {
                CurrentListing.SetListing(null);
            }
        }

        private async void GetNewListings()
        {
            var manager = new RequestManager();
            var newListings = await manager.GetListings();

            _listings = new ObservableCollection<Listing>(newListings);
            ListingsView.ItemsSource = _listings;

            if (_listings.Count > 0)
            {
                ListingsView.SelectedIndex = 0;
            }
        }
    }
}
