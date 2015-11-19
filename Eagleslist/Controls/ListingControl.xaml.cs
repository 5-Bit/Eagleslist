using System.Windows.Controls;
using System.Windows;
using System;
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
        public ListingControl()
        {
            InitializeComponent();
        }

        public void SetListing(Listing listing)
        {
            Visibility = listing == null ? Visibility.Collapsed : Visibility.Visible;

            listingTitleLabel.Content = listing?.Title;
            listingAskingPrice.Content = listing?.Price;
            listingConditionLabel.Content = listing?.Condition;
            listingContentTextBlock.Text = listing?.Content;
            listingTimePostedLabel.Content = HumanizeDateString(listing?.CreateDate.ToString(CultureInfo.InvariantCulture));
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
            BitmapImage image;

            Uri result;
            var success = Uri.TryCreate(listing.ImageURL, UriKind.Absolute, out result)
                && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);

            if (success)
            {
                image = await RequestManager.GetBitmapFromUri(result);
            }
            else
            {
                image = new BitmapImage(new Uri("pack://application:,,,/images/missing.png"));
            }

            listingImageView.Source = image;
        }
    }
}
