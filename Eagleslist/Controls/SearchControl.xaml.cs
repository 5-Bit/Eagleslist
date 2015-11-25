using System;
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
                _results = null;
                ResultsListView.ItemsSource = _results;
                return;
            }

            var results = await RequestManager.SearchForText(text);

            if (results != null && results.Count != 0)
            {
                _results = new ObservableCollection<Listing>(results);
                ResultsListView.ItemsSource = _results;
            }
        }

        private void SearchSubmitButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        public void RenderObject(object obj)
        {
            var listings = obj as List<Listing>;
        }

        private void ResultsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
