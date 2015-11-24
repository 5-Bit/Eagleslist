using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Eagleslist
{
    public class NavigationManagers<T> : Object
    {
        private static int _navigationIndex = -1;
        private static List<NavigationContext> _navigationStack = new List<NavigationContext>();
        private static Dictionary<ContentControl, Type> _buttonAssociations = new Dictionary<ContentControl, Type>();

        public static Instantiatable Instantiate<Instantiatable>() where Instantiatable : new()
        {
            var theObject = (Instantiatable)Activator.CreateInstance(typeof(Instantiatable));
            return theObject;
        }

        public static void AddAssociation<T, U>(ContentControl button) where T : Navigatable<T, U>
        {
            _buttonAssociations.Add(button, typeof(Instantiatable));
            //_buttonAssociations.Add(button, type);
        }

        public static void DropAssociations()
        {
            _buttonAssociations.Clear();
        }

        public static void NavigateFromClick(ContentControl contentControl)
        {

        }

        public static void NavigateBack()
        {

        }

        public static void NavigateForward()
        {

        }
    }
}


//internal void ContainerDisplayPanelAtIndex(int index)
//{
//    if (index < _primaryPanels.Count)
//    {
//        for (var iterator = 0; iterator < _primaryPanels.Count; iterator++)
//        {
//            _primaryPanels[iterator].Visibility = index == iterator
//                ? Visibility.Visible : Visibility.Collapsed;

//            if (iterator == 0) continue;
//            var button = PrimaryNavigationControls[iterator];

//            button.Background = iterator == index
//                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECEEF3"))
//                : new SolidColorBrush(Color.FromRgb(255, 255, 255));
//        }

//        foreach (var control in _secondaryPanels)
//        {
//            control.Visibility = Visibility.Collapsed;
//        }
//    }
//    else
//    {
//        for (var iterator = 0; iterator < _primaryPanels.Count; iterator++)
//        {
//            if (iterator != 0)
//            {
//                var button = PrimaryNavigationControls[iterator];
//                button.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
//            }

//            _primaryPanels[iterator].Visibility = Visibility.Collapsed;
//        }

//        for (var iterator = 0; iterator < _secondaryPanels.Count; iterator++)
//        {
//            _secondaryPanels[iterator].Visibility = index - _primaryPanels.Count == iterator ?
//                Visibility.Visible : Visibility.Collapsed;
//        }
//    }
//}

//internal void ShowSecondaryPanelAtIndex(int index)
//{
//    if (index == 0)
//    {
//        profileContainer.CurrentProfileUser = CredentialManager.GetCurrentUser();
//    }

//    ContainerDisplayPanelAtIndex(_primaryPanels.Count + index);
//}
