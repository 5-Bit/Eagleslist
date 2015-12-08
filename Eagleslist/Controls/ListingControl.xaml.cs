using System.Windows;
using System;
using System.Collections.ObjectModel;
using Humanizer;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for ListingControl.xaml
    /// </summary>
    public partial class ListingControl
    {
        private ObservableCollection<Comment> _comments = new ObservableCollection<Comment>();
        internal Listing Listing
        {
            get
            {
                return _listing;
            }
        }

        private MainWindow _mainWindow
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow);
            }
        }

        private Listing _listing;
        internal Func<bool> LoginTrigger;

        public ListingControl()
        {
            InitializeComponent();
        }

        public void SetListing(Listing listing)
        {
            _listing = listing;
            CommentsListView.ItemsSource = null;
            CurrentListingProgressBar.Visibility = Visibility.Visible;

            UserHandleButton.Content = listing?.Handle;
            ListingTitleLabel.Text = listing?.Title;
            ListingAskingPrice.Content = listing?.Price;
            ListingConditionLabel.Content = listing?.Condition;
            ListingContentTextBlock.Text = listing?.Content;
            ListingTimePostedLabel.Content = HumanizeDateString(listing?.CreateDate.ToString(CultureInfo.InvariantCulture));
            NewCommentTextBox.Text = string.Empty;

            ScrollViewerContainer.ScrollToHome();

            SetCurrentListingImage();
            GetComments();
        }

        private async void GetComments()
        {
            if (_listing == null)
            {
                CurrentListingProgressBar.Visibility = Visibility.Hidden;
                CommentsListView.ItemsSource = null;
                return;
            }

            CurrentListingProgressBar.Visibility = Visibility.Visible;
            var response = await RequestManager.GetCommentsForListing(_listing);

            if (!string.IsNullOrEmpty(response?.Error))
            {
                _comments = null;
            }
            else
            {
                if (response.Comments != null)
                {
                    _comments = new ObservableCollection<Comment>(response.Comments);
                }
                else
                {
                    _comments = null;
                }
            }

            CurrentListingProgressBar.Visibility = Visibility.Hidden;
            CommentsListView.ItemsSource = _comments;
        }

        private static string HumanizeDateString(string input)
        {
            if (input == null)
            {
                return null;
            }

            var date = DateTime.Parse(input, null, DateTimeStyles.RoundtripKind);
            return date.Humanize(false);
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
            if (CredentialManager.GetCurrentUser()?.SessionID != null)
            {
                PostNewComment();
            }
            else
            {
                if (LoginTrigger != null && LoginTrigger())
                {
                    PostNewComment();
                }
            }
        }

        private async void PostNewComment()
        {
            if (_listing == null)
            {
                return;
            }

            var comment = new Comment(-1, -1, null, NewCommentTextBox.Text, _listing.ListingID, DateTime.Now, DateTime.MaxValue.ToUniversalTime());
            ErrorResponse response = await RequestManager.PostNewCommentOnListing(comment, _listing);

            if (response == null || !string.IsNullOrWhiteSpace(response.Error))
            {
                Console.WriteLine(response?.Error);
                // something went wrong
            }
            else
            {
                GetComments();
                NewCommentTextBox.Text = string.Empty;
            }
        }

        private void NewCommentTextBoxTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var max = 250;
            var remainingCharacters = max - NewCommentTextBox.Text.Length;

            if (remainingCharacters >= 0)
            {
                var word = "character".ToQuantity(remainingCharacters);
                NewCommentInfoLabel.Content = $"{word} remaining";
                NewCommentInfoLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
                PostCommentButton.IsEnabled = NewCommentTextBox.Text.Length != 0;
            }
            else
            {
                var word = "character".ToQuantity(Math.Abs(remainingCharacters));
                NewCommentInfoLabel.Content = $"{word} over limit";
                NewCommentInfoLabel.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0000"));
                PostCommentButton.IsEnabled = false;
            }
        }

        internal bool ShouldAllowExit()
        {
            if (string.IsNullOrWhiteSpace(NewCommentTextBox.Text))
            {
                return true;
            }

            const string text = "You are about to navigate away from this listing. Your unsubmitted comment will not be saved. Are you sure you want to proceed?";
            const string caption = "Eagleslist - Unfinished Comment";

            const MessageBoxButton buttons = MessageBoxButton.YesNo;
            const MessageBoxImage icon = MessageBoxImage.Warning;

            var result = MessageBox.Show(text, caption, buttons, icon);

            return result == MessageBoxResult.Yes;
        }

        private void DeleteCommentButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var comment = button.DataContext as Comment;

            if (CredentialManager.GetCurrentUser()?.SessionID != null)
            {
                DeleteComment(comment);
            }
            else
            {
                if (LoginTrigger != null && LoginTrigger())
                {
                    DeleteComment(comment);
                }
            }
        }

        private async void DeleteComment(Comment comment)
        {
            var response = await RequestManager.DeleteComment(comment);
            
            if (response != null)
            {
                if (string.IsNullOrWhiteSpace(response.Error))
                {
                    GetComments();
                }
            }
        }

        private void ListingCreatorProfileClicked(object sender, RoutedEventArgs e)
        {
            var ownerId = _listing?.UserID;

            if (_listing?.UserID != null)
            {
                if (CredentialManager.GetCurrentUser()?.SessionID != null)
                {
                    NavigateToUserId(ownerId.Value);
                }
                else
                {
                    if (LoginTrigger != null && LoginTrigger())
                    {
                        NavigateToUserId(ownerId.Value);
                    }
                }
            }
        }

        private async void NavigateToUserId(int userId)
        {
            var user = await RequestManager.FetchUserById(userId);
            NavigationManager.NavigateFromClick(_mainWindow.topBar.ProfileComboBoxItem, user);
        }

        private void CommentCreatorButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var comment = button.DataContext as Comment;

            if (comment?.UserID != null)
            {
                if (CredentialManager.GetCurrentUser()?.SessionID != null)
                {
                    NavigateToUserId(comment.UserID);
                }
                else
                {
                    if (LoginTrigger != null && LoginTrigger())
                    {
                        NavigateToUserId(comment.UserID);
                    }
                }
            }
        }
    }
}
