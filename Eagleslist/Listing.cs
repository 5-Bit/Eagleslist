﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eagleslist
{
    public class Listing
    {
        public string Title { get; private set; }
        public string Content { get; private set; }
        public string Handle { get; private set; }
        public string Email { get; private set; }
        public string Bio { get; private set; }
        public string ImageURL { get; private set; }
        public string Price { get; private set; }
        public string Condition { get; private set; }

        public int ListingID { get; private set; }
        public int UserID { get; private set; }

        public Listing(string title, string Content, string Handle,
            string Email, string Bio, string ImageURL, string Price, string Condition,
            int ListingID, int UserID)
        {
            this.Title = title;
            this.Content = Content;
            this.Handle = Handle;
            this.Email = Email;
            this.Bio = Bio;
            this.ImageURL = ImageURL;
            this.Price = Price;
            this.Condition = Condition;
            this.ListingID = ListingID;
            this.UserID = UserID;
        }
    }
}
