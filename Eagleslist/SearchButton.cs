using System.Windows.Controls;
using System.Windows.Media;

namespace Eagleslist
{
    public class SearchButton : Button
    {
        private bool _isSelected = false;
        public bool isSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;
                var color = value ? "#006F41" : "#00885A"; 
                Background = Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            }
        }
    }
}
