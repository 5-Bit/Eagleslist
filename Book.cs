using System;

namespace Eagleslist
{
    public class Book
    {
        public string ISBN10 { get; private set; }
        public string ISBN13 { get; private set; }
        public string Title { get; private set; }
        public string ImageURL { get; private set; }

        public Book()
        {

        }
    }
}
