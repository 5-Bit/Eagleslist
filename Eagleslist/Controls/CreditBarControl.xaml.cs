using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for CreditBarControl.xaml
    /// </summary>
    public partial class CreditBarControl : UserControl
    {
        public CreditBarControl()
        {
            InitializeComponent();
        }

        private void CreditLinkClicked(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
