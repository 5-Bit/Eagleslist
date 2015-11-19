
namespace Eagleslist
{
    public class ValidatedListing
    {

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once NotAccessedField.Local
        private string SessionID;
        // ReSharper disable once NotAccessedField.Local
        // ReSharper disable once InconsistentNaming
        private Listing Listing;

        public ValidatedListing(string sessionId, Listing listing)
        {
            this.SessionID = sessionId;
            this.Listing = listing;
        }
    }
}
