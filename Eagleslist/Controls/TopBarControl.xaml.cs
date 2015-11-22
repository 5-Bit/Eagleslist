using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for TopBarControl.xaml
    /// </summary>
    public partial class TopBarControl : UserControl
    {
        internal MainWindow ContainingWindow;
        private SearchButton _searchButton;

        public TopBarControl()
        {
            InitializeComponent();
        }

        internal void SetLoggedInUi()
        {
            AccountOverlayButton.Content = CredentialManager.GetCurrentUser()?.Handle;
            ToggleVisibleAccountComboBoxItems();
        }

        internal void SetLoggedOutUi()
        {
            AccountOverlayButton.Content = "Account";
            ToggleVisibleAccountComboBoxItems();
        }

        internal void SearchButtonClicked(SearchButton button)
        {
            _searchButton = button;
            ToggleSearchUI();
            button.isSelected = !button.isSelected;
        }

        private void ToggleSearchUI()
        {
            //var to = (Color)ColorConverter.ConvertFromString(value ? "#006F41" : "#00885A");
            //var from = (Color)ColorConverter.ConvertFromString(value ? "#00885A" : "#006F41");

            //ColorAnimation animation = new ColorAnimation()
            //{
            //    To = to,
            //    From = from,
            //    Duration = TimeSpan.FromSeconds(0.15)
            //};

            //Background.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            var from = 0.0;
            var to = 0.0;

            if (SearchContainer.Width == SearchContainer.MaxWidth)
            {
                from = SearchContainer.MaxWidth;
                to = SearchContainer.MinWidth;
            }
            else
            {
                from = SearchContainer.MinWidth;
                to = SearchContainer.MaxWidth;
            }

            DoubleAnimation widthAnimation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(0.15)
            };

            widthAnimation.Completed += (object sender, EventArgs e) =>
            {
                if (_searchButton.isSelected)
                {
                    SearchBox.Focus();
                }
            };

            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(Grid.WidthProperty));
            Storyboard.SetTarget(widthAnimation, SearchContainer);

            Storyboard s = new Storyboard();
            s.Children.Add(widthAnimation);
            s.Begin();
        }

        private void ToggleVisibleAccountComboBoxItems()
        {
            string tag = CredentialManager.UserIsLoggedIn ? "LoggedIn" : "LoggedOut";

            foreach (ComboBoxItem item in AccountComboBox.Items)
            {
                if (item.Tag.Equals(tag))
                {
                    item.Visibility = Visibility.Visible;
                }
                else
                {
                    item.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ProfileButtonClicked()
        {
            ContainingWindow.ShowSecondaryPanelAtIndex(0);
        }

        private void MessagesButtonClicked()
        {
            ContainingWindow.ShowSecondaryPanelAtIndex(1);
        }

        private void AccountDropDownClicked(object sender, RoutedEventArgs e)
        {
            AccountComboBox.IsDropDownOpen = !AccountComboBox.IsDropDownOpen;
        }

        private void AccountComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (AccountComboBox.SelectedIndex)
            {
                case 0:
                    ShowLoginDialog();
                    break;
                case 1:
                    ShowSignUpDialog();
                    break;
                case 2:
                    ProfileButtonClicked();
                    break;
                case 3:
                    MessagesButtonClicked();
                    break;
                case 4:
                    ShowSignOutDialog();
                    break;
                default:
                    break;
            }

            AccountComboBox.IsDropDownOpen = false;
            AccountComboBox.SelectedIndex = -1;
        }

        private void ShowSignOutDialog()
        {
            string text = "Are you sure you want to sign out?";
            string caption = "Eagleslist - Sign Out";

            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(text, caption, buttons, icon);

            if (result == MessageBoxResult.Yes)
            {
                string currentSessionId = CredentialManager.GetCurrentUser()?.SessionID;
                if (currentSessionId != null)
                {
                    string sessionId = String.Copy(currentSessionId);
                    RequestManager.AttemptLogout(sessionId);
                }

                CredentialManager.SetCurrentUser(null, true);
                ContainingWindow?.ReloadLoginStateUi();
            }
        }

        internal bool ShowLoginDialog()
        {
            LoginPrompt prompt = new LoginPrompt();
            prompt.Owner = ContainingWindow;

            bool? success = prompt.ShowDialog();
            return success.HasValue && success.Value;
        }

        private void ShowSignUpDialog()
        {
            SignUpPrompt prompt = new SignUpPrompt();
            prompt.Owner = ContainingWindow;

            bool? _ = prompt.ShowDialog();
        }

        private void NavigationBackButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        private void NavigationForwardButtonClicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
