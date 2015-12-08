using System.Collections.ObjectModel;
using System.Windows.Controls;
using Humanizer;
using System.Windows;
using System;
using System.Windows.Threading;
using System.Windows.Media;
using System.Collections.Generic;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for ProfileViewerControl.xaml
    /// </summary>
    public partial class ProfileViewerControl : UserControl, Navigatable
    {
        private MainWindow _mainWindow
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow);
            }
        }

        private ObservableCollection<Listing> _listings = new ObservableCollection<Listing>();
        private Dictionary<string, string> _editDraft = null;

        private User _currentProfileUser;
        internal User CurrentProfileUser
        {
            get
            {
                return _currentProfileUser;
            }

            set
            {
                _currentProfileUser = value;

                ProfileUsername.Text = value?.Handle;
                ProfileBio.Text = value?.Bio;
                ProfileImageURL.Text = value?.ImageURL;

                GetListings();

                var current = CredentialManager.GetCurrentUser();
                if (value != null && current != null && value.Equals(current))
                {
                    EditButton.Visibility = Visibility.Visible;
                }
                else
                {
                    EditButton.Visibility = Visibility.Hidden;
                }
            }
        }

        private async void GetListings()
        {
            var listings = await RequestManager.FetchListingsByUser(CurrentProfileUser);

            if (listings == null)
            {
                _listings = new ObservableCollection<Listing>();
            }
            else
            {
                _listings = new ObservableCollection<Listing>(listings);
            }

            ListingsListView.ItemsSource = _listings;
            ListingsCountTextBlock.Text = "listing".ToQuantity(listings?.Count ?? 0);
        }

        public ProfileViewerControl()
        {
            InitializeComponent();
            tRestoreSaveToNormal.Tick += TRestoreSaveToNormal_Tick;
            tRestoreSaveToNormal.Interval = TimeSpan.FromSeconds(1);
        }

        private void TRestoreSaveToNormal_Tick(object sender, System.EventArgs e)
        {
            SaveButton.Content = "Save";
            tRestoreSaveToNormal.Stop();
        }

        public void RenderObject(object obj)
        {
            CurrentProfileUser = obj as User;
        }


        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditButton.Visibility = Visibility.Hidden;
            EditingContainer.Visibility = Visibility.Visible;
            SetEditingUi();

            _editDraft = new Dictionary<string, string>()
            {
                { "handle", ProfileUsername.Text ?? string.Empty },
                { "url", ProfileImageURL.Text ?? string.Empty },
                { "bio", ProfileBio.Text ?? string.Empty }
            };
        }

        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            EditButton.Visibility = Visibility.Visible;
            EditingContainer.Visibility = Visibility.Hidden;
            SetNotEditingUi();

            ProfileUsername.Text = _editDraft["handle"];
            ProfileImageURL.Text = _editDraft["url"];
            ProfileBio.Text = _editDraft["bio"];

            _editDraft = null;
        }

        private void SetEditingUi()
        {
            var focusable = true;
            var background = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            var thickness = new Thickness(1);

            ProfileUsername.Focusable = focusable;
            ProfileUsername.Background = background;
            ProfileUsername.BorderThickness = thickness;

            ProfileImageURL.Focusable = focusable;
            ProfileImageURL.Background = background;
            ProfileImageURL.BorderThickness = thickness;
            ProfileImageURL.Visibility = Visibility.Visible;

            ProfileBio.Focusable = focusable;
            ProfileBio.Background = background;
            ProfileBio.BorderThickness = thickness;
        }

        private void SetNotEditingUi()
        {
            var focusable = false;
            var background = new SolidColorBrush(Color.FromRgb(0xEC, 0xEE, 0xF3));
            var thickness = new Thickness(0);

            ProfileUsername.Focusable = focusable;
            ProfileUsername.Background = background;
            ProfileUsername.BorderThickness = thickness;

            ProfileImageURL.Focusable = focusable;
            ProfileImageURL.Background = background;
            ProfileImageURL.BorderThickness = thickness;
            ProfileImageURL.Visibility = Visibility.Collapsed;

            ProfileBio.Focusable = focusable;
            ProfileBio.Background = background;
            ProfileBio.BorderThickness = thickness;
        }

        DispatcherTimer tRestoreSaveToNormal = new DispatcherTimer(); 
        // Save the user's profile changes.
        private async void SaveEdit_Click(object sender, RoutedEventArgs e)
        {
            CancelButton.IsEnabled = false;
            SaveButton.Content = "Saving...";
            if (await RequestManager.IsAuthenticated())
            {
                User user = CredentialManager.GetCurrentUser();
                user.Bio = ProfileBio.Text;
                user.Handle = ProfileUsername.Text;
                user.ImageURL = ProfileImageURL.Text;

                string error = (await RequestManager.SaveNewUserInformation(user))?.Error;
                if (!string.IsNullOrEmpty(error))
                {
                    MessageBox.Show("Unable to save the profile information, please try again.");
                }

                CredentialManager.SetCurrentUser(user, CredentialManager.IsPersisting);
                SaveButton.Content = "Saved!";
                _mainWindow.topBar.UpdateProfileUi();

                EditButton.Visibility = Visibility.Visible;
                EditingContainer.Visibility = Visibility.Hidden;

                CancelButton.IsEnabled = true;
                SetNotEditingUi();
                _editDraft = null;
                tRestoreSaveToNormal.Start();
            }
            else
            {
                SaveButton.Content = "Failed to save";
                tRestoreSaveToNormal.Start();
                MessageBox.Show("You're not logged in.");
            }
        }

        private async void DeleteListingButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var listing = button.DataContext as Listing;

            var success = await RequestManager.DeleteListing(listing);

            if (success)
            {
                GetListings();
            }
            else
            {
                MessageBox.Show("Failed to delete listing. Please make sure you're logged in or try again later");
            }
        }
    }
}
