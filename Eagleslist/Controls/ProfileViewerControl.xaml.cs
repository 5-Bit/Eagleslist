using System.Collections.ObjectModel;
using System.Windows.Controls;
using Humanizer;
using System.Windows;
using System;
using System.Windows.Threading;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for ProfileViewerControl.xaml
    /// </summary>
    public partial class ProfileViewerControl : UserControl, Navigatable
    {
        private ObservableCollection<Listing> _listings = new ObservableCollection<Listing>();

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

        DispatcherTimer tRestoreSaveToNormal = new DispatcherTimer(); 
        // Save the user's profile changes.
        private async void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            SaveButton.Content = "Saving...";
            if (await RequestManager.IsAuthenticated())
            {
                User user = CredentialManager.GetCurrentUser();
                user.Bio = ProfileBio.Text;
                user.Handle = ProfileUsername.Text;
                user.ImageURL = ProfileImageURL.Text;
                string error = (await RequestManager.SaveNewUserInformation(user))?.Error;
                if (error == null || error != "")
                {
                    MessageBox.Show("Unable to save the profile information, please try again.");
                }
                CredentialManager.SetCurrentUser(user, CredentialManager.IsPersisting);
                SaveButton.Content = "Saved!";
                tRestoreSaveToNormal.Start();
            }
            else {
                MessageBox.Show("You're not logged in.");
            }
        }
    }
}
