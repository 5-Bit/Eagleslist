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
        private MainWindow ContainingWindow
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow);
            }
        }

        private SearchButton _searchButton;

        public TopBarControl()
        {
            InitializeComponent();

            NavigationManager.AddAssociation<ProfileViewerControl>(ProfileComboBoxItem);
            NavigationManager.AddAssociation<MessagesViewerControl>(NotificationsComboBoxItem);

            NavigationManager.NavigationStateChangeListener = () =>
            {
                NavBackButton.IsEnabled = NavigationManager.CanGoBack;
                NavForwardButton.IsEnabled = NavigationManager.CanGoForward;
            };
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

            if (button.isSelected)
            {
                NavigationManager.NavigateFromClick(button, null);
            }
            else
            {
                NavigationManager.NavigateBack();
            }
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

        private void AccountDropDownClicked(object sender, RoutedEventArgs e)
        {
            AccountComboBox.IsDropDownOpen = !AccountComboBox.IsDropDownOpen;
        }

        private void AccountComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            switch (AccountComboBox.SelectedIndex)
            {
                case 0:
                    ShowLoginDialog();
                    break;
                case 1:
                    ShowSignUpDialog();
                    break;
                case 2:
                case 3:
                    NavigationManager.NavigateFromClick((ComboBoxItem)comboBox.Items[comboBox.SelectedIndex], null);
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
            NavigationManager.NavigateBack();
        }

        private void NavigationForwardButtonClicked(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateForward();
        }

        private void SearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var grid = ContainingWindow.ContainerGrid;

            foreach (UIElement element in grid.Children)
            {
                if (Grid.GetRow(element) == 1 && Grid.GetColumn(element) == 1)
                {
                    var searchControl = element as SearchControl;

                    if (searchControl != null)
                    {
                        searchControl.SearchBoxUpdatedText((sender as TextBox)?.Text);
                    }
                }
            }
        }
    }
}
