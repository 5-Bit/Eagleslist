using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for ListingCreationControl.xaml
    /// </summary>
    public partial class ListingCreationControl : UserControl
    {
        private ListingCreation Draft = new ListingCreation();

        public ListingCreationControl()
        {
            InitializeComponent();

            NewListingConditionComboBox.ItemsSource = Enum.GetValues(typeof(BookCondition));
        }

        private void NewListingTitleChanged(object sender, RoutedEventArgs e)
        {
            Draft.Title = NewListingTitleBox.Text;
            PostNewListingButton.IsEnabled = Draft.RepresentsValidListing();
        }

        private void NewListingPriceChanged(object sender, RoutedEventArgs e)
        {
            Draft.Price = NewListingPriceBox.Text;
            PostNewListingButton.IsEnabled = Draft.RepresentsValidListing();
        }

        private void NewListingContentChanged(object sender, RoutedEventArgs e)
        {
            Draft.Content = NewListingContentBox.Text;
            PostNewListingButton.IsEnabled = Draft.RepresentsValidListing();
        }

        private void NewListingConditionChanged(object sender, RoutedEventArgs e)
        {
            Draft.Condition = BookConditionMethods.FromInt(NewListingConditionComboBox.SelectedIndex);
            PostNewListingButton.IsEnabled = Draft.RepresentsValidListing();
        }

        private void NewListingISBNChanged(object sender, RoutedEventArgs e)
        {
            Draft.ISBN = NewListingISBNBox.Text;
            PostNewListingButton.IsEnabled = Draft.RepresentsValidListing();
        }

        private void NewListingImagesChanged(object sender, RoutedEventArgs e)
        {

        }

        private static void ChooseImages()
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Eagleslist - Select Images";
            dialog.Multiselect = true;
            dialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";

            bool? x = dialog.ShowDialog();

            if (x.HasValue && x.Value)
            {
                foreach (String file in dialog.FileNames)
                {

                }
            }
        }
    }
}
