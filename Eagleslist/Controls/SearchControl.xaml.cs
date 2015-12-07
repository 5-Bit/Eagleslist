using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;


namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControl : UserControl, Navigatable
    {
        private ObservableCollection<Listing> _results = new ObservableCollection<Listing>();

        public SearchControl()
        {
            InitializeComponent();
        }

        public void SearchBoxUpdatedText(string text)
        {
            GetSearchResultsForText(text);
        }

        private async void GetSearchResultsForText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                ResultsListView.ItemsSource = null;
                return;
            }

            var results = await RequestManager.SearchForText(text);

            if (results != null && results.Count != 0)
            {
                _results = new ObservableCollection<Listing>(results);
                ResultsListView.ItemsSource = _results;
            }
        }

        public void RenderObject(object obj)
        {
            var listings = obj as List<Listing>;
        }

        private void ResultsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedList = sender as ListView;

            if (selectedList.SelectedIndex < _results.Count 
                && selectedList.SelectedIndex >= 0
                && CurrentListing?.Listing != null
                && _results != null
                && _results[selectedList.SelectedIndex].Equals(CurrentListing.Listing))
            {
                CurrentListing.Visibility = Visibility.Hidden;
                return;
            }

            if (CurrentListing.ShouldAllowExit())
            {
                if (selectedList != null && selectedList.SelectedIndex < _results.Count && selectedList.SelectedIndex >= 0)
                {
                    CurrentListing.Visibility = Visibility.Visible;
                    CurrentListing.SetListing(_results[selectedList.SelectedIndex]);
                }
                else
                {
                    CurrentListing.Visibility = Visibility.Hidden;
                    CurrentListing.SetListing(null);
                }
            }
            else
            {
                selectedList.SelectedIndex = _results?.IndexOf(CurrentListing.Listing) ?? 0;
            }
        }
    }
}
