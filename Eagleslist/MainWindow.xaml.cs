﻿using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System;

namespace Eagleslist
{
    public partial class MainWindow : Window
    {
        ObservableCollection<Listing> listings = new ObservableCollection<Listing>();
        List<Canvas> primaryPanels = new List<Canvas>();

        public MainWindow()
        {
            InitializeComponent();

            primaryPanels.Add(composeContainer);
            primaryPanels.Add(searchContainer);
            primaryPanels.Add(listingsContainer);
            primaryPanels.Add(coursesContainer);

            hideAllContainersExcept(null);
        }

        private void hideAllContainersExcept(Canvas container)
        {
            foreach (Canvas canvas in primaryPanels)
            {
                canvas.Visibility = Visibility.Collapsed;
            }

            if (container != null)
            {
                container.Visibility = Visibility.Visible;
            }
        }

        private void ComposeButtonClicked(object sender, RoutedEventArgs e)
        {
            hideAllContainersExcept(composeContainer);
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            hideAllContainersExcept(searchContainer);
        }

        private void ListingsButtonClicked(object sender, RoutedEventArgs e)
        {
            hideAllContainersExcept(listingsContainer);
            GetNewListings();
        }

        private async void GetFakeSearchListings()
        {
            RequestManager manager = new RequestManager();
            List<Listing> newListings = await manager.GetListings();
            List<Listing> tmp = new List<Listing>();
            tmp.Add(newListings[0]);

            listings = new ObservableCollection<Listing>(tmp);
            listingsView.ItemsSource = listings;

            if (listings.Count > 0)
            {
                listingsView.SelectedIndex = 0;
            }
        }

        private async void GetNewListings()
        {
            RequestManager manager = new RequestManager();
            List<Listing> newListings = await manager.GetListings();

            listings = new ObservableCollection<Listing>(newListings);
            listingsView.ItemsSource = listings;

            if (listings.Count > 0)
            {
                listingsView.SelectedIndex = 0;
            }
        }

        private void ListingsViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView selectedList = sender as ListView;

            if (selectedList.SelectedIndex < listings.Count && selectedList.SelectedIndex >= 0)
            {
                Listing selectedListing = listings[selectedList.SelectedIndex];
                listingTitleLabel.Content = selectedListing.Title;
                listingContentTextBlock.Text = selectedListing.Content;
                listingAskingPrice.Content = selectedListing.Price;
                listingConditionLabel.Content = selectedListing.Condition;
            }
        }

        private void CoursesButtonClicked(object sender, RoutedEventArgs e)
        {
            hideAllContainersExcept(coursesContainer);
        }

        private void MessagesButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        private void ProfileButtonClicked(object sender, RoutedEventArgs e)
        {
            LoginPrompt prompt = new LoginPrompt();
            prompt.Owner = this;

            bool? success = prompt.ShowDialog();

            if (success.HasValue && success.Value)
            {
                profileLabel.Content = "0x7fffffff";

                var bitmap = new BitmapImage(new Uri("pack://application:,,,/images/mick.png"));
                profileImage.Source = bitmap;
            }
        }

        private void SearchSubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            GetFakeSearchListings();
            hideAllContainersExcept(listingsContainer);
        }
    }
}
