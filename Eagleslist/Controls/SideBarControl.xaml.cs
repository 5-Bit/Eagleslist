using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for SideBarControl.xaml
    /// </summary>
    public partial class SideBarControl : UserControl
    {
        private MainWindow ContainingWindow
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow);
            }
        }

        public SideBarControl()
        {
            InitializeComponent();

            NavigationManager.AddAssociation<SearchControl>(SearchButton);
            NavigationManager.AddAssociation<ListingCreationControl>(ComposeButton);
            NavigationManager.AddAssociation<ListingBrowserControl>(ListingsButton);
            NavigationManager.AddAssociation<CoursesBrowserControl>(CoursesButton);
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            ContainingWindow.topBar.SearchButtonClicked(sender as SearchButton);
        }

        private void NavigationButtonClicked(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateFromClick(sender as Button, null);
        }

        private void SearchCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SearchButtonClicked(SearchButton, e);
        }

        private void OtherCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            NavigationButtonClicked(sender, e);
        }
    }
}
