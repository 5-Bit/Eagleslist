using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for MessagesViewerControl.xaml
    /// </summary>
    public partial class MessagesViewerControl : UserControl, Navigatable
    {
        private ObservableCollection<Comment> _comments = new ObservableCollection<Comment>();

        public MessagesViewerControl()
        {
            InitializeComponent();
            ReloadContent();
        }

        private void VisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                ReloadContent();
            }
        }

        private void ReloadContent()
        {

            MessagesListView.ItemsSource = _comments;
        }

        public void RenderObject(object obj)
        {

        }
    }
}
