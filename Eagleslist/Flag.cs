
namespace Eagleslist
{
    public class Flag
    {
        public User Raiser { get; private set; }
        public int PostID { get; private set; }
        public string Content { get; private set; }
        public FlagType Type { get; private set; }
    }
}
