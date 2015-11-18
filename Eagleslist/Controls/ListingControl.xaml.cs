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
    public partial class ListingControl : UserControl
    {
        public ListingControl()
        {
            InitializeComponent();
        }

        public void SetListing(Listing listing)
        {
            if (listing == null)
            {
                Visibility = Visibility.Collapsed;
            }
            else
            {
                Visibility = Visibility.Visible;
            }

            listingTitleLabel.Content = listing?.Title;
            listingAskingPrice.Content = listing?.Price;
            listingConditionLabel.Content = listing?.Condition;
            listingContentTextBlock.Text = listing?.Content;
            listingTimePostedLabel.Content = HumanizeDateString(listing?.CreateDate.ToString());
        }

        private string HumanizeDateString(string input)
        {
            if (input == null)
            {
                return null;
            }

            DateTime date = DateTime.Parse(input, null, DateTimeStyles.RoundtripKind);

            if (date == null)
            {
                return null;
            }

            return date.Humanize();
        }

        private async void SetCurrentListingImage(Listing listing)
        {
            BitmapImage image = null;

            Uri result = null;
            bool success = Uri.TryCreate(listing.ImageURL, UriKind.Absolute, out result)
                && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);

            if (success)
            {
                image = await RequestManager.GetBitmapFromURI(result);
            }
            else
            {
                image = new BitmapImage(new Uri("pack://application:,,,/images/missing.png"));
            }

            listingImageView.Source = image;
        }
    }
}
