using System.Windows;
using System;
using System.Collections.ObjectModel;
using Humanizer;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for ListingControl.xaml
    /// </summary>
    public partial class ListingControl
    {
        private ObservableCollection<Comment> _comments = new ObservableCollection<Comment>();
        private Listing _listing;

        public ListingControl()
        {
            InitializeComponent();
        }

        public void SetListing(Listing listing)
        {
            Visibility = listing == null ? Visibility.Collapsed : Visibility.Visible;

            ListingTitleLabel.Content = listing?.Title;
            ListingAskingPrice.Content = listing?.Price;
            ListingConditionLabel.Content = listing?.Condition;
            ListingContentTextBlock.Text = listing?.Content;
            ListingTimePostedLabel.Content = HumanizeDateString(listing?.CreateDate.ToString(CultureInfo.InvariantCulture));

            _listing = listing;

            SetCurrentListingImage();
            GetComments();
        }

        private async void GetComments()
        {
            if (_listing == null)
            {
                return;
            }

            var comments = await RequestManager.GetCommentsForListing(_listing);
            _comments = new ObservableCollection<Comment>(comments);
            CommentsListView.ItemsSource = _comments;
        }

        private static string HumanizeDateString(string input)
        {
            if (input == null)
            {
                return null;
            }

            var date = DateTime.Parse(input, null, DateTimeStyles.RoundtripKind);

            return date.Humanize();
        }

        private async void SetCurrentListingImage()
        {
            if (_listing == null || _listing.ImageURL == null)
            {
                SetDefaultImage();
                return;
            }

            Uri result;
            var success = Uri.TryCreate(_listing.ImageURL, UriKind.Absolute, out result)
                && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);

            if (success)
            {
                ListingImageView.Source = await RequestManager.GetBitmapFromUri(result);
            }
            else
            {
                SetDefaultImage();
            }
        }

        private void SetDefaultImage()
        {
            ListingImageView.Source = new BitmapImage(new Uri("pack://application:,,,/images/missing.png"));
        }

        private void PostCommentButtonClicked(object sender, RoutedEventArgs e)
        {
            PostNewComment();
        }

        private async void PostNewComment()
        {
            if (_listing == null)
            {
                return;
            }
            Console.WriteLine("Current listing ID: " + _listing.ListingID);
            var comment = new Comment()
            {
                Content = NewCommentTextBox.Text,
                ParentListingID = _listing.ListingID,
                CreateDate = DateTime.Now,
                EndDate = DateTime.MinValue
            };

            CommentCreationResponse response = await RequestManager.PostNewCommentOnListing(comment, _listing);

            if (response == null || !string.IsNullOrWhiteSpace(response.Error))
            {
                Console.WriteLine(response?.Error);
                // something went wrong
            }
            else
            {
                Console.WriteLine("Posting new comment");
                GetComments();
                NewCommentTextBox.Text = string.Empty;
            }
        }
    }
}
