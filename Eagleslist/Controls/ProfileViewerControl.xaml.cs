using System.Collections.ObjectModel;
using System.Windows.Controls;
using Humanizer;

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
                //ProfileEmail.Text = value?.Email;
                ProfileBio.Text = value?.Bio;

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
        }

        public void RenderObject(object obj)
        {
            CurrentProfileUser = obj as User;
        }

        // Save the user's profile changes.
        private async void SaveProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (await RequestManager.IsAuthenticated()) {
                User user = CredentialManager.GetCurrentUser();
                user.Bio = ProfileBio.Text;
                user.Handle = ProfileUsername.Text;
                user.ImageURL = ProfileImageURL.Text;
                string error = (await RequestManager.SaveNewUserInformation(user)).Error;
                if 
            }
        }
    }
}
