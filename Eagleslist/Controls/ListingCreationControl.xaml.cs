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
        internal MainWindow ContainingWindow;
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
            string text = "You are about to post a new listing to Eagleslist. Are you sure that you want to continue?";
            string caption = "Eagleslist - Post New Listing";

            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result = MessageBox.Show(text, caption, buttons, icon);

            if (result == MessageBoxResult.Yes)
            {
                string sessionID = CredentialManager.GetCurrentUser().SessionID;
                string condition = NewListingConditionComboBox.Items[NewListingConditionComboBox.SelectedIndex].ToString();
                Listing listing = new Listing(
                    NewListingTitleBox.Text, NewListingContentBox.Text, null,
                    null, null, null, NewListingISBNBox.Text, 
                    NewListingPriceBox.Text, condition, DateTime.Now, -1, -1
                );

                NewListingResponse response = await RequestManager.PostNewListing(listing, sessionID);

                if (response == null || !string.IsNullOrWhiteSpace(response.Error))
                {
                    Console.WriteLine("failed to post");
                    Console.WriteLine(response.Error);
                }
                else
                {
                    Console.WriteLine("new listing post succeded");
                }
            }
        }

        private static void ChooseImages()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Eagleslist - Select Images";
            dialog.Multiselect = true;
            dialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";

            bool? x = dialog.ShowDialog();

            if (x.HasValue && x.Value)
            {
                foreach (String file in dialog.FileNames)
                {

                }
            }
        }
    }
}
