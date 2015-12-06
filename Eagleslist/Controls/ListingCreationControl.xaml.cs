using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for ListingCreationControl.xaml
    /// </summary>
    public partial class ListingCreationControl : UserControl, Navigatable
    {
        private MainWindow ContainingWindow
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow);
            }
        }


        private Func<bool> LoginTrigger;
        private ListingCreation _draft = new ListingCreation();

        public ListingCreationControl()
        {
            InitializeComponent();
            NewListingConditionComboBox.ItemsSource = Enum.GetValues(typeof(BookCondition));
        }

        private void NewListingTitleChanged(object sender, RoutedEventArgs e)
        {
            _draft.Title = NewListingTitleBox.Text;
            PostNewListingButton.IsEnabled = _draft.RepresentsValidListing();
        }

        private void NewListingPriceChanged(object sender, RoutedEventArgs e)
        {
            _draft.Price = NewListingPriceBox.Text;
            PostNewListingButton.IsEnabled = _draft.RepresentsValidListing();
        }

        private void NewListingContentChanged(object sender, RoutedEventArgs e)
        {
            _draft.Content = NewListingContentBox.Text;
            PostNewListingButton.IsEnabled = _draft.RepresentsValidListing();
        }

        private void NewListingConditionChanged(object sender, RoutedEventArgs e)
        {
            _draft.Condition = BookConditionMethods.FromInt(NewListingConditionComboBox.SelectedIndex);
            PostNewListingButton.IsEnabled = _draft.RepresentsValidListing();
        }

        private void NewListingIsbnChanged(object sender, RoutedEventArgs e)
        {
            _draft.ISBN = NewListingIsbnBox.Text;
            PostNewListingButton.IsEnabled = _draft.RepresentsValidListing();
        }

        private void NewListingImagesChanged(object sender, RoutedEventArgs e)
        {

        }

        private void CreateListingButtonClicked(object sender, RoutedEventArgs e)
        {
            if (CredentialManager.GetCurrentUser()?.SessionID != null)
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

            var listing = new Listing
            {
                Title = NewListingTitleBox.Text,
                Content = NewListingContentBox.Text,
                ISBN = NewListingIsbnBox.Text,
                Price = NewListingPriceBox.Text,
                Condition = condition,
                CreateDate = DateTime.Now
            };

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

        public void RenderObject(object obj)
        {
            LoginTrigger = () => ContainingWindow.topBar.ShowLoginDialog();
        }

        private void AttemptAutofillButtonClicked(object sender, RoutedEventArgs e)
        {
            if (ShowAutoFill())
            {

            }
            else
            {

            }
        }

        private bool ShowAutoFill()
        {
            AutoFillPrompt prompt = new AutoFillPrompt();
            prompt.Owner = ContainingWindow;

            bool? success = prompt.ShowDialog();
            return success.HasValue && success.Value;
        }
    }
}
