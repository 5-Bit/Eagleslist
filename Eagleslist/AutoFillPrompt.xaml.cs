using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using Humanizer;
using System.Windows.Controls;

namespace Eagleslist
{
    /// <summary>
    /// Interaction logic for AutoFillPrompt.xaml
    /// </summary>
    public partial class AutoFillPrompt : Window
    {
        private ObservableCollection<GoogleBook> _listings = new ObservableCollection<GoogleBook>();
        public Action<GoogleBook> result;

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

        private void SelectBookButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var book = button.DataContext as GoogleBook;

            Console.WriteLine(book.volumeInfo.description);
            Console.WriteLine(book.ISBN);
            Console.WriteLine(book.Title);
            Console.WriteLine(book.volumeInfo.imageLinks.smallThumbnail);

            if (result != null)
            {
                result(book);
            }

            Close();
        }
    }
}
