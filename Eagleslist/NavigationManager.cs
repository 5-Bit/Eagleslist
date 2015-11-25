using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Eagleslist
{
    public class NavigationManager
    {
        private static MainWindow _mainWindow
        {
            get
            {
                return ((MainWindow)Application.Current.MainWindow);
            }
        }

        private static int _navIndexBacking = -1;
        private static int _navigationIndex
        {
            get
            {
                return _navIndexBacking;
            }

            set
            {
                _navIndexBacking = value;

                if (NavigationStateChangeListener != null)
                {
                    NavigationStateChangeListener();
                }
            }
        }

        private static List<NavigationContext> _navigationStack = new List<NavigationContext>();
        private static Dictionary<ContentControl, Type> _buttonAssociations = new Dictionary<ContentControl, Type>();

        public static Action _navigationStateChangeListener;
        public static Action NavigationStateChangeListener
        {
            get
            {
                return _navigationStateChangeListener;
            }

            set
            {
                _navigationStateChangeListener = value;

                if (_navigationStateChangeListener != null)
                {
                    _navigationStateChangeListener();
                }
            }
        }

        public static bool CanGoBack
        {
            get
            {
                return _navigationIndex > 0;
            }
        }

        public static bool CanGoForward
        {
            get
            {
                return _navigationIndex >= 0 && _navigationIndex < _navigationStack.Count - 1;
            }
        }

        public static void AddAssociation<T>(ContentControl button) where T : Navigatable, new()
        {
            _buttonAssociations.Add(button, typeof(T));
        }

        public static void ClearAllAssociations()
        {
            _buttonAssociations.Clear();
        }

        public static void NavigateFromClick(ContentControl contentControl, object obj)
        {
            DropForwardFromCurrentIndex();

            var context = new NavigationContext(obj, _buttonAssociations[contentControl]);

            if (_navigationIndex >= 0 && _navigationIndex < _navigationStack.Count)
            {
                var previousContext = _navigationStack[_navigationIndex];
                
                if (context.Equals(previousContext))
                {
                    return;
                }
            }

            _navigationStack.Add(context);
            _navigationIndex = _navigationStack.Count - 1;

            RenderContext(context);
        }

        private static void DropForwardFromCurrentIndex()
        {
            if (_navigationIndex >= 0 && _navigationIndex < _navigationStack.Count)
            {
                //_navigationStack.RemoveRange(_navigationIndex + 1, _navigationStack.Count - _navigationIndex + 1);
                _navigationStack = _navigationStack.GetRange(0, _navigationIndex + 1);
            } 
        }

        private static void RenderContext(NavigationContext context)
        {
            var nextNavItem = context.Instantiate();
            nextNavItem.RenderObject(context.DataObject);

            _mainWindow.RenderNavigationObject(nextNavItem);
        }

        public static void NavigateBack()
        {
            if (_navigationIndex - 1 >= 0)
            {
                RenderContext(_navigationStack[--_navigationIndex]);
            }
        }

        public static void NavigateForward()
        {
            if (_navigationIndex + 1 < _navigationStack.Count)
            {
                RenderContext(_navigationStack[++_navigationIndex]);
            }
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
