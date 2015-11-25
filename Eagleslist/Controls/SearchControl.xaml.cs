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
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControl : UserControl, Navigatable
    {
        public SearchControl()
        {
            InitializeComponent();
        }

        private void SearchSubmitButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        public void RenderObject(object obj)
        {

        }
    }
}
