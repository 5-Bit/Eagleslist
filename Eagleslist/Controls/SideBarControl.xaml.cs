using System.Windows;
using System.Windows.Controls;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for SideBarControl.xaml
    /// </summary>
    public partial class SideBarControl : UserControl
    {
        public SideBarControl()
        {
            InitializeComponent();

            NavigationManager.AddAssociation<SearchControl>(SearchButton);
            NavigationManager.AddAssociation<ListingCreationControl>(ComposeButton);
            NavigationManager.AddAssociation<ListingBrowserControl>(ListingsButton);
            NavigationManager.AddAssociation<CoursesBrowserControl>(CoursesButton);
        }

        private void NavigationButtonClicked(object sender, RoutedEventArgs e)
        {
            NavigationManager.NavigateFromClick(sender as Button, null);
        }
    }
}
