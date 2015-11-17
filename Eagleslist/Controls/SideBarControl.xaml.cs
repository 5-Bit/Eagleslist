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
        private MainWindow Container;
        internal MainWindow ContainingWindow
        {
            get
            {
                return Container;
            }
            set
            {
                Container = value;

                List<Button> buttons = new List<Button> {
                    searchButton, composeButton, listingsButton, coursesButton
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


            SelectControlButton(searchButton);
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
    }
}
