﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Eagleslist
{
    class ListingCreation
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ISBN { get; set; }
        public List<Bitmap> images { get; private set; }
        public string Price { get; set; }
        public BookCondition Condition { get; set; }

        public void AddImage(Bitmap image)
        {
            if (images == null)
            {
                images = new List<Bitmap>();
            }

            images.Add(image);
        }

        public Bitmap RemoveImageAtIndex(int i)
        {
            if (images != null && i < images.Count)
            {
                Bitmap image = images[i];
                images.RemoveAt(i);

                return image;
            }

            return null;
        }

        public bool RepresentsValidListing()
        {
            return !string.IsNullOrWhiteSpace(Title) 
                && !string.IsNullOrWhiteSpace(Content)
                && IsPriceValid()
                && IsConditionValid()
                && IsISBNValid();
        }

        public bool IsConditionValid()
        {
            return Condition != 0;
        }

        public bool IsPriceValid()
        {
            return IsNewPriceValid(Price);
        }

        public bool IsISBNValid()
        {
            Regex pattern = new Regex("(ISBN[-]*(1[03])*[ ]*(: ){0,1})*(([0-9Xx][- ]*){13}|([0-9Xx][- ]*){10})");
            return string.IsNullOrWhiteSpace(ISBN) || pattern.IsMatch(ISBN);
        }

        public static bool IsNewPriceValid(string price)
        {
            return ValidatedNewPrice(price) >= Decimal.Zero;
        }

        private static decimal ValidatedNewPrice(string price)
        {
            try
            {
                decimal converted = decimal.Parse(
                    price,
                    NumberStyles.Currency,
                    CultureInfo.CurrentUICulture
                );

                return converted;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}