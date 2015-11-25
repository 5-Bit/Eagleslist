using System;

namespace Eagleslist
{
    public class NavigationContext
    {
        public object DataObject { get; }
        public Type Type { get; }

        public NavigationContext(object dataObject, Type type)
        {
            DataObject = dataObject;
            Type = type;
        }

        public Navigatable Instantiate()
        {
            return Activator.CreateInstance(Type) as Navigatable;
        }
    }
}
