using System.Windows;
using System.Windows.Controls;
using System;
using Eagleslist;
using System.Collections.Generic;

namespace Eagleslist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Listing> listings = new List<Listing>();

        public MainWindow()
        {
            InitializeComponent();

            listingsView.ItemsSource = listings;

            GetUsers();
        }

        private async void GetUsers()
        {
            RequestManager manager = new RequestManager();
            List<Listing> newListings = await manager.GetListings();

            if (newListings != null)
            {
                listings.InsertRange(0, newListings);
            }
        }

        private void tabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl control = sender as TabControl;

            Console.WriteLine(control.SelectedIndex);
        }
    }
}
