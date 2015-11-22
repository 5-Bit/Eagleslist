using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for SideBarControl.xaml
    /// </summary>
    public partial class SideBarControl : UserControl
    {
        private MainWindow _container;
        internal MainWindow ContainingWindow
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;

                List<Button> buttons = new List<Button> {
                    SearchButton, ComposeButton, ListingsButton, CoursesButton
                };

                if (value?.PrimaryNavigationControls.Count < buttons.Count)
                {
                    value.PrimaryNavigationControls.InsertRange(0, buttons);
                }
            }
        }

        public SideBarControl()
        {
            InitializeComponent();
            SelectControlButton(SearchButton);
        }

        private void NavigationButtonClicked(object sender, RoutedEventArgs e)
        {
            SelectControlButton((Button)sender);
        }

        internal void SelectControlButton(Button button)
        {
            int tag = int.Parse(button.Tag.ToString());
            ContainingWindow?.ContainerDisplayPanelAtIndex(tag);
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            ContainingWindow.SearchButtonClicked(sender as SearchButton);
        }
    }
}
