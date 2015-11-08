
namespace Eagleslist
{
    public class ValidatedListing
    {
        public string SessionID { get; private set; }
        public Listing Listing { get; private set; }

        public ValidatedListing(string SessionID, Listing Listing)
        {
            this.SessionID = SessionID;
            this.Listing = Listing;
        }
    }
}
