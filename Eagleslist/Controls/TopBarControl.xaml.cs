using System;
using System.Windows;
using System.Windows.Controls;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for TopBarControl.xaml
    /// </summary>
    public partial class TopBarControl : UserControl
    {
        internal MainWindow ContainingWindow;

        public TopBarControl()
        {
            InitializeComponent();
        }

        internal void SetLoggedInUI()
        {
            accountOverlayButton.Content = ContainingWindow.CurrentUser.Handle;
            ToggleVisibleAccountComboBoxItems();
        }

        internal void SetLoggedOutUI()
        {
            accountOverlayButton.Content = "Account";
            ToggleVisibleAccountComboBoxItems();
        }

        private void ToggleVisibleAccountComboBoxItems()
        {
            string tag = ContainingWindow.userIsLoggedIn ? "LoggedIn" : "LoggedOut";

            foreach (ComboBoxItem item in accountComboBox.Items)
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

        private void accountDropDownClicked(object sender, RoutedEventArgs e)
        {
            accountComboBox.IsDropDownOpen = !accountComboBox.IsDropDownOpen;
        }

        private void accountComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (accountComboBox.SelectedIndex)
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

            accountComboBox.IsDropDownOpen = false;
            accountComboBox.SelectedIndex = -1;
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
                if (ContainingWindow.CurrentUser.SessionID != null)
                {
                    string sessionID = String.Copy(ContainingWindow.CurrentUser.SessionID);
                    RequestManager.AttemptLogout(sessionID);
                }

                ContainingWindow.CurrentUser = null;
            }
        }

        private void ShowLoginDialog()
        {
            LoginPrompt prompt = new LoginPrompt();
            prompt.Owner = ContainingWindow;

            bool? _ = prompt.ShowDialog();
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
