using System;
using System.Collections.ObjectModel;
using System.Windows;
using Humanizer;

namespace Eagleslist
{
    /// <summary>
    /// Interaction logic for AutoFillPrompt.xaml
    /// </summary>
    public partial class AutoFillPrompt : Window
    {
        private ObservableCollection<GoogleBook> _listings = new ObservableCollection<GoogleBook>();

        public AutoFillPrompt()
        {
            InitializeComponent();
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        private async void PerformSearch()
        {
            var results = await RequestManager.GetBooksMatchingTitle(SearchBox.Text);
            Console.WriteLine(results.Count);
            _listings = new ObservableCollection<GoogleBook>(results);
            ResultsListView.ItemsSource = _listings;

            ResultCountTextBlock.Text = "result".ToQuantity(_listings.Count) + " found";
        }
    }
}
