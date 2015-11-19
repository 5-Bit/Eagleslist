using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for ListingCreationControl.xaml
    /// </summary>
    public partial class ListingCreationControl : UserControl
    {
        internal Func<bool> LoginTrigger;
        private ListingCreation Draft = new ListingCreation();

        public ListingCreationControl()
        {
            InitializeComponent();
            NewListingConditionComboBox.ItemsSource = Enum.GetValues(typeof(BookCondition));
        }

        private void NewListingTitleChanged(object sender, RoutedEventArgs e)
        {
            Draft.Title = NewListingTitleBox.Text;
            PostNewListingButton.IsEnabled = Draft.RepresentsValidListing();
        }

        private void NewListingPriceChanged(object sender, RoutedEventArgs e)
        {
            Draft.Price = NewListingPriceBox.Text;
            PostNewListingButton.IsEnabled = Draft.RepresentsValidListing();
        }

        private void NewListingContentChanged(object sender, RoutedEventArgs e)
        {
            Draft.Content = NewListingContentBox.Text;
            PostNewListingButton.IsEnabled = Draft.RepresentsValidListing();
        }

        private void NewListingConditionChanged(object sender, RoutedEventArgs e)
        {
            Draft.Condition = BookConditionMethods.FromInt(NewListingConditionComboBox.SelectedIndex);
            PostNewListingButton.IsEnabled = Draft.RepresentsValidListing();
        }

        private void NewListingISBNChanged(object sender, RoutedEventArgs e)
        {
            Draft.ISBN = NewListingISBNBox.Text;
            PostNewListingButton.IsEnabled = Draft.RepresentsValidListing();
        }

        private void NewListingImagesChanged(object sender, RoutedEventArgs e)
        {

        }

        private void CreateListingButtonClicked(object sender, RoutedEventArgs e)
        {
            if (CredentialManager.GetCurrentUser().SessionID != null)
            {
                ShowConfirmPostDialog();
            } 
            else
            {
                if (LoginTrigger != null && LoginTrigger())
                {
                    ShowConfirmPostDialog();
                }
            }
        }

        private void VisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {

            }
            else
            {

            }
        }

        private async void ShowConfirmPostDialog()
        {
            const string text = "You are about to post a new listing to Eagleslist. Are you sure that you want to continue?";
            const string caption = "Eagleslist - Post New Listing";

            const MessageBoxButton buttons = MessageBoxButton.YesNo;
            const MessageBoxImage icon = MessageBoxImage.Question;

            var result = MessageBox.Show(text, caption, buttons, icon);

            if (result != MessageBoxResult.Yes) return;

            var sessionId = CredentialManager.GetCurrentUser().SessionID;
            var condition = NewListingConditionComboBox.Items[NewListingConditionComboBox.SelectedIndex].ToString();
            var listing = new Listing(
                NewListingTitleBox.Text, NewListingContentBox.Text, null,
                null, null, null, NewListingISBNBox.Text, 
                NewListingPriceBox.Text, condition, DateTime.Now, -1, -1
            );

            var response = await RequestManager.PostNewListing(listing, sessionId);

            if (response == null || !string.IsNullOrWhiteSpace(response.Error))
            {
                ShowListingCreationFailedDialog();
                Console.WriteLine("failed to post");
                Console.WriteLine(response.Error);
            }
            else
            {
                Console.WriteLine("new listing post succeded");
            }
        }

        private static void ShowListingCreationFailedDialog()
        {
            const string text = @"Looks like something went wrong. Please try posting this listing again later.
                                  If it still doesn't work, please let us know by emailing us at help@5BitStudios.com.
                                  Please note that a draft of your post has been saved, so you won't have to retype it later.";
            const string caption = "Eagleslist - Post New Listing Failed";

            MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void ChooseImages()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Eagleslist - Select Images",
                Multiselect = true,
                Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg"
            };

            var x = dialog.ShowDialog();

            if (!x.HasValue || !x.Value) return;

            foreach (var file in dialog.FileNames)
            {

            }
        }
    }
}
