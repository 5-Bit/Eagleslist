using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for ListingBrowserControl.xaml
    /// </summary>
    public partial class ListingBrowserControl : Navigatable
    {
        private MainWindow ContainingWindow
        {
            get
            {
                return (MainWindow)Application.Current.MainWindow;
            }
        }

        private ObservableCollection<Listing> _listings = new ObservableCollection<Listing>();
        private Func<bool> LoginTrigger {
            set
            {
                CurrentListing.LoginTrigger = value;
            }
        }

        public ListingBrowserControl()
        {
            InitializeComponent();
        }

        public void RenderObject(object obj)
        {
            LoginTrigger = () => ContainingWindow.topBar.ShowLoginDialog();
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

            if (selectedList.SelectedIndex < _listings.Count && selectedList.SelectedIndex >= 0
                && CurrentListing.Listing != null
                && _listings[selectedList.SelectedIndex].Equals(CurrentListing.Listing))
            {
                return;
            }

            if (CurrentListing.ShouldAllowExit())
            {
                if (selectedList != null && selectedList.SelectedIndex < _listings.Count && selectedList.SelectedIndex >= 0)
                {
                    CurrentListing.SetListing(_listings[selectedList.SelectedIndex]);
                }
                else
                {
                    CurrentListing.SetListing(null);
                }
            }
            else
            {
                selectedList.SelectedIndex = _listings.IndexOf(CurrentListing.Listing);
            }
        }

        private async void GetNewListings()
        {
            ListingsProgressBar.Visibility = Visibility.Visible;
            var manager = new RequestManager();
            var newListings = await manager.GetListings();

            if (newListings != null)
            {
                _listings = new ObservableCollection<Listing>(newListings);
            }

            ListingsView.ItemsSource = _listings;
            ListingsProgressBar.Visibility = Visibility.Collapsed;

            if (_listings.Count > 0)
            {
                ListingsView.SelectedIndex = 0;
            }
        }
    }
}
