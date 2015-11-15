using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace Eagleslist
{
    class ListingCreation
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<Bitmap> images { get; private set; }
        public string Price { get; set; }
        public string Condition { get; set; }

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
                && IsPriceValid();
        }

        public bool IsPriceValid()
        {
            return IsNewPriceValid(Price);
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
