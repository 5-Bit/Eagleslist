using System.Windows.Controls;

namespace Eagleslist.Controls
{
    /// <summary>
    /// Interaction logic for ProfileViewerControl.xaml
    /// </summary>
    public partial class ProfileViewerControl : UserControl, Navigatable
    {
        internal User CurrentProfileUser
        {
            set
            {
                ProfileUsername.Text = value.Handle;
                ProfileEmail.Text = value.Email;
                ProfileBio.Text = value.Bio;
            }
        }

        public ProfileViewerControl()
        {
            InitializeComponent();
        }

        public void RenderObject(object obj)
        {

        }
    }
}
