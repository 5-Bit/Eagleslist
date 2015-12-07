using System.Collections.Generic;

namespace Eagleslist
{
    public class GoogleBook
    {
        public class VolumeInfo
        {
            public string title { get; set; }
            public string subtitle { get; set; }
            public List<string> authors { get; set; }
            public string description { get; set; }
            public ImageLinks imageLinks { get; set; }
            public List<Dictionary<string, string>> industryIdentifiers { get; set; }
        }

        public class ImageLinks
        {
            public string smallThumbnail { get; set; }
            public string thumbnail { get; set; }
        }

        public string id { get; set; }
        public VolumeInfo volumeInfo { get; set; }

        public string Title
        {
            get
            {
                return volumeInfo?.title;
            }
        }

        public string Subtitle
        {
            get
            {
                return volumeInfo?.subtitle;
            }
        }

        public List<string> Authors
        {
            get
            {
                return volumeInfo?.authors;
            }
        }

        public string ISBN
        {
            get
            {
                return ISBN13 ?? ISBN10 ?? "";
            }
        }

        public string ISBN10
        {
            get
            {
                if (volumeInfo.industryIdentifiers == null)
                {
                    return null;
                }

                foreach (var industryId in volumeInfo.industryIdentifiers)
                {
                    if (industryId["type"] != null && industryId["type"].Equals("ISBN_10"))
                    {
                        return industryId["identifier"];
                    }
                }

                return null;
            }
        }

        public string ISBN13
        {
            get
            {
                if (volumeInfo.industryIdentifiers == null)
                {
                    return null;
                }

                foreach (var industryId in volumeInfo.industryIdentifiers)
                {
                    if (industryId["type"] != null && industryId["type"].Equals("ISBN_13"))
                    {
                        return industryId["identifier"];
                    }
                }

                return null;
            }
        }
    }
}
