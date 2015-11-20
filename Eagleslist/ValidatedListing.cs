
namespace Eagleslist
{
    public class ValidatedListing
    {

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once NotAccessedField.Local
        public string SessionID;
        // ReSharper disable once NotAccessedField.Local
        // ReSharper disable once InconsistentNaming
        public Listing Listing;

        public ValidatedListing(string sessionId, Listing listing)
        {
            this.SessionID = sessionId;
            this.Listing = listing;
        }
    }
}
