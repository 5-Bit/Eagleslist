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
            SetCurrentListingImage(listing);
            CommentsSectionGrid.Visibility = Visibility.Collapsed;
            GetComments(listing);
        }

        private async void GetComments(Listing listing)
        {
            var session = CredentialManager.GetCurrentUser()?.SessionID;
            if (listing == null || session == null)
            {
                return;
            }

            var comments = await RequestManager.GetCommentsForListing(listing, session);
            _comments = new ObservableCollection<Comment>(comments);
            CommentsListView.ItemsSource = _comments;

            CommentsSectionGrid.Visibility = comments?.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
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

        private async void SetCurrentListingImage(Listing listing)
        {
            if (listing == null)
            {
                SetDefaultImage();
                return;
            }

            Uri result;
            var success = Uri.TryCreate(listing.ImageURL, UriKind.Absolute, out result)
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
    }
}
