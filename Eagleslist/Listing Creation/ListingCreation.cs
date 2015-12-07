using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Eagleslist
{
    public class ListingCreation
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
            Regex pattern = new Regex(@"(\d{10}|\d{13})");
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

                return converted >= Decimal.Zero ? converted : -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
