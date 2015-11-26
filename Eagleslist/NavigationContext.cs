using System;

namespace Eagleslist
{
    public class NavigationContext : IEquatable<NavigationContext>
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

        public bool Equals(NavigationContext other)
        {
            return this == other
                || (Type.Equals(other.Type)
                && DataObject != null 
                && other.DataObject != null
                && DataObject.Equals(other.DataObject));
        }
    }
}
